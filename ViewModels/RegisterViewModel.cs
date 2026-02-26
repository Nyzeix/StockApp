using System;
using System.Collections.Generic;
using System.Windows.Input;
using StockApp.Services;

namespace StockApp.ViewModels
{

    /// <summary>
    /// ViewModel pour la page d'inscription, gère la création de nouveaux comptes utilisateurs.
    /// </summary>
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthDbService _auth;
        private readonly ILogService _log;
        private readonly string log_tag = "Register";

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


        /// <summary>
        /// Initialise une nouvelle instance de <see cref="RegisterViewModel"/>.
        /// </summary>
        /// <param name="auth">Service d'authentification.</param>
        /// <param name="log">Service de journalisation.</param>
        public RegisterViewModel(IAuthDbService auth, ILogService log)
        {
            _auth = auth;
            _log = log;
            RegisterCommand = new Command(async () => await OnRegisterAsync());
            BackToLoginCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }


        /// <summary>
        /// Effectue l'inscription d'un nouvel utilisateur.
        /// Valide les champs, vérifie l'unicité du pseudo et crée le compte.
        /// </summary>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
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
                _log.LogInfo(log_tag, "Failed to register user: " + Username);
                Error = "Impossible de créer le compte.";
                return;
            }
            _log.LogInfo(log_tag, "User registered: " + Username);
            await Shell.Current.DisplayAlert("Succès", "Compte créé, vous pouvez vous connecter.", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}