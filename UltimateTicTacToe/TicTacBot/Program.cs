using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Board b = new Board();
            Bot bt = new Bot();
            while(!b.Terminal())
            {
                var move = bt.ChooseMove(b);
                System.Console.WriteLine("Move: " + move + ", Score: " + b.Score());
                b.MakeMove(move);

                if(!b.Terminal())
                {
                    System.Console.WriteLine("Moves: " + String.Join(",", b.Moves()));
                    move = int.Parse(System.Console.ReadLine());
                    b.MakeMove(move);
                    System.Console.WriteLine("\nMove: " + move + ", Score: " + b.Score());
                }
            }
        }
    }
}
