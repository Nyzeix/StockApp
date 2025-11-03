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
            StockItems.Add(new Product { Name = "Pommes", Quantity = 50, Origin = "France", Color = "Rouge", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            StockItems.Add(new Product { Name = "Bananes", Quantity = 30, Origin = "Equateur", Color = "Jaune", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            StockItems.Add(new Product { Name = "Oranges", Quantity = 20, Origin = "Espagne", Color = "Orange", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
        }

        // Appelle la vue si une propriété évolue
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}