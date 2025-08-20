using ControlzEx.Theming;
using MaterialDesignColors;
using MaterialDesignExtensions.Themes;
using MaterialDesignThemes.Wpf;
using Microsoft.Windows.Themes;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading.Tasks;
using NAudio.Wave;
using static MaterialDesignThemes.Wpf.Theme;
using System.Windows.Controls.Primitives;
using System;

namespace ClockPlus
{
    public partial class App : System.Windows.Application
    {
        MaterialDesignThemes.Wpf.PaletteHelper paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();

        private System.Threading.Mutex mutex = new System.Threading.Mutex(false, "ApplicationName");

        static public NotifyIcon notifyIcon = new NotifyIcon();

        private ContextMenuStrip ContextMenu = new ContextMenuStrip();

        static public int Power_Mode = 0;
        static public string Title = null;      // アプリ名
        static public string ExeFile = null;    // exeファイル名
        static public string LnkFile = null;    // リンク名

        // フォーム参照のためのポインタ
        static public Form_Analog form_analog = new Form_Analog();
        static public Form_Digital1 form_digital1 = new Form_Digital1();
        static public Form_Digital2 form_digital2 = new Form_Digital2();
        static public Form_Weather1 form_weather1 = new Form_Weather1();
        static public Form_Weather2 form_weather2 = new Form_Weather2();

        private string jsFile = null;           // スクリプトファイル名

        private System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        private bool T500ms = false;
        private int Signal_Phase = 0;

        private WaveOutEvent outputDevice = new WaveOutEvent();
        private AudioFileReader afr;
        private WaveFileReader wfr;

        static public bool Sound_Play_Flag = false;
        static public bool Voice_Play_Flag = false;

        private Task_Ctrl task_ctrl = new Task_Ctrl();

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
            // 自身のexeファイル名を取得
            //            ExeFile = Path.GetFileName(System.Windows.Forms.Application.ExecutablePath);
            // 実行ファイルのフルパスを取得
            ExeFile = System.Windows.Forms.Application.ExecutablePath;

            // WSHスクリプト名
//            jsFile = Directory.GetParent(System.Windows.Forms.Application.ExecutablePath) + "\\addStartup.js";

            // ショートカットのリンク名
            string sMnu = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            LnkFile = sMnu + "\\" + Title + ".lnk";

            // コンテキストメニューの作成
            ContextMenu = ContextMenu_Create();
            ContextMenu.Items[0].Click += Setting_Click;
            ContextMenu.Items[2].Click += Alarm_Click;
            ContextMenu.Items[3].Click += Timer_Click;
            ContextMenu.Items[4].Click += StopWatch_Click;
            ContextMenu.Items[6].Click += Alarm_Stop_Click;
            ContextMenu.Items[8].Click += Weather_Click;
            ContextMenu.Items[10].Click += About_Click;
            ContextMenu.Items[12].Click += Close_Click;

            notifyIcon.Visible = true;
            notifyIcon.Icon = new Icon(".\\icon\\ClockPlus.ico");
            notifyIcon.Text = Title;
            notifyIcon.ContextMenuStrip = ContextMenu;

            // メイン設定を読み込む
            XML_Main _XML_Main = new XML_Main();
            _XML_Main.XML_Main_Create();

            // メッセージデータを読み込む
            XML_Message _XML_Message = new XML_Message();
            _XML_Message.XML_Message_Create();

            //////// test /////////
//            XML_Main.cnf.VoiceEngine.Engine_Name = "VoiceBox";
//            XML_Main.cnf.VoiceEngine.Path = "C:\\Users\\hosoq\\AppData\\Local\\Programs\\VOICEVOX\\vv-engine";
            //////// test /////////

            // Voice_Engineのプロセスを開始する
            if (XML_Main.cnf.VoiceEngine.Engine_Name != "無し")
            {
                Voice_Engine voice_Engine = new Voice_Engine();
                int Port = Voice_Engine.Engine_DefaultPort[XML_Main.cnf.VoiceEngine.Engine_Name];
                voice_Engine.Run(Port);
            }

            // タスクリストを読み込む
            XML_Task _XML_Task = new XML_Task();
            _XML_Task.XML_Task_Create();
            Task_Ctrl.Get_Next_Task();      // 次に有効なタスクのリスト番号を取得する

            // テーマを設定する
            MaterialDesignThemes.Wpf.Theme theme = new();
            Theme_Ctrl.Change_Theme(XML_Main.cnf.Theme.Mode, XML_Main.cnf.Theme.Color);

            // 時計表示
            form_analog.Show();
            form_digital1.Show();
            form_digital2.Show();

            // 天気予報表示
            form_weather1.Show();
            form_weather2.Show();

            /*
            form_analog.tooltip.InitialDelay = 1000;    // ToolTipが表示されるまでの時間(ms)
            form_analog.tooltip.ReshowDelay = 250;      // ToolTipが表示されている時に、別のToolTipを表示するまでの時間(ms)
            form_analog.tooltip.AutoPopDelay = 10000;   // ToolTipを表示する時間(ms)
            form_analog.tooltip.ShowAlways = true;      // フォームがアクティブでない時でもToolTipを表示する
            form_digital1.tooltip = form_analog.tooltip;
            form_digital1.tooltip = form_analog.tooltip;
            */

            timer1.Tick += new EventHandler(TimerMethod);
            timer1.Interval = 500;
            timer1.Enabled = true;      // タイマーをスタート

            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseClick);
            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseDoubleClick);
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (mutex != null)
            {
                // 終了処理
                XML_Main _XML_Main = new XML_Main();
                _XML_Main.XML_Main_save();

                XML_Message _XML_Message = new XML_Message();
                _XML_Message.XML_Message_save();

                XML_Task _XML_Task = new XML_Task();
                _XML_Task.XML_Task_save();

                // Voice_Engineのプロセスを閉じる
                Voice_Engine.Proc_Kill();

                // イベントハンドラを削除
                timer1.Enabled = false;             // タイマーを停止

                switch (Power_Mode)
                {
                    // アプリの再起動(通常モード)
                    case 1:
                        if (!App.Restart())
                        {
                            FormCtrl_Wpf.Error_Message("再起動に失敗しました。");
                        }
                        break;
                    // アプリの再起動(管理者モード)
                    case 2:
                        if (!App.AdminRestart())
                        {
                            FormCtrl_Wpf.Error_Message("管理者権限での起動に失敗しました。");
                        }
                        break;
                    // Windowsの再起動(通常)
                    case 3:
                        Task_Ctrl.Win_Reboot();
                        break;
                    // Windowsのシャットダウン
                    case 4:
                        Task_Ctrl.Win_PowerOff();
                        break;
                    default:
                        break;
                }
                mutex.ReleaseMutex();
                mutex.Close();
            }
        }

        // コンテキストメニューの作成
        static public ContextMenuStrip ContextMenu_Create()
        {
            ContextMenuStrip ContextMenu = new ContextMenuStrip();
            ContextMenu.Items.Clear();

            ToolStripMenuItem Menu_Setting = new ToolStripMenuItem();
            Menu_Setting.Text = "設定";

            ToolStripMenuItem Menu_Alarm = new ToolStripMenuItem();
            Menu_Alarm.Text = "アラーム";

            ToolStripMenuItem Menu_Timer = new ToolStripMenuItem();
            Menu_Timer.Text = "タイマー";

            ToolStripMenuItem Menu_StopWatch = new ToolStripMenuItem();
            Menu_StopWatch.Text = "ストップウォッチ";

            ToolStripMenuItem Menu_Alarm_Stop = new ToolStripMenuItem();
            Menu_Alarm_Stop.Text = "サウンド・音声を停止する";

            ToolStripMenuItem Menu_Weather = new ToolStripMenuItem();
            Menu_Weather.Text = "天気予報(気象庁)のサイトを開く";

            ToolStripMenuItem Menu_About = new ToolStripMenuItem();
            Menu_About.Text = "About";

            ToolStripMenuItem Menu_Close = new ToolStripMenuItem();
            Menu_Close.Text = "終了";

            ContextMenu.Items.Add(Menu_Setting);                // [0]
            ContextMenu.Items.Add(new ToolStripSeparator());    // [1]
            ContextMenu.Items.Add(Menu_Alarm);                  // [2]
            ContextMenu.Items.Add(Menu_Timer);                  // [3]
            ContextMenu.Items.Add(Menu_StopWatch);              // [4]
            ContextMenu.Items.Add(new ToolStripSeparator());    // [5]
            ContextMenu.Items.Add(Menu_Alarm_Stop);             // [6]
            ContextMenu.Items.Add(new ToolStripSeparator());    // [7]
            ContextMenu.Items.Add(Menu_Weather);                // [8]
            ContextMenu.Items.Add(new ToolStripSeparator());    // [9]
            ContextMenu.Items.Add(Menu_About);                  // [10]
            ContextMenu.Items.Add(new ToolStripSeparator());    // [11]
            ContextMenu.Items.Add(Menu_Close);                  // [12]
            return ContextMenu;
        }

        // 管理者モードで実行されているか
        static public bool IsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        // 通常モードで再起動する
        static public bool Restart()
        {
            try
            {
                ProcessStartInfo proc = new ProcessStartInfo();
//                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = System.Windows.Forms.Application.ExecutablePath;
//                proc.Verb = "runas"; //管理者として実行
                Process.Start(proc);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 管理者モードで再起動する
        static public bool AdminRestart()
        {
            try
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = System.Windows.Forms.Application.ExecutablePath;
                proc.Verb = "runas"; //管理者として実行
                Process.Start(proc);
                return true;
            }
            catch (Exception ex)
            {
                return false;
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

            }
        }

        private void Setting_Click(object sender, EventArgs e)
        {
            Form_Setting_Main.Tab_Page = "基本";
            WindowManager.ShowOrActivate<Form_Setting_Main>();
        }
        private void Alarm_Click(object sender, EventArgs e)
        {
            WindowManager.ShowOrActivate<Form_Alarm_List>();
        }
        private void Timer_Click(object sender, EventArgs e)
        {
            WindowManager.ShowOrActivate<Form_Timer_List>();
        }
        private void StopWatch_Click(object sender, EventArgs e)
        {
            WindowManager.ShowOrActivate<Form_StopWatch>();
        }
        private void Weather_Click(object sender, EventArgs e)
        {
            WeatherHacks weatherHacks = new WeatherHacks();
            weatherHacks.Weather_HP();
        }
        private void Alarm_Stop_Click(object sender, EventArgs e)
        {
            Task_Ctrl.Sound_Stop_Req();
        }
        private void About_Click(object sender, EventArgs e)
        {
            WindowManager.ShowOrActivate<Form_About>();
        }
        private void Close_Click(object sender, EventArgs e)
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
                // タスクトレイ表示
                notifyIcon.Text = Notify_disp();
                /*
                form_analog.tooltip.SetToolTip(form_analog.Pbox_Sec, Notify_disp());
                form_digital1.tooltip.SetToolTip(form_digital1.PicBox_Disp, App.Notify_disp());
                form_digital2.tooltip.SetToolTip(form_digital2.PicBox_Disp, App.Notify_disp());
                */

                if (Form_Analog.Analog_Disp_Check())
                {
                    FormCtrl_Net.valid_Form(form_analog);       // フォームを表示
                    form_analog.Analog_Disp_Drow();
                }
                else
                {
                    FormCtrl_Net.Hide_Form(form_analog);        // フォームを非表示
                }

                if (Digital_Drow.Digital_Disp_Check(0))
                {
                    FormCtrl_Net.valid_Form(form_digital1);     // フォームを表示
                    form_digital1.Disp1_Drow();
                }
                else
                {
                    FormCtrl_Net.Hide_Form(form_digital1);      // フォームを非表示
                }

                if (Digital_Drow.Digital_Disp_Check(1))
                {
                    FormCtrl_Net.valid_Form(form_digital2);     // フォームを表示
                    form_digital2.Disp2_Drow();
                }
                else
                {
                    FormCtrl_Net.Hide_Form(form_digital2);      // フォームを非表示
                }

                if (XML_Main.cnf.Weather.Weather_Disp[0].Enable)
                {
                    FormCtrl_Net.valid_Form(form_weather1);     // フォームを表示
                    form_weather1.Weather_Disp1_Drow();
                }
                else
                {
                    FormCtrl_Net.Hide_Form(form_weather1);      // フォームを非表示
                }

                if (XML_Main.cnf.Weather.Weather_Disp[1].Enable)
                {
                    FormCtrl_Net.valid_Form(form_weather2);     // フォームを表示
                    form_weather2.Weather_Disp2_Drow();
                }
                else
                {
                    FormCtrl_Net.Hide_Form(form_weather2);      // フォームを非表示
                }
            }

            // 時報処理
            Signal_Process();

            // タスク処理
            task_ctrl.Task_Process();
        }

        // タスクトレイ表示
        static public string Notify_disp()
        {
            string Text = Title + (DateTime.Now).ToString("\nyyyy年MM月dd日(ddd) HH:mm:ss\n\n");
            if (Task_Ctrl.Now_TaskNo > -1)
            {
                Task task = new Task();
                task = XML_Task.Task.TaskList[Task_Ctrl.Now_TaskNo];

                string Name = task.Name + "\n";
                Text = Text + Name + task.DateTime + "\n";

                if (task.Power.Enable)
                {
                    Text = Text + "("+ task.Power.Mode +")";
                }
                else
                {
                    if (task.Sound.Enable)
                    {
                        Text = Text + "(サウンド)";
                    }
                    if (task.Message.Enable)
                    {
                        Text = Text + "(メッセージ)";
                    }
                    if (task.App.Enable)
                    {
                        Text = Text + "(アプリ)";
                    }
                }
            }
            return Text;
        }

        static public void Show_BalloonTip(string Title, string Message, int Time)
        {
            /*
            //バルーンヒントのタイトル
            notifyIcon.BalloonTipTitle = Title;

            //バルーンヒントに表示するメッセージ
            notifyIcon.BalloonTipText = Message;

            //バルーンヒントに表示するアイコン
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;

            //バルーンヒントを表示する
            //表示する時間をミリ秒で指定する
            notifyIcon.ShowBalloonTip(Time);
            */
        }

        // 時報処理
        public void Signal_Process()
        {
            if (XML_Main.cnf.Signal.Enable)
            {
                int Cycle = XML_Main.cnf.Signal.Cycle;
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
                                string FileName = XML_Main.cnf.Signal.FileName;
                                int Vol = XML_Main.cnf.Signal.Volume;
                                int DeviceNo = XML_Main.cnf.Signal.Device;
                                Sound_Play(FileName, Vol, DeviceNo);
                                return;
                            }
                        }
                        return;
                    }
                    else
                    {
                        Signal_Phase = 1;
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

        private void Sound_Stop()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                afr.Close();
                outputDevice.Dispose();
            }
            Sound_Play_Flag = false;
            Voice_Play_Flag = false;
        }

        private void Sound_Play(string FileName, int Vol, int DeviceNo)
        {
            try
            {
                Sound_Stop();
                outputDevice = new WaveOutEvent();
                afr = new AudioFileReader(FileName);
                outputDevice.DeviceNumber = DeviceNo;
                outputDevice.Init(afr);
                outputDevice.Volume = (float)(Vol / 100f);
                outputDevice.Play();
                outputDevice.PlaybackStopped += Playback_Complete;
                Sound_Play_Flag = true;
            }
            catch (Exception e)
            {
                Sound_Play_Flag = false;
                Voice_Play_Flag = false;
            }
        }

        // 再生完了のイベント処理
        private async void Playback_Complete(object sender, EventArgs e)
        {
            outputDevice.Dispose();
            afr.Dispose();

            if ((XML_Main.cnf.Signal.Voice.Enable) && (!Voice_Play_Flag))
            {
                // Voiceの設定が利用可能な状態か
                if (Voice_Ctrl.Voice_Ready_Check(false))
                {
                    // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
                    if (Voice_Ctrl.Speaker_Check(XML_Main.cnf.Signal.Voice) == 0)
                    {
                        XML_Main.cnf.Signal.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                        XML_Main.cnf.Signal.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
                    }
                }

                Voice_Setting Setting = new Voice_Setting();
                Setting = XML_Main.cnf.Signal.Voice;
                string Text = XML_Message.Msg_Temp["時報"];
                Text = Voice_Ctrl.Text_Replace(Text, Setting);

                byte[] wav = await Voice_Ctrl.Voice_Gen(Text, Setting);
                if (wav != null)
                {
                    Voice_Play_Flag = true;
                    outputDevice = new WaveOutEvent();
                    wfr = new WaveFileReader(new MemoryStream(wav));
                    outputDevice.DeviceNumber = Setting.Query.Device;
                    outputDevice.Init(wfr);
                    outputDevice.Volume = (float)(100 / 100f);
                    outputDevice.Play();
                    outputDevice.PlaybackStopped += Playback_Complete;
                    return;
                }
            }
            Sound_Play_Flag = false;
            Voice_Play_Flag = false;
        }
    }
}
