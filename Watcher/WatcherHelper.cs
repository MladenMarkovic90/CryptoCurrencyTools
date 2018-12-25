using CryptoCurrency.Lib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CryptoCurrency.Watcher
{
    public static class WatcherHelper
    {
        public static void SetRow(DataGridView dataGrid, CryptoCurrencyInfo currencyInfo, int rowNumber)
        {
            if (currencyInfo != null)
            {
                try
                {
                    dataGrid[0, rowNumber].Value = currencyInfo.Name;
                    dataGrid[1, rowNumber].Value = currencyInfo.Symbol;
                    dataGrid[2, rowNumber].Value = decimal.Parse(currencyInfo.PriceUsd).ToString("00000.000000");
                    dataGrid[3, rowNumber].Value = decimal.Parse(currencyInfo.PriceBtc).ToString("0.00000000");
                    UpdatePercentRow(dataGrid[4, rowNumber], decimal.Parse(currencyInfo.HourlyChange));
                    UpdatePercentRow(dataGrid[5, rowNumber], decimal.Parse(currencyInfo.DailyChange));
                    UpdatePercentRow(dataGrid[6, rowNumber], decimal.Parse(currencyInfo.WeeklyChange));
                }
                catch
                {
                }
            }
        }

        public static void UpdatePercentRow(DataGridViewCell cell, decimal value)
        {
            cell.Value = (value < 0 ? "-" : "+") + Math.Abs(value).ToString("00000.00") + "%";

            if (value < 0)
            {
                cell.Style.ForeColor = Color.Red;
            }
            else
            {
                cell.Style.ForeColor = Color.Green;
            }
        }
    }
}