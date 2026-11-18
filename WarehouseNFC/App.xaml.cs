namespace WarehouseNFC;

public partial class App : Application
{
    public static string CurrentRole { get; set; } = string.Empty;
    public static string CurrentFullName { get; set; } = string.Empty;

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}