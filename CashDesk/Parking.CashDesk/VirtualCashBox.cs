using System;
using Parking.Data.Devices;
using Parking.Data.Devices.Commands;
using Parking.Network;
using RMLib.Collections;

namespace Parking.CashDesk
{
  public class VirtualCashBox
  {
    private readonly SynchronizedQueue<Packet> responseQueue;
    private readonly ManualCashBox device;
    private long commandCounter;

    #region [ properties ]

    public ManualCashBox Device
    {
      get { return device; }
    }

    public long CommandCounter
    {
      get { return commandCounter; }
    }

    #endregion

    public VirtualCashBox(ManualCashBox cashBox)
    {
      responseQueue = new SynchronizedQueue<Packet>();
      device = cashBox;
      commandCounter = 0;
    }

    public void AppendResponse(Packet response)
    {
      responseQueue.Enqueue(response);
    }

    public Packet GetResponse(Packet request)
    {
      //get response
      Packet response = responseQueue.Dequeue();
      if (response == null)
        response = GetStatusPacket();

      //set counter
      response.CommandCounter = request.CommandCounter;

      //total counter
      commandCounter++;
      if (commandCounter == Int64.MaxValue)
        commandCounter = 0;

      return response;
    }

    private Packet GetStatusPacket()
    {
        var answer = new Packet(PacketType.Short) {ShortCommand = (short) ShortCommands.Status};
        answer.Params[0] = (byte)device.Mode.Value;

      return answer;
    }

    public override string ToString()
    {
      return device.ToString();
    }
  }
}