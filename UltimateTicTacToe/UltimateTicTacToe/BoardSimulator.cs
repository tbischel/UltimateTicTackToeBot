using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UltimateTicTacToe
{
    public class BoardSimulator
    {
        public static int sampleSize = 10000;
        public static Random rand = new Random(0);
        public Dictionary<int, int> XWins = new Dictionary<int, int>();
        public Dictionary<int, int> OWins = new Dictionary<int, int>();
        public Dictionary<int, double> ProbXWins = new Dictionary<int, double>();
        public Dictionary<int, double> ProbOWins = new Dictionary<int, double>();
        public List<int> Keys = new List<int>();

        public BoardSimulator()
        {

        }

        public void SimulateAllPositions(int depth, int x, int o)
        {
            if(depth == 0)
            {
                Board b = new Board(x, o);
                int hash = b.GetHashCode();
                Keys.Add(hash);
                XWins[hash] = 0;
                OWins[hash] = 0;
                for(int i=0; i<sampleSize; i++)
                {
                    b = new Board(x, o);
                    var outcome = SimulateToEnd(b);
                    if (outcome == GameOutcome.XWon)
                        XWins[hash]++;
                    else if (outcome == GameOutcome.OWon)
                        OWins[hash]++;
                }
                ProbXWins[hash] = (double)XWins[hash] / (double)sampleSize;
                ProbOWins[hash] = (double)OWins[hash] / (double)sampleSize;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    var nextX = x << 1;
                    var nextO = o << 1;
                    if (i == 1) nextX |= 1;
                    if (i == 2) nextO |= 1;
                    SimulateAllPositions(depth - 1, nextX, nextO);
                }
            }
        }

        public GameOutcome SimulateToEnd(Board b)
        {
            var outcome = Board.Winner(b);
            if (outcome != GameOutcome.InProgress)
                return outcome;

            var moves = b.OpenSquares();
            var move = moves[rand.Next(moves.Count)];
            var player = rand.Next(2);
            var x = player == 1 ? move : 0;
            var o = player == 0 ? move : 0;
            b.MakeMove(x, o);
            return SimulateToEnd(b);
        }

        public void Save()
        {
            StreamWriter sr1 = new StreamWriter(@"E:\Visual Studio Projects\UltimateTicTacToe\UltimateTicTacToe\XProbs.csv");
            StreamWriter sr2 = new StreamWriter(@"E:\Visual Studio Projects\UltimateTicTacToe\UltimateTicTacToe\YProbs.csv");
            foreach(var key in Keys)
            {
                var p1 = (double)XWins[key]/(double)sampleSize;
                var p2 = (double)OWins[key]/(double)sampleSize;
                sr1.WriteLine(key + ", " + p1);
                sr2.WriteLine(key + ", " + p2);
            }
            sr1.Flush();
            sr1.Close();
            sr2.Flush();
            sr2.Close();
        }
    }
}
