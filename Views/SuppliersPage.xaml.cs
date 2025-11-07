using StockApp.Models;
using StockApp.ViewModels;
using System.Collections.ObjectModel;

namespace StockApp.Views
{
    public partial class SuppliersPage : ContentPage
    {
        private SupplierViewModel ViewModel => BindingContext as SupplierViewModel;
        

        public SuppliersPage()
        {
            InitializeComponent();
            BindingContext = new SupplierViewModel();
        }

        // SelectionChanged - signature correcte
        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.CurrentSelection?.FirstOrDefault() as Supplier;
            if (selected is null)
                return;

            await DisplayAlert("Sélection", $"Fournisseur sélectionné : {selected.Name}", "OK");

            if (sender is CollectionView cv)
                cv.SelectedItem = null;
        }


        // Affiche / masque le formulaire
        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            AddSupplierForm.IsVisible = !AddSupplierForm.IsVisible;
        }


        // Sauvegarde : ajoute le fournisseur à la collection du ViewModel
        private async void OnSaveSupplierClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry?.Text))
            {
                await DisplayAlert("Erreur", "Le nom du produit est obligatoire.", "OK");
                return;
            }

            int quantity = 0;
            if (!string.IsNullOrWhiteSpace(NameEntry?.Text))
                int.TryParse(typeEntry.Text, out quantity);

            var newSupplier = new Supplier
            {
                Name = NameEntry.Text?.Trim(),
                Type = typeEntry.Text?.Trim()
            };

            ViewModel.AddSupplier(newSupplier); // Renvoi l'item au ViewModel

            // Reset formulaire
            NameEntry.Text = string.Empty;
            typeEntry.Text = string.Empty;

            AddSupplierForm.IsVisible = false;

            await DisplayAlert("Succès", "Fournisseur ajouté avec succès.", "OK");
        }


        private void OnCancelClicked(object sender, EventArgs e)
        {
            NameEntry.Text = string.Empty;
            typeEntry.Text = string.Empty;

            AddSupplierForm.IsVisible = false;
        }
    }
}