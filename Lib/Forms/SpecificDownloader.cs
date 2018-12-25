using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoCurrency.Lib
{
    public partial class SpecificDownloader : Form
    {
        public CryptoCurrencyInfo Result = null;

        private HttpClient client = new HttpClient();
        private bool isDownloaded = false;
        private bool isFinished = false;
        private int progress = 0;
        private string currencyFullName = string.Empty;

        public SpecificDownloader(string currencyFullName)
        {
            this.currencyFullName = currencyFullName;

            this.InitializeComponent();

            this.Text = "Downloading data for " + currencyFullName;

            this.backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
        }

        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.RunAsync().GetAwaiter().GetResult();
        }

        private async Task RunAsync()
        {
            try
            {
                Uri uri = new Uri("https://api.coinmarketcap.com/v1/ticker/" + this.currencyFullName);

                client.BaseAddress = uri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                this.Result = await GetAsync(uri.PathAndQuery);

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

        private async Task<CryptoCurrencyInfo> GetAsync(string path)
        {
            CryptoCurrencyInfo data = null;
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsAsync<CryptoCurrencyInfo>();
            }

            return data;
        }

        private void SpecificDownloader_Shown(object sender, EventArgs e)
        {
            this.isFinished = false;

            this.backgroundWorker1.RunWorkerAsync();

            while (!this.isFinished)
            {
                Thread.Sleep(100);
                this.progressBar1.Value = (++progress) % 1000;
            }

            this.progressBar1.Value = 0;

            if (!this.isDownloaded)
            {
                MessageBox.Show("Failed to download data for " + this.currencyFullName + ".");
            }

            this.Close();
        }
    }
}