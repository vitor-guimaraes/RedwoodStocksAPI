using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedwoodStocksAPI.Domain.Models
{
    public class StockData
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
