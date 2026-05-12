namespace GraWKosci.Models;

public class Player
{
    public Player(string name, bool isAi = false)
    {
        Name = name;
        IsAi = isAi;
    }

    public string Name { get; }

    public bool IsAi { get; }

    public ScoreCard ScoreCard { get; } = new();
}