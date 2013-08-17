using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateTicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            BoardSimulator bs = new BoardSimulator();
            bs.SimulateAllPositions(9, 0, 0);
            bs.Save();
            HeuristicBuilder hb = new HeuristicBuilder();
            hb.XWins = bs.ProbXWins;
            hb.OWins = bs.ProbOWins;
            hb.Keys = hb.XWins.Keys.ToList();
            hb.GenerateSamples();
            hb.CreateSolver();
            hb.Save();
        }
    }
}
