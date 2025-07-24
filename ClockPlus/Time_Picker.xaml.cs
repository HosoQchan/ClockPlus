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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace ClockPlus
{
    public partial class Time_Picker : MetroWindow
    {
        static public DateTime Tpick_Time;
        static public bool Is24Hours = true;
        static public bool Option = false;

        private DateTime Tpick_Time_Backup;

        public Time_Picker()
        {
            InitializeComponent();

            // 一時的にテーマをライトモードにする
//            Theme_Ctrl.Change_Theme("Light", XML_Main_IO.Main.Theme.Color);

            Option_Grid.Visibility = Visibility.Collapsed;      //  非表示にする（コンポーネントの場所を詰める）
        }
        private void Form_Load(object sender, RoutedEventArgs e)
        {
            _Clock.Time = Tpick_Time;
            Tpick_Time_Backup = Tpick_Time;
            if (Option)
            {
                Option_Grid.Visibility = Visibility.Visible;    // 表示する
                _Clock.Is24Hours = Is24Hours;
                H24_CheckBox.IsChecked = Is24Hours;
            }
            else
            {
                _Clock.Is24Hours = true;
            }
        }

        private void H24_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _Clock.Is24Hours = true;
        }

        private void H24_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _Clock.Is24Hours = false;
        }

        private void NowTime_Button_Click(object sender, RoutedEventArgs e)
        {
            _Clock.Time = DateTime.Now;
        }

        public TimePick Timepick { get; set; }
        public class TimePick
        {
            public DateTime Time { get; set; }
        }

        private void Cansel_Botton_Click(object sender, RoutedEventArgs e)
        {
            Tpick_Time = Tpick_Time_Backup;
            this.Close();
        }

        private void Ok_Botton_Click(object sender, RoutedEventArgs e)
        {
            Tpick_Time = _Clock.Time;
            this.Close();
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            // テーマを再適用する
//            Theme_Ctrl.Change_Theme(XML_Main_IO.Main.Theme.Mode, XML_Main_IO.Main.Theme.Color);
        }
    }
}
