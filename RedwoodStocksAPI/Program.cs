﻿using CsvHelper;
using RedwoodStocksAPI;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;


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
            }

        }


        
    }

}

