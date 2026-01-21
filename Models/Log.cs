using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Models
{
    [Table("logs")]
    public class Log
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string message { get; set; } = "No message";
        public int level { get; set; } = 0;
        public int timestamp { get; set; }
    }
}
