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

namespace ClockPlus
{
    /// <summary>
    /// Form_Password.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Password : MetroWindow
    {
        private bool Window_Loading = false;
        private string Word = "";
        private int Tno = -1;

        public Form_Password(string Password)
        {
            Word = Password;
            InitializeComponent();
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            Window_Loading = true;
        }

        private void Form_Closed(object sender, EventArgs e)
        {

        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_Password.Text != Word)
            {
                FormCtrl_Wpf.Info_Message("パスワードが正しくありません。", 0);
                return;
            }
            Task_Ctrl.Sound_Stop();
            this.Close();
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
