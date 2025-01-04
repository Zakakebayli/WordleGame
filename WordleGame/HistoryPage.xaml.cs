namespace WordleGame;

public partial class HistoryPage : ContentPage
{
    public HistoryPage()
    {
        InitializeComponent();
        LoadHistory();
    }

    private void LoadHistory()
    {
        // Example data for history
        var historyData = new List<GameHistory>
        {
            new GameHistory { GameResult = "Victory! Word: APPLE", DatePlayed = "2025-01-03" },
            new GameHistory { GameResult = "Defeat! Word: GRAPE", DatePlayed = "2025-01-02" },
            new GameHistory { GameResult = "Victory! Word: LUCKY", DatePlayed = "2025-01-01" }
        };

        HistoryList.ItemsSource = historyData;
    }
}

public class GameHistory
{
    public string GameResult { get; set; }
    public string DatePlayed { get; set; }
}
