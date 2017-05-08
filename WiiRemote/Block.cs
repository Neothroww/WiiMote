using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WiiRemote
{
    public class Block
    {
        private bool rotate;
        private static Random rand = new Random();

        private Point currPosition = new Point(0, 0);
        public Point CurrPosition
        {
            get { return currPosition; }
            set { currPosition = value; }
        }

        private Point[] currShape;

        public Point[] CurrShape
        {
            get { return currShape; }
            set { currShape = value; }
        }

        public Brush CurrColor { get; set; } = Brushes.Transparent;

        public Block()
        {
            CurrShape = setRandomShape();
        }

        private Point[] setRandomShape()
        {
            switch (rand.Next(0, 7))
            {
                case 0:
                    rotate = true;
                    CurrColor = Brushes.Cyan;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(2,0)
                    };//x x x x
                case 1:
                    rotate = true;
                    CurrColor = Brushes.Blue;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(1,1),
                    };
                /* 
                    x x x
                        x
             */

                case 2:
                    rotate = true;
                    CurrColor = Brushes.Orange;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(1,-1)
                    };

                /* 
                        x
                    x x x
             */

                case 3:
                    rotate = false;
                    CurrColor = Brushes.Yellow;
                    return new Point[] {
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0),
                        new Point(1,1)
                    };

                /* 
                      x x
                      x x
                */

                case 4:
                    rotate = true;
                    CurrColor = Brushes.Green;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,-1),
                        new Point(1,0)
                    };
                /* 
                         x
                       x x x
                */
                case 5:
                    rotate = true;
                    CurrColor = Brushes.Purple;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,-1),
                        new Point(1,-1)
                    };

                /* 
                       x x
                         x x
                */
                case 6:
                    rotate = true;
                    CurrColor = Brushes.Red;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,1),
                        new Point(1,1)
                    };

                /*      x x
                      x x
               */

                default:
                    return null;
            }
        }

        public void movLeft()
        {
            currPosition.X -= 1;
        }

        public void movRight()
        {
            currPosition.X += 1;
        }

        public void movDown()
        {
            currPosition.Y += 1;
        }

        public void movRotate(bool direction)
        {
            if (rotate)
            {
                switch (direction)
                {
                    //counterclock
                    case true:
                        for (int i = 0; i < CurrShape.Length; i++)
                        {
                            double x = currShape[i].X;
                            currShape[i].X = currShape[i].Y * -1;
                            currShape[i].Y = x;
                        }
                        break;

                    //clock
                    case false:
                        for (int i = 0; i < currShape.Length; i++)
                        {
                            double x = currShape[i].X;
                            currShape[i].X = currShape[i].Y;
                            currShape[i].Y = -x;
                        }
                        break;
                }
            }
        }

    }
}
