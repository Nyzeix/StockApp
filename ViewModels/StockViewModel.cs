using StockApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace StockApp.ViewModels
{

    class StockViewModel : BaseViewModel, INotifyPropertyChanged
    {
        // Event de notification de changement de propriété
        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<Product> StockItems { get; set; } = new();

        public StockViewModel()
        {
            // Chargement des données d'essais, sans passer par une BDD
            LoadStockItems();

            //ApplyFilters();
        }

        private void LoadStockItems()
        {
            // Exemple de données statiques
            StockItems.Add(new Product { Name = "Pommes", Quantity = 50, BuyingPrice = 0.42M, SellingPrice = 0.85M, Origin = "France", Color = "Rouge", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            StockItems.Add(new Product { Name = "Bananes", Quantity = 30, BuyingPrice = 0.20M, SellingPrice = 0.40M, Origin = "Equateur", Color = "Jaune", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            StockItems.Add(new Product { Name = "Oranges", Quantity = 20, BuyingPrice = 0.50M, SellingPrice = 1.0M, Origin = "Espagne", Color = "Orange", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
        }


        public async Task<string> AddProductAsync(Product product)
        {
            var result = false; // Check if product name already exist
            if (result)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    //TODO: Add product to database
                    LoadStockItems();
                    OnPropertyChanged(nameof(StockItems));
                });
                return "Produit ajouté avec succès.";
            }
            else return "Le produit existe déjà.";
        }

        public async Task<string> DeleteProductAsync(Product product)
        {
            var result = false; // TODO: Delete from database
            if (result)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Reload stock items list and update view
                    LoadStockItems();
                    OnPropertyChanged(nameof(StockItems));
                });
                return "Produit supprimé avec succès.";
            }
            else return "Échec de la suppression du produit.";
        }

        // Appelle la vue si une propriété évolue
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}