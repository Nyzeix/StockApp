using StockApp.Services;
using Microsoft.Maui.Controls;

namespace StockApp.Views;

public partial class HomePage : ContentPage
{
    private readonly IAuthDbService _auth;
    public HomePage(IAuthDbService auth)
    {
        InitializeComponent();
        _auth = auth;
        // Message de bienvenue avec le nom d'utilisateur authentifi�
        if (_auth.CurrentUser != null)
        {
            WelcomeLabel.Text = $"Bienvenue, {_auth.CurrentUser.Username}!";
            // Affiche la carte Utilisateurs si l'utisateur connect� est admin
            if (_auth.CurrentUser.IsAdmin)
            {
                UsersCard.IsVisible = true;
            }
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Ajuste la taille des cartes en fonction de la largeur de l��cran
        double targetWidth = width * 0.8;   // 80% de la largeur de l��cran
        double targetHeight = height * 0.30; // 30% de la hauteur de l��cran

        // Min et Max
        targetWidth = Math.Clamp(targetWidth, 50, 800);
        targetHeight = Math.Clamp(targetHeight, 50, 800);

        UsersCard.WidthRequest = targetWidth;
        UsersCard.HeightRequest = targetHeight;

        StockCard.WidthRequest = targetWidth;
        StockCard.HeightRequest = targetHeight;

        SuppliersCard.WidthRequest = targetWidth;
        SuppliersCard.HeightRequest = targetHeight;
    }


    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        _auth.Logout();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    private async void OnTapUsersPage(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(UsersPage));

    private async void OnTapSuppliersPage(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(SuppliersPage));

    private async void OnTapStockPage(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(StockPage));

    private async void OnTapMovementsPage(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(MovementsPage));

}
