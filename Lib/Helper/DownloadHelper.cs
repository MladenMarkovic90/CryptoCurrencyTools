using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CryptoCurrency.Lib
{
    public static class DownloadHelper
    {
        private static async Task<CryptoCurrencyInfo> RunAsync(string uriPath)
        {
            List<CryptoCurrencyInfo> result = null;

            try
            {
                HttpClient client = new HttpClient();

                Uri uri = new Uri(uriPath);

                client.BaseAddress = uri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                result = await GetAsync(client, uri.PathAndQuery);
            }
            catch
            {
            }

            return result[0];
        }

        private static async Task<List<CryptoCurrencyInfo>> GetAsync(HttpClient client, string path)
        {
            List<CryptoCurrencyInfo> data = null;
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsAsync<List<CryptoCurrencyInfo>>();
            }

            return data;
        }

        public static async Task<CryptoCurrencyInfo> DownloadSpecificCurrencyInfo(string currencyId)
        {
            return await RunAsync("https://api.coinmarketcap.com/v1/ticker/" + currencyId);
        }
    }
}