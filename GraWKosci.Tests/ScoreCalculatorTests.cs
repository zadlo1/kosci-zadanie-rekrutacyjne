using GraWKosci.Models;
using GraWKosci.Services;
using Xunit;

namespace GraWKosci.Tests;

public class ScoreCalculatorTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private static Dice[] D(params int[] values) =>
        values.Select(v => new Dice { Value = v }).ToArray();

    // ── SEKCJA GÓRNA ─────────────────────────────────────────────────────────

    [Fact]
    public void Ones_BrakJedynek_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.Ones, D(2, 3, 4, 5, 6)));
    }

    [Fact]
    public void Ones_WszystkieJedynki_Zwraca5()
    {
        Assert.Equal(5, ScoreCalculator.Calculate(ScoreCategory.Ones, D(1, 1, 1, 1, 1)));
    }

    [Fact]
    public void Ones_KilkaJedynek_ZwracaSume()
    {
        Assert.Equal(2, ScoreCalculator.Calculate(ScoreCategory.Ones, D(1, 2, 1, 3, 4)));
    }

    [Fact]
    public void Twos_KilkaDwojek_ZwracaSume()
    {
        Assert.Equal(4, ScoreCalculator.Calculate(ScoreCategory.Twos, D(2, 2, 3, 4, 5)));
    }

    [Fact]
    public void Twos_BrakDwojek_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.Twos, D(1, 1, 3, 4, 5)));
    }

    [Fact]
    public void Threes_KilkaTrojek_ZwracaSume()
    {
        Assert.Equal(9, ScoreCalculator.Calculate(ScoreCategory.Threes, D(3, 3, 3, 1, 2)));
    }

    [Fact]
    public void Fours_KilkaCzworek_ZwracaSume()
    {
        Assert.Equal(16, ScoreCalculator.Calculate(ScoreCategory.Fours, D(4, 4, 4, 4, 1)));
    }

    [Fact]
    public void Fives_KilkaPiatek_ZwracaSume()
    {
        Assert.Equal(10, ScoreCalculator.Calculate(ScoreCategory.Fives, D(5, 5, 1, 2, 3)));
    }

    [Fact]
    public void Sixes_WszystkieSzostki_Zwraca30()
    {
        Assert.Equal(30, ScoreCalculator.Calculate(ScoreCategory.Sixes, D(6, 6, 6, 6, 6)));
    }

    [Fact]
    public void Sixes_BrakSzostek_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.Sixes, D(1, 2, 3, 4, 5)));
    }

    // ── TRÓJKA (ThreeOfAKind) ────────────────────────────────────────────────

    [Fact]
    public void ThreeOfAKind_DokladnieTrzy_ZwracaSumeWszystkich()
    {
        Assert.Equal(12, ScoreCalculator.Calculate(ScoreCategory.ThreeOfAKind, D(3, 3, 3, 1, 2)));
    }

    [Fact]
    public void ThreeOfAKind_CzteryJednakowe_SpelniaWarunekTrojki()
    {
        Assert.Equal(17, ScoreCalculator.Calculate(ScoreCategory.ThreeOfAKind, D(4, 4, 4, 4, 1)));
    }

    [Fact]
    public void ThreeOfAKind_PiecJednakowychSpelniaWarunek()
    {
        Assert.Equal(30, ScoreCalculator.Calculate(ScoreCategory.ThreeOfAKind, D(6, 6, 6, 6, 6)));
    }

    [Fact]
    public void ThreeOfAKind_DwiePary_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.ThreeOfAKind, D(2, 2, 3, 3, 4)));
    }

    [Fact]
    public void ThreeOfAKind_WszystkieRozne_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.ThreeOfAKind, D(1, 2, 3, 4, 5)));
    }

    [Fact]
    public void ThreeOfAKind_JednaPara_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.ThreeOfAKind, D(1, 1, 2, 3, 4)));
    }

    // ── CZWÓRKA (FourOfAKind) ────────────────────────────────────────────────

    [Fact]
    public void FourOfAKind_DokladnieCztery_ZwracaSumeWszystkich()
    {
        Assert.Equal(22, ScoreCalculator.Calculate(ScoreCategory.FourOfAKind, D(5, 5, 5, 5, 2)));
    }

    [Fact]
    public void FourOfAKind_PiecJednakowychSpelniaWarunek()
    {
        Assert.Equal(15, ScoreCalculator.Calculate(ScoreCategory.FourOfAKind, D(3, 3, 3, 3, 3)));
    }

    [Fact]
    public void FourOfAKind_TrzyjJednakowe_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.FourOfAKind, D(2, 2, 2, 1, 3)));
    }

    [Fact]
    public void FourOfAKind_BrakPowtorzen_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.FourOfAKind, D(1, 2, 3, 4, 5)));
    }

    // ── FULL (FullHouse) ─────────────────────────────────────────────────────

    [Fact]
    public void FullHouse_TrojkaPlusPara_Zwraca25()
    {
        Assert.Equal(25, ScoreCalculator.Calculate(ScoreCategory.FullHouse, D(1, 1, 2, 2, 2)));
    }

    [Fact]
    public void FullHouse_ParaPlusTrojka_Zwraca25()
    {
        Assert.Equal(25, ScoreCalculator.Calculate(ScoreCategory.FullHouse, D(5, 5, 5, 6, 6)));
    }

    [Fact]
    public void FullHouse_PiecJednakowychNieJestFull_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.FullHouse, D(3, 3, 3, 3, 3)));
    }

    [Fact]
    public void FullHouse_CzteryJednakoweBrakFull_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.FullHouse, D(4, 4, 4, 4, 1)));
    }

    [Fact]
    public void FullHouse_TrzyRozneWartosci_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.FullHouse, D(1, 1, 2, 3, 3)));
    }

    [Fact]
    public void FullHouse_WszystkieRozne_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.FullHouse, D(1, 2, 3, 4, 5)));
    }

    // ── MAŁY STRIT (SmallStraight) ───────────────────────────────────────────

    [Theory]
    [InlineData(1, 2, 3, 4, 4)]   // 1-2-3-4 z duplikatem
    [InlineData(2, 3, 4, 5, 1)]   // 2-3-4-5
    [InlineData(3, 4, 5, 6, 2)]   // 3-4-5-6
    [InlineData(1, 2, 3, 4, 5)]   // duży strit zawiera mały
    [InlineData(2, 3, 4, 5, 6)]   // duży strit 2-3-4-5-6 zawiera 2-3-4-5
    public void SmallStraight_PoprawnaKombinacja_Zwraca30(int a, int b, int c, int d, int e)
    {
        Assert.Equal(30, ScoreCalculator.Calculate(ScoreCategory.SmallStraight, D(a, b, c, d, e)));
    }

    [Theory]
    [InlineData(1, 2, 3, 5, 6)]   // brak 4
    [InlineData(2, 2, 2, 2, 2)]   // piec jednakowach
    [InlineData(1, 1, 1, 1, 1)]   // same jedynki
    [InlineData(1, 3, 5, 2, 6)]   // brak 4 kolejnych
    public void SmallStraight_NiepoprawnaKombinacja_Zwraca0(int a, int b, int c, int d, int e)
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.SmallStraight, D(a, b, c, d, e)));
    }

    // ── DUŻY STRIT (LargeStraight) ───────────────────────────────────────────

    [Theory]
    [InlineData(1, 2, 3, 4, 5)]
    [InlineData(2, 3, 4, 5, 6)]
    [InlineData(5, 4, 3, 2, 1)]   // kolejność dowolna
    public void LargeStraight_PoprawnaKombinacja_Zwraca40(int a, int b, int c, int d, int e)
    {
        Assert.Equal(40, ScoreCalculator.Calculate(ScoreCategory.LargeStraight, D(a, b, c, d, e)));
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 4)]   // mały strit z duplikatem
    [InlineData(1, 2, 3, 4, 6)]   // brak 5
    [InlineData(1, 1, 1, 1, 1)]
    public void LargeStraight_NiepoprawnaKombinacja_Zwraca0(int a, int b, int c, int d, int e)
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.LargeStraight, D(a, b, c, d, e)));
    }

    // ── YAHTZEE ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(6)]
    public void Yahtzee_PiecJednakowychDowolnaWartosc_Zwraca50(int val)
    {
        Assert.Equal(50, ScoreCalculator.Calculate(ScoreCategory.Yahtzee, D(val, val, val, val, val)));
    }

    [Fact]
    public void Yahtzee_CzteryJednakowe_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.Yahtzee, D(5, 5, 5, 5, 1)));
    }

    [Fact]
    public void Yahtzee_BrakPowtorzen_Zwraca0()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(ScoreCategory.Yahtzee, D(1, 2, 3, 4, 5)));
    }

    // ── SZANSA (Chance) ──────────────────────────────────────────────────────

    [Fact]
    public void Chance_ZwracaSumeWszystkichKosci()
    {
        Assert.Equal(15, ScoreCalculator.Calculate(ScoreCategory.Chance, D(1, 2, 3, 4, 5)));
    }

    [Fact]
    public void Chance_MaksymalnaSuma()
    {
        Assert.Equal(30, ScoreCalculator.Calculate(ScoreCategory.Chance, D(6, 6, 6, 6, 6)));
    }

    [Fact]
    public void Chance_MinimalnaSuma()
    {
        Assert.Equal(5, ScoreCalculator.Calculate(ScoreCategory.Chance, D(1, 1, 1, 1, 1)));
    }

    // ── APPLY ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(ScoreCategory.Ones,         3)]
    [InlineData(ScoreCategory.Twos,         6)]
    [InlineData(ScoreCategory.Threes,       9)]
    [InlineData(ScoreCategory.Fours,        12)]
    [InlineData(ScoreCategory.Fives,        15)]
    [InlineData(ScoreCategory.Sixes,        18)]
    [InlineData(ScoreCategory.ThreeOfAKind, 20)]
    [InlineData(ScoreCategory.FourOfAKind,  24)]
    [InlineData(ScoreCategory.FullHouse,    25)]
    [InlineData(ScoreCategory.SmallStraight,30)]
    [InlineData(ScoreCategory.LargeStraight,40)]
    [InlineData(ScoreCategory.Yahtzee,      50)]
    [InlineData(ScoreCategory.Chance,       17)]
    public void Apply_KazdaKategoria_UstawiaWlasciwosc(ScoreCategory category, int value)
    {
        var card = new ScoreCard();
        ScoreCalculator.Apply(card, category, value);
        Assert.True(card.IsUsed(category));
    }

    [Fact]
    public void Apply_Ones_UstawiaPoprawnaWartosc()
    {
        var card = new ScoreCard();
        ScoreCalculator.Apply(card, ScoreCategory.Ones, 3);
        Assert.Equal(3, card.Ones);
    }

    [Fact]
    public void Apply_Yahtzee_UstawiaPoprawnaWartosc()
    {
        var card = new ScoreCard();
        ScoreCalculator.Apply(card, ScoreCategory.Yahtzee, 50);
        Assert.Equal(50, card.Yahtzee);
    }

    [Fact]
    public void Apply_Wynik0_KategoriaTraktowanaJakoUzyta()
    {
        // Gracz może zapisać 0 pkt — kategoria jest zajęta
        var card = new ScoreCard();
        ScoreCalculator.Apply(card, ScoreCategory.FullHouse, 0);
        Assert.True(card.IsUsed(ScoreCategory.FullHouse));
        Assert.Equal(0, card.FullHouse);
    }
}
