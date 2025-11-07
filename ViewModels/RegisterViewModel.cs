using System;
using System.Collections.Generic;
using System.Windows.Input;
using StockApp.Services;

namespace StockApp.ViewModels
{

    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _auth;

        private string _username = "";
        public string Username { get => _username; set => Set(ref _username, value); }

        private string _password = "";
        public string Password { get => _password; set => Set(ref _password, value); }

        private string _password2 = "";
        public string Password2 { get => _password2; set => Set(ref _password2, value); }

        private string _error = "";
        public string Error { get => _error; set => Set(ref _error, value); }

        public ICommand RegisterCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public RegisterViewModel(IAuthService auth)
        {
            _auth = auth;
            RegisterCommand = new Command(async () => await OnRegisterAsync());
            BackToLoginCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        private async Task OnRegisterAsync()
        {
            Error = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                Error = "Pseudo et mot de passe requis.";
                return;
            }
            if (Password != Password2)
            {
                Error = "Les mots de passe ne correspondent pas.";
                return;
            }
            if (await _auth.UsernameExistsAsync(Username))
            {
                Error = "Ce pseudo existe déjà.";
                return;
            }

            var ok = await _auth.RegisterAsync(Username, Password);
            if (!ok)
            {
                Error = "Impossible de créer le compte.";
                return;
            }

            await Shell.Current.DisplayAlert("Succès", "Compte créé, vous pouvez vous connecter.", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}