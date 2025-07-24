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
using static ClockPlus.Date_Picker;

namespace ClockPlus
{
    /// <summary>
    /// Form_Edit_Timer.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Edit_Timer : MetroWindow
    {
        private bool Form_Loaded_Flag = false;
        static public int Edit_No = -1;
        public Task task = new Task();

        public Form_Edit_Timer()
        {
            InitializeComponent();
            ComboBox_Power.Items.Add("スリープへ移行");
            ComboBox_Power.Items.Add("スリープから復帰");
            ComboBox_Power.Items.Add("再起動");
            ComboBox_Power.Items.Add("シャットダウン");
            ComboBox_Power.SelectedIndex = 0;

            for (int i = 0; i <= 23; i++)
            {
                ComboBox_Hour.Items.Add(i.ToString());
            }
            ComboBox_Hour.SelectedIndex = 0;

            for (int i = 0; i <= 59; i++)
            {
                ComboBox_Min.Items.Add(i.ToString());
            }
            ComboBox_Min.SelectedIndex = 1;

            ToggleSwitch_Alarm.IsOn = true;
            ToggleSwitch_App.IsOn = false;
            ToggleSwitch_Power.IsOn = false;
        }

        private void CheckBox_Sec_Clear()
        {
            CheckBox_1s.IsChecked = false;
            CheckBox_3s.IsChecked = false;
            CheckBox_5s.IsChecked = false;
            CheckBox_10s.IsChecked = false;
            CheckBox_15s.IsChecked = false;
            CheckBox_30s.IsChecked = false;
            CheckBox_45s.IsChecked = false;
            CheckBox_60s.IsChecked = false;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox_Sec_Clear();

            Read_Setting();
            Visibility_Change();

            Form_Loaded_Flag = true;
        }

        private void Form_Closed(object sender, EventArgs e)
        {

        }

        private void Read_Setting()
        {
            task = new Task();
            // 新規
            if (Edit_No == -1)
            {
                TextBox_Task_Name.Text = Task_Name_Gen();
                TextBox_Task_Name.IsEnabled = true;
                /*
                int Count = (Form_Timer_List.Timer_List.Count) + 1;
                TextBox_Task_Name.Text = "タイマー" + Count.ToString();
                */
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

                Time_ComboBox_Gen(task.Timer);
                CheckBox_Sec_Gen();

                ToggleSwitch_Alarm.IsOn = task.Sound.Enable;
                ToggleSwitch_App.IsOn = task.App.Enable;
                ToggleSwitch_Power.IsOn = task.Power.Enable;
            }
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

        private void CheckBox_Sec_Gen()
        {
            Form_Loaded_Flag = false;
            CheckBox_Sec_Clear();
            if (ComboBox_Hour.Text == "0")
            {
                switch (ComboBox_Min.Text)
                {
                    case "1":
                        CheckBox_1s.IsChecked = true;
                        break;
                    case "3":
                        CheckBox_3s.IsChecked = true;
                        break;
                    case "5":
                        CheckBox_5s.IsChecked = true;
                        break;
                    case "10":
                        CheckBox_10s.IsChecked = true;
                        break;
                    case "15":
                        CheckBox_15s.IsChecked = true;
                        break;
                    case "30":
                        CheckBox_30s.IsChecked = true;
                        break;
                    case "45":
                        CheckBox_45s.IsChecked = true;
                        break;
                }
            }
            else
            {
                if ((ComboBox_Hour.Text == "1") && (ComboBox_Min.Text == "0"))
                {
                    CheckBox_60s.IsChecked = true;
                }
            }
            Form_Loaded_Flag = true;
        }

        private string Task_Name_Gen()
        {
            string Name = "";
            bool Flag = false;
            for (int i = 0; i <= Form_Timer_List.Max_count; i++)
            {
                Name = "タイマー" + (i + 1).ToString();
                Flag = false;
                foreach (Task task in Form_Timer_List.Timer_List)
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
            foreach (Task task in Form_Timer_List.Timer_List)
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
            if (ToggleSwitch_Power.IsOn == true)
            {
                ComboBox_Power.Visibility = Visibility.Visible;
                ToggleSwitch_Alarm.IsEnabled = false;
                ToggleSwitch_App.IsEnabled = false;
                ToggleSwitch_Alarm.IsOn = false;
                ToggleSwitch_App.IsOn = false;
            }
            else
            {
                ComboBox_Power.Visibility = Visibility.Hidden;
                ToggleSwitch_Alarm.IsEnabled = true;
                ToggleSwitch_App.IsEnabled = true;
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
            Form_Loaded_Flag = true;
        }

        private void TimePic_Button_Click(object sender, RoutedEventArgs e)
        {
            Date_Picker win = new Date_Picker();

            // ComboBoxに表示されている日時をDatetime型に変換する
            DateTime dateTime = DateTime.MinValue;
            string Year = "0001";
            string Month ="01";
            string Day = "01";
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

        private void Alarm_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Form_Edit_Sound Form = new Form_Edit_Sound();
                Form.task_sound = task.Sound;
                Form.ShowDialog();
                task.Sound = Form.task_sound;
            }
        }

        private void App_Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Form_Edit_App Form = new Form_Edit_App();
                Form.task_app = task.App;
                Form.ShowDialog();
                task.App = Form.task_app;
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

        private void ToggleSwitch_Power_Toggled(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Visibility_Change();    // 設定の内容によってコントーロールの表示/非表示を切り替える
            }
        }

        private void ComboBox_Power_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            Task_Save();
            this.Close();
        }

        // どれか一つでも実行するタスクが選択されているかのチェック
        private bool TaskCheck()
        {
            if (ToggleSwitch_Alarm.IsOn) { return true; }
            if (ToggleSwitch_App.IsOn) { return true; }
            if (ToggleSwitch_Power.IsOn) { return true; }
            return false;
        }

        private bool Task_Save()
        {
            if ((ComboBox_Hour.Text == "0") && (ComboBox_Min.Text == "0"))
            {
                FormCtrl_Wpf.Info_Message("時間が設定されていません。");
                return false;
            }

            // どれか一つでも実行するタスクが選択されているかのチェック
            if (TaskCheck() == false)
            {
                FormCtrl_Wpf.Info_Message("実行する科目が選択されていません。");
                return false;
            }

            if ((Edit_No == -1) && (Task_Name_Check(TextBox_Task_Name.Text)))
            {
                FormCtrl_Wpf.Info_Message("同じ名称が既に存在します。\r\n名称を変更してください。");
                return false;
            }

            task.Type = "Timer";
            task.Name = TextBox_Task_Name.Text;

            // ComboBoxに表示されている日時をDatetime型に変換する
            string Year = "0001";
            string Month = "01";
            string Day = "01";
            string Hour = ComboBox_Hour.Text;
            string Min = ComboBox_Min.Text;
            task.Timer = Date_Picker.str_DateTime(Year, Month, Day, Hour, Min, "0");

            task.Sound.Enable = ToggleSwitch_Alarm.IsOn;
            task.App.Enable = ToggleSwitch_App.IsOn;
            task.Power.Enable = ToggleSwitch_Power.IsOn;
            task.Power.Mode = ComboBox_Power.Text;

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
            return true;
        }

        private void CheckBox_Timer_Sec_Checked(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                Form_Loaded_Flag = false;
                CheckBox_Sec_Clear();
                System.Windows.Controls.CheckBox checkbox = (System.Windows.Controls.CheckBox)sender;
                switch (checkbox.Name)
                {
                    case "CheckBox_1s":
                        CheckBox_1s.IsChecked = true;
                        ComboBox_Hour.Text = "0";
                        ComboBox_Min.Text = "1";
                        break;
                    case "CheckBox_3s":
                        CheckBox_3s.IsChecked = true;
                        ComboBox_Hour.Text = "0";
                        ComboBox_Min.Text = "3";
                        break;
                    case "CheckBox_5s":
                        CheckBox_5s.IsChecked = true;
                        ComboBox_Hour.Text = "0";
                        ComboBox_Min.Text = "5";
                        break;
                    case "CheckBox_10s":
                        CheckBox_10s.IsChecked = true;
                        ComboBox_Hour.Text = "0";
                        ComboBox_Min.Text = "10";
                        break;
                    case "CheckBox_15s":
                        CheckBox_15s.IsChecked = true;
                        ComboBox_Hour.Text = "0";
                        ComboBox_Min.Text = "15";
                        break;
                    case "CheckBox_30s":
                        CheckBox_30s.IsChecked = true;
                        ComboBox_Hour.Text = "0";
                        ComboBox_Min.Text = "30";
                        break;
                    case "CheckBox_45s":
                        CheckBox_45s.IsChecked = true;
                        ComboBox_Hour.Text = "0";
                        ComboBox_Min.Text = "45";
                        break;
                    case "CheckBox_60s":
                        CheckBox_60s.IsChecked = true;
                        ComboBox_Hour.Text = "1";
                        ComboBox_Min.Text = "0";
                        break;
                }
                ComboBox_Hour.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Hour);
                ComboBox_Min.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(ComboBox_Min);

                TimePic_Button.IsEnabled = false;
                ComboBox_Hour.IsEnabled = false;
                ComboBox_Min.IsEnabled = false;
                Form_Loaded_Flag = true;
            }
        }

        private void CheckBox_Timer_Sec_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {
                if (CheckBox_1s.IsChecked == true) { return; };
                if (CheckBox_3s.IsChecked == true) { return; };
                if (CheckBox_5s.IsChecked == true) { return; };
                if (CheckBox_10s.IsChecked == true) { return; };

                if (CheckBox_15s.IsChecked == true) { return; };
                if (CheckBox_30s.IsChecked == true) { return; };
                if (CheckBox_45s.IsChecked == true) { return; };
                if (CheckBox_60s.IsChecked == true) { return; };
                TimePic_Button.IsEnabled = true;
                ComboBox_Hour.IsEnabled = true;
                ComboBox_Min.IsEnabled = true;
            }
        }

        private void ComboBox_Hour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {

            }
        }

        private void ComboBox_Min_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form_Loaded_Flag)
            {

            }
        }
    }
}
