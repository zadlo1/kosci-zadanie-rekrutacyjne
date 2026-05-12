using GraWKosci.Models;
using GraWKosci.UI;

namespace GraWKosci.Services;

public class GameEngine
{
    private readonly GameState _state;
    private readonly DiceRoller _roller;
    private readonly bool _showHints;

    public GameEngine(GameState state, DiceRoller roller, bool showHints)
    {
        _state = state;
        _roller = roller;
        _showHints = showHints;
    }

    public void Run()
    {
        while (!_state.IsGameFinished)
        {
            var player = _state.CurrentPlayer;

            if (!player.ScoreCard.IsComplete)
                PlayTurn(player);

            NextPlayer();
        }

        ShowResults();
    }

    private void PlayTurn(Player player)
    {
        var dice = _roller.RollAll();
        int rollsLeft = 2;

        while (true)
        {
            Console.WriteLine($"Rzut: {string.Join(" ", dice.Select(d => d.Value))}");

            if (_showHints && rollsLeft == 0)
            {
                ConsoleRenderer.RenderScoreCard(player, dice);
            }

            if (rollsLeft == 0)
                break;

            Console.WriteLine("Zatrzymać kości? (np. 1 3 5):");
            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                var keep = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .Select(int.Parse)
                                .ToHashSet();

                for (int i = 0; i < dice.Length; i++)
                    dice[i].IsHeld = keep.Contains(i + 1);
            }

            _roller.RollUnheld(dice);
            rollsLeft--;
        }

        ChooseCategory(player, dice);
    }

    private void ChooseCategory(Player player, Dice[] dice)
    {
        Console.WriteLine("\nWybierz kategorię:");

        var input = Console.ReadLine();

        if (!Enum.TryParse(input, out ScoreCategory category))
        {
            Console.WriteLine("Błędna kategoria - pomijam turę.");
            return;
        }

        int score = ScoreCalculator.Calculate(category, dice);
        ScoreCalculator.Apply(player.ScoreCard, category, score);

        Console.WriteLine($"Dodano: {score} pkt\n");
    }

    private void NextPlayer()
    {
        _state.CurrentPlayerIndex =
            (_state.CurrentPlayerIndex + 1) % _state.Players.Count;
    }

    private void ShowResults()
    {
        Console.WriteLine("\n=== WYNIKI ===");

        var ranking = _state.Players
            .OrderByDescending(p => p.ScoreCard.TotalScore);

        int i = 1;
        foreach (var p in ranking)
            Console.WriteLine($"{i++}. {p.Name} - {p.ScoreCard.TotalScore}");
    }
}