using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace assign5
{
    /* State reprentation of the board. */
    public enum BoardState
    {
        EMPTY,
        BLACK,
        RED,
    };

    public enum WinState { BLACKWIN, REDWIN, TIE };

    class Board
    {
        public int Width, Height;
        Int64 numRed, numBlack;

        int[] tilesPerColumn;

        // Double array representing the board pieces
        BoardState[,] theBoard;

        public BoardState CurrentPlayer, CurrentOpponent;
        public WinState? Winner { get; private set; }

        public bool isGameOver { get; private set; }

        public Board(int width, int height)
        {
            Width = width;
            Height = height;

            numRed = numBlack = 0;

            CurrentPlayer = BoardState.RED;
            CurrentOpponent = BoardState.BLACK;
            Winner = null;

            isGameOver = false;

            // initialize an array of all possible tiles
            // the number will represent which player holds the tile
            tilesPerColumn = new int[Width];
            for (int i = 0; i < Width; i++)
                tilesPerColumn[i] = 0;

            theBoard =  new BoardState[height, width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                    theBoard[i, j] = BoardState.EMPTY;
            }
        }

        // Creates a copy of the Board, so that when checking
        // future moves, it doesn't affect the current board
        public Board Copy()
        {
            Board newBoard = new Board(Width, Height);
            newBoard.CurrentPlayer = this.CurrentPlayer;
            newBoard.CurrentOpponent = this.CurrentOpponent;
            newBoard.numRed = this.numRed;
            newBoard.numBlack = this.numBlack;
            for (int i = 0; i < Width; i++)
                newBoard.tilesPerColumn[i] = this.tilesPerColumn[i];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                    newBoard.theBoard[i, j] = this.theBoard[i, j];
            }
            newBoard.isGameOver = this.isGameOver;
            newBoard.Winner = this.Winner;

            return newBoard;
        }

        // Gets all possible moves (7 or less) for the AI player
        public List<int> GetPossibleMoves()
        {
            List<int> possibleMoves = new List<int>();

            for (int i = 0; i < Width; i++)
            {
                if (CheckMove(i))
                    possibleMoves.Add(i);
            }
            return possibleMoves;
        }

        public void NextPlayer()
        {
            CurrentPlayer = (CurrentPlayer == BoardState.BLACK) ? BoardState.RED : BoardState.BLACK;
            CurrentOpponent = (CurrentOpponent == BoardState.BLACK) ? BoardState.RED : BoardState.BLACK;
        }

        /* Checks if the column specified is full. */
        public bool CheckMove(int column)
        {
            if (column > 6 || column < 0)
                return false;
            return tilesPerColumn[column] < Height;
        }

        /* Returns the current holder of a specified tile, or returns empty. */
        public BoardState GetTile(int row, int col)
        {
            if(theBoard[row, col] == BoardState.BLACK)
                return BoardState.BLACK;
            else if(theBoard[row, col] == BoardState.RED)
                return BoardState.RED;
            else
                return BoardState.EMPTY;
        }

        public void MakeMove(int col)
        {
            // Check if the move is valid, then place the piece
            if (CheckMove(col))
            {
                // change the tile to be held by the current player
                theBoard[tilesPerColumn[col], col] = CurrentPlayer;
                
                

                // increase the amount of black or red pieces on the board
                // used for determining the winner
                long position = ((long)1 << (tilesPerColumn[col] + 7 * col));
                if (CurrentPlayer == BoardState.BLACK)
                    numBlack ^= position;
                else
                    numRed ^= position;

                // increase the number of tiles in the column
                tilesPerColumn[col]++;
                
                // check for a winner otherwise proceed to next player
                /*if ((Winner = CheckWinState()) != null)
                {
                    isGameOver = true;
                    return;
                }
                else*/
                    NextPlayer();
            }
        }

        public bool CheckForWinner()
        {
            if ((Winner = CheckWinState()) != null)
            {
                isGameOver = true;
                return true;
            }
            else 
                return false;
        }

        public WinState? CheckWinState()
        {
            // check the current player for winstate
            // if the winstate is still null after return, move to next player

            // check for a draw
            bool draw = true;
            for (int col = 0; col < Width; col++)
            {
                if (tilesPerColumn[col] != 6)
                    draw = false;
            }
            if (draw)
                return WinState.TIE;

            if (CurrentPlayer == BoardState.RED)
            {
                long testDLR = numBlack & (numBlack >> 6);
                long testH = numBlack & (numBlack >> 7);
                long testDRL = numBlack & (numBlack >> 8);
                long testV = numBlack & (numBlack >> 1);               
                
                // check for horizontal win
                if ((testH & (testH >> 2 * 7)) > 0)
                    return WinState.BLACKWIN;

                // check vertical win
                if ((testV & (testV >> 2)) > 0)
                    return WinState.BLACKWIN;

                // check diagonal left to right
                if ((testDLR & (testDLR >> 2 * 6)) > 0)
                    return WinState.BLACKWIN;

                // check diagonal right to left
                if ((testDRL & (testDRL >> 2 * 8)) > 0)
                    return WinState.BLACKWIN;
            }
            else
            {
                long testDLR = numRed & (numRed >> 6);
                long testH = numRed & (numRed >> 7);
                long testDRL = numRed & (numRed >> 8);
                long testV = numRed & (numRed >> 1);

                // check for horizontal win
                if ((testH & (testH >> 2 * 7)) > 0)
                    return WinState.REDWIN;

                // check vertical win
                if ((testV & (testV >> 2)) > 0)
                    return WinState.REDWIN;

                // check diagonal left to right
                if ((testDLR & (testDLR >> 2 * 6)) > 0)
                    return WinState.REDWIN;

                // check diagonal right to left
                if ((testDRL & (testDRL >> 2 * 8)) > 0)
                    return WinState.REDWIN;
            }

            return null;
        }

        // remove the last move made and change back to the other player
        public void Unmove(int col)
        {
            tilesPerColumn[col]--;
            long position = ((long)1 << (tilesPerColumn[col] + 7 * col));

            if (CurrentOpponent == BoardState.BLACK)
                numBlack ^= position;
            else
                numRed ^= position;

            theBoard[tilesPerColumn[col], col] = BoardState.EMPTY;

            NextPlayer();
        }
    }
}
