using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using WindowsInput;
using GlobalHotKey;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace HolydayRun
{
    public partial class Form1 : Form
    {
        public static HotKeyManager hotKeyManager = new HotKeyManager();
        public static Point VendorPosition;
        public static Point LootRadious;
        public static Boolean IsRunning = true;
        public static Boolean Continue = true;
        public static Bitmap Looticon;
        InputSimulator sim = new InputSimulator();
        Stopwatch TimeOut = new Stopwatch();

        HotKey StartEngine;
        HotKey PauseEngine;
        HotKey ResumeEngine;

        Engine Eng = new Engine();

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            StartEngine = hotKeyManager.Register(Key.F1, System.Windows.Input.ModifierKeys.None);
            PauseEngine = hotKeyManager.Register(Key.F2, System.Windows.Input.ModifierKeys.None);
            ResumeEngine = hotKeyManager.Register(Key.F3, System.Windows.Input.ModifierKeys.None);
            hotKeyManager.KeyPressed += HotKeyManager_KeyPressed;
            Thread.Sleep(1500);

            Thread T = new Thread(() =>
            {
                sim.Mouse.MoveMouseTo(10, 10);
                for (int i = 0; i < 2; i++)
                {
                    sim.Mouse.LeftButtonDoubleClick();
                    Thread.Sleep(500);
                    sim.Mouse.LeftButtonDoubleClick();
                    Thread.Sleep(500);
                }
                Eye eye = new Eye();
                eye.Calibrate();
                Eng.LoadDatas();
                Eng.RunFirst();
                Eng.RunEngine();
                TimeOutCanceller();
            });
            T.Start();
            TimeOut.Start();
        }


        private void HotKeyManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == Key.F1 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                Eye eye = new Eye();
                eye.Calibrate();
                Eng.LoadDatas();
                Eng.RunFirst();
                Eng.RunEngine();
            }

            else if (e.HotKey.Key == Key.F2 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                Engine Eng = new Engine();
                Eng.STOP();
                Pause();
            }
            else if (e.HotKey.Key == Key.F2 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                Engine Eng = new Engine();
                Eng.STOP();
                Pause();
            }
            else if (e.HotKey.Key == Key.F3 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                Resume();
            }

        }

        private void btnFixEye_Click(object sender, EventArgs e)
        {
            Eye eye = new Eye();
            eye.Calibrate();
        }

        private void btnPathRecorder_Click(object sender, EventArgs e)
        {
            RecordForm RecForm = new RecordForm();
            this.Hide();
            RecForm.ShowDialog();
        }

        private void btnLoadDatas_Click(object sender, EventArgs e)
        {
            Engine eng = new Engine();
            eng.LoadDatas();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Calcs calculator = new Calcs();
            double x1 = 2;
            double y1 = 2;
            double facing = 45;
            double x2 = 3.9;
            double y2 = 4;
            MessageBox.Show(calculator.RadDegreeTurn(x1, y1, facing, x2, y2)[0].ToString() + "\n" + (calculator.RadDegreeTurn(x1, y1, facing, x2, y2)[1].ToString()));
        }

        public static void ReseCWtmousePosition()
        {

            Cursor.Position = new Point(300, Screen.PrimaryScreen.Bounds.Height / 8);

        }

        public static void ReseCCtmousePosition()
        {

            Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width - 300, Screen.PrimaryScreen.Bounds.Height / 8);

        }

        public static void VendorMousePosition()
        {
            Cursor.Position = new Point(VendorPosition.X, VendorPosition.Y);
        }

        public static void ResetmousePosition()
        {

            Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);

        }

        private void Resume()
        {
            Continue = true;
            IsRunning = true;
            Eng.RunEngine();
            Engine.StuckStopWatch.Stop();
            Engine.RunTaskStopWatch.Stop();
        }

        private void Pause()
        {
            Engine.StuckStopWatch.Stop();
            Engine.RunTaskStopWatch.Stop();
            IsRunning = false;
            Continue = false;
            SendKeys.SendWait("{F8}");
        }

        private void GetMouseColor()
        {
            MessageBox.Show(Cursor.GetHashCode().ToString());
        }

        private void TimeOutCanceller()
        {
            while (TimeOut.Elapsed.TotalMinutes < 5)
            {
                Thread.Sleep(10000);
            }

            SendKeys.SendWait("{F8}");
            SendKeys.SendWait("{A}");
            SendKeys.SendWait("{D}");

            TimeOut.Stop();
            TimeOut.Reset();
            Form1.hotKeyManager.Unregister(Key.F1, System.Windows.Input.ModifierKeys.None);
            Form1.hotKeyManager.Unregister(Key.F2, System.Windows.Input.ModifierKeys.None);
            Form1.hotKeyManager.Unregister(Key.F3, System.Windows.Input.ModifierKeys.None);

            this.Invoke(new Action(delegate
            {
                Form1.hotKeyManager.Dispose();
            }));

            Application.Restart();
            Thread.Sleep(1000);
            Environment.Exit(0);
        }


        private void GetVendorPos()
        {
            MouseClickStuffs msvc = new MouseClickStuffs();
            VendorPosition = msvc.GetCursorPosition();
            string ToSave;
            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "VendorPos.txt"))
            {
                ToSave = VendorPosition.X.ToString() + "," + VendorPosition.Y.ToString();
                sw.WriteLine(ToSave);
            }

            String ToReads = "TSM Saved :";
            ToReads += "\n";
            string line = "";
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "VendorPos.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    ToReads += line;
                    ToReads += "\n";
                }
            }
            MessageBox.Show(ToReads);
        }


        private void GetCircularLootR()
        {
            MouseClickStuffs msvc = new MouseClickStuffs();
            LootRadious = msvc.GetCursorPosition();
            string ToSave = "";

            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Circle.txt"))
            {
                ToSave = LootRadious.X.ToString() + "," + LootRadious.Y.ToString();
                sw.WriteLine(ToSave);
            }

            String ToReads = "R :";
            ToReads += "\n";
            string line = "";
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Circle.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    ToReads += line;
                    ToReads += "\n";
                }
            }
            MessageBox.Show(ToReads);
        }
    }
}
