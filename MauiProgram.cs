using Microsoft.Extensions.Logging;
using StockApp.Services;
using StockApp.ViewModels;
using StockApp.Views;
using CommunityToolkit.Maui;

namespace StockApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<IAuthDbService, AuthDbService>();
        builder.Services.AddSingleton<ILogService, LogService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<UserViewModel>();
        builder.Services.AddTransient<StockViewModel>();
        builder.Services.AddTransient<SupplierViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<UsersPage>();
        builder.Services.AddTransient<StockPage>();
        builder.Services.AddTransient<SuppliersPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
