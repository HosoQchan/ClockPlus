using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Form_Edit_App.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Edit_App : MetroWindow
    {
        public Task_Program task_app = new Task_Program();

        private bool Form_Loaded_Flag = false;

        public Form_Edit_App()
        {
            InitializeComponent();
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox_FileName.Text = task_app.FileName;
            TextBox_Option.Text = task_app.Option;
            Form_Loaded_Flag = true;
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            if ((TextBox_FileName.Text == "") && (TextBox_Option.Text == ""))
            {
                FormCtrl_Wpf.Info_Message("実行するアプリ・コマンドが設定されていません。");
                return;
            }
            else
            {
                task_app.FileName = TextBox_FileName.Text;
                task_app.Option = TextBox_Option.Text;
                this.Close();
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_File_Select_Click(object sender, RoutedEventArgs e)
        {
            string InitialDir = XML_Main.Sound_Dir + "\\";
            string InitialExt = ".EXE";
            string Filter = "EXEファイル(*.EXE)|*.EXE|BATファイル(*.BAT)|*.BAT|全てのファイル(*.*)|*.*";
            string FileName = Etc.Dlg_File_Select(InitialDir, Filter, InitialExt);

            if (FileName != "")
            {
                TextBox_FileName.Text = FileName;
            }
        }

        private void Button_Test_Click(object sender, RoutedEventArgs e)
        {
            if ((TextBox_FileName.Text == "") && (TextBox_Option.Text == ""))
            {
                FormCtrl_Wpf.Info_Message("実行するアプリ・コマンドが設定されていません。");
                return;
            }
            else
            {
                string Program = TextBox_FileName.Text;
                string Option = TextBox_Option.Text;
                string Command = Program + " " + Option;
                try
                {
                    ProcessStartInfo pInfo = new ProcessStartInfo();
                    pInfo.FileName = Program;
                    pInfo.Arguments = Option;
                    Process.Start(pInfo);
                }
                catch (Exception ex)
                {
                    FormCtrl_Wpf.Error_Message("アプリの起動でエラーが発生しました。");
                }
            }
        }
    }
}
