using StockApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.ViewModels
{
    class SupplierViewModel : BaseViewModel, INotifyPropertyChanged
    {
        // Event de notification de changement de propriété
        public event PropertyChangedEventHandler PropertyChanged;

        private string searchText;
        private string selectedType = "All";

        // Variable "Suppliers" utilisé pour le View
        public ObservableCollection<Supplier> Suppliers { get; set; } = new();

        public List<string> TypeList { get; set; } = new();


        // Liste complète pour filtrage
        private List<Supplier> allSuppliers { get; set; } = new();

        public string SearchText
        {
            get => searchText;
            set
            {
                if (searchText != value)
                {
                    searchText = value; //1
                    OnPropertyChanged(); //2
                    ApplyFilters(); //3
                }
            }
        }


        public SupplierViewModel()
        {
            // Chargement des données d'essais, sans passer par une BDD
            LoadTemplateSuppliersItems();

            // Initialisation de la liste des types pour le filtre
            TypeList = allSuppliers.Select(s => s.Type).Distinct().OrderBy(t => t).ToList();

            ApplyFilters();
        }


        public void AddSupplier(Supplier supplier)
        {
            Suppliers.Add(supplier);
            allSuppliers.Add(supplier);
            OnPropertyChanged(nameof(Suppliers));
        }


        // Applique les filtres à la liste "allSuppliers" et corrige la liste "Suppliers"
        private void ApplyFilters()
        {
            var filtered = allSuppliers.AsEnumerable();

            // Filtre par nom
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s => s.Name != null && s.Name.ToLower().Contains(SearchText.ToLower()));
            }

            // Filtre par type
            if (!string.IsNullOrWhiteSpace(selectedType) && selectedType != "All")
            {
                filtered = filtered.Where(s => s.Type == selectedType);
            }

            Suppliers.Clear();
            foreach (var supplier in filtered)
                Suppliers.Add(supplier);
        }


        // Appelle la vue si une propriété évolue
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void LoadTemplateSuppliersItems()
        {
            // Exemple de données statiques
            allSuppliers.Add(new Supplier { Name = "Fournisseur 1", Type = "Nourritures" });
            allSuppliers.Add(new Supplier { Name = "Fournisseur 2", Type = "Vêtements" });
            allSuppliers.Add(new Supplier { Name = "Fournisseur 3", Type = "Boulangerie" });
        }
    }
}
