namespace ParaTH;

static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        // headless logic tests: dotnet run -- --test
        if (Array.IndexOf(args, "--test") >= 0)
            return LogicTests.RunAll();

        using var g = new Engine();
        g.Run();
        return 0;
    }
}
