using CsvHelper;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RedwoodStocksAPI
{
    public class Functions
    {

        private static readonly string apiKey = "YOUR_ALPHA_VANTAGE_API_KEY";


        public static void WriteToCSV(List<StockData> stockPrices)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "Output");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            //string filePath = Path.Combine(folderPath, "STOCKS.txt");

            //int fileIndex = 1;
            //while (File.Exists(filePath))
            //{
            //    filePath = Path.Combine(folderPath, $"STOCKS{fileIndex}.txt");
            //    fileIndex++;
            //}

            //var csvPath = Path.Combine(Environment.CurrentDirectory, $"STOCKS-{DateTime.Now.ToFileTime()}.csv");
            //var csvPath = Path.Combine(Environment.CurrentDirectory, $"STOCKS.csv");
            var csvPath = Path.Combine(folderPath, $"STOCKS.csv");
            using (var streamWriter = new StreamWriter(csvPath))
            {
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture))
                {
                    csvWriter.Context.RegisterClassMap<StockDataClassMap>();
                    csvWriter.WriteRecords(stockPrices);
                }
            }
        }

        public static async Task<StockData> FetchStockDataAsync(string symbol)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.BaseAddress = new Uri("https://financialmodelingprep.com/api/v3/");

            try
            {
                var url = $"quote/{symbol}?apikey={apiKey}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var stocks = JsonSerializer.Deserialize<List<StockResponse>>(json);
                var stock = stocks?.FirstOrDefault();

                if (stock != null)
                {
                    return new StockData
                    {
                        Date = DateTime.Now,
                        Name = stock.name,
                        Price = stock.price,
                    };
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON for {symbol}: {ex.Message}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error for {symbol}: {ex.Message}");
                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized access. Please check your API key.");
                }
            }

            return null;
        }

        //private static async Task<StockData> GetStockData(string symbol)

        //{
        //    var client = new HttpClient();
        //    var path = client.BaseAddress = new Uri("https://www.alphavantage.co/query?function=TIME_SERIES_DAILY");
        //    //var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={apiKey}";
        //    var url = $"{path}&symbol={symbol}&apikey={apiKey}";
        //    var response = await client.GetStringAsync(url);
        //    return JsonConvert.DeserializeObject<StockData>(response);
        //}

        public static void WriteToCsvOLD(List<StockData> stockPrices)
        {
            using (var writer = new StreamWriter("stock_prices.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<StockData>();
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
