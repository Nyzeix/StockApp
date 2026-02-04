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

        public ICommand SimplePressEditCommand { get; private set; }


        public StockPage(StockViewModel StockVM, SupplierViewModel SuppliersVM)
        {
            BindingContext = this.StockVM = StockVM;
            SimplePressEditCommand = new Command<Product>(async (product) => await OnEditProductCommandAsync(product));
            InitializeComponent();
        }


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

        // Commandes pour la pression sur un produit

        private async Task OnEditProductCommandAsync(Product selectedProduct)
        {
            if (selectedProduct == null) return;

            // Affiche la popup
            var popup = new EditProductPopup(selectedProduct, StockVM.AvailableSuppliers.ToList());

            // 3. Attendre le résultat
            var result = await this.ShowPopupAsync(popup);

            if (result is Product modifiedProduct)
            {
                // TODO : Logique métier ici (Sauvegarde DB)
                await StockVM.UpdateProductAsync(modifiedProduct);


                await DisplayAlert("Succès", $"Le produit a bien été modifié.", "OK");

            }
        }

    }
}
