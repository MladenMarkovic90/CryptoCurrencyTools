using System;
using System.Collections.Generic;

namespace CryptoCurrency.Lib
{
    public class CryptoCurrencyData
    {
        public DateTime Date { get; set; }
        public List<CryptoCurrencyInfo> InfoData { get; set; } = new List<CryptoCurrencyInfo>();
    }
}