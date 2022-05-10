using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TicTacToe
{
    public class Game
    {
        private readonly AppMainWindow mainWindow;

        private Board board;

        private bool humanVersusComputer;

        public Game(AppMainWindow _mainWindow)
        {
            mainWindow = _mainWindow;

            NewGame();
        }

        /// <summary>
        /// Function used to reset the current game - the board, the opposition and the number of MCTS iterations
        /// </summary>
        public void NewGame()
        {
            board = new Board(mainWindow.GetIterations());

            humanVersusComputer = mainWindow.GetOpposition();
        }

        /// <summary>
        /// Function used to check if the game can go ahead - isn't a winner yet or are still moves to play
        /// </summary>
        private bool CheckResultAfterMove()
        {
            // Check if someone won, if yes mark the winning combination in green
            List<int> winnigCombination = board.CheckWinner();
            if (winnigCombination is not null)
            {
                mainWindow.SetResult(Tuple.Create(winnigCombination, board.GetOpponent()));
                return false;
            }

            // Check if are still vailable moves, if not its a draw
            if (board.NoMoreMoves())
            {
                mainWindow.SetResult(null);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Function used to make a move on  the board, is bonded to the interface
        /// </summary>
        public void MakeMove(int index)
        {
            // Make the move the human
            mainWindow.SetValueBoardCell(index, board.PlayerTurn);
            board.MakeMove(index);

            // Make the move for the computer if the human didn't win or are moves still to play
            if (humanVersusComputer && CheckResultAfterMove())
            {
                int result = board.MakeComputerMove();
                mainWindow.SetValueBoardCell(result, Move.O);
            }

            // Check the result again
            CheckResultAfterMove();
        }
    }
}
