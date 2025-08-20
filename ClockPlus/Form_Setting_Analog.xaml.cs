using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClockPlus
{
    /// <summary>
    /// Form_Setting_Analog.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Setting_Analog : MetroWindow
    {
        private Analog Setting_Backup = new Analog();
        private bool Edit_Cancel = true;
        private bool Form_Loaded_Flag = false;

        public Form_Setting_Analog()
        {
            InitializeComponent();
            // ディスプレイの縦横値からスライダーの最大値を決める
            Slider_Height.Maximum = XML_Main.Display_Height;
            Slider_Width.Maximum = XML_Main.Display_Width;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            // 設定値をバックアップしておく
            Setting_Backup = Etc.JsonSerializerClone(XML_Main.cnf.Display.Analog);

            Get_Skin_List();
            if (ComboBox_Skin.Items.Count == 0)
            {
                FormCtrl_Wpf.Error_Message("スキンが存在しません。");
                this.Close();
            }
            Read_Setting();
            Form_Loaded_Flag = true;
        }
        // 外観スキンのComboBoxリストを作成する
        private void Get_Skin_List()
        {
            //スキンフォルダ内にサブフォルダがあるか調べる
            if (0 < Directory.GetDirectories(XML_Main.Skin_Dir).Length)
            {
                string[] dirs = Directory.GetDirectories(XML_Main.Skin_Dir);
                foreach (string s in dirs)
                {
                    string dir = System.IO.Path.GetFileName(s);
                    ComboBox_Skin.Items.Add(dir);
                }
            }
        }

        private void Read_Setting()
        {
            Form_Loaded_Flag = false;
            ComboBox_Skin.Text = XML_Main.cnf.Display.Analog.Skin;
            ComboBox_Skin.SelectedIndex =  FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Skin);

            Slider_Opacity.Value = XML_Main.cnf.Display.Analog.Opacity * 100;
            TextBlock_Opacity.Text = Slider_Opacity.Value.ToString("F0");

            Slider_Width.Value = XML_Main.cnf.Display.Analog.PosX;
            TextBlock_Width.Text = Slider_Width.Value.ToString();
            Slider_Height.Value = XML_Main.cnf.Display.Analog.PosY;
            TextBlock_Height.Text = Slider_Height.Value.ToString();

            Slider_Zoom.Value = XML_Main.cnf.Display.Analog.Zoom;
            TextBlock_Zoom.Text = Slider_Zoom.Value.ToString("P0");

            ToggleSwitch_TopMost.IsOn = XML_Main.cnf.Display.Analog.TopMost;
            Form_Loaded_Flag = true;
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            if (Edit_Cancel)
            {
                // バックアップしておいた設定値を復元する
                XML_Main.cnf.Display.Analog = Etc.JsonSerializerClone(Setting_Backup);

                // アナログ時計の倍率設定
                Form_Analog.Analog_Disp_Size(App.form_analog);

                // 位置・不透明度・最前面の設定
                App.form_analog.Location = new System.Drawing.Point(XML_Main.cnf.Display.Analog.PosX, XML_Main.cnf.Display.Analog.PosY);
                App.form_analog.TopMost = XML_Main.cnf.Display.Analog.TopMost;
                App.form_analog.Opacity = XML_Main.cnf.Display.Analog.Opacity;

                App.form_analog.Analog_Disp_init();       // アナログ時計を表示
                App.form_analog.Analog_Disp_Drow();       // アナログ時計の描画更新
            }
        }

        private void ComboBox_Skin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                XML_Main.cnf.Display.Analog.Skin = ComboBox_Skin.SelectedItem.ToString();
                App.form_analog.Analog_Disp_init();       // アナログ時計を表示
                App.form_analog.Analog_Disp_Drow();       // アナログ時計の描画更新
            }
        }

        private void Slider_Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Zoom.Text = Slider_Zoom.Value.ToString("P0");
                XML_Main.cnf.Display.Analog.Zoom = Slider_Zoom.Value;
                // アナログ時計の倍率設定
                Form_Analog.Analog_Disp_Size(App.form_analog);
            }
        }

        private void Slider_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Opacity.Text = Slider_Opacity.Value.ToString("F0");
                XML_Main.cnf.Display.Analog.Opacity = (double)Slider_Opacity.Value / 100;
                App.form_analog.Opacity = XML_Main.cnf.Display.Analog.Opacity;
            }
        }

        private void Slider_Height_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Height.Text = Slider_Height.Value.ToString();
                XML_Main.cnf.Display.Analog.PosY = (int)Slider_Height.Value;
                App.form_analog.Location = new System.Drawing.Point(XML_Main.cnf.Display.Analog.PosX, XML_Main.cnf.Display.Analog.PosY);
            }
        }

        private void Slider_Width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Width.Text = Slider_Width.Value.ToString();
                XML_Main.cnf.Display.Analog.PosX = (int)Slider_Width.Value;
                App.form_analog.Location = new System.Drawing.Point(XML_Main.cnf.Display.Analog.PosX, XML_Main.cnf.Display.Analog.PosY);
            }
        }

        private void ToggleSwitch_TopMost_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                XML_Main.cnf.Display.Analog.TopMost = ToggleSwitch_TopMost.IsOn;
                App.form_analog.TopMost = XML_Main.cnf.Display.Analog.TopMost;
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
                XML_Main.cnf.Display.Analog = XML_Main.XML_Display_Analog_Initialize();
                Read_Setting();

                // アナログ時計の倍率設定
                Form_Analog.Analog_Disp_Size(App.form_analog);

                // 位置・不透明度・最前面の設定
                App.form_analog.Location = new System.Drawing.Point(XML_Main.cnf.Display.Analog.PosX, XML_Main.cnf.Display.Analog.PosY);
                App.form_analog.TopMost = XML_Main.cnf.Display.Analog.TopMost;
                App.form_analog.Opacity = XML_Main.cnf.Display.Analog.Opacity;

                App.form_analog.Analog_Disp_init();       // アナログ時計を表示
                App.form_analog.Analog_Disp_Drow();       // アナログ時計の描画更新
            }
        }
    }
}
