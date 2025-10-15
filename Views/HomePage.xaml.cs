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
        double targetWidth = width * 0.7;   // 60% de la largeur de l’écran
        double targetHeight = height * 0.40; // 40% de la hauteur de l’écran

        // Minimum et maximum pour éviter des tailles absurdes
        targetWidth = Math.Clamp(targetWidth, 50, 80000);
        targetHeight = Math.Clamp(targetHeight, 50, 800000);

        UsersCard.WidthRequest = targetWidth;
        UsersCard.HeightRequest = targetHeight;

        StockCard.WidthRequest = targetWidth;
        StockCard.HeightRequest = targetHeight;
    }

    /* Fonction Logout dans le XAML
     <VerticalStackLayout Padding="24" Spacing="16">
        <Label Text="Vous êtes connecté" FontSize="18"/>
        <BoxView BackgroundColor="LightCyan" HeightRequest="200" WidthRequest="300" HorizontalOptions="Center"/>
        <Button Text="Se déconnecter" Clicked="OnLogoutClicked"/>
    </VerticalStackLayout>
     */
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
