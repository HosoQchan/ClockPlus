using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class Setting_Signal_Win : MetroWindow
    {
        private bool FormLoad_Flag = false;
//        private Jihou _jihou = new Jihou();

        public Setting_Signal_Win()
        {
            InitializeComponent();
            Cycle_ComboBox.Items.Add("1");
            Cycle_ComboBox.Items.Add("30");
            Cycle_ComboBox.Items.Add("60");
        }
        private void Setting_Signal_Window_Loaded(object sender, RoutedEventArgs e)
        {
            Get_Setting_Data();             // 設定データの取り込み
            FormLoad_Flag = true;
        }

        // 設定データの取り込み
        private void Get_Setting_Data()
        {
            Cycle_ComboBox.Text = XML_Main_IO.Main.Signal.Cycle.ToString();
            Cycle_ComboBox.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(Cycle_ComboBox);

            Vol_Slider.Value = XML_Main_IO.Main.Signal.Volume;
            Vol_Label.Content = Vol_Slider.Value.ToString();
        }

        private void Setting_Signal_Window_Closed(object sender, EventArgs e)
        {
            Sound_Ctrl.Sound_Stop();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            Setting_Save();
            this.Close();
        }

        // 編集した内容を保存する
        private void Setting_Save()
        {
            string Item = Cycle_ComboBox.SelectedItem.ToString();
            int cycle = int.Parse(Item);

            XML_Main_IO.Main.Signal.Cycle = cycle;
            XML_Main_IO.Main.Signal.Volume = (int)Vol_Slider.Value;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Vol_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag)
            {
                Vol_Label.Content = Vol_Slider.Value.ToString();
            }
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            string FileName = XML_Main_IO.Main.Signal.FileName;
            int Vol = (int)Vol_Slider.Value;
            int DeviceNo = XML_Main_IO.Main.Signal.Device;
            Sound_Ctrl.Sound_Play(FileName, Vol, DeviceNo);
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            Sound_Ctrl.Sound_Stop();
        }

        private void Cycle_ComboBox_Selection_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag)
            {

            }
        }
    }
}
