using System;
using System.Windows;
using System.Windows.Input;

namespace WiiRemote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Tetris tetris;

        public MainWindow()
        {
            InitializeComponent();
            tetris = new Tetris(MainGrid, GridNext, GridNextNext, lblLinesValue, lblScoreValue);
        }

        private void Key_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Left:
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovLeft();
                    break;
                case System.Windows.Input.Key.Right:
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovRight();
                    break;
                case System.Windows.Input.Key.Down:
                    if (tetris.Timer.IsEnabled) tetris.GameProgress();
                    break;
                case System.Windows.Input.Key.Up:
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovRotate(true);
                    break;

                case System.Windows.Input.Key.F2:
                    tetris.GameStart();
                    break;

                case System.Windows.Input.Key.F3:
                    tetris.GamePause();
                    break;

                default:
                    break;
            }
        }
    }
}
