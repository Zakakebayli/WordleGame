namespace WordleGame;

// Represents the history page showing past game results
public partial class HistoryPage : ContentPage
{
    public HistoryPage()
    {
        InitializeComponent(); // Sets up the UI
        LoadHistory(); // Loads past game data
    }

    private void LoadHistory()
    {
        // Example list of past game results
        var historyData = new List<GameHistory>
        {
            new GameHistory { GameResult = "Victory! Word: APPLE", DatePlayed = "2025-01-03" },
            new GameHistory { GameResult = "Defeat! Word: GRAPE", DatePlayed = "2025-01-02" },
            new GameHistory { GameResult = "Victory! Word: LUCKY", DatePlayed = "2025-01-01" }
        };

        HistoryList.ItemsSource = historyData; // Display data in the UI
    }
}

// Defines the structure for each game entry in the history
public class GameHistory
{
    public string GameResult { get; set; }
    public string DatePlayed { get; set; }
}
