using System;
using Parking.Data.Devices;
using Parking.Data.Devices.Commands;
using Parking.Data.Devices.Parameters;
using Parking.Network;
using RMLib.Collections;

namespace Parking.CashDesk
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Data;
    using RMLib;

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

    private Packet CreateCashTransaction35(Transaction transaction, bool online, ManualCashBox manualCashBox, Card card, Tariff tariff)
    {
        Packet p = new Packet(PacketType.Long);
        p.LongCommand = (byte)LongCommands.CashTransaction;

        byte[] data = p.Data;
        

        //networkAddress
        data[0] = (byte)manualCashBox.NetworkAddress;
        //version
        data[1] = 1;
        //type
        data[2] = (byte)CashMessageType.Transaction;
        //timeStamp
        Utils.PackDateTime(data, 3, transaction.Time, false); 
        //isOnline
        data[9] = (byte)(online ? 1 : 0);
        //lastSessionOpenTime
        Utils.PackDateTime(data, 10, DataContract.DefaultDateTime, false);

        //transactionType
    //    CashTransactionType transactionType = IsOneOf(50) ? RuntimeHelper.GetRandomValue<CashTransactionType>() : CashTransactionType.Payment;
        data[16] = (byte)transaction.Type;
        //paymentMethod
        data[17] = (byte)transaction.PaymentMethod;
        //sessionID
        //sessionID
        data[18] = 0;
        data[19] = 0;
        //amount
        int amount = MoneyConverter.FromMoney(transaction.Payment);
        Int32ToBuffer(data, 20, amount);

        //amount = amountInCurrentZone + debt - discount + fine - prepayment
        //int amount = 0;
        //int amountInCurrentZone = GenerateMoney(50, 5000);
        //int debt = IsOneOf(5) ? 0 : GenerateMoney(50, 5000);
        //amount = amountInCurrentZone + debt;
        //int discount = (IsOneOf(10) ? amount : (IsOneOf(10) ? (int)(amount * Math.Min(MathHelper.Rnd.NextDouble(), 0.5)) : 0)) / MoneyDiscrete;
        //discount *= MoneyDiscrete;
        //amount -= discount;
        //int fine = IsOneOf(10) ? 0 : GenerateMoney(50, 1000);
        //amount += fine;
        //int prepayment = IsOneOf(10) ? 0 : GenerateMoney(200, 2000);
        //amount -= prepayment;
        //amount = Math.Max(0, amount);
        //BufferHelper.Int32ToBuffer(data, 20, MoneyConverter.FromMoney(amount));

        //completed
        data[24] = 0;

        //long cardID = BufferHelper.BufferToInt32(transaction.Data, 8);
        //if (((CardType)transaction.Data[13]).IsLightweight())
        //    cardID = DataExtensions.ConvertLightweightCardID(cardID);

        //Card card = GetService<ICardManager>()[cardID];
        //if (card != null)
        //{
        //    //customerFacilityID
        //    data[25] = (byte)card.Type;
        //    //customerParams
        //    BufferHelper.Int32ToBuffer(data, 26, (int)(card.ID & 0xFFFFFFFF));
        //    //BufferHelper.IntToBuffer(data, 30, (int)(card.ID >> 32));
        //    //customerType
        //    data[46] = (byte)card.CustomerType;
        //    //customerGroupID
        //    data[47] = (byte)(card.CustomerGroupID & 0xFF);
        //}

        //customerFacilityID
        data[25] = (card != null) ? (byte)card.Type : (byte)0;
        //customerParams
        long cardID = transaction.CardID;
        Int32ToBuffer(data, 26, (int)(cardID & 0xFFFFFFFF));
        Int32ToBuffer(data, 30, (int)(cardID >> 32));
        //customerType
        data[46] = (byte)transaction.CustomerType;
        //customerGroupID
        data[47] = (byte)(transaction.CustomerGroupID & 0xFF);

        if (tariff != null)
        {
            var tariff30 = tariff as Tariff30;
            if (tariff30 != null)
            {
                //tariffType
                data[48] = (byte)tariff30.Type;
                //tariffGroup
                data[49] = (byte)tariff30.Class;
            }
            //tariffID
            data[50] = (byte)(tariff.ID & 0xFF);
        }

        ////timeEntry
        //DateTime dte = Utils.UnpackDateTime(transaction.Data, 40);
        //Utils.PackDateTime(data, 51, dte, false);
        ////timePayment
        //DateTime dtp = dte.AddMinutes(MathHelper.Rnd.Next(5, 300));
        //Utils.PackDateTime(data, 57, dtp, false);
        ////timeExit
        //Utils.PackDateTime(data, 63, IsOneOf(2) ? dtp : dtp.AddMinutes(-dtp.Minute).AddHours(1), false);
        ////amountInCurrentZone
        //BufferHelper.Int32ToBuffer(data, 69, MoneyConverter.FromMoney(amountInCurrentZone));
        ////debt
        //BufferHelper.Int32ToBuffer(data, 73, MoneyConverter.FromMoney(debt));
        ////discount
        //BufferHelper.Int32ToBuffer(data, 77, MoneyConverter.FromMoney(discount));
        ////fine
        //BufferHelper.Int32ToBuffer(data, 81, MoneyConverter.FromMoney(fine));
        ////prepayment
        //BufferHelper.Int32ToBuffer(data, 85, MoneyConverter.FromMoney(prepayment));
        ////income
        //int income = ((m == PaymentMethod.Cash) ? (IsOneOf(10) ? (int)(amount * 1.5) : amount) : amount) / MoneyDiscrete;
        //income *= MoneyDiscrete;
        //BufferHelper.Int32ToBuffer(data, 89, MoneyConverter.FromMoney(income));
        ////change
        //int change = income - amount;
        //BufferHelper.Int32ToBuffer(data, 93, MoneyConverter.FromMoney(change));
        ////amountLeftOnError
        //int amountLeftOnError = ((m == PaymentMethod.Cash) ? (IsOneOf(100) ? (int)(change * Math.Min(MathHelper.Rnd.NextDouble(), 0.5)) : 0) : 0) / MoneyDiscrete;
        //amountLeftOnError *= MoneyDiscrete;
        //BufferHelper.Int32ToBuffer(data, 97, MoneyConverter.FromMoney(amountLeftOnError));

        ////receiptState
        //EjectionResult receiptState = (IsOneOf(20) ? RuntimeHelper.GetRandomValue<EjectionResult>() : EjectionResult.OK);
        //data[101] = (byte)receiptState;
        ////bankingCardState
        //EjectionResult bankingCardState = ((m == PaymentMethod.BankingCard) ? (IsOneOf(20) ? RuntimeHelper.GetRandomValue<EjectionResult>() : EjectionResult.OK) : EjectionResult.Unknown);
        //data[102] = (byte)bankingCardState;
        ////ticketState
        //EjectionResult ticketState = (IsOneOf(20) ? RuntimeHelper.GetRandomValue<EjectionResult>() : EjectionResult.OK);
        //data[103] = (byte)ticketState;
        ////errorCode
        //byte errorCode = (byte)(((receiptState == EjectionResult.NotEjected) || (bankingCardState == EjectionResult.NotEjected) || (ticketState == EjectionResult.NotEjected)) ? MathHelper.Rnd.Next(1, 255) : 0);
        //data[104] = errorCode;


        //timeEntry
        Utils.PackDateTime(data, 51, transaction.TimeEntry, false);
        //timePayment
        Utils.PackDateTime(data, 57, transaction.TimePayment, false);
        //timeExit
        Utils.PackDateTime(data, 63, transaction.TimeExit, false);
        //amountInCurrentZone
        Int32ToBuffer(data, 69, amount);
        //debt
        Int32ToBuffer(data, 73, MoneyConverter.FromMoney(transaction.Debt));
        //discount
        Int32ToBuffer(data, 77, MoneyConverter.FromMoney(transaction.Discounts.Sum(d => d.Amount))); 
        //fine
        Int32ToBuffer(data, 81, MoneyConverter.FromMoney(0));
        //prepayment
        Int32ToBuffer(data, 85, MoneyConverter.FromMoney(0));
        //income
        Int32ToBuffer(data, 89, amount);
        //change
        Int32ToBuffer(data, 93, MoneyConverter.FromMoney(0));
        //amountLeftOnError
        Int32ToBuffer(data, 97, MoneyConverter.FromMoney(0));

        //receiptState
        data[101] = (byte)EjectionResult.OK;
        //bankingCardState
//        EjectionResult bankingCardState = ((m == PaymentMethod.BankingCard) ? EjectionResult.OK : EjectionResult.Unknown);
        data[102] = (byte)EjectionResult.Unknown;
        //ticketState
        data[103] = (byte)EjectionResult.OK;
        //errorCode
        data[104] = 0;


        //if (m == PaymentMethod.Cash)
        //{
        //    IEnumerable<int> wb = null;
        //    IEnumerable<int> wc = null;
        //    int j = 105;
        //    if (amount > 0)
        //    {
        //        //banknotes
        //        int banknoteAmount = IsOneOf(10) ? (3 * amount / 5) : amount;
        //        int coinAmount = amount - banknoteAmount;
        //        int r;
        //        wb = MoneyConverter.BanknoteNominals.Weigh(banknoteAmount, out r);
        //        foreach (int n in wb)
        //            data[j++] = (byte)n;

        //        j += 2; //reserved
        //        coinAmount += r;

        //        CashFiscalState f = SafeGetFiscalState(c.ID);
        //        f.UpdateBanknotes(wb);

        //        //coins
        //        if (coinAmount > 0)
        //        {
        //            wc = MoneyConverter.CoinNominals.Weigh(MoneyConverter.FromMoney(coinAmount));
        //            foreach (int n in wc)
        //            {
        //                data[j] = (byte)n;
        //                j += 2;
        //            }

        //            f.UpdateCoins(wc);
        //        }
        //    }

        //    //upperDispenserCassetteOutput
        //    //BufferHelper.Int16ToBuffer(data, j, (wb != null) ? wb.Sum() : 0);
        //    j += 2;
        //    //middleDispenserCassetteOutput
        //    j += 2;
        //    //lowerDispenserCassetteOutput
        //    j += 2;
        //    //nearHopperOutput
        //    //BufferHelper.Int16ToBuffer(data, j, (wc != null) ? wc.Sum() : 0);
        //    j += 2;
        //    //farHopperOutput
        //    //j += 2;

        //    p.DataLength = 138;
        //}
        //else if (m == PaymentMethod.BankingCard)
        //{
        //    //referenceNumber
        //    StringBuilder sb = new StringBuilder(12);
        //    for (int i = 0; i < 3; i++)
        //        sb.AppendFormat("{0:0000}", MathHelper.Rnd.Next(1, 9999));

        //    byte[] ba = CashMessageEncoding.GetBytes(sb.ToString());
        //    Buffer.BlockCopy(ba, 0, data, 105, 12);

        //    //answerCode
        //    ba = CashMessageEncoding.GetBytes("00");
        //    Buffer.BlockCopy(ba, 0, data, 117, 2);

        //    //card info
        //    sb.Length = 0;
        //    for (int i = 0; i < 4; i++)
        //        sb.AppendFormat("{0:0000}", MathHelper.Rnd.Next(1, 9999));
        //    string cn = sb.ToString();
        //    sb.Length = 0;
        //    sb.AppendFormat("{1}{0}{2}{0}{3}{0}{4}{0}", (char)0x1B,
        //      MathHelper.Rnd.Next(1, 9999999), cn, "VISA", String.Empty);
        //    ba = CashMessageEncoding.GetBytes(sb.ToString());
        //    Buffer.BlockCopy(ba, 0, data, 119, ba.Length);

        //    p.DataLength = (byte)(119 + ba.Length);
        //}

        ////update counters for extended status
        //c.Update(m, amount, income, change, amountLeftOnError);
        return p;
    }
    private void Int32ToBuffer(byte[] buffer, int offset, int value)
    {
        buffer[offset] = (byte)(value & 0xFF);
        buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
        buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
        buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
    }
    public override string ToString()
    {
      return device.ToString();
    }
  }
}