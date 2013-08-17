using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacBot
{
    public class Minimax
    {
        public double Score(Board node, int depth)
        {
            if (node.Terminal() || depth <= 0)
            {
                return node.Score();
            }
            else
            {
                var alpha = -10000.0;
                foreach (var move in node.Moves())
                {
                    var child = node.Copy();
                    child.MakeMove(move);
                    alpha = Math.Max(alpha, -Score(child, depth-1));
                }
                return alpha;
            }
        }
    }
}
