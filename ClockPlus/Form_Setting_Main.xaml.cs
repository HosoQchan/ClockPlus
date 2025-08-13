using MahApps.Metro.Controls;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ClockPlus.Theme_Ctrl;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using VoicevoxClientSharp.ApiClient.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using NAudio.Wave;

namespace ClockPlus
{
    /// <summary>
    /// Form_Setting_Main.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Setting_Main : MetroWindow
    {
        static public string Tab_Page = "基本";

        private string Theme_Mode;
        private string Theme_Color;

        private bool Window_Loading = false;

        public Form_Setting_Main()
        {
            InitializeComponent();
            Grid_VoicePath.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)

            ComboBox_Theme_Gen();
            ComboBox_VoiceEngine_Gen();

            ComboBox_Weather_Pref();
            ComboBox_Weather_Area("東京都");

            switch (Tab_Page)
            {
                case "音声":
                    TabItem_Voice.IsSelected = true;
                    break;
                case "時計":
                    TabItem_Display.IsSelected = true;
                    break;
                case "天気":
                    TabItem_Weather.IsSelected = true;
                    break;
                default:
                    TabItem_Main.IsSelected = true;
                    break;
            }
        }

        // テーマカラーの一覧を取得
        private void ComboBox_Theme_Gen()
        {
            List<MyColor> lists = new List<MyColor>();
            lists.Clear();
            foreach (var (name, color) in MahApps_color)
            {
                MyColor myColor = new MyColor();
                myColor.Name = name;
                myColor.Color = Theme_Ctrl.ToColorOrDefault(color);
                lists.Add(myColor);
            }
            // 最後の色"Yellow"については、テーマによって見づらくなるので外す
            lists.RemoveAt(lists.Count - 1);
            ComboBox_Color.DataContext = lists;
        }

        // Voice Engineの一覧を取得
        private void ComboBox_VoiceEngine_Gen()
        {
            ComboBox_Engine.Items.Clear();
            foreach (string Name in Voice_Engine.Engine_DefaultPath.Keys)
            {
                ComboBox_Engine.Items.Add(Name);
            }
            ComboBox_Engine.SelectedIndex = 0;
        }

        // 天気予報(都道府県一覧)を作成
        private void ComboBox_Weather_Pref()
        {
            Window_Loading = false;
            ComboBox_Pref.Items.Clear();
            string pref = "";
            for (int i = 0; i < WeatherHacks.AreaCode.Length; i++)
            {
                if (pref != WeatherHacks.AreaCode[i][1])
                {
                    pref = WeatherHacks.AreaCode[i][1];
                    ComboBox_Pref.Items.Add(pref);
                }
            }
            // 初期値(東京都)
            ComboBox_Pref.SelectedIndex = 18;
            Window_Loading = true;
        }

        // 指定された都道府県の地域一覧を作成
        private void ComboBox_Weather_Area(string Pref)
        {
            Window_Loading = false;
            ComboBox_Area.Items.Clear();
            for (int i = 0; i < WeatherHacks.AreaCode.Length; i++)
            {
                if (Pref == WeatherHacks.AreaCode[i][1])
                {
                    ComboBox_Area.Items.Add(WeatherHacks.AreaCode[i][2]);
                }
            }
            ComboBox_Area.SelectedIndex = 0;
            Window_Loading = true;
        }

        // 指定された都道府県と地域から、AreaCodeを取得する
        private string Weather_AreaCode_Get(string pref,string area)
        {
            string AreaCode = "";
            for (int i = 0; i < WeatherHacks.AreaCode.Length; i++)
            {
                if ((pref == WeatherHacks.AreaCode[i][1]) && (area == WeatherHacks.AreaCode[i][2]))
                {
                    AreaCode = WeatherHacks.AreaCode[i][0];
                    break;
                }
            }
            return AreaCode;
        }

        private void Read_Setting_Weather()
        {
            Window_Loading = false;
            string pref = "";
            string area = "";
            for (int i = 0; i < WeatherHacks.AreaCode.Length; i++)
            {
                if (WeatherHacks.AreaCode[i][0] == XML_Main.cnf.Weather.AreaCode)
                {
                    pref = WeatherHacks.AreaCode[i][1];
                    area = WeatherHacks.AreaCode[i][2];
                    break;
                }
            }
            ComboBox_Pref.Text = pref;
            ComboBox_Pref.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Pref);

            ComboBox_Weather_Area(pref);
            ComboBox_Area.Text = area;
            ComboBox_Area.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Area);

            Window_Loading = false;
            ToggleSwich_Voice.IsOn = XML_Main.cnf.Weather.Voice.Enable;
            Voice_Visibility();
            Window_Loading = true;
        }

        private void Read_Setting_Theme()
        {
            Window_Loading = false;

            // 色のモード
            RadioButton_System.IsChecked = false;
            RadioButton_Light.IsChecked = false;
            RadioButton_Dark.IsChecked = false;

            Theme_Mode = XML_Main.cnf.Theme.Mode;
            Theme_Color = XML_Main.cnf.Theme.Color;

            switch (Theme_Mode)
            {
                case "Light":
                    RadioButton_Light.IsChecked = true;
                    break;
                case "Dark":
                    RadioButton_Dark.IsChecked = true;
                    break;
                default:
                    RadioButton_System.IsChecked = true;
                    break;
            }
            ComboBox_Color.Text = Theme_Color;
            ComboBox_Color.SelectedIndex = ComboBox_Color_Index_Get();
            Window_Loading = true;
        }

        private void Read_Setting_Voice()
        {
            Window_Loading = false;
            ComboBox_Engine.Text = XML_Main.cnf.VoiceEngine.Engine_Name;
            ComboBox_Engine.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Engine);
            TextBox_EnginePath.Text = XML_Main.cnf.VoiceEngine.Path;
            Voice_Path_Visibility();
            Window_Loading = true;
        }

        private void Voice_Path_Visibility()
        {
            if (ComboBox_Engine.SelectedItem.ToString() == "無し")
            {
                Grid_VoicePath.Visibility = Visibility.Hidden;      // 非表示にする(コンポーネントの場所はそのまま)
            }
            else
            {
                Grid_VoicePath.Visibility = Visibility.Visible;     // 表示する
            }
        }

        private int ComboBox_Color_Index_Get()
        {
            int Index = -1;
            List<MyColor> lists = new List<MyColor>();
            lists = (List<MyColor>)ComboBox_Color.DataContext;
            for (int i = 0; i < lists.Count; i++)
            {
                if (lists[i].Name == ComboBox_Color.Text)
                {
                    Index = i;
                    break;
                }
            }
            return Index;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            Read_Setting_Theme();
            Read_Setting_Voice();
            Read_Setting_Weather();
            Read_Setting();
            Visibility_Change();
            Window_Loading = true;
        }

        private void Read_Setting()
        {
            Window_Loading = false;
            ToggleSwich_AutoStart.IsOn = XML_Main.cnf.AutoStart;
            ToggleSwich_Signal.IsOn = XML_Main.cnf.Signal.Enable;
            ToggleSwich_Analog.IsOn = XML_Main.cnf.Display.Analog.Enable;
            ToggleSwich_Digital1.IsOn = XML_Main.cnf.Display.Digtal[0].Enable;
            ToggleSwich_Digital2.IsOn = XML_Main.cnf.Display.Digtal[1].Enable;
            ToggleSwich_Weather1.IsOn = XML_Main.cnf.Weather.Weather_Disp[0].Enable;
            ToggleSwich_Weather2.IsOn = XML_Main.cnf.Weather.Weather_Disp[1].Enable;
            TextBox_Password.Text = XML_Main.cnf.Password;
            Window_Loading = true;
        }

        // 設定の内容によってコントーロールの表示/非表示を切り替える
        private void Visibility_Change()
        {
            Button_Signal.Visibility = Visibility.Hidden;               // 非表示にする(コンポーネントの場所はそのまま)
            Button_Alalog_Setting.Visibility = Visibility.Hidden;       // 非表示にする(コンポーネントの場所はそのまま)
            Button_Digital1_Setting.Visibility = Visibility.Hidden;     // 非表示にする(コンポーネントの場所はそのまま)
            Button_Digital2_Setting.Visibility = Visibility.Hidden;     // 非表示にする(コンポーネントの場所はそのまま)
            Button_Weather1_Setting.Visibility = Visibility.Hidden;     // 非表示にする(コンポーネントの場所はそのまま)
            Button_Weather2_Setting.Visibility = Visibility.Hidden;     // 非表示にする(コンポーネントの場所はそのまま)

            if (ToggleSwich_Signal.IsOn)
            {
                Button_Signal.Visibility = Visibility.Visible;              // 表示する
            }
            if (ToggleSwich_Analog.IsOn)
            {
                Button_Alalog_Setting.Visibility = Visibility.Visible;      // 表示する
            }
            if (ToggleSwich_Digital1.IsOn)
            {
                Button_Digital1_Setting.Visibility = Visibility.Visible;    // 表示する
            }
            if (ToggleSwich_Digital2.IsOn)
            {
                Button_Digital2_Setting.Visibility = Visibility.Visible;    // 表示する
            }
            if (ToggleSwich_Weather1.IsOn)
            {
                Button_Weather1_Setting.Visibility = Visibility.Visible;    // 表示する
            }
            if (ToggleSwich_Weather2.IsOn)
            {
                Button_Weather2_Setting.Visibility = Visibility.Visible;    // 表示する
            }
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            XML_Main.cnf.Password = TextBox_Password.Text;

            // 音声Engineが変更された場合
            string Engine_Name = ComboBox_Engine.SelectedItem.ToString();
            /*
            if ((Engine_Name != XML_Main.cnf.VoiceEngine.Engine_Name) || (TextBox_EnginePath.Text != XML_Main.cnf.VoiceEngine.Path))
            {
                if (Engine_Name == "無し")
                {
                    XML_Main.cnf.VoiceEngine.Engine_Name = Engine_Name;
                    XML_Main.cnf.VoiceEngine.Path = TextBox_EnginePath.Text;
                }
                else
                {
                    if (FormCtrl_Wpf.YesNo_Message("音声Engineの設定が変更されました。\r\n再起動してよろしいですか？\r\n[はい] -> 再起動     [いいえ] -> 元の状態を保つ"))
                    {
                        XML_Main.cnf.VoiceEngine.Engine_Name = Engine_Name;
                        XML_Main.cnf.VoiceEngine.Path = TextBox_EnginePath.Text;
                        App.Power_Mode = 1;
                        System.Windows.Application.Current.Shutdown();
                    }
                }
            }
            */
            XML_Main.cnf.VoiceEngine.Engine_Name = Engine_Name;
            XML_Main.cnf.VoiceEngine.Path = TextBox_EnginePath.Text;
        }

        private void RadioButton_Theme_Click(object sender, RoutedEventArgs e)
        {
            if (Window_Loading)
            {
                if (ComboBox_Color.SelectedIndex == -1)
                {
                    ComboBox_Color.SelectedIndex = 0;
                }
                Theme_Change();
            }
        }
        private void ComboBox_Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Window_Loading)
            {
                Theme_Change();
            }
        }
        private void Theme_Change()
        {
            if (RadioButton_System.IsChecked == true)
            {
                Theme_Mode = "Auto";
            }
            else if (RadioButton_Light.IsChecked == true)
            {
                Theme_Mode = "Light";
            }
            else if (RadioButton_Dark.IsChecked == true)
            {
                Theme_Mode = "Dark";
            }
            else
            {
                return;
            }
            List<MyColor> lists = new List<MyColor>();
            lists = (List<MyColor>)ComboBox_Color.DataContext;
            Theme_Color = lists[ComboBox_Color.SelectedIndex].Name;
            Theme_Ctrl.Change_Theme(Theme_Mode, Theme_Color);

            XML_Main.cnf.Theme.Mode = Theme_Mode;
            XML_Main.cnf.Theme.Color = Theme_Color;
        }

        private void Button_Signal_Click(object sender, RoutedEventArgs e)
        {
            Form_Setting_Signal Form = new Form_Setting_Signal();
            Form.ShowDialog();
        }

        private void Button_Alalog_Setting_Click(object sender, RoutedEventArgs e)
        {
            Form_Setting_Analog Form = new Form_Setting_Analog();
            Form.ShowDialog();
        }

        private void Button_Digital1_Setting_Click(object sender, RoutedEventArgs e)
        {
            Form_Setting_Digital Form = new Form_Setting_Digital(0);
            Form.ShowDialog();
        }

        private void Button_Digital2_Setting_Click(object sender, RoutedEventArgs e)
        {
            Form_Setting_Digital Form = new Form_Setting_Digital(1);
            Form.ShowDialog();
        }

        private void Button_Weather1_Setting_Click(object sender, RoutedEventArgs e)
        {
            Form_Setting_Weather Form = new Form_Setting_Weather(0);
            Form.ShowDialog();
        }

        private void Button_Weather2_Setting_Click(object sender, RoutedEventArgs e)
        {
            Form_Setting_Weather Form = new Form_Setting_Weather(1);
            Form.ShowDialog();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ToggleSwich_AutoStart_Toggled(object sender, RoutedEventArgs e)
        {
            if (Window_Loading)
            {
                if (ToggleSwich_AutoStart.IsOn)
                {
                    // 管理者モードで実行されているか
                    if (App.IsAdmin())
                    {
                        // スタートアップにLinkを作成
                        if (Startup_Lnk_Create())
                        {
                            XML_Main.cnf.AutoStart = true;
                        }
                        else
                        {
                            FormCtrl_Wpf.Error_Message("スタートアップへの登録に失敗しました。");
                            Window_Loading = false;
                            ToggleSwich_AutoStart.IsOn = false;
                            Window_Loading = true;
                        }
                    }
                    // 管理者モードで実行されていない場合は、管理者モードで再起動する
                    else
                    {
//                        FormCtrl_Wpf.Info_Message("この設定の操作は\r\n" + App.Title + " が管理者権限で起動されている必要があります。\r\n管理者権限で再起動してください。");
                        
                        if (FormCtrl_Wpf.YesNo_Message("管理者権限で実行する必要があります。\r\n管理者権限で再起動しますか？"))
                        {
                            App.Power_Mode = 2;
                            System.Windows.Application.Current.Shutdown();
                            this.Close();
                        }
                        
                        Window_Loading = false;
                        ToggleSwich_AutoStart.IsOn = false;
                        Window_Loading = true;
                    }
                }
                else
                {
                    // 管理者モードで実行されているか
                    if (App.IsAdmin())
                    {
                        // スタートアップのLinkを削除
                        if (Startup_Lnk_Delete())
                        {
                            XML_Main.cnf.AutoStart = false;
                        }
                        else
                        {
                            FormCtrl_Wpf.Error_Message("スタートアップからの削除に失敗しました。");
                            Window_Loading = false;
                            ToggleSwich_AutoStart.IsOn = true;
                            Window_Loading = true;
                        }
                    }
                    // 管理者モードで実行されていない場合は、管理者モードで再起動する
                    else
                    {
//                        FormCtrl_Wpf.Info_Message("この設定の操作は\r\n" + App.Title + " が管理者権限で起動されている必要があります。\r\n管理者権限で再起動してください。");
                        
                        if (FormCtrl_Wpf.YesNo_Message("管理者権限で実行する必要があります。\r\n管理者権限で再起動しますか？"))
                        {
                            App.Power_Mode = 2;
                            System.Windows.Application.Current.Shutdown();
                            this.Close();
                        }
                        
                        Window_Loading = false;
                        ToggleSwich_AutoStart.IsOn = true;
                        Window_Loading = true;
                    }
                }
            }
        }

        private bool Startup_Lnk_Create()
        {
            string shortcutPath = App.LnkFile;
            string targetPath = App.ExeFile;
            string description = App.Title + "へのショートカット";
            string iconLocation = App.ExeFile + ",0";
            try
            {
                // IWshRuntimeLibraryへの参照を追加
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                shortcut.TargetPath = targetPath;
                shortcut.Description = description;
                shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                if (!string.IsNullOrEmpty(iconLocation))
                {
                    shortcut.IconLocation = iconLocation;
                }
                shortcut.Save();
                Debug.WriteLine($"ショートカット '{shortcutPath}' を作成しました。");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"エラーが発生しました: {ex.Message}");
                return false;
            }
        }

        //スタートアップのLinkを削除
        private bool Startup_Lnk_Delete()
        {
            // スタートアップフォルダに登録済みか確認
            if (!System.IO.File.Exists(App.LnkFile))
            {
                return true;
            }
            else
            {
                // スタートアップのショートカットを削除
                try
                {
                    System.IO.File.Delete(App.LnkFile);
                    return true;
                }
                catch (IOException ex)
                {
                    return false;
                }
            }
        }

        private void ToggleSwich_Signal_Toggled(object sender, RoutedEventArgs e)
        {
            if (Window_Loading)
            {
                Visibility_Change();
                XML_Main.cnf.Signal.Enable = ToggleSwich_Signal.IsOn;
            }
        }

        private void ToggleSwich_Analog_Toggled(object sender, RoutedEventArgs e)
        {
            if (Window_Loading)
            {
                Visibility_Change();
                XML_Main.cnf.Display.Analog.Enable = ToggleSwich_Analog.IsOn;
            }
        }

        private void ToggleSwich_Digital1_Toggled(object sender, RoutedEventArgs e)
        {
            if (Window_Loading)
            {
                Visibility_Change();
                XML_Main.cnf.Display.Digtal[0].Enable = ToggleSwich_Digital1.IsOn;
            }
        }

        private void ToggleSwich_Digital2_Toggled(object sender, RoutedEventArgs e)
        {
            if (Window_Loading)
            {
                Visibility_Change();
                XML_Main.cnf.Display.Digtal[1].Enable = ToggleSwich_Digital2.IsOn;
            }
        }

        private void ToggleSwich_Weather1_Toggled(object sender, RoutedEventArgs e)
        {
            if (Window_Loading)
            {
                Visibility_Change();
                XML_Main.cnf.Weather.Weather_Disp[0].Enable = ToggleSwich_Weather1.IsOn;
            }
        }

        private void ToggleSwich_Weather2_Toggled(object sender, RoutedEventArgs e)
        {
            if (Window_Loading)
            {
                Visibility_Change();
                XML_Main.cnf.Weather.Weather_Disp[1].Enable = ToggleSwich_Weather2.IsOn;
            }
        }

        private void TextBox_Password_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (Window_Loading)
            {
                // 半角英数字のみであるかを判定する
                bool result = !(Etc.Keychr_chk(e.Text, @"^[0-9a-zA-Z]+"));
                e.Handled = result;
            }
        }

        private void TextBox_Password_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Window_Loading)
            {
                // 貼り付けの場合
                if (e.Command == ApplicationCommands.Paste)
                {
                    // 処理済みにします。
                    e.Handled = true;
                }
            }
        }

        private void ComboBox_Engine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Window_Loading)
            {
                string Name = ComboBox_Engine.SelectedItem.ToString();
                TextBox_EnginePath.Text = Voice_Engine.Engine_DefaultPath[Name];
                Voice_Path_Visibility();
            }
        }

        private void Button_FolderSelect_Click(object sender, RoutedEventArgs e)
        {
            string FilePath = Etc.Dlg_Folder_Select(TextBox_EnginePath.Text);
            if (FilePath != "")
            {
                string FileName = FilePath + "\\run.exe";
                if (!System.IO.File.Exists(FileName))
                {
                    FormCtrl_Wpf.Error_Message("音声Engine本体(run.exe)が見つかりません。");
                    return;
                }
                TextBox_EnginePath.Text = FilePath;
            }
        }

        private void ComboBox_Pref_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Window_Loading)
            {
                string pref = ComboBox_Pref.SelectedItem.ToString();
                if (pref != null)
                {
                    ComboBox_Weather_Area(pref);
                    Area_Change();
                }
            }
        }

        private void ComboBox_Area_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Window_Loading)
            {
                Area_Change();
            }
        }

        // 予報エリアの設定が変更された時の処理
        private void Area_Change()
        {
            string area = ComboBox_Area.SelectedItem.ToString();
            string pref = ComboBox_Pref.SelectedItem.ToString();
            string AreaCode = Weather_AreaCode_Get(pref, area);

            XML_Main.cnf.Weather.AreaCode = AreaCode;
            XML_Main.cnf.Weather.Acquisition = DateTime.MinValue;
        }

        private void Button_Voice_Click(object sender, RoutedEventArgs e)
        {
            // Voiceの設定が利用可能な状態か
            if (!Voice_Ctrl.Voice_Ready_Check(true))
            {
                return;
            }

            // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
            if (Voice_Ctrl.Speaker_Check(XML_Main.cnf.Weather.Voice) == 0)
            {
                XML_Main.cnf.Weather.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                XML_Main.cnf.Weather.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
            }

            Form_Edit_Voice Form = new Form_Edit_Voice();
            Form.Voice = XML_Main.cnf.Weather.Voice;
            Form.ShowDialog();
            XML_Main.cnf.Weather.Voice = Form.Voice;
        }

        private void ToggleSwich_Voice_Toggled(object sender, RoutedEventArgs e)
        {
            if (Window_Loading)
            {
                if (ToggleSwich_Voice.IsOn)
                {
                    // Voiceの設定が利用可能な状態か
                    if (Voice_Ctrl.Voice_Ready_Check(true))
                    {
                        // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
                        if (Voice_Ctrl.Speaker_Check(XML_Main.cnf.Weather.Voice) == 0)
                        {
                            XML_Main.cnf.Weather.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                            XML_Main.cnf.Weather.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
                        }
                    }
                    else
                    {
                        Window_Loading = false;
                        ToggleSwich_Voice.IsOn = false;
                        Window_Loading = true;
                    }
                }
                XML_Main.cnf.Weather.Voice.Enable = ToggleSwich_Voice.IsOn;
                Voice_Visibility();
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
                TextBlock_Voice.Text = "予報を音声で知らせる";
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

        private void Button_Weather_Click(object sender, RoutedEventArgs e)
        {
            if (XML_Main.cnf.Weather.Voice.Enable)
            {
                // Voiceの設定が利用可能な状態か
                if (Voice_Ctrl.Voice_Ready_Check(true))
                {
                    // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
                    if (Voice_Ctrl.Speaker_Check(XML_Main.cnf.Weather.Voice) == 0)
                    {
                        XML_Main.cnf.Weather.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                        XML_Main.cnf.Weather.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
                    }
                }
            }
            WeatherHacks weatherHacks = new WeatherHacks();
            weatherHacks.Weather_HP();
        }
    }
}
