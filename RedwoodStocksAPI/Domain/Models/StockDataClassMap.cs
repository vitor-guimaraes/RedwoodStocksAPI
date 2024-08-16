using CsvHelper.Configuration;

namespace RedwoodStocksAPI.Domain.Models
{
    public class StockDataClassMap : ClassMap<StockData>
    {
        public StockDataClassMap()
        {
            Map(s => s.Date).Name("Date");
            Map(s => s.Name).Name("ShareName");
            Map(s => s.Price).Name("Value (USD)");

        }
    }
}
