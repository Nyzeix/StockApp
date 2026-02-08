using CommunityToolkit.Maui.Views;
using StockApp.Models;
using CommunityToolkit.Maui.Core;

namespace StockApp.Views
{
    public partial class EditProductPopup : Popup
    {
        private Product _productToEdit;

        // On passe le produit, ainsi que les listes pour remplir les Pickers
        public EditProductPopup(Product product, List<string> availableSuppliers)
        {
            InitializeComponent();
            _productToEdit = product;

            // 1. Configuration des Pickers
            SupplierPicker.ItemsSource = availableSuppliers;

            // 2. Pré-remplissage des champs (Mapping)
            NameEntry.Text = product.Name;
            OriginEntry.Text = product.Origin;
            QuantityEntry.Text = product.Quantity.ToString();
            ExpirationDatePicker.Date = product.ExpirationDate;

            // Sélectionner les bonnes valeurs dans les pickers
            SupplierPicker.SelectedItem = product.Supplier;

            Opened += OnPopupOpened;

        }

        
        // OnOpened pour connaître la taille de la fenêtre parente
        private void OnPopupOpened(object? sender, PopupOpenedEventArgs e)
        {
            if (Shell.Current?.CurrentPage != null)
            {
                double targetWidth = Shell.Current.CurrentPage.Width * 0.60;
                PopupContainer.WidthRequest = targetWidth;
                var measure = PopupContainer.Measure(targetWidth, double.PositiveInfinity);
                Size = new Size(targetWidth, measure.Height);
            }
        }
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                !int.TryParse(QuantityEntry.Text, out int quantity) ||
                string.IsNullOrWhiteSpace(QuantityEntry.Text))
            {
                return;
            }

            _productToEdit.Name = NameEntry.Text;
            _productToEdit.Quantity = quantity;
            _productToEdit.Origin = OriginEntry.Text;
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
