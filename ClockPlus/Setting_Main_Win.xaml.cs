using MahApps.Metro.Controls;
using Microsoft.Win32;
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
using Application = System.Windows.Forms.Application;

namespace ClockPlus
{
    public partial class Setting_Main_Win : MetroWindow
    {
        private bool FormLoad_Flag = false;

        public Setting_Main_Win()
        {
            InitializeComponent();

            Signal_CheckBox.IsChecked = false;
            Signal_Setting_Button.IsEnabled = false;

            Voice_CheckBox.IsEnabled = false;
            Voice_CheckBox.IsChecked = false;
            Voice_Setting_Button.IsEnabled = false;

            Analog_CheckBox.IsChecked = false;
            Analog_Setting_Button.IsEnabled = false;

            Digital1_CheckBox.IsChecked = false;
            Digital1_Setting_Button.IsEnabled = false;

            Digital2_CheckBox.IsChecked = false;
            Digital2_Setting_Button.IsEnabled = false;
        }

        private void Setting_Main_Window_Loaded(object sender, RoutedEventArgs e)
        {
            Get_Setting_Data();             // 設定データの取り込み
            FormLoad_Flag = true;
        }

        // 設定データの取り込み
        private void Get_Setting_Data()
        {
            // スタートアップフォルダに登録済みか確認
            /*
            bool Flag =  File.Exists(Main_Win.LnkFile);
            AutoStart_CheckBox.IsChecked = Flag;
            */
            // スタートアップフォルダに登録済みか確認
            AutoStart_CheckBox.IsChecked = IsStartupEnabled();

            // 暫定的に無効
            AutoStart_CheckBox.IsEnabled = false;
            AutoStart_CheckBox.IsChecked = false;
            // 暫定的に無効

            if (XML_Main_IO.Main.Voice.Enable)
            {
                Voice_CheckBox.IsChecked = true;
                Voice_Setting_Button.IsEnabled = true;
            }

            if (XML_Main_IO.Main.Signal.Enable)
            {
                Signal_CheckBox.IsChecked = true;
                Signal_Setting_Button.IsEnabled = true;
                if (Voice_Ctrl.SynthList.Count > 0)
                {
                    Voice_CheckBox.IsEnabled = true;
                }
            }
            if (XML_Main_IO.Main.Display.Analog.Enable)
            {
                Analog_CheckBox.IsChecked = true;
                Analog_Setting_Button.IsEnabled = true;
            }
            if (XML_Main_IO.Main.Display.Digtal[0].Enable)
            {
                Digital1_CheckBox.IsChecked = true;
                Digital1_Setting_Button.IsEnabled = true;
            }
            if (XML_Main_IO.Main.Display.Digtal[1].Enable)
            {
                Digital2_CheckBox.IsChecked = true;
                Digital2_Setting_Button.IsEnabled = true;
            }
        }

        private void Analog_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                //スキンフォルダ内にサブフォルダがあるか調べる
                if (Directory.Exists(XML_Main_IO.Skin_Dir))
                {
                    if (0 < Directory.GetDirectories(XML_Main_IO.Skin_Dir).Length)

                    {
                        Analog_Setting_Button.IsEnabled = true;
                        XML_Main_IO.Main.Display.Analog.Enable = true;
                        FormCtrl_Net.valid_Form(App._Analog_Form);     // フォームを表示
                        return;
                    }
                }
                FormCtrl_Wpf.Error_Message("Skinが見つかりません。");
                Analog_Setting_Button.IsEnabled = false;
                Analog_CheckBox.IsChecked = false;
                XML_Main_IO.Main.Display.Analog.Enable = false;
                FormCtrl_Net.Hide_Form(App._Analog_Form);              // フォームを非表示
            }
        }

        private void Analog_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                Analog_Setting_Button.IsEnabled = false;
                XML_Main_IO.Main.Display.Analog.Enable = false;
                FormCtrl_Net.Hide_Form(App._Analog_Form);              // フォームを非表示
            }
        }

        private void Analog_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Setting_Analog_Win _Setting_Analog_Win = new Setting_Analog_Win();
            _Setting_Analog_Win.ShowDialog();
        }

        private void Digital1_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                Digital1_Setting_Button.IsEnabled = true;
                XML_Main_IO.Main.Display.Digtal[0].Enable = true;
                FormCtrl_Net.valid_Form(App._Digital1_Form);            // フォームを表示
                App._Digital1_Form.Disp1_Drow();                        // デジタル時計２を描画
            }        
        }

        private void Digital1_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                Digital1_Setting_Button.IsEnabled = false;
                XML_Main_IO.Main.Display.Digtal[0].Enable = false;
                FormCtrl_Net.Hide_Form(App._Digital1_Form);              // フォームを非表示
            }
        }

        private void Digital1_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Setting_Digital_Win.Setting_Dno = 0;
            Setting_Digital_Win _Setting_Digital_Win = new Setting_Digital_Win();
            _Setting_Digital_Win.ShowDialog();
        }

        private void Digital2_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                Digital2_Setting_Button.IsEnabled = true;
                XML_Main_IO.Main.Display.Digtal[1].Enable = true;
                FormCtrl_Net.valid_Form(App._Digital2_Form);            // フォームを表示
                App._Digital2_Form.Disp2_Drow();                        // デジタル時計２を描画
            }
        }

        private void Digital2_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                Digital2_Setting_Button.IsEnabled = false;
                XML_Main_IO.Main.Display.Digtal[1].Enable = false;
                FormCtrl_Net.Hide_Form(App._Digital2_Form);              // フォームを非表示
            } 
        }

        private void Digital2_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Setting_Digital_Win.Setting_Dno = 1;
            Setting_Digital_Win _Setting_Digital_Win = new Setting_Digital_Win();
            _Setting_Digital_Win.ShowDialog();
        }

        private void AutoStart_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                /*
                // スタートアップにLinkを作成
                if (Startup_Lnk_Create())
                {
                    XML_Main_IO.Main.AutoStart = true;
                }
                */
                if (!IsStartupEnabled())
                {
                    SetRegKey();
                    XML_Main_IO.Main.AutoStart = true;
                }
            }          
        }

        private void AutoStart_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                /*
                // スタートアップのLinkを削除
                if (Startup_Lnk_Delete())
                {
                    XML_Main_IO.Main.AutoStart = false;
                }
                */
                if (IsStartupEnabled())
                {
                    DeleteRegKey();
                    XML_Main_IO.Main.AutoStart = false;
                }
            }               
        }

        private void Signal_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Signal.Enable = true;
                Signal_Setting_Button.IsEnabled = true;

                if (Voice_Ctrl.SynthList.Count > 0)
                {
                    Voice_CheckBox.IsEnabled = true;
                    Voice_Setting_Button.IsEnabled = true;
                }
            }     
        }

        private void Signal_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Signal.Enable = false;
                Signal_Setting_Button.IsEnabled = false;

                Voice_CheckBox.IsEnabled = false;
                Voice_Setting_Button.IsEnabled = false;
            }   
        }

        private void Signal_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Setting_Signal_Win _Setting_Signal_Win = new Setting_Signal_Win();
            _Setting_Signal_Win.ShowDialog();
        }

        private void Voice_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Voice_Ctrl.SynthList.Count > 0)
            {
                XML_Main_IO.Main.Voice.Enable = true;
                Voice_Setting_Button.IsEnabled = true;
            }
            else
            {
                FormCtrl_Wpf.Error_Message("使用出来ません。");
                Voice_CheckBox.IsChecked = false;
                Voice_Setting_Button.IsEnabled = false;
            }
        }

        private void Voice_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            XML_Main_IO.Main.Voice.Enable = false;
            Voice_Setting_Button.IsEnabled = false;
        }

        private void Voice_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Setting_Voice_Win _Setting_Voice_Win = new Setting_Voice_Win();
            _Setting_Voice_Win.ShowDialog();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // レジストリ書き込み
        private void SetRegKey()
        {
            try
            {
                Microsoft.Win32.RegistryKey regkey =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", true);
                regkey.SetValue(Application.ProductName, Application.ExecutablePath);
                regkey.Close();
            }
            catch { }
        }

        // レジストリ削除
        private void DeleteRegKey()
        {
            try
            {
                Microsoft.Win32.RegistryKey regkey =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", true);
                regkey.DeleteValue(Application.ProductName, false);
                regkey.Close();
            }
            catch { }
        }

        // レジストリ読み込み
        private bool IsStartupEnabled()
        {
            string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey rKey = Registry.CurrentUser.OpenSubKey(keyName))
            {
                return (rKey.GetValue(Application.ProductName) != null) ? true : false;
            }
        }
    }
}
