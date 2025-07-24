using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockPlus
{
    public class Signal_Ctrl
    {
        static public WaveOutEvent outputDevice;
        static public AudioFileReader afr;

        static public bool Sound_Play_Flag = false;

        // 音声を再生中か？
        static public bool Sound_Info()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                return true;
            }
            return false;
        }

        static public void Sound_Stop()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                afr.Close();
                outputDevice.Dispose();
            }
            Sound_Play_Flag = false;
        }

        static public bool Sound_Play(string FileName, int Vol, int DeviceNo)
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
                return true;
            }
            catch (Exception e)
            {
                Sound_Play_Flag = false;
                return false;
            }
        }

        // 再生完了のイベント処理
        static public void Playback_Complete(object sender, EventArgs e)
        {
            outputDevice.Dispose();
            afr.Dispose();
            Sound_Play_Flag = false;
        }
    }
}
