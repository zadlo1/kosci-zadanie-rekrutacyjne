using GraWKosci.Models;
using GraWKosci.Services;
using Xunit;

namespace GraWKosci.Tests;

public class ScoreCardTests
{
    // ── UpperSectionTotal ────────────────────────────────────────────────────

    [Fact]
    public void UpperSectionTotal_PustaKarta_Zwraca0()
    {
        var card = new ScoreCard();
        Assert.Equal(0, card.UpperSectionTotal);
    }

    [Fact]
    public void UpperSectionTotal_WszystkieWypelnione_ZwracaPoprawnaSume()
    {
        var card = new ScoreCard
        {
            Ones   = 3,
            Twos   = 6,
            Threes = 9,
            Fours  = 12,
            Fives  = 15,
            Sixes  = 18
        };
        Assert.Equal(63, card.UpperSectionTotal);
    }

    [Fact]
    public void UpperSectionTotal_CzesciowoWypelniona_NullTraktowaneJako0()
    {
        var card = new ScoreCard { Ones = 5, Threes = 9 };
        Assert.Equal(14, card.UpperSectionTotal);
    }

    [Fact]
    public void UpperSectionTotal_ZapisBrzegowy0_NieWliczaNulla()
    {
        // Ones = 0 (zapisano 0 pkt) vs Twos = null (niezapisane) — oba dają 0 w sumie
        var card = new ScoreCard { Ones = 0 };
        Assert.Equal(0, card.UpperSectionTotal);
    }

    // ── UpperBonus ───────────────────────────────────────────────────────────

    [Fact]
    public void UpperBonus_SumaPonizej63_Zwraca0()
    {
        var card = new ScoreCard { Ones = 1, Twos = 2 };
        Assert.Equal(0, card.UpperBonus);
    }

    [Fact]
    public void UpperBonus_SumaRowna63_Zwraca35()
    {
        var card = new ScoreCard
        {
            Ones = 3, Twos = 6, Threes = 9, Fours = 12, Fives = 15, Sixes = 18
        };
        Assert.Equal(35, card.UpperBonus);
    }

    [Fact]
    public void UpperBonus_SumaPowyżej63_Zwraca35()
    {
        var card = new ScoreCard
        {
            Ones = 5, Twos = 10, Threes = 9, Fours = 12, Fives = 15, Sixes = 18
        };
        Assert.True(card.UpperSectionTotal > 63);
        Assert.Equal(35, card.UpperBonus);
    }

    [Fact]
    public void UpperBonus_BonusAppliedTrue_Zwraca0NawetGdySuma63()
    {
        var card = new ScoreCard
        {
            Ones = 3, Twos = 6, Threes = 9, Fours = 12, Fives = 15, Sixes = 18,
            BonusApplied = true
        };
        Assert.Equal(0, card.UpperBonus);
    }

    [Fact]
    public void UpperBonus_Prog62_BrakPremii()
    {
        // Wartość graniczna: 62 pkt — brak premii
        var card = new ScoreCard
        {
            Ones = 2, Twos = 6, Threes = 9, Fours = 12, Fives = 15, Sixes = 18
        };
        Assert.Equal(62, card.UpperSectionTotal);
        Assert.Equal(0, card.UpperBonus);
    }

    // ── LowerSectionTotal ────────────────────────────────────────────────────

    [Fact]
    public void LowerSectionTotal_PustaKarta_Zwraca0()
    {
        Assert.Equal(0, new ScoreCard().LowerSectionTotal);
    }

    [Fact]
    public void LowerSectionTotal_WszystkieWypelnione_ZwracaPoprawnaSume()
    {
        var card = new ScoreCard
        {
            ThreeOfAKind = 20,
            FourOfAKind  = 24,
            FullHouse    = 25,
            SmallStraight= 30,
            LargeStraight= 40,
            Yahtzee      = 50,
            Chance       = 17
        };
        Assert.Equal(206, card.LowerSectionTotal);
    }

    // ── TotalScore ───────────────────────────────────────────────────────────

    [Fact]
    public void TotalScore_PustaKarta_Zwraca0()
    {
        Assert.Equal(0, new ScoreCard().TotalScore);
    }

    [Fact]
    public void TotalScore_ZPremia_PoprawnaArytmetyka()
    {
        var card = new ScoreCard
        {
            Ones = 3, Twos = 6, Threes = 9, Fours = 12, Fives = 15, Sixes = 18,
            Yahtzee = 50
        };
        // 63 (górna) + 35 (premia) + 50 (dolna) = 148
        Assert.Equal(148, card.TotalScore);
    }

    [Fact]
    public void TotalScore_BezPremii_PoprawnaArytmetyka()
    {
        var card = new ScoreCard { Ones = 1, Chance = 10 };
        Assert.Equal(11, card.TotalScore);
    }

    // ── IsComplete ───────────────────────────────────────────────────────────

    [Fact]
    public void IsComplete_PustaKarta_FalseReturn()
    {
        Assert.False(new ScoreCard().IsComplete);
    }

    [Fact]
    public void IsComplete_12z13Kategorii_FalseReturn()
    {
        var card = new ScoreCard
        {
            Ones = 0, Twos = 0, Threes = 0, Fours = 0, Fives = 0, Sixes = 0,
            ThreeOfAKind = 0, FourOfAKind = 0, FullHouse = 0,
            SmallStraight = 0, LargeStraight = 0, Yahtzee = 0
            // Chance brakuje
        };
        Assert.False(card.IsComplete);
    }

    [Fact]
    public void IsComplete_Wszystkie13Kategorii_TrueReturn()
    {
        var card = new ScoreCard
        {
            Ones = 0, Twos = 0, Threes = 0, Fours = 0, Fives = 0, Sixes = 0,
            ThreeOfAKind = 0, FourOfAKind = 0, FullHouse = 0,
            SmallStraight = 0, LargeStraight = 0, Yahtzee = 0, Chance = 0
        };
        Assert.True(card.IsComplete);
    }

    [Fact]
    public void IsComplete_KategorieZWartoscia0SaWypelnione()
    {
        // Wartość 0 to prawidłowy wynik — pole jest zajęte
        var card = new ScoreCard
        {
            Ones = 0, Twos = 0, Threes = 0, Fours = 0, Fives = 0, Sixes = 0,
            ThreeOfAKind = 0, FourOfAKind = 0, FullHouse = 0,
            SmallStraight = 0, LargeStraight = 0, Yahtzee = 0, Chance = 0
        };
        Assert.True(card.IsComplete);
    }

    // ── IsUsed ───────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(ScoreCategory.Ones)]
    [InlineData(ScoreCategory.Twos)]
    [InlineData(ScoreCategory.Threes)]
    [InlineData(ScoreCategory.Fours)]
    [InlineData(ScoreCategory.Fives)]
    [InlineData(ScoreCategory.Sixes)]
    [InlineData(ScoreCategory.ThreeOfAKind)]
    [InlineData(ScoreCategory.FourOfAKind)]
    [InlineData(ScoreCategory.FullHouse)]
    [InlineData(ScoreCategory.SmallStraight)]
    [InlineData(ScoreCategory.LargeStraight)]
    [InlineData(ScoreCategory.Yahtzee)]
    [InlineData(ScoreCategory.Chance)]
    public void IsUsed_KategoriaNiezapisana_FalseReturn(ScoreCategory category)
    {
        Assert.False(new ScoreCard().IsUsed(category));
    }

    [Theory]
    [InlineData(ScoreCategory.Ones)]
    [InlineData(ScoreCategory.Yahtzee)]
    [InlineData(ScoreCategory.Chance)]
    public void IsUsed_KategoriaZapisanaZWartosciaPozytywna_TrueReturn(ScoreCategory category)
    {
        var card = new ScoreCard();
        ScoreCalculator.Apply(card, category, 10);
        Assert.True(card.IsUsed(category));
    }

    [Theory]
    [InlineData(ScoreCategory.Ones)]
    [InlineData(ScoreCategory.FullHouse)]
    [InlineData(ScoreCategory.Yahtzee)]
    public void IsUsed_KategoriaZapisanaZ0_TrueReturn(ScoreCategory category)
    {
        var card = new ScoreCard();
        ScoreCalculator.Apply(card, category, 0);
        Assert.True(card.IsUsed(category));
    }
}
