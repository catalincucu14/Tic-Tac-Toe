using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

#nullable enable
namespace TicTacToe
{
    public partial class AppMainWindow : Window
    {
        private Game game;

        private List<Button> boardButtons;

        private Label resultLabel;

        public AppMainWindow()
        {
            InitializeComponent();

            boardButtons = Board.Children.Cast<Button>().ToList();

            resultLabel = ResultLabel;

            game = new Game(this);

            NewGame();
        }

        /// <summary>
        /// Function used to reset the current game when a UI Button is pressed
        /// </summary>
        private void Button_Click_NewGame(object sender, RoutedEventArgs e) => NewGame();

        /// <summary>
        /// Function used to make a move on the board when a UI board Button is pressed
        /// </summary>
        private void Button_Click_Move(object sender, RoutedEventArgs e) => game.MakeMove(3 * Grid.GetRow((Button)sender) + Grid.GetColumn((Button)sender));

        /// <summary>
        /// Function used to reset the current game
        /// </summary>
        private void NewGame()
        {
            // Reset the game - the board and the oposition
            game.NewGame();

            // Reset the button on the board
            boardButtons.ForEach(button =>
            {
                button.Content = "";
                button.IsEnabled = true;
                button.Style = (Style)FindResource("BoardButton");
            });


            // Reset the result label to indicate the opposition
            resultLabel.Content = GetOpposition() ? 
                "You (X) against Computer (O)" : "Player 1 (X) against Player 2 (O)";
        }

        /// <summary>
        /// Function used to set the result of the game
        /// </summary>
        public void SetResult(Tuple<List<int>, Move>? result)
        {
            if (result is not null)
            {
                MarkWinningCombination(result.Item1);
                SetWinnerResultLabel(result.Item2);
            }
            else
                resultLabel.Content = "Draw!";
        }

        /// <summary>
        /// Function used to make a winning combination in green
        /// </summary>
        private void MarkWinningCombination(List<int> combinations)
        {
            combinations.ForEach(i => boardButtons[i].Style = (Style)FindResource("WinBoardButton"));
            boardButtons.ForEach(button => button.IsEnabled = false);
        }

        /// <summary>
        /// Function used to show the winner in the UI Result Label
        /// </summary>
        public void SetWinnerResultLabel(Move winner) => resultLabel.Content = winner == Move.X ?
            GetOpposition() ? "You (X) win!" : "Player 1 (X) wins!" :
            GetOpposition() ? "Computer (O) wins!" : "Player 2 (O) wins!";

        /// <summary>
        /// Function used to mark a move on the UI board
        /// </summary>
        public void SetValueBoardCell(int index, Move move)
        {
            boardButtons[index].Content = move == Move.X ? "X" : "O";
            boardButtons[index].IsEnabled = false;
        }

        /// <summary>
        /// Function used to get the opposition from the UI Combobox
        /// </summary>
        public bool GetOpposition()
        {
            dynamic comboBox = (ComboBoxItem)GameType.SelectedItem;
            return comboBox.Content.ToString() == "Computer";
        }

        /// <summary>
        /// Function used to get the number of iterations for MCTS
        /// </summary>
        public int GetIterations()
        {
            return (int)Iterations.Value;
        }
    }
}
