using RedwoodStocksAPI;
using System.IO;
using System.Net;

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

namespace ConsoleTests
{
    internal class Program
    {
        private static readonly string apiKey = "YOUR_ALPHA_VANTAGE_API_KEY";
        private static async Task Main(string[] args)
        {
            //stock list
            var stockSymbols = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "TSLA" };
            List<Stocks> stockPrices = new List<Stocks>();

            //connection to stocks api
            using var client = new HttpClient();
            //var path = client.BaseAddress = new Uri("https://www.alphavantage.co/query?function=TIME_SERIES_DAILY");

            //client.BaseAddress = new Uri("https://www.deckofcardsapi.com/api/");
            //HttpResponseMessage response = await client.GetAsync("deck/new/shuffle/?deck_count=1");

            client.BaseAddress = new Uri("https://www.deckofcardsapi.com/api/");
            var path = client.BaseAddress; 


            foreach (var symbol in stockSymbols)
            {
                //calls the api
                try
                {
                    //var url = $"{path}&symbol={symbol}&apikey={apiKey}";
                    var url = $"{path}deck/new/shuffle/?deck_count=1";

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);

                    //dynamic jsonData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    //Console.WriteLine(jsonData);

                    //Stocks stocks = JsonSerializer.Deserialize<Stocks>(json);
                    DeckResponse deck = JsonSerializer.Deserialize<DeckResponse>(json);

                    Console.WriteLine($"Success: {deck.success}");
                    Console.WriteLine($"Deck ID: {deck.deck_id}");
                    Console.WriteLine($"Remaining Cards: {deck.remaining}");
                    Console.WriteLine($"Shuffled: {deck.shuffled}");

                    //writes to txt file
                    string output = $"{deck.deck_id}, {deck.success}, {deck.shuffled}";
                    //string filePath = "deck_info.txt";
                    //await File.WriteAllTextAsync(filePath, output);
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string folderPath = Path.Combine(desktopPath, "Output");

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
                catch (Exception ex) 
                {
                    //Console.WriteLine($"Error retrieving data for {symbol}: {ex.Message}");
                }
                //gets the latest data
                //break;


            }

        }
    }
}