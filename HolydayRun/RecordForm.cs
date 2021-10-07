using GlobalHotKey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;

namespace HolydayRun
{
    public partial class RecordForm : Form
    {
        private HotKeyManager hotKeyManager = new HotKeyManager();
        Eye eye = new Eye();
        private Task FetchRecordData;
        private CancellationTokenSource TokenSurce;
        private Cordination CurrentPath = new Cordination();
        private Boolean IsLootable = false;
        private int TaskWaitTime = 30;
        int lastX = 0;int lastY = 0;


        public RecordForm()
        {
            InitializeComponent();
        }

        private void RecordForm_Load(object sender, EventArgs e)
        {
            var StartRecording = hotKeyManager.Register(Key.F9, System.Windows.Input.ModifierKeys.None);
            var SaveLootZone = hotKeyManager.Register(Key.F10, System.Windows.Input.ModifierKeys.None);
            var StopRecording = hotKeyManager.Register(Key.F11, System.Windows.Input.ModifierKeys.None);
            hotKeyManager.KeyPressed += HotKeyManager_KeyPressed;
        }

        private void HotKeyManager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == Key.F9 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                //MessageBox.Show("F9 pressed");
                SaveNonLootable();
            }
            if (e.HotKey.Key == Key.F10 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                //MessageBox.Show("F10 pressed");
                SaveLootable();
            }
            if (e.HotKey.Key == Key.F11 && e.HotKey.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                //MessageBox.Show("F11 pressed");
                SaveLast();
            }
        }

        private void btnBackForm_Click(object sender, EventArgs e)
        { 
            Form1 OrginForm = new Form1();
            this.Hide();
            OrginForm.ShowDialog();
        }


        void Record() 
        {

        }

        private void btnFixEye_Click(object sender, EventArgs e)
        {
            eye.Calibrate();
        }


        private void SaveNonLootable()
        {  
            Boolean Lootable = false;
            Calcs calculator = new Calcs();
            List<string> RecDatas = new List<string>();
            RecDatas = null;
            Again:;
            try { RecDatas = eye.FetchData(); } catch { goto Again; }
            CurrentPath.CordX = Convert.ToInt32(RecDatas[0]);
            CurrentPath.CordY = Convert.ToInt32(RecDatas[1]);
            CurrentPath.Facing = Convert.ToDouble(RecDatas[2]);

            string TempUnlootablePath = "111" + "," + CurrentPath.CordX + "," + CurrentPath.CordY + "," + CurrentPath.Facing + "," + Lootable;

            Writer(TempUnlootablePath, "Paths.txt");

            PlaySound sound = new PlaySound(AppDomain.CurrentDomain.BaseDirectory + @"\Ding\ding.mp3"); sound.playSound(); 
        }


        private void SaveLast()
        {
            Calcs calculator = new Calcs();
            List<string> RecDatas = new List<string>();
            RecDatas = null;
            Again:;
            try { RecDatas = eye.FetchData(); } catch { goto Again; }
            CurrentPath.CordX = Convert.ToInt32(RecDatas[0]);
            CurrentPath.CordY = Convert.ToInt32(RecDatas[1]);
            CurrentPath.Facing = Convert.ToDouble(RecDatas[2]);

            Writer("999" + ",88888,77777,111,False", "Paths.txt");

            PlaySound sound = new PlaySound(AppDomain.CurrentDomain.BaseDirectory + @"\Ding\ding3.mp3"); sound.playSound();
        }

        private void SaveLootable() 
        {
            Boolean Lootable = true;
            Calcs calculator = new Calcs();
            List<string> RecDatas = new List<string>();
            Again:;
            try { RecDatas = eye.FetchData(); } catch { goto Again; }
            CurrentPath.CordX = Convert.ToInt32(RecDatas[0]);
            CurrentPath.CordY = Convert.ToInt32(RecDatas[1]);
            CurrentPath.Facing = Convert.ToDouble(RecDatas[2]);

            string TempLootablePaths = "111" + "," + CurrentPath.CordX + "," + CurrentPath.CordY + "," + CurrentPath.Facing + "," + Lootable;

            Writer(TempLootablePaths, "Paths.txt");

            PlaySound sound = new PlaySound(AppDomain.CurrentDomain.BaseDirectory + @"\Ding\ding2.mp3"); sound.playSound();

            //MessageBox.Show("New Lootable Zone Saved : " + "\n" + CurrentPath.CordX + "\n" + CurrentPath.CordY + "\n" + CurrentPath.Facing);

        }

        private void RunRecordEngine()
        {

            TokenSurce = new CancellationTokenSource();
            CancellationToken token = TokenSurce.Token;
            token.ThrowIfCancellationRequested();
            Calcs calculator = new Calcs();
            int Row = 0;

            FetchRecordData = new Task(() =>
            {
                while (true)
                {
                    List<string> RecDatas = new List<string>();
                    RecDatas = null;
                    Row++;
                    if (token.IsCancellationRequested)
                    {
                        Task.Run(() => { MessageBox.Show("Record Ends");});

                        Writer("999"+ ",88888,77777,111,False", "Paths.txt"); 

                        token.ThrowIfCancellationRequested();
                    }
                    Again:;
                    try { RecDatas = eye.FetchData(); } catch { goto Again; }
                    CurrentPath.CordX = Convert.ToInt32(RecDatas[0]);
                    CurrentPath.CordY = Convert.ToInt32(RecDatas[1]);
                    CurrentPath.Facing = Convert.ToDouble( RecDatas[2]);

                    if (CurrentPath.CordX != lastX || CurrentPath.CordY != lastY)
                    {
                        if (IsLootable == false && calculator.CalcDisstance(CurrentPath.CordX, CurrentPath.CordY, lastX, lastY) > 500)
                        {
                            string TempUnlootablePath = "111" + "," + CurrentPath.CordX + "," + CurrentPath.CordY + "," + CurrentPath.Facing + "," + IsLootable;

                            Writer(TempUnlootablePath, "Paths.txt");
                        }

                        else if (IsLootable == true)
                        {
                            string TempLootablePaths = "111" + "," + lastX + "," + lastY + "," + CurrentPath.Facing + "," + IsLootable;

                            MessageBox.Show("New Lootable Zone Saved : " + "\n" + CurrentPath.CordX + "\n" + CurrentPath.CordY + "\n" + CurrentPath.Facing);

                            Writer(TempLootablePaths, "Paths.txt"); 

                            IsLootable = false;
                        }
                        lastX = CurrentPath.CordX; lastY = CurrentPath.CordY;

                    }
                    else { Row--; }

                    Task.Delay(TaskWaitTime).Wait();
                }
            });

            FetchRecordData.Start();
        }

        private void StopRecordEngine() 
        {
            TokenSurce.Cancel();
        }

        private void Test()
        {
            Writer("Hi", "Paths.txt");
        }

        public void Writer(string Data ,string FileName) 
        {
            string line = "";
            List <string> Pre=new List<string>();
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + FileName))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        Pre.Add(line);
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + FileName))
            {
                Pre.Add(Data);
                foreach (var item in Pre)
                {
                    sw.WriteLine(item);
                }
            }
        }

        private void btnRec_Click(object sender, EventArgs e)
        {
             eye.FetchData();
        }
    }
}
