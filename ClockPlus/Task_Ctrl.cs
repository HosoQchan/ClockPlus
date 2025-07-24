using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ClockPlus.Date_Picker;

namespace ClockPlus
{
    public class Task_Ctrl
    {
        public enum Win32ShutdownFlags
        {
            // ログオフ（サインアウト）
            Logoff = 0,
            // シャットダウン
            Shutdown = 1,
            // 再起動
            Reboot = 2,
            // 電源オフ
            PowerOff = 8,
            // 強制的に実行
            Forced = 4
        }

        // 実行中のタスク
        static public int Now_TaskNo = -1;
        static public bool Task_Run_Flg = false;
        static public bool Task_Comp_Flg = false;
        private Task task = new Task();

        // スリープ制御
        private delegate void TimerSetDelegate();
        private delegate void TimerCompleteDelegate();
        private event TimerSetDelegate OnTimerSet;
        private event TimerCompleteDelegate OnTimerCompleted;
        [DllImport("kernel32.dll")]
        static extern IntPtr CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);
        [DllImport("kernel32.dll")]
        static extern bool SetWaitableTimer(IntPtr hTimer, [In] ref long ft, int lPeriod, TimerCompleteDelegate pfnCompletionRoutine, IntPtr pArgToCompletionRoutine, bool fResume);
        [DllImport("kernel32.dll")]
        static extern bool CancelWaitableTimer(IntPtr hTimer);
        private IntPtr handle;


        // 電源制御
        static public bool Forced_Flag = false;

        // タスク処理
        public void Task_Process()
        {
            if (Now_TaskNo < 0)
            {
                CancelWaitableTimer(handle);
            }
            else
            {
                task = XML_Task.Task.TaskList[Now_TaskNo];

                DateTime Now_Dtime = (DateTime.Now).RoundDownToSecond();    // 秒下の値は切り捨てる
                DateTime Tgt_Dtime = (task.DateTime).RoundDownToSecond();   // 秒下の値は切り捨てる
//                Debug.WriteLine("Now>" + Now_Dtime);
//                Debug.WriteLine("Tgt>" + Tgt_Dtime);

                // 指定日時と現在の日時が一致した場合
                if (Tgt_Dtime == Now_Dtime)
                {
                    CancelWaitableTimer(handle);
                    Debug.WriteLine("Hit!!");
                    // タスクを実行する
                    Task_Run();
                    return;
                }
                else
                {
                    // 現在の日時が、指定日時を過ぎている場合
                    if (Tgt_Dtime < Now_Dtime)
                    {
                        CancelWaitableTimer(handle);
                        // 現在のタスクを終了し、次のタスクへ移行する
                        Task_Completed();
                        return;
                    }
                    // 指定日時が、現在の日時より先の場合
                    else
                    {
                        // スリープからの復帰処理
                        Sleep_WakeUp_Process(Now_Dtime, Tgt_Dtime);

                        Task_Run_Flg = false;
                        Task_Comp_Flg = false;
                        return;
                    }
                }
            }
        }

        // スリープからの復帰処理
        private void Sleep_WakeUp_Process(DateTime Now_Dtime, DateTime Tgt_Dtime)
        {
            // スリープからの復帰処理
            if (task.Power.Enable)
            {
                if (task.Power.Mode == "スリープから復帰")
                {
                    // 現時刻から指定時間までの相対時間を算出
                    double ts1 = (Tgt_Dtime - Now_Dtime).TotalSeconds;
                    ts1 = ts1 * 10000000;       // 100ns単位なので秒数に10000000を掛ける
                    ts1 = -ts1;                 // マイナス値に変換
                    SetTimer((long)ts1);        // スリープ解除用タイマーセット
                    return;
                }
            }
            CancelWaitableTimer(handle);
        }
        // スリープ解除用タイマー
        public void SetTimer(long Interval)
        {
            handle = CreateWaitableTimer(IntPtr.Zero, true, "WaitableTimer");
            SetWaitableTimer(handle, ref Interval, 0, null, IntPtr.Zero, true);
            if (OnTimerSet != null)
            {
                OnTimerSet();
            }
        }

        // タスクを実行する
        private void Task_Run()
        {
            if (Task_Run_Flg)
            {
                return;
            }
            Task_Run_Flg = true;

            // 電源制御
            if (task.Power.Enable)
            {
                switch (task.Power.Mode)
                {
                    case "スリープへ移行":
                    case "再起動":
                    case "シャットダウン":
                        // メッセージ（警告）を鳴らす
                        System.Media.SystemSounds.Exclamation.Play();

                        Form_Power Form = new Form_Power(task.Power.Mode);
                        Form.ShowDialog();
                        break;
                }
            }
            else
            {
                // 音声
                if (task.Sound.Enable)
                {
                    // 既にアラーム再生中などで、再生途中の場合は何もしない
                    if (!Sound_Ctrl.Sound_Play_Flag)
                    {
                        Sound_Ctrl.Repeat = task.Sound.Repeat;
                        Sound_Ctrl.Password_Enable = task.Sound.Password_Enable;
                        Sound_Ctrl.Password = XML_Main.cnf.Password;
                        Sound_Ctrl.Sound_Play(task.Sound.FileName, task.Sound.Volume, task.Sound.Device);
                    }
                }
                // アプリ
                if (task.App.Enable)
                {
                    string Program = task.App.FileName;
                    string Option = task.App.Option;
                    string Command = Program + " " + Option;
                    try
                    {
                        ProcessStartInfo pInfo = new ProcessStartInfo();
                        pInfo.FileName = Program;
                        pInfo.Arguments = Option;
                        Process.Start(pInfo);
                    }
                    catch (Exception ex)
                    {
                        FormCtrl_Wpf.Error_Message("アプリの起動でエラーが発生しました。");
                    }
                }
            }
            // 現在のタスクを終了し、次のタスクへ移行する
            Task_Completed();
        }

        // 現在のタスクを終了し、次のタスクへ移行する
        public void Task_Completed()
        {
            if (Task_Comp_Flg)
            {
                return;
            }
            Task_Comp_Flg = true;

            if (XML_Task.Task.TaskList[Now_TaskNo].Cycle == "一回")
            {
                // アラームなら登録リストから削除
                if (XML_Task.Task.TaskList[Now_TaskNo].Type == "Alarm")
                {
                    XML_Task.Task.TaskList.RemoveAt(Now_TaskNo);
//                    XML_Task.Task.TaskList[Now_TaskNo].Enable = false;
                }
                // タイマーならタスクを無効にする
                else
                {
                    XML_Task.Task.TaskList[Now_TaskNo].Enable = false;
                }
            }
            else
            {
                // 現在の日時から、指定された回数の中で最も近い日時を求める
                XML_Task.Task.TaskList[Now_TaskNo] = Alarm_Dtime_Gen(XML_Task.Task.TaskList[Now_TaskNo]);
                Sort_Task_List();       // リストを日時の昇順にソートする
            }
            Get_Next_Task();        // 次に有効なタスクのリスト番号を取得する
        }

        // タスクを日時の昇順にソートする
        static public void Sort_Task_List()
        {
            XML_Task.Task.TaskList.Sort((a, b) => DateTime.Compare(a.DateTime, b.DateTime));
        }

        // 次に有効なタスクのリスト番号を取得する
        static public void Get_Next_Task()
        {
            Task_Ctrl.Now_TaskNo = -1;
            for (int i = 0; i < XML_Task.Task.TaskList.Count; i++)
            {
                Task task = new Task();
                task = XML_Task.Task.TaskList[i];
                if (task.Enable)
                {
                    Task_Ctrl.Now_TaskNo = i;
                    Task_Run_Flg = false;
                    Task_Comp_Flg = false;
                    break;
                }
            }
        }

        // 現在の日時から、指定された回数の中で最も近い日時を求める
        static public Task Alarm_Dtime_Gen(Task task)
        {
            Str_Dtime_Item Now_Dtime = new Str_Dtime_Item();
            Now_Dtime = DateTime_str(DateTime.Now);

            Str_Dtime_Item Tgt_Dtime = new Str_Dtime_Item();
            Tgt_Dtime = DateTime_str(task.DateTime);

            DateTime dt = DateTime.Now;
            switch (task.Cycle)
            {
                case "一回":
                    break;
                case "毎日":
                    task.DateTime = DateTime.Parse(Now_Dtime.Date + " " + Tgt_Dtime.Time);
                    if (task.DateTime <= DateTime.Now)
                    {
                        task.DateTime = (task.DateTime).AddDays(1);
                    }
                    break;
                case "毎週":
                    dt = DateTime.Parse(Now_Dtime.Date + " " + Tgt_Dtime.Time);
                    List<DateTime> Week_Hit_List = new List<DateTime>();
                    Week_Hit_List.Clear();
                    // 月曜
                    if (task.Week_List.Mon)
                    {
                        DateTime monday = GetNearestDayOfWeek(dt, DayOfWeek.Monday);
                        Week_Hit_List.Add(monday);
                    }
                    // 火曜
                    if (task.Week_List.Tue)
                    {
                        DateTime monday = GetNearestDayOfWeek(dt, DayOfWeek.Tuesday);
                        Week_Hit_List.Add(monday);
                    }
                    // 水曜
                    if (task.Week_List.Wed)
                    {
                        DateTime monday = GetNearestDayOfWeek(dt, DayOfWeek.Wednesday);
                        Week_Hit_List.Add(monday);
                    }
                    // 木曜
                    if (task.Week_List.Thu)
                    {
                        DateTime monday = GetNearestDayOfWeek(dt, DayOfWeek.Thursday);
                        Week_Hit_List.Add(monday);
                    }
                    // 金曜                   
                    if (task.Week_List.Fri)
                    {
                        DateTime monday = GetNearestDayOfWeek(dt, DayOfWeek.Friday);
                        Week_Hit_List.Add(monday);
                    }
                    // 土曜                   
                    if (task.Week_List.Sat)
                    {
                        DateTime monday = GetNearestDayOfWeek(dt, DayOfWeek.Saturday);
                        Week_Hit_List.Add(monday);
                    }
                    // 日曜                   
                    if (task.Week_List.Sun)
                    {
                        DateTime monday = GetNearestDayOfWeek(dt, DayOfWeek.Sunday);
                        Week_Hit_List.Add(monday);
                    }

                    // 現在の日時から、一番近い日時を取得
                    task.DateTime = ReturnClosest(DateTime.Now, Week_Hit_List);
                    break;
                case "毎月":
                    task.DateTime = DateTime.Parse(Now_Dtime.Date_Y + "/" + Now_Dtime.Date_M + "/" + Tgt_Dtime.Date_D + " " + Tgt_Dtime.Time);
                    if (task.DateTime <= DateTime.Now)
                    {
                        task.DateTime = task.DateTime.AddMonths(1);
                    }
                    break;
                case "毎年":
                    task.DateTime = DateTime.Parse(Now_Dtime.Date_Y + "/" + Tgt_Dtime.Date_M + "/" + Tgt_Dtime.Date_D + " " + Tgt_Dtime.Time);
                    if (task.DateTime <= DateTime.Now)
                    {
                        task.DateTime = task.DateTime.AddYears(1);
                    }
                    break;
            }
            return task;
        }

        // 引数(wantDayOfWeek)で指定した曜日で、指定日(day)から最も近い未来日を返す
        static public DateTime GetNearestDayOfWeek(DateTime Dt, DayOfWeek wantDayOfWeek)
        {
            DayOfWeek dayOfWeek = Dt.DayOfWeek;
            Dt = Dt.AddDays(((int)(DayOfWeek.Saturday - Dt.DayOfWeek + wantDayOfWeek) % 7) + 1);
            return Dt;
        }

        // 指定日時に一番近いデータをListから取得
        static public DateTime ReturnClosest(DateTime dt, List<DateTime> dateTimes)
        {
            DateTime closestDateToNow = dateTimes[0];
            for (int i = 0; i < dateTimes.Count; i++)
            {
                TimeSpan Day1 = closestDateToNow - dt;
                TimeSpan Day2 = dateTimes[i] - dt;
                if (Day2 < Day1)
                {
                    closestDateToNow = dateTimes[i];
                }
            }
            return closestDateToNow;
        }

        // ログオフ（サインアウト）
        public void Win_LogOff()
        {
            RunWin32Shutdown(Win32ShutdownFlags.Logoff);
        }

        // シャットダウン
        public void Win_Shutdown()
        {
            RunWin32Shutdown(Win32ShutdownFlags.Shutdown);
        }

        // 再起動
        static public void Win_Reboot()
        {
            RunWin32Shutdown(Win32ShutdownFlags.Reboot);
        }

        // 電源オフ
        static public void Win_PowerOff()
        {
            RunWin32Shutdown(Win32ShutdownFlags.PowerOff);
        }

        // 電源制御コマンドを実行
        static public void RunWin32Shutdown(Win32ShutdownFlags shutdownFlags)
        {
            int flags = (int)shutdownFlags;
            if (Forced_Flag)
            {
                // 強制的に実行
                flags += (int)Win32ShutdownFlags.Forced;
            }
            Win32Shutdown(flags);
        }
        static public void Win32Shutdown(int shutdownFlags)
        {
            Thread thread = new Thread(() =>
            {
                // Win32_OperatingSystemクラスを作成する
                using (ManagementClass managementClass = new ManagementClass("Win32_OperatingSystem"))
                {
                    // Win32_OperatingSystemオブジェクトを取得する
                    managementClass.Get();
                    // 権限を有効化する
                    managementClass.Scope.Options.EnablePrivileges = true;

                    // WMIのオブジェクトのコレクションを取得する
                    ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
                    // WMIのオブジェクトを列挙する
                    foreach (ManagementObject managementObject in managementObjectCollection)
                    {
                        // InvokeMethodでWMIのメソッドを実行する
                        managementObject.InvokeMethod(
                            // 実行メソッド名
                            "Win32Shutdown",
                            // メソッドの引数をオブジェクト配列で指定
                            new object[] { shutdownFlags, 0 }
                            );

                        // WMIのオブジェクトのリソースを開放
                        managementObject.Dispose();
                    }
                }
            });
            // スレッドモデルをSTAに設定する
            thread.SetApartmentState(ApartmentState.STA);
            // スレッドを実行する
            thread.Start();
            // スレッドの終了を待つ
            thread.Join();
        }
    }
}
