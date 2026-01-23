using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using StockApp.Models;
using StockApp.ViewModels;
using StockApp.Views.Popups; 
namespace StockApp.Views
{
    public partial class StockPage : ContentPage
    {
        private StockViewModel ViewModel => BindingContext as StockViewModel;
        internal SupplierViewModel SuppliersVM { get; }
        private ObservableCollection<Product> allProducts;

        // Commande pour l'appui long
        public ICommand SimplePressEditCommand { get; private set; }
        public ICommand LongPressDeleteCommand { get; private set; }

        public StockPage()
        {
            InitializeComponent();
            BindingContext = new StockViewModel();
            SuppliersVM = new SupplierViewModel();

            SimplePressEditCommand = new Command<Product>(async (product) => await OnEditProduct(product));
            // Initialisation de la commande de suppression (Appui Long)
            LongPressDeleteCommand = new Command<Product>(async (product) => await OnLongPressDelete(product));

            if (ViewModel?.StockItems != null)
                allProducts = new ObservableCollection<Product>(ViewModel.StockItems);
            else
                allProducts = new ObservableCollection<Product>();

            setupPickers();
        }

        private void setupPickers()
        {
            List<String> tmpOriginPickerData = [.. allProducts.Select(p => p.Origin).Distinct().OrderBy(o => o)];
            tmpOriginPickerData.Insert(0, "Tous");
            OriginFilterPicker.ItemsSource = tmpOriginPickerData;

            QuantityFilterPicker.ItemsSource = new List<string> { "Tous", "0-10", "11-50", "51-100", "101+" };

            List<String> tmpSuppliersPickerData = [.. SuppliersVM.Suppliers.Select(s => s.Name).Distinct().OrderBy(o => o)];
            tmpSuppliersPickerData.Insert(0, "Tous");
            SupplierFilterPicker.ItemsSource = tmpSuppliersPickerData;

            OriginFilterPicker.SelectedIndex = 0;
            QuantityFilterPicker.SelectedIndex = 0;
            SupplierFilterPicker.SelectedIndex = 0;
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            var suppliersList = SuppliersVM.Suppliers.Select(s => s.Name).Distinct().OrderBy(o => o).ToList();

            var popup = new AddProductPopup(suppliersList);
            var result = await this.ShowPopupAsync(popup);

            if (result is Product newProduct)
            {
                ViewModel?.StockItems.Add(newProduct);
                allProducts.Add(newProduct);
                setupPickers();

                await DisplayAlert("Succès", $"Produit {newProduct.Name} ajouté !", "OK");
            }
        }

        private async Task OnEditProduct(Product selectedProduct)
        {
            if (selectedProduct == null) return;

            var originsList = allProducts.Select(p => p.Origin).Distinct().OrderBy(o => o).ToList();
            var suppliersList = SuppliersVM.Suppliers.Select(s => s.Name).Distinct().OrderBy(n => n).ToList();

            // Affiche la popup
            var popup = new EditProductPopup(selectedProduct, originsList, suppliersList);

            // 3. Attendre le résultat
            var result = await this.ShowPopupAsync(popup);

            if (result is Product modifiedProduct)
            {
                // TODO : Logique métier ici (Sauvegarde DB)
                // L'UI est déjà à jour car c'est le même objet en mémoire
            }
        }

        // Gestion de la supression avec appui long
        private async Task OnLongPressDelete(Product product)
        {
            if (product == null) return;
            bool confirm = await DisplayAlert("Suppression",
                                              $"Voulez-vous supprimer définitivement '{product.Name}' ?",
                                              "Oui, supprimer", "Annuler");
            if (confirm)
            {
                ViewModel?.StockItems.Remove(product);
                allProducts.Remove(product);
                setupPickers();
            }
        }

        // Filtres
        private void FilterProducts()
        {
            string searchText = ProductSearchBar.Text?.ToLower() ?? string.Empty;
            string selectedOrigin = OriginFilterPicker.SelectedItem as string;
            string selectedQuantity = QuantityFilterPicker.SelectedItem as string;
            string selectedSupplier = SupplierFilterPicker.SelectedItem as string;

            var filtered = allProducts.Where(p =>
                (string.IsNullOrWhiteSpace(searchText) || (p.Name?.ToLower().Contains(searchText) ?? false)) &&
                (selectedOrigin == "Tous" || selectedOrigin == null || p.Origin == selectedOrigin) &&
                (selectedSupplier == "Tous" || selectedSupplier == null || p.Supplier == selectedSupplier) &&
                CheckQuantity(p.Quantity, selectedQuantity)
            ).ToList();

            ViewModel.StockItems.Clear();
            foreach (var item in filtered)
                ViewModel.StockItems.Add(item);
        }

        // Séparation du filtre quantité pour plus de clarté
        private bool CheckQuantity(int qty, string filter)
        {
            if (filter == "Tous" || string.IsNullOrEmpty(filter)) return true;
            if (filter == "0-10") return qty <= 10;
            if (filter == "11-50") return qty > 10 && qty <= 50;
            if (filter == "51-100") return qty > 50 && qty <= 100;
            if (filter == "101+") return qty > 100;
            return true;
        }

        // Event Handlers
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e) => FilterProducts();
        private void OriginPicker_SelectedIndexChanged(object sender, EventArgs e) => FilterProducts();
        private void QuantityPicker_SelectedIndexChanged(object sender, EventArgs e) => FilterProducts();
        private void SupplierPicker_SelectedIndexChanged(object sender, EventArgs e) => FilterProducts();
    }
}
