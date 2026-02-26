using StockApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    public class SupplierViewModel : BaseViewModel
    {
        private readonly IDatabaseService _db;

        // Variable "Suppliers" utilisé pour le View
        public ObservableCollection<Supplier> Suppliers { get; set; } = new();
        // Liste interne
        private List<Supplier> _suppliers { get; set; } = new List<Supplier>();

        public ObservableCollection<string> TypeList { get; set; } = new();


        // Propriétés de filtrage
        // Explication: la variable privée stocke la valeur réelle, et subit les modifications.
        // La variable publique, est uniquement utilisé pour l'affichage.
        // Son getter et setter se calquent sur la valeur de la variable privée.
        // Le Setter applique les modifications, et notifie la vue.
        // Le Getter renvoi la valeur de la variable privée.
        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value; //1
                    OnPropertyChanged(); //2
                    ApplyFilters(); //3
                }
            }
        }

        private string _selectedType = "Tous";
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        // Edition / Suppression
        public ICommand LongPressDeleteCommand { get; private set; }

        private bool _itemsLoaded = false;

        public SupplierViewModel(IDatabaseService db)
        {
            _db = db;
            Task.Run(async () => await LoadSuppliersAsync());

            // Initialisation de la liste des types pour le filtre
            TypeList.Add("All");

            // SimplePressEditCommand TODO
            LongPressDeleteCommand = new Command<Supplier>(async (supplier) => await DeleteSupplierCommandAsync(supplier));


            while (_itemsLoaded == false)
            {
                Task.Delay(100).Wait();
            }
            LoadSupplierTypeList();

        }

        private async Task LoadSuppliersAsync()
        {
            var suppliers = await _db.GetSuppliersAsync();
            _suppliers.Clear();
            foreach (var supplier in suppliers)
            {
                _suppliers.Add(supplier);
            }
            _itemsLoaded = true;
            ApplyFilters(); //Update UI
        }

        public void LoadSupplierTypeList()
        {
            TypeList.Clear();
            TypeList.Add("Tous");
            foreach (var supplier in Suppliers)
            {
                if (!TypeList.Contains(supplier.Type))
                {
                    TypeList.Add(supplier.Type);
                }
            }
            OnPropertyChanged(nameof(TypeList));
        }

        public async Task<bool> AddSupplierAsync(Supplier supplier)
        {
            try
            {
                await _db.AddSupplierAsync(supplier);
                await LoadSuppliersAsync();
                OnPropertyChanged(nameof(Suppliers));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateSupplierAsync(Supplier modifiedSupplier)
        {
            try
            {
                await _db.UpdateSupplierAsync(modifiedSupplier);
                await LoadSuppliersAsync();
                ApplyFilters();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private async Task<bool> DeleteSupplierCommandAsync(Supplier supplier)
        {
            if (supplier == null) return false;
            try
            {
                await _db.DeleteSupplierAsync(supplier);
                await LoadSuppliersAsync();
                OnPropertyChanged(nameof(Suppliers));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Supplier> GetSuppliers()
        {
            return Suppliers.ToList();
        }


        // Applique les filtres à la liste "Suppliers" et corrige la liste "Suppliers"
        private void ApplyFilters()
        {
            IEnumerable<Supplier> filtered = _suppliers;

            // Filtre par nom
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s => s.Name != null && s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Filtre par type
            if (!string.IsNullOrWhiteSpace(SelectedType) && SelectedType != "Tous")
            {
                filtered = filtered.Where(s => s.Type == SelectedType);
            }

            Suppliers.Clear();
            foreach (var supplier in filtered)
                Suppliers.Add(supplier);
        }
    }
}
