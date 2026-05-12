using GraWKosci.Models;
using GraWKosci.Services;

var state = new GameState();

Console.WriteLine("Podaj liczbę graczy (2-4):");

int count;
while (!int.TryParse(Console.ReadLine(), out count) || count < 2 || count > 4)
{
    Console.WriteLine("Podaj poprawną liczbę graczy (2-4):");
}

Console.WriteLine("Tryb gry:");
Console.WriteLine("1 - Z podpowiedziami");
Console.WriteLine("2 - Hardcore");

bool showHints = Console.ReadLine() == "1";

for (int i = 0; i < count; i++)
{
    Console.WriteLine($"Nazwa gracza {i + 1}:");
    var name = Console.ReadLine();

    while (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Nazwa nie może być pusta:");
        name = Console.ReadLine();
    }

    state.Players.Add(new Player(name));
}

var engine = new GameEngine(
    state,
    new DiceRoller(),
    showHints
);

engine.Run();