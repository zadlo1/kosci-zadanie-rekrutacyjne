using GraWKosci.Models;
using GraWKosci.Services;

namespace GraWKosci.UI;

public static class ConsoleRenderer
{
    public static void RenderDice(Dice[] dices)
    {
        Console.WriteLine();

        for (var i = 0; i < dices.Length; i++)
        {
            var held = dices[i].IsHeld ? "*" : " ";

            Console.Write($"[{i + 1}:{dices[i].Value}{held}] ");
        }

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void RenderScoreCard(Player player, Dice[] dices)
    {
        Console.WriteLine("=== Dostępne Kategorie ===");

        var available = Enum
            .GetValues<ScoreCategory>()
            .Where(x => !player.ScoreCard.IsUsed(x))
            .ToList();

        for (var i = 0; i < available.Count; i++)
        {
            var category = available[i];
            var score = ScoreCalculator.Calculate(category, dices);

            Console.WriteLine($"{i + 1}. {category} -> {score} pkt");
        }

        Console.WriteLine();
    }
}