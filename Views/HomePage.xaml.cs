using StockApp.Services;
using Microsoft.Maui.Controls;

namespace StockApp.Views;

public partial class HomePage : ContentPage
{
    private readonly IAuthService _auth;
    public HomePage(IAuthService auth)
    {
        InitializeComponent();
        _auth = auth;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Ajuste la taille des cartes en fonction de la largeur de l’écran
        double targetWidth = width * 0.7;   // 70% de la largeur de l’écran
        double targetHeight = height * 0.40; // 40% de la hauteur de l’écran

        // Min et Max
        targetWidth = Math.Clamp(targetWidth, 50, 800);
        targetHeight = Math.Clamp(targetHeight, 50, 800);

        UsersCard.WidthRequest = targetWidth;
        UsersCard.HeightRequest = targetHeight;

        StockCard.WidthRequest = targetWidth;
        StockCard.HeightRequest = targetHeight;
    }


    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        _auth.Logout();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    private async void OnTapUsersPage(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(UsersPage));

    private async void OnTapStockPage(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync(nameof(StockPage));

}
