using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoCurrency.Lib
{
    public partial class FullDownloader : Form
    {
        public CryptoCurrencyData Result = new CryptoCurrencyData();

        private HttpClient client = new HttpClient();
        private bool isDownloaded = false;
        private bool isFinished = false;
        private int progress = 0;

        public FullDownloader()
        {
            this.InitializeComponent();
            this.backgroundWorker1.DoWork += this.BackgroundWorker1_DoWork;
        }

        public CryptoCurrencyData Download()
        {
            this.Result.Date = DateTime.Now;
            this.Result.InfoData = null;
            this.isDownloaded = false;
            this.isFinished = false;

            this.backgroundWorker1.RunWorkerAsync();

            while (!this.isFinished)
            {
                Thread.Sleep(100);
                this.progressBar1.Value = (++progress) % this.progressBar1.Maximum;
            }

            this.progressBar1.Value = this.progressBar1.Maximum;

            if (!this.isDownloaded)
            {
                MessageBox.Show("Failed to download new data.");
            }

            return this.Result;
        }

        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.RunAsync().GetAwaiter().GetResult();
        }

        private async Task RunAsync()
        {
            try
            {
                Uri uri = new Uri("https://api.coinmarketcap.com/v1/ticker/?limit=0");

                client.BaseAddress = uri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                string dataPath = myDocuments + @"\CryptoCurrencyData";
                string filePath = string.Format(dataPath + @"\{0}.txt", DateTime.Today.ToString("yyyyMMdd"));

                this.Result.InfoData = await GetAsync(uri.PathAndQuery);

                Directory.CreateDirectory(dataPath);

                File.WriteAllText(filePath, JsonConvert.SerializeObject(this.Result.InfoData, Formatting.Indented));

                this.isDownloaded = true;
            }
            catch
            {
            }
            finally
            {
                this.isFinished = true;
            }
        }

        private async Task<List<CryptoCurrencyInfo>> GetAsync(string path)
        {
            List<CryptoCurrencyInfo> data = null;
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsAsync<List<CryptoCurrencyInfo>>();
            }

            return data;
        }

        private void FullDownloader_Shown(object sender, EventArgs e)
        {
            this.Download();
            this.Close();
        }
    }
}