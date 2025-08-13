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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.DataFormats;

namespace ClockPlus
{
    /// <summary>
    /// Form_Menu.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Menu : MetroWindow
    {
        public Form_Menu()
        {
            InitializeComponent();
            // フォームの位置を設定

        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Form_Closed(object sender, EventArgs e)
        {

        }

        private void Button_About_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowOrActivate<Form_About>();
            this.Close();
        }

        private void Button_Setting_Click(object sender, RoutedEventArgs e)
        {
            Form_Setting_Main.Tab_Page = "基本";
            WindowManager.ShowOrActivate<Form_Setting_Main>();
            this.Close();
        }

        private void Button_Alarm_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowOrActivate<Form_Alarm_List>();
            this.Close();
        }

        private void Button_Timer_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowOrActivate<Form_Timer_List>();
            this.Close();
        }

        private void Button_StopWatch_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowOrActivate<Form_StopWatch>();
            this.Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
