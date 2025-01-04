namespace WordleGame;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

       
        Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
        Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
    }
}
