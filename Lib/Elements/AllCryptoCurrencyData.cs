using System;
using System.Collections.Generic;

namespace CryptoCurrency.Lib
{
    public class AllCryptoCurrencyData
    {
        public List<CryptoCurrencyData> AllData { get; private set; } = new List<CryptoCurrencyData>();

        public void AddCryptoCurrencyData(List<CryptoCurrencyInfo> data, DateTime date)
        {
            this.AllData.Add(new CryptoCurrencyData() { InfoData = data, Date = date });
        }
    }
}