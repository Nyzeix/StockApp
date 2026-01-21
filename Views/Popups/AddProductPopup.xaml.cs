using StockApp.Models;
using CommunityToolkit.Maui.Views;

namespace StockApp.Views.Popups
{
    public partial class AddProductPopup : CommunityToolkit.Maui.Views.Popup
    {
        // On passe la liste des fournisseurs au constructeur pour remplir le picker
        public AddProductPopup(List<string> suppliers)
        {
            InitializeComponent();
            SupplierPicker.ItemsSource = suppliers;
            ExpirationDatePicker.Date = DateTime.Today;
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close(null); // Retourne null si annulé
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text)) return;

            int.TryParse(QuantityEntry.Text, out int qty);

            var newProduct = new Product
            {
                Name = NameEntry.Text,
                Quantity = qty,
                Color = ColorEntry.Text,
                Origin = OriginEntry.Text,
                Supplier = SupplierPicker.SelectedItem?.ToString() ?? "Inconnu",
                ExpirationDate = ExpirationDatePicker.Date
            };

            Close(newProduct); // Retourne l'objet créé à la page principale
        }
    }
}
