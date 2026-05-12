using GraWKosci.Services;

namespace GraWKosci;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var engine = new GameEngine();
        engine.Start();
    }
}