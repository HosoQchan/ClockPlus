using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClockPlus
{
    /// <summary>
    /// Form_Setting_Digital.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Setting_Digital : MetroWindow
    {
        // 日時フォーマット
        private string[] Clock_Format =
        {
            "yyyy/MM/dd",
            "yyyy/MM/dd (ddd)",
            "yyyy/MM/dd dddd",
            "yy/M/d",
            "yy/M/d (ddd)",
            "ggy年M月d日",
            "ggy年M月d日 (ddd)",
            "ggy年M月d日 dddd",
            "HH:mm",
            "HH:mm:ss",
            "H時m分",
            "H時m分s秒",
            "tth時m分",
            "tth時m分s秒"
        };

        // 装飾の種類
        private string[] Style_Item =
        {
            "Off",
            "HatchBrush",
            "LinearGradientBrush"
        };

        // LinearGradientBrushの種類
        private string[] LgBrush_Item =
        {
            "左から右へ",
            "上から下へ",
            "左上から右下へ",
            "右上から左下へ"
        };

        // HatchBrushの種類
        private List<string> Get_Hblush_Item()
        {
            List<string> item = new List<string>();
            item.Clear();
            foreach (string hs in Enum.GetNames(typeof(HatchStyle)))
            {
                item.Add(hs);
            }
            // 最後の3種類については、適用するとエラーを吐き出すので外す
            item.RemoveAt(item.Count - 1);
            item.RemoveAt(item.Count - 1);
            item.RemoveAt(item.Count - 1);
            return item;
        }

        // 設定対象のデジタル時計番号
        private int Setting_Dno = -1;

        private bool Form_Loaded_Flag = false;
        private bool Edit_Cancel = true;
        private Digital Setting_Backup = new Digital();

        public Form_Setting_Digital(int No)
        {
            Setting_Dno = No;
            InitializeComponent();

            // ディスプレイの縦横値からスライダーの最大値を決める
            Slider_Height.Maximum = XML_Main.Display_Height;
            Slider_Width.Maximum = XML_Main.Display_Width;

            Title = Title + (Setting_Dno + 1).ToString() + "時計";

            ComboBox_Format.Items.Clear();
            foreach (string item in Clock_Format)
            {
                ComboBox_Format.Items.Add(item);
            }
            ComboBox_Format.SelectedIndex = 0;

            ComboBox_Style.Items.Clear();
            foreach (string item in Style_Item)
            {
                ComboBox_Style.Items.Add(item);
            }
            ComboBox_Style.SelectedIndex = 0;

            ComboBox_HbBrush.Items.Clear();
            foreach (string item in Get_Hblush_Item())
            {
                ComboBox_HbBrush.Items.Add(item);
            }
            ComboBox_HbBrush.SelectedIndex = 0;

            ComboBox_LgBrush.Items.Clear();
            foreach (string item in LgBrush_Item)
            {
                ComboBox_LgBrush.Items.Add(item);
            }
            ComboBox_LgBrush.SelectedIndex = 0;

            OneColor_Grid.Visibility = Visibility.Hidden;   // 非表示にする(コンポーネントの場所はそのまま)
            HbBrush_Grid.Visibility = Visibility.Hidden;    // 非表示にする(コンポーネントの場所はそのまま)
            LgBrush_Grid.Visibility = Visibility.Hidden;    // 非表示にする(コンポーネントの場所はそのまま)
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            // 設定値をバックアップしておく
            Setting_Backup = Etc.JsonSerializerClone(XML_Main.cnf.Display.Digtal[Setting_Dno]);

            Read_Setting();
            Form_Loaded_Flag = true;
        }

        private void Read_Setting()
        {
            Form_Loaded_Flag = false;
            System.Drawing.Font fnt = Etc.Font_fromStr(XML_Main.cnf.Display.Digtal[Setting_Dno].Font);
            System.Drawing.FontFamily FontFamily = new System.Drawing.FontFamily(fnt.FontFamily.Name);
            TextBox_FamilyName.Text = FontFamily.Name;

            ComboBox_Format.Text = XML_Main.cnf.Display.Digtal[Setting_Dno].Format;
            ComboBox_Format.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Format);

            System.Drawing.Color Dcolor = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].Outline_Color);
            System.Windows.Media.Color Mcolor = Etc.ToMediaColor(Dcolor);
            ColorPicker_Outline.SelectedColor = Mcolor;

            Slider_Edge.Value = XML_Main.cnf.Display.Digtal[Setting_Dno].Outline_Thick;
            TextBlock_Edge.Text = Slider_Edge.Value.ToString();

            ComboBox_Style.Text = XML_Main.cnf.Display.Digtal[Setting_Dno].Paint_Style;
            ComboBox_Style.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Style);
            Style_Changed();

            Slider_Width.Value = XML_Main.cnf.Display.Digtal[Setting_Dno].PosX;
            TextBlock_Width.Text = Slider_Width.Value.ToString();
            Slider_Height.Value = XML_Main.cnf.Display.Digtal[Setting_Dno].PosY;
            TextBlock_Height.Text = Slider_Height.Value.ToString();

            Slider_Opacity.Value = XML_Main.cnf.Display.Digtal[Setting_Dno].Opacity * 100;
            TextBlock_Opacity.Text = Slider_Opacity.Value.ToString("F0");

            Slider_Zoom.Value = XML_Main.cnf.Display.Digtal[Setting_Dno].Zoom;
            TextBlock_Zoom.Text = Slider_Zoom.Value.ToString("P0");

            ToggleSwitch_TopMost.IsOn = XML_Main.cnf.Display.Digtal[Setting_Dno].TopMost;
            Form_Loaded_Flag = true;
        }


        private void Form_Closed(object sender, EventArgs e)
        {
            if (Edit_Cancel)
            {
                // バックアップしておいた設定値を復元する
                XML_Main.cnf.Display.Digtal[Setting_Dno] = Etc.JsonSerializerClone(Setting_Backup);
                if (Setting_Dno == 0)
                {
                    App.form_digital1.Location = new System.Drawing.Point(XML_Main.cnf.Display.Digtal[Setting_Dno].PosX, XML_Main.cnf.Display.Digtal[Setting_Dno].PosY);
                    App.form_digital1.TopMost = XML_Main.cnf.Display.Digtal[Setting_Dno].TopMost;
                    App.form_digital1.Opacity = XML_Main.cnf.Display.Digtal[Setting_Dno].Opacity;
                }
                else
                {
                    App.form_digital2.Location = new System.Drawing.Point(XML_Main.cnf.Display.Digtal[Setting_Dno].PosX, XML_Main.cnf.Display.Digtal[Setting_Dno].PosY);
                    App.form_digital2.TopMost = XML_Main.cnf.Display.Digtal[Setting_Dno].TopMost;
                    App.form_digital2.Opacity = XML_Main.cnf.Display.Digtal[Setting_Dno].Opacity;
                }
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        // 色塗種類の項目が変更された場合に更新
        private void Style_Changed()
        {
            string Item = XML_Main.cnf.Display.Digtal[Setting_Dno].Paint_Style;
            if (Item != null)
            {
                OneColor_Grid.Visibility = Visibility.Hidden;   // 非表示にする(コンポーネントの場所はそのまま)
                HbBrush_Grid.Visibility = Visibility.Hidden;    // 非表示にする(コンポーネントの場所はそのまま)
                LgBrush_Grid.Visibility = Visibility.Hidden;    // 非表示にする(コンポーネントの場所はそのまま)

                switch (Item)
                {
                    case "HatchBrush":
                        System.Drawing.Color Dcolor;
                        System.Windows.Media.Color Mcolor;

                        Dcolor = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].HB_Color1);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        ColorPicker_HbBrush1.SelectedColor = Mcolor;
                        Dcolor = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].HB_Color2);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        ColorPicker_HbBrush2.SelectedColor = Mcolor;

                        //描画先とするImageオブジェクトを作成する
                        Bitmap bitmap = new Bitmap(120, 50);        // コントロールのサイズに合わせる
                        Image_HbBrush.Source = Brush_GrdDraw(Item, bitmap);

                        OneColor_Grid.Visibility = Visibility.Hidden;
                        HbBrush_Grid.Visibility = Visibility.Visible;         // 表示する
                        LgBrush_Grid.Visibility = Visibility.Hidden;
                        break;
                    case "LinearGradientBrush":
                        Dcolor = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].LGB_Color1);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        ColorPicker_LgBrush1.SelectedColor = Mcolor;
                        Dcolor = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].LGB_Color2);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        ColorPicker_LgBrush2.SelectedColor = Mcolor;

                        //描画先とするImageオブジェクトを作成する
                        bitmap = new Bitmap(120, 50);               // コントロールのサイズに合わせる
                        Image_LgBrush.Source = Brush_GrdDraw(Item, bitmap);

                        OneColor_Grid.Visibility = Visibility.Hidden;
                        HbBrush_Grid.Visibility = Visibility.Hidden;
                        LgBrush_Grid.Visibility = Visibility.Visible;         // 表示する
                        break;
                    default:
                        Dcolor = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].Font_Color);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        ColorPicker_OneColor.SelectedColor = Mcolor;

                        OneColor_Grid.Visibility = Visibility.Visible;         // 表示する
                        HbBrush_Grid.Visibility = Visibility.Hidden;
                        LgBrush_Grid.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }

        private ImageSource Brush_GrdDraw(string brush, Bitmap bitmap)
        {
            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics graphics = Graphics.FromImage(bitmap);

            HatchStyle style = 0;
            LinearGradientMode mode = 0;

            System.Drawing.Color HB_Color1 = System.Drawing.Color.White;
            System.Drawing.Color HB_Color2 = System.Drawing.Color.Black;
            System.Drawing.Color LGB_Color1 = System.Drawing.Color.White;
            System.Drawing.Color LGB_Color2 = System.Drawing.Color.Black;

            style = (HatchStyle)XML_Main.cnf.Display.Digtal[Setting_Dno].HB_Style;
            HB_Color1 = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].HB_Color1);
            HB_Color2 = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].HB_Color2);
            mode = (LinearGradientMode)XML_Main.cnf.Display.Digtal[Setting_Dno].LGB_Mode;
            LGB_Color1 = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].LGB_Color1);
            LGB_Color2 = Etc.Color_Code(XML_Main.cnf.Display.Digtal[Setting_Dno].LGB_Color2);

            switch (brush)
            {
                case "HatchBrush":
                    HatchBrush hb = new HatchBrush(style, HB_Color1, HB_Color2);
                    //四角を描く
                    graphics.FillRectangle(hb, graphics.VisibleClipBounds);
                    //リソースを解放する
                    hb.Dispose();
                    graphics.Dispose();
                    break;
                case "LinearGradientBrush":
                    System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(
                    graphics.VisibleClipBounds, LGB_Color1, LGB_Color2, mode);
                    //四角を描く
                    graphics.FillRectangle(gb, graphics.VisibleClipBounds);
                    //リソースを解放する
                    gb.Dispose();
                    graphics.Dispose();
                    break;
            }
            return Etc.ToImageSource(bitmap);
        }

        private void Button_Font_Select_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Font font = System.Drawing.SystemFonts.DefaultFont;
            font = Etc.Font_fromStr(XML_Main.cnf.Display.Digtal[Setting_Dno].Font);

            var dlg = new System.Windows.Forms.FontDialog();
            dlg.Font = font;
            dlg.MaxSize = 1638;
            dlg.FontMustExist = true;       // エラーメッセージを表示
            dlg.ShowColor = false;          // 色を選択出来ないようにする
            dlg.ShowEffects = true;         // 取り消し線、下線、テキストの色などのオプションを選択出来ないようにする

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XML_Main.cnf.Display.Digtal[Setting_Dno].Font = Etc.Font_fromFnt(dlg.Font);
                System.Drawing.FontFamily FontFamily = new System.Drawing.FontFamily(dlg.Font.FontFamily.Name);
                TextBox_FamilyName.Text = FontFamily.Name;
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void Slider_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Opacity.Text = Slider_Opacity.Value.ToString("F0");
                XML_Main.cnf.Display.Digtal[Setting_Dno].Opacity = (double)Slider_Opacity.Value / 100;

                if (Setting_Dno == 0)
                {
                    App.form_digital1.Opacity = XML_Main.cnf.Display.Digtal[Setting_Dno].Opacity;
                }
                else
                {
                    App.form_digital2.Opacity = XML_Main.cnf.Display.Digtal[Setting_Dno].Opacity;
                }
            }
        }

        private void Slider_Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Zoom.Text = Slider_Zoom.Value.ToString("P0");
                XML_Main.cnf.Display.Digtal[Setting_Dno].Zoom = Slider_Zoom.Value;
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ComboBox_Format_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                XML_Main.cnf.Display.Digtal[Setting_Dno].Format = ComboBox_Format.SelectedItem.ToString();
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void Slider_Edge_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Edge.Text = Slider_Edge.Value.ToString();
                XML_Main.cnf.Display.Digtal[Setting_Dno].Outline_Thick = (int)Slider_Edge.Value;
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ComboBox_Style_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                XML_Main.cnf.Display.Digtal[Setting_Dno].Paint_Style = ComboBox_Style.SelectedItem.ToString();
                Style_Changed();
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ComboBox_HbBrush_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                XML_Main.cnf.Display.Digtal[Setting_Dno].HB_Style = ComboBox_HbBrush.SelectedIndex;
                Style_Changed();
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ColorPicker_HbBrush1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_HbBrush1.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Display.Digtal[Setting_Dno].HB_Color1 = Etc.RGB_Code(Dcolor);
                Style_Changed();
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ColorPicker_HbBrush2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_HbBrush2.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Display.Digtal[Setting_Dno].HB_Color2 = Etc.RGB_Code(Dcolor);
                Style_Changed();
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ComboBox_LgBrush_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                XML_Main.cnf.Display.Digtal[Setting_Dno].LGB_Mode = ComboBox_LgBrush.SelectedIndex;
                Style_Changed();
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ColorPicker_LgBrush1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_LgBrush1.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Display.Digtal[Setting_Dno].LGB_Color1 = Etc.RGB_Code(Dcolor);
                Style_Changed();
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ColorPicker_LgBrush2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_LgBrush2.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Display.Digtal[Setting_Dno].LGB_Color2 = Etc.RGB_Code(Dcolor);
                Style_Changed();
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ColorPicker_Outline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_Outline.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Display.Digtal[Setting_Dno].Outline_Color = Etc.RGB_Code(Dcolor);
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void ColorPicker_OneColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_OneColor.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Display.Digtal[Setting_Dno].Font_Color = Etc.RGB_Code(Dcolor);
                Style_Changed();
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }

        private void Slider_Height_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Height.Text = Slider_Height.Value.ToString();
                XML_Main.cnf.Display.Digtal[Setting_Dno].PosY = (int)Slider_Height.Value;
                if (Setting_Dno == 0)
                {
                    App.form_digital1.Location = new System.Drawing.Point(XML_Main.cnf.Display.Digtal[Setting_Dno].PosX, XML_Main.cnf.Display.Digtal[Setting_Dno].PosY);
                }
                else
                {
                    App.form_digital2.Location = new System.Drawing.Point(XML_Main.cnf.Display.Digtal[Setting_Dno].PosX, XML_Main.cnf.Display.Digtal[Setting_Dno].PosY);
                }
            }
        }

        private void Slider_Width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Width.Text = Slider_Width.Value.ToString();
                XML_Main.cnf.Display.Digtal[Setting_Dno].PosX = (int)Slider_Width.Value;
                if (Setting_Dno == 0)
                {
                    App.form_digital1.Location = new System.Drawing.Point(XML_Main.cnf.Display.Digtal[Setting_Dno].PosX, XML_Main.cnf.Display.Digtal[Setting_Dno].PosY);
                }
                else
                {
                    App.form_digital2.Location = new System.Drawing.Point(XML_Main.cnf.Display.Digtal[Setting_Dno].PosX, XML_Main.cnf.Display.Digtal[Setting_Dno].PosY);
                }
            }
        }

        private void ToggleSwitch_TopMost_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                XML_Main.cnf.Display.Digtal[Setting_Dno].TopMost = ToggleSwitch_TopMost.IsOn;
                if (Setting_Dno == 0)
                {
                    App.form_digital1.TopMost = XML_Main.cnf.Display.Digtal[Setting_Dno].TopMost;
                }
                else
                {
                    App.form_digital2.TopMost = XML_Main.cnf.Display.Digtal[Setting_Dno].TopMost;
                }
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Edit_Cancel = true;
            this.Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            Edit_Cancel = false;
            this.Close();
        }

        private void Button_Init_Click(object sender, RoutedEventArgs e)
        {
            if (FormCtrl_Wpf.YesNo_Message("設定を初期化します。よろしいですか？"))
            {
                XML_Main.cnf.Display.Digtal[Setting_Dno] = XML_Main.XML_Display_Digital_Initialize(Setting_Dno);
                Read_Setting();
                if (Setting_Dno == 0)
                {
                    App.form_digital1.Location = new System.Drawing.Point(XML_Main.cnf.Display.Digtal[Setting_Dno].PosX, XML_Main.cnf.Display.Digtal[Setting_Dno].PosY);
                    App.form_digital1.TopMost = XML_Main.cnf.Display.Digtal[Setting_Dno].TopMost;
                    App.form_digital1.Opacity = XML_Main.cnf.Display.Digtal[Setting_Dno].Opacity;
                }
                else
                {
                    App.form_digital2.Location = new System.Drawing.Point(XML_Main.cnf.Display.Digtal[Setting_Dno].PosX, XML_Main.cnf.Display.Digtal[Setting_Dno].PosY);
                    App.form_digital2.TopMost = XML_Main.cnf.Display.Digtal[Setting_Dno].TopMost;
                    App.form_digital2.Opacity = XML_Main.cnf.Display.Digtal[Setting_Dno].Opacity;
                }
                Digital_Drow.Digital_Disp_ReDrow(Setting_Dno);          // デジタル表示の再描画
            }
        }
    }
}
