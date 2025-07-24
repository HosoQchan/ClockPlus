using ClockPlus.Properties;
using ControlzEx.Standard;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace ClockPlus
{
    public class Sound_Ctrl
    {
        static public WaveOutEvent outputDevice;
        static public AudioFileReader afr;

        static public bool Sound_Play_Flag = false;
        static public int Repeat = 0;
        static public bool Password_Enable = false;
        static public string Password = "";
        static public System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();

        // リソースからサウンドファイルを作成する
        public void Create_Sound_Files()
        {
            DirectoryInfo Info;
            // Soundフォルダを作成
            Info = Directory.CreateDirectory(XML_Main.App_Path + "sound");
            XML_Main.Sound_Dir = Info.FullName;

            /*
            if (!File.Exists(XML_Main.Sound_Dir + "\\Jihou.wav"))
            {
                Sound_File_Save("ClockPlus.sound.Jihou.wav", XML_Main.Sound_Dir + "\\Jihou.wav");
            }
            if (!File.Exists(XML_Main.Sound_Dir + "\\Sleep.wav"))
            {
                Sound_File_Save("ClockPlus.sound.Sleep.wav", XML_Main.Sound_Dir + "\\Sleep.wav");
            }
            if (!File.Exists(XML_Main.Sound_Dir + "\\Warning.wav"))
            {
                Sound_File_Save("ClockPlus.sound.Warning.wav", XML_Main.Sound_Dir + "\\Warning.wav");
            }

            if (!File.Exists(XML_Main.Sound_Dir + "\\Alarm.wav"))
            {
                Sound_File_Save("ClockPlus.sound.Alarm.wav", XML_Main.Sound_Dir + "\\Alarm.wav");
            }
            if (!File.Exists(XML_Main.Sound_Dir + "\\Furusato.wav"))
            {
                Sound_File_Save("ClockPlus.sound.Furusato.wav", XML_Main.Sound_Dir + "\\Furusato.wav");
            }
            if (!File.Exists(XML_Main.Sound_Dir + "\\School.wav"))
            {
                Sound_File_Save("ClockPlus.sound.School.wav", XML_Main.Sound_Dir + "\\School.wav");
            }
            if (!File.Exists(XML_Main.Sound_Dir + "\\Tetudou_sanka.wav"))
            {
                Sound_File_Save("ClockPlus.sound.Tetudou_sanka.wav", XML_Main.Sound_Dir + "\\Tetudou_sanka.wav");
            }
            */
        }
        private void Sound_File_Save(string Resource_Name,string File_Path)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();

            // リソース名は 名前空間名 ＋ リソース名 です
            Stream strm = asm.GetManifestResourceStream(Resource_Name);
            byte[] buffer = new Byte[strm.Length];
            strm.Read(buffer, 0, (int)buffer.Length);
            strm.Close();

            // ファイルに書き込む
            System.IO.FileStream fs = new System.IO.FileStream(File_Path, System.IO.FileMode.Create);
            fs.Write(buffer, 0, (int)buffer.Length);
            fs.Close();
        }

        // 出力デバイス名を取得
        static public List<string> Get_Device_List()
        {
            List<string> Device_List = new List<string>();
            Device_List.Clear();
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var capabilities = WaveOut.GetCapabilities(i);
                Device_List.Add(capabilities.ProductName);
            }
            return Device_List;
        }

        // 音声を再生中か？
        static public bool Sound_Info()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                return true;
            }
            return false;
        }

        static public void Sound_Stop_Req()
        {
            if (!Password_Enable)
            {
                Sound_Stop();
            }
            else
            {
                // パスワード入力画面へ
                Form_Password Form = new Form_Password(Password);
                Form.ShowDialog();
            }
        }

        static public void Sound_Stop()
        {
            timer1.Stop();
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                afr.Close();
                outputDevice.Dispose();
            }
            Sound_Play_Flag = false;
        }

        static public void Sound_Play(string FileName, int Vol, int DeviceNo)
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

                // 繰り返し？
                if (Repeat != 0)
                {
                    timer1.Interval = Repeat * 1000;
                    timer1.Tick += Timer1_Tick;
                    timer1.Start();
                }
            }
            catch (Exception e)
            {
                Sound_Play_Flag = false;
            }
        }

        // 再生完了のイベント処理
        static public void Playback_Complete(object sender, EventArgs e)
        {
            if (Repeat == 0)
            {
                outputDevice.Dispose();
                afr.Dispose();
                Sound_Play_Flag = false;
            }
        }

        static public void Timer1_Tick(object sender, EventArgs e)
        {
            afr.Position = 0;
            outputDevice.Play();
        }
    }
}
