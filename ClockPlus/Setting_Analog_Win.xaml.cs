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
    /// Setting_Analog.xaml の相互作用ロジック
    /// </summary>
    public partial class Setting_Analog_Win : MetroWindow
    {
        private bool FormLoad_Flag = false;
        public Analog _Analog = new Analog();

        public Setting_Analog_Win()
        {
            InitializeComponent();
            Height_Slider.Maximum = Etc.Disp_Height;
            Width_Slider.Maximum = Etc.Disp_Width;

            // 外観(Skin)のリストを取得
            Get_Skin_List();
        }

        private void Setting_Analog_Window_Loaded(object sender, RoutedEventArgs e)
        {
            // キャンセルされた時のために、設定値をバックアップしておく
            _Analog = Etc.JsonSerializerClone(XML_Main_IO.Main.Display.Analog);

            Get_Setting_Data();             // 設定データの取り込み
            FormLoad_Flag = true;
        }

        // 設定データの取り込み
        private void Get_Setting_Data()
        {
            // 外観
            Skin_ComboBox.Text = XML_Main_IO.Main.Display.Analog.Skin;
            Skin_ComboBox.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(Skin_ComboBox);

            // 不透明度
            Opacity_Slider.Value = XML_Main_IO.Main.Display.Analog.Opacity * 100;
            Opacity_Label.Content = Opacity_Slider.Value.ToString() + "%";

            // 位置
            Height_Slider.Value = XML_Main_IO.Main.Display.Analog.PosY;
            Height_Label.Content = Height_Slider.Value.ToString();
            Width_Slider.Value = XML_Main_IO.Main.Display.Analog.PosX;
            Width_Label.Content = Width_Slider.Value.ToString();
            TopMost_CheckBox.IsChecked = XML_Main_IO.Main.Display.Analog.TopMost;

            // 倍率
            Size_Slider.Value = XML_Main_IO.Main.Display.Analog.Zoom;
            Size_Label.Content = Size_Slider.Value.ToString("P0");
        }

        // 外観(Skin)のリストを取得
        private void Get_Skin_List()
        {
            //スキンフォルダ内にサブフォルダがあるか調べる
            if (0 < Directory.GetDirectories(XML_Main_IO.Skin_Dir).Length)
            {
                string[] dirs = Directory.GetDirectories(XML_Main_IO.Skin_Dir);
                foreach (string s in dirs)
                {
                    string dir = System.IO.Path.GetFileName(s);
                    Skin_ComboBox.Items.Add(dir);
                }
            }
        }

        private void Position_Ini_Button_Click(object sender, RoutedEventArgs e)
        {
            XML_Main_IO.Main.Display.Analog.PosX = Etc.Disp_Width / 2;
            XML_Main_IO.Main.Display.Analog.PosY = Etc.Disp_Height / 2;
            XML_Main_IO.Main.Display.Analog.TopMost = false;

            Height_Slider.Value = XML_Main_IO.Main.Display.Analog.PosY;
            Height_Label.Content = Height_Slider.Value.ToString();
            Width_Slider.Value = XML_Main_IO.Main.Display.Analog.PosX;
            Width_Label.Content = Width_Slider.Value.ToString();
            TopMost_CheckBox.IsChecked = XML_Main_IO.Main.Display.Analog.TopMost;

            App._Analog_Form.Location = new System.Drawing.Point(XML_Main_IO.Main.Display.Analog.PosX, XML_Main_IO.Main.Display.Analog.PosY);
            App._Analog_Form.TopMost = XML_Main_IO.Main.Display.Analog.TopMost;
        }

        private void Skin_ComboBox_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                string Item = "";
                Item = Skin_ComboBox.SelectedItem.ToString();
                XML_Main_IO.Main.Display.Analog.Skin = Item;

                /*
                Main_Win._Analog_Form.Size = new System.Drawing.Size(200, 200);
                Main_Win._Analog_Form.Pbox_face.Size = new System.Drawing.Size(10, 10);
                Main_Win._Analog_Form.Pbox_Hour.Size = new System.Drawing.Size(10, 10);
                Main_Win._Analog_Form.Pbox_Min.Size = new System.Drawing.Size(10, 10);
                Main_Win._Analog_Form.Pbox_Sec.Size = new System.Drawing.Size(10, 10);
                */
                App._Analog_Form.Analog_Disp_init();       // アナログ時計を表示
                App._Analog_Form.Analog_Disp_Drow();       // アナログ時計の描画更新
            }
        }

        private void Opacity_Slider_ValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Analog.Opacity = (double)Opacity_Slider.Value / 100;
                Opacity_Label.Content = Opacity_Slider.Value.ToString() + "%";
                App._Analog_Form.Opacity = XML_Main_IO.Main.Display.Analog.Opacity;
            }
        }

        private void Width_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Analog.PosX = (int)Width_Slider.Value;
                Width_Label.Content = Width_Slider.Value.ToString();
                App._Analog_Form.Location = new System.Drawing.Point(XML_Main_IO.Main.Display.Analog.PosX, XML_Main_IO.Main.Display.Analog.PosY);
            }
        }

        private void Height_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Analog.PosY = (int)Height_Slider.Value;
                Height_Label.Content = Height_Slider.Value.ToString();
                App._Analog_Form.Location = new System.Drawing.Point(XML_Main_IO.Main.Display.Analog.PosX, XML_Main_IO.Main.Display.Analog.PosY);
            }
        }

        private void TopMost_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Analog.TopMost = true;
                App._Analog_Form.TopMost = true;
            }
        }

        private void TopMost_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Analog.TopMost = false;
                App._Analog_Form.TopMost = false;
            }
        }

        private void Size_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag == true)
            {
                Size_Label.Content = Size_Slider.Value.ToString("P0");
                XML_Main_IO.Main.Display.Analog.Zoom = Size_Slider.Value;

                Analog_Form.Analog_Disp_Size(App._Analog_Form);    // アナログ時計の倍率設定
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            // バックアップしておいた設定を読み込む
            XML_Main_IO.Main.Display.Analog = _Analog;

            App._Analog_Form.Opacity = XML_Main_IO.Main.Display.Analog.Opacity;
            App._Analog_Form.Location = new System.Drawing.Point(XML_Main_IO.Main.Display.Analog.PosX, XML_Main_IO.Main.Display.Analog.PosY);

            App._Analog_Form.Analog_Disp_init();       // アナログ時計を表示
            App._Analog_Form.Analog_Disp_Drow();       // アナログ時計の描画更新
            this.Close();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
