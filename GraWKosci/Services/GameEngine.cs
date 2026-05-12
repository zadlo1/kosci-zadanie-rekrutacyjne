using GraWKosci.Models;
using GraWKosci.UI;

namespace GraWKosci.Services;

public class GameEngine
{
    private readonly GameState _state;
    private readonly DiceRoller _roller;
    private readonly AiPlayer _ai = new();

    public GameEngine(GameState state, DiceRoller roller)
    {
        _state = state;
        _roller = roller;
    }

    public void Run()
    {
        while (!_state.IsGameFinished)
        {
            var player = _state.CurrentPlayer;

            if (!player.ScoreCard.IsComplete)
            {
                if (player.IsAi)
                    PlayAiTurn(player);
                else
                    PlayHumanTurn(player);
            }

            NextPlayer();
        }

        ShowResults();
    }

    private void PlayHumanTurn(Player player)
    {
        Console.WriteLine($"\n=== Tura gracza: {player.Name} ===");
        var dice = _roller.RollAll();
        int rollsLeft = 2;

        while (true)
        {
            ConsoleRenderer.RenderDice(dice);

            if (rollsLeft == 0)
            {
                ConsoleRenderer.RenderScoreCard(player, dice);
                break;
            }

            Console.WriteLine($"Pozostałe rzuty: {rollsLeft}");
            Console.WriteLine("Zatrzymać kości? Podaj numery (np. 1 3 5) lub Enter, aby rzucić wszystkimi:");
            var input = Console.ReadLine();

            foreach (var d in dice) d.IsHeld = false;

            if (!string.IsNullOrWhiteSpace(input))
            {
                var keep = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .Where(s => int.TryParse(s, out _))
                                .Select(int.Parse)
                                .ToHashSet();

                for (int i = 0; i < dice.Length; i++)
                    dice[i].IsHeld = keep.Contains(i + 1);
            }

            _roller.RollUnheld(dice);
            rollsLeft--;
        }

        ChooseCategoryHuman(player, dice);
    }

    private void ChooseCategoryHuman(Player player, Dice[] dice)
    {
        while (true)
        {
            Console.WriteLine("Wybierz kategorię (wpisz nazwę):");
            var input = Console.ReadLine()?.Trim();

            if (!Enum.TryParse(input, ignoreCase: true, out ScoreCategory category))
            {
                Console.WriteLine("Nieznana kategoria, spróbuj ponownie.");
                continue;
            }

            if (player.ScoreCard.IsUsed(category))
            {
                Console.WriteLine("Ta kategoria jest już użyta, wybierz inną.");
                continue;
            }

            int score = ScoreCalculator.Calculate(category, dice);
            ScoreCalculator.Apply(player.ScoreCard, category, score);
            Console.WriteLine($"Zapisano: {category} -> {score} pkt");
            break;
        }
    }

    private void PlayAiTurn(Player player)
    {
        Console.WriteLine($"\n=== Tura AI: {player.Name} ===");
        Thread.Sleep(400);

        var dice = _roller.RollAll();
        int rollsLeft = 2;

        while (true)
        {
            ConsoleRenderer.RenderDice(dice);

            if (rollsLeft == 0)
                break;

            var holdDecision = _ai.ChooseDiceToHold(dice, rollsLeft, player.ScoreCard);
            for (int i = 0; i < 5; i++)
                dice[i].IsHeld = holdDecision[i];

            var heldNums = Enumerable.Range(0, 5)
                .Where(i => holdDecision[i])
                .Select(i => (i + 1).ToString());

            string heldStr = string.Join(" ", heldNums);
            Console.WriteLine($"AI zatrzymuje kości: [{(heldStr.Length > 0 ? heldStr : "zadnej")}]");

            Thread.Sleep(600);
            _roller.RollUnheld(dice);
            rollsLeft--;
        }

        var chosenCategory = _ai.ChooseCategory(dice, player.ScoreCard);
        int score = ScoreCalculator.Calculate(chosenCategory, dice);
        ScoreCalculator.Apply(player.ScoreCard, chosenCategory, score);

        Console.WriteLine($"AI wybiera kategorie: {chosenCategory} -> {score} pkt");
    }

    private void NextPlayer()
    {
        _state.CurrentPlayerIndex =
            (_state.CurrentPlayerIndex + 1) % _state.Players.Count;
    }

    private void ShowResults()
    {
        Console.WriteLine("\n==========================");
        Console.WriteLine("       KONIEC GRY         ");
        Console.WriteLine("==========================\n");

        var ranking = _state.Players
            .OrderByDescending(p => p.ScoreCard.TotalScore)
            .ToList();

        int i = 1;
        foreach (var p in ranking)
        {
            string badge = i == 1 ? " <<< ZWYCIEZCA" : "";
            Console.WriteLine($"{i++}. {p.Name}{(p.IsAi ? " [AI]" : "")} - {p.ScoreCard.TotalScore} pkt{badge}");
        }
    }
}