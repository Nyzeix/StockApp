using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Models
{
    class Product
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; } = 0;
        
        public decimal Price { get; set; }
        public string? Supplier { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string? Origin { get; set; } = null;
        public string? Color { get; set; } = null;
        public int CreatedAt { get; set; } = 0;
    }
}
