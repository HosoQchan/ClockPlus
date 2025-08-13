using MahApps.Metro.Controls;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClockPlus
{
    /// <summary>
    /// Form_Edit_Sound.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Edit_Sound : MetroWindow
    {
        public Task_Sound task_sound = new Task_Sound();

        private bool Form_Loaded_Flag = false;

        private WaveOutEvent outputDevice = new WaveOutEvent();
        private AudioFileReader afr;

        private bool Sound_Play_Flag = false;
        private System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();

        public Form_Edit_Sound()
        {
            InitializeComponent();
            ComboBox_Cycle.ToolTip = "[0:繰り返し無し]\r\n[1:以上]の場合、音声は再生されません！";

            List<string> Device_List = Task_Ctrl.Get_Device_List();
            foreach (string Device in Device_List)
            {
                ComboBox_Device.Items.Add(Device);
            }
            ComboBox_Device.SelectedIndex = 0;

            for (int i = 0; i < 60; i++)
            {
                ComboBox_Cycle.Items.Add(i.ToString());
            }
            ComboBox_Cycle.SelectedIndex = 0;
            ToggleSwich_Password.IsOn = false;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox_Device.SelectedIndex = task_sound.Device;
            TextBox_FileName.Text = task_sound.FileName;
            
            ComboBox_Cycle.Text = task_sound.Repeat.ToString();
            ComboBox_Cycle.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Cycle);

            ToggleSwich_Password.IsOn = task_sound.Password_Enable;

            Slider_Volume.Value = task_sound.Volume;
            TextBlock_Volume.Text = Slider_Volume.Value.ToString();

            Form_Loaded_Flag = true;
        }

        private void Button_FileSelect_Click(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                string InitialDir = XML_Main.Sound_Dir + "\\";
                string InitialExt = ".WAV";
                string Filter = "WAVファイル(*.WAV)|*.WAV|MP3ファイル(*.MP3)|*.MP3|全てのファイル(*.*)|*.*";
                string FileName = Etc.Dlg_File_Select(InitialDir, Filter, InitialExt);

                if (FileName != "")
                {
                    TextBox_FileName.Text = FileName;
                }
            }
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            timer1.Stop();
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
            task_sound.Device = ComboBox_Device.SelectedIndex;
            task_sound.FileName = TextBox_FileName.Text;
            task_sound.Repeat = int.Parse(ComboBox_Cycle.Text);
            task_sound.Password_Enable = ToggleSwich_Password.IsOn;
            task_sound.Volume = (int)Slider_Volume.Value;
            this.Close();
        }

        private void ComboBox_Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_Cycle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ToggleSwich_Password_Toggled(object sender, RoutedEventArgs e)
        {

        }

        private void Slider_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Volume.Text = Slider_Volume.Value.ToString();


            }
        }

        private void Visibility_Change(bool Visibility)
        {
            if (Visibility)
            {
                ComboBox_Device.IsEnabled = true;
                Button_FileSelect.IsEnabled = true;
                ComboBox_Cycle.IsEnabled = true;
                ToggleSwich_Password.IsEnabled = true;
                Slider_Volume.IsEnabled = true;
            }
            else
            {
                ComboBox_Device.IsEnabled = false;
                Button_FileSelect.IsEnabled = false;
                ComboBox_Cycle.IsEnabled = false;
                ToggleSwich_Password.IsEnabled = false;
                Slider_Volume.IsEnabled = false;
            }
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            string FileName = TextBox_FileName.Text;
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

        private void Sound_Stop()
        {
            timer1.Stop();
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
        private void Sound_Play(string FileName, int DeviceNo, int Vol)
        {
            try
            {
                Sound_Stop();
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

                // 繰り返し？
                if (ComboBox_Cycle.Text != "0")
                {
                    int cycle = int.Parse(ComboBox_Cycle.Text);
                    cycle = cycle * 1000;
                    timer1.Interval = cycle;
                    timer1.Tick += Timer1_Tick;
                    timer1.Start();
                }
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
            if (ComboBox_Cycle.Text == "0")
            {
                outputDevice.Dispose();
                afr.Dispose();

                Sound_Play_Flag = false;
                Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Play.ico"));
                Visibility_Change(true);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            afr.Position = 0;
            outputDevice.Play();
        }
    }
}
