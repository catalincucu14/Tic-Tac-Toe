using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace TicTacToe
{
    public class MonteCarloTreeSearch
    {
        private static Random random = new Random();

        /// <summary>
        /// Function used to get the best move on the current board's state by applying the MCTS algorithm
        /// </summary>
        public static int GetMoveMCTS(Board board)
        {
            // Create root node
            Node root = new Node(null, board.Clone(), -1);

            foreach (int i in Enumerable.Range(0, Board.Iterations))
            {
                // Selection phase of the MCTS algorithm
                Node node = Selection(root);

                // Simulation phase of the MCTS algorithm
                int score = Simulation(node);

                // Backpropagation phase of the MCTS algorithm
                Backpropagation(node, score);
            }

            // Return the best move found
            return GetBestMove(root).Move;
        }

        /// <summary>
        /// Select a node
        /// </summary>
        private static Node? Selection(Node root)
        {
            // If a node is leaf the game is either won or drawn
            while (!root.IsLeaf)
            {
                // Get the best move from a fully expanded node
                if (root.FullyExpanded)
                    root = GetBestMove(root);

                // If is not fully expanded cuntinue expansion
                else
                    return Expansion(root);
            }

            // Return the best node
            return root;
        }

        /// <summary>
        /// Expand a node by creating a new child
        /// </summary>
        private static Node? Expansion(Node node)
        {
            // Get all possible moves on the node's current board
            List<Tuple<Board, int>> posibleMoves = node.Board.GetAllPosibleMovesOnTheCurrentBoard();

            foreach (Tuple<Board, int> move in posibleMoves)
            {
                if (node.IsValidMove(move.Item2))
                {
                    // Create and add the child to the parent node
                    Node newNode = new Node(node, node.Board.Clone(), move.Item2);
                    node.AddChild(newNode);

                    // Case when the node is fully expanded
                    if (posibleMoves.Count() == node.Childs.Count())
                        node.FullyExpanded = true;

                    return newNode;
                }
            }

            // It should not get here
            return null;
        }

        /// <summary>
        /// Simulate a game by making random moves until we reach the end of the game
        /// </summary>
        private static int Simulation(Node node)
        {
            Board board = node.Board.Clone();

            while (!board.HasAnyoneWon())
            {
                // Case if is a draw
                if (board.NoMoreMoves())
                    return 0;

                // Make a random move
                List<Tuple<Board, int>> posibleMoves = board.GetAllPosibleMovesOnTheCurrentBoard();
                board.MakeMove(posibleMoves[random.Next(posibleMoves.Count())].Item2);
            }

            // If the next move belongs to the opponent then the computer (O) won
            return board.PlayerTurn == Move.X ? 1 : -1;
        }

        /// <summary>
        /// Update the node with the number of visits and score up to the root
        /// </summary>
        private static void Backpropagation(Node node, int score)
        {
            var nodeTemp = node;
            while (nodeTemp is not null)
            {
                nodeTemp.Visits += 1;
                nodeTemp.Score += score;
                nodeTemp = nodeTemp.Parent;
            }
        }

        /// <summary>
        /// Returns the best move from the childs of a given root
        /// </summary>
        private static Node GetBestMove(Node node)
        {
            double bestScore = double.MinValue;
            double moveScore;

            List<Node> bestMoves = new List<Node>();

            foreach (Node child in node.Childs)
            {
                // Calulate the UTC
                moveScore = child.Score / child.Visits + Math.Sqrt(2 * Math.Log(node.Visits) / child.Visits);

                // Update the best score
                if (moveScore > bestScore)
                {
                    bestScore = moveScore;
                    bestMoves = new List<Node>() { child };
                }
                else if (moveScore == bestScore)
                    bestMoves.Add(child);
            }

            // If are multiple nodes that have the same UTC return a random one
            return bestMoves[random.Next(bestMoves.Count)];
        }
    }
}