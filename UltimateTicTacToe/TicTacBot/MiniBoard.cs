using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TicTacBot
{
    public enum MiniState
    {
        XWon,
        OWon,
        CatsGame,
        InProgress
    }

    public enum Player
    {
        X,
        O
    }

    public class MiniBoard
    {
        public static int MoveCount = 0;
        private static readonly Dictionary<int, int> _decoder = new Dictionary<int, int>
        {
            {1, 0},
            {2, 1},
            {4, 2},
            {8, 3},
            {16, 4},
            {32, 5},
            {64, 6},
            {128, 7},
            {256, 8}
        };
        private static readonly Dictionary<int, double> _probXWins = Load(@"E:\Visual Studio Projects\UltimateTicTacToe\UltimateTicTacToe\XProbs.csv");
        private static readonly Dictionary<int, double> _probOWins = Load(@"E:\Visual Studio Projects\UltimateTicTacToe\UltimateTicTacToe\OProbs.csv");
        private static readonly int boardMask = 511;
        private static readonly List<int> masks = new List<int> { 448, 56, 7, 292, 146, 73, 273, 84 };
        private MiniState _state = MiniState.InProgress;
        private int _x = 0;
        private int _o = 0;

        public MiniBoard()
        {
            
        }

        private static Dictionary<int, double> Load(String file)
        {
            var result = new Dictionary<int, double>();

            StreamReader sr = new StreamReader(file);
            while(!sr.EndOfStream)
            {
                var vals = sr.ReadLine().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                result[int.Parse(vals[0])] = double.Parse(vals[1]);
            }
            sr.Close();
            return result;
        }

        private int _encode(int move)
        {
            return 1 << move;
        }

        private int _decode(int move)
        {
            return _decoder[move];
        }

        public List<int> Moves()
        {
            List<int> results = new List<int>();
            var available = ~(_x | _o) & boardMask;
            while(available != 0)
            {
                var next = available & ~(available - 1);
                results.Add(_decode(next));
                available = available & ~next;
                MoveCount++;
            }
            return results;
        }

        public void MakeMove(int move, Player p)
        {
            if (p == Player.X) _x |= _encode(move);
            else if (p == Player.O) _o |= _encode(move);

            if (_state == MiniState.InProgress) 
                _evaluate();
        }

        private void _evaluate()
        {
            if (masks.Any(m => (m & _x) == m))
                _state = MiniState.XWon;
            else if (masks.Any(m => (m & _o) == m))
                _state = MiniState.OWon;
            else if ((_x | _o) == boardMask)
                _state = MiniState.CatsGame;
        }

        public double Probability(Player p)
        {
            if (p == Player.X)
            {
                if (_state == MiniState.XWon)
                    return 1.0;
                if (_state == MiniState.OWon)
                    return 0.0;
                return _probXWins[GetHashCode()];
            }
            else if (p == Player.O)
            {
                if (_state == MiniState.OWon)
                    return 1.0;
                if (_state == MiniState.XWon)
                    return 0.0;
                return _probOWins[GetHashCode()];
            }
            return 0.0;
        }

        public MiniState State()
        {
            return this._state;
        }

        public override int GetHashCode()
        {
            return (_x << 9) | _o;
        }

        public MiniBoard Copy()
        {
            var c = new MiniBoard();
            c._state = _state;
            c._x = _x;
            c._o = _o;
            return c;
        }
    }
}
