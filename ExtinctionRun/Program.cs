using System;

namespace ExtinctionRun
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new ExtinctionRun())
                game.Run();
        }
    }
}
