using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WiiRemote
{
    public class Board
    {
        private int Rows;
        private int Cols;

        private int RowsNext;
        private int ColsNext;

        public bool GameOver { get; set; } = false;

        public int Score { get; set; } = 0;
        public int LinesFilled { get; set; } = 0;

        private Block currBlock;
        private Block nextBlock;
        private Block nextNextBlock;

        private Label[,] BlockControls;
        private Label[,] BlockControlsNext;
        private Label[,] BlockControlsNextNext;

        private static Brush NoBrush = Brushes.Transparent;
        private static Brush SilverBrush = Brushes.Gray;



        public MediaPlayer Rotate = new MediaPlayer();
        public MediaPlayer Fix = new MediaPlayer();

        public MediaPlayer Start = new MediaPlayer();
        public MediaPlayer Over = new MediaPlayer();

        public MediaPlayer Clear = new MediaPlayer();
        public MediaPlayer TripleClear = new MediaPlayer();
        public MediaPlayer DoubleClear = new MediaPlayer();
        public MediaPlayer Tetris = new MediaPlayer();

        public Controller Controller { get; set; }

        public bool Mute;


        public Board(Grid TetrisGrid, Grid NextGrid, Grid NextNextGrid, Controller controller)
        {
            Controller = controller;

            Rows = TetrisGrid.RowDefinitions.Count;
            Cols = TetrisGrid.ColumnDefinitions.Count;

            RowsNext = NextGrid.RowDefinitions.Count;
            ColsNext = NextGrid.ColumnDefinitions.Count;

            BlockControls = new Label[Cols, Rows];
            BlockControlsNext = new Label[ColsNext, RowsNext];
            BlockControlsNextNext = new Label[ColsNext, RowsNext];

            TetrisGrid.Children.Clear();

            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    BlockControls[i, j] = new Label();
                    BlockControls[i, j].Background = NoBrush;
                    BlockControls[i, j].BorderBrush = SilverBrush;
                    BlockControls[i, j].BorderThickness = new Thickness(1);
                    Grid.SetColumn(BlockControls[i, j], i);
                    Grid.SetRow(BlockControls[i, j], j);

                    TetrisGrid.Children.Add(BlockControls[i, j]);
                }
            }

            NextNextGrid.Children.Clear();
            NextGrid.Children.Clear();

            for (int i = 0; i < ColsNext; i++)
            {
                for (int j = 0; j < RowsNext; j++)
                {
                    BlockControlsNext[i, j] = new Label();
                    BlockControlsNext[i, j].Background = NoBrush;
                    BlockControlsNext[i, j].BorderBrush = SilverBrush;
                    BlockControlsNext[i, j].BorderThickness = new Thickness(1);
                    Grid.SetColumn(BlockControlsNext[i, j], i);
                    Grid.SetRow(BlockControlsNext[i, j], j);

                    NextGrid.Children.Add(BlockControlsNext[i, j]);


                    BlockControlsNextNext[i, j] = new Label();
                    BlockControlsNextNext[i, j].Background = NoBrush;
                    BlockControlsNextNext[i, j].BorderBrush = SilverBrush;
                    BlockControlsNextNext[i, j].BorderThickness = new Thickness(1);
                    Grid.SetColumn(BlockControlsNextNext[i, j], i);
                    Grid.SetRow(BlockControlsNextNext[i, j], j);

                    NextNextGrid.Children.Add(BlockControlsNextNext[i, j]);
                }
            }


            currBlock = new Block();
            nextBlock = new Block();
            nextNextBlock = new Block();

            currBlockDraw();
            nextBlocksDraw();

            SelectMusic();


            Rotate.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Resources/rotate_block.wav"));
            Fix.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Resources/SFX_PieceFall.wav"));

            Tetris.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Resources/SFX_SpecialTetris.wav"));
            Clear.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Resources/SFX_SpecialLineClearSingle.wav"));
            DoubleClear.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Resources/SFX_SpecialLineClearDouble.wav"));
            TripleClear.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Resources/SFX_SpecialLineClearTriple.wav"));



            Over.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Resources/SFX_GameOver.wav"));



        }

        private void nextBlocksDraw()
        {
            for (int i = 0; i < ColsNext; i++)
            {
                for (int j = 0; j < RowsNext; j++)
                {
                    BlockControlsNext[i, j].Background = NoBrush;
                    BlockControlsNextNext[i, j].Background = NoBrush;
                }
            }


            Point Position = nextBlock.CurrPosition;
            Point[] Shape = nextBlock.CurrShape;

            foreach (Point S in Shape)
            {
                BlockControlsNext[(int)(S.X) + ((Cols / 2) - 3), (int)(S.Y) + 3].Background = nextBlock.CurrColor;
            }

            Position = nextNextBlock.CurrPosition;
            Shape = nextNextBlock.CurrShape;

            foreach (Point S in Shape)
            {
                BlockControlsNextNext[(int)(S.X) + ((Cols / 2) - 3), (int)(S.Y) + 3].Background = nextNextBlock.CurrColor;
            }


        }

        private void currBlockDraw()
        {
            Point Position = currBlock.CurrPosition;
            Point[] Shape = currBlock.CurrShape;


            if (Position.Y == 0)
            {
                foreach (Point S in Shape)
                {
                    if (BlockControls[(int)(S.X + Position.X) + ((Cols / 2) - 1), (int)(S.Y + Position.Y) + 2].Background != NoBrush)
                    {
                        GameOver = true;
                        break;
                    };
                }
            }


            if (!GameOver)
            {
                foreach (Point S in Shape)
                {
                    BlockControls[(int)(S.X + Position.X) + ((Cols / 2) - 1), (int)(S.Y + Position.Y) + 2].Background = currBlock.CurrColor;
                }
            }
        }

        private void currBlockErase()
        {
            Point Position = currBlock.CurrPosition;
            Point[] Shape = currBlock.CurrShape;
            foreach (Point S in Shape)
            {
                BlockControls[(int)(S.X + Position.X) + ((Cols / 2) - 1), (int)(S.Y + Position.Y) + 2].Background = NoBrush;
            }
        }

        private void CheckRows()
        {
            bool full;
            int rows = 0;

            //van boven naar beneden controleren zodat bij meerder rijen de rijen die reeds naar onderen geplaats zijn ook worden meegenomen!
            for (int i = 1; i < Rows; i++)
            {
                full = true;
                for (int j = 0; j < Cols; j++)
                {
                    if (BlockControls[j, i].Background == NoBrush)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {

                    RemoveRow(i);
                    rows += 1;
                }
            }

            CalculateScore(rows);

        }

        private void CalculateScore(int rows)
        {
            LinesFilled += rows;

           

            if (rows != 0){
                Controller.RumbleLEDS(rows);
                
            }

            switch (rows)
            {
                case 1:
                    Score += 40;

                    Clear.Stop();
                    Clear.Play();

                    break;
                case 2:
                    Score += 100;

                    DoubleClear.Stop();
                    DoubleClear.Play();
                    break;
                case 3:
                    Score += 300;
                    TripleClear.Stop();
                    TripleClear.Play();
                    break;
                case 4:
                    Score += 1200;
                    Tetris.Stop();
                    Tetris.Play();
                    break;
                default:
                    break;
            }

        }

        private void RemoveRow(int row)
        {
            //van beneden naar boven alle rijen naar onderen plaatsen
            for (int i = row; i > 2; i--)
            {
                for (int j = 0; j < Cols; j++)
                {
                    BlockControls[j, i].Background = BlockControls[j, i - 1].Background;
                }
            }

        }

        public void CurrBlockMovLeft()
        {
            Point Position = currBlock.CurrPosition;
            Point[] Shape = currBlock.CurrShape;
            bool move = true;

            currBlockErase();

            foreach (Point S in Shape)
            {
                if (((int)(S.X + Position.X) + ((Cols / 2) - 1) - 1) < 0)
                {
                    move = false;
                    break;
                }
                else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1) - 1), (int)(S.Y + Position.Y + 2)].Background != NoBrush)
                {
                    move = false;
                    break;
                }
            }

            if (move)
            {
                currBlock.movLeft();
                currBlockDraw();

                Rotate.Stop();
                Rotate.Play();

            }
            else
            {
                currBlockDraw();
            }
        }
        public void CurrBlockMovRight()
        {
            Point Position = currBlock.CurrPosition;
            Point[] Shape = currBlock.CurrShape;
            bool move = true;

            currBlockErase();

            foreach (Point S in Shape)
            {
                if (((int)(S.X + Position.X) + ((Cols / 2) - 1) + 1) >= Cols)
                {
                    move = false;
                    break;
                }
                else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1) + 1), (int)(S.Y + Position.Y + 2)].Background != NoBrush)
                {
                    move = false;
                    break;
                }
            }

            if (move)
            {
                currBlock.movRight();
                currBlockDraw();

                Rotate.Stop();
                Rotate.Play();

            }
            else
            {
                currBlockDraw();
            }


        }
        public void CurrBlockMovDown()
        {
            Point Position = currBlock.CurrPosition;
            Point[] Shape = currBlock.CurrShape;
            bool move = true;

            currBlockErase();

            foreach (Point S in Shape)
            {
                if (((int)(S.Y + Position.Y) + 2 + 1) >= Rows)
                {
                    move = false;
                    break;
                }
                else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1)), (int)(S.Y + Position.Y) + 2 + 1].Background != NoBrush)
                {
                    move = false;
                    break;
                }
            }

            if (move)
            {
                currBlock.movDown();
                currBlockDraw();


            }
            else
            {

                currBlockDraw();

                currBlock = nextBlock;
                nextBlock = nextNextBlock;
                nextNextBlock = new Block();

                Fix.Stop();
                Fix.Play();

                CheckRows();

                currBlockDraw();
                if (!GameOver) nextBlocksDraw();

            }


        }

        private bool CanRotate(Point[] S, Point Position, bool move, int i)
        {

            if (((int)((S[i].Y + Position.Y) + 2)) >= Rows)
            {
                move = false;
            }
            else if (((int)(S[i].X + Position.X) + ((Cols / 2) - 1)) < 0)
            {
                move = false;
            }
            else if (((int)(S[i].X + Position.X) + ((Cols / 2) - 1)) >= Rows)
            {
                move = false;
            }
            else if (BlockControls[((int)(S[i].X + Position.X) + ((Cols / 2) - 1)), (int)(S[i].Y + Position.Y) + 2].Background != NoBrush)
            {
                move = false;
            }

            return move;
        }

        public void CurrBlockMovRotate(bool direction)
        {
            Point Position = currBlock.CurrPosition;
            Point[] Shape = currBlock.CurrShape;
            Point[] S = new Point[4];
            Shape.CopyTo(S, 0);

            currBlockErase();

            bool move = true;

            switch (direction)
            {
                //counterclock
                case true:
                    for (int i = 0; i < S.Length; i++)
                    {
                        double x = S[i].X;
                        S[i].X = S[i].Y * -1;
                        S[i].Y = x;

                        move = CanRotate(S, Position, move, i);

                        if (!move)
                        {
                            break;
                        }


                    }
                    break;

                //clock
                case false:
                    for (int i = 0; i < S.Length; i++)
                    {
                        double x = S[i].X;
                        S[i].X = S[i].Y;
                        S[i].Y = -x;

                        move = CanRotate(S, Position, move, i);

                        if (!move)
                        {
                            break;
                        }

                    }
                    break;
            }

            if (move)
            {
                currBlock.movRotate(direction);
                currBlockDraw();

                Rotate.Stop();
                Rotate.Play();


            }
            else
            {
                currBlockDraw();
            }

        }



        private void SelectMusic()
        {
            Random test = new Random();
            int rand = test.Next(1, 6);

            Start.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory +"/Resources/tetris-gameboy-0" + rand + ".wav"));
            Start.MediaEnded += MediaEnded_Loop;
            Start.Volume = 0.1;
            Start.Play();
        }

        private void MediaEnded_Loop(object sender, EventArgs e)
        {
            Start.Stop();
            Start = sender as MediaPlayer;
            if (Start == null)
                return;

            Start.Position = new TimeSpan(0);
            Start.Play();

        }
    }
}

