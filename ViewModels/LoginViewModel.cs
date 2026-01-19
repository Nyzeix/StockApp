using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using StockApp.Services;
using StockApp.Views; // <-- nécessaire pour HomePage

namespace StockApp.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private string _username = "";
    private string _password = "";
    private string _error = "";

    public event PropertyChangedEventHandler PropertyChanged;

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

    public ICommand LoginCommand => new Command(OnLogin);

    private async void OnLogin()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            Error = "Veuillez remplir tous les champs.";
            return;
        }

        bool success = TestDBService.TestLoginSimple(Username, Password);

        if (success)
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
        else
        {
            Error = "Nom d'utilisateur ou mot de passe incorrect ❌";
        }
    }

    private void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
