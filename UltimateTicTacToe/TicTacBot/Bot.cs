using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacBot
{
    public class Bot
    {
        public int ChooseMove(Board b)
        {
            //var search = new Negamax();
            //var search = new Minimax();
            var search = new AlphaBeta();
            double bestscore = -999999.0;
            int bestmove = -1;
            foreach(var move in b.Moves())
            {
                var child = b.Copy();
                child.MakeMove(move);
                //var score = search.Score(child, 80, 10000.0, -10000.0, 1);
                //var score = search.Score(child, 7);
                var score = search.Score(child, 8);
                System.Console.WriteLine("Move: " + move + ", Score: " + score);
                if(score > bestscore)
                {
                    bestmove = move;
                    bestscore = score;
                }
            }
            return bestmove;
        }
    }
}
