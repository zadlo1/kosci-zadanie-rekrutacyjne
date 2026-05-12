using GraWKosci.Models;

namespace GraWKosci.Services;

public class DiceRoller
{
    private readonly Random _random = new();

    public Dice[] RollAll()
    {
        return Enumerable.Range(0, 5)
            .Select(_ => new Dice { Value = Roll(), IsHeld = false })
            .ToArray();
    }

    public void RollUnheld(Dice[] dice)
    {
        foreach (var d in dice)
        {
            if (!d.IsHeld)
                d.Value = Roll();
        }
    }

    private int Roll() => _random.Next(1, 7);
}