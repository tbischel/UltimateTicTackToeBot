using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TicTacBot
{
    public class Board
    {
        public Player CurrentPlayer
        {
            get
            {
                return _currentPlayer;
            }
        }

        private static List<double> weights = Load(@"E:\Visual Studio Projects\UltimateTicTacToe\UltimateTicTacToe\Weights.csv");

        private static List<List<int>> PossibleTicTacToes = new List<List<int>> 
            {
                new List<int> { 8, 7, 6 },
                new List<int> { 5, 4, 3 },
                new List<int> { 2, 1, 0 },
                new List<int> { 8, 5, 2 },
                new List<int> { 7, 4, 1 },
                new List<int> { 6, 3, 0 },
                new List<int> { 8, 4, 0 },
                new List<int> { 2, 4, 6 }
            };

        private Player _currentPlayer;
        private int _currentBoard;
        private List<MiniBoard> _miniBoards;
        public Board()
        {
            _currentPlayer = Player.X;
            _currentBoard = -1;
            _miniBoards = Enumerable.Range(0, 9).Select(i => new MiniBoard()).ToList();
        }

        public Board(String history) : this()
        {
            var moves = history.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToList();
            moves.ForEach(m => this.MakeMove(m));
        }

        public static List<double> Load(String file)
        {
            var sr = new StreamReader(file);
            var result = sr.ReadLine().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => double.Parse(s)).ToList();
            sr.Close();
            return result;
        }

        public Board Copy()
        {
            Board c = new Board();
            c._currentPlayer = _currentPlayer;
            c._currentBoard = _currentBoard;
            c._miniBoards = _miniBoards.Select(m => m.Copy()).ToList();
            return c;
        }

        public double Score()
        {
            var inp = _createInput();
            if (inp.Skip(1).Where((v, i) => i % 2 == 0).Any(v => v == 1.0))
                return 1.0;
            if (inp.Skip(1).Where((v, i) => i % 2 == 1).Any(v => v == 1.0))
                return 0.0;
            return weights.Zip(inp, (a, b) => a * b).Sum();
        }
        

        public List<double> _createInput()
        {
            List<double> result = new List<double> { 1.0 };
            result.AddRange(combineProbs(8, 7, 6));
            result.AddRange(combineProbs(5, 4, 3));
            result.AddRange(combineProbs(2, 1, 0));
            result.AddRange(combineProbs(8, 5, 2));
            result.AddRange(combineProbs(7, 4, 1));
            result.AddRange(combineProbs(6, 3, 0));
            result.AddRange(combineProbs(8, 4, 0));
            result.AddRange(combineProbs(2, 4, 6));
            return result;
        }

        public new List<double> combineProbs(int square1, int square2, int square3)
        {
            return new List<double>()
            {
                _miniBoards[square1].Probability(Player.X)*_miniBoards[square2].Probability(Player.X)*_miniBoards[square3].Probability(Player.X),
                _miniBoards[square1].Probability(Player.O)*_miniBoards[square2].Probability(Player.O)*_miniBoards[square3].Probability(Player.O),
            };
        }

        public List<int> Moves()
        {
            var moves = new List<int>();
            if (_currentBoard != -1)
                moves = _miniBoards[_currentBoard].Moves().Select(m => _currentBoard * 9 + m).ToList();

            if (moves.Count == 0)
                moves = _miniBoards.SelectMany((b, i) => b.Moves().Select(m => i * 9 + m)).ToList();

            return moves;
        }

        public void MakeMove(int move)
        {
            var board = move / 9;
            var miniboard = _miniBoards[board];
            var mv = move % 9;
            miniboard.MakeMove(mv, _currentPlayer);
            _currentBoard = mv;
            _currentPlayer = _currentPlayer == Player.X ? Player.O : Player.X;
        }

        public bool Terminal()
        {
            if (_miniBoards.All(b => b.State() != MiniState.InProgress))
                return true;
            foreach(var mask in PossibleTicTacToes)
            {
                var i1 = _miniBoards[mask.First()].State();
                if (i1 == MiniState.XWon || i1 == MiniState.OWon)
                {
                    if (mask.All(t => _miniBoards[t].State() == i1))
                        return true;
                }
            }
            return false;
        }
    }
}
