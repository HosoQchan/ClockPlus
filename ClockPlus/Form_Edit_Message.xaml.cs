using MahApps.Metro.Controls;
using Microsoft.Win32.TaskScheduler;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ClockPlus
{
    /// <summary>
    /// Form_Edit_Message.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Edit_Message : MetroWindow
    {
        public Task_Message task_message = new Task_Message();
        private bool Form_Loaded_Flag = false;
        public const int Max_Leng = 127;
        public const int Max_count = 16;

        private int List_SelNo = -1;
        private bool Edit_Flag = false;

        private WaveOutEvent outputDevice;
        private WaveFileReader wfr;
        private bool Voice_Play_Flag = false;

        public Voice_Setting Voice = new Voice_Setting();

        public Form_Edit_Message()
        {
            InitializeComponent();
            ToggleSwich_Voice.ToolTip = "サウンド再生が有効な場合は、サウンド再生後に音声が再生されます。\r\n※サウンド再生が「繰り返し」設定されている場合、音声は再生されません！";

            ComboBox_Time.Items.Clear();
            for (int i = 0; i <= 30; i++)
            {
                ComboBox_Time.Items.Add(i.ToString());
            }
            ComboBox_Time.SelectedIndex = 0;
            ComboBox_Time.ToolTip = "[0:無制限]\r\n[1:以上]の場合、タイムアウト後、自動的にウィンドウが閉じられます。";

            ToggleSwich_Voice.IsOn = false;
            Voice_Visibility();
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            Read_Setting();

            Form_Loaded_Flag = true;
        }

        private void Form_Closed(object sender, EventArgs e)
        {

        }

        private void Read_Setting()
        {
            Form_Loaded_Flag = false;

            ComboBox_Time.Text = task_message.Timer.ToString();
            ComboBox_Time.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Time);

            ListView_Gen();
            ListView_Template.SelectedIndex = task_message.Message_No;
            List_SelNo = ListView_Template.SelectedIndex;
            ToggleSwich_Voice.IsOn = task_message.Voice.Enable;
            Voice_Visibility();
            Template_Load(List_SelNo);

            Form_Loaded_Flag = true;
        }

        private void Template_Load(int Index_No)
        {
            TextBox_Title.Text = XML_Message.Message_List[Index_No].Name;
            TextBox_Message.Text = XML_Message.Message_List[Index_No].Text;

            // 既存のテンプレートの場合は編集不可
            if (Msg_Temp_Check())
            {
                TextBox_Title.IsReadOnly = true;
                TextBox_Message.IsReadOnly = false;
                TextBox_Title.ToolTip = "既存のテンプレート名を編集することは出来ません";
                TextBox_Message.ToolTip = "メッセージ内容の仕様については、左の[?]アイコンをクリックしてください";
            }
            else
            {
                TextBox_Title.IsReadOnly = false;
                TextBox_Message.IsReadOnly = false;
                TextBox_Title.ToolTip = "メッセージ内容のテンプレート名";
                TextBox_Message.ToolTip = "メッセージ内容の仕様については、左の[?]アイコンをクリックしてください";
            }
            Edit_Flag = false;
        }

        // 選択されているテンプレートが既存のテンプレートか
        private bool Msg_Temp_Check()
        {
            foreach(string Name in XML_Message.Msg_Temp.Keys)
            {
                if (XML_Message.Message_List[List_SelNo].Name == Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void ListView_Gen()
        {
            ListView_Template.Items.Clear();
            foreach (Message Msg in XML_Message.Message_List)
            {
                ListView_Template.Items.Add(Msg.Name);
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
                TextBlock_Voice.Text = "メッセージを音声で知らせる";
                if (ToggleSwich_Voice.IsOn == true)
                {
                    Button_Voice.Visibility = Visibility.Visible;       // 表示する
                    Button_Play.Visibility = Visibility.Visible;        // 表示する
                    TextBlock_Button_Play.Text = "再生テスト";
                }
                else
                {
                    Button_Voice.Visibility = Visibility.Hidden;        // 非表示にする(コンポーネントの場所はそのまま)
                    Button_Play.Visibility = Visibility.Hidden;         // 非表示にする(コンポーネントの場所はそのまま)
                    TextBlock_Button_Play.Text = "";
                }
            }
        }

        // 新規テンプレート名の作成
        private string Title_Name_Gen()
        {
            string Name = "";
            bool Flag = false;
            for (int i = 0; i <= Max_count; i++)
            {
                Name = "メッセージ" + (i + 1).ToString();
                Flag = false;
                foreach (Message Msg in XML_Message.Message_List)
                {
                    if (Msg.Name == Name)
                    {
                        Flag = true;
                        break;
                    }
                }
                if (!Flag)
                {
                    break;
                }
            }
            return Name;
        }

        // テンプレート名の重複チェック
        private bool Title_Name_Check(string Name)
        {
            foreach (Message Msg in XML_Message.Message_List)
            {
                if (Msg.Name == Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            // メッセージ設定が正しいかの確認
            if (Message_Check())
            {
                // 既存のテンプレートの場合は保存不可
                if (!Msg_Temp_Check())
                {
                    XML_Message.Message_List[List_SelNo].Name = TextBox_Title.Text;
                    XML_Message.Message_List[List_SelNo].Text = TextBox_Message.Text;
                }
                if (Edit_Flag)
                {
                    Message message = new Message();
                    message.Name = TextBox_Title.Text;
                    message.Text = TextBox_Message.Text;
                    XML_Message.Message_List.Add(message);
                    List_SelNo = XML_Message.Message_List.Count - 1;
                }
                task_message.Message_No = List_SelNo;
                task_message.Voice.Enable = ToggleSwich_Voice.IsOn;

                int Time = int.Parse((string)ComboBox_Time.SelectedItem);
                task_message.Timer = Time;
                this.Close();
            }
        }

        // メッセージ設定が正しいかの確認
        private bool Message_Check()
        {
            if ((TextBox_Title.Text != "") && (TextBox_Message.Text != ""))
            {
                // 文字列のByte数が最大値を超えているか
                if (Etc.Text_Byte_Leng(TextBox_Message.Text) > Max_Leng)
                {
                    FormCtrl_Wpf.Info_Message("文字列のByte数が許容値を超えています。", 0);
                    return false;
                }

                if ((!TextBox_Title.IsReadOnly) && (Title_Name_Check(TextBox_Message.Text)))
                {
                    FormCtrl_Wpf.Info_Message("同じ名称が既に存在します。\r\n名称を変更してください。", 0);
                    return false;
                }
                return true;
            }
            else
            {
                FormCtrl_Wpf.Info_Message("テンプレート名、及びメッセージ内容が設定されていません。", 0);
                return false;
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (List_SelNo < 0)
            {
                FormCtrl_Wpf.Info_Message("アイテムが選択されていません。", 0);
                return;
            }
            // 既存のテンプレートの場合は削除不可
            if (Msg_Temp_Check())
            {
                FormCtrl_Wpf.Info_Message("既存のテンプレートを削除することは出来ません。", 0);
                return;
            }
            if (!FormCtrl_Wpf.YesNo_Message(XML_Message.Message_List[List_SelNo].Name + " を削除します。\r\nよろしいですか？"))
            {
                return;
            }

            // 選択項目を削除する
            XML_Message.Message_List.RemoveAt(List_SelNo);

            Form_Loaded_Flag = false;
            ListView_Gen();
            ListView_Template.SelectedIndex = 0;
            List_SelNo = ListView_Template.SelectedIndex;
            Template_Load(List_SelNo);
            Form_Loaded_Flag = true;
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Title.Text = Title_Name_Gen();
            TextBox_Message.Text = "";
            TextBox_Title.IsEnabled = true;
            TextBox_Message.IsEnabled = true;
            Edit_Flag = true;
        }

        private void Button_Question_Click(object sender, RoutedEventArgs e)
        {
            Form_Message_Edit_Info Form = new Form_Message_Edit_Info();
            Form.ShowDialog();
        }

        private void Button_Voice_Click(object sender, RoutedEventArgs e)
        {
            // Voiceの設定が利用可能な状態か
            if (!Voice_Ctrl.Voice_Ready_Check(true))
            {
                return;
            }

            // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
            if (Voice_Ctrl.Speaker_Check(task_message.Voice) == 0)
            {
                task_message.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                task_message.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
            }

            Form_Edit_Voice Form = new Form_Edit_Voice();
            Form.Voice = task_message.Voice;
            Form.ShowDialog();
            task_message.Voice = Form.Voice;
        }

        private void ListView_Template_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                if (ListView_Template.SelectedIndex >= 0)
                {
                    List_SelNo = ListView_Template.SelectedIndex;
                    Template_Load(List_SelNo);
                }
            }
        }

        private void ComboBox_Time_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {

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
                        if (Voice_Ctrl.Speaker_Check(task_message.Voice) == 0)
                        {
                            task_message.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                            task_message.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
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

        private async void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            if (Voice_Play_Flag)
            {
                Voice_Stop();
            }
            else
            {
                // Voiceの設定が利用可能な状態か
                if (!Voice_Ctrl.Voice_Ready_Check(true))
                {
                    return;
                }

                Voice_Setting voice_Setting = new Voice_Setting();
                voice_Setting = task_message.Voice;

                // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
                if (Voice_Ctrl.Speaker_Check(voice_Setting) == 0)
                {
                    task_message.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                    task_message.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
                    voice_Setting.Style.Name = task_message.Voice.Style.Name;
                    voice_Setting.Style.Type = task_message.Voice.Style.Type;
                }

                // メッセージ設定が正しいかの確認
                if (Message_Check())
                {
                    Voice_Play_Flag = true;
                    Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Stop.ico"));

                    // 特殊文字を含んだメッセージを変換する
                    string text = Voice_Ctrl.Text_Replace(TextBox_Message.Text, voice_Setting);

                    byte[] wav = await Voice_Ctrl.Voice_Gen(text, voice_Setting);
                    Voice_Play(wav, 100, voice_Setting.Query.Device);

                    Image_Button_Play.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/Stop.ico"));
                }
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
