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
using System.Xml.Serialization;
using static ClockPlus.Date_Picker;
using static MaterialDesignThemes.Wpf.Theme;

namespace ClockPlus
{
    public partial class Form_Alarm_List : MetroWindow
    {
        private ObservableCollection<ListView_Task_Item> task_data = new ObservableCollection<ListView_Task_Item>();
        
        private bool Form_Loaded_Flag = false;
        private int TaskList_Select_No = -1;
        public const int Max_count = 16;
        static public List<Task> Alarm_List = new List<Task>();

        public Form_Alarm_List()
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
            Alarm_List.Clear();
            for (int i = 0; i < XML_Task.Task.TaskList.Count; i++)
            {
                Task task = new Task();
                task = XML_Task.Task.TaskList[i];
                if (task.Type == "Alarm")
                {
                    Alarm_List.Add(task);

                    ListView_Task_Item _task_item = new ListView_Task_Item();
                    _task_item.List_No = i;
                    _task_item.Enable = task.Enable;
                    _task_item.Name = task.Name;
                    _task_item.Cycle = task.Cycle;

                    Str_Dtime_Item str_dtime = new Str_Dtime_Item();
                    str_dtime = Date_Picker.DateTime_str(task.DateTime);
                    _task_item.Year = str_dtime.Date_Y;
                    _task_item.Month = str_dtime.Date_M;
                    _task_item.Day = str_dtime.Date_D;
                    _task_item.Hour = str_dtime.Time_H;
                    _task_item.Min = str_dtime.Time_M;

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
             
                    _task_item.Mon = task.Week_List.Mon;
                    _task_item.Tue = task.Week_List.Tue;
                    _task_item.Wed = task.Week_List.Wed;
                    _task_item.Thu = task.Week_List.Thu;
                    _task_item.Fri = task.Week_List.Fri;
                    _task_item.Sat = task.Week_List.Sat;
                    _task_item.Sun = task.Week_List.Sun;

                    _task_item = Visibility_Change(_task_item);
                    task_data.Add( _task_item);
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
            ListView_Task.SelectedIndex = -1;
            TaskList_Select_No = -1;
        }

        // 回数設定の内容によってコントーロールの表示/非表示を切り替える
        private ListView_Task_Item Visibility_Change(ListView_Task_Item _task_item)
        {
            switch (_task_item.Cycle)
            {
                case "一回":
                    _task_item.Grid_Week = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    _task_item.Grid_Date = Visibility.Visible;          // 表示する
                    _task_item.Grid_Year = Visibility.Visible;          // 表示する
                    _task_item.Grid_Month = Visibility.Visible;         // 表示する
                    break;
                case "毎日":
                    _task_item.Grid_Week = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)
                    _task_item.Grid_Date = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    break;
                case "毎週":
                    _task_item.Grid_Week = Visibility.Visible;          // 表示する
                    _task_item.Grid_Date = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    break;
                case "毎月":
                    _task_item.Grid_Week = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    _task_item.Grid_Date = Visibility.Visible;          // 表示する
                    _task_item.Grid_Year = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)
                    _task_item.Grid_Month = Visibility.Collapsed;       // 非表示にする(コンポーネントの場所を詰める)
                    break;
                case "毎年":
                    _task_item.Grid_Week = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    _task_item.Grid_Date = Visibility.Visible;          // 表示する
                    _task_item.Grid_Year = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)
                    _task_item.Grid_Month = Visibility.Visible;         // 表示する
                    break;
            }
            return _task_item;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Select_Check())
            {
                if (FormCtrl_Wpf.YesNo_Message(XML_Task.Task.TaskList[TaskList_Select_No].Name + "\r\nを削除します。よろしいですか？"))
                {
                    // 選択項目を削除する
                    XML_Task.Task.TaskList.RemoveAt(TaskList_Select_No);
                    Task_Ctrl.Sort_Task_List();     // リストを日時の昇順にソートする
                    Task_Ctrl.Get_Next_Task();      // 次に有効なタスクのリスト番号を取得する
                }
            }
            else
            {
                FormCtrl_Wpf.Info_Message("アイテムが選択されていません。", 0);
            }
            Read_Data();
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (Select_Check())
            {
                Form_Edit_Alarm.Edit_No = TaskList_Select_No;
                Form_Edit_Alarm Form = new Form_Edit_Alarm();
                Form.ShowDialog();
            }
            else
            {
                FormCtrl_Wpf.Info_Message("アイテムが選択されていません。", 0);
            }
            Read_Data();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (Alarm_List.Count >= Max_count)
            {
                FormCtrl_Wpf.Info_Message("登録できる件数(" + Max_count.ToString() + "件まで)を超えています。", 0);
                return;
            }
            Form_Edit_Alarm.Edit_No = -1;
            Form_Edit_Alarm Form = new Form_Edit_Alarm();
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

        private void ToggleSwitch_Enable_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                // フォーカスを受け取ったコントロールのデータコンテキストを得る
                if (sender is System.Windows.Controls.Control ctl
                && ctl.DataContext is ListView_Task_Item data)
                {
                    // data（結び付けられているデータコンテキスト）を使って何かする
                    // 例：ListViewでこのデータを持っている項目を選択する
                    //     ＝フォーカスを受け取ったコントロールを含む項目が選択される
                    this.ListView_Task.SelectedItem = data;

                    // ListView内でのインデックスを得る
                    if (this.ListView_Task.ItemsSource is IList<ListView_Task_Item> list)
                    {
                        int i = task_data[list.IndexOf(data)].List_No;
                        TaskList_Select_No = i;
                        Form_Loaded_Flag = false;
                        this.ListView_Task.SelectedIndex = list.IndexOf(data);
                        Form_Loaded_Flag = true;

                        Task task = new Task();
                        task = XML_Task.Task.TaskList[TaskList_Select_No];

                        DateTime Tgt_dtime = task.DateTime;
                        DateTime Now_dtime = (DateTime.Now).RoundDownToMinute();    // 分下の値は切り捨てる

                        if (task_data[list.IndexOf(data)].Enable)
                        {
                            if (task.Cycle == "一回")
                            {
                                if (Tgt_dtime <= Now_dtime)
                                {
                                    FormCtrl_Wpf.Info_Message("過去又は現在の日時を有効にすることは出来ません。", 0);
                                    // 無効にします。
                                    task_data[list.IndexOf(data)].Enable = false;
                                    Read_Data();
                                    return;
                                }
                            }
                        }
                        XML_Task.Task.TaskList[TaskList_Select_No].Enable = task_data[list.IndexOf(data)].Enable;
                        Task_Ctrl.Get_Next_Task();      // 次に有効なタスクのリスト番号を取得する
                    }
                }
            }
        }

        private bool Select_Check()
        {
            if (TaskList_Select_No < 0)
            {
                return false;
            }
            return true;
        }
    }
}
