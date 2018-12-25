using CryptoCurrency.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CryptoCurrency.Watcher
{
    public partial class CCWatcher : Form
    {
        private int waitSecondsForUpdate = 30;
        private bool workerFinished = false;
        private BackgroundWorker worker = new BackgroundWorker();
        private CryptoCurrencyData cryptoCurrencies = null;
        private List<DisplayData> cryptoCurrencyInfos = new List<DisplayData>();

        public CCWatcher()
        {
            this.InitializeComponent();

            this.grvCurrencyData.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);

            this.grvCurrencyData.Columns.Add("CurrencyName", "Name");
            this.grvCurrencyData.Columns.Add("CurrencySimbol", "Symbol");
            this.grvCurrencyData.Columns.Add("CurrencyPriceUsd", "Price $");
            this.grvCurrencyData.Columns.Add("CurrencyPriceBtc", "Price BTC");
            this.grvCurrencyData.Columns.Add("CurrencyHourlyChange", "Hourly change");
            this.grvCurrencyData.Columns.Add("CurrencyDailyChange", "Daily change");
            this.grvCurrencyData.Columns.Add("CurrencyWeeklyChange", "Weekly change");

            foreach (DataGridViewColumn item in this.grvCurrencyData.Columns)
            {
                item.SortMode = DataGridViewColumnSortMode.NotSortable;
                item.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += this.BackgroundWorker1_DoWork;
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            bool updateRowData = false;
            DateTime now = DateTime.Now;

            while (!worker.CancellationPending)
            {
                int rowNumber = 0;

                List<DisplayData> update = new List<DisplayData>(cryptoCurrencyInfos);

                foreach (DisplayData item in update)
                {
                    if (this.grvCurrencyData.Rows.Count < rowNumber + 1)
                    {
                        this.Invoke(new Action(() => this.grvCurrencyData.Rows.Add(item.Data.Name, item.Data.Symbol)));
                    }

                    CryptoCurrencyInfo info = this.cryptoCurrencies.InfoData.FirstOrDefault(x => x.ID == item.ID);

                    if (updateRowData)
                    {
                        this.grvCurrencyData.Rows[rowNumber].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold);
                        Thread.Sleep(100);
                    }

                    this.Invoke(new Action(() => WatcherHelper.SetRow(this.grvCurrencyData, info, rowNumber)));

                    this.grvCurrencyData.Rows[rowNumber].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);

                    rowNumber++;
                }

                while (this.grvCurrencyData.Rows.Count > rowNumber)
                {
                    this.Invoke(new Action(() => this.grvCurrencyData.Rows.RemoveAt(this.grvCurrencyData.Rows.Count - 1)));
                }

                if (now.AddSeconds(this.waitSecondsForUpdate) < DateTime.Now)
                {
                    updateRowData = true;
                    this.cryptoCurrencies = new FullDownloader().Download();
                    now = DateTime.Now;
                }
                else
                {
                    updateRowData = false;
                    Thread.Sleep(100);
                }
            }

            this.workerFinished = true;
            this.Invoke(new Action(() => this.Close()));
        }

        private void CurrencyWatcher_Shown(object sender, EventArgs e)
        {
            FullDownloader form = new FullDownloader();
            form.ShowDialog();
            this.cryptoCurrencies = form.Result;

            this.listBox1.Items.AddRange(this.cryptoCurrencies.InfoData.OrderBy(x => x.ToString().ToLower()).Select(x => x.ToString()).ToArray());

            this.worker.RunWorkerAsync();
        }

        private void ListBoxDoubleClick(object sender, MouseEventArgs e)
        {
            this.AddCurrency((string)this.listBox1.SelectedItem);
        }

        private void AddCurrency(string currency)
        {
            if (!string.IsNullOrWhiteSpace(currency))
            {
                currency = currency.ToLower();

                CryptoCurrencyInfo result = cryptoCurrencies.InfoData.FirstOrDefault(x => x.Name.ToLower() == currency) ??
                                            cryptoCurrencies.InfoData.FirstOrDefault(x => x.Symbol.ToLower() == currency) ??
                                            cryptoCurrencies.InfoData.FirstOrDefault(x => x.ToString().ToLower() == currency);

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
                    this.cryptoCurrencyInfos.Add(new DisplayData(result));
                }
            }
        }

        private void RemoveClick(object sender, EventArgs e)
        {
            try
            {
                int index = this.grvCurrencyData.SelectedCells[0].RowIndex;

                string currencyName = (string)this.grvCurrencyData[0, index].Value;

                DisplayData item = this.cryptoCurrencyInfos.SingleOrDefault(x => x.Data.Name == currencyName);

                if (item != null)
                {
                    this.cryptoCurrencyInfos.Remove(item);
                }
            }
            catch
            {
            }
        }

        private void LoadCCList(object sender, EventArgs e)
        {
            try
            {
                string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                string filePath = myDocuments + @"\CryptoCurrencyData\CCWatcher\CCList.txt";

                List<DisplayData> list = JsonConvert.DeserializeObject<List<DisplayData>>(File.ReadAllText(filePath));

                foreach (DisplayData item in list)
                {
                    item.Data.HourlyChange = string.Empty;
                    item.Data.DailyChange = string.Empty;
                    item.Data.WeeklyChange = string.Empty;
                    item.Data.PriceBtc = string.Empty;
                    item.Data.PriceUsd = string.Empty;
                }

                if (list != null)
                {
                    this.cryptoCurrencyInfos.Clear();
                    this.cryptoCurrencyInfos.AddRange(list);
                }
            }
            catch
            {
                MessageBox.Show("Error while loading list.");
            }
        }

        private void SaveCCList(object sender, EventArgs e)
        {
            try
            {
                string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                string dataPath = myDocuments + @"\CryptoCurrencyData\CCWatcher";
                string filePath = dataPath + @"\CCList.txt";

                Directory.CreateDirectory(dataPath);

                File.WriteAllText(filePath, JsonConvert.SerializeObject(this.cryptoCurrencyInfos, Formatting.Indented));
            }
            catch
            {
                MessageBox.Show("Error while saving list.");
            }
        }

        private void AddClick(object sender, EventArgs e)
        {
            CurrencyPicker form = new CurrencyPicker(this.cryptoCurrencies);
            form.ShowDialog();
            this.AddCurrency(form.Result);
        }

        private void CCWatcher_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.workerFinished)
            {
                this.worker.CancelAsync();
                this.Hide();
                e.Cancel = true;
            }
        }
    }
}