using MahApps.Metro.Controls;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ClockPlus
{

    public partial class Menu_Win : MetroWindow
    {
        public Menu_Win()
        {
            InitializeComponent();
        }

        private void MainMenu_Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Setting_Main_Win setting_Main_Win = new Setting_Main_Win();
            setting_Main_Win.ShowDialog();
        }

        private void Alarm_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Timer_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void Stopwach_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
