using GraWKosci.Models;

namespace GraWKosci.Services;

public class DiceRoller
{
    private readonly Random _random = new();

    public void Roll(Dice[] dices)
    {
        foreach (var dice in dices)
        {
            if (dice.IsHeld)
            {
                continue;
            }

            dice.Value = _random.Next(1, 7);
        }
    }
}