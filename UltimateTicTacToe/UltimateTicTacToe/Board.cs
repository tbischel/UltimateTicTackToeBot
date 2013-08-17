using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateTicTacToe
{
    public enum GameOutcome
    {
        XWon,
        OWon,
        InProgress,
        CatsGame,
        Unknown
    }

    public class Board
    {
        private int X;
        private int O;
        private static readonly int boardFull = 511;
        private static readonly List<int> masks = new List<int> { 448, 56, 7, 292, 146, 73, 273, 84 };

        public Board()
        {
            X = O = 0;
        }

        public Board(int x, int o)
        {
            X = x;
            O = o;
        }

        public List<int> OpenSquares()
        {
            var results = new List<int>();
            var squares = (~(X & O)) & boardFull;
            while (squares != 0)
            {
                var next = squares & ~(squares - 1);
                results.Add(next);
                squares = squares & ~next;
            }
            return results;
        }

        public void MakeMove(int x, int o)
        {
            X |= x;
            O |= o;
        }

        public static GameOutcome Winner(Board board)
        {
            var xWon = masks.Any(m => (board.X & m) == m);
            var yWon = masks.Any(m => (board.O & m) == m);
            var full = boardFull == (board.X | board.O);

            if (xWon && yWon)
                return GameOutcome.Unknown;
            if (xWon)
                return GameOutcome.XWon;
            if (yWon)
                return GameOutcome.OWon;
            if (full)
                return GameOutcome.CatsGame;
            return GameOutcome.InProgress;
        }

        public override int GetHashCode()
        {
            return (X << 9) | O;
        }

        public Board Copy()
        {
            return new Board(X, O);
        }

    }
}
