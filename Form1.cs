using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake_Game
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>(); //A list for the body of the snake
        private Circle food = new Circle(); //The food for the snake
        public Form1()
        {
            InitializeComponent();
            new Settings(); //Linking setting to this class of ours

            gameTimer.Interval = 1000 / Settings.Speed; // Changing of the game timers to the settings speed
            gameTimer.Tick += updateScreen;
            gameTimer.Start();

            startGame();

        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            Input.changeState(e.KeyCode, true);
        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            Input.changeState(e.KeyCode, false);
        }

        private void updateGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if(Settings.GameOver == false)
            {
                //If the game has not ended

                Brush snakeColour;

                for(int i = 0; i < Snake.Count; i++)
                {
                    if (i == 0)
                    {
                        snakeColour = Brushes.Black; //Head of the snake is black
                    }
                    else
                    {
                        snakeColour = Brushes.Green;
                    }

                    //Drawing of the Snake and its body
                    canvas.FillEllipse(snakeColour,
                                            new Rectangle(
                                                Snake[i].X * Settings.Width,
                                                Snake[i].Y * Settings.Height,
                                                Settings.Width, Settings.Height
                                                ));
                }

                //Drawing of the food
                canvas.FillEllipse(Brushes.Red,
                                    new Rectangle(
                                        food.X * Settings.Width,
                                        food.Y * Settings.Height,
                                        Settings.Width, Settings.Height
                                        ));
            }
            else
            {
                //For when the game is over

                string gameOver = "Game Over \n" + "Your Final Score is " + Settings.Score + "\n Press enter to Restart \n";
                label3.Text = gameOver;
                label3.Visible = true;
            }
        }

        private void startGame()
        {
            label3.Visible = false;
            new Settings();
            Snake.Clear();
            Circle head = new Circle { X = 10, Y = 5 };
            Snake.Add(head);

            label2.Text = Settings.Score.ToString();

            generateFood();

        }

        private void movePlayer()
        {
            // Move body segments first (from tail to neck)
            for (int i = Snake.Count - 1; i > 0; i--)
            {
                Snake[i].X = Snake[i - 1].X;
                Snake[i].Y = Snake[i - 1].Y;
            }

            // Then move the head
            switch (Settings.Direction)
            {
                case Directions.Right:
                    Snake[0].X++;
                    break;
                case Directions.Left:
                    Snake[0].X--;
                    break;
                case Directions.Down:
                    Snake[0].Y++;
                    break;
                case Directions.Up:
                    Snake[0].Y--;
                    break;
            }

            // Boundary wrapping - snake comes out the other side
            int maxXpos = pictureBox1.Size.Width / Settings.Width;
            int maxYpos = pictureBox1.Size.Height / Settings.Height;

            if (Snake[0].X < 0)
            {
                Snake[0].X = maxXpos - 1;
            }
            else if (Snake[0].X >= maxXpos)
            {
                Snake[0].X = 0;
            }

            if (Snake[0].Y < 0)
            {
                Snake[0].Y = maxYpos - 1;
            }
            else if (Snake[0].Y >= maxYpos)
            {
                Snake[0].Y = 0;
            }

            // Self-collision detection
            for (int j = 1; j < Snake.Count; j++)
            {
                if (Snake[0].X == Snake[j].X && Snake[0].Y == Snake[j].Y)
                {
                    die();
                }
            }

            // Food detection
            if (Snake[0].X == food.X && Snake[0].Y == food.Y)
            {
                eat();
            }
        }

        private void generateFood()
        {
            int maxXpos = pictureBox1.Size.Width / Settings.Width;
            int maxYpos = pictureBox1.Size.Height / Settings.Height;

            Random rnd = new Random();
            food = new Circle { X = rnd.Next(0, maxXpos), Y = rnd.Next(0, maxYpos) };
        }

        private void eat()
        {
            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);
            Settings.Score += Settings.Points;
            label2.Text = Settings.Score.ToString();
            generateFood();
        }

        private void die()
        {
            Settings.GameOver = true;
        }

        private void updateScreen(object sender, EventArgs e)
        {
            //Each tick of the timer runs this very function 

            if (Settings.GameOver == true)
            {
                if (Input.KeyPress(Keys.Enter))
                {
                    startGame(); //If the game is over and enter is pressed then start the game
                }
            }
            else //If the game is not over then the following must be done
            {
                if(Input.KeyPress(Keys.Right) && Settings.Direction != Directions.Left)
                {
                    Settings.Direction = Directions.Right;
                }
                else if(Input.KeyPress(Keys.Left) && Settings.Direction != Directions.Right)
                {
                    Settings.Direction = Directions.Left;
                }
                else if(Input.KeyPress(Keys.Up) && Settings.Direction != Directions.Down)
                {
                    Settings.Direction = Directions.Up;
                }
                else if(Input.KeyPress(Keys.Down) && Settings.Direction != Directions.Up)
                {
                    Settings.Direction = Directions.Down;
                }

                movePlayer();
            }

            pictureBox1.Invalidate();
        }
    }
}