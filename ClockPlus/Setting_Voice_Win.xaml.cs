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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClockPlus
{
    public partial class Setting_Voice_Win : MetroWindow
    {
        private bool FormLoad_Flag = false;
//        private Voice _voice = new Voice();

        public Setting_Voice_Win()
        {
            InitializeComponent();

        }

        private void Setting_Voice_Window_Loaded(object sender, RoutedEventArgs e)
        {
            //            Voice_Ctrl.Voice_List_Get(Voice_ComboBox);
            Get_Setting_Data();             // 設定データの取り込み
            FormLoad_Flag = true;
        }

        // 設定データの取り込み
        private void Get_Setting_Data()
        {
            Vol_Slider.Value = XML_Main_IO.Main.Voice.Volume;
            Vol_Label.Content = Vol_Slider.Value.ToString();
        }

        private void Setting_Voice_Window_Closed(object sender, EventArgs e)
        {
            Voice_Ctrl.Voice_Stop();
        }

        private void Voice_ComboBox_Selection_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag)
            {

            }
        }

        private void Vol_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag)
            {
                Vol_Label.Content = Vol_Slider.Value.ToString();
            }              
        }

        /*
        private void Speed_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag)
            {
                Speed_Label.Content = Speed_Slider.Value.ToString();
            }           
        }
        */

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            string Text = dt.ToString("H時m分 をお知らせします");

            //            string Voice = Voice_ComboBox.SelectedItem.ToString();
            string Voice = "";
            int Vol = (int)Vol_Slider.Value;
            int Rate = XML_Main_IO.Main.Voice.Rate;
            Voice_Ctrl.Voice_Play(Text, Voice, Vol, Rate);
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            Voice_Ctrl.Voice_Stop();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            XML_Main_IO.Main.Voice.Volume = (int)Vol_Slider.Value;
            this.Close();
        }
    }
}
