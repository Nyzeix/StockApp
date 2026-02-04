using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Models
{
    [Table("StockMovementLogs")]
    public class StockMovementLogs
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int timestamp { get; set; }
        public string ProductName { get; set; } = "Unknown Product"; // Nom
        public int QuantityChanged { get; set; } // Evolution de la quantité (valeur qui évolue par rapport à la valeur précédente?)
        public string? message { get; set; } = null; // Message optionnel, utile s'il s'agit d'un changement de fournisseur, origine, ou autre.
    }
}
