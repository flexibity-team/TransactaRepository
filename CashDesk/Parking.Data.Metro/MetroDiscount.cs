using System;
using System.Text;

namespace Parking.Data.Metro
{
  /// <summary>
  /// Состояние скидки метро
  /// </summary>
  public enum MetroDiscountState : byte
  {
    /// <summary>
    /// Только въезд
    /// </summary>
    Entered = 0,

    /// <summary>
    /// Предоставлена
    /// </summary>
    Discounted = 1,

    /// <summary>
    /// Оплачена
    /// </summary>
    Paid = 2
  }

  /// <summary>
  /// Скидка метро
  /// </summary>
  public class MetroDiscount
  {
    private bool enabled;
    private DateTime time;
    private bool isTimeValid;

      #region [ properties ]

    public bool Enabled
    {
      get { return enabled; }
    }

    public DateTime Time
    {
      get { return time; }
      set { time = value; }
    }

    public long DeviceId { get; private set; }

      public MetroDiscountState State { get; set; }

      public int ParkingId { get; private set; }

      public bool CanUseDiscount
    {
      get
      {
        return (enabled && isTimeValid && (State == MetroDiscountState.Discounted));
      }
    }

    #endregion

    public MetroDiscount()
    {
      enabled = false;
      time = DataContract.DefaultDateTime;
      isTimeValid = false;
      DeviceId = DataContract.DefaultID;
      State = MetroDiscountState.Entered;
      ParkingId = (int)DataContract.DefaultID;
    }

    public MetroDiscount(int parkingId, int deviceId,  bool enabled)
    {
      this.enabled = enabled;
      time = DataContract.DefaultDateTime;
      isTimeValid = false;
      DeviceId = deviceId;
      State = MetroDiscountState.Entered;
      ParkingId = parkingId;
    }

    public void PackData(byte[] data)
    {
      data[0] = (byte)(enabled ? 1 : 0);
      Utils.PackDateTime(data, 1, time);
      data[9] = (byte)(DeviceId & 0xFF);
      data[10] = (byte)((DeviceId >> 8) & 0xFF);
      data[11] = (byte)State;
      data[12] = (byte)(ParkingId & 0xFF);
      data[13] = (byte)((ParkingId >> 8) & 0xFF);
    }

    public void UnpackData(byte[] data)
    {
      enabled = (data[0] != 0);
      time = Utils.UnpackDateTime(data, 1);
      isTimeValid = ((time != DataContract.DefaultDateTime) && (time < DateTime.Now));
      DeviceId = data[9];
      DeviceId += data[10] << 8;
      State = (MetroDiscountState)data[11];
      ParkingId = data[12];
      ParkingId += data[13] << 8;
    }

    public override string ToString()
    {
      return State.GetString();
    }

    public string ToLogString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0} = {1}\r\n", "Время", DataFormatter.FormatDateTime(time));
      sb.AppendFormat("{0} = {1}\r\n", "Льгота", enabled.GetString());
      sb.AppendFormat("{0} = {1}\r\n", "Состояние", State.GetString());
      sb.AppendFormat("{0} = {1}\r\n", "ID устройства", DeviceId);
      sb.AppendFormat("{0} = {1}", "ID парковки", ParkingId);

      return sb.ToString();
    }
  }
}