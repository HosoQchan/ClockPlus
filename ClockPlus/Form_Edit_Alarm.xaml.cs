using ControlzEx.Standard;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
using System.Xml.Serialization;
using static ClockPlus.Date_Picker;
using static MaterialDesignThemes.Wpf.Theme;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ClockPlus
{
    public partial class Form_Edit_Alarm : MetroWindow
    {
        private bool Form_Loaded_Flag = false;
        static public int Edit_No = -1;
        public Task task = new Task();

        public Form_Edit_Alarm()
        {
            InitializeComponent();
            ComboBox_Cycle.Items.Add("一回");
            ComboBox_Cycle.Items.Add("毎日");
            ComboBox_Cycle.Items.Add("毎週");
            ComboBox_Cycle.Items.Add("毎月");
            ComboBox_Cycle.Items.Add("毎年");
            ComboBox_Cycle.SelectedIndex = 0;

            ComboBox_Power.Items.Add("スリープへ移行");
            ComboBox_Power.Items.Add("スリープから復帰");
            ComboBox_Power.Items.Add("再起動");
            ComboBox_Power.Items.Add("シャットダウン");
            ComboBox_Power.SelectedIndex = 0;

            int Year = (DateTime.Now).Year;
            for (int i = Year; i <= Year + 10; i++)
            {
                ComboBox_Year.Items.Add(i.ToString());
            }

            for (int i = 1; i <= 12; i++)
            {
                ComboBox_Month.Items.Add(i.ToString());
            }

            for (int i = 0; i <= 23; i++)
            {
                ComboBox_Hour.Items.Add(i.ToString());
            }

            for (int i = 0; i <= 59; i++)
            {
                ComboBox_Min.Items.Add(i.ToString());
            }

            for (int i = 1; i <= 31; i++)
            {
                ComboBox_Day.Items.Add(i.ToString());
            }

            Date_ComboBox_Gen(DateTime.Now);
            Time_ComboBox_Gen(DateTime.Now);

            ToggleSwitch_Alarm.IsOn = true;
            ToggleSwitch_App.IsOn = false;
            ToggleSwitch_Balloon.IsOn = false;
            ToggleSwitch_Power.IsOn = false;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            Read_Setting();
            Visibility_Change();

            Form_Loaded_Flag = true;
        }
        private void Form_Closed(object sender, EventArgs e)
        {
           
        }

        private void Read_Setting()
        {
            Form_Loaded_Flag = false;

            task = new Task();
            // 新規
            if (Edit_No == -1)
            {
                TextBox_Task_Name.Text = Task_Name_Gen();
                TextBox_Task_Name.IsEnabled = true;
                if (task.Sound.FileName == "")
                {
                    task.Sound.FileName = XML_Main.Sound_Dir + "\\Alarm.wav";
                }
            }
            // 編集
            else
            {
                task = XML_Task.Task.TaskList[Edit_No];

                TextBox_Task_Name.Text = task.Name;
                TextBox_Task_Name.IsEnabled = false;

                ComboBox_Cycle.Text = task.Cycle;
                ComboBox_Cycle.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Cycle);

                Int_Dtime_Item dtime_Item = new Int_Dtime_Item();
                dtime_Item = Date_Picker.DateTime_int(task.DateTime);

                ComboBox_Year.Text = dtime_Item.Date_Y.ToString();
                ComboBox_Month.Text = dtime_Item.Date_M.ToString();
                ComboBox_Day.Text = dtime_Item.Date_D.ToString();
                ComboBox_Hour.Text = dtime_Item.Time_H.ToString();
                ComboBox_Min.Text = dtime_Item.Time_M.ToString();

                ComboBox_Year.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Year);
                ComboBox_Month.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Month);
                ComboBox_Day.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Day);
                ComboBox_Hour.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Hour);
                ComboBox_Min.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Min);

                switch (ComboBox_Cycle.Text)
                {
                    case "一回":
                        Int_Dtime_Item dtime_Item1 = new Int_Dtime_Item();
                        Int_Dtime_Item dtime_Item2 = new Int_Dtime_Item();
                        dtime_Item1 = Date_Picker.DateTime_int(DateTime.Now);
                        dtime_Item2 = Date_Picker.DateTime_int((DateTime.Now).AddDays(1));

                        CheckBox_Now_Day.IsChecked = false;
                        CheckBox_Tomorrow.IsChecked = false;
                        // 今日？
                        if ((dtime_Item.Date_Y == dtime_Item1.Date_Y) &&
                           (dtime_Item.Date_M == dtime_Item1.Date_M) &&
                           (dtime_Item.Date_D == dtime_Item1.Date_D))
                        {
                            if ((task.DateTime.TimeOfDay) >= (DateTime.Now).TimeOfDay)
                            {
                                CheckBox_Now_Day.IsChecked = true;
                            }
                        }
                        else
                        {
                            // 明日？
                            if ((dtime_Item.Date_Y == dtime_Item2.Date_Y) &&
                                (dtime_Item.Date_M == dtime_Item2.Date_M) &&
                                (dtime_Item.Date_D == dtime_Item2.Date_D))
                            {
                                CheckBox_Tomorrow.IsChecked = true;
                            }
                        }
                        break;
                }

                Mon_CheckBox.IsChecked = task.Week_List.Mon;
                Tue_CheckBox.IsChecked = task.Week_List.Tue;
                Wed_CheckBox.IsChecked = task.Week_List.Wed;
                Thu_CheckBox.IsChecked = task.Week_List.Thu;
                Fri_CheckBox.IsChecked = task.Week_List.Fri;
                Sat_CheckBox.IsChecked = task.Week_List.Sat;
                Sun_CheckBox.IsChecked = task.Week_List.Sun;

                ToggleSwitch_Alarm.IsOn = task.Sound.Enable;
                ToggleSwitch_App.IsOn = task.App.Enable;
                ToggleSwitch_Balloon.IsOn = task.Message.Enable;
                ToggleSwitch_Power.IsOn = task.Power.Enable;

                ComboBox_Power.Text = task.Power.Mode;
                ComboBox_Power.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Power);
            }
            Form_Loaded_Flag = true;
        }

        private string Task_Name_Gen()
        {
            string Name = "";
            bool Flag = false;
            for (int i = 0; i <= Form_Alarm_List.Max_count; i++)
            {
                Name = "アラーム" + (i + 1).ToString();
                Flag = false;
                foreach(Task task in Form_Alarm_List.Alarm_List)
                {
                    if (task.Name == Name)
                    {
                        Flag = true;
                        break;
                    }
                }
                if (!Flag)
                {
                    break;
                }
            }
            return Name;
        }

        private bool Task_Name_Check(string Name)
        {
            foreach (Task task in Form_Alarm_List.Alarm_List)
            {
                if (task.Name == Name)
                {
                    return true;
                }
            }
            return false;
        }

        // 回数設定の内容によってコントーロールの表示/非表示を切り替える
        private void Visibility_Change()
        {
            Form_Loaded_Flag = false;

            DatePic_Button.IsEnabled = true;
            ComboBox_Year.IsEnabled = true;
            ComboBox_Month.IsEnabled = true;
            ComboBox_Day.IsEnabled = true;

            switch (ComboBox_Cycle.SelectedItem.ToString())
            {
                case "一回":
                    Grid_Once.Visibility = Visibility.Visible;          // 表示する
                    if ((!(bool)CheckBox_Now_Day.IsChecked) && (!(bool)CheckBox_Tomorrow.IsChecked))
                    {
                        DatePic_Button.IsEnabled = true;
                        ComboBox_Year.IsEnabled = true;
                        ComboBox_Month.IsEnabled = true;
                        ComboBox_Day.IsEnabled = true;
                    }
                    else
                    {
                        DatePic_Button.IsEnabled = false;
                        ComboBox_Year.IsEnabled = false;
                        ComboBox_Month.IsEnabled = false;
                        ComboBox_Day.IsEnabled = false;
                    }

                    Week_Grid.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    Date_Grid.Visibility = Visibility.Visible;          // 表示する
                    Year_Grid.Visibility = Visibility.Visible;          // 表示する
                    Month_Grid.Visibility = Visibility.Visible;         // 表示する
                    break;
                case "毎日":
                    Grid_Once.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    Week_Grid.Visibility = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)
                    Date_Grid.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    break;
                case "毎週":
                    Grid_Once.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    Week_Grid.Visibility = Visibility.Visible;          // 表示する
                    Date_Grid.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    break;
                case "毎月":
                    Grid_Once.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    Week_Grid.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    Date_Grid.Visibility = Visibility.Visible;          // 表示する
                    Year_Grid.Visibility = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)
                    Month_Grid.Visibility = Visibility.Collapsed;       // 非表示にする(コンポーネントの場所を詰める)
                    break;
                case "毎年":
                    Grid_Once.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    Week_Grid.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
                    Date_Grid.Visibility = Visibility.Visible;          // 表示する
                    Year_Grid.Visibility = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)
                    Month_Grid.Visibility = Visibility.Visible;         // 表示する
                    break;
            }

            if (ToggleSwitch_Power.IsOn == true)
            {
                ComboBox_Power.Visibility = Visibility.Visible;
                ToggleSwitch_Alarm.IsEnabled = false;
                ToggleSwitch_Alarm.IsOn = false;
                ToggleSwitch_App.IsEnabled = false;
                ToggleSwitch_App.IsOn = false;
                ToggleSwitch_Balloon.IsEnabled = false;
                ToggleSwitch_Balloon.IsOn = false;
            }
            else
            {
                ComboBox_Power.Visibility = Visibility.Hidden;
                ToggleSwitch_Alarm.IsEnabled = true;
                ToggleSwitch_App.IsEnabled = true;
                ToggleSwitch_Balloon.IsEnabled = true;
            }

            if (ToggleSwitch_Alarm.IsOn == true)
            {
                Alarm_Setting_Button.Visibility = Visibility.Visible;   // 表示する
            }
            else
            {
                Alarm_Setting_Button.Visibility = Visibility.Hidden;    // 非表示にする(コンポーネントの場所はそのまま)
            }

            if (ToggleSwitch_App.IsOn == true)
            {
                App_Setting_Button.Visibility = Visibility.Visible;     // 表示する
            }
            else
            {
                App_Setting_Button.Visibility = Visibility.Hidden;      // 非表示にする(コンポーネントの場所はそのまま)
            }

            if (ToggleSwitch_Balloon.IsOn == true)
            {
                Balloon_Setting_Button.Visibility = Visibility.Visible; // 表示する
            }
            else
            {
                Balloon_Setting_Button.Visibility = Visibility.Hidden;  // 非表示にする(コンポーネントの場所はそのまま)
            }

            Form_Loaded_Flag = true;
        }

        // ComboBoxの対象月からComboBoxの日付(リスト)を作成
        private void Day_ComboBox_Gen()
        {
            string Text = ComboBox_Day.SelectedItem.ToString();
            ComboBox_Day.Items.Clear();

            int Day;
            // 回数が月指定有の場合は、ComboBoxの対象月からComboBoxの日付(リスト)を作成する
            if ((ComboBox_Cycle.SelectedItem == "一回") || (ComboBox_Cycle.SelectedItem == "毎年"))
            {
                Day = DateTime.DaysInMonth(int.Parse(ComboBox_Year.SelectedItem.ToString()), int.Parse(ComboBox_Month.SelectedItem.ToString()));
            }
            // 回数が月指定無しの場合は、３１日分の日付(リスト)を作成する
            else
            {
                Day = 31;
            }
            for (int i = 1; i <= Day; i++)
            {
                ComboBox_Day.Items.Add(i.ToString());
            }
            ComboBox_Day.Text = Text;
            ComboBox_Day.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Day);
        }

        private void ComboBox_Cycle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Form_Loaded_Flag = false;
                CheckBox_Now_Day.IsChecked = false;
                CheckBox_Tomorrow.IsChecked = false;
                Form_Loaded_Flag = true;
                Day_ComboBox_Gen();         // ComboBoxの対象月からComboBoxの日付(リスト)を作成
                Visibility_Change();        // 設定の内容によってコントーロールの表示/非表示を切り替える
                
            }          
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            // どれか一つでも曜日が選択されているかのチェック
            if (Week_Check() == false)
            {
                FormCtrl_Wpf.Info_Message("曜日が選択されていません。", 0);
                return;
            }

            // どれか一つでも実行するタスクが選択されているかのチェック
            if (TaskCheck() == false)
            {
                FormCtrl_Wpf.Info_Message("実行する科目が選択されていません。", 0);
                return;
            }

            if ((Edit_No == -1) && (Task_Name_Check(TextBox_Task_Name.Text)))
            {
                FormCtrl_Wpf.Info_Message("同じ名称が既に存在します。\r\n名称を変更してください。", 0);
                return;
            }

            string Cycle = (string)ComboBox_Cycle.SelectedItem;

            int Year = int.Parse(ComboBox_Year.Text);
            int Month = int.Parse(ComboBox_Month.Text);
            int Day = int.Parse(ComboBox_Day.Text);
            int Hour = int.Parse(ComboBox_Hour.Text);
            int Min = int.Parse(ComboBox_Min.Text);

            Debug.WriteLine("保存時 = " + ComboBox_Min.Text);

            DateTime Tgt_dtime = Int_DateTime(Year, Month, Day, Hour, Min, 0);
            DateTime Now_dtime = (DateTime.Now).RoundDownToMinute();    // 分下の値は切り捨てる

            if (Cycle == "一回")
            {
                if (Tgt_dtime <= Now_dtime)
                {
                    FormCtrl_Wpf.Info_Message("過去又は現在の日時を設定することは出来ません。", 0);
                    return;
                }
            }
            
            task.Enable = true;
            task.Type = "Alarm";
            task.Name = TextBox_Task_Name.Text;

            task.Cycle = Cycle;
            task.DateTime = Tgt_dtime;

            task.Week_List.Mon = (bool)Mon_CheckBox.IsChecked;
            task.Week_List.Tue = (bool)Tue_CheckBox.IsChecked;
            task.Week_List.Wed = (bool)Wed_CheckBox.IsChecked;
            task.Week_List.Thu = (bool)Thu_CheckBox.IsChecked;
            task.Week_List.Fri = (bool)Fri_CheckBox.IsChecked;
            task.Week_List.Sat = (bool)Sat_CheckBox.IsChecked;
            task.Week_List.Sun = (bool)Sun_CheckBox.IsChecked;

            task.Sound.Enable = ToggleSwitch_Alarm.IsOn;
            task.App.Enable = ToggleSwitch_App.IsOn;
            task.Power.Enable = ToggleSwitch_Power.IsOn;
            task.Power.Mode = ComboBox_Power.Text;

            // 現在の日時から、指定された回数の中で最も近い日時を求める
            task = Task_Ctrl.Alarm_Dtime_Gen(task);

            // 新規
            if (Edit_No == -1)
            {
                XML_Task.Task.TaskList.Add(task);
            }
            // 編集
            else
            {
                XML_Task.Task.TaskList[Edit_No] = task;
            }
            Task_Ctrl.Sort_Task_List();     // リストを日時の昇順にソートする
            Task_Ctrl.Get_Next_Task();      // 次に有効なタスクのリスト番号を取得する
            this.Close();
        }

        // どれか一つでも曜日が選択されているかのチェック
        private bool Week_Check()
        {
            if (ComboBox_Cycle.SelectedItem == "毎週")
            {
                if (Mon_CheckBox.IsChecked == true) { return true; }
                if (Tue_CheckBox.IsChecked == true) { return true; }
                if (Wed_CheckBox.IsChecked == true) { return true; }
                if (Thu_CheckBox.IsChecked == true) { return true; }
                if (Fri_CheckBox.IsChecked == true) { return true; }
                if (Sat_CheckBox.IsChecked == true) { return true; }
                if (Sun_CheckBox.IsChecked == true) { return true; }
                return false;
            }
            return true;
        }

        // どれか一つでも実行するタスクが選択されているかのチェック
        private bool TaskCheck()
        {
            if (ToggleSwitch_Alarm.IsOn) { return true; }
            if (ToggleSwitch_Balloon.IsOn) { return true; }
            if (ToggleSwitch_App.IsOn) { return true; }
            if (ToggleSwitch_Power.IsOn) { return true; }
            return false;
        }

        private void Year_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {

            }
        }

        private void Month_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {

                Day_ComboBox_Gen();         // ComboBoxの対象月からComboBoxの日付(リスト)を作成
            }
        }

        private void Day_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {


            }
        }

        private void Now_Dtime_Button_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void DatePic_Button_Click(object sender, RoutedEventArgs e)
        {

            Date_Picker Form = new Date_Picker();

            // ComboBoxに表示されている日時をDatetime型に変換する
            DateTime dateTime;
            string Year = ComboBox_Year.Text;
            string Month = ComboBox_Month.Text;
            string Day = ComboBox_Day.Text;
            string Hour = ComboBox_Hour.Text;
            string Min = ComboBox_Min.Text;
            dateTime = Date_Picker.str_DateTime(Year, Month, Day, Hour, Min, "0");

            Form.Date_Start = DateTime.Now;
            Form.Date_End = (DateTime.Now).AddYears(1);
            Form.Dpick_date = dateTime;
            Form.ShowDialog();

            Date_ComboBox_Gen(Form.Dpick_date);
        }

        private void TimePic_Button_Click(object sender, RoutedEventArgs e)
        {
            Date_Picker win = new Date_Picker();

            // ComboBoxに表示されている日時をDatetime型に変換する
            DateTime dateTime;
            string Year = ComboBox_Year.Text;
            string Month = ComboBox_Month.Text;
            string Day = ComboBox_Day.Text;
            string Hour = ComboBox_Hour.Text;
            string Min = ComboBox_Min.Text;
            dateTime = Date_Picker.str_DateTime(Year, Month, Day, Hour, Min, "0");

            Time_Picker.Tpick_Time = dateTime;
            Time_Picker.Option = true;
            Time_Picker _Time_Picker = new Time_Picker();
            _Time_Picker.ShowDialog();

            // 設定日時の表示
            TimeSpan Ts = Time_Picker.Tpick_Time.TimeOfDay;
            DateTime Dt = dateTime.Date;
            Dt = Dt + Ts;
            Time_ComboBox_Gen(Dt);
        }

        // 設定日付を表示に反映させる
        private void Date_ComboBox_Gen(DateTime dateTime)
        {
            Form_Loaded_Flag = false;

            // Datetime型を分割し、int型に変換する
            Int_Dtime_Item int_dtime = new Int_Dtime_Item();
            int_dtime = Date_Picker.DateTime_int(dateTime);
            ComboBox_Year.Text = int_dtime.Date_Y.ToString();
            ComboBox_Month.Text = int_dtime.Date_M.ToString();
            ComboBox_Day.Text = int_dtime.Date_D.ToString();
            
            ComboBox_Year.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Year);
            ComboBox_Month.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Month);
            ComboBox_Day.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Day);

            Day_ComboBox_Gen();     // ComboBoxの対象月からComboBoxの日付(リスト)を作成
            Form_Loaded_Flag = true;
        }

        // 設定時刻を表示に反映させる
        private void Time_ComboBox_Gen(DateTime dateTime)
        {
            Form_Loaded_Flag = false;
            Int_Dtime_Item int_dtime = new Int_Dtime_Item();
            int_dtime = Date_Picker.DateTime_int(dateTime);

            ComboBox_Hour.Text = int_dtime.Time_H.ToString();
            ComboBox_Min.Text = int_dtime.Time_M.ToString();
            ComboBox_Hour.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Hour);
            ComboBox_Min.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Min);
            Form_Loaded_Flag = true;
        }

        private void ComboBox_Power_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {

            }
        }

        private void Alarm_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Form_Edit_Sound Form = new Form_Edit_Sound();
            Form.task_sound = task.Sound;
            Form.ShowDialog();
            task.Sound = Form.task_sound;
        }

        private void App_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Form_Edit_App Form = new Form_Edit_App();
            Form.task_app = task.App;
            Form.ShowDialog();
            task.App = Form.task_app;
        }

        private void Balloon_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Form_Edit_Message Form = new Form_Edit_Message();
            Form.task_message = task.Message;
            Form.ShowDialog();
            task.Message = Form.task_message;
        }

        private void ToggleSwitch_Power_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Visibility_Change();    // 設定の内容によってコントーロールの表示/非表示を切り替える
            }
        }

        private void ToggleSwitch_Alarm_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Visibility_Change();    // 設定の内容によってコントーロールの表示/非表示を切り替える
            }
        }

        private void ToggleSwitch_App_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Visibility_Change();    // 設定の内容によってコントーロールの表示/非表示を切り替える
            }
        }
        private void ToggleSwitch_Balloon_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Visibility_Change();    // 設定の内容によってコントーロールの表示/非表示を切り替える
            }
        }

        private void CheckBox_Now_Day_Checked(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                if ((bool)CheckBox_Now_Day.IsChecked)
                {
                    CheckBox_Tomorrow.IsChecked = false;
                }
                Date_ComboBox_Gen(DateTime.Now);
                Time_ComboBox_Gen(DateTime.Now);
                Visibility_Change();        // 設定の内容によってコントーロールの表示/非表示を切り替える
            }
        }
        private void CheckBox_Now_Day_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Visibility_Change();        // 設定の内容によってコントーロールの表示/非表示を切り替える
            }
        }

        private void CheckBox_Tomorrow_Checked(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                if ((bool)CheckBox_Tomorrow.IsChecked)
                {
                    CheckBox_Now_Day.IsChecked = false;
                }
                Date_ComboBox_Gen((DateTime.Now).AddDays(1));
                Time_ComboBox_Gen(DateTime.Now);
                Visibility_Change();        // 設定の内容によってコントーロールの表示/非表示を切り替える
            }
        }
        private void CheckBox_Tomorrow_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Visibility_Change();        // 設定の内容によってコントーロールの表示/非表示を切り替える
            }
        }

        private void ComboBox_Hour_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                // 半角英数字のみであるかを判定する
                if (!Etc.Keychr_chk(e.Text, @"^[0-9]+"))
                {
                    e.Handled = true;
                    return;
                }
                if (ComboBox_Hour.Text.Length > 2)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void ComboBox_Hour_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
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
