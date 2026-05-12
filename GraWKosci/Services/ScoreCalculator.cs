using GraWKosci.Models;

namespace GraWKosci.Services;

public static class ScoreCalculator
{
    public static int Calculate(ScoreCategory category, Dice[] dices)
    {
        var values = dices.Select(x => x.Value).ToArray();
        var grouped = values.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

        return category switch
        {
            ScoreCategory.Ones => Sum(values, 1),
            ScoreCategory.Twos => Sum(values, 2),
            ScoreCategory.Threes => Sum(values, 3),
            ScoreCategory.Fours => Sum(values, 4),
            ScoreCategory.Fives => Sum(values, 5),
            ScoreCategory.Sixes => Sum(values, 6),

            ScoreCategory.ThreeOfAKind => grouped.Any(x => x.Value >= 3) ? values.Sum() : 0,
            ScoreCategory.FourOfAKind => grouped.Any(x => x.Value >= 4) ? values.Sum() : 0,

            ScoreCategory.FullHouse =>
                grouped.Count == 2 && grouped.Any(x => x.Value == 3)
                    ? 25
                    : 0,

            ScoreCategory.SmallStraight => HasSmallStraight(values) ? 30 : 0,

            ScoreCategory.LargeStraight => HasLargeStraight(values) ? 40 : 0,

            ScoreCategory.Yahtzee => grouped.Any(x => x.Value == 5) ? 50 : 0,

            ScoreCategory.Chance => values.Sum(),

            _ => 0
        };
    }

    private static int Sum(int[] values, int target)
        => values.Where(x => x == target).Sum();

    private static bool HasSmallStraight(int[] values)
    {
        var distinct = values.Distinct().OrderBy(x => x).ToArray();

        return Contains(distinct, new[] { 1, 2, 3, 4 }) ||
               Contains(distinct, new[] { 2, 3, 4, 5 }) ||
               Contains(distinct, new[] { 3, 4, 5, 6 });
    }

    private static bool HasLargeStraight(int[] values)
    {
        var distinct = values.Distinct().OrderBy(x => x).ToArray();

        return distinct.SequenceEqual(new[] { 1, 2, 3, 4, 5 }) ||
               distinct.SequenceEqual(new[] { 2, 3, 4, 5, 6 });
    }

    private static bool Contains(int[] source, int[] target)
        => target.All(source.Contains);
}