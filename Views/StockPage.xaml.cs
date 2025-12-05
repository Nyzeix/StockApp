using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using StockApp.Models;
using StockApp.ViewModels;

namespace StockApp.Views
{
    public partial class StockPage : ContentPage
    {
        private StockViewModel ViewModel => BindingContext as StockViewModel;

        // Read-Only
        internal SupplierViewModel SuppliersVM { get; }

        // Liste complète pour filtrage
        private ObservableCollection<Product> allProducts;

        public StockPage()
        {
            InitializeComponent();
            BindingContext = new StockViewModel();
            SuppliersVM = new SupplierViewModel();

            // Définit une date par défaut
            ExpirationDatePicker.Date = DateTime.Today;

            // Sauvegarde tous les produits pour la recherche
            if (ViewModel?.StockItems != null)
                allProducts = new ObservableCollection<Product>(ViewModel.StockItems);
            else
                allProducts = new ObservableCollection<Product>();

            setupPickers();
            OriginPicker.SelectedIndex = 0;
            QuantityPicker.SelectedIndex = 0;
        }

        private void setupPickers()
        {
            // Remplir le Picker Origine
            List<String> tmpOriginPickerData = [.. allProducts.Select(p => p.Origin).Distinct().OrderBy(o => o)];
            tmpOriginPickerData.Insert(0, "Tous");
            OriginPicker.ItemsSource = tmpOriginPickerData;

            // Remplir le Picker Quantity
            QuantityPicker.ItemsSource = new List<string> { "Tous", "0-10", "11-50", "51-100", "101+" };
        }

        // SelectionChanged - signature correcte
        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.CurrentSelection?.FirstOrDefault() as Product;
            if (selected is null)
                return;

            await DisplayAlert("Sélection", $"Produit sélectionné : {selected.Name}", "OK");

            if (sender is CollectionView cv)
                cv.SelectedItem = null;
        }

        // Affiche / masque le formulaire
        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            List<String> tmpSuppliersPickerData = [.. SuppliersVM.Suppliers.Select(s => s.Name).Distinct().OrderBy(o => o)];
            SupplierPicker.ItemsSource = tmpSuppliersPickerData;
            AddProductForm.IsVisible = !AddProductForm.IsVisible;
        }

        // Sauvegarde : ajoute le produit à la collection du ViewModel
        private async void OnSaveProductClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry?.Text))
            {
                await DisplayAlert("Erreur", "Le nom du produit est obligatoire.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(QuantityEntry?.Text))
            {
                await DisplayAlert("Erreur", "La quantité ne peut pas être nulle.", "OK");
                return;
            }
            else if (!int.TryParse(QuantityEntry.Text, out _))
            {
                await DisplayAlert("Erreur", "La quantité doit être un nombre entier.", "OK");
                return;
            }

            int quantity = 0;
            if (!string.IsNullOrWhiteSpace(QuantityEntry?.Text))
                int.TryParse(QuantityEntry.Text, out quantity);

            var newProduct = new Product
            {
                Name = NameEntry.Text?.Trim(),
                Quantity = quantity,
                Supplier = SupplierPicker.SelectedItem.ToString(),
                Color = ColorEntry?.Text,
                Origin = OriginEntry?.Text,
                ExpirationDate = ExpirationDatePicker.Date
            };

            ViewModel?.StockItems.Add(newProduct);
            allProducts.Add(newProduct); // ajoute également à la liste complète

            // Mise à jour du Picker Origine
            setupPickers();
            
            // Reset formulaire
            NameEntry.Text = string.Empty;
            QuantityEntry.Text = string.Empty;
            ColorEntry.Text = string.Empty;
            OriginEntry.Text = string.Empty;
            ExpirationDatePicker.Date = DateTime.Today;

            AddProductForm.IsVisible = false;

            await DisplayAlert("Succès", "Produit ajouté avec succès.", "OK");
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            NameEntry.Text = string.Empty;
            QuantityEntry.Text = string.Empty;
            ColorEntry.Text = string.Empty;
            OriginEntry.Text = string.Empty;
            ExpirationDatePicker.Date = DateTime.Today;

            AddProductForm.IsVisible = false;
        }

        private async void OnDeleteProductClicked(object sender, EventArgs e)
        {
            //Remove product clicked
            string message = "";
            if (sender is Button button && button.BindingContext is Product product)
            {
                bool confirm = await DisplayAlert("Confirmer", $"Supprimer {product.Name} ?", "Oui", "Non");
                if (confirm)
                {
                    //message = await ViewModel.DeleteUserAsync(user.Username);
                    await DisplayAlert("Résultat", message, "OK");
                }
            }
            await DisplayAlert("Info", "Fonction de suppression non implémentée.", "OK");
        }

        private async void OnEditProductClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Info", "Fonction d'édition non implémentée.", "OK");
        }

        // Filtrage combiné : SearchBar + Origine
        private void FilterProducts()
        {
            string searchText = ProductSearchBar.Text?.ToLower() ?? string.Empty;
            string selectedOrigin = OriginPicker.SelectedItem as string;
            string selectedQuantity = QuantityPicker.SelectedItem as string;

            var filtered = allProducts.Where(p =>
                (string.IsNullOrWhiteSpace(searchText)
                    || (p.Name?.ToLower().Contains(searchText.ToLower()) ?? false))
                &&
                (selectedOrigin == "Tous"
                    || string.IsNullOrWhiteSpace(selectedOrigin)
                    || p.Origin == selectedOrigin)
                &&
                (selectedQuantity == "Tous"
                    || string.IsNullOrWhiteSpace(selectedQuantity)
                    || (selectedQuantity == "0-10" && (p.Quantity >= 0 && p.Quantity <= 10))
                    || (selectedQuantity == "11-50" && (p.Quantity >= 11 && p.Quantity <= 50))
                    || (selectedQuantity == "51-100" && (p.Quantity >= 51 && p.Quantity <= 100))
                    || (selectedQuantity == "101+" && p.Quantity >= 101)
                    )
            ).ToList();

            ViewModel.StockItems.Clear();
            foreach (var item in filtered)
                ViewModel.StockItems.Add(item);
        }

        // SearchBar
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            FilterProducts();
        }

        // Picker Origine
        private void OriginPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }

        private void QuantityPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }
    }
}
