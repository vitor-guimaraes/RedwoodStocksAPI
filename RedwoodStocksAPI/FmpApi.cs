using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedwoodStocksAPI
{
    public class FmpApi
    {
        private string _apiKey;
        private string _baseUrl;
        public FmpApi(string apiKey, string baseUrl)
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl;
        }

        public async Task<string> GetAsync(string endpoint)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{_baseUrl}{endpoint}");
                Console.WriteLine(response.Content);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                return json;
            }
        }

        public async Task<StockData> GetQuoteAsync(string symbol)
        {
            var endpoint = $"quote/{symbol}?apikey={_apiKey}";

            var json = await GetAsync(endpoint);
            List<StockResponse> stocks = JsonSerializer.Deserialize<List<StockResponse>>(json!)!;

            StockData stockData = new StockData();
            if (stocks.Count > 0)
            {
                stockData = new StockData()
                {
                    Date = DateTime.Now,
                    Name = stocks[0].name,
                    Price = stocks[0].price,
                };
            }
            else
            {
                stockData = new StockData()
                {
                    Date = DateTime.Now,
                    Name = "Information not found",
                    Price = 0,
                };
            }
            return stockData;

        }
    }
}
