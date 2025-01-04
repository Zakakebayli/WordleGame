namespace WordleGame;

public partial class GamePage : ContentPage
{
    private const int WordLength = 5;
    private const int MaxGuesses = 6;
    private string TargetWord = string.Empty;
    private int CurrentGuess = 0;

    private static readonly HttpClient HttpClient = new HttpClient();

    public GamePage()
    {
        InitializeComponent();
        StartNewGame();
    }

    private async void StartNewGame()
    {
        TargetWord = await FetchRandomWord();
        CurrentGuess = 0;

        WordGrid.Children.Clear();
        InitializeWordGrid();

        GuessInput.Text = string.Empty;
        GuessInput.IsEnabled = true;   
    }

    private void InitializeWordGrid()
    {
        WordGrid.RowDefinitions.Clear();
        WordGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < MaxGuesses; i++)
        {
            WordGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        for (int j = 0; j < WordLength; j++)
        {
            WordGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        for (int row = 0; row < MaxGuesses; row++)
        {
            for (int col = 0; col < WordLength; col++)
            {
                Label cell = new Label
                {
                    BackgroundColor = Colors.LightGray,
                    TextColor = Colors.Black,
                    FontSize = 24,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    WidthRequest = 50,
                    HeightRequest = 50,
                    Margin = new Thickness(2)
                };
                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);
                WordGrid.Children.Add(cell);
            }
        }
    }

    private void OnGuessInputChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue?.Length > WordLength)
        {
            GuessInput.Text = e.NewTextValue.Substring(0, WordLength).ToUpper();
        }
        else
        {
            GuessInput.Text = e.NewTextValue?.ToUpper();
        }
    }

    private async void OnSubmitGuessClicked(object sender, EventArgs e)
    {
        string guess = GuessInput.Text;

        if (string.IsNullOrWhiteSpace(guess) || guess.Length != WordLength || !await IsValidWord(guess))
        {
            await DisplayAlert("Invalid Guess", "Please enter a valid 5-letter word.", "OK");
            return;
        }

        var rowChildren = WordGrid.Children
            .Where(c => Grid.GetRow(c as View) == CurrentGuess)
            .OfType<Label>()
            .ToList();

        for (int i = 0; i < WordLength; i++)
        {
            rowChildren[i].Text = guess[i].ToString();

            if (guess[i] == TargetWord[i])
            {
                rowChildren[i].BackgroundColor = Colors.Green;
            }
            else if (TargetWord.Contains(guess[i]))
            {
                rowChildren[i].BackgroundColor = Colors.Yellow;
            }
            else
            {
                rowChildren[i].BackgroundColor = Colors.Gray;
            }
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
            var response = await HttpClient.GetAsync("https://random-word-api.herokuapp.com/word?length=5");
            if (response.IsSuccessStatusCode)
            {
                var words = await response.Content.ReadAsStringAsync();
              
                return words.Trim('[', ']', '"');
            }
        }
        catch
        {
           
            return "APPLE";
        }

        return "APPLE";
    }

    private async Task<bool> IsValidWord(string word)
    {
        try
        {
            var response = await HttpClient.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false; 
        }
    }
}
