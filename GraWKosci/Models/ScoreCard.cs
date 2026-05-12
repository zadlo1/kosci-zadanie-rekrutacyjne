namespace GraWKosci.Models;

public class ScoreCard
{
    // Tabelka 1
    public int? Ones { get; set; }
    public int? Twos { get; set; }
    public int? Threes { get; set; }
    public int? Fours { get; set; }
    public int? Fives { get; set; }
    public int? Sixes { get; set; }

    // Tabelka 2
    public int? ThreeOfAKind { get; set; }
    public int? FourOfAKind { get; set; }
    public int? FullHouse { get; set; }
    public int? SmallStraight { get; set; }
    public int? LargeStraight { get; set; }
    public int? Yahtzee { get; set; }
    public int? Chance { get; set; }

    public bool BonusApplied { get; set; }

    public int UpperSectionTotal =>
        (Ones ?? 0) +
        (Twos ?? 0) +
        (Threes ?? 0) +
        (Fours ?? 0) +
        (Fives ?? 0) +
        (Sixes ?? 0);

    public int UpperBonus =>
        (!BonusApplied && UpperSectionTotal >= 63) ? 35 : 0;

    public int LowerSectionTotal =>
        (ThreeOfAKind ?? 0) +
        (FourOfAKind ?? 0) +
        (FullHouse ?? 0) +
        (SmallStraight ?? 0) +
        (LargeStraight ?? 0) +
        (Yahtzee ?? 0) +
        (Chance ?? 0);

    public int TotalScore =>
        UpperSectionTotal + UpperBonus + LowerSectionTotal;

    public bool IsComplete =>
        Ones.HasValue &&
        Twos.HasValue &&
        Threes.HasValue &&
        Fours.HasValue &&
        Fives.HasValue &&
        Sixes.HasValue &&
        ThreeOfAKind.HasValue &&
        FourOfAKind.HasValue &&
        FullHouse.HasValue &&
        SmallStraight.HasValue &&
        LargeStraight.HasValue &&
        Yahtzee.HasValue &&
        Chance.HasValue;
    
    public bool IsUsed(ScoreCategory category)
    {
        return category switch
        {
            ScoreCategory.Ones => Ones.HasValue,
            ScoreCategory.Twos => Twos.HasValue,
            ScoreCategory.Threes => Threes.HasValue,
            ScoreCategory.Fours => Fours.HasValue,
            ScoreCategory.Fives => Fives.HasValue,
            ScoreCategory.Sixes => Sixes.HasValue,

            ScoreCategory.ThreeOfAKind => ThreeOfAKind.HasValue,
            ScoreCategory.FourOfAKind => FourOfAKind.HasValue,
            ScoreCategory.FullHouse => FullHouse.HasValue,
            ScoreCategory.SmallStraight => SmallStraight.HasValue,
            ScoreCategory.LargeStraight => LargeStraight.HasValue,
            ScoreCategory.Yahtzee => Yahtzee.HasValue,
            ScoreCategory.Chance => Chance.HasValue,

            _ => false
        };
    }
}