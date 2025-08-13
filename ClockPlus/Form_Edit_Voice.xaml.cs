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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VoicevoxClientSharp.ApiClient.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClockPlus
{
    /// <summary>
    /// Form_Edit_Voice.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Edit_Voice : MetroWindow
    {
        private bool Form_Loaded_Flag = false;

        private WaveOutEvent outputDevice;
        private WaveFileReader wfr;
        private bool Voice_Play_Flag = false;

        public Voice_Setting Voice = new Voice_Setting();

        

        public Form_Edit_Voice()
        {
            InitializeComponent();
            ComboBox_Name_Gen();

            List<string> Device_List = Task_Ctrl.Get_Device_List();
            foreach (string Device in Device_List)
            {
                ComboBox_Device.Items.Add(Device);
            }
            ComboBox_Device.SelectedIndex = 0;
        }

        // キャラクタ一覧を取得する
        private void ComboBox_Name_Gen()
        {
            ComboBox_Name.Items.Clear();
            ComboBox_Name.SelectedIndex = -1;
            if (Voice_Ctrl.speakers != null)
            {
                foreach (Speaker speaker in Voice_Ctrl.speakers)
                {
                    ComboBox_Name.Items.Add(speaker.Name);
                }
                ComboBox_Name.SelectedIndex = 0;
            }
        }

        // スタイル一覧を取得する
        private void ComboBox_type_Gen(string Name)
        {
            ComboBox_Type.Items.Clear();
            ComboBox_Type.SelectedIndex = -1;
            if (Voice_Ctrl.speakers != null)
            {
                foreach (Speaker speaker in Voice_Ctrl.speakers)
                {
                    if (speaker.Name == Name)
                    {
                        foreach (SpeakerStyle style in speaker.Styles)
                        {
                            ComboBox_Type.Items.Add(style.Name);
                        }
                        break;
                    }
                }
                ComboBox_Type.SelectedIndex = 0;
            }
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            Read_Setting();
            Form_Loaded_Flag = true;
        }

        private void Read_Setting()
        {
            Form_Loaded_Flag = false;
            ComboBox_Name.Text = Voice.Style.Name;
            ComboBox_Name.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Name);

            ComboBox_type_Gen(ComboBox_Name.Text);
            ComboBox_Type.Text = Voice.Style.Type;
            ComboBox_Type.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Type);

            ComboBox_Device.SelectedIndex = Voice.Query.Device;

            Slider_Volume.Value = Decimal.ToDouble(Voice.Query.VolumeScale);
            Slider_Speed.Value = Decimal.ToDouble(Voice.Query.SpeedScale);
            Slider_Pitch.Value = Decimal.ToDouble(Voice.Query.PitchScale);
            Slider_PrePhone.Value = Decimal.ToDouble(Voice.Query.PrePhonemeLength);
            Slider_PostPhone.Value = Decimal.ToDouble(Voice.Query.PostPhonemeLength);

            TextBlock_Volume.Text = Slider_Volume.Value.ToString("F2");
            TextBlock_Speed.Text = Slider_Speed.Value.ToString("F2");
            TextBlock_Pitch.Text = Slider_Pitch.Value.ToString("F2");
            TextBlock_PrePhone.Text = Slider_PrePhone.Value.ToString("F2");
            TextBlock_PostPhone.Text = Slider_PostPhone.Value.ToString("F2");
            Form_Loaded_Flag = true;
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                wfr.Close();
                outputDevice.Dispose();
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            Voice.Style.Name = ComboBox_Name.SelectedItem.ToString();
            Voice.Style.Type = ComboBox_Type.SelectedItem.ToString();

            Voice.Query.Device = ComboBox_Device.SelectedIndex;

            Voice.Query.VolumeScale = (decimal)Slider_Volume.Value;
            Voice.Query.SpeedScale = (decimal)Slider_Speed.Value;
            Voice.Query.PitchScale = (decimal)Slider_Pitch.Value;
            Voice.Query.PrePhonemeLength = (decimal)Slider_PrePhone.Value;
            Voice.Query.PostPhonemeLength = (decimal)Slider_PostPhone.Value;
            this.Close();
        }

        private void ComboBox_Name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Form_Loaded_Flag = false;
                ComboBox_type_Gen(ComboBox_Name.SelectedItem.ToString());
                ComboBox_Type.SelectedIndex = 0;
                Form_Loaded_Flag = true;
            }
        }

        private void ComboBox_Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {

            }
        }

        private void ComboBox_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {

            }
        }

        private void Slider_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Speed.Text = Slider_Speed.Value.ToString("F2");
            }
        }

        private void Slider_Pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Pitch.Text = Slider_Pitch.Value.ToString("F2");
            }
        }

        private void Slider_PrePhone_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_PrePhone.Text = Slider_PrePhone.Value.ToString("F2");
            }
        }

        private void Slider_PostPhone_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_PostPhone.Text = Slider_PostPhone.Value.ToString("F2");
            }
        }

        private void Slider_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Form_Loaded_Flag)
            {
                TextBlock_Volume.Text = Slider_Volume.Value.ToString("F2");
            }
        }

        private async void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            if (Voice_Play_Flag)
            {
                Voice_Stop();
            }
            else
            {
                Voice_Play_Flag = true;
                Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Stop.ico"));

                Voice_Setting voice_Setting = new Voice_Setting();
                voice_Setting.Style.Name = ComboBox_Name.SelectedItem.ToString();
                voice_Setting.Style.Type = ComboBox_Type.SelectedItem.ToString();

                voice_Setting.Query.Device = ComboBox_Device.SelectedIndex;

                voice_Setting.Query.VolumeScale = (decimal)Slider_Volume.Value;
                voice_Setting.Query.SpeedScale = (decimal)Slider_Speed.Value;
                voice_Setting.Query.PitchScale = (decimal)Slider_Pitch.Value;
                voice_Setting.Query.PrePhonemeLength = (decimal)Slider_PrePhone.Value;
                voice_Setting.Query.PostPhonemeLength = (decimal)Slider_PostPhone.Value;

                string Text = XML_Message.Msg_Temp["挨拶"];
                Text = Voice_Ctrl.Text_Replace(Text, voice_Setting);

                byte[] wav = await Voice_Ctrl.Voice_Gen(Text, voice_Setting);
                Voice_Play(wav, 100, voice_Setting.Query.Device);

                Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Stop.ico"));
            }
        }

        private void Voice_Stop()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                wfr.Close();
                outputDevice.Dispose();
            }
            Voice_Play_Flag = false;
            Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Play.ico"));
        }

        private void Voice_Play(byte[] wavBytes, int Vol, int DeviceNo)
        {
            try
            {
                Voice_Stop();
                outputDevice = new WaveOutEvent();
                wfr = new WaveFileReader(new MemoryStream(wavBytes));
                outputDevice.DeviceNumber = DeviceNo;
                outputDevice.Init(wfr);
                outputDevice.Volume = (float)(Vol / 100f);
                outputDevice.Play();
                outputDevice.PlaybackStopped += Playback_Complete;
            }
            catch
            {
                Voice_Play_Flag = false;
            }
        }
        // 再生完了のイベント処理
        private void Playback_Complete(object sender, EventArgs e)
        {
            outputDevice.Dispose();
            wfr.Dispose();

            Voice_Play_Flag = false;
            Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Play.ico"));
        }
    }
}
