using WarehouseNFC.Services;
using WarehouseNFC.Views;
using WarehouseNFC.ViewModels;

namespace WarehouseNFC;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Сервисы
        builder.Services.AddSingleton<INfcService, MockNfcService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<InventoryViewModel>();
        builder.Services.AddTransient<InventoryResultsViewModel>();

        // Страницы
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<InventoryPage>();
        builder.Services.AddTransient<InventoryResultsPage>();

        return builder.Build();
    }
}