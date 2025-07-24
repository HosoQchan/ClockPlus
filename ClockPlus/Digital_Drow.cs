using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ClockPlus
{
    static public class Digital_Drow
    {
        // 画像にグラデーションをかける
        static public void Gradation_Draw(int Dno,string brush, Graphics graphics, GraphicsPath graphicspath, Size size)
        {
            switch (brush)
            {
                case "HatchBrush":
                    HatchBrush(Dno,graphics, graphicspath);
                    break;
                case "LinearGradientBrush":
                    LinearGradientBrush(Dno,graphics, graphicspath, size);
                    break;
            }
        }

        static public void LinearGradientBrush(int Dno,Graphics g, GraphicsPath gp, Size siz)
        {
            // LinearGradientBrush
            //
            // LinearGradientMode.
            // Horizontal       左から右へのグラデーション
            // Vertical         上から下へのグラデーション
            // ForwardDiagonal  左上から右下へのグラデーション
            // BackwardDiagonal 右上から左下へのグラデーション

            int data = XML_Main.cnf.Display.Digtal[Dno].LGB_Mode;
            LinearGradientMode mode = (LinearGradientMode)data;
            Color Color1 = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Dno].LGB_Color1);
            Color Color2 = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Dno].LGB_Color2);

            LinearGradientBrush gb = new LinearGradientBrush(
            new Rectangle(new Point(1, 1), siz), Color1, Color2, mode);
            g.FillPath((gb), gp);
        }

        static public void HatchBrush(int Dno,Graphics g, GraphicsPath gp)
        {
            int data = XML_Main.cnf.Display.Digtal[Dno].HB_Style;
            HatchStyle style = (HatchStyle)data;
            Color Color1 = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Dno].HB_Color1);
            Color Color2 = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Dno].HB_Color2);

            HatchBrush hb = new HatchBrush(style, Color1, Color2);
            g.FillPath((hb), gp);
        }

        // 文字列を描画するときの大きさを取得
        static public Size TextRenderer_SizeGet(int Dno,string Data, Label label)
        {
            Font font = Etc.Font_fromStr(XML_Main.cnf.Display.Digtal[Dno].Font);
            label.Font = font;
            label.AutoSize = true;
            label.Text = Data;

            // ディスプレイの画面解像度と倍率を求める
            double dpiMag = SystemParameters.PrimaryScreenHeight;   // 拡大時の画面高さ(Pixel)
            double dpi = Screen.PrimaryScreen.Bounds.Height;        // 画面の設定(Pixel)
            double dpiMagRate = dpi / dpiMag;                       // 倍率

            int w = (int)((label.Width / dpiMagRate) + XML_Main.cnf.Display.Digtal[Dno].Outline_Thick);
            int h = (int)((label.Height / dpiMagRate) + XML_Main.cnf.Display.Digtal[Dno].Outline_Thick);

            Size size = new Size(w,h);
            return size;
        }

        // デジタル時計の表示条件を満たしているか
        static public bool Digital_Disp_Check(int Dno)
        {
            if (XML_Main.cnf.Display.Digtal[Dno].Enable)
            {
                return true;
            }
            else
            {
                if (FormCtrl_Net.Display_Event_Timer > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // デジタル表示の再描画
        static public void Digital_Disp_ReDrow(int Setting_Dno)
        {
            switch (Setting_Dno)
            {
                case 0:
                    App.form_digital1.Disp1_Drow();       // デジタル時計１を描画
                    break;
                case 1:
                    App.form_digital2.Disp2_Drow();       // デジタル時計２を描画
                    break;
            }
        }
    }
}
