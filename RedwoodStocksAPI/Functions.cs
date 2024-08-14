using CsvHelper;
using Nest;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text.Json;

namespace RedwoodStocksAPI
{
    public class Functions
    {

        private static readonly string apiKey = "YOUR_ALPHA_VANTAGE_API_KEY";

        public static void WriteToCSV(List<StockData> stockPrices, string timestamp)
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
            var csvPath = Path.Combine(folderPath, $"STOCKS_{timestamp}.csv");
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
        public static async Task<string> GetStockPriceAsync(string stockSymbol, string apiKey)
        {
            // Construct the API URL with the stock symbol and API key
            string apiUrl = $"https://financialmodelingprep.com/api/v3/quote/{stockSymbol}?apikey={apiKey}";

            using (var client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the API
                    var response = await client.GetAsync(apiUrl);

                    // Check if the response was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and return the response content as a string
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        // Log and throw an exception for failed requests
                        string error = $"Error fetching stock price: {response.StatusCode}";
                        LogError(error);
                        throw new Exception(error);
                    }
                }
                catch (HttpRequestException e)
                {
                    // Log any HTTP request errors
                    LogError($"Request error: {e.Message}");
                    throw;
                }
            }
        }

        // Logs errors to a text file
        //private static void LogError(string message)
        //{
        //    // Append the error message to a log file with a timestamp
        //    File.AppendAllText("error_log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");
        //}

        public static void SendEmail(string message, string timestamp)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "Output");
            string filePath = Path.Combine(folderPath, $"STOCKS_{timestamp}.csv");

            // Setup mail message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("Papercut@papercut.com");
            mail.To.Add("Papercut@user.com");
            mail.To.Add("guimvitor@gmail.com");
            mail.Subject = "Test Email";
            mail.Body = message;

            //EXCEPTION IF FILE DOESNT EXIST
            //if (File.Exists(filePath))
            //{
            //    var attachment = new System.Net.Mail.Attachment($"{folderPath}/STOCKS.csv");
            //    mail.Attachments.Add(attachment);
            //}

            // Setup SMTP client to use localhost
            SmtpClient smtpClient = new SmtpClient("localhost", 25);
            smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                // Send the email
                if (File.Exists(filePath))
                {
                    var attachment = new System.Net.Mail.Attachment($"{folderPath}/STOCKS_{timestamp}.csv");
                    mail.Attachments.Add(attachment);
                }
                else
                {
                    //send error log
                }
                smtpClient.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }

        public static void LogError(string message)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logsFolderPath = Path.Combine(desktopPath, "logs");

            // Create the "logs" directory if it doesn't exist
            if (!Directory.Exists(logsFolderPath))
            {
                Directory.CreateDirectory(logsFolderPath);
            }

            string logFilePath = Path.Combine(logsFolderPath, "error_log.txt");

            // Write the error message to the log file
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }

    }

}

