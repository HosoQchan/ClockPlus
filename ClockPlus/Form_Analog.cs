using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace ClockPlus
{
    public partial class Form_Analog : Form
    {
        private ContextMenuStrip ContextMenu = new ContextMenuStrip();

        private Point mousePoint;
        private const int Width = 960;
        private const int Height = 960;

        public Bitmap Clk_Face;
        public Bitmap Clk_Second;
        public Bitmap Clk_Minute;
        public Bitmap Clk_Hour;

        public System.Windows.Forms.ToolTip tooltip = new System.Windows.Forms.ToolTip();

        public Form_Analog()
        {
            InitializeComponent();
            Pbox_face.Dock = DockStyle.Fill;                    // pictureBoxをForm1に合わせて適切なサイズに調節
            Pbox_face.SizeMode = PictureBoxSizeMode.Zoom;       // pictureBoxをサイズ比率を維持して拡大・縮小

            Pbox_Hour.Dock = DockStyle.Fill;
            Pbox_Hour.SizeMode = PictureBoxSizeMode.Zoom;

            Pbox_Min.Dock = DockStyle.Fill;
            Pbox_Min.SizeMode = PictureBoxSizeMode.Zoom;

            Pbox_Sec.Dock = DockStyle.Fill;
            Pbox_Sec.SizeMode = PictureBoxSizeMode.Zoom;

            // コンテキストメニューの作成
            ContextMenu = App.ContextMenu_Create();
            ContextMenu.Items[0].Click += Setting_Click;
            ContextMenu.Items[2].Click += Alarm_Click;
            ContextMenu.Items[3].Click += Timer_Click;
            ContextMenu.Items[4].Click += StopWatch_Click;
            ContextMenu.Items[6].Click += Alarm_Stop_Click;
            ContextMenu.Items[8].Click += Weather_Click;
            ContextMenu.Items[10].Click += About_Click;

            ToolStripMenuItem Menu_Close = new ToolStripMenuItem();
            Menu_Close.Text = "閉じる";
            ContextMenu.Items.RemoveAt(12);
            ContextMenu.Items.Add(Menu_Close);
            ContextMenu.Items[12].Click += Close_Click;
            this.ContextMenuStrip = ContextMenu;

            // フォーム非表示
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            this.TransparencyKey = this.BackColor;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void Analog_Disp_Load(object sender, EventArgs e)
        {
            // アナログ時計の倍率設定
            Form_Analog.Analog_Disp_Size(App.form_analog);

            // 位置・不透明度・最前面の設定
            this.Location = new Point(XML_Main.cnf.Display.Analog.PosX, XML_Main.cnf.Display.Analog.PosY);
            this.Opacity = XML_Main.cnf.Display.Analog.Opacity;
            this.TopMost = XML_Main.cnf.Display.Analog.TopMost;

            Analog_Disp_init();                     // アナログ時計を表示
            if (Analog_Disp_Check())
            {
                FormCtrl_Net.valid_Form(this);      // フォームを表示
                Analog_Disp_Drow();                 // アナログ時計の描画更新
            }

            Pbox_Sec.MouseDown += PBox_Sec_MouseDown;
            Pbox_Sec.MouseMove += PBox_Sec_MouseMove;
        }

        private void Analog_Disp_FormClosed(object sender, FormClosedEventArgs e)
        {
            Pbox_Sec.MouseDown -= PBox_Sec_MouseDown;
            Pbox_Sec.MouseMove -= PBox_Sec_MouseMove;
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
                XML_Main.cnf.Display.Analog.PosX = point.X;
                XML_Main.cnf.Display.Analog.PosY = point.Y;
            }
        }

        private void Setting_Click(object sender, EventArgs e)
        {
            Form_Setting_Main.Tab_Page = "時計";
            WindowManager.ShowOrActivate<Form_Setting_Main>();
        }
        private void Alarm_Click(object sender, EventArgs e)
        {
            WindowManager.ShowOrActivate<Form_Alarm_List>();
        }
        private void Timer_Click(object sender, EventArgs e)
        {
            WindowManager.ShowOrActivate<Form_Timer_List>();
        }
        private void StopWatch_Click(object sender, EventArgs e)
        {
            WindowManager.ShowOrActivate<Form_StopWatch>();
        }
        private void Weather_Click(object sender, EventArgs e)
        {
            WeatherHacks weatherHacks = new WeatherHacks();
            weatherHacks.Weather_HP();
        }
        private void Alarm_Stop_Click(object sender, EventArgs e)
        {
            Task_Ctrl.Sound_Stop_Req();
        }
        private void About_Click(object sender, EventArgs e)
        {
            WindowManager.ShowOrActivate<Form_About>();
        }
        private void Close_Click(object sender, EventArgs e)
        {
            XML_Main.cnf.Display.Analog.Enable = false;
        }

        // アナログ時計を表示
        public void Analog_Disp_init()
        {
            string Dir;
            Dir = XML_Main.Skin_Dir + "\\" + XML_Main.cnf.Display.Analog.Skin;
            if (Directory.Exists(Dir) == false)
            {
                XML_Main.cnf.Display.Analog.Skin = "default";
            }
            Dir = XML_Main.Skin_Dir + "\\" + XML_Main.cnf.Display.Analog.Skin + "\\";
            Clk_Face = (Bitmap)Image.FromFile(Dir + "Clock_Face.png");           // 時計の文字盤
            Clk_Second = (Bitmap)Image.FromFile(Dir + "Clock_Hand_Sec.png");     // 秒
            Clk_Minute = (Bitmap)Image.FromFile(Dir + "Clock_Hand_Min.png");     // 分
            Clk_Hour = (Bitmap)Image.FromFile(Dir + "Clock_Hand_Hours.png");     // 時

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
            // アナログ時計の倍率設定
            Form_Analog.Analog_Disp_Size(this);

            // 位置の設定
            this.Location = new Point(XML_Main.cnf.Display.Analog.PosX, XML_Main.cnf.Display.Analog.PosY);

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

        // アナログ時計の表示条件を満たしているか
        static public bool Analog_Disp_Check()
        {
            if (XML_Main.cnf.Display.Analog.Enable)
            {
                return true;
            }
            else
            {
                if ((XML_Main.cnf.Display.Analog.Display_Event) && (FormCtrl_Net.Display_Event_Timer > 0))
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
            Wid = (int)(Width * (XML_Main.cnf.Display.Analog.Zoom));
            Hei = (int)(Height * (XML_Main.cnf.Display.Analog.Zoom));
            System.Drawing.Size Form_size = new System.Drawing.Size(Wid, Hei);
            form.Size = Form_size;
        }
    }
}
