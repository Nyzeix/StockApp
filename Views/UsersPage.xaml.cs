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

        public UsersPage()
        {
            InitializeComponent();
            BindingContext = new UserViewModel();

            // Copie complète pour recherche
            if (ViewModel?.ExampleUsers != null)
                allUsers = new ObservableCollection<User>(ViewModel.ExampleUsers);
            else
                allUsers = new ObservableCollection<User>();
        }

        // Gestion de la sélection d'un utilisateur dans le CollectionView
        public void TODO(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.CurrentSelection?.FirstOrDefault() as User;
            if (selected != null)
            {
                DisplayAlert("Utilisateur sélectionné", $"Nom : {selected.Username}", "OK");

                if (sender is CollectionView cv)
                    cv.SelectedItem = null;
            }
        }

        // Affiche / masque le formulaire d'ajout
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

            var newUser = new User
            {
                Username = UsernameEntry.Text.Trim(),
                IsAdmin = IsAdminSwitch.IsToggled,
            };

            ViewModel?.ExampleUsers.Add(newUser);
            allUsers.Add(newUser); // ajoute aussi à la liste complète

            UsernameEntry.Text = string.Empty;
            IsAdminSwitch.IsToggled = false;
            AddUserForm.IsVisible = false;

            await DisplayAlert("Succès", "Utilisateur ajouté avec succès.", "OK");
        }

        // Annuler l'ajout et réinitialiser le formulaire
        private void OnCancelUserClicked(object sender, EventArgs e)
        {
            UsernameEntry.Text = string.Empty;
            IsAdminSwitch.IsToggled = false;
            AddUserForm.IsVisible = false;
        }

        // Supprimer un utilisateur
        private async void OnDeleteUserClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is User user)
            {
                bool confirm = await DisplayAlert("Confirmer", $"Supprimer {user.Username} ?", "Oui", "Non");
                if (confirm)
                {
                    ViewModel?.ExampleUsers.Remove(user);
                    allUsers.Remove(user); // aussi dans la liste complète
                }
            }
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
