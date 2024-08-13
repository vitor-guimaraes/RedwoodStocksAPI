﻿using System.Net;
using System.Text.Json;


// -------------------------------------------------------------------------

namespace RedwoodStocksAPI
{
    internal class Program
    {
        //private static readonly string apiKey = "TScUaZy2mkXv9D0L1qSNRlNcHXZVn86M";
        private static readonly string apiKey = "TScUaZy2mkXZVn86M";//test apikey
        private static async Task Main(string[] args)
        {
            //stock list
            var stockSymbols = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "TSLA" };// get pra pegar os simbolos
            List<StockData> stockPrices = new List<StockData>();

            //connection to stocks api
            using (var client = new HttpClient())
            {

                // Add authentication header (e.g., API Key)
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://financialmodelingprep.com/api/v3/");

                var path = client.BaseAddress;
                bool error = false;

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
                        List<StockResponse> stocks = JsonSerializer.Deserialize<List<StockResponse>>(json!)!;

                        foreach (var stock in stocks)
                        {
                            Console.WriteLine($"Date: {DateTime.Now}");
                            Console.WriteLine($"ShareName: {stock.name}");
                            Console.WriteLine($"Price: {stock.price}");

                            StockData stockData = new StockData()
                            {
                                Date = DateTime.Now,
                                Name = stock.name,
                                Price = stock.price,
                            };

                            stockPrices.Add(stockData);

                            //writes to txt file
                            string output = $"{DateTime.Now}, {stock.name}, {stock.price}";
                            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            string folderPath = Path.Combine(desktopPath, "Output");

                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                        }

                        Functions.WriteToCSV(stockPrices);
                    }
                    catch (JsonException ex)
                    {
                        error = true;
                        Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                        File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Request error: {ex.Message}");

                        if (ex.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            error = true;

                            Console.WriteLine("Unauthorized access. Please check your API key.");
                            File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                        }
                        else
                        {
                            error = true;

                            Console.WriteLine($"Error retrieving data for {symbol}: {ex.Message}");
                            File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                        }
                    }
                        
                }
                if (error == false)
                {
                    Functions.SendEmail("Success");
                }
                else
                {
                    Functions.SendEmail("Error. Check Logs");
                }

            }


        }



        //}

        //class Program
        //{
        //    // Define the API key for authentication (replace with your actual API key)
        //    private const string ApiKey = "TScUaZy2mkXv9D0L1qSNRlNcHXZVn86M";

        //    static async Task Main(string[] args)
        //    {
        //        // List of stock symbols to fetch prices for
        //        List<string> stockSymbols = new List<string> { "AAPL", "GOOGL", "MSFT", "AMZN", "FB" };
        //        string outputPath = "stock_prices.txt";  // Output file path

        //        using (StreamWriter writer = new StreamWriter(outputPath))
        //        {
        //            foreach (var symbol in stockSymbols)
        //            {
        //                try
        //                {
        //                    // Fetch stock price using the Functions class
        //                    string stockPrice = await Functions.GetStockPriceAsync(symbol, ApiKey);

        //                    // Format the output as "Date, ShareName, Value"
        //                    string output = $"{DateTime.Now}, {symbol}, {stockPrice}";

        //                    // Display and write the output to the file
        //                    Console.WriteLine(output);
        //                    writer.WriteLine(output);
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Log any errors encountered while fetching prices
        //                    Console.WriteLine($"Error retrieving price for {symbol}: {ex.Message}");
        //                }
        //            }
        //        }

        //        // Send email with the output and confirmation
        //        EmailSender.SendDailyReport(outputPath);
        //    }
    }

}

