using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacBot
{
    public class AlphaBeta
    {
        public double Score(Board node, int depth)
        {
            return alphaBetaMax(node, -10000.0, 10000.0, depth);
        }

        private double alphaBetaMax(Board node, double alpha, double beta, int depth)
        {
            if ( depth == 0 || node.Terminal()) return node.Score();
            foreach(var move in node.Moves())
            {
                var child = node.Copy();
                child.MakeMove(move);
                var score = alphaBetaMin(node, alpha, beta, depth - 1);
                if (score >= beta)
                    return beta;
                if (score > alpha)
                    alpha = score;
            }
            return alpha;
        }

        private double alphaBetaMin(Board node, double alpha, double beta, int depth) 
        {
            if ( depth == 0 || node.Terminal()) return -node.Score();
            foreach (var move in node.Moves())
            {
                var child = node.Copy();
                child.MakeMove(move);
                var score = alphaBetaMax(node, alpha, beta, depth - 1);
                if( score <= alpha )
                    return alpha;
                if( score < beta )
                    beta = score;
            }
            return beta;
        }
    }
}
