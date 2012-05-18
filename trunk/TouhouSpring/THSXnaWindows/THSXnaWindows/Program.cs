using System;

namespace TouhouSpring
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameApp game = new GameApp())
            {
                game.Run();
            }
        }
    }
#endif
}

