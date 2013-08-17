using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacBot
{
    public class Negamax
    {
        public double Score(Board node, int depth, double alpha, double beta)
        {
            if (node.Terminal() || depth <= 0)
            {
                return node.Score();
            }
            else
            {
                foreach (var move in node.Moves())
                {
                    var child = node.Copy();
                    child.MakeMove(move);
                    var val = -Score(child, depth-1, -beta, -alpha);
                    if (val >= beta)
                        return val;
                    if (val > alpha)
                        alpha = val;
                }
                return alpha;
            }
        }
    }
}
