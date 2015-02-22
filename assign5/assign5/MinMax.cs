using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace assign5
{
    class MinMax
    {
        int Strength = 30000;
        Random rnd = new Random();

        public void SetStrength(int str)
        {
            Strength = str;
        }

        public float FindDeepValue(Board GameBoard, int col)
        {
            int value = 0;

            for (int i = 0; i < Strength; i++)
            {
                Board newBoard = GameBoard.Copy();
                newBoard.MakeMove(col);

                WinState? winner = CheckNextMoves(newBoard);

                // if the AI would win, increase the value of the move
                // if ai would lose, or tie, decrese the value
                //if (winner = WinState.BLACKWIN && Game1.AIColor == BoardState.BLACK)
                if (winner == WinState.BLACKWIN && GameBoard.CurrentPlayer == BoardState.BLACK)
                    value++;
                //else if (winner = WinState.REDWIN && Game1.AIColor == BoardState.RED)
                else if (winner == WinState.REDWIN && GameBoard.CurrentPlayer == BoardState.RED)
                    value++;
                else if (winner == WinState.TIE)
                    value = 0;
                else
                    value--;
            }

            // make the return value either -1 or 1
            return (value / (float)Strength);
        }

        /* Checks all the next possible moves given a Board. */
        WinState? CheckNextMoves(Board GameBoard)
        {
            // will be randomized from all possible moves
            int nextMove;

            // play a single move, then keep playing checking
            // every move to find the best ones
            while (!GameBoard.CheckForWinner())
            {
                List<int> possibleMoves = GameBoard.GetPossibleMoves();

                // choose a random move from the possible moves
                nextMove = rnd.Next(0, possibleMoves.Count - 1);

                GameBoard.MakeMove(possibleMoves[nextMove]);
            }

            // return who would win the game
            return GameBoard.Winner;
        }
    }
}
