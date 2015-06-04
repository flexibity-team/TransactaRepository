using System;
using Parking.Data.Devices;
using Parking.Data.Devices.Commands;
using Parking.Network;
using RMLib.Collections;

namespace Parking.CashDesk
{
  public class VirtualCashBox
  {
    private SynchronizedQueue<Packet> _responseQueue;
    private ManualCashBox _device;
    private long _commandCounter;

    #region [ properties ]

    public ManualCashBox Device
    {
      get { return _device; }
    }

    public long CommandCounter
    {
      get { return _commandCounter; }
    }

    #endregion

    public VirtualCashBox(ManualCashBox cashBox)
    {
      _responseQueue = new SynchronizedQueue<Packet>();
      _device = cashBox;
      _commandCounter = 0;
    }

    public void AppendResponse(Packet response)
    {
      _responseQueue.Enqueue(response);
    }

    public Packet GetResponse(Packet request)
    {
      //get response
      Packet response = _responseQueue.Dequeue();
      if (response == null)
        response = GetStatusPacket();

      //set counter
      response.CommandCounter = request.CommandCounter;

      //total counter
      _commandCounter++;
      if (_commandCounter == Int64.MaxValue)
        _commandCounter = 0;

      return response;
    }

    private Packet GetStatusPacket()
    {
      Packet answer = new Packet(PacketType.Short);
      answer.ShortCommand = (short)ShortCommands.Status;
      answer.Params[0] = (byte)_device.Mode.Value;

      return answer;
    }

    public override string ToString()
    {
      return _device.ToString();
    }
  }
}