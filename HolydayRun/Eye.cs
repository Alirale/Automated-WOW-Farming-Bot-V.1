using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HolydayRun
{
    public class Eye
    {
        public static List<Point> ColorPoints = new List<Point>();
        private Bitmap bmp;
        static public List<Int64> Hexs;
        private Bitmap image;
        private Graphics G;
        private Bitmap img2;
        public static Mutex EyeLock = new Mutex();

        private static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private Bitmap Save(Int32 x, Int32 y, Int32 w, Int32 h, Size s)
        {
            Rectangle rect = new Rectangle(x, y, w, h);
            bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, s, CopyPixelOperation.SourceCopy);
            return bmp;
        }


        private Bitmap TakeScreenShot()
        {
            var size = Screen.PrimaryScreen.Bounds.Size;
            var height = Screen.PrimaryScreen.Bounds.Height / 5;
            var width = Screen.PrimaryScreen.Bounds.Width / 5;
            var x = 0;
            var y = 0;
            Bitmap screen = Save(x, y, width, height, size);
            return screen;
        }


        public Bitmap ScreenCapture2()
        {
            using (image = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.CopyFromScreen(0, 0, 0, 0, image.Size, CopyPixelOperation.SourceCopy);
                }
                return image;
            }
        }


        public Bitmap Capture(Rectangle Region)
        {
            img2 = null;
            Task.Run(() =>
            {
                image = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

                using (G = Graphics.FromImage(image))
                {
                    G.CopyFromScreen(Region.Location, Point.Empty, Region.Size, CopyPixelOperation.SourceCopy);
                    G.Flush();
                    img2 = image;
                    return image;
                }
            });
            return img2;
        }

        public void Dispoer()
        {
            image.Dispose();         
        }

        public Bitmap TakeFullScreenShot()
        {
            var size = Screen.PrimaryScreen.Bounds.Size;
            var height = Screen.PrimaryScreen.Bounds.Height;
            var width = Screen.PrimaryScreen.Bounds.Width;
            var x = 0;
            var y = 0;
            Bitmap screen = Save(x, y, width, height, size);
            return screen;
        }

        private string GetHTMLColor(Color pixelColor)
        {
            return ColorTranslator.ToHtml(pixelColor);
        }
        public void Calibrate()
        {
                Bitmap bmp = TakeScreenShot();
                if (bmp == null)
                {
                    Application.Restart();
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
                Hexs = new List<Int64>();
                int x = 3, y = 2;
                Color pixelColor = bmp.GetPixel(x, y);
                Color black = bmp.GetPixel(x, y);
                Color purple = bmp.GetPixel(x, y);
                Boolean imcond = true;
                Boolean BlackCond = true;
                Boolean Purple1 = true; Boolean Purple2 = true;

                while (imcond)
                {
                    while (BlackCond)
                    {
                        pixelColor = bmp.GetPixel(x, y);
                        y++;
                        if (GetHTMLColor(pixelColor) == GetHTMLColor(black))
                        {
                            break;
                        }
                        y = y + 2;
                        purple = bmp.GetPixel(x, y);
                        imcond = false;
                        BlackCond = false;
                    }
                }


                while (Purple1)
                {
                    Again:
                    while (Purple2)
                    {

                        pixelColor = bmp.GetPixel(x, y);
                        y++;
                        if (GetHTMLColor(pixelColor) == GetHTMLColor(purple))
                        {
                            break;
                        }
                        if (GetHTMLColor(bmp.GetPixel(x, y - 1)) != GetHTMLColor(purple))
                        {
                            y = y + 2;
                            Point temp = Point.Empty; temp.X = x; temp.Y = y;
                            ColorPoints.Add(temp);
                            Purple1 = false; Purple2 = false;
                        }
                        else { goto Again; }
                    }

                }

                bool cond1 = true; bool cond2 = true;
                for (int i = 0; i < 4; i++)
                {
                    cond1 = true; cond2 = true;
                    Color Choosen = bmp.GetPixel(x, y);

                    while (cond1)
                    {
                        Again:;
                        while (cond2)
                        {
                            pixelColor = bmp.GetPixel(x, y);
                            y++;
                            if (GetHTMLColor(pixelColor) == GetHTMLColor(Choosen) || GetHTMLColor(pixelColor) == GetHTMLColor(purple))
                            {
                                break;
                            }
                            if (GetHTMLColor(bmp.GetPixel(x, y + 1)) == GetHTMLColor(pixelColor))
                            {
                                y = y + 2;
                                cond1 = false; cond2 = false;
                                Point temp = Point.Empty; temp.X = x; temp.Y = y;
                                ColorPoints.Add(temp);
                            }
                            else { goto Again; }
                        }
                    }
                }

                foreach (var item in ColorPoints)
                {
                    var temp = GetHTMLColor(bmp.GetPixel(item.X, item.Y));
                    Int64 Hex = Convert.ToInt64(temp.Trim('#'), 16);
                    Hexs.Add(Hex);
                }
                List<string> Datas = new List<string>();

                Datas = FetchData();

                string Temp2 = "";
                foreach (var item in Datas)
                {
                    Temp2 += item;
                    Temp2 += "\n";
                }
        }


        public void GetMouseColor()
        {
            MouseClickStuffs mvsc = new MouseClickStuffs();
            Point Temp = mvsc.GetCursorPosition();
            Bitmap bmp = TakeFullScreenShot();
            Color TempColor = bmp.GetPixel(Temp.X, Temp.Y);
            MessageBox.Show(GetHTMLColor(TempColor));
        }

        public List<string> FetchData()
        {
            Bitmap bmp = TakeScreenShot();

            List<Int64> newHexs = new List<Int64>();
            List<string> Outputs = new List<string>();

            foreach (var item in ColorPoints)
            {
                Color TmpColor = bmp.GetPixel(item.X, item.Y);
                string temp = GetHTMLColor(TmpColor);
                Int64 Hex = Convert.ToInt64(temp.Trim('#'), 16);
                newHexs.Add(Hex);
                temp = null;
            }

            Outputs.Add(newHexs[0].ToString());
            Outputs.Add(newHexs[1].ToString());
            int first = Convert.ToInt32(newHexs[2]) / 100000;
            int last = Convert.ToInt32(newHexs[2]) % 100000;
            string RadFace = first.ToString() + "." + last.ToString();
            Double RadianFace = Convert.ToDouble(RadFace);
            Outputs.Add((DegreeTurn(RadianFace)).ToString());
            Outputs.Add((newHexs[3]).ToString());
            Outputs.Add((newHexs[4]).ToString());

            if (Outputs == null)
            {
                MessageBox.Show("null fetch");
            }

            return Outputs;
        }

        private double DegreeTurn(double Rad)
        {
            return Rad * (180 / Math.PI);
        }

    }
}