using RedwoodStocksAPI.Application;
using RedwoodStocksAPI.Domain.Models;
using RedwoodStocksAPI.Domain.Services;
using RedwoodStocksAPI.Extensions;
using System.Net;


namespace RedwoodStocksAPI
{
    internal class Program
    {
        private static readonly string apiKey = "TScUaZy2mkXv9D0L1qSNRlNcHXZVn86M";
        //private static readonly string apiKey = "TScUaZy2mkXZVn86M";//test apikey

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
            //stock list
            var stockSymbols = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "TSLA" };// get pra pegar os simbolos
            //var stockSymbols = new List<string> { "AAPL", "KRAMERICA", "GOOGL", "AMZN", "TSLA" };// get pra pegar os simbolos

            List<StockData> stockPrices = new List<StockData>();

            var path = "https://financialmodelingprep.com/api/v3/";

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

