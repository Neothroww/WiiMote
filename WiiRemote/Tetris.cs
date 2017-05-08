using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WiiRemote
{
    public class Tetris
    {
        public DispatcherTimer Timer;
        public Board myBoard;

        public Grid myGridMain;
        public Grid myGridNext;
        public Grid myGridNextNext;

        public Label Lines;
        public Label Score;

        public Controller Controller;


        public Tetris(Grid grid, Grid next, Grid nextnext, Label lines, Label score)
        {
            Controller = new Controller(this);

            Timer = new DispatcherTimer();
            Timer.Tick += GameTick;
            Timer.Interval = TimeSpan.FromMilliseconds(400);

            myGridMain = grid;
            myGridNext = next;
            myGridNextNext = nextnext;

            Lines = lines;
            Score = score;
           
        }

        public void GameStart()
        {
            if(myBoard != null) myBoard.Start.Stop();
            myBoard = new Board(myGridMain, myGridNext, myGridNextNext, Controller);
            Timer.Start();
        }

        

        public void GameTick(object sender, EventArgs e)
        {
            GameProgress();
        }

        public void GameProgress()
        {
            if (!myBoard.GameOver)
            {
                Score.Content = myBoard.Score.ToString("0000000000000");
                Lines.Content = myBoard.LinesFilled.ToString("0000000000000");
                myBoard.CurrBlockMovDown();
            }
            else
            {
                GameOver();
            }

        }

        public void GamePause()
        {
            if (!myBoard.GameOver)
            {
                if (Timer.IsEnabled) Timer.Stop();
                else Timer.Start();
            }
        }

        public void GameOver()
        {
            Timer.Stop();
            myBoard.Start.Stop();
            myBoard.Over.Play();
        }
    }
}
