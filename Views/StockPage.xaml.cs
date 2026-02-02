using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using StockApp.Models;
using StockApp.ViewModels;
using StockApp.Views.Popups; 
namespace StockApp.Views
{
    public partial class StockPage : ContentPage
    {
        private readonly StockViewModel StockVM;
        private readonly SupplierViewModel SuppliersVM;
        //private ObservableCollection<Product> allProducts;

        public StockPage(StockViewModel StockVM, SupplierViewModel SuppliersVM)
        {
            BindingContext = this.StockVM = StockVM;
            InitializeComponent();

            /*if (this.StockVM?.StockItems != null)
                allProducts = new ObservableCollection<Product>(this.StockVM.StockItems);
            else
                allProducts = new ObservableCollection<Product>();*/

            //setupPickers();
        }

        /*private void setupPickers()
        {
            List<String> tmpOriginPickerData = [.. StockVM.StockItems.Select(p => p.Origin).Distinct().OrderBy(o => o)];
            tmpOriginPickerData.Insert(0, "Tous");
            OriginFilterPicker.ItemsSource = tmpOriginPickerData;

            QuantityFilterPicker.ItemsSource = new List<string> { "Tous", "0-10", "11-50", "51-100", "101+" };

            List<String> tmpSuppliersPickerData = [.. SuppliersVM.Suppliers.Select(s => s.Name).Distinct().OrderBy(o => o)];
            tmpSuppliersPickerData.Insert(0, "Tous");
            SupplierFilterPicker.ItemsSource = tmpSuppliersPickerData;

            OriginFilterPicker.SelectedIndex = 0;
            QuantityFilterPicker.SelectedIndex = 0;
            SupplierFilterPicker.SelectedIndex = 0;
        }*/

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            var popup = new AddProductPopup(this.StockVM.AvailableSuppliers.ToList());
            var result = await this.ShowPopupAsync(popup);

            if (result is Product newProduct)
            {
                if (await StockVM.AddProductAsync(newProduct))
                {
                    await DisplayAlert("Succès", $"Produit {newProduct.Name} ajouté !", "OK");
                } else
                {
                    await DisplayAlert("Erreur", $"Erreur lors de l'ajout du produit.", "OK");
                }
            }
        }

        /*private async Task OnEditProduct(Product selectedProduct)
        {
            if (selectedProduct == null) return;

            // Cascadé la liste plutôt que de la recréer ici afin d'optimiser les ressources
            var originsList = StockVM.StockItems.Select(p => p.Origin).Distinct().OrderBy(o => o).ToList();
            var suppliersList = SuppliersVM.Suppliers.Select(s => s.Name).Distinct().OrderBy(n => n).ToList();

            // Affiche la popup
            var popup = new EditProductPopup(selectedProduct, originsList, suppliersList);

            // 3. Attendre le résultat
            var result = await this.ShowPopupAsync(popup);

            if (result is Product modifiedProduct)
            {
                // TODO : Logique métier ici (Sauvegarde DB)


                await DisplayAlert("Succès", $"Le produit à été modifié", "OK");

            }
        }*/

        // Gestion de la supression avec appui long
        /*private async Task OnLongPressDelete(Product product)
        {
            if (product == null) return;
            bool confirm = await DisplayAlert("Suppression",
                                              $"Voulez-vous supprimer définitivement '{product.Name}' ?",
                                              "Oui, supprimer", "Annuler");
            if (confirm)
            {
                StockVM.StockItems.Remove(product);
                setupPickers();
            }
        }*/

    }
}
