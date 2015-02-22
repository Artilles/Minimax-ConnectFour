using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace assign5
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font1, font2;
        Texture2D t_board;
        Texture2D t_red, t_black;

        Board GameBoard;
        BoardState? PlayerColor;
        public static BoardState? AIColor;
        MouseState mouse, lastMouse;

        int width = 7;
        int height = 6;

        Vector2 tileSize = new Vector2(125f, 100f);

        Rectangle r_red;
        Rectangle r_black;
        Rectangle r_reset;

        AIPlayer ai;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 650;
            graphics.PreferredBackBufferWidth = 875;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // initialize the tiles
            GameBoard = new Board(width, height);

            PlayerColor = null;
            AIColor = null;

            r_red = new Rectangle(50, 100, 200, 150);
            r_black = new Rectangle(graphics.GraphicsDevice.Viewport.Width - 250, 100, 200, 150);

            r_reset = new Rectangle(graphics.GraphicsDevice.Viewport.Width - 105, 5, 100, 40);

            ai = new AIPlayer();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font1 = Content.Load<SpriteFont>("font");
            font2 = Content.Load<SpriteFont>("font2");

            t_board = Content.Load<Texture2D>("board");
            t_red = Content.Load<Texture2D>("red");
            t_black = Content.Load<Texture2D>("black");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        void MakeMove(int x)
        {
            int col;
            // get the row and column for the mouse click
            if (GameBoard.CurrentPlayer == PlayerColor)
                col = (int)(x / 125);
            else
                col = x;
            //col = (int)(x / 125);
            GameBoard.MakeMove(col);

            GameBoard.CheckForWinner();
        }

        void SelectColor(int x, int y)
        {
            if (r_red.Contains(new Point(x, y)))
            {
                PlayerColor = BoardState.RED;
                AIColor = BoardState.BLACK;
            }
            else if (r_black.Contains(new Point(x, y)))
            {
                PlayerColor = BoardState.BLACK;
                AIColor = BoardState.RED;
            }
        }

        void ResetGame()
        {
            GameBoard = new Board(7, 6);
            ai = new AIPlayer();
            PlayerColor = null;
            AIColor = null;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            mouse = Mouse.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (PlayerColor == null)
            {
                if (mouse.LeftButton == ButtonState.Released
                    && lastMouse.LeftButton == ButtonState.Pressed)
                {
                    SelectColor(mouse.X, mouse.Y);
                }
            }
            else
            {
                // Game is not over, keep checking for mouse clicks
                if (!GameBoard.isGameOver)
                {
                    if (GameBoard.CurrentPlayer == PlayerColor)
                    {
                        // It is the players turn
                        if (mouse.LeftButton == ButtonState.Released && lastMouse.LeftButton == ButtonState.Pressed)
                        {
                            MakeMove(mouse.X);
                           // GameBoard.CheckForWinner();
                        }
                    }
                    else if(GameBoard.CurrentPlayer == AIColor && !GameBoard.isGameOver)
                    {
                        Board newBoard = GameBoard.Copy();
                        // It is the AI's turn
                        MakeMove(ai.TakeTurn(GameBoard));
                        /*if (mouse.LeftButton == ButtonState.Released && lastMouse.LeftButton == ButtonState.Pressed)
                        {
                            MakeMove(mouse.X);
                        }*/
                        //GameBoard.CheckForWinner();
                    }
                }

                if (mouse.LeftButton == ButtonState.Released && lastMouse.LeftButton == ButtonState.Pressed)
                {
                    if (r_reset.Contains(mouse.X, mouse.Y))
                        ResetGame();
                }
            }

            lastMouse = mouse;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.NavajoWhite);

            spriteBatch.Begin();

            // Check if the player has selected a color
            if (PlayerColor != null)
            {
                // Player has chosen a color, start drawing the tiles

                // Loop through all the board tiles, check the state and draw tokens
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        // Get the BoardState for each Tile to see who owns it
                        if (GameBoard.GetTile(row, col) == BoardState.BLACK)
                        {
                            Rectangle rect = new Rectangle(col * 125,
                                graphics.GraphicsDevice.Viewport.Height - (row * 100) - 100, 125, 100);
                            spriteBatch.Draw(t_black, rect, Color.White);
                        }
                        else if (GameBoard.GetTile(row, col) == BoardState.RED)
                        {
                            Rectangle rect = new Rectangle(col * 125,
                                graphics.GraphicsDevice.Viewport.Height - (row * 100) - 100, 125, 100);
                            spriteBatch.Draw(t_red, rect, Color.White);
                        }
                    }
                }

                // Draw the game board
                spriteBatch.Draw(t_board, new Rectangle(0, 50, 875, 600), Color.White);

                // Draw the game information (player color and who's turn)
                spriteBatch.DrawString(font2, "Player Color: " + PlayerColor.ToString(),
                    new Vector2(10, 10), Color.Black);

                if (GameBoard.CurrentPlayer == PlayerColor)
                    spriteBatch.DrawString(font2, "Your Turn",
                        new Vector2((int)(graphics.GraphicsDevice.Viewport.Width / 2 - (font2.MeasureString("Your Turn").X / 2)), 10), Color.Black);
                else
                    spriteBatch.DrawString(font2, "AI Turn",
                        new Vector2((int)(graphics.GraphicsDevice.Viewport.Width / 2 - (font2.MeasureString("AI Turn").X / 2)), 10), Color.Black);

                // Draw the RESET button
                Texture2D t_reset = new Texture2D(graphics.GraphicsDevice, r_reset.Width, r_reset.Height);
                Color[] data = new Color[t_reset.Width * t_reset.Height];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Color.Green;
                t_reset.SetData(data);
                spriteBatch.Draw(t_reset, r_reset, Color.Green);
                spriteBatch.DrawString(font2, "RESET", new Vector2((int)(820 - (font2.MeasureString("RESET").X / 2)), 12), Color.LightBlue);

                // If the game is over, draw text of who won
                if (GameBoard.isGameOver)
                {
                    // draw winner text here
                    if (GameBoard.Winner != WinState.TIE)
                    {
                        string winner = "The winner is " + GameBoard.CurrentOpponent.ToString() + "!";
                        spriteBatch.DrawString(font1, winner,
                            new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (font1.MeasureString(winner).X / 2),
                                (graphics.GraphicsDevice.Viewport.Height / 2)), Color.Green);
                    }
                    else
                    {
                        string winner = "The game was a TIE!";
                        spriteBatch.DrawString(font1, winner,
                            new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (font1.MeasureString(winner).X / 2),
                                (graphics.GraphicsDevice.Viewport.Height / 2)), Color.Green);
                    }
                }
            }
            // Player has not selected a color, draw the two options
            else
            {
                // Player needs to choose a color
                Texture2D c_red = new Texture2D(graphics.GraphicsDevice, r_red.Width, r_red.Height);
                Texture2D c_black = new Texture2D(graphics.GraphicsDevice, r_black.Width, r_black.Height);

                Color[] data = new Color[c_red.Width * c_red.Height];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Color.Red;
                c_red.SetData(data);
                data = new Color[c_black.Width * c_black.Height];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Color.Black;
                c_black.SetData(data);

                spriteBatch.DrawString(font1, "Choose a Color", 
                    new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - (font1.MeasureString("Choose a Color").X / 2), 30),
                    Color.Black);

                spriteBatch.Draw(c_red, r_red, Color.Red);
                spriteBatch.Draw(c_black, r_black, Color.Black);

                spriteBatch.DrawString(font2, "Go First", new Vector2(100, 260), Color.Black);
                spriteBatch.DrawString(font2, "Go Second", new Vector2(graphics.GraphicsDevice.Viewport.Width - 200, 260), Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
