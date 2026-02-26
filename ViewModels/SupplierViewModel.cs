using StockApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    /// <summary>
    /// ViewModel pour la gestion des fournisseurs, incluant le filtrage, l'ajout, la modification et la suppression.
    /// </summary>
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


        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SupplierViewModel"/>.
        /// Charge les fournisseurs et initialise les filtres.
        /// </summary>
        /// <param name="db">Service de base de données.</param>
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


        /// <summary>
        /// Charge les fournisseurs depuis la base de données de manière asynchrone.
        /// </summary>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
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


        /// <summary>
        /// Charge la liste des types de fournisseurs distincts pour le filtre.
        /// </summary>
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


        /// <summary>
        /// Ajoute un fournisseur en base de données et rafraîchit la liste.
        /// </summary>
        /// <param name="supplier">Le fournisseur à ajouter.</param>
        /// <returns><c>true</c> si l'ajout a réussi ; sinon <c>false</c>.</returns>
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


        /// <summary>
        /// Met à jour un fournisseur existant en base de données et rafraîchit la liste.
        /// </summary>
        /// <param name="modifiedSupplier">Le fournisseur modifié.</param>
        /// <returns><c>true</c> si la mise à jour a réussi ; sinon <c>false</c>.</returns>
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


        /// <summary>
        /// Supprime un fournisseur de la base de données et rafraîchit la liste.
        /// </summary>
        /// <param name="supplier">Le fournisseur à supprimer.</param>
        /// <returns><c>true</c> si la suppression a réussi ; sinon <c>false</c>.</returns>
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


        /// <summary>
        /// Retourne la liste complète des fournisseurs.
        /// </summary>
        /// <returns>Une liste de <see cref="Supplier"/>.</returns>
        public List<Supplier> GetSuppliers()
        {
            return Suppliers.ToList();
        }


        /// <summary>
        /// Applique les filtres de recherche et de type sur la liste des fournisseurs.
        /// </summary>
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
