using MahApps.Metro.Controls;
using NAudio.Wave;
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
    /// Form_Setting_Signal.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Setting_Signal : MetroWindow
    {
        private bool Form_Loaded_Flag = false;

        private WaveOutEvent outputDevice = new WaveOutEvent();
        private AudioFileReader afr;
        private bool Sound_Play_Flag = false;

        public Form_Setting_Signal()
        {
            InitializeComponent();
            List<string> Device_List = Sound_Ctrl.Get_Device_List();
            foreach (string Device in Device_List)
            {
                ComboBox_Device.Items.Add(Device);
            }
            ComboBox_Device.SelectedIndex = 0;

            ComboBox_Cycle.Items.Add("1");
            ComboBox_Cycle.Items.Add("30");
            ComboBox_Cycle.Items.Add("60");
            ComboBox_Cycle.SelectedIndex = 0;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox_Device.SelectedIndex = XML_Main.cnf.Signal.Device;
            ComboBox_Cycle.Text = XML_Main.cnf.Signal.Cycle.ToString();
            ComboBox_Cycle.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Cycle);

            Slider_Volume.Value = XML_Main.cnf.Signal.Volume;
            TextBlock_Volume.Text = Slider_Volume.Value.ToString();

            Form_Loaded_Flag = true;
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                afr.Close();
                outputDevice.Dispose();
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            XML_Main.cnf.Signal.Device = ComboBox_Device.SelectedIndex;
            XML_Main.cnf.Signal.Cycle = int.Parse(ComboBox_Cycle.Text);
            XML_Main.cnf.Signal.Volume = (int)Slider_Volume.Value;
            this.Close();
        }

        private void ComboBox_Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_Cycle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Slider_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Volume.Text = Slider_Volume.Value.ToString();
            }
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            string FileName = XML_Main.cnf.Signal.FileName;
            int Vol = (int)Slider_Volume.Value;
            int DeviceNo = ComboBox_Device.SelectedIndex;

            if (Sound_Play_Flag)
            {
                Sound_Stop();
            }
            else
            {
                Sound_Play(FileName, DeviceNo, Vol);
            }
        }

        private void Visibility_Change(bool Visibility)
        {
            if (Visibility)
            {
                ComboBox_Device.IsEnabled = true;
                ComboBox_Cycle.IsEnabled = true;
                Slider_Volume.IsEnabled = true;
            }
            else
            {
                ComboBox_Device.IsEnabled = false;
                ComboBox_Cycle.IsEnabled = false;
                Slider_Volume.IsEnabled = false;
            }
        }

        private void Sound_Stop()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                afr.Close();
                outputDevice.Dispose();
            }
            Sound_Play_Flag = false;
            Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Play.ico"));
            Visibility_Change(true);
        }

        private void Sound_Play(string FileName,int DeviceNo,int Vol)
        {
            try
            {
                outputDevice = new WaveOutEvent();
                afr = new AudioFileReader(FileName);
                outputDevice.DeviceNumber = DeviceNo;
                outputDevice.Init(afr);
                outputDevice.Volume = (float)(Vol / 100f);
                outputDevice.Play();
                outputDevice.PlaybackStopped += Playback_Complete;
                Sound_Play_Flag = true;
                Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Stop.ico"));
                Visibility_Change(false);
            }
            catch
            {
                Visibility_Change(true);
                Sound_Play_Flag = false;
            }
        }

        // 再生完了のイベント処理
        private void Playback_Complete(object sender, EventArgs e)
        {
            outputDevice.Dispose();
            afr.Dispose();

            Sound_Play_Flag = false;
            Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Play.ico"));
            Visibility_Change(true);
        }
    }
}
