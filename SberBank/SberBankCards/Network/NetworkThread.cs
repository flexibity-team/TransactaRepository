namespace Parking.SberBank
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Network;
    using Parking.Network;
    using RMLib;
    using RMLib.Log;

    internal enum RequestMode
    {
    Status = 0x00,
    Cancel = 0x01,
    Reconcilation = 0x02
  }

  public partial class SberBank
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

    private int lastCounter = -1;
    private RequestMode lastRequest = RequestMode.Status;

    #endregion

    internal void NetworkThread(object param)
    {
      DateTime startTime = DateTime.Now;
      try
      {
        //wait for request
        if (!networkEvent.WaitOne(networkProtocol.PacketTimeout))
        {
          //if (_logger.CanWrite(LogLevel.Verbose))
          //  _logger.Write(LogLevel.Verbose, "Время ожидания запроса истекло", SberBankLogCategories.Network);

          return;
        }

        //get last received packet
        Packet inputPacket = receivedPacket;
        bool isStatus = false;

        LogLevel level = isStatus ? LogLevel.Verbose : LogLevel.Debug;
        if (logger.CanWrite(level))
          logger.Write(level, String.Format("Получен пакет\r\n{0}", inputPacket.ToLogString()), SberBankLogCategories.NetworkPacket);

        //commit recent
        Packet p = queue.Peek();
        if (p != null)
          if ((p.Type == PacketType.Short) || ((inputPacket.CommandCounter - lastCounter) == 1))
            queue.Dequeue();

        lastCounter = inputPacket.CommandCounter;

        //proceed with packet
        ProcessInputPacket(inputPacket);

        //get outgoing packet
        Packet outputPacket = queue.Peek();
        if (outputPacket == null)
        {
          logger.Write(LogLevel.Error, "Empty queue!!!");
          outputPacket = GetStatusPacket(PacketType.Short, inputPacket.Params, false);
        }

        //send to network
        networkEvent.Reset();
        bool sent = false;
        while (networkThread.Started)
        {
          if (!NetworkForceDisable)
            try
            {
              if (networkProtocol.State != NetworkProtocolState.Opened)
                networkProtocol.Open();

              sent = networkProtocol.SendPacket(outputPacket);
              NetworkState = String.Empty;
              break;
            }
            catch (Exception e)
            {
              NetworkState = new StringReader(e.Message).ReadLine();

              logger.Write(e, "Сеть недоступна", SberBankLogCategories.Network);
            }

          if (networkThread.Started)
            Thread.Sleep(NetworkRetryTime);
        }

        if (sent)
        {
          if (logger.CanWrite(LogLevel.Debug))
            logger.Write(LogLevel.Debug, String.Format("Отправлен пакет {0}", outputPacket.ToLogString()), SberBankLogCategories.NetworkPacket);
        }
        else if (!NetworkForceDisable) //not sent
        {
          NetworkErrors++;

          if (logger.CanWrite(LogLevel.Debug))
            if (networkThread.Started)
              logger.Write(LogLevel.Debug,
                String.Format("Не удалось отправить пакет, состояние сетевого протокола : {0}", NetworkProtocolHelper.GetString((NetworkProtocolState) networkProtocol.State)),
                SberBankLogCategories.NetworkPacket);
        }
      }
      catch (Exception e)
      {
        NetworkErrors++;

        logger.Write(e, "Ошибка при обмене данными", SberBankLogCategories.Network);
      }
    }

    private void ProcessInputPacket(Packet input)
    {
      byte[] data = input.Params;
      RequestMode mode = (RequestMode)data[0];
      switch (mode)
      {
        case RequestMode.Cancel:
          if (lastRequest != RequestMode.Cancel)
            if (!device.CancelTransaction())
              mode = RequestMode.Status;

          break;
        case RequestMode.Reconcilation:
          if ((lastRequest == RequestMode.Status) && (mode == RequestMode.Reconcilation))
            device.BeginReconcilation();

          break;
        case RequestMode.Status:
          EquipmentState s = device.State;
          int a = BitConverter.ToInt32(data, 4);
          if (s == EquipmentState.Idle)
          {
            if (a > 0)
            {
              int r = BitConverter.ToInt16(data, 2);
              if (device.BeginTransaction(r, a))
                TransactionStarted++;
            }
          }
          else if ((s == EquipmentState.CardTaken) && (a == 0))
          {
            if (device.EndTransaction())
              TransactionCompleted++;
          }

          if ((lastRequest == RequestMode.Reconcilation) || (device.Reconcilation != ReconcilationState.Idle))
            if (device.EndReconcilation())
            {
              ReconcilationCount++;
              ReconcilationRecentTime = DateTime.Now;
            }

          break;
      }
      lastRequest = mode;

      if (!Enumerable.Any<Packet>(queue))
        queue.Enqueue(GetStatusPacket(PacketType.Short, data, (mode == RequestMode.Cancel)));
    }

    private Packet GetStatusPacket(PacketType t, byte[] input, bool cancel)
    {
      Packet p = new Packet(t);
      p.ShortCommand = 0x02;
      //p.Address = 0;
      //p.CommandCounter = 0;

      byte[] data = p.Params;
      data[0] = (byte)(device.Ready ? 1 : 0);
      data[1] = (byte)(cancel ? 1 : 0);
      data[2] = (byte)(device.LastTransactionResult);

      EquipmentState s = device.State;
      byte b = 0;
      if ((s == EquipmentState.CardInserted) || (s == EquipmentState.PinEntered) || (s == EquipmentState.AuthCompleted))
        b |= 0x03;

      if (s == EquipmentState.CardTaken)
        b |= 0x07;

      data[3] = (byte)(b | (byte)device.Reconcilation);

      if (input != null)
        Buffer.BlockCopy(input, 4, data, 4, 4);

      return p;
    }

    private void CreateTransactionPackets(AuthResponse response)
    {
      byte[] input = receivedPacket.Params;
      Packet p = GetStatusPacket(PacketType.Long, input, false);
      p.ShortCommand = 0x03;
      p.SeriesCount = 1;
      p.SeriesIndex = 1;

      byte[] data = response.Source;
      int l = Math.Min(Packet.MaxDataLength, data.Length);
      Buffer.BlockCopy(data, 0, p.Data, 0, l);
      p.DataLength = (byte)l;
      //DumpPacket(p);
      queue.Enqueue(p);

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
      queue.Enqueue(p);
    }

    private void DumpPacket(Packet p)
    {
      logger.Write(LogLevel.Warning, p.ToByteStream().Array.GetString());
    }
  }
}