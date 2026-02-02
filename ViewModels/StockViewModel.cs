using StockApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockApp.ViewModels
{

    public class StockViewModel : BaseViewModel, INotifyPropertyChanged
    {
        // Event de notification de changement de propriété
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly SupplierViewModel _supplierViewModel;

        // Change le type ici pour l'interface
        private readonly IDatabaseService _db;

        // Liste affichée
        public ObservableCollection<Product> StockItems { get; set; } = new();
        // Liste interne
        private List<Product> _StockItems { get; set; } = new List<Product>();

        public ObservableCollection<string> AvailableOrigins
        {
            get;
            set;
        } = new();
        public ObservableCollection<string> AvailableSuppliers { get; set; } = new();
        public ObservableCollection<string> AvailableQuantities { get; set; } = new()
        {
            "Tous", "0-10", "11-50", "51-100", "101+"
        };


        // Propriétés de filtrage
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }
        private string _selectedOrigin = "Tous";
        public string SelectedOrigin
        {
            get => _selectedOrigin;
            set
            {
                if (_selectedOrigin != value)
                {
                    _selectedOrigin = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        private string _selectedSupplier = "Tous";
        public string SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                if (_selectedSupplier != value)
                {
                    _selectedSupplier = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        private string _selectedQuantity = "Tous";
        public string SelectedQuantity
        {
            get => _selectedQuantity;
            set
            {
                if (_selectedQuantity != value)
                {
                    _selectedQuantity = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }


        // Commandes pour la pression sur un produit
        public ICommand SimplePressEditCommand { get; private set; }
        public ICommand LongPressDeleteCommand { get; private set; }

        private bool _itemsLoaded = false;

        // Change le type du paramètre ici aussi
        public StockViewModel(IDatabaseService db, SupplierViewModel svm)
        {
            _supplierViewModel = svm;
            _db = db;
            Task.Run(async () => await LoadDataAsync());

            //SimplePressEditCommand = new Command<Product>(async (product) => await OnEditProduct(product));
            // Initialisation de la commande de suppression (Appui Long)
            LongPressDeleteCommand = new Command<Product>(async (product) => await DeleteProductCommandAsync(product));

            // Hotfix pour le chargement des listes de filtres après le chargement des produits.
            // Sans ça, la liste des origines reste vide car le LoadDataAsync n'est pas terminé.
            // Ce n'est pas propre et doit être modifié.
            while (_itemsLoaded == false)
            {
                Task.Delay(100).Wait();
            }
            LoadSuppliersList();
            LoadOriginsList();
        }

        public async Task LoadDataAsync()
        {
            var products = await _db.GetProductsAsync();
            _StockItems.Clear();
            foreach (var product in products)
            {
                _StockItems.Add(product);
            }
            _itemsLoaded = true;
            ApplyFilters(); // Update UI
        }

        /*private void LoadStockItems()
        {
            // Exemple de données statiques
            for(int i = 1; i<= 20; i++)
            {
                StockItems.Add(new Product { Name = $"Produit {i}", Quantity = i * 10, CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            }
            StockItems.Add(new Product { Name = "Pommes", Quantity = 50, Origin = "France", Color = "Rouge", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            StockItems.Add(new Product { Name = "Bananes", Quantity = 30, Origin = "Equateur", Color = "Jaune", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
            StockItems.Add(new Product { Name = "Oranges", Quantity = 20, Origin = "Espagne", Color = "Orange", CreatedAt = (int)DateTimeOffset.Now.ToUnixTimeSeconds() });
        }*/

        // Récupère les données de fournisseurs depuis le VM SupplierViewModel
        public void LoadSuppliersList()
        {
            // On vide la liste
            AvailableSuppliers.Clear();

            // Données de suppliers ViewModel
            var suppliersData = _supplierViewModel.GetSuppliers();

            AvailableSuppliers.Add("Tous");
            foreach (var supplier in suppliersData)
            {
                AvailableSuppliers.Add(supplier.Name);
            }

            // Notifie l'interface que la liste a potentiellement changé (utile si on réassigne l'objet collection complète)
            OnPropertyChanged(nameof(AvailableSuppliers));
        }

        // Récupère les données d'origines depuis la liste des produits en stock
        public void LoadOriginsList()
        {
            AvailableOrigins.Clear();
            var originsData = _StockItems.Select(p => p.Origin).Distinct().OrderBy(o => o);
            AvailableOrigins.Add("Tous");
            foreach (var origin in originsData)
            {
                AvailableOrigins.Add(origin);
            }
            OnPropertyChanged(nameof(AvailableOrigins));
        }


        public async Task<bool> AddProductAsync(Product product)
        {
            try {
                await _db.AddProductAsync(product);
                await LoadDataAsync();
                ApplyFilters();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private async Task DeleteProductCommandAsync(Product product)
        {
            if (product == null) return;

            // Suppression BDD
            await _db.DeleteProductAsync(product);

            // Suppression Mémoire
            _StockItems.Remove(product);

            // Rafraichir l'affichage
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<Product> filtered = _StockItems;

            // Recherche
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p => p.Name != null && p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            // Originie
            if (!string.IsNullOrEmpty(SelectedOrigin) && SelectedOrigin != "Tous")
            {
                filtered = filtered.Where(p => p.Origin == SelectedOrigin);
            }
            // Fournisseur
            if (!string.IsNullOrEmpty(SelectedSupplier) && SelectedSupplier != "Tous")
            {
                filtered = filtered.Where(p => p.Supplier == SelectedSupplier);
            }
            // Quantité
            if (!string.IsNullOrEmpty(SelectedQuantity) && SelectedQuantity != "Tous")
            {
                filtered = filtered.Where(p => CheckQuantity(p.Quantity, SelectedQuantity));
            }

            // Mise à jour de la collection observable
            StockItems.Clear();
            foreach (var item in filtered)
            {
                StockItems.Add(item);
            }
        }

        private bool CheckQuantity(int qty, string filter)
        {
            return filter switch
            {
                "0-10" => qty <= 10,
                "11-50" => qty > 10 && qty <= 50,
                "51-100" => qty > 50 && qty <= 100,
                "101+" => qty > 100,
                _ => true
            };
        }

        // Appelle la vue si une propriété évolue
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}