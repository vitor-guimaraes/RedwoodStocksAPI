using CsvHelper.Configuration;

namespace RedwoodStocksAPI
{

    public class StockResponse
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public double changesPercentage { get; set; }
        public double change { get; set; }
        public double dayLow { get; set; }
        public double dayHigh { get; set; }
        public double yearHigh { get; set; }
        public double yearLow { get; set; }
        public long marketCap { get; set; }
        public double priceAvg50 { get; set; }
        public double priceAvg200 { get; set; }
        public string exchange { get; set; }
        public int volume { get; set; }
        public int avgVolume { get; set; }
        public double open { get; set; }
        public double previousClose { get; set; }
        public double eps { get; set; }
        public double pe { get; set; }
        //public DateTimeOffset earningsAnnouncement { get; set; }
        //public long sharesOutstanding { get; set; }
        //public int timestamp { get; set; }
    }

    public class StockData
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

    }

    public class StockDataClassMap: ClassMap<StockData>
    {
        public StockDataClassMap()
        {
            Map(s => s.Date).Name("Date");
            Map(s => s.Name).Name("ShareName");
            Map(s => s.Price).Name("Value (USD)");

        }
    }

    //public class DeckResponse
    //{
    //    public bool success { get; set; }
    //    public string deck_id { get; set; }
    //    public bool shuffled { get; set; }
    //    public int remaining { get; set; }
    //}

    //public class StockData
    //{
    //    [JsonProperty("Time Series (Daily)")]
    //    public Dictionary<string, Stocks> TimeSeriesDaily { get; set; }
    //}


    //public class Stocks { 
    //[JsonProperty("1. open")]
    //public string _1open { get; set; }

    //[JsonProperty("2. high")]
    //public string _2high { get; set; }

    //[JsonProperty("3. low")]
    //public string _3low { get; set; }

    //[JsonProperty("4. close")]
    //public string _4close { get; set; }

    //[JsonProperty("5. volume")]
    //public string _5volume { get; set; }
    //}

    //public class StockResponse
    //{
    //    [JsonProperty("1. symbol")]
    //    public string symbol { get; set; }

    //    [JsonProperty("2. name")]
    //    public string name { get; set; }

    //    [JsonProperty("3. price")]
    //    public float price { get; set; }

    //    [JsonProperty("4. exchange")]
    //    public string exchange { get; set; }

    //    [JsonProperty("5. exchangeShortName")]
    //    public string exchangeShortName { get; set; }

    //    [JsonProperty("6. type")]
    //    public string type { get; set; }
    //}
}
