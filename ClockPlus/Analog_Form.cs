using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClockPlus
{
    public partial class Analog_Form : Form
    {
        private Point mousePoint;

        public Bitmap Clk_Face;
        public Bitmap Clk_Second;
        public Bitmap Clk_Minute;
        public Bitmap Clk_Hour;

        static public Size Clk_Size;

        public Analog_Form()
        {
            InitializeComponent();
            FormCtrl_Net.Hide_Form(this);      // フォームを非表示

            // フォーム非表示
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            this.TransparencyKey = this.BackColor;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void Analog_Disp_Load(object sender, EventArgs e)
        {
            // フォームの位置を設定
            this.Location = new Point(XML_Main_IO.Main.Display.Analog.PosX, XML_Main_IO.Main.Display.Analog.PosY);
            this.Opacity = XML_Main_IO.Main.Display.Analog.Opacity;

            Default_Skin_Create();      // デフォルトスキンの生成

            Pbox_Sec.MouseDown += PBox_Sec_MouseDown;
            Pbox_Sec.MouseMove += PBox_Sec_MouseMove;
            Pbox_Sec.MouseDoubleClick += PBox_Sec_MouseDoubleClick;

            Analog_Disp_init();                     // アナログ時計を表示
            if (Analog_Disp_Check())
            {
                FormCtrl_Net.valid_Form(this);      // フォームを表示
                Analog_Disp_Drow();                 // アナログ時計の描画更新
            }
        }

        private void Analog_Disp_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pbox_Sec.MouseDown -= PBox_Sec_MouseDown;
            Pbox_Sec.MouseMove -= PBox_Sec_MouseMove;
            Pbox_Sec.MouseDoubleClick -= PBox_Sec_MouseDoubleClick;
        }
        private void PBox_Sec_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousePoint = e.Location;
            }
        }
        private void PBox_Sec_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int xDiff = e.Location.X - mousePoint.X;
                int yDiff = e.Location.Y - mousePoint.Y;

                // フォームの位置を設定
                this.Location = new Point(this.Location.X + xDiff, this.Location.Y + yDiff);
                Point point = this.Location;
                // フォームの位置を取得する
                XML_Main_IO.Main.Display.Analog.PosX = point.X;
                XML_Main_IO.Main.Display.Analog.PosY = point.Y;
            }
        }
        private void PBox_Sec_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (e.Clicks == 2))
            {

             
            }
        }

        // アナログ時計を表示
        public void Analog_Disp_init()
        {
            string Dir;
            Dir = XML_Main_IO.Skin_Dir + "\\" + XML_Main_IO.Main.Display.Analog.Skin;
            Dir = XML_Main_IO.Skin_Dir + "\\" + XML_Main_IO.Main.Display.Analog.Skin;
            if (Directory.Exists(Dir) == false)
            {
                XML_Main_IO.Main.Display.Analog.Skin = "default";
            }
            Dir = XML_Main_IO.Skin_Dir + "\\" + XML_Main_IO.Main.Display.Analog.Skin + "\\";
            Dir = XML_Main_IO.Skin_Dir + "\\" + XML_Main_IO.Main.Display.Analog.Skin + "\\";

            Clk_Face = (Bitmap)Image.FromFile(Dir + "Clock_Face.png");           // 時計の文字盤
            Clk_Second = (Bitmap)Image.FromFile(Dir + "Clock_Hand_Sec.png");     // 秒
            Clk_Minute = (Bitmap)Image.FromFile(Dir + "Clock_Hand_Min.png");     // 分
            Clk_Hour = (Bitmap)Image.FromFile(Dir + "Clock_Hand_Hours.png");     // 時

            // 時計の基本サイズ
            int x = Clk_Face.Width;
            int y = Clk_Face.Height;
            Clk_Size = new Size(x, y);
            Analog_Disp_Size(App._Analog_Form);        // アナログ時計の倍率設定

            Pbox_face.Dock = DockStyle.Fill;                          // pictureBoxをForm1に合わせて適切なサイズに調節
            Pbox_face.SizeMode = PictureBoxSizeMode.Zoom;             // pictureBoxをサイズ比率を維持して拡大・縮小

            Pbox_Hour.Dock = DockStyle.Fill;
            Pbox_Hour.SizeMode = PictureBoxSizeMode.Zoom;

            Pbox_Min.Dock = DockStyle.Fill;
            Pbox_Min.SizeMode = PictureBoxSizeMode.Zoom;

            Pbox_Sec.Dock = DockStyle.Fill;
            Pbox_Sec.SizeMode = PictureBoxSizeMode.Zoom;

            Clk_Face.MakeTransparent();
            Clk_Hour.MakeTransparent();
            Clk_Minute.MakeTransparent();
            Clk_Second.MakeTransparent();

            System.Drawing.Graphics g;
            g = Pbox_Hour.CreateGraphics();
            g.DrawImage(Clk_Hour, new System.Drawing.Point(0, 0));
            g.Dispose();

            g = Pbox_Min.CreateGraphics();
            g.DrawImage(Clk_Minute, new System.Drawing.Point(0, 0));
            g.Dispose();

            g = Pbox_Sec.CreateGraphics();
            g.DrawImage(Clk_Second, new System.Drawing.Point(0, 0));
            g.Dispose();

            Pbox_face.Image = Clk_Face;

            Pbox_Hour.Parent = Pbox_face;
            Pbox_Min.Parent = Pbox_Hour;
            Pbox_Sec.Parent = Pbox_Min;

            Pbox_face.BackColor = Color.Transparent;    // 背景色を透過にする
            Pbox_Hour.BackColor = Color.Transparent;
            Pbox_Min.BackColor = Color.Transparent;
            Pbox_Sec.BackColor = Color.Transparent;
        }

        // アナログ時計の描画更新
        public void Analog_Disp_Drow()
        {
            if (!XML_Main_IO.Main.Display.Analog.Enable)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            this.WindowState = FormWindowState.Normal;

            DateTime time = DateTime.Now;
            float SecondAng = (float)(time.Second * 6.0);
            float MinuteAng = (float)((time.Minute + time.Second / 60.0) * 6.0);
            float HourAng = (float)((time.Hour + time.Minute / 60.0) * 30.0);

            if (Pbox_Sec.Image != null)
            {
                Pbox_Sec.Image.Dispose();
            }
            if (Pbox_Min.Image != null)
            {
                Pbox_Min.Image.Dispose();
            }
            if (Pbox_Hour.Image != null)
            {
                Pbox_Hour.Image.Dispose();
            }
            Pbox_Sec.Image = Analog_RotateBitmap(Clk_Second, SecondAng, Clk_Second.Width / 2, Clk_Second.Height / 2);
            Pbox_Min.Image = Analog_RotateBitmap(Clk_Minute, MinuteAng, Clk_Minute.Width / 2, Clk_Minute.Height / 2);
            Pbox_Hour.Image = Analog_RotateBitmap(Clk_Hour, HourAng, Clk_Hour.Width / 2, Clk_Hour.Height / 2);
        }

        // 時計の針画像の回転
        public Bitmap Analog_RotateBitmap(Bitmap org_bmp, float angle, int x, int y)
        {
            Bitmap result_bmp;
            Graphics g;

            result_bmp = new Bitmap((int)org_bmp.Width, (int)org_bmp.Height);
            g = Graphics.FromImage(result_bmp);
            g.TranslateTransform(-x, -y);
            g.RotateTransform(angle, System.Drawing.Drawing2D.MatrixOrder.Append);
            g.TranslateTransform(x, y, System.Drawing.Drawing2D.MatrixOrder.Append);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(org_bmp, 0, 0);
            g.Dispose();
            return result_bmp;
        }
        private void Default_Skin_Create()
        {
            Bitmap bmp;
            string Dir = XML_Main_IO.Skin_Dir + "\\default\\";                  // skinのあるディレクトリ名
            if (Directory.Exists(Dir) == false)
            {
                Directory.CreateDirectory(Dir);
                //リソースを読み込む
                bmp = new Bitmap(Properties.Resources.Clock_Face);
                //ファイルを書き込む
                bmp.Save(Dir + "Clock_Face.png", ImageFormat.Png);

                //リソースを読み込む
                bmp = new Bitmap(Properties.Resources.Clock_Hand_Hours);
                //ファイルを書き込む
                bmp.Save(Dir + "Clock_Hand_Hours.png", ImageFormat.Png);

                //リソースを読み込む
                bmp = new Bitmap(Properties.Resources.Clock_Hand_Min);
                //ファイルを書き込む
                bmp.Save(Dir + "Clock_Hand_Min.png", ImageFormat.Png);

                //リソースを読み込む
                bmp = new Bitmap(Properties.Resources.Clock_Hand_Sec);
                //ファイルを書き込む
                bmp.Save(Dir + "Clock_Hand_Sec.png", ImageFormat.Png);
            }
        }

        private void 設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting_Analog_Win _Setting_Analog_Win = new Setting_Analog_Win();
            _Setting_Analog_Win.ShowDialog();
        }

        private void アラームToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void タイマーToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ストップウォッチToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void アラームの解除ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XML_Main_IO.Main.Display.Analog.Enable = false;
            FormCtrl_Net.Hide_Form(App._Analog_Form);      // フォームを非表示
        }

        // アナログ時計の表示条件を満たしているか
        static public bool Analog_Disp_Check()
        {
            if (XML_Main_IO.Main.Display.Analog.Enable)
            {
                return true;
            }
            else
            {
                if ((XML_Main_IO.Main.Display.Analog.Display_Event) && (FormCtrl_Net.Display_Event_Timer > 0))
                {
                    return true;
                }
            }
            return false;
        }

        // アナログ時計の倍率設定
        static public void Analog_Disp_Size(System.Windows.Forms.Form form)
        {
            int Wid;
            int Hei;
            Wid = (int)((Analog_Form.Clk_Size.Width) * (XML_Main_IO.Main.Display.Analog.Zoom));
            Hei = (int)((Analog_Form.Clk_Size.Height) * (XML_Main_IO.Main.Display.Analog.Zoom));
            System.Drawing.Size Form_size = new System.Drawing.Size(Wid, Hei);
            form.Size = Form_size;
        }
    }
}
