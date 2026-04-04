namespace ParaTH;

static class Program
{
    [STAThread]
    static void Main()
    {
        using var g = new Engine();
        g.Run();
    }
}
