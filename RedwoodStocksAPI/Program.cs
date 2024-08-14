using Nest;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;


// -------------------------------------------------------------------------

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
            //bool error = false;

            var error = await StocksPriceWriteToCSVErrorHandler();


            if (error == false)
            {
                Functions.SendEmail("Success", timestamp);
            }
            else
            {
                Functions.SendEmail("Error. Check Logs", timestamp);
            }

        }

        private static async Task StocksPriceWriteToCSV()
        {
            //stock list
            //var stockSymbols = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "TSLA" };// get pra pegar os simbolos
            var stockSymbols = new List<string> { "AAPL", "KRAMERICA", "GOOGL", "AMZN", "TSLA" };// get pra pegar os simbolos
            List<StockData> stockPrices = new List<StockData>();

            var path = "https://financialmodelingprep.com/api/v3/";

            foreach (var symbol in stockSymbols)
            {
                FmpApi api = new FmpApi(apiKey, path);
                var stockData = await api.GetQuoteAsync(symbol);

                if (stockData is not null)
                {
                    stockPrices.Add(stockData);
                }
                else
                {
                    break;
                }
            }
            stockPrices.WriteToCSV(folderPath, timestamp);

            //Functions.WriteToCSV(stockPrices, timestamp);
        }

        private static async Task<bool> StocksPriceWriteToCSVErrorHandler()
        {
            bool error = false;

            try
            {
                await StocksPriceWriteToCSV();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");

                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized access. Please check your API key.");
                    //File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                    Functions.LogError("Unauthorized access. Please check your API key");

                }
                else if (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine($"Error retrieving data for : {ex.Message}");
                    //File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                    Functions.LogError($"Error retrieving data for : {ex.Message}");
                }
                else
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    //File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                    Functions.LogError($"Error retrieving data: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                //File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                Functions.LogError($"Error retrieving data: {ex.Message}");
            }

            return error;
        }

    }

}

