using System;

namespace TRPG2
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            while(true)
            {
                game.Process();
            }
        }
    }
}
