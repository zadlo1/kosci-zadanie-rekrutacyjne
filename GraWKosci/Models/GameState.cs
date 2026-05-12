namespace GraWKosci.Models;

public class GameState
{
    public List<Player> Players { get; } = new();

    public int CurrentPlayerIndex { get; set; } = 0;

    public int Round => Players.Max(p => p.ScoreCard.IsComplete ? 0 : 1);

    public bool IsGameFinished =>
        Players.All(p => p.ScoreCard.IsComplete);

    public Player CurrentPlayer =>
        Players[CurrentPlayerIndex];
}