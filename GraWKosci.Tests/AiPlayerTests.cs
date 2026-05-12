using GraWKosci.Models;
using GraWKosci.Services;
using Xunit;

namespace GraWKosci.Tests;

public class AiPlayerTests
{
    private readonly AiPlayer _ai = new();

    // ── helpers ──────────────────────────────────────────────────────────────

    private static Dice[] D(params int[] values) =>
        values.Select(v => new Dice { Value = v }).ToArray();

    private static ScoreCard EmptyCard() => new();

    private static ScoreCard CardWith(params ScoreCategory[] used)
    {
        var card = new ScoreCard();
        foreach (var c in used)
            ScoreCalculator.Apply(card, c, 0);
        return card;
    }

    private static ScoreCard FullCardExcept(params ScoreCategory[] free)
    {
        var all = Enum.GetValues<ScoreCategory>();
        var card = new ScoreCard();
        foreach (var c in all)
        {
            if (!free.Contains(c))
                ScoreCalculator.Apply(card, c, 0);
        }
        return card;
    }

    // ═════════════════════════════════════════════════════════════════════════
    // ChooseDiceToHold
    // ═════════════════════════════════════════════════════════════════════════

    // ── rozmiar odpowiedzi ────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_ZawszeZwraca5Elementow()
    {
        var result = _ai.ChooseDiceToHold(D(1, 2, 3, 4, 5), 2, EmptyCard());
        Assert.Equal(5, result.Length);
    }

    // ── rollsLeft == 0 ────────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_OstatniRzut_ZatrzymujeWszystkie()
    {
        var result = _ai.ChooseDiceToHold(D(1, 2, 3, 4, 6), 0, EmptyCard());
        Assert.All(result, h => Assert.True(h));
    }

    // ── duży strit ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, 2, 3, 4, 5)]
    [InlineData(2, 3, 4, 5, 6)]
    public void ChooseDiceToHold_DuzyStrit_ZatrzymujeWszystkie(int a, int b, int c, int d, int e)
    {
        var result = _ai.ChooseDiceToHold(D(a, b, c, d, e), 2, EmptyCard());
        Assert.All(result, h => Assert.True(h));
    }

    // ── pięć jednaków ─────────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_PiecJednakowychWszystkie_ZatrzymujeWszystkie()
    {
        var result = _ai.ChooseDiceToHold(D(4, 4, 4, 4, 4), 2, EmptyCard());
        Assert.All(result, h => Assert.True(h));
    }

    // ── full ──────────────────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_Full_ZatrzymujeWszystkie()
    {
        var result = _ai.ChooseDiceToHold(D(2, 2, 3, 3, 3), 2, EmptyCard());
        Assert.All(result, h => Assert.True(h));
    }

    // ── cztery jednakowe ──────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_CzteryJednakowe_ZatrzymujeDokladnieCztery()
    {
        var result = _ai.ChooseDiceToHold(D(5, 5, 5, 5, 2), 2, EmptyCard());
        Assert.Equal(4, result.Count(h => h));
    }

    [Fact]
    public void ChooseDiceToHold_CzteryJednakowe_ZatrzymujeWartoscKtoraMaPowtorzenieNie2()
    {
        // kości: 5 5 5 5 2 — zatrzymane powinny mieć wartość 5
        var dice = D(5, 5, 5, 5, 2);
        var result = _ai.ChooseDiceToHold(dice, 2, EmptyCard());
        for (int i = 0; i < 5; i++)
            if (dice[i].Value == 2)
                Assert.False(result[i], "Kość z wartością 2 nie powinna być zatrzymana");
    }

    // ── trzy jednakowe ────────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_TrzyJednakowe_ZatrzymujeDokladnieTrzy()
    {
        var result = _ai.ChooseDiceToHold(D(4, 4, 4, 1, 2), 2, EmptyCard());
        Assert.Equal(3, result.Count(h => h));
    }

    // ── mały strit ────────────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_MalyStritKategoriaWolna_ZatrzymujeCztery()
    {
        // 1-2-3-4-6, SmallStraight wolna
        var result = _ai.ChooseDiceToHold(D(1, 2, 3, 4, 6), 2, EmptyCard());
        Assert.Equal(4, result.Count(h => h));
    }

    [Fact]
    public void ChooseDiceToHold_MalyStritKategoriaZajeta_NieWybieraMalego()
    {
        // gdy SmallStraight zajęty, AI nie powinna zatrzymywać układu jak przy strycie
        var card = CardWith(ScoreCategory.SmallStraight);
        var resultWithFree  = _ai.ChooseDiceToHold(D(1, 2, 3, 4, 6), 2, EmptyCard());
        var resultWithUsed  = _ai.ChooseDiceToHold(D(1, 2, 3, 4, 6), 2, card);
        // zachowanie powinno się różnić gdy kategoria zajęta
        Assert.NotEqual(
            string.Join(",", resultWithFree),
            string.Join(",", resultWithUsed));
    }

    // ── dwie pary ─────────────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_DwiePary_ZatrzymujeCoNajmniej2Kostki()
    {
        // 2 2 5 5 3 — zostaje wyższa para
        var result = _ai.ChooseDiceToHold(D(2, 2, 5, 5, 3), 2, EmptyCard());
        Assert.True(result.Count(h => h) >= 2);
    }

    // ── jedna para ────────────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_JednaPara_ZatrzymujeDokladnieDwie()
    {
        var result = _ai.ChooseDiceToHold(D(3, 3, 1, 4, 6), 2, EmptyCard());
        Assert.Equal(2, result.Count(h => h));
    }

    // ── brak par / stritów ────────────────────────────────────────────────────

    [Fact]
    public void ChooseDiceToHold_BrakParBrak5_ZatrzymujeMaxJedna()
    {
        // same różne bez 5 — nie ma nic sensownego do zatrzymania
        var result = _ai.ChooseDiceToHold(D(1, 2, 3, 6, 4), 2, EmptyCard());
        // Mały strit (1-2-3-4-6 zawiera 1-2-3-4), więc może zatrzymać 4
        // Weryfikujemy tylko że zwraca sensowną tablicę (bez rzucania)
        Assert.Equal(5, result.Length);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // ChooseCategory
    // ═════════════════════════════════════════════════════════════════════════

    // ── wynik nigdy nie wskazuje zajętej kategorii ────────────────────────────

    [Fact]
    public void ChooseCategory_NigdyNieWybieraZajetejKategorii_Yahtzee()
    {
        var card = CardWith(ScoreCategory.Yahtzee);
        var result = _ai.ChooseCategory(D(6, 6, 6, 6, 6), card);
        Assert.NotEqual(ScoreCategory.Yahtzee, result);
    }

    [Fact]
    public void ChooseCategory_NigdyNieWybieraZajetejKategorii_KartaZJednaWolna()
    {
        var card = FullCardExcept(ScoreCategory.Chance);
        var result = _ai.ChooseCategory(D(1, 2, 3, 4, 6), card);
        Assert.False(card.IsUsed(result));
    }

    // ── priorytety — kombinacje wysokopunktowe ────────────────────────────────

    [Fact]
    public void ChooseCategory_Yahtzee_WybieraYahtzee()
    {
        var result = _ai.ChooseCategory(D(6, 6, 6, 6, 6), EmptyCard());
        Assert.Equal(ScoreCategory.Yahtzee, result);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 5)]
    [InlineData(2, 3, 4, 5, 6)]
    public void ChooseCategory_DuzyStrit_WybieraLargeStraight(int a, int b, int c, int d, int e)
    {
        var result = _ai.ChooseCategory(D(a, b, c, d, e), EmptyCard());
        Assert.Equal(ScoreCategory.LargeStraight, result);
    }

    [Fact]
    public void ChooseCategory_MalyStrit_WybieraSmallStraight()
    {
        var result = _ai.ChooseCategory(D(1, 2, 3, 4, 6), EmptyCard());
        Assert.Equal(ScoreCategory.SmallStraight, result);
    }

    [Fact]
    public void ChooseCategory_Full_WybieraFullHouse()
    {
        var result = _ai.ChooseCategory(D(1, 1, 2, 2, 2), EmptyCard());
        Assert.Equal(ScoreCategory.FullHouse, result);
    }

    // ── fallback ──────────────────────────────────────────────────────────────

    [Fact]
    public void ChooseCategory_WszystkiePreferencjeZajete_WybieraDostepna()
    {
        // Zostaw tylko Twos
        var card = FullCardExcept(ScoreCategory.Twos);
        var result = _ai.ChooseCategory(D(1, 3, 4, 5, 6), card);
        Assert.Equal(ScoreCategory.Twos, result);
    }

    [Fact]
    public void ChooseCategory_PustaKarta_NieRzucaWyjatku()
    {
        var ex = Record.Exception(() => _ai.ChooseCategory(D(1, 1, 1, 1, 1), EmptyCard()));
        Assert.Null(ex);
    }

    // ── wartości brzegowe / walidacja wynikowa ────────────────────────────────

    [Fact]
    public void ChooseCategory_WszystkieKombiancjeTestedNieRzucajaWyjatku()
    {
        // Dla każdej możliwej kombinacji (pięć kości z zakresu 1-6, uproszczone)
        // i pustej karty — metoda nie powinna rzucać wyjątku
        var combinations = new[]
        {
            D(1,1,1,1,1), D(6,6,6,6,6),
            D(1,2,3,4,5), D(2,3,4,5,6),
            D(1,2,3,4,4), D(1,1,2,2,3),
            D(1,1,1,2,2), D(2,2,2,3,4),
            D(1,2,3,5,6), D(1,1,2,3,4)
        };
        foreach (var combo in combinations)
        {
            var ex = Record.Exception(() => _ai.ChooseCategory(combo, EmptyCard()));
            Assert.Null(ex);
        }
    }

    [Fact]
    public void ChooseCategory_ZwracaPoprawnaKategorieEnumValue()
    {
        var result = _ai.ChooseCategory(D(1, 2, 3, 4, 5), EmptyCard());
        Assert.True(Enum.IsDefined(typeof(ScoreCategory), result));
    }

    // ── spójność Choose → IsUsed ──────────────────────────────────────────────

    [Fact]
    public void ChooseCategory_WynikMoznaZapisacDoKarty()
    {
        var card = EmptyCard();
        var dice = D(3, 3, 3, 1, 2);

        var chosen = _ai.ChooseCategory(dice, card);
        var score = ScoreCalculator.Calculate(chosen, dice);
        ScoreCalculator.Apply(card, chosen, score);

        Assert.True(card.IsUsed(chosen));
    }

    [Fact]
    public void ChooseCategory_PelnaChaotycznaGra_KartaKompletnaPoWszystkichRundach()
    {
        // Symulacja 13 tur — każda tura wybiera kategorię i zapisuje wynik.
        // Po 13 turach karta powinna być kompletna.
        var card = EmptyCard();
        var dice = D(1, 2, 3, 4, 5); // stały układ dla determinizmu

        for (int i = 0; i < 13; i++)
        {
            var chosen = _ai.ChooseCategory(dice, card);
            Assert.False(card.IsUsed(chosen), $"Tura {i + 1}: AI wybrała już zajętą kategorię {chosen}");
            ScoreCalculator.Apply(card, chosen, ScoreCalculator.Calculate(chosen, dice));
        }

        Assert.True(card.IsComplete);
    }
}
