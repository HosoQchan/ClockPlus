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
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ClockPlus
{
    /// <summary>
    /// Form_StopWatch.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_StopWatch : MetroWindow
    {
        readonly Stopwatch StWatch = new Stopwatch();
        readonly DispatcherTimer DsTimer = new DispatcherTimer();
        private bool Watch_Operation = false;

        public Form_StopWatch()
        {
            InitializeComponent();
            Button_Log.Visibility = Visibility.Visible;          // 表示する

            TextBlock_Watch.Text = "00:00:00:00";
            DsTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);    //インターバルを10msに設定
            DsTimer.Tick += new EventHandler(TimerMethod);      //インターバル毎に発生するイベントを設定
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Form_Closed(object sender, EventArgs e)
        {
            DsTimer.Stop();         // タイマー測定停止
            StWatch.Stop();         // ストップウォッチ停止
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (Watch_Operation)
            {
                Form_StopWatch_Log.StopWatch_Log_Add(Get_Watch_Data());

                DsTimer.Stop();         // タイマー 測定停止
                StWatch.Reset();        // ストップウォッチ初期化
                TextBlock_Watch.Text = "00:00:00:00";

                StWatch.Start();        // タイマー測定開始
                DsTimer.Start();        // ストップウォッチ開始
            }
            else
            {
                DsTimer.Stop();         // タイマー 測定停止
                StWatch.Reset();        // ストップウォッチ初期化
                TextBlock_Watch.Text = "00:00:00:00";
            }
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (Watch_Operation)
            {
                Watch_Stop();
            }
            else
            {
                Watch_Start();
            }
        }

        private void Watch_Start()
        {
            Watch_Operation = true;
            Image_Button_Start.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/stop.ico"));
            Button_Log.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)

            StWatch.Start();        // タイマー測定開始
            DsTimer.Start();        // ストップウォッチ開始
        }
        private void Watch_Stop()
        {
            DsTimer.Stop();         // タイマー測定停止
            TextBlock_Watch.Text = Get_Watch_Data();
            StWatch.Stop();         // ストップウォッチ停止

            if (Watch_Operation)
            {
                Form_StopWatch_Log.StopWatch_Log_Add(TextBlock_Watch.Text);
            }
            Watch_Operation = false;
            Image_Button_Start.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/play.ico"));
            Button_Log.Visibility = Visibility.Visible;          // 表示する
        }

        private string Get_Watch_Data()
        {
            string Text;
            var result = StWatch.Elapsed;
            Text = result.Hours.ToString("00");
            Text = Text + ":" + result.Minutes.ToString("00");
            Text = Text + ":" + result.Seconds.ToString("00");
            Text = Text + ":" + (result.Milliseconds / 10).ToString("00");
            return Text;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TimerMethod(object sender, EventArgs e)
        {
            string Text;
            var result = StWatch.Elapsed;
            Text = result.Hours.ToString("00");
            Text = Text + ":" + result.Minutes.ToString("00");
            Text = Text + ":" + result.Seconds.ToString("00");
            Text = Text + ":" + (result.Milliseconds / 10).ToString("00");
            TextBlock_Watch.Text = Text;
        }

        private void Button_Log_Click(object sender, RoutedEventArgs e)
        {
            Form_StopWatch_Log Form = new Form_StopWatch_Log();
            Form.ShowDialog();
        }
    }
}
