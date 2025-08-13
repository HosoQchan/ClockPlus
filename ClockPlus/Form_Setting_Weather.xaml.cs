using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Form_Weather_Edit.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Setting_Weather : MetroWindow
    {
        private bool Form_Loaded_Flag = false;
        private bool Edit_Cancel = true;

        // 設定対象の日付番号
        private int Setting_No = -1;

        private Weather_Disp Setting_Backup = new Weather_Disp();

        public Form_Setting_Weather(int setting_No)
        {
            Setting_No = setting_No;
            InitializeComponent();

            // ディスプレイの縦横値からスライダーの最大値を決める
            Slider_Height.Maximum = XML_Main.Display_Height;
            Slider_Width.Maximum = XML_Main.Display_Width;

            Title = "天気予報";
            if (Setting_No == 0)
            {
                Title = Title + "(今日)";
            }
            else
            {
                Title = Title + "(明日)";
            }
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            // 設定値をバックアップしておく
            Setting_Backup = Etc.JsonSerializerClone(XML_Main.cnf.Weather.Weather_Disp[Setting_No]);

            Read_Setting();
            Form_Loaded_Flag = true;
        }

        private void Read_Setting()
        {
            Form_Loaded_Flag = false;

            System.Drawing.Color Dcolor = Etc.Color_Code(XML_Main.cnf.Weather.Weather_Disp[Setting_No].Font_Color);
            System.Windows.Media.Color Mcolor = Etc.ToMediaColor(Dcolor);
            ColorPicker_Other.SelectedColor = Mcolor;

            Dcolor = Etc.Color_Code(XML_Main.cnf.Weather.Weather_Disp[Setting_No].Font_Color1);
            Mcolor = Etc.ToMediaColor(Dcolor);
            ColorPicker_TmpHigh.SelectedColor = Mcolor;

            Dcolor = Etc.Color_Code(XML_Main.cnf.Weather.Weather_Disp[Setting_No].Font_Color2);
            Mcolor = Etc.ToMediaColor(Dcolor);
            ColorPicker_TmpLow.SelectedColor = Mcolor;

            ToggleSwitch_BackColor.IsOn = XML_Main.cnf.Weather.Weather_Disp[Setting_No].Clear_Background;
            Dcolor = Etc.Color_Code(XML_Main.cnf.Weather.Weather_Disp[Setting_No].Back_Color);
            Mcolor = Etc.ToMediaColor(Dcolor);
            ColorPicker_BackColor.SelectedColor = Mcolor;

            Slider_Width.Value = XML_Main.cnf.Weather.Weather_Disp[Setting_No].PosX;
            TextBlock_Width.Text = Slider_Width.Value.ToString();
            Slider_Height.Value = XML_Main.cnf.Weather.Weather_Disp[Setting_No].PosY;
            TextBlock_Height.Text = Slider_Height.Value.ToString();

            Slider_Opacity.Value = XML_Main.cnf.Weather.Weather_Disp[Setting_No].Opacity * 100;
            TextBlock_Opacity.Text = Slider_Opacity.Value.ToString("F0");

            Slider_Zoom.Value = XML_Main.cnf.Weather.Weather_Disp[Setting_No].Zoom;
            TextBlock_Zoom.Text = Slider_Zoom.Value.ToString("P0");

            ToggleSwitch_TopMost.IsOn = XML_Main.cnf.Weather.Weather_Disp[Setting_No].TopMost;

            Back_Color_Visibility();
            Form_Loaded_Flag = true;
        }

        private void Back_Color_Visibility()
        {

            if (ToggleSwitch_BackColor.IsOn)
            {
                Grid_BackColor.Visibility = Visibility.Hidden;      // 非表示にする(コンポーネントの場所はそのまま)
            }
            else
            {
                Grid_BackColor.Visibility = Visibility.Visible;     // 表示する
            }
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            if (Edit_Cancel)
            {
                // バックアップしておいた設定値を復元する
                XML_Main.cnf.Weather.Weather_Disp[Setting_No] = Etc.JsonSerializerClone(Setting_Backup);

                // 位置・不透明度・最前面・拡大縮小の変更
                Setting_Change();

                if (Setting_No == 0)
                {
                    // 今日の天気予報を描画する
                    App.form_weather1.Weather_Disp1_Drow();
                }
                else
                {
                    // 明日の天気予報を描画する
                    App.form_weather2.Weather_Disp2_Drow();
                }
            }
        }

        // 位置・不透明度・最前面・拡大縮小の変更
        private void Setting_Change()
        {
            if (Setting_No == 0)
            {
                // 位置・不透明度・最前面の設定
                App.form_weather1.Location = new System.Drawing.Point(XML_Main.cnf.Weather.Weather_Disp[Setting_No].PosX, XML_Main.cnf.Weather.Weather_Disp[Setting_No].PosY);
                App.form_weather1.TopMost = XML_Main.cnf.Weather.Weather_Disp[Setting_No].TopMost;
                App.form_weather1.Opacity = XML_Main.cnf.Weather.Weather_Disp[Setting_No].Opacity;

                // 倍率設定
                Form_Weather1.Weather_Disp1_Size(App.form_weather1);
            }
            else
            {
                // 位置・不透明度・最前面の設定
                App.form_weather2.Location = new System.Drawing.Point(XML_Main.cnf.Weather.Weather_Disp[Setting_No].PosX, XML_Main.cnf.Weather.Weather_Disp[Setting_No].PosY);
                App.form_weather2.TopMost = XML_Main.cnf.Weather.Weather_Disp[Setting_No].TopMost;
                App.form_weather2.Opacity = XML_Main.cnf.Weather.Weather_Disp[Setting_No].Opacity;

                // 倍率設定
                Form_Weather2.Weather_Disp2_Size(App.form_weather2);
            }
        }

        private void Button_Init_Click(object sender, RoutedEventArgs e)
        {
            if (FormCtrl_Wpf.YesNo_Message("設定を初期化します。よろしいですか？"))
            {
                XML_Main.cnf.Weather.Weather_Disp[Setting_No] = XML_Main.XML_Display_Weather_Initialize();

                // 位置・不透明度・最前面・拡大縮小の変更
                Setting_Change();

                if (Setting_No == 0)
                {
                    // 今日の天気予報を描画する
                    App.form_weather1.Weather_Disp1_Drow();
                }
                else
                {
                    // 明日の天気予報を描画する
                    App.form_weather2.Weather_Disp2_Drow();
                }
                Read_Setting();
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

        private void ColorPicker_TmpHigh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_TmpHigh.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].Font_Color1 = Etc.RGB_Code(Dcolor);

                if (Setting_No == 0)
                {
                    // 今日の天気予報を描画する
                    App.form_weather1.Weather_Disp1_Drow();
                }
                else
                {
                    // 明日の天気予報を描画する
                    App.form_weather2.Weather_Disp2_Drow();
                }
            }
        }

        private void ColorPicker_TmpLow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_TmpLow.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].Font_Color2 = Etc.RGB_Code(Dcolor);

                if (Setting_No == 0)
                {
                    // 今日の天気予報を描画する
                    App.form_weather1.Weather_Disp1_Drow();
                }
                else
                {
                    // 明日の天気予報を描画する
                    App.form_weather2.Weather_Disp2_Drow();
                }
            }
        }

        private void ColorPicker_Other_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_Other.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].Font_Color = Etc.RGB_Code(Dcolor);

                if (Setting_No == 0)
                {
                    // 今日の天気予報を描画する
                    App.form_weather1.Weather_Disp1_Drow();
                }
                else
                {
                    // 明日の天気予報を描画する
                    App.form_weather2.Weather_Disp2_Drow();
                }
            }
        }

        private void Slider_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Opacity.Text = Slider_Opacity.Value.ToString("F0");
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].Opacity = (double)Slider_Opacity.Value / 100;

                // 位置・不透明度・最前面・拡大縮小の変更
                Setting_Change();
            }
        }

        private void Slider_Height_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Height.Text = Slider_Height.Value.ToString();
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].PosY = (int)Slider_Height.Value;

                // 位置・不透明度・最前面・拡大縮小の変更
                Setting_Change();
            }
        }

        private void Slider_Width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Width.Text = Slider_Width.Value.ToString();
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].PosX = (int)Slider_Width.Value;

                // 位置・不透明度・最前面・拡大縮小の変更
                Setting_Change();
            }
        }

        private void ToggleSwitch_TopMost_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].TopMost = ToggleSwitch_TopMost.IsOn;

                // 位置・不透明度・最前面・拡大縮小の変更
                Setting_Change();
            }
        }

        private void Slider_Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Zoom.Text = Slider_Zoom.Value.ToString("P0");
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].Zoom = Slider_Zoom.Value;

                // 位置・不透明度・最前面・拡大縮小の変更
                Setting_Change();
            }
        }

        private void ToggleSwitch_BackColor_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].Clear_Background = ToggleSwitch_BackColor.IsOn;
                Back_Color_Visibility();

                if (Setting_No == 0)
                {
                    // 今日の天気予報を描画する
                    App.form_weather1.Weather_Disp1_Drow();
                }
                else
                {
                    // 明日の天気予報を描画する
                    App.form_weather2.Weather_Disp2_Drow();
                }
            }
        }

        private void ColorPicker_BackColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                System.Windows.Media.Color Mcolor = ColorPicker_BackColor.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main.cnf.Weather.Weather_Disp[Setting_No].Back_Color = Etc.RGB_Code(Dcolor);

                if (Setting_No == 0)
                {
                    // 今日の天気予報を描画する
                    App.form_weather1.Weather_Disp1_Drow();
                }
                else
                {
                    // 明日の天気予報を描画する
                    App.form_weather2.Weather_Disp2_Drow();
                }
            }
        }
    }
}
