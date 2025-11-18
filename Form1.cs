using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake_Game
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>(); //List for the body of the snake
        private Circle food = new Circle(); //Normal food (plus 1)
        private Circle bonusFood = new Circle(); // Plus 5 Food and not the normal one
        private bool bonusFoodActive = false;
        private int bonusFoodTimer = 0;
        private Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();

            
            this.BackColor = Color.FromArgb(26, 26, 46); //Dark background
            this.DoubleBuffered = true;

            new Settings();

            gameTimer.Interval = 1000 / Settings.Speed;
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
            canvas.SmoothingMode = SmoothingMode.AntiAlias;

            if (Settings.GameOver == false)
            {
                // Draw snake with modern gradient
                for (int i = 0; i < Snake.Count; i++)
                {
                    Rectangle rect = new Rectangle(
                        Snake[i].X * Settings.Width,
                        Snake[i].Y * Settings.Height,
                        Settings.Width, Settings.Height);

                    if (i == 0)
                    {
                        // Head - gradient from dark to light green
                        using (LinearGradientBrush brush = new LinearGradientBrush(
                            rect, Color.FromArgb(34, 197, 94), Color.FromArgb(22, 163, 74), 45f))
                        {
                            canvas.FillEllipse(brush, rect);
                        }
                        // Snake eyes
                        using (SolidBrush eyeBrush = new SolidBrush(Color.White))
                        {
                            int eyeSize = Settings.Width / 4;
                            canvas.FillEllipse(eyeBrush,
                                Snake[i].X * Settings.Width + Settings.Width / 3,
                                Snake[i].Y * Settings.Height + Settings.Height / 3,
                                eyeSize, eyeSize);
                            canvas.FillEllipse(eyeBrush,
                                Snake[i].X * Settings.Width + Settings.Width * 2 / 3,
                                Snake[i].Y * Settings.Height + Settings.Height / 3,
                                eyeSize, eyeSize);
                        }
                    }
                    else
                    {
                        // Body - lighter green with gradient
                        using (LinearGradientBrush brush = new LinearGradientBrush(
                            rect, Color.FromArgb(74, 222, 128), Color.FromArgb(134, 239, 172), 45f))
                        {
                            canvas.FillEllipse(brush, rect);
                        }
                    }

                    // Add outline
                    using (Pen pen = new Pen(Color.FromArgb(22, 163, 74), 2))
                    {
                        canvas.DrawEllipse(pen, rect);
                    }
                }

                // Drawing of the regular food with green gradient
                Rectangle foodRect = new Rectangle(
                    food.X * Settings.Width,
                    food.Y * Settings.Height,
                    Settings.Width, Settings.Height);

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    foodRect, Color.FromArgb(239, 68, 68), Color.FromArgb(220, 38, 38), 45f))
                {
                    canvas.FillEllipse(brush, foodRect);
                }
                using (Pen pen = new Pen(Color.FromArgb(185, 28, 28), 2))
                {
                    canvas.DrawEllipse(pen, foodRect);
                }

                // Draw bonus food if active
                if (bonusFoodActive)
                {
                    Rectangle bonusRect = new Rectangle(
                        bonusFood.X * Settings.Width - Settings.Width / 4,
                        bonusFood.Y * Settings.Height - Settings.Height / 4,
                        (int)(Settings.Width * 1.5), (int)(Settings.Height * 1.5));

                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        bonusRect, Color.FromArgb(251, 191, 36), Color.FromArgb(245, 158, 11), 45f))
                    {
                        canvas.FillEllipse(brush, bonusRect);
                    }
                    using (Pen pen = new Pen(Color.FromArgb(217, 119, 6), 3))
                    {
                        canvas.DrawEllipse(pen, bonusRect);
                    }

                    // Draw sparkle effect
                    using (Pen sparkle = new Pen(Color.White, 2))
                    {
                        int centerX = bonusFood.X * Settings.Width + Settings.Width / 2;
                        int centerY = bonusFood.Y * Settings.Height + Settings.Height / 2;
                        canvas.DrawLine(sparkle, centerX - 5, centerY, centerX + 5, centerY);
                        canvas.DrawLine(sparkle, centerX, centerY - 5, centerX, centerY + 5);
                    }
                }
            }
            else
            {
                // Better looking game over screen with modern feel
                string gameOver = "GAME OVER";
                string score = $"Score: {Settings.Score}";
                string restart = "Press ENTER to restart";

                Font titleFont = new Font("Segoe UI", 32, FontStyle.Bold);
                Font scoreFont = new Font("Segoe UI", 24, FontStyle.Regular);
                Font restartFont = new Font("Segoe UI", 14, FontStyle.Regular);

                SizeF gameOverSize = canvas.MeasureString(gameOver, titleFont);
                SizeF scoreSize = canvas.MeasureString(score, scoreFont);
                SizeF restartSize = canvas.MeasureString(restart, restartFont);

                float centerX = pictureBox1.Width / 2;
                float centerY = pictureBox1.Height / 2;

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(239, 68, 68)))
                {
                    canvas.DrawString(gameOver, titleFont, brush,
                        centerX - gameOverSize.Width / 2, centerY - 80);
                }

                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    canvas.DrawString(score, scoreFont, brush,
                        centerX - scoreSize.Width / 2, centerY - 20);
                    canvas.DrawString(restart, restartFont, brush,
                        centerX - restartSize.Width / 2, centerY + 40);
                }
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
            bonusFoodActive = false;
            bonusFoodTimer = 0;

            generateFood();
        }

        private void movePlayer()
        {
            // Move body segments first
            for (int i = Snake.Count - 1; i > 0; i--)
            {
                Snake[i].X = Snake[i - 1].X;
                Snake[i].Y = Snake[i - 1].Y;
            }

            // Move the head
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

            // Boundary wrapping
            int maxXpos = pictureBox1.Size.Width / Settings.Width;
            int maxYpos = pictureBox1.Size.Height / Settings.Height;

            if (Snake[0].X < 0) Snake[0].X = maxXpos - 1;
            else if (Snake[0].X >= maxXpos) Snake[0].X = 0;

            if (Snake[0].Y < 0) Snake[0].Y = maxYpos - 1;
            else if (Snake[0].Y >= maxYpos) Snake[0].Y = 0;

            // Self-collision detection
            for (int j = 1; j < Snake.Count; j++)
            {
                if (Snake[0].X == Snake[j].X && Snake[0].Y == Snake[j].Y)
                {
                    die();
                }
            }

            // Regular food detection
            if (Snake[0].X == food.X && Snake[0].Y == food.Y)
            {
                eat();
            }

            // Bonus food detection
            if (bonusFoodActive && Snake[0].X == bonusFood.X && Snake[0].Y == bonusFood.Y)
            {
                eatBonusFood();
            }

            // Bonus food timer management
            bonusFoodTimer++;
            if (bonusFoodTimer >= 100 && !bonusFoodActive) // Spawn bonus food every ~5 seconds
            {
                generateBonusFood();
                bonusFoodTimer = 0;
            }

            if (bonusFoodActive && bonusFoodTimer >= 150) // Bonus food disappears after ~7.5 seconds
            {
                bonusFoodActive = false;
                bonusFoodTimer = 0;
            }
        }

        private void generateFood()
        {
            int maxXpos = pictureBox1.Size.Width / Settings.Width;
            int maxYpos = pictureBox1.Size.Height / Settings.Height;

            food = new Circle { X = rnd.Next(0, maxXpos), Y = rnd.Next(0, maxYpos) };
        }

        private void generateBonusFood()
        {
            int maxXpos = pictureBox1.Size.Width / Settings.Width;
            int maxYpos = pictureBox1.Size.Height / Settings.Height;

            bonusFood = new Circle { X = rnd.Next(0, maxXpos), Y = rnd.Next(0, maxYpos) };
            bonusFoodActive = true;
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

        private void eatBonusFood()
        {
            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);
            Settings.Score += Settings.Points * 5; // 5x points for bonus food
            label2.Text = Settings.Score.ToString();
            bonusFoodActive = false;
            bonusFoodTimer = 0;
        }

        private void die()
        {
            Settings.GameOver = true;
        }

        private void updateScreen(object sender, EventArgs e)
        {
            if (Settings.GameOver == true)
            {
                if (Input.KeyPress(Keys.Enter))
                {
                    startGame();
                }
            }
            else
            {
                if (Input.KeyPress(Keys.Right) && Settings.Direction != Directions.Left)
                {
                    Settings.Direction = Directions.Right;
                }
                else if (Input.KeyPress(Keys.Left) && Settings.Direction != Directions.Right)
                {
                    Settings.Direction = Directions.Left;
                }
                else if (Input.KeyPress(Keys.Up) && Settings.Direction != Directions.Down)
                {
                    Settings.Direction = Directions.Up;
                }
                else if (Input.KeyPress(Keys.Down) && Settings.Direction != Directions.Up)
                {
                    Settings.Direction = Directions.Down;
                }

                movePlayer();
            }

            pictureBox1.Invalidate();
        }
    }
}