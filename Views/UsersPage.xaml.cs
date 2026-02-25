using CommunityToolkit.Maui.Views;
using StockApp.Models;
using StockApp.ViewModels;
using StockApp.Views.Popups;
using System.Windows.Input;

namespace StockApp.Views
{
    public partial class UsersPage : ContentPage
    {
        private readonly UserViewModel UsersVM;

        public ICommand SimplePressEditCommand { get; private set; }

        public UsersPage(UserViewModel vm)
        {
            BindingContext = this.UsersVM = vm;
            SimplePressEditCommand = new Command<User>(async (user) => await OnEditUserCommandAsync(user));
            InitializeComponent();
        }


        // Affiche / masque le formulaire
        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            var popup = new AddUserPopup();
            var result = await this.ShowPopupAsync(popup);
            if (result is User newUser)
            {
                //if (await UsersVM.AddUserAsync(newUser.Username, newUser.PasswordHash, newUser.IsAdmin))
                if (await UsersVM.AddUserAsync(newUser))
                {
                    await DisplayAlert("Succès", $"Utilisateur {newUser.Username} ajouté !", "OK");
                } else
                {
                    await DisplayAlert("Erreur", $"Erreur lors de l'ajout de l'utilisateur.", "OK");
                }
            }
        }

        private async Task OnEditUserCommandAsync(User selectedUser)
        {
            if (selectedUser == null) return;

            var popup = new EditUserPopup(selectedUser);

            var result = await this.ShowPopupAsync(popup);

            if (result is User modifiedUser)
            {
                await UsersVM.UpdateUserAsync(modifiedUser);
                await DisplayAlert("Succès", $"L'utilisateur a bien été modifié.", "OK");
            }
        }
    }
}
