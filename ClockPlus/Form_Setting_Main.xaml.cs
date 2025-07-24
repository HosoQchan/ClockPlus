using MahApps.Metro.Controls;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ClockPlus.Theme_Ctrl;
using IWshRuntimeLibrary;
using Microsoft.Win32;

namespace ClockPlus
{
    /// <summary>
    /// Form_Setting_Main.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Setting_Main : MetroWindow
    {
        private string Theme_Mode;
        private string Theme_Color;

        private bool Window_Loading = false;

        public Form_Setting_Main()
        {
            InitializeComponent();
            ComboBox_Gen();
        }

        // テーマカラーの一覧を取得
        private void ComboBox_Gen()
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
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            XML_Main.cnf.Password = TextBox_Password.Text;
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
//            Form.Setting_Dno = 0;
            Form.ShowDialog();
        }

        private void Button_Digital2_Setting_Click(object sender, RoutedEventArgs e)
        {
            Form_Setting_Digital Form = new Form_Setting_Digital(1);
//            Form.Setting_Dno = 1;
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
                            App.Admin_Restart = true;
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
                            App.Admin_Restart = true;
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
    }
}
