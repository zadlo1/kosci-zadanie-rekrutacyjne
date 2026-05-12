namespace GraWKosci.Models;

public class ScoreCard
{
    private readonly Dictionary<ScoreCategory, int?> _scores = new();

    public ScoreCard()
    {
        foreach (ScoreCategory category in Enum.GetValues<ScoreCategory>())
        {
            _scores[category] = null;
        }
    }

    public bool IsUsed(ScoreCategory category)
    {
        return _scores[category].HasValue;
    }

    public void SetScore(ScoreCategory category, int score)
    {
        _scores[category] = score;
 }

    public int? GetScore(ScoreCategory category)
    {
        return _scores[category];
    }

    public Dictionary<ScoreCategory, int?> GetAllScores()
    {
        return _scores;
    }

    public int UpperSectionTotal =>
        (_scores[ScoreCategory.Ones] ?? 0) +
        (_scores[ScoreCategory.Twos] ?? 0) +
        (_scores[ScoreCategory.Threes] ?? 0) +
        (_scores[ScoreCategory.Fours] ?? 0) +
        (_scores[ScoreCategory.Fives] ?? 0) +
        (_scores[ScoreCategory.Sixes] ?? 0);

    public int UpperBonus => UpperSectionTotal >= 63 ? 35 : 0;

    public int TotalScore =>
        _scores.Values.Where(x => x.HasValue).Sum(x => x!.Value) + UpperBonus;

    public bool IsComplete => _scores.Values.All(x => x.HasValue);
}