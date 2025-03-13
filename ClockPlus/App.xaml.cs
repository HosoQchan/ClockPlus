using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ClockPlus
{
    public partial class App : System.Windows.Application
    {
        private System.Threading.Mutex mutex = new System.Threading.Mutex(false, "ApplicationName");

        static public string Title = null;              // アプリ名
        static public string ExeFile = null;            // exeファイル名
        static public string LnkFile = null;            // リンク名

        ContextMenuStrip menu = new ContextMenuStrip();
        private NotifyIcon notifyIcon = new NotifyIcon();

        // フォーム参照のためのポインタ
        static public Analog_Form _Analog_Form = new Analog_Form();
        static public Digital1_Form _Digital1_Form = new Digital1_Form();
        static public Digital2_Form _Digital2_Form = new Digital2_Form();

        private string jsFile = null;                           // スクリプトファイル名

        private System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        private bool T500ms = false;
        private int Signal_Phase = 0;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // ミューテックスの所有権を要求
            if (!mutex.WaitOne(0, false))
            {
                // 既に起動しているため終了させる
                System.Windows.MessageBox.Show("ApplicationName は既に起動しています。", "二重起動", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                mutex.Close();
                mutex = null;
                this.Shutdown();
            }

            // プロジェクト＞プロパティ＞アセンブリ情報　で指定した「タイトル」を取得
            var assembly = Assembly.GetExecutingAssembly();
            var attribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
            Title = attribute.Title;

            // コンテキストメニュー
            ToolStripMenuItem Menu_Setting = new ToolStripMenuItem();
            Menu_Setting.Text = "基本設定";
            Menu_Setting.Click += (sender, e) => Menu_Setting_Click();

            ToolStripMenuItem Menu_Info = new ToolStripMenuItem();
            Menu_Info.Text = "About";
            Menu_Info.Click += (sender, e) => Menu_About_Click();

            ToolStripMenuItem Menu_Close = new ToolStripMenuItem();
            Menu_Close.Text = "終了";
            Menu_Close.Click += (sender, e) => Menu_Close_Click();

            menu.Items.Add(Menu_Setting);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(Menu_Info);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(Menu_Close);

            //タスクトレイを作る
            string uri = ".\\Resources\\Clock.ico";
            Icon icon = new System.Drawing.Icon(uri);
            notifyIcon.Visible = true;
            notifyIcon.Icon = icon;
            notifyIcon.Text = Title;
            notifyIcon.ContextMenuStrip = menu;

            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseClick);
            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseDoubleClick);

            // メイン設定を読み込む
            XML_Main_IO _XML_Main_IO = new XML_Main_IO();
            _XML_Main_IO.XML_Main_Create();

            // 時計表示の初期化
            _Analog_Form.Show();
            _Digital1_Form.Show();
            _Digital2_Form.Show();

            // タイマーをスタート
            timer1.Tick += new EventHandler(TimerMethod);
            timer1.Interval = 500;
            timer1.Enabled = true;

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (mutex != null)
            {
                // イベントハンドラを削除
                timer1.Enabled = false;             // タイマーを停止
                System.Windows.Forms.Application.ApplicationExit -= new EventHandler(TimerMethod);

                XML_Main_IO _XML_Main_IO = new XML_Main_IO();
                _XML_Main_IO.XML_Main_save();

                mutex.ReleaseMutex();
                mutex.Close();
            }
        }

        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                // マウスの左ボタン
                case MouseButtons.Left:
                    break;
                // マウスの中央ボタン
                case MouseButtons.Middle:
                    break;
                // マウスの右ボタン
                case MouseButtons.Right:
                    break;
            }
        }
        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (e.Clicks == 2))
            {
                WindowManager.ShowOrActivate<Menu_Win>();
                /*
                Menu_Win menu_Win = new Menu_Win();
                menu_Win.ShowDialog();
                */
            }
        }

        private void Menu_Setting_Click()
        {
            WindowManager.ShowOrActivate<Setting_Main_Win>();
            /*
            Setting_Main_Win main_Win = new Setting_Main_Win();
            main_Win.ShowDialog();
            */
        }

        private void Menu_About_Click()
        {
            WindowManager.ShowOrActivate<About_Win>();
            /*
            About_Win about_Win = new About_Win();
            about_Win.ShowDialog();
            */
        }

        private void Menu_Close_Click()
        {
            System.Windows.Application.Current.Shutdown();
        }

        // タイマーイベント
        private void TimerMethod(object sender, EventArgs e)
        {
            T500ms = !T500ms;
            if (T500ms)
            // 1s間隔の処理
            {
                Notify_disp();                      // タスクトレイ表示

                if (Analog_Form.Analog_Disp_Check())
                {
                    FormCtrl_Net.valid_Form(_Analog_Form);             // フォームを表示
                    _Analog_Form.Analog_Disp_Drow();                   // アナログ時計の描画更新
                }
                else { FormCtrl_Net.Hide_Form(_Analog_Form); }         // フォームを非表示

                if (Digital_Drow.Digital_Disp_Check(0))
                {
                    FormCtrl_Net.valid_Form(_Digital1_Form);           // フォームを表示
                    _Digital1_Form.Disp1_Drow();                       // デジタル時計１を描画
                }
                else { FormCtrl_Net.Hide_Form(_Digital1_Form); }       // フォームを非表示

                if (Digital_Drow.Digital_Disp_Check(1))
                {
                    FormCtrl_Net.valid_Form(_Digital2_Form);           // フォームを表示
                    _Digital2_Form.Disp2_Drow();                       // デジタル時計２を描画
                }
                else { FormCtrl_Net.Hide_Form(_Digital2_Form); }       // フォームを非表示
            }
            Signal_Event();                     // 時報処理
        }

        // タスクトレイ表示
        private void Notify_disp()
        {
            DateTime Now = DateTime.Now;
            notifyIcon.Text = Title + Now.ToString("\n現在の日時\nyyyy年MM月dd日(ddd) HH:mm:ss\n\n");
        }

        // 時報処理
        public void Signal_Event()
        {
            if (XML_Main_IO.Main.Signal.Enable)
            {
                int Cycle = XML_Main_IO.Main.Signal.Cycle;
                int Now_Sec = (DateTime.Now).Second;
                int Now_Min = (DateTime.Now).Minute;

                if (Now_Sec >= 52)
                {
                    // 時報
                    if (Now_Sec >= 57)
                    {
                        if (Signal_Check(Cycle, Now_Min, Now_Sec))
                        {
                            if (Signal_Phase == 1)
                            {
                                Signal_Phase = 0;
                                string FileName = XML_Main_IO.Main.Signal.FileName;
                                int Vol = XML_Main_IO.Main.Signal.Volume;
                                int DeviceNo = XML_Main_IO.Main.Signal.Device;
                                Sound_Ctrl.Sound_Play(FileName, Vol, DeviceNo);
                                return;
                            }
                        }
                        return;
                    }
                    else
                    {
                        if (XML_Main_IO.Main.Voice.Enable)
                        {
                            if (Signal_Check(Cycle, Now_Min, Now_Sec))
                            {
                                if (Signal_Phase == 0)
                                {
                                    Signal_Phase = 1;
                                    DateTime dt = DateTime.Now;
                                    dt = dt.AddMinutes(1);
                                    string Text = dt.ToString("H時m分 をお知らせします");
                                    string Voice = "";
                                    int Vol = XML_Main_IO.Main.Voice.Volume;
                                    int Rate = XML_Main_IO.Main.Voice.Rate;
                                    Voice_Ctrl.Voice_Play(Text, Voice, Vol, Rate);
                                    return;
                                }
                            }
                            return;
                        }
                        else
                        {
                            Signal_Phase = 1;
                            return;
                        }
                    }
                }
                else
                {
                    Signal_Phase = 0;
                }
            }
        }

        private bool Signal_Check(int Cycle, int Now_Min, int Now_Sec)
        {
            bool Flag = false;
            switch (Cycle)
            {
                case 1:
                    Flag = true;
                    break;
                case 30:
                    if ((Now_Min == 29) || (Now_Min == 59))
                    {
                        Flag = true;
                    }
                    break;
                case 60:
                    if (Now_Min == 59)
                    {
                        Flag = true;
                    }
                    break;
            }
            return Flag;
        }
    }
}
