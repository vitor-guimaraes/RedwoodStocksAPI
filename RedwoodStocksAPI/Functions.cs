using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedwoodStocksAPI
{
    public class Functions
    {

        private static readonly string apiKey = "YOUR_ALPHA_VANTAGE_API_KEY";

        private static async Task<StockData> GetStockData(string symbol)
        {
            var client = new HttpClient();
            var path = client.BaseAddress = new Uri("https://www.alphavantage.co/query?function=TIME_SERIES_DAILY");
            //var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={apiKey}";
            var url = $"{path}&symbol={symbol}&apikey={apiKey}";
            var response = await client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<StockData>(response);
        }

        public static void WriteToCsv(List<Stocks> stockPrices)
        {
            using (var writer = new StreamWriter("stock_prices.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<Stocks>();
                csv.NextRecord();
                foreach (var stockPrice in stockPrices)
                {
                    csv.WriteRecord(stockPrice);
                    csv.NextRecord();
                }
            }

            Console.WriteLine("Stock prices written to stock_prices.csv");
        }
    }
}
