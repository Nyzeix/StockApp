using SQLite;

namespace StockApp.Models
{
    [Table("Movements")]
    public class ProductMovement
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }

        // Type of movement: "Entry" for stock entry, "Exit" for stock exit, "Up" for stock increase, "Down" for stock decrease
        // Use MovementEnum constants for better code readability
        public string Type { get; set; } = string.Empty; // "Entry", "Exit", "Up", "Down"

    }
}
