using CommunityToolkit.Maui.Views;
using StockApp.Models;

namespace StockApp.Views
{
    public partial class EditProductPopup : Popup
    {
        private Product _productToEdit;

        // On passe le produit, ainsi que les listes pour remplir les Pickers
        public EditProductPopup(Product product, List<string> availableOrigins, List<string> availableSuppliers)
        {
            InitializeComponent();
            _productToEdit = product;

            // 1. Configuration des Pickers
            OriginPicker.ItemsSource = availableOrigins;
            SupplierPicker.ItemsSource = availableSuppliers;

            // 2. Pré-remplissage des champs (Mapping)
            NameEntry.Text = product.Name;
            QuantityEntry.Text = product.Quantity.ToString();
            ExpirationDatePicker.Date = product.ExpirationDate;

            // Sélectionner les bonnes valeurs dans les pickers
            OriginPicker.SelectedItem = product.Origin;
            SupplierPicker.SelectedItem = product.Supplier;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                !int.TryParse(QuantityEntry.Text, out int quantity))
            {
                return;
            }

            _productToEdit.Name = NameEntry.Text;
            _productToEdit.Origin = OriginPicker.SelectedItem?.ToString() ?? "Inconnu";
            _productToEdit.Quantity = quantity;
            _productToEdit.Supplier = SupplierPicker.SelectedItem?.ToString() ?? "Inconnu";
            _productToEdit.ExpirationDate = ExpirationDatePicker.Date;

            await CloseAsync(_productToEdit);
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await CloseAsync(null);
        }
    }
}
