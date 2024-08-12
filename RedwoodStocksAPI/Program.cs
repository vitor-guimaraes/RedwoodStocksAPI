using CsvHelper;
using RedwoodStocksAPI;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http.Headers;


// -------------------------------------------------------------------------
// if using .NET Framework
// https://docs.microsoft.com/en-us/dotnet/api/system.web.script.serialization.javascriptserializer?view=netframework-4.8
// This requires including the reference to System.Web.Extensions in your project
// -------------------------------------------------------------------------
// if using .Net Core
// https://docs.microsoft.com/en-us/dotnet/api/system.text.json?view=net-5.0
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
// -------------------------------------------------------------------------

namespace RedwoodStocksAPI
{
    internal class Program
    {
        private static readonly string apiKey = "TScUaZy2mkXv9D0L1qSNRlNcHXZVn86M";
        private static async Task Main(string[] args)
        {
            //stock list
            var stockSymbols = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "TSLA" };
            List<StockData> stockPrices = new List<StockData>();

            //connection to stocks api
            using var client = new HttpClient();

            // Add authentication header (e.g., API Key)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            client.BaseAddress = new Uri("https://financialmodelingprep.com/api/v3/");
            
            var path = client.BaseAddress;

            foreach (var symbol in stockSymbols)
            {
                //calls the api
                try
                {
                    var url = $"{path}quote/{symbol}?apikey={apiKey}";

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    //TO DO: exception when it gets a 401 response

                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);

                    //deserialize the data
                    List<StockResponse> stocks = JsonSerializer.Deserialize<List<StockResponse>>(json);
                    Console.WriteLine($"Date: {DateTime.Now}");

                    foreach (var stock in stocks)
                    {
                        Console.WriteLine($"Date: {DateTime.Now}");
                        Console.WriteLine($"ShareName: {stock.name}");
                        Console.WriteLine($"Price: {stock.price}");

                        StockData stockData = new StockData() { 
                            Date = DateTime.Now,
                            Name = stock.name,
                            Price = stock.price,
                            };

                        //writes to txt file
                        string output = $"{DateTime.Now}, {stock.name}, {stock.price}";

                        stockPrices.Add(stockData);
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        //using (StreamWriter writer = new StreamWriter(desktopPath, append: false))
                        //{
                        //    writer.WriteLine("Date, ShareName, Value");
                        //}
                        string folderPath = Path.Combine(desktopPath, "Output");


                        using (StreamWriter file = new StreamWriter("yourfile.txt", true)) // true enables append mode
                        {
                            file.Write("your text");
                        }

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string filePath = Path.Combine(folderPath, "deck_info.txt");

                        // If the file exists, generate a new file name with a numeric suffix
                        int fileIndex = 1;
                        while (File.Exists(filePath))
                        {
                            filePath = Path.Combine(folderPath, $"deck_info_{fileIndex}.txt");
                            fileIndex++;
                        }

                        await File.WriteAllTextAsync(filePath, output);

                    }

                    WriteToCsv(stockPrices);
                    Write(stockPrices);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");

                    if (ex.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("Unauthorized access. Please check your API key.");
                    }
                    else
                    {
                        Console.WriteLine($"Error retrieving data for {symbol}: {ex.Message}");
                    }
                }
                //gets the latest data
                //break;
            }

        }

        public static void WriteToCsv(List<StockData> stockPrices)
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

        public static void Write(List<StockData> stockPrices)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "Output");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
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
    }

}

