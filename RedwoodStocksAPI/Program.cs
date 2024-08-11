﻿using CsvHelper;
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
        //private static readonly string apiKey = "YOUR_ALPHA_VANTAGE_API_KEY";
        private static readonly string apiKey = "TScUaZy2mkXv9D0L1qSNRlNcHXZVn86M";
        private static async Task Main(string[] args)
        {
            //stock list
            var stockSymbols = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "TSLA" };
            //var stockSymbols = new List<string> { "STA.L", "AFTR-WT", "SCT-DE" };
            //List<Stocks> stockPrices = new List<Stocks>();
            List<StockResponse> stockPrices = new List<StockResponse>();

            //connection to stocks api
            using var client = new HttpClient();

            // Add authentication header (e.g., API Key)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            //client.BaseAddress = new Uri("https://www.alphavantage.co/query?function=TIME_SERIES_DAILY");

            //client.BaseAddress = new Uri("https://www.deckofcardsapi.com/api/");
            client.BaseAddress = new Uri("https://financialmodelingprep.com/api/v3/");
            
            var path = client.BaseAddress;

            foreach (var symbol in stockSymbols)
            {
                //calls the api
                try
                {
                    //var url = $"{path}&symbol={symbol}&apikey={apiKey}";
                    //var url = $"{path}deck/new/shuffle/?deck_count=1";

                    //var url = $"{path}stock/list?apikey={apiKey}";
                    //var url = $"{path}etf/list?apikey={apiKey}";
                    //var url = $"{path}search?query={symbol}&apikey={apiKey}";
                    var url = $"{path}quote/{symbol}?apikey={apiKey}";

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    //TO DO: exception when it gets a 401 response

                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);

                    //deserialize the data
                    //dynamic jsonData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    //Console.WriteLine(jsonData);
                    //var stockData = JsonSerializer.Deserialize<StockData>(json);
                    //List<StockData> stocks = new List<StockData>();

                    List<StockResponse> stocks = JsonSerializer.Deserialize<List<StockResponse>>(json);
                    Console.WriteLine($"Date: {DateTime.Now}");

                    //DeckResponse deck = JsonSerializer.Deserialize<DeckResponse>(json);
                    //Console.WriteLine($"Success: {deck.success}");
                    //Console.WriteLine($"Deck ID: {deck.deck_id}");
                    //Console.WriteLine($"Remaining Cards: {deck.remaining}");
                    //Console.WriteLine($"Shuffled: {deck.shuffled}");

                    foreach (var stock in stocks)
                    {
                        Console.WriteLine($"Date: {DateTime.Now}");
                        Console.WriteLine($"ShareName: {stock.name}");
                        Console.WriteLine($"Price: {stock.price}");


                        //writes to txt file
                        //string output = $"{deck.deck_id}, {deck.success}, {deck.shuffled}";
                        //string output = $"{DateTime.Now}, {symbol}, ";
                        string output = $"{DateTime.Now}, {stock.name}, {stock.price}";

                        stockPrices.Add(stock);
                        //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        ////using (StreamWriter writer = new StreamWriter(desktopPath, append: false))
                        ////{
                        ////    writer.WriteLine("Date, ShareName, Value");
                        ////}
                        //string folderPath = Path.Combine(desktopPath, "Output");


                        //using (StreamWriter file = new StreamWriter("yourfile.txt", true)) // true enables append mode
                        //{
                        //    file.Write("your text");
                        //}

                        //if (!Directory.Exists(folderPath))
                        //{
                        //    Directory.CreateDirectory(folderPath);
                        //}

                        //string filePath = Path.Combine(folderPath, "deck_info.txt");

                        //// If the file exists, generate a new file name with a numeric suffix
                        //int fileIndex = 1;
                        //while (File.Exists(filePath))
                        //{
                        //    filePath = Path.Combine(folderPath, $"deck_info_{fileIndex}.txt");
                        //    fileIndex++;
                        //}

                        //await File.WriteAllTextAsync(filePath, output);

                    }

                    WriteToCsv(stockPrices);
                }
                catch (JsonException ex)
                {
                    //Console.WriteLine($"Error retrieving data for {symbol}: {ex.Message}");
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

        public static void WriteToCsv(List<StockResponse> stockPrices)
        {
            using (var writer = new StreamWriter("stock_prices.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<StockResponse>();
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

