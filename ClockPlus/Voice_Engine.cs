using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Printing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using VoicevoxClientSharp.ApiClient;
using VoicevoxClientSharp.ApiClient.Models;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace ClockPlus
{
    public class Voice_Engine
    {
        static public Dictionary<string, string> Engine_DefaultPath = new Dictionary<string, string>()
        {
            {"無し",""},
            {"VoiceBox",Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Programs\\VOICEVOX\\vv-engine"},
            {"AivisSpeech",Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Programs\\AivisSpeech\\AivisSpeech-Engine"},
        };

        static public Dictionary<string, int> Engine_DefaultPort = new Dictionary<string, int>()
        {
            {"無し",0},
            {"VoiceBox",50021},
            {"AivisSpeech",10101},
        };

        static public Process process = null;
        static public Proc_step Process_Step = Proc_step.None;
        static public int Connect_Port = 0;

        private string Command = "";
        private int RetryCount = 10;    // Port接続リトライ回数

        public delegate void MyEventHandler(object sender, DataReceivedEventArgs e);
        public event MyEventHandler myEvent = null;

        public enum Proc_step
        {
            None = 0,
            Connect = 1,
            Ready = 5,
            Port_Error = 6,
            Engine_Error = 7,
        };

        public void Run(int Port)
        {
            // 指定されたポート番号以降の、最初の空きポートを取得
            Port = GetAvailablePort(Port);

            Debug.WriteLine("Port接続リトライ回数:" + RetryCount);

            string Path = XML_Main.cnf.VoiceEngine.Path;
            if ((Path == "") || (Port == 0))
            {
                return;
            }

            Command = "\"" + (Path + "\\run.exe") + "\"";
            Command = Command + " --port " + Port.ToString();
            Connect_Port = Port;

            Process_Step = Proc_step.Connect;
            myEvent = new MyEventHandler(event_DataReceived);

            process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = "/c ";
            process.StartInfo.Arguments = process.StartInfo.Arguments + Command;

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            // 標準出力をリダイレクト
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += new DataReceivedEventHandler(process_DataReceived);

            // エラー出力をリダイレクト
            process.StartInfo.RedirectStandardError = true;
            process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorReceived);

            // プロセス終了時のイベントハンドラ
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(process_ExitedReceived);

            process.Start();
            process.BeginOutputReadLine();
        }

        private void event_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine("Event:" + e.Data);
        }

        private async void process_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                Debug.WriteLine("data:" + e.Data);

                // AivisSpeechの場合
                if (XML_Main.cnf.VoiceEngine.Engine_Name == "AivisSpeech")
                {
                    Match mh;
                    mh = Regex.Match(e.Data, @".+Press CTRL\+C to quit", RegexOptions.None);
                    if (mh.Success)
                    {
                        Debug.WriteLine("Connect Ok!!!:");
                        try
                        {
                            // APIクライアントを生成(CreateForAvisSpeechの使用を許可)
                            string uri = "http://localhost:" + Connect_Port.ToString();
                            using var apiClient = VoicevoxApiClient.CreateForAvisSpeech(baseUrl: uri);

                            // スピーカー一覧を取得する
                            Speaker[] speakers = null;
                            Voice_Ctrl.speakers = await apiClient.GetSpeakersAsync();

                            // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
                            XML_Main.Voice_Setting_Check();
                            XML_Task.Voice_Setting_Check();

                            Process_Step = Proc_step.Ready;
                        }
                        catch
                        {
                            Process_Step = Proc_step.Engine_Error;
                        }
                        return;
                    }
                    mh = Regex.Match(e.Data, @".+アクセス許可で禁じられた方法でソケットにアクセスしようとしました。", RegexOptions.None);
                    if (mh.Success)
                    {
                        Debug.WriteLine("Port Error!!!:");
                        Process_Step = Proc_step.Port_Error;
                    }
                }
                // VoiceBoxの場合
                else
                {
                    Match mh;
                    mh = Regex.Match(e.Data, @"emitting double-array: 100.+", RegexOptions.None);
                    if (mh.Success)
                    {
                        Debug.WriteLine("Connect Ok!!!:");
                        try
                        {
                            // APIクライアントを生成(CreateForAvisSpeechの使用を許可)
                            string uri = "http://localhost:" + Connect_Port.ToString();
                            using var apiClient = VoicevoxApiClient.CreateForAvisSpeech(baseUrl: uri);

                            // スピーカー一覧を取得する
                            Speaker[] speakers = null;
                            Voice_Ctrl.speakers = await apiClient.GetSpeakersAsync();

                            // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
                            XML_Main.Voice_Setting_Check();
                            XML_Task.Voice_Setting_Check();

                            Process_Step = Proc_step.Ready;
                        }
                        catch
                        {
                            Process_Step = Proc_step.Engine_Error;
                        }
                        return;
                    }
                    mh = Regex.Match(e.Data, @".+error while attempting to bind on address.+", RegexOptions.None);
                    if (mh.Success)
                    {
                        Debug.WriteLine("Port Error!!!:");
                        Process_Step = Proc_step.Port_Error;
                    }
                }
            }
        }
        private void process_ErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                Debug.WriteLine("Engine Error!!!:" + e.Data);
                Process_Step = Proc_step.Engine_Error;
            }
        }

        private void process_ExitedReceived(object sender, EventArgs e)
        {
            Process proc = (System.Diagnostics.Process)sender;
            Debug.WriteLine("プロセスが終了しました。プロセスID：" + proc.Id.ToString());
            
            // ポートエラーが発生した場合は、ポートを+1してリトライする
            if (Process_Step == Proc_step.Port_Error)
            {
                if (RetryCount > 0)
                {
                    RetryCount--;
                    Connect_Port++;
                    Run(Connect_Port);
                }
                else
                {
                    Process_Step = Proc_step.Engine_Error;
                }
            }
            else
            {
                Process_Step = Proc_step.None;
            }
        }

        // Voice_Engineのプロセスを閉じる
        static public void Proc_Kill()
        {
            try
            {
                // ADBのプロセス名（adb.exe）を特定します
                Process[] processes = Process.GetProcessesByName("run");

                // 該当するプロセスが存在する場合、終了させます
                if (processes.Length > 0)
                {
                    foreach (Process process in processes)
                    {
                        process.Kill();                 // プロセスを強制終了
                        process.WaitForExit();          // 終了待ち
                    }
                    Debug.WriteLine("processes killed.");               // 終了確認メッセージ
                }
                else
                {
                    Debug.WriteLine("process not found.");              // プロセスが存在しない場合
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");    // 例外発生時の処理
            }
        }

        // 指定したポート番号以降の最初の空きポートを取得する
        static public int GetAvailablePort(int startPort)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            var connections = ipGlobalProperties.GetActiveTcpConnections().Select(x => x.LocalEndPoint);
            var tcpListeners = ipGlobalProperties.GetActiveTcpListeners();
            var udpListeners = ipGlobalProperties.GetActiveUdpListeners();

            var activePorts = new HashSet<int>(connections
                .Concat(tcpListeners)
                .Concat(udpListeners)
                .Where(x => x.Port >= startPort)
                .Select(x => x.Port));

            for (var port = startPort; port <= 65535; port++)
            {
                if (!activePorts.Contains(port))
                {
                    return port;
                }
            }
        return -1;
        }
    }
}