using GraWKosci.Models;
using GraWKosci.Services;

var state = new GameState();

Console.WriteLine("=== GRA W KOSCI ===\n");
Console.WriteLine("Podaj liczbe graczy ludzkich (1-4):");

int humanCount;
while (!int.TryParse(Console.ReadLine(), out humanCount) || humanCount < 1 || humanCount > 4)
{
    Console.WriteLine("Podaj poprawna liczbe graczy (1-4):");
}

Console.WriteLine("Podaj liczbe graczy AI (0-3, lacznie max 4 graczy):");

int aiCount;
int maxAi = 4 - humanCount;
while (!int.TryParse(Console.ReadLine(), out aiCount) || aiCount < 0 || aiCount > maxAi)
{
    Console.WriteLine($"Podaj poprawna liczbe graczy AI (0-{maxAi}):");
}

if (humanCount + aiCount < 2)
{
    Console.WriteLine("Dodaje drugiego gracza AI, bo wymagane minimum 2 graczy.");
    aiCount = 2 - humanCount;
}

for (int i = 0; i < humanCount; i++)
{
    Console.WriteLine($"Nazwa gracza {i + 1}:");
    var name = Console.ReadLine();

    while (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Nazwa nie moze byc pusta:");
        name = Console.ReadLine();
    }

    state.Players.Add(new Player(name));
}

for (int i = 0; i < aiCount; i++)
{
    state.Players.Add(new Player($"AI-{i + 1}", isAi: true));
    Console.WriteLine($"Dodano gracza AI: AI-{i + 1}");
}

Console.WriteLine($"\nGraja: {string.Join(", ", state.Players.Select(p => p.Name + (p.IsAi ? " [AI]" : "")))}");
Console.WriteLine("Nacisnij Enter, aby rozpoczac...");
Console.ReadLine();

var engine = new GameEngine(state, new DiceRoller());
engine.Run();