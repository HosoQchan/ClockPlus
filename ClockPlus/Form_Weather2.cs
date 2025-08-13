using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ClockPlus.WeatherHacks;
using Svg;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows;
using System.Reflection.Metadata;
using ControlzEx.Standard;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Controls.Primitives;

namespace ClockPlus
{
    public partial class Form_Weather2 : Form
    {
        private ContextMenuStrip ContextMenu = new ContextMenuStrip();

        private bool Form_Loading = false;
        private System.Drawing.Point mousePoint;
        private const int Width = 278;
        private const int Height = 194;

        static public Bitmap WeatherCapt = null;

        public Form_Weather2()
        {
            InitializeComponent();

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

            this.TransparencyKey = System.Drawing.SystemColors.ActiveCaption;  // フォームの透明色を指定
            this.BackColor = this.TransparencyKey;
            this.FormBorderStyle = FormBorderStyle.None;

            FormCtrl_Net.Hide_Form(this);           // フォームを非表示
        }

        private void Form_Load(object sender, EventArgs e)
        {
            // 位置・不透明度・最前面の設定
            this.Location = new System.Drawing.Point(XML_Main.cnf.Weather.Weather_Disp[1].PosX, XML_Main.cnf.Weather.Weather_Disp[1].PosY);
            this.Opacity = XML_Main.cnf.Weather.Weather_Disp[1].Opacity;
            this.TopMost = XML_Main.cnf.Weather.Weather_Disp[1].TopMost;

            // 倍率設定
            Weather_Disp2_Size(App.form_weather2);

            if (XML_Main.cnf.Weather.Weather_Disp[1].Enable)
            {
                FormCtrl_Net.valid_Form(this);      // フォームを表示

                // 明日の天気予報を描画する
                Weather_Disp2_Drow();
            }

            PictureBox_Form.MouseDown += Panel_MouseDown;
            PictureBox_Form.MouseMove += Panel_MouseMove;
        }

        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            PictureBox_Form.MouseDown -= Panel_MouseDown;
            PictureBox_Form.MouseMove -= Panel_MouseMove;
        }

        // 倍率設定
        static public void Weather_Disp2_Size(System.Windows.Forms.Form form)
        {
            int Wid;
            int Hei;
            Wid = (int)(Width * (XML_Main.cnf.Weather.Weather_Disp[1].Zoom));
            Hei = (int)(Height * (XML_Main.cnf.Weather.Weather_Disp[1].Zoom));
            System.Drawing.Size Form_size = new System.Drawing.Size(Wid, Hei);
            form.Size = Form_size;
        }

        // 明日の天気予報を描画
        public void Weather_Disp2_Drow()
        {
            Bitmap canvas = null;

            // 描画先とするオブジェクトを作成する
            canvas = new Bitmap(Form_Weather2.Width, Form_Weather2.Height);
            // ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(canvas);

            // アンチエイリアス設定 (必要に応じて)
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            // 画像を拡大、縮小して描画する時の補間設定
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            // フォント
            System.Drawing.Font fnt = new System.Drawing.Font("メイリオ", 8);

            // 中央揃え
            StringFormat sf_center = new StringFormat();
            sf_center.Alignment = StringAlignment.Center;       // 水平方向
            sf_center.LineAlignment = StringAlignment.Center;   // 垂直方向

            // 左寄せ
            StringFormat sf_left = new StringFormat();
            sf_left.Alignment = StringAlignment.Near;           // 水平方向
            sf_left.LineAlignment = StringAlignment.Center;     // 垂直方向

            // 右寄せ
            StringFormat sf_right = new StringFormat();
            sf_right.Alignment = StringAlignment.Far;           // 水平方向
            sf_right.LineAlignment = StringAlignment.Center;    // 垂直方向

            // 文字の色
            Brush Text_Color = new SolidBrush(Etc.Color_Code(XML_Main.cnf.Weather.Weather_Disp[1].Font_Color));     // メイン
            Brush Tmpmax_Color = new SolidBrush(Etc.Color_Code(XML_Main.cnf.Weather.Weather_Disp[1].Font_Color1));  // 最高気温 
            Brush Tmpmin_Color = new SolidBrush(Etc.Color_Code(XML_Main.cnf.Weather.Weather_Disp[1].Font_Color2));  // 最低気温 

            string Text = "";
            // RectangleF(横位置 , 縦位置 , 横幅 , 高さ)
            // RectangleF(《Point》,《Size》)

            g.DrawString("最高気温", fnt, Tmpmax_Color, new RectangleF(98, 59, 74, 23), sf_right);
            g.DrawString("最低気温", fnt, Tmpmin_Color, new RectangleF(98, 82, 74, 23), sf_right);
            g.DrawString("降水確率", fnt, Text_Color, new RectangleF(12, 123, 250, 23), sf_center);
            g.DrawString("00-06", fnt, Text_Color, new RectangleF(12, 146, 58, 23), sf_center);
            g.DrawString("06-12", fnt, Text_Color, new RectangleF(76, 146, 58, 23), sf_center);
            g.DrawString("12-18", fnt, Text_Color, new RectangleF(140, 146, 58, 23), sf_center);
            g.DrawString("18-24", fnt, Text_Color, new RectangleF(204, 146, 58, 23), sf_center);

            // 指定されたcityCodeの天気予報データを取得する
            WeatherHacks weatherHacks = new WeatherHacks();
            weatherHacks.Get_Weather(XML_Main.cnf.Weather.AreaCode);

            // 天気
            if (tenki != null)
            {
                // 明日の天気情報を取得する
                Forecast forecast = new Forecast();
                forecast = WeatherHacks.tenki.forecasts[1];

                // タイトル・見出し（例・福岡県 久留米 の天気）
                Text = WeatherHacks.tenki.title;
                g.DrawString(Text, fnt, Text_Color, new RectangleF(12, 9, 194, 23), sf_left);

                // 予報日（今日・明日・明後日のいずれか）
                Text = forecast.dateLabel;
                g.DrawString(Text, fnt, Text_Color, new RectangleF(212, 9, 50, 23), sf_right);

                // 天気（晴れ、曇り、雨など）
                Text = forecast.telop;
                g.DrawString(Text, fnt, Text_Color, new RectangleF(12, 32, 250, 23), sf_left);

                // 最高気温
                Text = forecast.temperature.max.celsius;
                g.DrawString(Text, fnt, Tmpmax_Color, new RectangleF(178, 59, 50, 23), sf_left);

                // 最低気温
                Text = forecast.temperature.min.celsius;
                g.DrawString(Text, fnt, Tmpmin_Color, new RectangleF(178, 82, 50, 23), sf_left);

                // 降水確率
                Text = forecast.chanceOfRain.T00_06;
                g.DrawString(Text, fnt, Text_Color, new RectangleF(12, 169, 58, 23), sf_center);
                Text = forecast.chanceOfRain.T06_12;
                g.DrawString(Text, fnt, Text_Color, new RectangleF(76, 169, 58, 23), sf_center);
                Text = forecast.chanceOfRain.T12_18;
                g.DrawString(Text, fnt, Text_Color, new RectangleF(140, 169, 58, 23), sf_center);
                Text = forecast.chanceOfRain.T18_24;
                g.DrawString(Text, fnt, Text_Color, new RectangleF(204, 169, 58, 23), sf_center);

                // 天気アイコン
                Bitmap bmp = Etc.ConvertSvgToBitmap(forecast.image.url, forecast.image.width, forecast.image.height);
                //画像をcanvasの指定座標に描画する
                g.DrawImage(bmp, 12, 59, 80, 60);
            }
            if (XML_Main.cnf.Weather.Weather_Disp[1].Clear_Background)
            {
                PictureBox_Form.BackColor = Color.Transparent;    // 背景色を透過にする
            }
            else
            {
                PictureBox_Form.BackColor = Etc.Color_Code(XML_Main.cnf.Weather.Weather_Disp[1].Back_Color);
            }
            PictureBox_Form.Image = canvas;
            g.Dispose();
        }

        private void Setting_Click(object sender, EventArgs e)
        {
            Form_Setting_Main.Tab_Page = "天気";
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
            XML_Main.cnf.Weather.Weather_Disp[1].Enable = false;
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousePoint = e.Location;
            }
        }
        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int xDiff = e.Location.X - mousePoint.X;
                int yDiff = e.Location.Y - mousePoint.Y;

                // フォームの位置を設定
                this.Location = new System.Drawing.Point(this.Location.X + xDiff, this.Location.Y + yDiff);
                System.Drawing.Point point = this.Location;
                // フォームの位置を取得する
                XML_Main.cnf.Weather.Weather_Disp[1].PosX = point.X;
                XML_Main.cnf.Weather.Weather_Disp[1].PosY = point.Y;
            }
        }
    }
}
