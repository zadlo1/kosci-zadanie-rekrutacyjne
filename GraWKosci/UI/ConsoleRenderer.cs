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
        Console.WriteLine("\n=== PODPOWIEDZI (MOŻLIWE WYNIKI) ===");

        var categories = Enum.GetValues<ScoreCategory>();

        foreach (var category in categories)
        {
            if (player.ScoreCard.IsUsed(category))
                continue;

            var score = ScoreCalculator.Calculate(category, dices);
            Console.WriteLine($"{category} -> {score} pkt");
        }

        Console.WriteLine();
    }
}