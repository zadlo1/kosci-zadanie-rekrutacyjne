using GraWKosci.Models;
using GraWKosci.UI;

namespace GraWKosci.Services;

public class GameEngine
{
    private readonly DiceRoller _diceRoller = new();

    private readonly List<Player> _players = [];

    public void Start()
    {
        SetupPlayers();
        
        while (!_players.All(x => x.ScoreCard.IsComplete))
        {
            foreach (var player in _players)
            {
                if (player.ScoreCard.IsComplete)
                {
                    continue;
                }

                PlayTurn(player);
            }
        }

        ShowResults();
    }

    private void SetupPlayers()
    {
        Console.Write("Podaj liczbę graczy (2-4): ");

        var playerCount = int.Parse(Console.ReadLine()!);
        
        for (var i = 1; i <= playerCount; i++)
        {
            Console.Write($"Nazwa gracza {i}: ");

            var name = Console.ReadLine()!;

            _players.Add(new Player(name));
        }
    }

    private void PlayTurn(Player player)
    {
        Console.Clear();

        Console.WriteLine($"=== Tura gracza: {player.Name} ===");

        var dices = CreateDices();

        for (var roll = 1; roll <= 3; roll++)
        {
            Console.WriteLine();
            Console.WriteLine($"Rzut {roll}/3");
            
            _diceRoller.Roll(dices);

            ConsoleRenderer.RenderDice(dices);

            if (roll < 3)
            {
                HandleHoldInput(dices);
            }
        }

        ChooseCategory(player, dices);

        Console.WriteLine();
        Console.WriteLine("Naciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    private Dice[] CreateDices()
    {
        return
        [
            new Dice(),
            new Dice(),
            new Dice(),
            new Dice(),
            new Dice()
        ];
    }

    private void HandleHoldInput(Dice[] dices)
    {
        Console.WriteLine("Które kości zatrzymać? (np. 1 3 5)");
        Console.Write("> ");

        var input = Console.ReadLine();

        foreach (var dice in dices)
        {
            dice.IsHeld = false;
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }
        
        var indexes = input
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

        foreach (var index in indexes)
        {
            dices[index - 1].IsHeld = true;
        }
    }

    private void ChooseCategory(Player player, Dice[] dices)
    {
        Console.WriteLine();

        ConsoleRenderer.RenderScoreCard(player, dices);

        var available = Enum
            .GetValues<ScoreCategory>()
            .Where(x => !player.ScoreCard.IsUsed(x))
            .ToList();
            
        Console.Write("Wybierz kategorię: ");

        var selectedIndex = int.Parse(Console.ReadLine()!) - 1;

        var category = available[selectedIndex];

        var score = ScoreCalculator.Calculate(category, dices);

        player.ScoreCard.SetScore(category, score);

        Console.WriteLine();
        Console.WriteLine($"Zapisano {score} pkt do kategorii {category}");
    }

    private void ShowResults()
    {
        Console.Clear();

        Console.WriteLine("=== KONIEC GRY ===");
        Console.WriteLine();

        var ranking = _players
            .OrderByDescending(x => x.ScoreCard.TotalScore)
            .ToList();
            
        for (var i = 0; i < ranking.Count; i++)
        {
            var player = ranking[i];

            Console.WriteLine(
                $"{i + 1}. {player.Name} - {player.ScoreCard.TotalScore} pkt");
        }

        Console.WriteLine();
    }
}