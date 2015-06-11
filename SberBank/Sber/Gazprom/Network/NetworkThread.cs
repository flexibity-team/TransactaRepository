using System;
using System.IO;
using System.Linq;
using System.Threading;
using Parking.Network;
using RMLib;
using RMLib.Log;

namespace Parking.Gazprom
{
  internal enum RequestMode : int
  {
    Status = 0x00,
    Cancel = 0x01,
    Reconcilation = 0x02
  }

  public partial class Gazprom
  {
    #region [ const ]

    private const int ThreadBreakTime = 100;    //пауза при простое потока
    private const int NetworkRetryTime = 5000;  //пауза при повторном обращении к сети

    #endregion

    #region [ static ]

    internal static bool NetworkForceDisable = false;
    internal static string NetworkState = String.Empty;
    internal static int NetworkErrors = 0;
    internal static int TimedOutPackets = 0;

    internal static int TransactionStarted = 0;
    internal static int TransactionCompleted = 0;
    internal static int TransactionCancelled = 0;
    internal static int AuthSuccessfull = 0;
    internal static int AuthRejected = 0;
    internal static int ReconcilationCount = 0;
    internal static DateTime ReconcilationRecentTime = DateTime.MinValue;

    private int _lastCounter = -1;
    private RequestMode _lastRequest = RequestMode.Status;

    #endregion

    internal void NetworkThread(object param)
    {
      DateTime startTime = DateTime.Now;
      try
      {
        //wait for request
        if (!_networkEvent.WaitOne(_networkProtocol.PacketTimeout))
        {
          //if (_logger.CanWrite(LogLevel.Verbose))
          //  _logger.Write(LogLevel.Verbose, "Время ожидания запроса истекло", GazpromLogCategories.Network);

          return;
        }

        //get last received packet
        Packet inputPacket = _receivedPacket;
        bool isStatus = false;

        LogLevel level = isStatus ? LogLevel.Verbose : LogLevel.Debug;
        if (_logger.CanWrite(level))
          _logger.Write(level, String.Format("Получен пакет\r\n{0}", inputPacket.ToLogString()), GazpromLogCategories.NetworkPacket);

        //commit recent
        Packet p = _queue.Peek();
        if (p != null)
          if ((p.Type == PacketType.Short) || ((inputPacket.CommandCounter - _lastCounter) == 1))
            _queue.Dequeue();

        _lastCounter = inputPacket.CommandCounter;

        //proceed with packet
        ProcessInputPacket(inputPacket);

        //get outgoing packet
        Packet outputPacket = _queue.Peek();
        if (outputPacket == null)
        {
          _logger.Write(LogLevel.Error, "Empty queue!!!");
          outputPacket = GetStatusPacket(PacketType.Short, inputPacket.Params, false);
        }

        //send to network
        _networkEvent.Reset();
        bool sent = false;
        while (_networkThread.Started)
        {
          if (!NetworkForceDisable)
            try
            {
              if (_networkProtocol.State != NetworkProtocolState.Opened)
                _networkProtocol.Open();

              sent = _networkProtocol.SendPacket(outputPacket);
              NetworkState = String.Empty;
              break;
            }
            catch (Exception e)
            {
              NetworkState = new StringReader(e.Message).ReadLine();

              _logger.Write(e, "Сеть недоступна", GazpromLogCategories.Network);
            }

          if (_networkThread.Started)
            Thread.Sleep(NetworkRetryTime);
        }

        if (sent)
        {
          if (_logger.CanWrite(LogLevel.Debug))
            _logger.Write(LogLevel.Debug, String.Format("Отправлен пакет {0}", outputPacket.ToLogString()), GazpromLogCategories.NetworkPacket);
        }
        else if (!NetworkForceDisable) //not sent
        {
          NetworkErrors++;

          if (_logger.CanWrite(LogLevel.Debug))
            if (_networkThread.Started)
              _logger.Write(LogLevel.Debug,
                String.Format("Не удалось отправить пакет, состояние сетевого протокола : {0}", _networkProtocol.State.GetString()),
                GazpromLogCategories.NetworkPacket);
        }
      }
      catch (Exception e)
      {
        NetworkErrors++;

        _logger.Write(e, "Ошибка при обмене данными", GazpromLogCategories.Network);
      }
    }

    private void ProcessInputPacket(Packet input)
    {
      byte[] data = input.Params;
      RequestMode mode = (RequestMode)data[0];
      switch (mode)
      {
        case RequestMode.Cancel:
          if (_lastRequest != RequestMode.Cancel)
            if (_device.CancelTransaction())
            {
              //TransactionCancelled++;
              //if (_device.EndTransaction())
              //  TransactionCompleted++;
            }
            else
              mode = RequestMode.Status;

          break;
        case RequestMode.Reconcilation:
          if ((_lastRequest == RequestMode.Status) && (mode == RequestMode.Reconcilation))
            _device.BeginReconcilation();

          break;
        case RequestMode.Status:
          EquipmentState s = _device.State;
          int a = BitConverter.ToInt32(data, 4);
          if (s == EquipmentState.Idle)
          {
            if (a > 0)
            {
              int r = BitConverter.ToInt16(data, 2);
              if (_device.BeginTransaction(r, a))
                TransactionStarted++;
            }
          }
          else if ((s == EquipmentState.CardTaken) && (a == 0))
          {
            if (_device.EndTransaction())
              TransactionCompleted++;
          }

          if ((_lastRequest == RequestMode.Reconcilation) || (_device.Reconcilation != ReconcilationState.Idle))
            if (_device.EndReconcilation())
            {
              ReconcilationCount++;
              ReconcilationRecentTime = DateTime.Now;
            }

          break;
      }
      _lastRequest = mode;

      if (!_queue.Any())
        _queue.Enqueue(GetStatusPacket(PacketType.Short, data, (mode == RequestMode.Cancel)));
    }

    private Packet GetStatusPacket(PacketType t, byte[] input, bool cancel)
    {
      Packet p = new Packet(t);
      p.ShortCommand = 0x02;
      //p.Address = 0;
      //p.CommandCounter = 0;

      byte[] data = p.Params;
      data[0] = (byte)(_device.Ready ? 1 : 0);
      data[1] = (byte)(cancel ? 1 : 0);
      data[2] = (byte)(_device.LastTransactionResult);

      EquipmentState s = _device.State;
      byte b = 0;
      if ((s == EquipmentState.CardInserted) || (s == EquipmentState.PinEntered) || (s == EquipmentState.AuthCompleted))
        b |= 0x03;

      if (s == EquipmentState.CardTaken)
        b |= 0x07;

      data[3] = (byte)(b | (byte)_device.Reconcilation);

      if (input != null)
        Buffer.BlockCopy(input, 4, data, 4, 4);

      return p;
    }

    private void CreateTransactionPackets(AuthResponse response)
    {
      byte[] input = _receivedPacket.Params;
      Packet p = GetStatusPacket(PacketType.Long, input, false);
      p.ShortCommand = 0x03;
      p.SeriesCount = 1;
      p.SeriesIndex = 1;

      byte[] data = response.Source;
      int l = Math.Min(Packet.MaxDataLength, data.Length);
      Buffer.BlockCopy(data, 0, p.Data, 0, l);
      p.DataLength = (byte)l;
      //DumpPacket(p);
      _queue.Enqueue(p);

      int j = response.Source.Length - l;
      if (j <= 0)
        return;

      p.SeriesCount = 2;
      p = GetStatusPacket(PacketType.Long, input, false);
      p.ShortCommand = 0x03;
      p.SeriesCount = 2;
      p.SeriesIndex = 2;

      Buffer.BlockCopy(data, l, p.Data, 0, j);
      p.DataLength = (byte)j;
      //DumpPacket(p);
      _queue.Enqueue(p);
    }

    private void DumpPacket(Packet p)
    {
      _logger.Write(LogLevel.Warning, p.ToByteStream().Array.GetString());
    }
  }
}