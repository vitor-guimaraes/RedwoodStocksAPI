using CsvHelper;
using RedwoodStocksAPI.Domain.Models;
using System.Globalization;

namespace RedwoodStocksAPI.Extensions
{
    public static class StockListExtensions
    {
        public static void WriteToCSV(this IEnumerable<StockData> stockPrices, string folderPath, string timestamp)
        {
            Directory.CreateDirectory(folderPath);

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
    }
}
