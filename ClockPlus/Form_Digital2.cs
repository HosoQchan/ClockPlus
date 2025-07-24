using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace ClockPlus
{
    public partial class Form_Digital2 : Form
    {
        private ContextMenuStrip ContextMenu = new ContextMenuStrip();

        private Point mousePoint;
        private Size Ddisp1_strSize;
        static public int Dno = 1;         // デジタル表示no

        public System.Windows.Forms.ToolTip tooltip = new System.Windows.Forms.ToolTip();

        public Form_Digital2()
        {
            InitializeComponent();

            // コンテキストメニューの作成
            ContextMenu = App.ContextMenu_Create();
            ContextMenu.Items[0].Click += Setting_Click;
            ContextMenu.Items[2].Click += Alarm_Click;
            ContextMenu.Items[3].Click += Timer_Click;
            ContextMenu.Items[4].Click += StopWatch_Click;
            ContextMenu.Items[6].Click += Alarm_Stop_Click;
            ContextMenu.Items[8].Click += About_Click;

            ToolStripMenuItem Menu_Close = new ToolStripMenuItem();
            Menu_Close.Text = "閉じる";
            ContextMenu.Items.RemoveAt(10);
            ContextMenu.Items.Add(Menu_Close);
            ContextMenu.Items[10].Click += Close_Click;
            this.ContextMenuStrip = ContextMenu;

            // コンストラクタ内もしくはGUIの設定でMinimizedを指定しておく
            // これを指定しないと起動した瞬間一瞬だけ表示されてしまう
            this.WindowState = FormWindowState.Minimized;

            this.TransparencyKey = SystemColors.ActiveCaption;  // フォームの透明色を指定
            this.FormBorderStyle = FormBorderStyle.None;        // フォーム枠非表示
            // フォームを非表示
            this.Visible = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(XML_Main.cnf.Display.Digtal[1].PosX, XML_Main.cnf.Display.Digtal[1].PosY);
            this.Opacity = XML_Main.cnf.Display.Digtal[1].Opacity;
            this.TopMost = XML_Main.cnf.Display.Digtal[1].TopMost;

            PicBox_Disp.MouseDown += PicBox_Disp_MouseDown;
            PicBox_Disp.MouseMove += PicBox_Disp_MouseMove;
            PicBox_Disp.MouseDoubleClick += PicBox_Disp_MouseDoubleClick;

            if (Digital_Drow.Digital_Disp_Check(Dno))
            {
                FormCtrl_Net.valid_Form(this);          // フォームを表示
                Disp2_Drow();                           // デジタル表示２を描画
            }
        }

        private void Digital_Disp2_FormClosed(object sender, FormClosedEventArgs e)
        {
            PicBox_Disp.MouseDown -= PicBox_Disp_MouseDown;
            PicBox_Disp.MouseMove -= PicBox_Disp_MouseMove;
            PicBox_Disp.MouseDoubleClick -= PicBox_Disp_MouseDoubleClick;
        }
        private void PicBox_Disp_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousePoint = e.Location;
            }
        }
        private void PicBox_Disp_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int xDiff = e.Location.X - mousePoint.X;
                int yDiff = e.Location.Y - mousePoint.Y;

                // フォームの位置を設定
                this.Location = new Point(this.Location.X + xDiff, this.Location.Y + yDiff);
                Point point = this.Location;
                // フォームの位置を取得する
                XML_Main.cnf.Display.Digtal[Dno].PosX = point.X;
                XML_Main.cnf.Display.Digtal[Dno].PosY = point.Y;
            }
        }
        private void PicBox_Disp_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (e.Clicks == 2))
            {
                if (Sound_Ctrl.Sound_Play_Flag)
                {
                    Sound_Ctrl.Sound_Stop_Req();
                }
                else
                {
                    /*
                    Form_Menu Form = new Form_Menu();
                    Form.ShowDialog();
                    */
                }
            }
        }

        private void Setting_Click(object sender, EventArgs e)
        {
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

        private void Alarm_Stop_Click(object sender, EventArgs e)
        {
            Sound_Ctrl.Sound_Stop_Req();
        }

        private void About_Click(object sender, EventArgs e)
        {
            WindowManager.ShowOrActivate<Form_About>();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            XML_Main.cnf.Display.Digtal[1].Enable = false;
        }

        // デジタル表示２を描画
        public void Disp2_Drow()
        {
            string _Format = XML_Main.cnf.Display.Digtal[Dno].Format;
            Font _Font = Etc.Font_fromStr(XML_Main.cnf.Display.Digtal[Dno].Font);

            FontFamily F_family = _Font.FontFamily;
            int F_style = (int)_Font.Style;
            float F_size = _Font.Size;

            Color _Font_Color = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Dno].Font_Color);
            Color _Back_Color;
            if (XML_Main.cnf.Display.Digtal[Dno].Clear_Background)
            {
                _Back_Color = this.TransparencyKey;
            }
            else
            {
                _Back_Color = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Dno].Back_Color);
            }

            int _Outline_Thick = XML_Main.cnf.Display.Digtal[Dno].Outline_Thick;
            Color _Outline_Color = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Dno].Outline_Color);
            string _Paint_Style = XML_Main.cnf.Display.Digtal[Dno].Paint_Style;

            CultureInfo Japanese = new CultureInfo("ja-JP");
            Japanese.DateTimeFormat.Calendar = new JapaneseCalendar();

            string str = DateTime.Now.ToString(_Format);
            if (_Format.Contains("gg"))
            {
                str = DateTime.Now.ToString(_Format, Japanese);
            }
            switch (_Format)
            {
                case "yy/M/d":
                    str = String.Format("{0,8}", str);
                    break;
                case "yy/M/d (ddd)":
                    str = String.Format("{0,12}", str);
                    break;
                case "ggy年M月d日":
                    str = String.Format("{0,11}", str);
                    break;
                case "ggy年M月d日 (ddd)":
                    str = String.Format("{0,15}", str);
                    break;
                case "ggy年M月d日 dddd":
                    str = String.Format("{0,15}", str);
                    break;
                case "H時m分":
                    str = String.Format("{0,6}", str);
                    break;
                case "H時m分s秒":
                    str = String.Format("{0,9}", str);
                    break;
                case "tth時m分":
                    str = String.Format("{0,8}", str);
                    break;
                case "tth時m分s秒":
                    str = String.Format("{0,11}", str);
                    break;
                default:
                    break;
            }
            Size StrSize = Digital_Drow.TextRenderer_SizeGet(Dno, str, label1);   // 文字列を描画するときの大きさを取得
            PicBox_Disp.BackColor = _Back_Color;                         // 時計の背景色

            Bitmap bitmap = new Bitmap(StrSize.Width, StrSize.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            GraphicsPath graphicspath = new GraphicsPath();

            graphicspath.AddString(str, F_family, F_style, F_size, new Point(1, 1), new StringFormat());
            // 淵処理
            if (XML_Main.cnf.Display.Digtal[Dno].Outline_Thick > 0)
            {
                graphics.DrawPath(new Pen(_Outline_Color, _Outline_Thick)
                { LineJoin = LineJoin.Bevel }, graphicspath);
            }
            // グラデーション
            if (_Paint_Style == "Off")
            {
                // Offの場合は、とりあえずHatchBrushで代用
                HatchBrush hb = new HatchBrush(0, _Font_Color, _Font_Color);
                graphics.FillPath((hb), graphicspath);
            }
            else
            {
                // 画像にグラデーションをかける
                Digital_Drow.Gradation_Draw(Dno, _Paint_Style, graphics, graphicspath, PicBox_Disp.Size);
            }

            // 拡大縮小
            Bitmap src = bitmap;
            int w = (int)(src.Width * XML_Main.cnf.Display.Digtal[Dno].Zoom);
            int h = (int)(src.Height * XML_Main.cnf.Display.Digtal[Dno].Zoom);
            this.Width = w;
            this.Height = h;
            Bitmap dest = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(dest);
            g.InterpolationMode = InterpolationMode.Default;
            g.DrawImage(src, 0, 0, w, h);
            bitmap = src;

            PicBox_Disp.Dock = DockStyle.Fill;               // pictureBoxをFormに合わせる
            PicBox_Disp.SizeMode = PictureBoxSizeMode.Zoom;  // 画像比率はそのままにPictureBoxのサイズを拡大縮小

            // 画像を表示
            if (PicBox_Disp.Image != null)
            {
                PicBox_Disp.Image.Dispose();
                PicBox_Disp.Image = bitmap;
                graphics.Dispose();
                graphics = null;
                graphicspath.Dispose();
                graphicspath = null;
            }
            else
            {
                PicBox_Disp.Image = bitmap;
                graphics.Dispose();
                graphics = null;
                graphicspath.Dispose();
                graphicspath = null;
            }
            this.Opacity = XML_Main.cnf.Display.Digtal[Dno].Opacity;
        }

    }
}
