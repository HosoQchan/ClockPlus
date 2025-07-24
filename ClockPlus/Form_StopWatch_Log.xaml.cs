using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ClockPlus
{
    public class StopWatch_Item
    {
        public string Time { get; set; } = "";
    }

    public partial class Form_StopWatch_Log : MetroWindow
    {
        static public List<StopWatch_Item> StopWatch_Log_List = new List<StopWatch_Item>();
        private ObservableCollection<StopWatch_Item> watch_log = new ObservableCollection<StopWatch_Item>();

        private bool Form_Loaded_Flag = false;

        public Form_StopWatch_Log()
        {
            InitializeComponent();
            Grid_NoLog.Visibility = Visibility.Hidden;      // 非表示にする(コンポーネントの場所はそのまま)
            ListView_Log.Visibility = Visibility.Hidden;    // 非表示にする(コンポーネントの場所はそのまま)

            if (StopWatch_Log_List == null)
            {
                StopWatch_Log_List.Clear();
            }
        }

        static public void StopWatch_Log_Clear()
        {
            StopWatch_Log_List.Clear();
        }
        static public void StopWatch_Log_Add(string Time)
        {
            if (StopWatch_Log_List == null)
            {
                StopWatch_Log_List.Clear();
            }
            if (StopWatch_Log_List.Count >= 10)
            {
                StopWatch_Log_List.RemoveAt(0);
            }
            StopWatch_Item item = new StopWatch_Item();
            item.Time = Time;
            StopWatch_Log_List.Add(item);
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            Read_Data();
            Form_Loaded_Flag = true;
        }

        private void Form_Closed(object sender, EventArgs e)
        {

        }

        private void Read_Data()
        {
            for (int i = StopWatch_Log_List.Count -1; i >= 0; i--)
            {
                StopWatch_Item item = new StopWatch_Item();
                item.Time = StopWatch_Log_List[i].Time;
                watch_log.Add(item);
            }
            ListView_Log.ItemsSource = watch_log;
            ListView_Log.Items.Refresh();  /// <<<<<<<<<<<<<<<< これが必要

            if (StopWatch_Log_List.Count > 0)
            {
                Grid_NoLog.Visibility = Visibility.Hidden;      // 非表示にする(コンポーネントの場所はそのまま)
                ListView_Log.Visibility = Visibility.Visible;   // 表示する
            }
            else
            {
                Grid_NoLog.Visibility = Visibility.Visible;     // 表示する
                ListView_Log.Visibility = Visibility.Hidden;    // 非表示にする(コンポーネントの場所はそのまま)
            }
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            StopWatch_Log_Clear();
            Read_Data();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListView_Log_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
