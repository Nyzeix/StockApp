using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using StockApp.Services;
using StockApp.Views;

namespace StockApp.ViewModels;

/// <summary>
/// View model for the login page that handles user authentication.
/// Implements <see cref="INotifyPropertyChanged"/> to support data binding.
/// </summary>
/// <remarks>
/// This view model manages the login process, including username and password validation,
/// authentication through <see cref="IAuthDbService"/>, and navigation upon successful login.
/// It also provides commands for login action and navigation to the registration page.
/// </remarks>
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IAuthDbService _auth;
    private readonly ILogService _log;
    private string log_tag = "Login";

    private string _username = "";
    private string _password = "";
    private string _error = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public string Error
    {
        get => _error;
        set { _error = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; } // => new Command(OnLoginAsync);
    public ICommand GoRegisterCommand { get; } // => new Command(async () => await Shell.Current.GoToAsync(nameof(RegisterPage)));


    /// <summary>
    /// Initialise une nouvelle instance de <see cref="LoginViewModel"/>.
    /// </summary>
    /// <param name="auth">Service d'authentification.</param>
    /// <param name="log">Service de journalisation.</param>
    public LoginViewModel(IAuthDbService auth, ILogService log)
    {
        _auth = auth;
        _log = log;
        LoginCommand = new Command(async () => await OnLoginAsync());
        GoRegisterCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(RegisterPage)));
    }


    /// <summary>
    /// Effectue la tentative de connexion de l'utilisateur.
    /// Vérifie les champs, authentifie via le service et navigue vers la page d'accueil en cas de succès.
    /// </summary>
    /// <returns>Une tâche représentant l'opération asynchrone.</returns>
    private async Task OnLoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            Error = "Veuillez remplir tous les champs.";
            return;
        }

        bool success = await _auth.LoginAsync(Username, Password);

        if (success)
        {
            _log.LogInfo(log_tag, "User logged in: " + Username);
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
        else
        {
            _log.LogInfo(log_tag, "Login failed for user: " + Username);
            Error = "Nom d'utilisateur ou mot de passe incorrect ❌";
        }
    }


    /// <summary>
    /// Notifie l'interface qu'une propriété a changé.
    /// </summary>
    /// <param name="name">Le nom de la propriété modifiée (renseigné automatiquement).</param>
    private void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
