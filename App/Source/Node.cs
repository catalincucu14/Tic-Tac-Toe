using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace TicTacToe
{
    public class Node
    {
        public Board Board { get; set; }

        public Node? Parent { get; set; }

        public List<Node> Childs { get; set; }

        public bool IsLeaf { get; set; }

        public bool FullyExpanded { get; set; }

        public double Visits { get; set; }

        public int Move { get; set; }

        public double Score { get; set; }

        public Node(Node? parent, Board board, int move)
        {
            Board = board;
            if (move != -1)
                Board.MakeMove(move);
            Parent = parent;
            Childs = new List<Node>();
            FullyExpanded = IsLeaf = CheckIfIsLeaf();
            Move = move;
            Visits = Score = 0;
        }

        /// <summary>
        /// Function used to determine if a node is a leaf or not, is if the game is done, win or draw
        /// </summary>
        private bool CheckIfIsLeaf() => Board.CheckWinner() is not null || Board.NoMoreMoves() ? true : false;

        /// <summary>
        /// Function used to add children to the list
        /// </summary>
        public void AddChild(Node child) => Childs.Add(child);

        /// <summary>
        /// Function used to determine if a move is valid, if the move is made by a child
        /// </summary>
        public bool IsValidMove(int move)
        {
            foreach (Node child in Childs)
                if (child.Move == move)
                    return false;
            return true;
        }
    }
}
