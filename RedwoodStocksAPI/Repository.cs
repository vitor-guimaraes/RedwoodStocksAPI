using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace RedwoodStocksAPI
{
    public class Stocks { 
    [JsonProperty("1. open")]
    public string _1open { get; set; }

    [JsonProperty("2. high")]
    public string _2high { get; set; }

    [JsonProperty("3. low")]
    public string _3low { get; set; }

    [JsonProperty("4. close")]
    public string _4close { get; set; }

    [JsonProperty("5. volume")]
    public string _5volume { get; set; }
    }

    public class StockData
    {
        [JsonProperty("Time Series (Daily)")]
        public Dictionary<string, Stocks> TimeSeriesDaily { get; set; }
    }

    public class DeckResponse
    {
        public bool success { get; set; }
        public string deck_id { get; set; }
        public bool shuffled { get; set; }
        public int remaining { get; set; }
    }



    public class StockResponse
    {
        [JsonProperty("1. symbol")]
        public string symbol { get; set; }
        
        [JsonProperty("2. name")]
        public string name { get; set; }

        [JsonProperty("3. price")]
        public float price { get; set; }
        
        [JsonProperty("4. exchange")]
        public string exchange { get; set; }
        
        [JsonProperty("5. exchangeShortName")]
        public string exchangeShortName { get; set; }
        
        [JsonProperty("6. type")]
        public string type { get; set; }
    }
}
