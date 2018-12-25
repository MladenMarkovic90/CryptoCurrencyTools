using CryptoCurrency.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CryptoCurrency.ExchangeChecker
{
    public partial class CCExchangeChecker : Form
    {
        private int waitSecondsForUpdate = 30;
        private CryptoCurrencyData cryptoCurrencies = null;
        private List<DisplayData> cryptoCurrencyInfos = new List<DisplayData>();
        private bool refreshing = false;

        public CCExchangeChecker()
        {
            this.InitializeComponent();

            this.grvCurrencyData.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);

            this.grvCurrencyData.Columns.Add("CurrencySymbol", "Symbol");
            this.grvCurrencyData.Columns.Add("CurrencyValue", "Value");

            foreach (DataGridViewColumn item in this.grvCurrencyData.Columns)
            {
                item.SortMode = DataGridViewColumnSortMode.NotSortable;
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            this.grvCurrencyData.Rows.Add("USD");
        }

        private void AddClick(object sender, EventArgs e)
        {
            CurrencyPicker form = new CurrencyPicker(this.cryptoCurrencies);
            form.ShowDialog();
            this.AddCurrency(form.Result);
        }

        private void AddCurrency(string currency)
        {
            if (!string.IsNullOrWhiteSpace(currency) && !this.refreshing)
            {
                CryptoCurrencyInfo result = this.Find(currency);

                if (result == null)
                {
                    MessageBox.Show("Does not exist.");
                }
                else if (this.cryptoCurrencyInfos.Any(x => x.Data.ID == result.ID))
                {
                    MessageBox.Show("Already added.");
                }
                else
                {
                    this.grvCurrencyData.Rows.Add(result.Symbol);

                    this.cryptoCurrencyInfos.Add(new DisplayData(result));
                }
            }
        }

        private void CCExchangeChecker_Shown(object sender, EventArgs e)
        {
            FullDownloader form = new FullDownloader();
            form.ShowDialog();
            this.cryptoCurrencies = form.Result;

            this.cmbCurrency.Items.AddRange(this.cryptoCurrencies.InfoData.OrderBy(x => x.ToString().ToLower()).Select(x => x.ToString()).ToArray());
        }

        private CryptoCurrencyInfo Find(string currency)
        {
            CryptoCurrencyInfo result = null;

            if (!string.IsNullOrWhiteSpace(currency))
            {
                currency = currency.ToLower();

                result = cryptoCurrencies.InfoData.FirstOrDefault(x => x.Name.ToLower() == currency) ??
                         cryptoCurrencies.InfoData.FirstOrDefault(x => x.Symbol.ToLower() == currency) ??
                         cryptoCurrencies.InfoData.FirstOrDefault(x => x.ToString().ToLower() == currency);
            }

            return result;
        }

        private bool CheckIfExists(string currency)
        {
            return this.Find(currency) != null;
        }

        private void RefreshClick(object sender, EventArgs e)
        {
            if (this.CheckIfExists(this.cmbCurrency.Text))
            {
                if (!this.refreshing)
                {
                    this.refreshing = true;
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += this.Worker_DoWork;
                    worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
                    worker.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show(this.cmbCurrency.Text + " does not exist.");
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.refreshing = false;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool updateRow = false;

            if (this.cryptoCurrencies.Date.AddSeconds(this.waitSecondsForUpdate) < DateTime.Now)
            {
                updateRow = true;
                this.cryptoCurrencies = new FullDownloader().Download();
            }

            CryptoCurrencyInfo main = null;

            this.Invoke(new Action(() => main = this.Find(this.cmbCurrency.Text)));

            decimal price = 0;
            decimal value = 0;
            string displayValue = string.Empty;

            if (!decimal.TryParse(this.txtValue.Text, out value))
            {
                MessageBox.Show("Value not valid.");
                return;
            }

            if (updateRow)
            {
                this.Invoke(new Action(() => this.grvCurrencyData.Rows[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold)));
                Thread.Sleep(100);
            }

            if (main != null)
            {
                price = value * decimal.Parse(main.PriceUsd);
                displayValue = price.ToString("0.00000000");
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    this.grvCurrencyData[1, 0].Value = "ERROR";
                    this.grvCurrencyData.Rows[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);
                }));

                return;
            }

            this.Invoke(new Action(() =>
            {
                this.grvCurrencyData[1, 0].Value = displayValue;
                this.grvCurrencyData.Rows[0].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);
            }));

            int rowNumber = 1;

            List<DisplayData> update = new List<DisplayData>(cryptoCurrencyInfos);

            foreach (DisplayData item in update)
            {
                displayValue = string.Empty;
                CryptoCurrencyInfo info = this.cryptoCurrencies.InfoData.FirstOrDefault(x => x.ID == item.ID);
                decimal secondPrice = 0;

                if (updateRow)
                {
                    this.Invoke(new Action(() => this.grvCurrencyData.Rows[rowNumber].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold)));
                    Thread.Sleep(100);
                }

                secondPrice = decimal.Parse(info.PriceUsd);

                if (secondPrice == 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.grvCurrencyData[1, rowNumber].Value = "ERROR";
                        this.grvCurrencyData.Rows[rowNumber].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);
                    }));
                }
                else
                {
                    displayValue = (price / secondPrice).ToString("0.00000000");

                    this.Invoke(new Action(() =>
                    {
                        this.grvCurrencyData[1, rowNumber].Value = displayValue;
                        this.grvCurrencyData.Rows[rowNumber].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);
                    }));
                }

                rowNumber++;
            }
        }

        private void RemoveClick(object sender, EventArgs e)
        {
            if (!this.refreshing)
            {
                try
                {
                    int index = this.grvCurrencyData.SelectedCells[0].RowIndex;

                    string currencySymbol = (string)this.grvCurrencyData[0, index].Value;

                    DisplayData item = this.cryptoCurrencyInfos.SingleOrDefault(x => x.Data.Symbol == currencySymbol);

                    if (item != null)
                    {
                        this.cryptoCurrencyInfos.Remove(item);
                        this.grvCurrencyData.Rows.RemoveAt(index);
                    }
                }
                catch
                {
                }
            }
        }

        private void NumberKeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxHelper.NumberKeyPress(sender as TextBox, e);
        }

        private void NumberChanged(object sender, EventArgs e)
        {
            TextBoxHelper.NumberChanged(sender as TextBox, e);
        }
    }
}