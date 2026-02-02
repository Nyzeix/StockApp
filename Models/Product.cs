using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Models
{
    [Table("Products")]
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Quantity { get; set; } = 0;
        
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string? Supplier { get; set; } = "Default Supplier";
        public DateTime ExpirationDate { get; set; }
        public string? Origin { get; set; } = null;
        public string? Color { get; set; } = null;
        public int CreatedAt { get; set; } = 0;
    }
}
