using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockPlus
{
    public class Sound_Ctrl
    {
        static public WaveOutEvent outputDevice;
        static public AudioFileReader afr;

        // デフォルトの音声ファイルを生成 
        static public void Sound_File_Create()
        {
            // アラーム
            if (Directory.Exists(XML_Main_IO.Sound_Dir + "\\Alarm.wav") == false)
            {
                Stream strm = Properties.Resources.Alarm;
                byte[] buffer = new Byte[strm.Length];
                strm.Read(buffer, 0, (int)buffer.Length);
                strm.Close();

                FileStream fs = new FileStream(XML_Main_IO.Sound_Dir + "\\Alarm.wav", FileMode.Create);
                fs.Write(buffer, 0, (int)buffer.Length);
                fs.Close();
            }
            // 時報
            if (Directory.Exists(XML_Main_IO.Sound_Dir + "\\Jihou.wav") == false)
            {
                Stream strm = Properties.Resources.Jihou;
                byte[] buffer = new Byte[strm.Length];
                strm.Read(buffer, 0, (int)buffer.Length);
                strm.Close();

                FileStream fs = new FileStream(XML_Main_IO.Sound_Dir + "\\Jihou.wav", FileMode.Create);
                fs.Write(buffer, 0, (int)buffer.Length);
                fs.Close();
            }
            // スリープ
            if (Directory.Exists(XML_Main_IO.Sound_Dir + "\\Sleep.wav") == false)
            {
                Stream strm = Properties.Resources.Sleep;
                byte[] buffer = new Byte[strm.Length];
                strm.Read(buffer, 0, (int)buffer.Length);
                strm.Close();

                FileStream fs = new FileStream(XML_Main_IO.Sound_Dir + "\\Sleep.wav", FileMode.Create);
                fs.Write(buffer, 0, (int)buffer.Length);
                fs.Close();
            }
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

        static public void Sound_Play(string FileName, int Vol, int Device_No)
        {
            try
            {
                Sound_Stop();
                if (System.IO.File.Exists(FileName) == true)
                {
                    if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
                    { }
                    else
                    {
                        outputDevice = new WaveOutEvent();
                        afr = new AudioFileReader(FileName);
                        outputDevice.DeviceNumber = Device_No;
                        outputDevice.Init(afr);
                        outputDevice.Volume = (float)(Vol / 100f);
                        outputDevice.Play();
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        static public void Sound_Stop()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                afr.Close();
                outputDevice.Dispose();
            }
        }

        static public void Sound_Play_Stop(string FileName, int Vol, int Device_No)
        {
            if(Sound_Info())
            {
                outputDevice.Stop();
                afr.Close();
                outputDevice.Dispose();
            }
            else
            {
                Sound_Play(FileName, Vol, Device_No);
            }
        }
    }
}
