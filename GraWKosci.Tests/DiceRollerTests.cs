using GraWKosci.Models;
using GraWKosci.Services;
using Xunit;

namespace GraWKosci.Tests;

public class DiceRollerTests
{
    private readonly DiceRoller _roller = new();

    // ── RollAll ──────────────────────────────────────────────────────────────

    [Fact]
    public void RollAll_ZwracaDokladnie5Kosci()
    {
        var dice = _roller.RollAll();
        Assert.Equal(5, dice.Length);
    }

    [Fact]
    public void RollAll_WszystkieKosciNiezatrzymane()
    {
        var dice = _roller.RollAll();
        Assert.All(dice, d => Assert.False(d.IsHeld));
    }

    [Fact]
    public void RollAll_WartosciWZakresie1Do6()
    {
        var dice = _roller.RollAll();
        Assert.All(dice, d =>
        {
            Assert.True(d.Value >= 1);
            Assert.True(d.Value <= 6);
        });
    }

    [Fact]
    public void RollAll_WielokrotneWywolania_WartosciWZakresie()
    {
        for (int i = 0; i < 100; i++)
        {
            var dice = _roller.RollAll();
            Assert.All(dice, d =>
            {
                Assert.True(d.Value >= 1, $"Wartość {d.Value} poniżej 1");
                Assert.True(d.Value <= 6, $"Wartość {d.Value} powyżej 6");
            });
        }
    }

    [Fact]
    public void RollAll_ZwracaNoweTablice_NieZwracaReferencjiDoTejSamej()
    {
        var dice1 = _roller.RollAll();
        var dice2 = _roller.RollAll();
        Assert.NotSame(dice1, dice2);
    }

    // ── RollUnheld ───────────────────────────────────────────────────────────

    [Fact]
    public void RollUnheld_ZatrzymaneKosciNieZmieniajWartosci()
    {
        var dice = new[]
        {
            new Dice { Value = 6, IsHeld = true },
            new Dice { Value = 6, IsHeld = true },
            new Dice { Value = 6, IsHeld = true },
            new Dice { Value = 6, IsHeld = true },
            new Dice { Value = 6, IsHeld = true }
        };

        _roller.RollUnheld(dice);

        Assert.All(dice, d => Assert.Equal(6, d.Value));
    }

    [Fact]
    public void RollUnheld_NiezatrzymaneKosciMajaWartoscWZakresie()
    {
        var dice = new[]
        {
            new Dice { Value = 1, IsHeld = false },
            new Dice { Value = 1, IsHeld = false },
            new Dice { Value = 1, IsHeld = false },
            new Dice { Value = 1, IsHeld = false },
            new Dice { Value = 1, IsHeld = false }
        };

        for (int i = 0; i < 50; i++)
        {
            _roller.RollUnheld(dice);
            Assert.All(dice, d =>
            {
                Assert.True(d.Value >= 1);
                Assert.True(d.Value <= 6);
            });
            // reset
            foreach (var d in dice) d.Value = 1;
        }
    }

    [Fact]
    public void RollUnheld_CzescZatrzymana_CzescNie()
    {
        var dice = new[]
        {
            new Dice { Value = 6, IsHeld = true },
            new Dice { Value = 1, IsHeld = false },
            new Dice { Value = 6, IsHeld = true },
            new Dice { Value = 1, IsHeld = false },
            new Dice { Value = 6, IsHeld = true }
        };

        _roller.RollUnheld(dice);

        Assert.Equal(6, dice[0].Value);
        Assert.Equal(6, dice[2].Value);
        Assert.Equal(6, dice[4].Value);
        Assert.True(dice[1].Value >= 1 && dice[1].Value <= 6);
        Assert.True(dice[3].Value >= 1 && dice[3].Value <= 6);
    }

    [Fact]
    public void RollUnheld_WszystkieZatrzymane_ZadnaNieZmieniaSie()
    {
        var dice = Enumerable.Range(0, 5)
            .Select(i => new Dice { Value = i + 1, IsHeld = true })
            .ToArray();

        _roller.RollUnheld(dice);

        for (int i = 0; i < 5; i++)
            Assert.Equal(i + 1, dice[i].Value);
    }

    [Fact]
    public void RollUnheld_ZadnaZatrzymana_WszystkieOtrzymujaNowWartosci()
    {
        // Ustawiamy wszystkie kości na wartość niemożliwą do wyrzucenia (7),
        // by mieć pewność, że wartości faktycznie się zmieniły na poprawne.
        var dice = Enumerable.Range(0, 5)
            .Select(_ => new Dice { Value = 7, IsHeld = false })
            .ToArray();

        _roller.RollUnheld(dice);

        Assert.All(dice, d =>
        {
            Assert.True(d.Value >= 1);
            Assert.True(d.Value <= 6);
        });
    }
}
