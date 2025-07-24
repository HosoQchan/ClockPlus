using MahApps.Metro.Controls;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClockPlus
{
    /// <summary>
    /// About.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_About : MetroWindow
    {
        public Form_About()
        {
            InitializeComponent();
            Grid_NewVer.Visibility = Visibility.Hidden;           // 非表示にする(コンポーネントの場所はそのまま)
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            PART_Image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/icon/ClockPlus.ico"));
            Label_Now_Ver.Content = This_Ver();

        }
        private void Form_Closed(object sender, EventArgs e)
        {

        }

        private string This_Ver()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            var n1 = assemblyName.Name;    // "アセンブリ名"
            var v1 = assemblyName.Version; // "1.0.0.0"
            return v1.ToString();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_GitHub_Click(object sender, RoutedEventArgs e)
        {
            Link_Display("https://github.com/HosoQchan/ClockPlus");
        }
        private void Button_License_Click(object sender, RoutedEventArgs e)
        {
            Link_Display("https://licenses.opensource.jp/MIT/MIT.html");
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Link_Display(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        // リンク先を開く
        private void Link_Display(string URL)
        {
            //レジストリキー（HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice）
            //を、新規作成されない様に開く
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                            @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice", false);

            //レジストリ"ProgId"の値を読み取り
            string progId = regkey.GetValue("ProgId").ToString();

            //"progID"の書かれているキーを開く
            regkey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(
                            string.Format(@"{0}\shell\open\command", progId), false);
            string Browser = regkey.GetValue(null).ToString();
            // commandには"C:\Program Files\Google\Chrome\Application\chrome.exe" --single-argument %1　のように
            //ファイパス以降に不要な文字が入るので　2つめの'”'を探してそれ以降を削除する
            string gomiStr = Browser.Substring(Browser.IndexOf("\"", Browser.IndexOf("\"") + 1) + 1);
            Browser = Browser.Replace(gomiStr, "");

            ProcessStartInfo info = new ProcessStartInfo();
            info.UseShellExecute = true;
            // デフォルトブラウザ
            info.FileName = Browser;
            // URLを加える
            info.Arguments = URL;
            Process p = new Process();
            p.StartInfo = info;

            try
            {
                p.Start();
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                FormCtrl_Wpf.Error_Message("リンクが開けませんでした。");
            }
            catch (Exception ex)
            {
                FormCtrl_Wpf.Error_Message("リンクが開けませんでした。");
            }
        }
    }
}
