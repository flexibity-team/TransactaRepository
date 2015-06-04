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
    private bool _enabled;
    private DateTime _time;
    private bool _isTimeValid;
    private long _deviceID;
    private MetroDiscountState _state;
    private int _parkingID;

    #region [ properties ]

    public bool Enabled
    {
      get { return _enabled; }
    }

    public DateTime Time
    {
      get { return _time; }
      set { _time = value; }
    }

    public long DeviceID
    {
      get { return _deviceID; }
    }

    public MetroDiscountState State
    {
      get { return _state; }
      set { _state = value; }
    }

    public int ParkingID
    {
      get { return _parkingID; }
    }

    public bool CanUseDiscount
    {
      get
      {
        return (_enabled && _isTimeValid && (_state == MetroDiscountState.Discounted));
      }
    }

    #endregion

    public MetroDiscount()
    {
      _enabled = false;
      _time = DataContract.DefaultDateTime;
      _isTimeValid = false;
      _deviceID = DataContract.DefaultID;
      _state = MetroDiscountState.Entered;
      _parkingID = (int)DataContract.DefaultID;
    }

    public MetroDiscount(int parkingID, int deviceID,  bool enabled)
    {
      _enabled = enabled;
      _time = DataContract.DefaultDateTime;
      _isTimeValid = false;
      _deviceID = deviceID;
      _state = MetroDiscountState.Entered;
      _parkingID = parkingID;
    }

    public void PackData(byte[] data)
    {
      data[0] = (byte)(_enabled ? 1 : 0);
      Utils.PackDateTime(data, 1, _time);
      data[9] = (byte)(_deviceID & 0xFF);
      data[10] = (byte)((_deviceID >> 8) & 0xFF);
      data[11] = (byte)_state;
      data[12] = (byte)(_parkingID & 0xFF);
      data[13] = (byte)((_parkingID >> 8) & 0xFF);
    }

    public void UnpackData(byte[] data)
    {
      _enabled = (data[0] != 0);
      _time = Utils.UnpackDateTime(data, 1);
      _isTimeValid = ((_time != DataContract.DefaultDateTime) && (_time < DateTime.Now));
      _deviceID = data[9];
      _deviceID += data[10] << 8;
      _state = (MetroDiscountState)data[11];
      _parkingID = data[12];
      _parkingID += data[13] << 8;
    }

    public override string ToString()
    {
      return _state.GetString();
    }

    public string ToLogString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0} = {1}\r\n", "Время", DataFormatter.FormatDateTime(_time));
      sb.AppendFormat("{0} = {1}\r\n", "Льгота", _enabled.GetString());
      sb.AppendFormat("{0} = {1}\r\n", "Состояние", _state.GetString());
      sb.AppendFormat("{0} = {1}\r\n", "ID устройства", _deviceID);
      sb.AppendFormat("{0} = {1}", "ID парковки", _parkingID);

      return sb.ToString();
    }
  }
}