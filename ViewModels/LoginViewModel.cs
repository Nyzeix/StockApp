using StockApp.Services;
using StockApp.Views;
using StockApp.ViewModels;
using System.Windows.Input;

namespace StockApp.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _auth;

    private string _username = "";
    public string Username { get => _username; set => Set(ref _username, value); }

    private string _password = "";
    public string Password { get => _password; set => Set(ref _password, value); }

    private string _error = "";
    public string Error { get => _error; set => Set(ref _error, value); }

    public ICommand LoginCommand { get; }
    public ICommand GoRegisterCommand { get; }

    public LoginViewModel(IAuthService auth)
    {
        _auth = auth;
        LoginCommand = new Command(async () => await OnLoginAsync());
        GoRegisterCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(RegisterPage)));
    }

    private async Task OnLoginAsync()
    {
        Error = "";
        var ok = await _auth.LoginAsync(Username, Password);
        if (ok)
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}"); // nav root vers Home
            Username = Password = "";
        }
        else
        {
            Error = "Identifiants invalides.";
        }
    }
}
