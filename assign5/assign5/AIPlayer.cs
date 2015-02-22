using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace assign5
{
    class AIPlayer
    {
        // how deep the minmax alg looks
        private const int DEEPNESS = 4;
        int Strength = 30000;

        MinMax alg = new MinMax();

        public AIPlayer()
        {
            Strength = 40000;
            alg.SetStrength(Strength);
        }

        // use minmax to find best move
        public int TakeTurn(Board GameBoard)
        {
            List<int> possibleMoves = GameBoard.GetPossibleMoves();

            // represents the best move value (start at obsurdly low)
            float best = float.MinValue;

            // the column that the AI will play
            int bestColumn = -1;

            // loop through each column and check the value of the move
            foreach(int col in possibleMoves)
            {
                float value = FindMoveValue(GameBoard, col, 0);

                // if the column has a better value for the AI than the current best move
                // use that column as the best
                if(value > best)
                {
                    best = value;
                    bestColumn = col;
                }
            }
            
            // return the best column fo the AI to play
            return bestColumn;
        }

        // finds the value of a specific move in a column
        // deep is how many moves ahead the alg is looking
        public float FindMoveValue(Board GameBoard, int col, int deep)
        {
            // create temporary Board, make the best move and check for next best
            //Board newBoard = GameBoard.Copy();

            // check if the move will win the game
            //WinState? win = newBoard.CheckWinState();
            WinState? win = GameBoard.CheckWinState();

            // if the game is going to end with the move
            if (win != null)
            {
                // check if it will end with draw
                if (win == WinState.TIE)
                    return 0f;
                // return 1 (best) for win, and -1 (worst) for lose
                //else if (win == WinState.BLACKWIN && Game1.AIColor == BoardState.BLACK)
                else if(win == WinState.BLACKWIN && GameBoard.CurrentPlayer == BoardState.BLACK)
                    return 1f;
                //else if (win == WinState.REDWIN && Game1.AIColor == BoardState.RED)
                else if(win == WinState.REDWIN && GameBoard.CurrentPlayer == BoardState.RED)
                    return 1f;
                else
                    return -1f;
            }            

            // if we have looked forward the maximum amount
            // return the value of the move
            if (deep == DEEPNESS)
            {
                // MCScore
                int newStrength = Convert.ToInt32(Strength / ((double)Math.Pow(7, DEEPNESS)));
                alg.SetStrength(newStrength);

                return alg.FindDeepValue(GameBoard, col);
            }

            //newBoard.MakeMove(col);
            GameBoard.MakeMove(col);

            // Get the possible moves for the newBoard (the next move would be players)
            List<int> possibleMoves = GameBoard.GetPossibleMoves(); //newBoard.GetPossibleMoves();

            // start looking into deeper moves
            float value = float.MinValue;
            foreach (int col2 in possibleMoves)
                value = Math.Max(value, -1f * FindMoveValue(GameBoard, col2, deep + 1));

            // remove the last move made so it doesnt stay permanent
            GameBoard.Unmove(col);

            return value;
        }

        
    }
}
