using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumericalMethods;
using System.IO;

namespace UltimateTicTacToe
{
    public class HeuristicBuilder
    {
        public static Random rand = new Random(0);
        public List<Tuple<List<double>, double>> Samples = new List<Tuple<List<double>, double>>();
        public Dictionary<int, double> XWins = new Dictionary<int, double>();
        public Dictionary<int, double> OWins = new Dictionary<int, double>();
        public List<int> Keys = new List<int>();
        public Matrix solver = new Matrix(3,3);
        public int SimulationCount = 1000;
        public int SampleCount = 20000;


        public List<List<int>> Positions = new List<List<int>>
        {
            new List<int>{256, 128,  64},
            new List<int>{32,  16,   8},
            new List<int>{4,   2,    1}
        };

        public void CreateSolver()
        {
            Matrix x = new Matrix(SampleCount, 17);
            Matrix y = new Matrix(SampleCount, 1);
            for(int i=0; i<SampleCount; i++)
            {
                var sample = Samples[i];
                var xs = sample.Item1;
                var ys = sample.Item2;
                y[i, 0] = ys;
                for(int j=0; j<17; j++)
                {
                    x[i, j] = xs[j];
                }
            }
            var xt = x.Transpose();
            var inv = (xt * x).Inverse();
            var a = inv * (xt * y);
            solver = a;
        }

        public void GenerateSamples()
        {
            Samples = new List<Tuple<List<double>, double>>();
            int cnt = 0;
            while(cnt<SampleCount)
            {
                try
                {
                    var sample = GenerateSample();
                    Samples.Add(sample);
                    cnt++;
                }
                catch(Exception ex)
                {

                }
            }
        }

        public Tuple<List<double>, double> GenerateSample()
        {
            var p1 = new Dictionary<int, Tuple<double, double>>();
            var p2 = new Dictionary<int, Tuple<double, double>>();
            Board b = new Board();
            for(int i=0; i<9; i++)
            {
                var k1 = 1 << i;
                var k2 = Keys[rand.Next(Keys.Count)];
                p1[k1] = new Tuple<double, double>(XWins[k2], OWins[k2]);
                if (XWins[k2] == 1.0)
                    b.MakeMove(k1, 0);
                else if (OWins[k2] == 1.0)
                    b.MakeMove(0, k1);
                else
                    p2[k1] = p1[k1];
            }
            if (Board.Winner(b) == GameOutcome.Unknown)
                throw new Exception("Invalid Sample");

            var input = GenerateInputs(p1);
            var output = ScorePosition(b, p2);
            return new Tuple<List<double>, double>(input, output);
        }

        public List<double> GenerateInputs(Dictionary<int, Tuple<double, double>> probs)
        {
            List<double> result = new List<double>{1.0};
            result.AddRange(Score(256, 128, 64, probs));
            result.AddRange(Score(32, 16, 8, probs));
            result.AddRange(Score(4, 2, 1, probs));
            result.AddRange(Score(256, 32, 4, probs));
            result.AddRange(Score(128, 16, 2, probs));
            result.AddRange(Score(64, 8, 1, probs));
            result.AddRange(Score(256, 16, 8, probs));
            result.AddRange(Score(4, 16, 64, probs));
            return result;
        }

        public new List<double> Score(int square1, int square2, int square3, Dictionary<int, Tuple<double, double>> probs)
        {
            return new List<double>()
            {
                probs[square1].Item1*probs[square2].Item1*probs[square3].Item1,
                probs[square1].Item2*probs[square2].Item2*probs[square3].Item2,
            };
        }

        public double ScorePosition(Board b, Dictionary<int, Tuple<double, double>> probs)
        {
            int xWon, oWon;
            xWon = oWon = 0;
            for(int i=0; i<SimulationCount; i++)
            {
                Board boardCopy = b.Copy();
                var probsCopy = new Dictionary<int, Tuple<double, double>>(probs);
                var outcome = SimulateOneGame(boardCopy, probsCopy);
                if (outcome == GameOutcome.XWon)
                    xWon++;
                if (outcome == GameOutcome.OWon)
                    oWon++;
            }
            var result = (double)(xWon-oWon)/(double)SimulationCount;
            return result;
        }

        public GameOutcome SimulateOneGame(Board b, Dictionary<int, Tuple<double, double>> probs)
        {
            var winner = Board.Winner(b);
            if (winner != GameOutcome.InProgress)
                return winner;
            if (probs.Count == 0)
                return winner;

            var key = probs.Keys.ToList()[rand.Next(probs.Keys.Count)];
            var roll = rand.NextDouble();
            var x = 0;
            var o = 0;
            if(probs[key].Item1 < roll)
                x = key;
            else if(probs[key].Item2 + probs[key].Item1 < roll)
                o = key;
            b.MakeMove(x, o);
            probs.Remove(key);
            return SimulateOneGame(b, probs);
        }

        public void Save()
        {
            List<double> weights = new List<double>();
            for(int i=0; i<solver.Rows; i++)
            {
                weights.Add(solver[i, 0]);
            }
            StreamWriter sr = new StreamWriter(@"E:\Visual Studio Projects\UltimateTicTacToe\UltimateTicTacToe\Weights.csv");
            sr.WriteLine(String.Join(", ", weights));
            sr.Flush();
            sr.Close();
        }
    }
}
