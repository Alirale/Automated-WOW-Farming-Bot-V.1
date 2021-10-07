using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;

namespace HolydayRun
{
    public class Engine : Eye
    {
        private static List<KeyValuePair<List<double>, Boolean>> Destinations = new List<KeyValuePair<List<double>, bool>>();
        private Task EngineTask;
        private List<string> Datas = new List<string>();
        Cordination CurrentCord = new Cordination();
        Cordination CurrentDestination = new Cordination();
        int CurrentDestinationRow = 0;
        int DelayTime = 50;
        Eye eye = new Eye();
        MouseClickStuffs mvsc = new MouseClickStuffs();
        Keys keys = new Keys();
        InputSimulator sim = new InputSimulator();
        CancellationTokenSource TokenSurce = null;
        Boolean CurrentLootcondition = false;
        private Task KillTask;
        private Task Vendor;


        public static Stopwatch RunTaskStopWatch = new Stopwatch();
        public static Stopwatch StuckStopWatch = new Stopwatch();
        Mutex TaskLock = new Mutex();
        bool KillConfirmed = false;
        Random VendorMinuteDelay = new Random();
        int RandomVendorMinuteDelay = 0;

        Calcs Calculator = new Calcs();


        private Double MouseAngelFixer(int X1, int Y1, double Facing, int X2, int Y2)
        {

            List<string> Outputs = new List<string>(2);
            double Degree = 0;

            Calcs Calculator = new Calcs();

            Outputs = Calculator.RadDegreeTurn(X1, Y1, Facing, X2, Y2);

            Degree = Calculator.MouseTurn(Convert.ToDouble(Outputs[0]));

            if (Convert.ToDouble(Outputs[0]) > 2)
            {

                if (Outputs[1] == "cw")
                {
                    Form1.ReseCWtmousePosition();

                    mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                    SlowCWTurn(Convert.ToInt32(Degree));

                    Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();

                }
                else if (Outputs[1] == "cc")
                {
                    Form1.ReseCCtmousePosition();

                    mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                    SlowCCTurn(Convert.ToInt32(Degree));

                    Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();

                }
            }
            return Degree;
        }


        private Double KeyAngleFixer(int X1, int Y1, double Facing, int X2, int Y2)
        {
            try
            {
                List<string> Outputs = new List<string>(2);
                double WaitDegree = 0;

                Calcs Calculator = new Calcs();

                Outputs = Calculator.RadDegreeTurn(X1, Y1, Facing, X2, Y2);

                WaitDegree = Calculator.StandingKeyTurnTime(Convert.ToDouble(Outputs[0]));


                if (Outputs[1] == "cw")
                {
                    int Sahih = Convert.ToInt32(WaitDegree / 100);
                    int Ashari = Convert.ToInt32(WaitDegree % 100);

                    sim.Keyboard.KeyDown(VirtualKeyCode.VK_D);

                    for (int i = 0; i < Sahih; i++)
                    {
                        if (Form1.IsRunning)
                        {
                            Task.Delay(Convert.ToInt32(100)).Wait();
                        }
                    }
                    Task.Delay(Convert.ToInt32(Ashari)).Wait();

                    sim.Keyboard.KeyUp(VirtualKeyCode.VK_D);
                }
                else if (Outputs[1] == "cc")
                {
                    int Sahih = Convert.ToInt32(WaitDegree / 100);
                    int Ashari = Convert.ToInt32(WaitDegree % 100);

                    sim.Keyboard.KeyDown(VirtualKeyCode.VK_A);

                    for (int i = 0; i < Sahih; i++)
                    {
                        if (Form1.IsRunning)
                        {
                            Task.Delay(Convert.ToInt32(100)).Wait();
                        }
                    }
                    Task.Delay(Convert.ToInt32(Ashari)).Wait();

                    sim.Keyboard.KeyUp(VirtualKeyCode.VK_A);
                }


                return Convert.ToDouble(Outputs[0]);
            }
            catch 
            {
                Form1.hotKeyManager.Unregister(Key.F1, System.Windows.Input.ModifierKeys.None);
                Form1.hotKeyManager.Unregister(Key.F2, System.Windows.Input.ModifierKeys.None);
                Form1.hotKeyManager.Unregister(Key.F3, System.Windows.Input.ModifierKeys.None);
                Application.Restart();
                Thread.Sleep(1000);
                Environment.Exit(0);
            }
            return 0;
        }


        private void LootAngelFixer(double DestinationFacing)
        {

            List<string> Datas2_ = new List<string>();
            Calcs Calculator2 = new Calcs();
            Datas2_ = FetchData();

            Double CurrentFacing = Convert.ToDouble(Datas2_[2]);
            DestinationFacing = Destinations[CurrentDestinationRow].Key[3];

            List<string> Outputs = new List<string>(2);
            double Degree = 0;


            if ((CurrentFacing - DestinationFacing) >= 0 && (CurrentFacing - DestinationFacing) <= 180)//cw
            {
                Degree = CurrentFacing - DestinationFacing;
                Form1.ReseCWtmousePosition();

                mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                SlowCWTurn(Convert.ToInt32(Degree));

                Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();
            }

            else if ((CurrentFacing - DestinationFacing) > 180)//cc
            {
                Degree = 360 - (CurrentFacing - DestinationFacing);
                Form1.ReseCCtmousePosition();

                mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                SlowCCTurn(Convert.ToInt32(Degree));

                Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();
            }
            else if ((CurrentFacing - DestinationFacing) < 0 && (CurrentFacing - DestinationFacing) >= -180) //cc
            {
                Degree = (CurrentFacing - DestinationFacing);

                Form1.ReseCWtmousePosition();

                mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                SlowCCTurn(Convert.ToInt32(Degree));

                Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();
            }
            else if ((CurrentFacing - DestinationFacing) < -180) //cw
            {
                Degree = 360 + (CurrentFacing - DestinationFacing);

                Form1.ReseCWtmousePosition();

                mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                SlowCWTurn(Convert.ToInt32(Degree));

                Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();
            }
        }




        private void MouseLootAngelFixer(double DestinationFacing, double CurrentFacing)
        {
            double Degree = 0;

            if ((CurrentFacing - DestinationFacing) >= 0 && (CurrentFacing - DestinationFacing) <= 180)//cw
            {
                Degree = CurrentFacing - DestinationFacing;
                Form1.ReseCWtmousePosition();

                mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                SlowCWTurn(Convert.ToInt32(Degree));

                Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();
            }

            else if ((CurrentFacing - DestinationFacing) > 180)//cc
            {
                Degree = 360 - (CurrentFacing - DestinationFacing);
                Form1.ReseCCtmousePosition();

                mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                SlowCCTurn(Convert.ToInt32(Degree));

                Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();
            }
            else if ((CurrentFacing - DestinationFacing) < 0 && (CurrentFacing - DestinationFacing) >= -180) //cc
            {
                Degree = (CurrentFacing - DestinationFacing);

                Form1.ReseCWtmousePosition();

                mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                SlowCCTurn(Convert.ToInt32(Degree));

                Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();
            }
            else if ((CurrentFacing - DestinationFacing) < -180) //cw
            {
                Degree = 360 + (CurrentFacing - DestinationFacing);

                Form1.ReseCWtmousePosition();

                mvsc.DoRightMouseClickdown(); Task.Delay(DelayTime).Wait();

                SlowCWTurn(Convert.ToInt32(Degree));

                Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickup();
            }

        }


        private void KeyLootAngelFixer(double DestinationFacing, double CurrentFacing)
        {
            double Degree = 0;
            double WaitDegree = 0;
            Calcs Calculator = new Calcs();

            if ((CurrentFacing - DestinationFacing) >= 0 && (CurrentFacing - DestinationFacing) <= 180)//cw
            {
                Degree = CurrentFacing - DestinationFacing;
                WaitDegree = Calculator.StandingKeyTurnTime(Degree);
                sim.Keyboard.KeyDown(VirtualKeyCode.VK_D);
                Task.Delay(Convert.ToInt32(WaitDegree)).Wait();
                sim.Keyboard.KeyUp(VirtualKeyCode.VK_D);
            }

            else if ((CurrentFacing - DestinationFacing) > 180)//cc
            {
                Degree = 360 - (CurrentFacing - DestinationFacing);
                WaitDegree = Calculator.StandingKeyTurnTime(Degree);
                sim.Keyboard.KeyDown(VirtualKeyCode.VK_A);
                Task.Delay(Convert.ToInt32(WaitDegree)).Wait();
                sim.Keyboard.KeyUp(VirtualKeyCode.VK_A);
            }
            else if ((CurrentFacing - DestinationFacing) < 0 && (CurrentFacing - DestinationFacing) >= -180) //cc
            {
                Degree = (CurrentFacing - DestinationFacing);
                WaitDegree = Math.Abs(Calculator.StandingKeyTurnTime(Degree));
                sim.Keyboard.KeyDown(VirtualKeyCode.VK_A);
                Task.Delay(Convert.ToInt32(WaitDegree)).Wait();
                sim.Keyboard.KeyUp(VirtualKeyCode.VK_A);
            }
            else if ((CurrentFacing - DestinationFacing) < -180) //cw
            {
                Degree = 360 + (CurrentFacing - DestinationFacing);
                WaitDegree = Math.Abs(Calculator.StandingKeyTurnTime(Degree));
                sim.Keyboard.KeyDown(VirtualKeyCode.VK_D);
                Task.Delay(Convert.ToInt32(WaitDegree)).Wait();
                sim.Keyboard.KeyUp(VirtualKeyCode.VK_D);
            }

        }


        public void SlowCCTurn(int ToTurn)
        {
            MouseClickStuffs mvsc = new MouseClickStuffs();
            int DelayTime = 10;

            Task.Delay(DelayTime).Wait();

            Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickdown();
            int UnitTurn = 200;

            if (ToTurn >= UnitTurn)
            {
                int Sahih = ToTurn / UnitTurn;
                int Ashari = ToTurn % UnitTurn;

                for (int Counter = 0; Counter < (Sahih); Counter++)
                {
                    Cursor.Position = new Point(Cursor.Position.X - (Convert.ToInt32(UnitTurn)), Cursor.Position.Y);
                    Task.Delay(DelayTime).Wait();
                }
                Cursor.Position = new Point(Cursor.Position.X - (Convert.ToInt32(Ashari)), Cursor.Position.Y);
                Task.Delay(DelayTime).Wait();

                mvsc.DoRightMouseClickup();
            }
            else
            {
                Cursor.Position = new Point(Cursor.Position.X - (Convert.ToInt32(ToTurn)), Cursor.Position.Y);
                Task.Delay(DelayTime).Wait();
            }

        }


        public void SlowCWTurn(int ToTurn)
        {
            MouseClickStuffs mvsc = new MouseClickStuffs();
            int DelayTime = 30;

            Task.Delay(DelayTime).Wait();

            Task.Delay(DelayTime).Wait(); mvsc.DoRightMouseClickdown();
            int UnitTurn = 100;

            if (ToTurn >= UnitTurn)
            {
                int Sahih = ToTurn / UnitTurn;
                int Ashari = ToTurn % UnitTurn;

                for (int Counter = 0; Counter < (Sahih); Counter++)
                {
                    Cursor.Position = new Point(Cursor.Position.X + (Convert.ToInt32(UnitTurn)), Cursor.Position.Y);
                    Task.Delay(DelayTime).Wait();
                }

                Cursor.Position = new Point(Cursor.Position.X + (Convert.ToInt32(Ashari)), Cursor.Position.Y);
                Task.Delay(DelayTime).Wait();

                mvsc.DoRightMouseClickup();
            }
            else
            {
                Cursor.Position = new Point(Cursor.Position.X + (Convert.ToInt32(ToTurn)), Cursor.Position.Y);
                Task.Delay(DelayTime).Wait();
            }

        }

        public void Test()
        {
            Task.Delay(1000).Wait();
            SendKeys.SendWait(keys.OpenVendorKey);
            Task.Delay(3000).Wait();

            SendKeys.SendWait(keys.TargetVendorKey);
            Task.Delay(1000).Wait();
            SendKeys.SendWait(keys.IntracVendorKey);
            Task.Delay(2000).Wait();

            SendKeys.SendWait(keys.OpenVendorKey);
            Task.Delay(1000).Wait();

            SendKeys.SendWait(keys.BagKey);
            Task.Delay(900).Wait();

            Form1.ResetmousePosition();
            Task.Delay(500).Wait();
            MessageBox.Show("Vendor Done");
        }


        public void RunFirst()
        {
            CurrentDestinationRow = Convert.ToInt32(Calculator.Reader("CurrentDestination.txt"));
            CurrentDestination.CordX = Convert.ToInt32(Destinations[CurrentDestinationRow].Key[1]);
            CurrentDestination.CordY = Convert.ToInt32(Destinations[CurrentDestinationRow].Key[2]);
            CurrentDestination.Facing = Calculator.To_180(Destinations[CurrentDestinationRow].Key[3]);
        }

        public void RunEngine()
        {
            try
            {
                KillTask = new Task(() =>
                {
                    while (Form1.Continue)
                    {
                        if (Form1.IsRunning)
                        {
                            if (KillConfirmed)
                            {
                                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                                Task.Delay(200).Wait();
                                sim.Keyboard.KeyPress(VirtualKeyCode.VK_1);
                                Task.Delay(200).Wait();
                                sim.Keyboard.KeyPress(VirtualKeyCode.VK_3);
                                Task.Delay(200).Wait();
                            }
                            else Task.Delay(DelayTime).Wait();
                        }
                    }
                });

                if (Form1.IsRunning)
                {
                    GO();
                }

                Task.Delay(DelayTime).Wait();

                EngineTask = new Task(() =>
                {
                    RandomVendorMinuteDelay = 900000000;
                    while (Form1.Continue)
                    {
                        if (Form1.IsRunning)
                        {
                            if (RunTaskStopWatch.Elapsed.TotalMinutes > RandomVendorMinuteDelay)
                            {
                                KillConfirmed = false;
                                STOP();
                                TaskLock.WaitOne();
                                Vendor = VendorLuncher();
                                Vendor.Start();
                                Vendor.Wait();
                                RunTaskStopWatch.Restart();
                                TaskLock.ReleaseMutex();
                            }
                            else
                            {
                                Again:;
                                List<string> Datas_ = new List<string>();
                                int x1, x2, y1, y2;
                                Double facing;
                                Datas_ = FetchData();
                                Task.Delay(DelayTime).Wait();

                                if (Datas_ == null)
                                {
                                    Form1.hotKeyManager.Unregister(Key.F1, System.Windows.Input.ModifierKeys.None);
                                    Form1.hotKeyManager.Unregister(Key.F2, System.Windows.Input.ModifierKeys.None);
                                    Form1.hotKeyManager.Unregister(Key.F3, System.Windows.Input.ModifierKeys.None);
                                    Application.Restart();
                                    Thread.Sleep(1000);
                                    Environment.Exit(0);
                                }

                                x1 = Convert.ToInt32(Datas_[0]); y1 = Convert.ToInt32(Datas_[1]); facing = Calculator.To_180(Convert.ToDouble(Datas_[2]));
                                x2 = CurrentDestination.CordX; y2 = CurrentDestination.CordY;

                                StuckStopWatch.Start();

                                KeyAngleFixer(x1, y1, facing, x2, y2);

                                DoRandomFunction();

                                for (int i = 0; i < 2; i++)
                                {
                                    sim.Keyboard.KeyPress(VirtualKeyCode.F4);
                                    Task.Delay(DelayTime).Wait();
                                }
                                if (Form1.IsRunning)
                                {
                                    GO();
                                }
                                Task.Delay(DelayTime).Wait();

                                KillConfirmed = true;


                                while (CheckIfArrive(200) == false && StuckStopWatch.Elapsed.TotalSeconds < 10 && Form1.IsRunning == true)
                                {
                                    Task.Delay(DelayTime).Wait();
                                }

                                StuckStopWatch.Stop();

                                if (StuckStopWatch.Elapsed.TotalSeconds >= 10)
                                {
                                    IfStucked();
                                    StuckStopWatch.Reset();
                                    goto Again;
                                }

                                StuckStopWatch.Reset();

                                Task.Delay(DelayTime).Wait();
                                DestinationChanger();
                            }
                        }
                    }
                });

                EngineTask.Start();
                RunTaskStopWatch.Start();
                KillTask.Start();
            }
            catch
            {
                Application.Restart();
                Thread.Sleep(1000);
                Environment.Exit(0);
            }
            
        }



        public void IntrackMouseover()
        {
            SendKeys.SendWait("{F6}");
        }

        private Task VendorLuncher()
        {
            return new Task(() =>
            {
                Task.Delay(500).Wait();
                SendKeys.SendWait(keys.BRBKey);
                Task.Delay(2000).Wait();

                for (int j = 0; j < 5; j++)
                {
                    SendKeys.SendWait(keys.TabKey);
                    Task.Delay(600).Wait();
                    SendKeys.SendWait(keys.SunFireKey);
                    Task.Delay(600).Wait();
                }

                Task.Delay(1000).Wait();
                SendKeys.SendWait(keys.OpenVendorKey);
                Task.Delay(3000).Wait();

                SendKeys.SendWait(keys.TargetVendorKey);
                Task.Delay(1000).Wait();
                SendKeys.SendWait(keys.IntracVendorKey);
                Task.Delay(2000).Wait();

                Form1.VendorMousePosition();
                for (int i = 0; i < 3; i++)
                {
                    Task.Delay(DelayTime).Wait();
                    mvsc.DoLeftMouseClickdown();
                    Task.Delay(DelayTime).Wait();
                    mvsc.DoLeftMouseClickup();
                    Task.Delay(DelayTime).Wait();
                }

                SendKeys.SendWait(keys.BagKey);
                Task.Delay(900).Wait();

                SendKeys.SendWait(keys.OpenVendorKey);
                Task.Delay(1000).Wait();

                Form1.ResetmousePosition();
                Task.Delay(500).Wait();
            });
        }


        private void IfStucked()
        {
            for (int i = 0; i < 2; i++)
            {
                sim.Keyboard.KeyPress(VirtualKeyCode.F4);

                Task.Delay(DelayTime).Wait();
            }
            GO();
            for (int i = 0; i < 3; i++)
            {
                Task.Delay(DelayTime).Wait();
                sim.Keyboard.KeyDown(VirtualKeyCode.SPACE);
                Task.Delay(DelayTime * 3).Wait();
                sim.Keyboard.KeyUp(VirtualKeyCode.SPACE);
                Task.Delay(DelayTime).Wait();
            }
            Random R = new Random();
            if (R.Next(0, 2) == 0)
            {
                Form1.ReseCCtmousePosition();
                mvsc.DoRightMouseClickdown();
                Task.Delay(DelayTime).Wait();
                Cursor.Position = new Point(Cursor.Position.X - (600), Cursor.Position.Y);
                mvsc.DoRightMouseClickup();
                Task.Delay(DelayTime).Wait();
            }

            else
            {
                Form1.ReseCWtmousePosition();
                mvsc.DoRightMouseClickdown();
                Task.Delay(DelayTime).Wait();
                Cursor.Position = new Point(Cursor.Position.X + (600), Cursor.Position.Y);
                mvsc.DoRightMouseClickup();
                Task.Delay(DelayTime).Wait();
            }
            for (int i = 0; i < 3; i++)
            {
                Task.Delay(DelayTime).Wait();
                sim.Keyboard.KeyDown(VirtualKeyCode.SPACE);
                Task.Delay(DelayTime * 3).Wait();
                sim.Keyboard.KeyUp(VirtualKeyCode.SPACE);
                Task.Delay(DelayTime).Wait();
            }
        }


        private void DoRandomFunction()
        {
            Random r = new Random();
            int choice = r.Next(0, 5);
            switch (choice)
            {
                case 0:
                    sim.Keyboard.KeyPress(VirtualKeyCode.VK_4);
                    break;
                case 1:
                    sim.Keyboard.KeyPress(VirtualKeyCode.VK_5);
                    break;

                case 2:
                    sim.Keyboard.KeyPress(VirtualKeyCode.VK_6);
                    break;

                case 3:
                    sim.Keyboard.KeyPress(VirtualKeyCode.VK_7);
                    break;

                default:
                    break;
            }
        }

        public void StopEngine()
        {
            STOP();
            Task.Delay(DelayTime).Wait();
            TokenSurce.Cancel();
        }

        private void DestinationChanger()
        {
            Calcs calculator = new Calcs();

            CurrentDestinationRow++;
            if ((Destinations[CurrentDestinationRow].Key[0]) == 999)
            {
                CurrentDestinationRow = 0;
            }
            CurrentLootcondition = Destinations[CurrentDestinationRow].Value;
            calculator.Writer(CurrentDestinationRow.ToString(), "CurrentDestination.txt");
            CurrentDestination.CordX = Convert.ToInt32(Destinations[CurrentDestinationRow].Key[1]);
            CurrentDestination.CordY = Convert.ToInt32(Destinations[CurrentDestinationRow].Key[2]);
            CurrentDestination.Facing = calculator.To_180(Destinations[CurrentDestinationRow].Key[3]);
        }

        private void ToyLoot()
        {
            Task.Delay(DelayTime).Wait();
            for (int i = 0; i < 11; i++)
            {
                SendKeys.SendWait(keys.LootKey);
                Task.Delay(900).Wait();
            }
        }


        public void LoadDatas()
        {
            String[] DatasOfEachLine = new string[4];

            List<string> RawLines = new List<string>();
            Calcs calculator = new Calcs();
            RawLines = calculator.DataExtractor("Paths.txt");
            foreach (var item in RawLines)
            {
                List<double> Datas = new List<double>();
                string temp = item;
                DatasOfEachLine = temp.Split(',');
                Boolean IsLootable = false;
                Datas.Add(Convert.ToDouble(DatasOfEachLine[0]));
                Datas.Add(Convert.ToDouble(DatasOfEachLine[1]));
                Datas.Add(Convert.ToDouble(DatasOfEachLine[2]));
                Datas.Add(calculator.To_180(Convert.ToDouble(DatasOfEachLine[3])));
                if (DatasOfEachLine[4] == "True")
                {
                    IsLootable = true;
                }
                else if (DatasOfEachLine[4] == "False")
                {
                    IsLootable = false;
                }
                KeyValuePair<List<double>, Boolean> FinalDestinationOfThisLevel = new KeyValuePair<List<double>, bool>(Datas, IsLootable);
                Destinations.Add(FinalDestinationOfThisLevel);
            }

        }

        private bool CheckIfArrive(Double Radius)
        {
            int x1, x2, y1, y2;
            Double facing;
            Calcs Calculator = new Calcs();

            List<string> Datas_ = new List<string>();
            Datas_ = FetchData();
            Task.Delay(DelayTime).Wait();

            if (Datas_ == null)
            {
                Application.Restart();
                Thread.Sleep(1000);
                Environment.Exit(0);
            }

            x1 = Convert.ToInt32(Datas_[0]); y1 = Convert.ToInt32(Datas_[1]); facing = Calculator.To_180(Convert.ToDouble(Datas_[2]));
            x2 = CurrentDestination.CordX; y2 = CurrentDestination.CordY;
            if ((Calculator.CalcDisstance(x1, y1, x2, y2) <= Radius))
            {
                return true;
            }
            KeyAngleFixer(x1, y1, facing, x2, y2);

            return false;
        }


        public void GO()
        {
            SendKeys.SendWait("{F7}");
        }

        public void STOP()
        {
            SendKeys.SendWait("{F8}");
        }
    }
    
}
