using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using StockApp.Models;
using StockApp.ViewModels;

namespace StockApp.Views
{
    public partial class StockPage : ContentPage
    {
        private StockViewModel ViewModel => BindingContext as StockViewModel;

        // Liste complète pour filtrage
        private ObservableCollection<Product> allProducts;

        public StockPage()
        {
            InitializeComponent();
            BindingContext = new StockViewModel();

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

            int quantity = 0;
            if (!string.IsNullOrWhiteSpace(QuantityEntry?.Text))
                int.TryParse(QuantityEntry.Text, out quantity);

            var newProduct = new Product
            {
                Name = NameEntry.Text?.Trim(),
                Quantity = quantity,
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
