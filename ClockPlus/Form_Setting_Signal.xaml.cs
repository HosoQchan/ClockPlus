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
using static System.Net.Mime.MediaTypeNames;

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
        private WaveFileReader wfr;
        private bool Sound_Play_Flag = false;
        private bool Voice_Play_Flag = false;

        public Form_Setting_Signal()
        {
            InitializeComponent();
            List<string> Device_List = Task_Ctrl.Get_Device_List();
            foreach (string Device in Device_List)
            {
                ComboBox_Device.Items.Add(Device);
            }
            ComboBox_Device.SelectedIndex = 0;

            ComboBox_Cycle.Items.Add("1");
            ComboBox_Cycle.Items.Add("30");
            ComboBox_Cycle.Items.Add("60");
            ComboBox_Cycle.SelectedIndex = 0;

            ToggleSwich_Voice.IsOn = false;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox_Device.SelectedIndex = XML_Main.cnf.Signal.Device;
            ComboBox_Cycle.Text = XML_Main.cnf.Signal.Cycle.ToString();
            ComboBox_Cycle.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Cycle);

            Slider_Volume.Value = XML_Main.cnf.Signal.Volume;
            TextBlock_Volume.Text = Slider_Volume.Value.ToString();

            ToggleSwich_Voice.IsOn = XML_Main.cnf.Signal.Voice.Enable;
            Voice_Visibility();
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

        private void Voice_Visibility()
        {
            if (Voice_Engine.Process_Step == Voice_Engine.Proc_step.None)
            {
                Grid_Voice.Visibility = Visibility.Hidden;      // 非表示にする(コンポーネントの場所はそのまま)
                TextBlock_Voice.Text = "音声Engineが起動されていません。";
            }
            else
            {
                Grid_Voice.Visibility = Visibility.Visible;     // 表示する
                TextBlock_Voice.Text = "サウンド再生後、時刻を音声で知らせる";
                if (ToggleSwich_Voice.IsOn == true)
                {
                    Button_Voice.Visibility = Visibility.Visible;       // 表示する
                }
                else
                {
                    Button_Voice.Visibility = Visibility.Hidden;        // 非表示にする(コンポーネントの場所はそのまま)
                }
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
            XML_Main.cnf.Signal.Voice.Enable = ToggleSwich_Voice.IsOn;
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

        private void ToggleSwich_Voice_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                if (ToggleSwich_Voice.IsOn)
                {
                    // Voiceの設定が利用可能な状態か
                    if (Voice_Ctrl.Voice_Ready_Check(true))
                    {
                        // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
                        if (Voice_Ctrl.Speaker_Check(XML_Main.cnf.Signal.Voice) == 0)
                        {
                            XML_Main.cnf.Signal.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                            XML_Main.cnf.Signal.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
                        }
                    }
                    else
                    {
                        Form_Loaded_Flag = false;
                        ToggleSwich_Voice.IsOn = false;
                        Form_Loaded_Flag = true;
                    }
                }
                Voice_Visibility();
            }
        }

        private void Button_Voice_Click(object sender, RoutedEventArgs e)
        {
            // Voiceの設定が利用可能な状態か
            if (!Voice_Ctrl.Voice_Ready_Check(true))
            {
                return;
            }

            // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
            if (Voice_Ctrl.Speaker_Check(XML_Main.cnf.Signal.Voice) == 0)
            {
                XML_Main.cnf.Signal.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                XML_Main.cnf.Signal.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
            }

            Form_Edit_Voice Form = new Form_Edit_Voice();
            Form.Voice = XML_Main.cnf.Signal.Voice;
            Form.ShowDialog();
            XML_Main.cnf.Signal.Voice = Form.Voice;
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            if ((ToggleSwich_Voice.IsOn) && (Button_Voice.Visibility == Visibility.Visible))
            {
                // Voiceの設定が利用可能な状態か
                if (!Voice_Ctrl.Voice_Ready_Check(true))
                {
                    return;
                }
            }

            // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
            if (Voice_Ctrl.Speaker_Check(XML_Main.cnf.Signal.Voice) == 0)
            {
                XML_Main.cnf.Signal.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                XML_Main.cnf.Signal.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
            }

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
                ToggleSwich_Voice.IsEnabled = true;
                Button_Voice.IsEnabled = true;
            }
            else
            {
                ComboBox_Device.IsEnabled = false;
                ComboBox_Cycle.IsEnabled = false;
                Slider_Volume.IsEnabled = false;
                ToggleSwich_Voice.IsEnabled = false;
                Button_Voice.IsEnabled = false;
            }
        }

        private void Sound_Stop()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                afr.Close();
                wfr.Close();
                outputDevice.Dispose();
            }
            Sound_Play_Flag = false;
            Voice_Play_Flag = false;
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
                Voice_Play_Flag = false;
            }
        }

        // 再生完了のイベント処理
        private async void Playback_Complete(object sender, EventArgs e)
        {
            outputDevice.Dispose();
            if (afr != null)
            {
                afr.Dispose();
            }
            if (wfr != null)
            {
                wfr.Dispose();
            }
            if ((ToggleSwich_Voice.IsOn) && (!Voice_Play_Flag))
            {
                Voice_Setting Setting = new Voice_Setting();
                Setting = XML_Main.cnf.Signal.Voice;
                string Text = XML_Message.Msg_Temp["時報"];
                Text = Voice_Ctrl.Text_Replace(Text, Setting);

                byte[] wav = await Voice_Ctrl.Voice_Gen(Text, Setting);
                if (wav != null)
                {
                    Voice_Play_Flag = true;
                    outputDevice = new WaveOutEvent();
                    wfr = new WaveFileReader(new MemoryStream(wav));
                    outputDevice.DeviceNumber = Setting.Query.Device;
                    outputDevice.Init(wfr);
                    outputDevice.Volume = (float)(100 / 100f);
                    outputDevice.Play();
                    outputDevice.PlaybackStopped += Playback_Complete;
                    return;
                }
            }
            Sound_Play_Flag = false;
            Voice_Play_Flag = false;
            Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Play.ico"));
            Visibility_Change(true);
        }
    }
}
