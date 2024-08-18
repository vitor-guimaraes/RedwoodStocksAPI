using Microsoft.Extensions.Configuration;
using RedwoodStocksAPI.Application;
using RedwoodStocksAPI.Domain.Models;
using RedwoodStocksAPI.Domain.Services;
using RedwoodStocksAPI.Extensions;
using System.Net;


namespace RedwoodStocksAPI
{
    internal class Program
    {
        static readonly string apiKey = Environment.GetEnvironmentVariable("FMPAPIKey");
        //static readonly string apiKey = Environment.GetEnvironmentVariable("testAPIKey");

        public static readonly string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        public static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static string folderPath = Path.Combine(desktopPath, "Output");

        private static async Task Main(string[] args)
        {
            var error = await StocksPriceWriteToCSVErrorHandler();

            if (error == false)
            {
                SendEmailService.SendEmail("Success. Here are your stocks!", timestamp);
            }
            else
            {
                SendEmailService.SendEmail("Error. Please, check error logs.", timestamp);
            }

        }

        private static async Task StockPriceWriteToCSV()
        {
            var stockSymbolsEnv = Environment.GetEnvironmentVariable("StockSymbols");
            //var stockSymbolsEnv = Environment.GetEnvironmentVariable("testStockSymbols");
            var path = Environment.GetEnvironmentVariable("path");

            List<StockData> stockPrices = new List<StockData>();

            if (!string.IsNullOrEmpty(stockSymbolsEnv))
            {
                var stockSymbols = stockSymbolsEnv.Split(',').ToList();

                if (stockSymbols != null)
                {
                    foreach (var symbol in stockSymbols)
                    {
                        FmpApi api = new FmpApi(apiKey, path);
                        var stockData = await api.GetQuoteAsync(symbol);

                        if (stockData.Name == "Information not found")
                        {
                            LogErrorService.LogError($"Could not find information about {symbol} stocks.");
                        }
                        stockPrices.Add(stockData);
                    }
                    stockPrices.WriteToCSV(folderPath, timestamp);
                }
            }
        }

        private static async Task<bool> StocksPriceWriteToCSVErrorHandler()
        {
            bool error = false;

            try
            {
                await StockPriceWriteToCSV();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");

                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    error = true;
                    Console.WriteLine("Unauthorized access. Please check your API key.");
                    LogErrorService.LogError("Unauthorized access. Please check your API key");

                }
                else if (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    error = true;
                    Console.WriteLine($"Error. {ex.Message}");
                    LogErrorService.LogError($"Error retrieving data. {ex.Message}");
                }
                else
                {
                    error = true;
                    Console.WriteLine($"Error: {ex.Message}");
                    LogErrorService.LogError($"Error retrieving data: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                error = true;
                Console.WriteLine($"Error: {ex.Message}");
                LogErrorService.LogError($"Error retrieving data: {ex.Message}");
            }
            return error;
        }

    }

}

