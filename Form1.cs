using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

using System.IO;

using System.Drawing.Imaging;//voor de jpg compressor
//voor numeric
//using System.ComponentModel;
//using System.Drawing;
//using System.Windows.Forms;


namespace Classic_snake_game
{
    public partial class Form1 : Form
    {
        //var filePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //string resFolder = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Resources");
        //SoundPlayer player = new SoundPlayer(@"C:\Users\Administrator\Music\toek_geluid.wav");
        SoundPlayer start = new SoundPlayer(@"Resources/game-start-6104.wav");
        SoundPlayer gameover = new SoundPlayer(@"resources/game-over-arcade-6435.wav");
        SoundPlayer click = new SoundPlayer(@"resources/click.wav");
        SoundPlayer coin = new SoundPlayer(@"resources/coin.wav");

        private List<Circle> Snake = new List<Circle>();
        private List<Rectangle> wall = new List<Rectangle>();
        //private Circle food = new Circle();
        private List<Circle> food = new List<Circle>();

        int maxWidth;
        int maxHeight;

        bool vol = false;

        bool isClicked = true;
        bool isChecked = false;

        //Image canvasImage = new Image;

        int score;

        //bool image;

        int highScore;

        Random rand = new Random();

        bool goLeft, goRight, goDown, goUp;
        public Form1()
        {
            InitializeComponent();

            new Settings();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.A) && Settings.directions != "right")
            {
                goLeft = true;
                e.SuppressKeyPress = true;
            } else { e.SuppressKeyPress = true; }
            if ((e.KeyCode == Keys.Right||e.KeyCode ==Keys.D) && Settings.directions != "left")
            {
                goRight = true;
                e.SuppressKeyPress = true;
            }
            else { e.SuppressKeyPress = true; }
            if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.W) && Settings.directions != "down")
            {
                goUp = true;
                e.SuppressKeyPress = true;
            }
            else { e.SuppressKeyPress = true; }
            if ((e.KeyCode == Keys.Down || e.KeyCode == Keys.S) && Settings.directions != "up")
            {
                goDown = true;
                e.SuppressKeyPress = true;
            }
            else { e.SuppressKeyPress = true; }
            if ((e.KeyCode == Keys.K || e.KeyCode == Keys.Delete))
            {
                GameOver();
                e.SuppressKeyPress = true;
            }
            else { e.SuppressKeyPress = true; }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
            {
                goDown = false;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {   
            RestartGame();
            //testsound.Play();
        }

        private void TakeSnapShot(object sender, EventArgs e)
        {

            isClicked = false;
            Label caption = new Label();
            caption.Text = "I scored: " + score + " and my Highscore is " + highScore + " on the game Snake";
            caption.Font = new Font("Ariel", 12, FontStyle.Bold);
            caption.ForeColor = Color.Black;
            caption.AutoSize = false;
            caption.Width = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Snake Game";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);
            }
            else isClicked = true;
            {
                picCanvas.Controls.Remove(caption);
            }
            click.Play();
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            if (goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goDown)
            {
                Settings.directions = "down";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }
            // end of directions

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {

                    switch (Settings.directions)
                    {
                        case "left":
                            Snake[i].X--;
                            break;
                        case "right":
                            Snake[i].X++;
                            break;
                        case "down":
                            Snake[i].Y++;
                            break;
                        case "up":
                            Snake[i].Y--;
                            break;
                    }

                    if (Snake[i].X < 0)
                    {
                        Snake[i].X = maxWidth;
                    }
                    if (Snake[i].X > maxWidth)
                    {
                        Snake[i].X = 0;
                    }
                    if (Snake[i].Y < 0)
                    {
                        Snake[i].Y = maxHeight;
                    }
                    if (Snake[i].Y > maxHeight)
                    {
                        Snake[i].Y = 0;
                    }

                    for (int d = 0; d < food.Count; d++)
                    {
                        if (Snake[i].X == food[d].X && Snake[i].Y == food[d].Y)
                        {
                            //Circle eten = new Circle { X = food[d].X, Y = food[d].Y };
                            food.RemoveAt(d);

                            EatFood();
                        }
                    }

                    for (int j = 1; j < Snake.Count; j++)
                    {

                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            GameOver();
                        }

                    }

                    for (int j = 1; j < wall.Count; j++)
                    {

                        if (Snake[i].X == wall[j].X || Snake[i].Y == wall[j].Y)
                        {
                            GameOver();
                        }

                    }

                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
            picCanvas.Invalidate();
        }

        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;

            for (int i = 0; i < Snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                { 
                    snakeColour = Brushes.DarkGreen;
                }

                canvas.FillEllipse(snakeColour, new Rectangle
                    (
                    Snake[i].X * Settings.Width,
                    Snake[i].Y * Settings.Height,
                    Settings.Width, Settings.Height
                    ));
            }
            for (int i = 0; i < food.Count; i++)
            {
                    canvas.FillEllipse(Brushes.DarkRed, new Rectangle
                (
                food[i].X * Settings.Width,
                food[i].Y * Settings.Height,
                Settings.Width, Settings.Height
                ));
            }
            for (int i = 0; i < wall.Count; i++)
            {
                canvas.FillRectangle(Brushes.BlueViolet, new Rectangle
            (
            wall[i].X * Settings.Width,
            wall[i].Y * Settings.Height,
            wall[i].Width, wall[i].Height
            ));
            }


        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {   
            int gameSpeed = Convert.ToInt32(Math.Round(numericUpDown1.Value, 0));
            
            if (gameSpeed <= 0)
            {
                gameSpeed = 1;
            }
            gameTimer.Interval = gameSpeed;
            click.Play();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //player.Play();
            click.Play();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {   
            int foodAmount = Convert.ToInt32(Math.Round(numericUpDown2.Value, 0));
            click.Play();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e) //fullscreen
        {

            if (fullscreen.Text == "full screen")
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                fullscreen.Text = "exit full screen";
            }
            else if (fullscreen.Text == "exit full screen")
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
                fullscreen.Text = "full screen";
            }
            click.Play();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            isChecked = radioButton1.Checked;
            click.Play();
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked && !isChecked)
                radioButton1.Checked = false;
            else
            {
                radioButton1.Checked = true;
                isChecked = false;
            }
        }


        private void picCanvas_Click(object sender, EventArgs e)
        {   
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            int formWidth = this.Width;
            numericUpDown3.Maximum = formWidth-184;
            click.Play();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            int formHeight = this.Height;
            numericUpDown4.Maximum = formHeight - 84 ;
            click.Play();
        }

        private void RestartGame()
        {


            //locatie van alles voor de dinamiese veranderingen
            int formheight = this.Height;
            int formWidth = this.Width;
            int canvasX = Convert.ToInt32(Math.Round(numericUpDown3.Value, 0));
            canvasX -= canvasX % 16;
             numericUpDown3.Value = canvasX;


            int canvasY = Convert.ToInt32(Math.Round(numericUpDown4.Value, 0));// - 84
            canvasY -= canvasY % 16;
            numericUpDown4.Value = canvasY;

            this.picCanvas.Size = new Size(canvasX, canvasY);

            maxWidth = picCanvas.Width / Settings.Width - 1 ;
            maxHeight = picCanvas.Height / Settings.Height - 1;

            exit.Location = new Point(canvasX + 34, exit.Location.Y);
            startButton.Location = new Point(canvasX + 33, startButton.Location.Y);
            radioButton1.Location = new Point(canvasX + 45, radioButton1.Location.Y);
            fullscreen.Location = new Point(canvasX + 34, fullscreen.Location.Y);
            button1.Location = new Point(canvasX + 34, button1.Location.Y);
            snapButton.Location = new Point(canvasX + 33, snapButton.Location.Y);
            numericUpDown1.Location = new Point(canvasX + 45, numericUpDown1.Location.Y);
            numericUpDown2.Location = new Point(canvasX + 45, numericUpDown2.Location.Y);
            numericUpDown3.Location = new Point(canvasX + 45, numericUpDown3.Location.Y);
            numericUpDown4.Location = new Point(canvasX + 45, numericUpDown4.Location.Y);
            txtScore.Location = new Point(canvasX + 34, txtScore.Location.Y);
            txtHighScore.Location = new Point(canvasX + 34, txtHighScore.Location.Y);
            Game_speed.Location = new Point(canvasX + 42, Game_speed.Location.Y);
            label1.Location = new Point(canvasX + 42, label1.Location.Y);
            label2.Location = new Point(canvasX + 42, label2.Location.Y);
            label3.Location = new Point(canvasX + 42, label3.Location.Y);
            label4.Location = new Point(canvasX + 15, label4.Location.Y);
            label5.Location = new Point(canvasX + 33, label5.Location.Y);



            Settings.directions = "left";

            Snake.Clear();
            food.Clear();
            wall.Clear();

            startButton.Enabled = false; //knop uit
            snapButton.Enabled = false;
            radioButton1.Enabled = false;
            exit.Enabled = false;
            button1.Enabled = false;
            fullscreen.Enabled = false;
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            numericUpDown3.Enabled = false;
            numericUpDown4.Enabled = false;
            score = 0;
            txtScore.Text = $"Score: {score}";

            if (radioButton1.Checked == true)
            {
/*                Rectangle start = new Rectangle { X = 0, Y = 0 };
                wall.Add(start);*/

                for (int i = 0; i < 1; i++)//spawn lengte van de muur 545  picCanvas.Width / 15.95
                {
                    Rectangle muur = new Rectangle { X = i, Y = 0, Width = picCanvas.Width, Height = 16 };
                    Rectangle muur2 = new Rectangle { X = i, Y = picCanvas.Height / 16-1, Width = picCanvas.Width, Height = 16 };
                    wall.Add(muur2);
                    wall.Add(muur);
                }

                for (int i = 0; i < 1; i++)//spawn lengte van de muur 545 picCanvas.Height / 15.95
                {
                    Rectangle muur = new Rectangle { X = 0, Y = i, Width = 16, Height = picCanvas.Height };
                    Rectangle muur2 = new Rectangle { X = picCanvas.Width/16  -1  , Y = i, Width = 16, Height = picCanvas.Height };
                    wall.Add(muur2);
                    wall.Add(muur);
                }
            }

            Circle head = new Circle { X = 25, Y = 5 };
            Snake.Add(head); // adding the head part of the snake to the list

            for (int i = 0; i < 10; i++)//spawn lengte van de slang
            {
                Circle body = new Circle();
                Snake.Add(body);
            }

            Circle eten = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };
            food.Add(eten);

            gameTimer.Start();

             start.Play();


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // create reader & open file
            TextReader tr = new StreamReader(@"resources/SavedGame.txt");

            // read lines of text
            string xCoordString = tr.ReadLine();


            //Convert the strings to int
            highScore = Convert.ToInt32(xCoordString);

            // close the stream
            tr.Close();

            txtHighScore.Text = $"High Score: {Environment.NewLine}{highScore}";
            txtHighScore.ForeColor = Color.Maroon;
            txtHighScore.TextAlign = ContentAlignment.MiddleCenter;

             picCanvas.Image = Image.FromFile(@"resources/achtergrond.png");
        }


        private void EatFood()
        {
            //testsound.Play();
            int gameSpeed = Convert.ToInt32(Math.Round(numericUpDown1.Value, 0));
            int foodAmount = Convert.ToInt32(Math.Round(numericUpDown2.Value, 0));
            
            if (radioButton1.Checked)
            {
                score += 200 / gameSpeed/foodAmount;
            }
            else
            {
                score += 100 / gameSpeed/foodAmount;
            }
            //score += 1;

            txtScore.Text = $"Score: {score}";

            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);

            //int niks = 1;
            //int coordinaten = Circle.Join(Snake);
            if (!vol)
            {
                for (int i = 0; i < foodAmount; i++)
                {
                    int min;
                    if (isChecked)
                    {
                        min = 1;
                    }
                    else
                    {
                        min = 0;
                    }

                    int x = getalBehalveX(Snake[0].X);//uitzondering
                    int y = getalBehalveY(Snake[0].Y);
                    Circle z = SpawnPosition(Snake, food, maxWidth-min, maxHeight-min);
                    Circle eten = new Circle { X = z.X /*rand.Next(2, maxWidth)*/, Y = z.Y/*rand.Next(2, maxHeight)*/ }; ;
                    food.Add(eten);
                }
            }

            if (foodAmount<4)
            {
                coin.Play();
            }
        }

        private Circle SpawnPosition(List<Circle> snake, List<Circle> food, int maxX, int maxY)
        {   
            var result = new List<Circle>();
            int min;
            if (isChecked)
            {
                min = 1;
            } else
            {
                min = 0;
            }
            //iterate over game field and find points that are outside of snake
            for (int y = min; y <= maxY; y++)
            {
                for (int x = min; x <= maxX; x++)
                {
                    if (!snake.Any(p => p.X == x && p.Y == y)&& !food.Any(p => p.X == x && p.Y == y))
                    {
                        result.Add(new Circle { X = x, Y = y });
                    }
                }
            }
            //return random point from found
            var rnd = new Random();

            try
            {
                Circle cirkel = result[rand.Next(result.Count)];
                return cirkel;
            }
            catch
            {
                Circle cirkel = new Circle { X = 0, Y = 0 };
                vol = true;
                return cirkel;
            }

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            int formWidth = this.Width;
            numericUpDown3.Maximum = formWidth - 184;
            int formHeight = this.Height;
            numericUpDown4.Maximum = formHeight - 84;

            numericUpDown3.Value = formWidth - 184;
            numericUpDown4.Value = formHeight - 84;
            click.Play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private int getalBehalveX(int except)
        {
            int number;
            do
            {
                number = rand.Next(2, maxWidth);
            } while (number == except);
            
            return number;
        }

        private int getalBehalveY(int except)
        {
            int number;
            do
            {
                number = rand.Next(2, maxHeight);
            } while (number == except);

            return number;
        }

        private void GameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true; //knop aan
            snapButton.Enabled = true;
            fullscreen.Enabled = true;
            radioButton1.Enabled = true;
            exit.Enabled = true;
            button1.Enabled = true;
            numericUpDown1.Enabled = true;
            numericUpDown2.Enabled = true;
            numericUpDown3.Enabled = true;
            numericUpDown4.Enabled = true;

            vol = false;


            if (score>highScore)
            {
                highScore = score;

                txtHighScore.Text = $"High Score: {Environment.NewLine}{highScore}";
                txtHighScore.ForeColor = Color.Maroon;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;

                //TextWriter tw = new StreamWriter(@"resources/SavedGame.txt");

                // write lines of text to the file
                //tw.WriteLine(highScore);

                // close the stream     
                //tw.Close();
/*                String newHigh;
                newHigh = highScore.ToString();

                string text = File.ReadAllText("resources/SavedGame.txt");
                text = text.Replace(text, newHigh);
                File.WriteAllText("resources/SavedGame.txt", text);*/
            }
            else
            {
                txtHighScore.Text = $"High Score: {Environment.NewLine}{highScore}";
                txtHighScore.ForeColor = Color.Maroon;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;
            }
            gameover.Play();
        }
    }
}

//geluid
//uiterlijk verbeteren
//zelf op eet bug op lossen
//achtegrond optie
//performence problemen oplossen
//te veel geluid achter elkaar