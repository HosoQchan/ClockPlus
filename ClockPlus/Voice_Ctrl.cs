using System.Speech.Synthesis;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;
using System;
using NAudio.Wave;

namespace ClockPlus
{
    public class Voice_Ctrl
    {
        static public SpeechSynthesizer Synth = new SpeechSynthesizer();
        static public ReadOnlyCollection<InstalledVoice> SynthList = Synth.GetInstalledVoices();  // 音声リストの取得

        //        static public string Speech_Voice;      // 音声
        //        static public int Speech_Vol = 100;     // 音量 (0から100まで)
        //        static public int Speech_Rate = 0;      // 速度 (-10から10まで)
        //        static public string Speech_Text = "";  // テキスト

        static public List<string> Voice_List_Get(System.Windows.Controls.ComboBox cbox)
        {
            bool flag = false;
            int index = 0;
            List<string> Voice_List = new List<string>();

            foreach (InstalledVoice Slist in SynthList)
            {
                Voice_List.Add(Slist.VoiceInfo.Name);
            }
            return Voice_List;
        }

        static public void Voice_Play(string Text,string Voice,int Vol,int Rate)
        {
            // 読み上げ中なら中止
            Synth.SpeakAsyncCancelAll();

//            Synth.SelectVoice(Voice);    // 音声の設定
            Synth.Volume = Vol;          // 音量の設定
            Synth.Rate = Rate;           // 速度の設定

            try
            {

                Synth.SpeakAsync(Text);       // 文字列の同期読み上げ 

            }
            catch (Exception err)
            {
                MessageBox.Show("音声合成でエラーが発生しました。\r\n" + err);
                return;
            }
        }

        static public void Voice_Stop()
        {
            Synth.SpeakAsyncCancelAll();
        }
    }
}
