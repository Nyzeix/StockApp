using StockApp.Views;

namespace StockApp;

public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(UsersPage), typeof(UsersPage));
            Routing.RegisterRoute(nameof(StockPage), typeof(StockPage));

        // Démarrage sur Login
        //GoToAsync($"//{nameof(LoginPage)}");
    }
    }