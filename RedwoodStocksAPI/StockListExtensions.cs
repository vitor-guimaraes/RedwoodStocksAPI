using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedwoodStocksAPI
{
    public static class StockListExtensions
    {
        public static void WriteToCSV(this IEnumerable<StockData> stockPrices, string folderPath, string timestamp)
        {
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string folderPath = Path.Combine(desktopPath, "Output");

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
