static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        using var g = new ParaTH.Engine();
        g.Run();
    }
}
