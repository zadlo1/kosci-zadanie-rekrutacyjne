using GraWKosci.Models;

namespace GraWKosci.Services;

public static class ScoreCalculator
{
    public static int Calculate(ScoreCategory category, Dice[] dice)
    {
        var values = dice.Select(d => d.Value).ToArray();

        return category switch
        {
            ScoreCategory.Ones => values.Where(v => v == 1).Sum(),
            ScoreCategory.Twos => values.Where(v => v == 2).Sum(),
            ScoreCategory.Threes => values.Where(v => v == 3).Sum(),
            ScoreCategory.Fours => values.Where(v => v == 4).Sum(),
            ScoreCategory.Fives => values.Where(v => v == 5).Sum(),
            ScoreCategory.Sixes => values.Where(v => v == 6).Sum(),

            ScoreCategory.ThreeOfAKind =>
                HasOfAKind(values, 3) ? values.Sum() : 0,

            ScoreCategory.FourOfAKind =>
                HasOfAKind(values, 4) ? values.Sum() : 0,

            ScoreCategory.FullHouse =>
                IsFullHouse(values) ? 25 : 0,

            ScoreCategory.SmallStraight =>
                HasSmallStraight(values) ? 30 : 0,

            ScoreCategory.LargeStraight =>
                HasLargeStraight(values) ? 40 : 0,

            ScoreCategory.Yahtzee =>
                HasOfAKind(values, 5) ? 50 : 0,

            ScoreCategory.Chance =>
                values.Sum(),

            _ => 0
        };
    }

    public static void Apply(ScoreCard card, ScoreCategory category, int value)
    {
        switch (category)
        {
            case ScoreCategory.Ones: card.Ones = value; break;
            case ScoreCategory.Twos: card.Twos = value; break;
            case ScoreCategory.Threes: card.Threes = value; break;
            case ScoreCategory.Fours: card.Fours = value; break;
            case ScoreCategory.Fives: card.Fives = value; break;
            case ScoreCategory.Sixes: card.Sixes = value; break;

            case ScoreCategory.ThreeOfAKind: card.ThreeOfAKind = value; break;
            case ScoreCategory.FourOfAKind: card.FourOfAKind = value; break;
            case ScoreCategory.FullHouse: card.FullHouse = value; break;
            case ScoreCategory.SmallStraight: card.SmallStraight = value; break;
            case ScoreCategory.LargeStraight: card.LargeStraight = value; break;
            case ScoreCategory.Yahtzee: card.Yahtzee = value; break;
            case ScoreCategory.Chance: card.Chance = value; break;
        }
    }

    private static bool HasOfAKind(int[] values, int count) =>
        values.GroupBy(v => v).Any(g => g.Count() >= count);

    private static bool IsFullHouse(int[] values) =>
        values.GroupBy(v => v).Select(g => g.Count()).OrderBy(x => x)
        .SequenceEqual(new[] { 2, 3 });

    private static bool HasSmallStraight(int[] v)
    {
        var set = v.ToHashSet();
        return
            set.IsSupersetOf(new[] { 1, 2, 3, 4 }) ||
            set.IsSupersetOf(new[] { 2, 3, 4, 5 }) ||
            set.IsSupersetOf(new[] { 3, 4, 5, 6 });
    }

    private static bool HasLargeStraight(int[] v)
    {
        var set = v.ToHashSet();
        return
            set.SetEquals(new[] { 1, 2, 3, 4, 5 }) ||
            set.SetEquals(new[] { 2, 3, 4, 5, 6 });
    }
}