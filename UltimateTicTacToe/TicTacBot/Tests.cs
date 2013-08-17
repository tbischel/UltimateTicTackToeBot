using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TicTacBot
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestMiniBoard()
        {
            var board = new MiniBoard();
            Assert.AreEqual(board.Probability(Player.X), 0.5, 0.05, "a new board has even probability for X");
            Assert.AreEqual(board.Probability(Player.O), 0.5, 0.05, "same for O");

            var moves = board.Moves();
            var expected = Enumerable.Range(0, 9).ToList();
            CollectionAssert.AreEqual(expected, moves, "all nine moves are available");

            var hash = board.GetHashCode();
            Assert.AreEqual(0, hash, "proper hash for an empty board");

            var p1 = board.Probability(Player.X);
            board.MakeMove(4, Player.X);
            var p2 = board.Probability(Player.X);
            board.MakeMove(3, Player.X);
            var p3 = board.Probability(Player.X);
            board.MakeMove(5, Player.X);
            var p4 = board.Probability(Player.X);

            Assert.Greater(p2, p1);
            Assert.Greater(p3, p2);
            Assert.Greater(p4, p3);
            Assert.AreEqual(p4, 1.0, "exact match on a win");

            moves = board.Moves();
            expected = new List<int> { 0, 1, 2, 6, 7, 8 };
            CollectionAssert.AreEqual(expected, moves, "six moves are left");

            p1 = board.Probability(Player.O);
            Assert.AreEqual(0.0, p1);
            board.MakeMove(6, Player.O);
            p2 = board.Probability(Player.O);
            Assert.AreEqual(0.0, p2);
            board.MakeMove(7, Player.O);
            p3 = board.Probability(Player.O);
            Assert.AreEqual(0.0, p3);
            board.MakeMove(8, Player.O);
            p4 = board.Probability(Player.O);
            Assert.AreEqual(0.0, p4);
            
        }

        [Test]
        public void TestBoard()
        {
            Board b = new Board();
            Assert.IsFalse(b.Terminal());

            var s1 = b.Score();
            Assert.AreEqual(0.5, s1, 0.05, "game starts mostly even...");

            var moves = b.Moves();
            var expected = Enumerable.Range(0, 81).ToList();
            CollectionAssert.AreEqual(expected, moves);

            b.MakeMove(40);
            var s2 = b.Score();
            Assert.Greater(s2, s1);

            moves = b.Moves();
            expected = new List<int> { 36, 37, 38, 39, 41, 42, 43, 44 };
            CollectionAssert.AreEqual(expected, moves);

            b.MakeMove(36);
            var s3 = b.Score();
            Assert.Less(s3, s2);

            moves = b.Moves();
            expected = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            CollectionAssert.AreEqual(expected, moves);
        }

        [Test]
        public void TestGame()
        {
            List<int> game = new List<int> { 0, 1, 9, 2, 22, 36, 4, 37, 13, 38, 26, 72, 8, 73, 17, 74, 18 };
            Board b = new Board();
            var last = b.Score();
            var current = 0.0;
            for(int i=0; i<game.Count; i++)
            {
                Assert.False(b.Terminal(), "Game still not over");
                CollectionAssert.Contains(b.Moves(), game[i], "Our planned move is available");
                b.MakeMove(game[i]);
                current = b.Score();
                if (i % 2 == 0)
                    Assert.Greater(current, last, "X has improved their position");
                else
                    Assert.Greater(last, current, "O has improved their position");
                last = current;
            }
            Assert.True(b.Terminal(), "Game is finished");
            current = b.Score();
            Assert.AreEqual(1.0, current, "Final position is won by X");
        }

        [Test]
        public void TestSearch()
        {
            var nm = new Negamax();
            var mm = new Minimax();

            Board b = new Board();
            for(int i=0; i<4; i++)
            {
                var m1 = nm.Score(b, i, 10000.0, -100000.0);
                var m2 = mm.Score(b, i);

                Assert.AreEqual(m2, m1, "scoring works correctly at depth " + i);
            }
        }
    }
}
