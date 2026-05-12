using GraWKosci.Models;
using GraWKosci.Services;
using Xunit;

namespace GraWKosci.Tests;

public class GameStateTests
{
    private static ScoreCard FullCard()
    {
        var card = new ScoreCard
        {
            Ones = 0, Twos = 0, Threes = 0, Fours = 0, Fives = 0, Sixes = 0,
            ThreeOfAKind = 0, FourOfAKind = 0, FullHouse = 0,
            SmallStraight = 0, LargeStraight = 0, Yahtzee = 0, Chance = 0
        };
        return card;
    }

    private static Player PlayerWithFullCard(string name) =>
        new Player(name) { };

    // ── CurrentPlayer ────────────────────────────────────────────────────────

    [Fact]
    public void CurrentPlayer_IndeksDomyslny0_ZwracaPierwszegoGracza()
    {
        var state = new GameState();
        var p1 = new Player("Ala");
        var p2 = new Player("Bob");
        state.Players.Add(p1);
        state.Players.Add(p2);

        Assert.Equal(p1, state.CurrentPlayer);
    }

    [Fact]
    public void CurrentPlayer_IndeksPrzesunienty_ZwracaWlasciwego()
    {
        var state = new GameState();
        var p1 = new Player("Ala");
        var p2 = new Player("Bob");
        state.Players.Add(p1);
        state.Players.Add(p2);

        state.CurrentPlayerIndex = 1;
        Assert.Equal(p2, state.CurrentPlayer);
    }

    // ── IsGameFinished ───────────────────────────────────────────────────────

    [Fact]
    public void IsGameFinished_WszyscyGraczeUkonczeni_TrueReturn()
    {
        var state = new GameState();

        foreach (var name in new[] { "Ala", "Bob" })
        {
            var p = new Player(name);
            FillCard(p.ScoreCard);
            state.Players.Add(p);
        }

        Assert.True(state.IsGameFinished);
    }

    [Fact]
    public void IsGameFinished_JedenGraczNieUkonczony_FalseReturn()
    {
        var state = new GameState();

        var p1 = new Player("Ala");
        FillCard(p1.ScoreCard);
        state.Players.Add(p1);

        state.Players.Add(new Player("Bob")); // pusta karta

        Assert.False(state.IsGameFinished);
    }

    [Fact]
    public void IsGameFinished_WszyscyGraczeZPustaKarta_FalseReturn()
    {
        var state = new GameState();
        state.Players.Add(new Player("Ala"));
        state.Players.Add(new Player("Bob"));

        Assert.False(state.IsGameFinished);
    }

    // ── Players ──────────────────────────────────────────────────────────────

    [Fact]
    public void Players_DomyslniePustaLista()
    {
        var state = new GameState();
        Assert.Empty(state.Players);
    }

    [Fact]
    public void Players_MoznaDeodacGraczy()
    {
        var state = new GameState();
        state.Players.Add(new Player("Ala"));
        state.Players.Add(new Player("Bob"));
        Assert.Equal(2, state.Players.Count);
    }

    // ── CurrentPlayerIndex ───────────────────────────────────────────────────

    [Fact]
    public void CurrentPlayerIndex_DomyslnieZero()
    {
        Assert.Equal(0, new GameState().CurrentPlayerIndex);
    }

    [Fact]
    public void CurrentPlayerIndex_MoznaZmienic()
    {
        var state = new GameState();
        state.Players.Add(new Player("A"));
        state.Players.Add(new Player("B"));
        state.CurrentPlayerIndex = 1;
        Assert.Equal(1, state.CurrentPlayerIndex);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static void FillCard(ScoreCard card)
    {
        card.Ones = 0; card.Twos = 0; card.Threes = 0;
        card.Fours = 0; card.Fives = 0; card.Sixes = 0;
        card.ThreeOfAKind = 0; card.FourOfAKind = 0; card.FullHouse = 0;
        card.SmallStraight = 0; card.LargeStraight = 0; card.Yahtzee = 0;
        card.Chance = 0;
    }
}
