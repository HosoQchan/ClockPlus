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
    /// Form_Power.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Power : MetroWindow
    {
        private string Mode = "";
        private System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        private int Timer_Count = 0;
        private bool Cancel_Flag = false;

        public Form_Power(string PowerMode)
        {
            Mode = PowerMode;
            InitializeComponent();
            TextBlock_Message.Text = PowerMode;
            ProgressBar_Time.Value = 100;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            Timer_Count = 10;
            TextBlock_Time.Text = Timer_Count.ToString();

            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            timer1.Stop();
            if (!Cancel_Flag)
            {
                Debug.WriteLine(Mode);
                switch (Mode)
                {
                    case "スリープへ移行":
                        System.Windows.Forms.Application.SetSuspendState(PowerState.Suspend, false, false);
                        break;
                    case "スリープから復帰":

                        break;
                    case "再起動":
                        Task_Ctrl.Win_Reboot();
                        break;
                    case "シャットダウン":
                        Task_Ctrl.Win_PowerOff();
                        break;
                }
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel_Flag = true;
            this.Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Timer_Count --;
            TextBlock_Time.Text = Timer_Count.ToString();
            ProgressBar_Time.Value = ProgressBar_Time.Value - 10;

            if (Timer_Count == 0)
            {
                this.Close();
            }
        }
    }
}
