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
        private static async Task Main(string[] args)
        {
            //stock list
            var stockSymbols = new List<string> { "AAPL", "MSFT", "GOOGL", "AMZN", "TSLA" };// get pra pegar os simbolos
            //var stockSymbols = new List<string> { "AAPL", "KRAMERICA", "GOOGL", "AMZN", "TSLA" };// get pra pegar os simbolos
            List<StockData> stockPrices = new List<StockData>();

            //connection to stocks api
            using (var client = new HttpClient())
            {

                // Add authentication header (e.g., API Key)
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                //client.BaseAddress = new Uri("https://financialmodelingprep.com/api/v3/");
                //client.BaseAddress = new Uri("https://financialmodelingprep.com");

                //var path = client.BaseAddress;
                var path = "https://financialmodelingprep.com/api/v3/";
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

                        if (json.Equals("[]"))
                        {
                            error = true;
                            throw new UnauthorizedAccessException();  
                        }                     
                        //deserialize the data
                        List<StockResponse> stocks = JsonSerializer.Deserialize<List<StockResponse>>(json!)!;

                        //foreach (var stock in stocks)
                        //{
                            //Console.WriteLine($"Date: {DateTime.Now}");
                            //Console.WriteLine($"ShareName: {stock.name}");
                            //Console.WriteLine($"Price: {stock.price}");

                            StockData stockData = new StockData()
                            {
                                Date = DateTime.Now,
                                Name = stocks[0].name,
                                Price = stocks[0].price,
                            };

                            if (stockData is not null)
                            {
                                stockPrices.Add(stockData);
                            }
                            else
                            {
                                break;
                                //throw new Exception(HttpRequestException ex);
                                //stockData = new StockData()
                                //{
                                //    Date = DateTime.Now,
                                //    Name = "could not retrieve information",
                                //    Price = stock.price,
                                //};
                            }


                            //writes to txt file
                            //string output = $"{DateTime.Now}, {stock.name}, {stock.price}";
                            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            //string folderPath = Path.Combine(desktopPath, "Output");

                            //if (!Directory.Exists(folderPath))
                            //{
                            //    Directory.CreateDirectory(folderPath);
                            //}

                        //}

                        Functions.WriteToCSV(stockPrices, timestamp);
                    }
                    catch (JsonException ex)
                    {
                        error = true;
                        Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                        //File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                        Functions.LogError($"Error retrieving data for {symbol}: {ex.Message}");

                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Request error: {ex.Message}");

                        if (ex.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            error = true;

                            Console.WriteLine("Unauthorized access. Please check your API key.");
                            //File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                            Functions.LogError("Unauthorized access. Please check your API key");

                        }
                        else if (ex.StatusCode == HttpStatusCode.Forbidden) 
                        {
                            error = true;

                            Console.WriteLine($"Error retrieving data for {symbol}: {ex.Message}");
                            //File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                            Functions.LogError($"Error retrieving data for {symbol}: {ex.Message}");
                        }
                    }
                        
                }
                if (error == false)
                {
                    Functions.SendEmail("Success", timestamp);
                }
                else
                {
                    Functions.SendEmail("Error. Check Logs", timestamp);
                }

            }


        }

        
    }

}

