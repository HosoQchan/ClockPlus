using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClockPlus
{
    /// <summary>
    /// Window2.xaml の相互作用ロジック
    /// </summary>
    public partial class Date_Picker : MetroWindow
    {
        public class Int_Dtime_Item
        {
            public int Date_Y { get; set; }
            public int Date_M { get; set; }
            public int Date_D { get; set; }
            public string Week { get; set; }
            public int Time_H { get; set; }
            public int Time_M { get; set; }
            public int Time_S { get; set; }
        }
        public Int_Dtime_Item int_dtime = new Int_Dtime_Item();

        public class Str_Dtime_Item
        {
            public string Date { get; set; }
            public string Date_Y { get; set; }
            public string Date_M { get; set; }
            public string Date_D { get; set; }
            public string Week { get; set; }
            public string Time { get; set; }
            public string Time_H { get; set; }
            public string Time_M { get; set; }
            public string Time_S { get; set; }
        }
        public Str_Dtime_Item str_dtime = new Str_Dtime_Item();

        public class Week_Item
        {
            public bool Mon { get; set; }
            public bool Tue { get; set; }
            public bool Wed { get; set; }
            public bool Thu { get; set; }
            public bool Fri { get; set; }
            public bool Sat { get; set; }
            public bool Sun { get; set; }
        }
        public Week_Item Week = new Week_Item();

        public int Dpick_Mode = 0;
        public DateTime Date_Start;
        public DateTime Date_End;
        public DateTime Dpick_date;

        private DateTime Dpick_date_Backup;

        public Date_Picker()
        {
            InitializeComponent();
            str_dtime = DateTime_str(Dpick_date);
        }

        private void Form_Load(object sender, RoutedEventArgs e)
        {
            _Calender.DisplayDateStart = Date_Start;
            _Calender.DisplayDateEnd = Date_End;
            _Calender.DisplayDate = Dpick_date;
            _Calender.SelectedDate = Dpick_date;

            if (Dpick_Mode == 0)
            {
                // 単一の日付を選択
                _Calender.SelectionMode = CalendarSelectionMode.SingleDate;
            }
            else
            {
                // 選択を許可しない
                _Calender.SelectionMode = CalendarSelectionMode.None;
            }
        }

        private void Form_Closed(object sender, EventArgs e)
        {

        }


        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            Dpick_date = (DateTime)_Calender.SelectedDate;
            str_dtime = DateTime_str(Dpick_date);
            this.Close();
        }

        private void NowDate_Button_Click(object sender, RoutedEventArgs e)
        {
            _Calender.DisplayDate = Dpick_date;
            _Calender.SelectedDate = DateTime.Now;
        }

        // int型をDatetime型に変換する
        static public DateTime Int_DateTime(int Year, int Month, int Day, int Hour, int Min, int Sec)
        {
            string Datetime = Year.ToString("0000") + "/";
            Datetime = Datetime + Month.ToString("00") + "/";
            Datetime = Datetime + Day.ToString("00") + " ";
            Datetime = Datetime + Hour.ToString("00") + ":";
            Datetime = Datetime + Min.ToString("00") + ":";
            Datetime = Datetime + Sec.ToString("00");
            return DateTime.Parse(Datetime);
        }

        // string型をDatetime型に変換する
        static public DateTime str_DateTime(string Year,string Month,string Day,string Hour,string Min,string Sec)
        {
            int Date_Y = int.Parse(Year);
            int Date_M = int.Parse(Month);
            int Date_D = int.Parse(Day);
            int Time_H = int.Parse(Hour);
            int Time_M = int.Parse(Min);
            int Time_S = int.Parse(Sec);

            string Datetime = Date_Y.ToString("0000") + "/";
            Datetime = Datetime + Date_M.ToString("00") + "/";
            Datetime = Datetime + Date_D.ToString("00") + " ";
            Datetime = Datetime + Time_H.ToString("00") + ":";
            Datetime = Datetime + Time_M.ToString("00") + ":";
            Datetime = Datetime + Time_S.ToString("00");
            return DateTime.Parse(Datetime);
        }
        // Datetime型を分割し、String型に変換する
        static public Str_Dtime_Item DateTime_str(DateTime Dt)
        {
            Str_Dtime_Item str_dtime = new Str_Dtime_Item();
            str_dtime.Date = Dt.ToString("yyyy/MM/dd");
            str_dtime.Date_Y = Dt.ToString("yyyy");
            str_dtime.Date_M = Dt.ToString("MM");
            str_dtime.Date_D = Dt.ToString("dd");
            str_dtime.Week = Dt.ToString("ddd");
            str_dtime.Time = Dt.ToString("HH:mm");
            str_dtime.Time_H = Dt.ToString("HH");
            str_dtime.Time_M = Dt.ToString("mm");
            str_dtime.Time_S = Dt.ToString("ss");
            return str_dtime;
        }

        // Datetime型を分割し、int型に変換する
        static public Int_Dtime_Item DateTime_int(DateTime Dt)
        {
            Int_Dtime_Item int_dtime = new Int_Dtime_Item();
            int_dtime.Date_Y = int.Parse(Dt.ToString("yyyy"));
            int_dtime.Date_M = int.Parse(Dt.ToString("MM"));
            int_dtime.Date_D = int.Parse(Dt.ToString("dd"));
            int_dtime.Week = Dt.ToString("ddd");
            int_dtime.Time_H = int.Parse(Dt.ToString("HH"));
            int_dtime.Time_M = int.Parse(Dt.ToString("mm"));
            int_dtime.Time_S = int.Parse(Dt.ToString("ss"));
            return int_dtime;
        }
    }
}
