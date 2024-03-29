using System;

namespace hackerWorldNS
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (hackerWorld game = new hackerWorld())
            {
                game.Run();
            }
        }
    }
#endif
}

