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

        public ObservableCollection<string> AvailableSuppliers { get; set; } = new();

        SupplierViewModel supplierViewModel = new SupplierViewModel();

        public StockViewModel()
        {
            // Chargement des données d'essais, sans passer par une BDD
            LoadStockItems();
            LoadSuppliersList();

            //ApplyFilters();
        }

        private void LoadStockItems()
        {
            // Exemple de données statiques
            for(int i = 1; i<= 20; i++)
            {
                StockItems.Add(new Product { Name = $"Produit {i}", Quantity = i * 10, CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            }
            StockItems.Add(new Product { Name = "Pommes", Quantity = 50, Origin = "France", Color = "Rouge", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            StockItems.Add(new Product { Name = "Bananes", Quantity = 30, Origin = "Equateur", Color = "Jaune", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            StockItems.Add(new Product { Name = "Oranges", Quantity = 20, Origin = "Espagne", Color = "Orange", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
        }

        // Récupère les données de fournisseurs depuis le VM SupplierViewModel
        public void LoadSuppliersList()
        {
            // On vide la liste
            AvailableSuppliers.Clear();

            // Données de suppliers ViewModel
            var suppliersData = supplierViewModel.GetSuppliers();
                
            foreach (var supplier in suppliersData)
            {
                AvailableSuppliers.Add(supplier.Name);
            }

            // Notifie l'interface que la liste a potentiellement changé (utile si on réassigne l'objet collection complète)
            OnPropertyChanged(nameof(AvailableSuppliers));
        }


        // Appelle la vue si une propriété évolue
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}