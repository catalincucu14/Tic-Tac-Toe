using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace TicTacToe
{
    public class Board
    {
        private List<Move> Moves { get; set; }

        public Move PlayerTurn { get; set; }

        public static int Iterations { get; set; }

        public Board(int _simulations) => ResetBoard(_simulations);

        public Board(List<Move> moves, Move playerTrun)
        {
            Moves = moves;
            PlayerTurn = playerTrun;
        }

        /// <summary>
        /// Function used to clone the currrent board, used in MCTS
        /// </summary>
        public Board Clone()
        {
            return new Board(
                Moves.Select(move => move).ToList(),
                PlayerTurn == Move.X ? Move.X : Move.O);
        }

        /// <summary>
        /// Function used to reset the moves made on the board, use when to start a new game
        /// </summary>
        public void ResetBoard(int iterations)
        {
            Moves = Enumerable.Range(1, 9).Select(i => Move.FREE).ToList();
            PlayerTurn = Move.X;
            Iterations = iterations;
        }

        /// <summary>
        /// Function used to get the opponent, used to switch the player turn after a move
        /// </summary>
        private Move SwitchPlayer() => PlayerTurn == Move.X ? Move.O : Move.X;

        /// <summary>
        /// Function used to do the same thing as "SwitchPlayer" but have a different name :)
        /// </summary>
        public Move GetOpponent() => SwitchPlayer();

        /// <summary>
        /// Function used to make a move on the board on a given position
        /// </summary>
        public void MakeMove(int index)
        {
            Moves[index] = PlayerTurn;
            PlayerTurn = SwitchPlayer();
        }

        /// <summary>
        /// Function used to make a move for the computer using MCTS
        /// </summary>
        public int MakeComputerMove()
        {
            // Get the best move using Monte Carlo Tree Search algorithm and make the move
            dynamic result = MonteCarloTreeSearch.GetMoveMCTS(Clone());
            MakeMove(result);

            // Return the result to make the change the interface as well
            return result;
        }

        /// <summary>
        /// Function used to check the board if are any winning combinations
        /// </summary>
        public List<int>? CheckWinner()
        {
            foreach (int i in Enumerable.Range(0, 3))
            {
                // Check the horizontal combinations
                if (Moves[i * 3] != Move.FREE && (Moves[i * 3 + 1] == Moves[i * 3] && Moves[i * 3 + 2] == Moves[i * 3]))
                    return new List<int>() { i * 3, i * 3 + 1, i * 3 + 2 };

                // Check the vertical combinations
                if (Moves[i] != Move.FREE && (Moves[1 * 3 + i] == Moves[i] && Moves[2 * 3 + i] == Moves[i]))
                    return new List<int>() { i, 1 * 3 + i, 2 * 3 + i };

                // Check the diagonal combinations
                if (i == 1)
                    continue;
                else if (Moves[i] != Move.FREE && (Moves[4] == Moves[i] && Moves[8 - i] == Moves[i]))
                    return new List<int>() { i, 4, 8 - i };
            }

            // If no one won return null to continue the match
            return null;
        }

        /// <summary>
        /// Return all available moves on the current board
        /// </summary>
        private List<int> AvailableMoves() => Moves
            .Select((value, index) => new { value = value, index = index })
            .Where(pair => pair.value == Move.FREE)
            .Select(pair => pair.index)
            .ToList();

        /// <summary>
        /// Function used to check if are still moves available
        /// </summary>
        public bool NoMoreMoves() => !Moves.Any(cell => cell == Move.FREE);

        /// <summary>
        /// Function used to get a list of boards with all possible moves on the current board
        /// </summary>
        public List<Tuple<Board, int>> GetAllPosibleMovesOnTheCurrentBoard()
        {
            List<Tuple<Board, int>> posibleMoves = new List<Tuple<Board, int>>();
            AvailableMoves().ForEach(move => {
                Board boardCopy = Clone();
                boardCopy.MakeMove(move);
                posibleMoves.Add(Tuple.Create(boardCopy, move));
            });
            return posibleMoves;
        }

        /// <summary>
        /// Function used to check if anyone won
        /// </summary>
        public bool HasAnyoneWon() => CheckWinner() is not null;

        /// <summary>
        /// Function used to get a string with board's representation, for debugging purposes
        /// </summary>
        public string PrintBoard()
        {
            string result = "";
            foreach (int i in Enumerable.Range(0, 3))
            {
                foreach (int j in Enumerable.Range(0, 3))
                {
                    var move = Moves[3 * i + j];
                    result += move == Move.X ? "X " : move == Move.O ? "O " : ". ";
                }
                result += "\n";
            }
            return result;
        }
    }
}
