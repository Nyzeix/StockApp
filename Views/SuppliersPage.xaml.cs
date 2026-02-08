using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using StockApp.Models;
using StockApp.ViewModels;
using StockApp.Views.Popups; 

namespace StockApp.Views
{
    public partial class SuppliersPage : ContentPage
    {
        private readonly SupplierViewModel SuppliersVM;

        public ICommand SimplePressEditCommand { get; private set; }
        
        public SuppliersPage(SupplierViewModel SuppliersVM)
        {
            BindingContext = this.SuppliersVM = SuppliersVM;
            SimplePressEditCommand = new Command<Supplier>(async (supplier) => await OnEditSupplierCommandAsync(supplier));
            InitializeComponent();
        }


        // Affiche / masque le formulaire
        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            var popup = new AddSupplierPopup();
            var result = await this.ShowPopupAsync(popup);
            if (result is Supplier newSupplier)
            {
                if (await SuppliersVM.AddSupplierAsync(newSupplier))
                {
                    await DisplayAlert("Succès", $"Fournisseur {newSupplier.Name} ajouté !", "OK");
                } else
                {
                    await DisplayAlert("Erreur", $"Erreur lors de l'ajout du fournisseur.", "OK");
                }
            }
        }

        private async Task OnEditSupplierCommandAsync(Supplier selectedSupplier)
        {
            if (selectedSupplier == null) return;

            var popup = new EditSupplierPopup(selectedSupplier);

            var result = await this.ShowPopupAsync(popup);

            if (result is Supplier modifiedSupplier)
            {
                await SuppliersVM.UpdateSupplierAsync(modifiedSupplier);
                await DisplayAlert("Succès", $"Le fournisseur a bien été modifié.", "OK");
            }
        }
    }
}