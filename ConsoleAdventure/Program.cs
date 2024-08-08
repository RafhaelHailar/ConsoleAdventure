using System;
using System.Timers;
using static Game;

namespace ConsoleAdventure
{
    internal class Program  
    {
        private static System.Timers.Timer aTimer;
        static string intro = "Welcome, to the game of console";
        static int pointer = 0;
        static int counter = 0;
        static void Main(string[] args)
        {
            Game game = new Game();
            setTimer();
            Console.ReadLine();
        }

        private static void setTimer()
        {
            aTimer = new System.Timers.Timer(70);
            aTimer.Elapsed += OnTimeEvent;
            aTimer.Enabled = true;
            aTimer.AutoReset = true;
        }

        private static void OnTimeEvent(Object source, ElapsedEventArgs e)
        {
            counter++;
            if (pointer == intro.Length)
            {
                aTimer.Stop();
                aTimer.Dispose();
                return;
            }

            char symbol = intro[pointer];
            Console.Write(symbol);

            pointer++;
        }
    }
}
