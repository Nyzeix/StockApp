using StockApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Maui.ApplicationModel;

namespace StockApp.ViewModels
{

    class StockViewModel : BaseViewModel, INotifyPropertyChanged
    {
        // Event de notification de changement de propriété
        public event PropertyChangedEventHandler PropertyChanged;

        // Source de produits visible (UI)
        public ObservableCollection<Product> StockItems { get; set; } = new();

        // Source de produits complête (De la BDD)
        private readonly List<Product> _allItems = new();

        // Pagination / état
        private bool _isLoadingMore;
        private bool _hasMoreItems = true;
        private int _pageSize = 20;
        private int _currentPage = 0;

        public bool HasMoreItems => _hasMoreItems;

        public StockViewModel()
        {
            SeedAllItems();
            _ = LoadMoreItemsAsync();
        }

        // Récupère données depuis la BDD (Simulé pour l'instant
        private void SeedAllItems()
        {
            for (int i = 0; i < 100; i++)
            {
                _allItems.Add(new Product
                {
                    Name = $"Produit {i + 1}",
                    Quantity = i * 10,
                    BuyingPrice = 1.0M + i,
                    SellingPrice = 1.5M + i,
                    CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds()
                });
            }
            _allItems.Add(new Product { Name = "Pommes", Quantity = 50, BuyingPrice = 0.42M, SellingPrice = 0.85M, Origin = "France", Color = "Rouge", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            _allItems.Add(new Product { Name = "Bananes", Quantity = 30, BuyingPrice = 0.20M, SellingPrice = 0.40M, Origin = "Equateur", Color = "Jaune", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            _allItems.Add(new Product { Name = "Oranges", Quantity = 20, BuyingPrice = 0.50M, SellingPrice = 1.0M, Origin = "Espagne", Color = "Orange", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
        }

        // Charge la page suivante et met à jour StockItems
        public async Task LoadMoreItemsAsync()
        {
            if (_isLoadingMore || !_hasMoreItems)
                return;

            _isLoadingMore = true;
            try
            {
                // Simule latence IO; remplacez par appel service réel
                await Task.Delay(100);

                var page = await GetPageAsync(_currentPage, _pageSize).ConfigureAwait(false);

                if (page.Count == 0)
                {
                    _hasMoreItems = false;
                    return;
                }

                _currentPage++;

                // Mise à jour UI sur le thread principal
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var item in page)
                    {
                        StockItems.Add(item);
                    }
                    OnPropertyChanged(nameof(StockItems));
                });
            }
            finally
            {
                _isLoadingMore = false;
            }
        }

        // Récupère la "page" depuis la source complète.
        // Dans une vraie appli utilisez un service paginé (API / DB).
        private Task<List<Product>> GetPageAsync(int pageIndex, int pageSize)
        {
            var items = _allItems
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(items);
        }

        public async Task<string> AddProductAsync(Product product)
        {
            if (_allItems.Any(p => string.Equals(p.Name, product.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return "Le produit existe déjà.";
            }

            // Ajout à la source complète
            _allItems.Insert(0, product);

            // Si le produit appartient aux pages déjà chargées, l'insérer dans la collection visible.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StockItems.Insert(0, product);
                OnPropertyChanged(nameof(StockItems));
            });

            return "Produit ajouté avec succès.";
        }

        public async Task<string> DeleteProductAsync(Product product)
        {
            var removedFromAll = _allItems.Remove(product);
            var removedFromVisible = false;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                removedFromVisible = StockItems.Remove(product);
                if (removedFromVisible)
                    OnPropertyChanged(nameof(StockItems));
            });

            return (removedFromAll || removedFromVisible) ? "Produit supprimé avec succès." : "Échec de la suppression du produit.";
        }

        // Appelle la vue si une propriété évolue
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}