using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using WII.HID.Lib;

namespace WiiRemote
{
    public class Controller
    {
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private HIDDevice _device;

        private bool DPadLeft;
        private bool DPadRight;
        private bool DPadUp;
        private bool DPadDown;

        private bool Left;
        private bool Right;
        private bool Rotate;
        private bool Progress;

        private bool Two;
        private bool One;

        private bool Minus;
        private bool Plus;


        private bool A;
        private bool B;

        private bool Home;

        private Tetris tetris;

        public Controller(Tetris tetris)
        {
            this.tetris = tetris;

            _device = HIDDevice.GetHIDDevice(0x57E, 0x306);

            if (_device != null)
            {
                // Lees request starten, OnReadReport aanroepen als het resultaat er is
                _device.ReadReport(OnReadReport);

                //mode buttons and accelerometer
                byte[] bytes = new byte[1] { 0x00 };
                this.SendReport(0x11, bytes);

                //mode buttons and accelerometer
                bytes = new byte[2] { 0x04, 0x31 };
                this.SendReport(0x12, bytes);
            }
        }

        private void OnReadReport(HIDReport report)
        {
            if (Thread.CurrentThread != _dispatcher.Thread)
            {
                this._dispatcher.Invoke(new ReadReportCallback(OnReadReport), report);
            }
            else
            {
                switch (report.ReportID)
                {
                    case 0x20:
                        // Status report
                        break;
                    case 0x21:
                        // Memory en register read report 
                        break;
                    case 0x22:
                        // Acknowledge report 
                        break;
                    case 0x30:
                        // Core buttons data report 
                        break;
                    case 0x31:
                        // Core buttons en accelerometer data report
                        ReadData(report.Data);
                        break;
                    case 0x37:
                        break;
                }
                _device.ReadReport(OnReadReport);
            }
        }

        private void ReadData(byte[] data)
        {

            AnalyzeCoreButtonsFirst(data[0]);
            AnalyzeCoreButtonsSecond(data[1]);
            AnalyzeAcceleration(data[2], data[3], data[4]);

        }

        private void AnalyzeAcceleration(byte x, byte y, byte z)
        {
            Console.WriteLine("x: " + (x - 0x79) + " y: " + (y - 0x79) + " z: " + (z - 0x79));

            int acx = (int)(x - 0x79);
            int acy = (int)(y - 0x79);
            int acz = (int)(z - 0x79);

            int thresholdMaxTilt = 15;
            int thresholdMaxSwipeX = 15;
            int thresholdMaxSwipeY = 15;
            int thresholdZero = 5;


            //tilt LR
            if (acx >= thresholdMaxTilt && acy < thresholdZero && acy > -thresholdZero && acz < thresholdZero && acz > -thresholdZero)
            {
                if (!Rotate)
                {
                    Rotate = true;
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovRotate(true);
                    Console.WriteLine("Swipe right");

                    return;
                }
                return;
            }
            if (acx <= -thresholdMaxTilt && acy < thresholdZero && acy > -thresholdZero && acz < thresholdZero && acz > -thresholdZero)
            {
                if (!Rotate)
                {
                    Rotate = true;
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovRotate(false);
                    Console.WriteLine("Swipe left");
                    return;
                }
                return;
            }
           


            //swipe LR
            if (acx >= thresholdMaxSwipeX && acy >= 10)
            {
                if (!Left)
                {
                    Left = true;
                    Right = true;
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovLeft();
                    Console.WriteLine("Swipe left");
                    return;
                }
                return;
            }
            if (acx <= -thresholdMaxSwipeX && acy >= thresholdMaxSwipeY)
            {
               
                if (!Right)
                {
                    Right = true;
                    Left = true;
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovRight();
                    Console.WriteLine("Swipe right");

                    return;
                }
                return;
            }
            if (acx >= -thresholdZero && acx <= thresholdZero && acy <= thresholdZero && acy >= -thresholdZero && acz >= thresholdMaxTilt)
            {
                Rotate = false;
                Right = false;
                Left = false;
            }

        }

        private void AnalyzeCoreButtonsSecond(byte buttonSecond)
        {
            if ((buttonSecond & (0x01)) != 0)
            {
                if (!Two)
                {
                    Two = true;
                    Console.WriteLine("Two");
                }
            }
            else Two = false;

            if ((buttonSecond & (0x02)) != 0)
            {
                if (!One)
                {
                    One = true;
                    Console.WriteLine("One");
                }
            }
            else One = false;

            if ((buttonSecond & (0x04)) != 0)
            {
                if (!B)
                {
                    B = true;
                    tetris.GamePause();
                    Console.WriteLine("B");
                }
            }
            else B = false;


            if ((buttonSecond & (0x08)) != 0)
            {
                if (!A)
                {
                    A = true;
                    tetris.GameStart();
                    Console.WriteLine("A");
                }
            }
            else A = false;


            if ((buttonSecond & (0x10)) != 0)
            {
                if (!Minus)
                {
                    Minus = true;
                    Console.WriteLine("Minus");
                }
            }
            else Minus = false;



            if ((buttonSecond & (0x80)) != 0)
            {
                if (!Home)
                {
                    Home = true;
                    Console.WriteLine("Home");
                }
            }
            else Home = false;
        }

        private void AnalyzeCoreButtonsFirst(byte buttonFirst)
        {
            if ((buttonFirst & (0x01)) != 0)
            {
                if (!DPadLeft)
                {
                    DPadLeft = true;
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovLeft();
                    Console.WriteLine("DPadLeft");
                }
            }
            else DPadLeft = false;

            if ((buttonFirst & (0x02)) != 0)
            {
                if (!DPadRight)
                {
                    DPadRight = true;
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovRight();
                    Console.WriteLine("DPadRight");
                }
            }
            else DPadRight = false;

            if ((buttonFirst & (0x04)) != 0)
            {
                if (!DPadDown)
                {
                    DPadDown = true;
                    if (tetris.Timer.IsEnabled) tetris.GameProgress();
                    Console.WriteLine("DPadDown");
                }
            }
            else DPadDown = false;

            if ((buttonFirst & (0x08)) != 0)
            {
                if (!DPadUp)
                {
                    DPadUp = true;
                    if (tetris.Timer.IsEnabled) tetris.myBoard.CurrBlockMovRotate(true);
                    Console.WriteLine("DPadUp");
                }
            }
            else DPadUp = false;


            if ((buttonFirst & (0x10)) != 0)
            {
                if (!Plus)
                {
                    Plus = true;
                    Console.WriteLine("Plus");
                }
            }
            else Plus = false;



        }

        public async Task RumbleLEDS(int rows)
        {
           

            SendReport(0x11, new byte[1] { 0x01 });
            await Task.Delay(200);
            SendReport(0x11, new byte[1] { 0x00 });

            LightLeds(rows);

        }

        public void LightLeds(int rows)
        {
           
            switch (rows)
            {
                case 1:
                    SendReport(0x11, new byte[1] { 0x10 });
                    break;
                case 2:
                    SendReport(0x11, new byte[1] { 0x30 });
                    break;
                case 3:
                    SendReport(0x11, new byte[1] { 0x70 });
                    break;
                case 4:
                    SendReport(0x11, new byte[1] { 0xF0 });
                    break;
                default:
                    break;
            }
        }

        public void SendReport(byte reportId, byte[] data)
        {
            //Report aanmaken HIDReport 
            HIDReport report = _device.CreateReport();

            report.ReportID = reportId;

            for (int i = 0; i < data.Length; i++)
            {
                report.Data[i] = data[i];
            }
            //Het report versturen 
            _device.WriteReport(report);
        }



    }
}
