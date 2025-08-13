using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static ClockPlus.Date_Picker;

namespace ClockPlus
{
    /// <summary>
    /// Form_Timer_List.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Timer_List : MetroWindow
    {
        private ObservableCollection<ListView_Task_Item> task_data = new ObservableCollection<ListView_Task_Item>();

        private bool Form_Loaded_Flag = false;
        private int TaskList_Select_No = -1;
        public const int Max_count = 16;
        static public List<Task> Timer_List = new List<Task>();

        public Form_Timer_List()
        {
            InitializeComponent();
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
            task_data.Clear();
            Timer_List.Clear();
            for (int i = 0; i < XML_Task.Task.TaskList.Count; i++)
            {
                Task task = new Task();
                task = XML_Task.Task.TaskList[i];
                if (task.Type == "Timer")
                {
                    Timer_List.Add(task);

                    ListView_Task_Item _task_item = new ListView_Task_Item();
                    _task_item.List_No = i;
                    _task_item.Name = task.Name;

                    Str_Dtime_Item str_dtime = new Str_Dtime_Item();
                    str_dtime = Date_Picker.DateTime_str(task.Timer);

                    Int_Dtime_Item int_dtime = new Int_Dtime_Item();
                    int_dtime = Date_Picker.DateTime_int(task.Timer);

                    /*
                    if (int_dtime.Time_H == 0)
                    {
                        _task_item.Min = int_dtime.Time_M.ToString() + "分";
                    }
                    else
                    {
                        _task_item.Min = int_dtime.Time_H.ToString() + "時間";
                        if (int_dtime.Time_M != 0)
                        {
                            _task_item.Min = _task_item.Min + str_dtime.Time + "分";
                        }
                    }
                    */
                    _task_item.Min = str_dtime.Time;

                    if (task.Power.Enable)
                    {
                        _task_item.Power = "電源制御(" + task.Power.Mode + ")";
                        _task_item.Grid_Power = Visibility.Visible;         // 表示する
                    }
                    else
                    {
                        if (task.Sound.Enable)
                        {
                            _task_item.Alarm = "サウンド再生";
                        }
                        if (task.Message.Enable)
                        {
                            _task_item.Balloon = "メッセージ";
                        }
                        if (task.App.Enable)
                        {
                            _task_item.App = "アプリの起動";
                        }
                        _task_item.Grid_AlarmApp = Visibility.Visible;      // 表示する
                    }
                    task_data.Add(_task_item);
                }
            }
            ListView_Task.ItemsSource = task_data;
            ListView_Task.Items.Refresh();  /// <<<<<<<<<<<<<<<< これが必要

            if (task_data.Count == 0)
            {
                Grid_Task_List.Visibility = Visibility.Hidden;      // 非表示にする(コンポーネントの場所はそのまま)
                Grid_NoTask.Visibility = Visibility.Visible;        // 表示する

                Button_Delete.Visibility = Visibility.Hidden;       // 非表示にする(コンポーネントの場所はそのまま)
                Button_Edit.Visibility = Visibility.Hidden;         // 非表示にする(コンポーネントの場所はそのまま)
            }
            else
            {
                Grid_Task_List.Visibility = Visibility.Visible;     // 表示する
                Grid_NoTask.Visibility = Visibility.Hidden;         // 非表示にする(コンポーネントの場所はそのまま)

                Button_Delete.Visibility = Visibility.Visible;      // 表示する
                Button_Edit.Visibility = Visibility.Visible;        // 表示する
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList_Select_No > -1)
            {
                Int_Dtime_Item int_dtime = new Int_Dtime_Item();
                int_dtime = Date_Picker.DateTime_int(XML_Task.Task.TaskList[TaskList_Select_No].Timer);

                // 現在の時刻にタイマー値を加算した時間を設定して開始する
                DateTime dt = DateTime.Now;
                dt = dt.AddHours(int_dtime.Time_H);
                dt = dt.AddMinutes(int_dtime.Time_M);

                XML_Task.Task.TaskList[TaskList_Select_No].DateTime = dt;
                XML_Task.Task.TaskList[TaskList_Select_No].Enable = true;
                Task_Ctrl.Sort_Task_List();     // リストを日時の昇順にソートする
                Task_Ctrl.Get_Next_Task();      // 次に有効なタスクのリスト番号を取得する
                this.Close();
            }
            else
            {
                FormCtrl_Wpf.Info_Message("アイテムが選択されていません。", 0);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if ((ListView_Task.SelectedIndex > -1) && (TaskList_Select_No > -1))
            {
                if (FormCtrl_Wpf.YesNo_Message(XML_Task.Task.TaskList[TaskList_Select_No].Name + "\r\nを削除します。よろしいですか？"))
                {
                    // 選択項目を削除する
                    XML_Task.Task.TaskList.RemoveAt(TaskList_Select_No);
                    Task_Ctrl.Sort_Task_List();     // リストを日時の昇順にソートする
                    Task_Ctrl.Get_Next_Task();      // 次に有効なタスクのリスト番号を取得する
                    Read_Data();
                }
            }
            else
            {
                FormCtrl_Wpf.Info_Message("アイテムが選択されていません。", 0);
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            if ((ListView_Task.SelectedIndex > -1) && (TaskList_Select_No > -1))
            {
                Form_Edit_Timer.Edit_No = TaskList_Select_No;
                Form_Edit_Timer Form = new Form_Edit_Timer();
                Form.ShowDialog();
                Read_Data();
            }
            else
            {
                FormCtrl_Wpf.Info_Message("アイテムが選択されていません。", 0);
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if(Timer_List.Count >= Max_count)
            {
                FormCtrl_Wpf.Info_Message("登録できる件数(" + Max_count.ToString() + "件まで)を超えています。", 0);
                return;
            }
            Form_Edit_Timer.Edit_No = -1;
            Form_Edit_Timer Form = new Form_Edit_Timer();
            Form.ShowDialog();
            Read_Data();
        }

        private void ListView_Task_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                if (ListView_Task.SelectedIndex > -1)
                {
                    TaskList_Select_No = task_data[ListView_Task.SelectedIndex].List_No;
                }
            }
        }
    }
}
