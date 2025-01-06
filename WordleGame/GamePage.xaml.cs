namespace WordleGame;

public partial class GamePage : ContentPage
{
    private const int WordLength = 5; // Length of the target word
    private const int MaxGuesses = 6; // Maximum guesses allowed
    private string TargetWord = string.Empty;
    private int CurrentGuess = 0;

    private static readonly HttpClient HttpClient = new HttpClient();

    public GamePage()
    {
        InitializeComponent();
        StartNewGame(); // Begin a new game when the page loads
    }

    private async void StartNewGame()
    {
        TargetWord = await FetchRandomWord(); // Get a random word
        CurrentGuess = 0;

        WordGrid.Children.Clear(); // Reset the grid
        InitializeWordGrid(); // Create a fresh grid

        GuessInput.Text = string.Empty;
        GuessInput.IsEnabled = true;
    }

    private void InitializeWordGrid()
    {
        WordGrid.RowDefinitions.Clear();
        WordGrid.ColumnDefinitions.Clear();

        // Create rows and columns for the grid
        for (int i = 0; i < MaxGuesses; i++)
        {
            WordGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        for (int j = 0; j < WordLength; j++)
        {
            WordGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        // Add cells to the grid
        for (int row = 0; row < MaxGuesses; row++)
        {
            for (int col = 0; col < WordLength; col++)
            {
                Label cell = new Label
                {
                    BackgroundColor = Colors.LightGray,
                    WidthRequest = 50,
                    HeightRequest = 50,
                    Margin = new Thickness(2),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };
                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);
                WordGrid.Children.Add(cell);
            }
        }
    }

    private async void OnSubmitGuessClicked(object sender, EventArgs e)
    {
        string guess = GuessInput.Text;

        // Validate the guess
        if (string.IsNullOrWhiteSpace(guess) || guess.Length != WordLength || !await IsValidWord(guess))
        {
            await DisplayAlert("Invalid Guess", "Enter a valid 5-letter word.", "OK");
            return;
        }

        // Update the grid with the guess result
        var rowChildren = WordGrid.Children
            .Where(c => Grid.GetRow(c as View) == CurrentGuess)
            .OfType<Label>()
            .ToList();

        for (int i = 0; i < WordLength; i++)
        {
            rowChildren[i].Text = guess[i].ToString();

            if (guess[i] == TargetWord[i])
                rowChildren[i].BackgroundColor = Colors.Green; // Correct position
            else if (TargetWord.Contains(guess[i]))
                rowChildren[i].BackgroundColor = Colors.Yellow; // Present but wrong position
            else
                rowChildren[i].BackgroundColor = Colors.Gray; // Not in the word
        }

        if (guess == TargetWord)
        {
            await DisplayAlert("Congratulations!", $"You guessed the word: {TargetWord}!", "New Game");
            StartNewGame();
            return;
        }

        CurrentGuess++;

        if (CurrentGuess >= MaxGuesses)
        {
            await DisplayAlert("Game Over", $"The correct word was: {TargetWord}", "New Game");
            StartNewGame();
        }
        else
        {
            GuessInput.Text = string.Empty;
        }
    }

    private async Task<string> FetchRandomWord()
    {
        try
        {
            // Get a random 5-letter word
            var response = await HttpClient.GetAsync("https://random-word-api.herokuapp.com/word?length=5");
            if (response.IsSuccessStatusCode)
            {
                var words = await response.Content.ReadAsStringAsync();
                return words.Trim('[', ']', '"');
            }
        }
        catch
        {
            return "APPLE"; // Default word if API fails
        }

        return "APPLE";
    }

    private async Task<bool> IsValidWord(string word)
    {
        try
        {
            // Check if the word is valid
            var response = await HttpClient.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
