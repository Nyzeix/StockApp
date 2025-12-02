using Microsoft.Maui.Controls;
using StockApp.Models;
using StockApp.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace StockApp.Views
{
    public partial class UsersPage : ContentPage
    {
        private UserViewModel ViewModel => BindingContext as UserViewModel;

        // Liste complète pour filtrage
        private ObservableCollection<User> allUsers;

        public UsersPage(UserViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

            // Copie complète pour recherche
            if (ViewModel?.ExampleUsers != null)
                allUsers = new ObservableCollection<User>(ViewModel.ExampleUsers);
            else
                allUsers = new ObservableCollection<User>();
        }

        // Gestion de la sélection d'un utilisateur dans le CollectionView
        public async void TODO(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.CurrentSelection?.FirstOrDefault() as User;
            if (selected != null)
            {
                // 🔹 Afficher le mot de passe à la place
                string passwordToShow = string.IsNullOrEmpty(selected.PasswordHash) ? "(aucun mot de passe)" : selected.PasswordHash;

                await DisplayAlert("Utilisateur sélectionné", $"Nom : {selected.Username}\nMot de passe : {passwordToShow}", "OK");

                // Désélection pour pouvoir recliquer
                if (sender is CollectionView cv)
                    cv.SelectedItem = null;
            }
        }

        // Affiche / masque le formulaire d’ajout
        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            AddUserForm.IsVisible = !AddUserForm.IsVisible;
        }

        // Sauvegarde un nouvel utilisateur
        private async void OnSaveUserClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
            {
                await DisplayAlert("Erreur", "Le nom d'utilisateur est obligatoire.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Erreur", "Le mot de passe est obligatoire.", "OK");
                return;
            }

            string message = await ViewModel.AddUserAsync(UsernameEntry.Text.Trim(), PasswordEntry.Text.Trim(), IsAdminSwitch.IsToggled);

            // Réinitialiser le formulaire
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            IsAdminSwitch.IsToggled = false;
            AddUserForm.IsVisible = false;

            await DisplayAlert("Résultat", message, "OK");
        }

        // Supprimer un utilisateur
        private async void OnDeleteUserClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is User user)
            {
                bool confirm = await DisplayAlert("Confirmer", $"Supprimer {user.Username} ?", "Oui", "Non");
                if (confirm)
                {
                    string message = await ViewModel.DeleteUserAsync(user.Username);
                }
            }
        }


        // Annuler l’ajout et réinitialiser le formulaire
        private void OnCancelUserClicked(object sender, EventArgs e)
        {
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            IsAdminSwitch.IsToggled = false;
            AddUserForm.IsVisible = false;
        }

        // Barre de recherche
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = e.NewTextValue?.ToLower() ?? string.Empty;

            var filtered = allUsers
                .Where(u => !string.IsNullOrWhiteSpace(u.Username) && u.Username.ToLower().Contains(filter))
                .ToList();

            ViewModel.ExampleUsers.Clear();
            foreach (var item in filtered)
                ViewModel.ExampleUsers.Add(item);
        }
    }
}
