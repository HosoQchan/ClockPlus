using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoicevoxClientSharp;
using VoicevoxClientSharp.ApiClient.Models;
using System.Diagnostics;
using System.Windows.Forms;
using VoicevoxClientSharp.ApiClient;
using System.Media;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;

using System.Media;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.WebRequestMethods;
using NAudio.Wave;
using System.Numerics;
using System.Windows;
using System.Xml.Serialization;
using static ClockPlus.Voice_Engine;
using NAudio.CoreAudioApi;
using Microsoft.Win32.TaskScheduler;
using static ClockPlus.Date_Picker;
using static System.Windows.Forms.DataFormats;


namespace ClockPlus
{
    public class Voice_Setting
    {
        public bool Enable { get; set; } = false;
        public Style Style { get; set; } = new Style();
        public Query Query { get; set; } = new Query();
    }

    public class Style
    {
        public string Name { get; set; } = "Anneli";
        public string Type { get; set; } = "ノーマル";
    }

    public class Query
    {
        public int Device { get; set; } = 0;                        // 出力デバイス
        public decimal VolumeScale { get; set; } = 1.00M;           // 声の音量

        //        AccentPhrase[] AccentPhrases { get; set; } = Array.Empty<AccentPhrase>();   // アクセント句のリスト
        public decimal SpeedScale { get; set; } = 1.00M;            // 声の速さ
        public decimal PitchScale { get; set; } = 0.00M;            // 声の高さ
//        public decimal IntonationScale { get; set; } = 1M;        // 声の高低
        
        public decimal PrePhonemeLength { get; set; } = 0.10M;      // 読み上げ前の待機時間
        public decimal PostPhonemeLength { get; set; } = 0.10M;     // 読み上げ後の時間
//        public decimal? PauseLength { get; set; } = 0.1M;         // 読み上げ途中の待機時間
//        public decimal? PauseLengthScale { get; set; } = 1M;      // 読み上げ途中の待機時間の倍率
        public int OutputSamplingRate { get; set; } = 24000;        // 声のサンプリングレート
        public bool OutputStereo { get; set; } = false;              // ステレオ出力の有効/無効
    }


    public class Voice_Ctrl
    {
        static public Speaker[] speakers = null;

        static public async Task<byte[]> Voice_Gen(string Text, Voice_Setting setting)
        {
            byte[] wav = null;

            if (Voice_Engine.Process_Step != Proc_step.Ready)
            {
                return null;
            }
            try
            {
                // APIクライアントを生成(CreateForAvisSpeechの使用を許可)
                string uri = "http://localhost:" + Voice_Engine.Connect_Port.ToString();
                using var apiClient = VoicevoxApiClient.CreateForAvisSpeech(baseUrl: uri);

                if (Text != "")
                {
                    if (speakers != null)
                    {
                        int Style_Id;
                        Speaker speaker = speakers.FirstOrDefault(s => s.Name == setting.Style.Name);
                        // 指定のスピーカー名からスタイルIDを取得
                        if (speaker != null)
                        {
                            Style_Id = speaker?.Styles.FirstOrDefault(x => x.Name == setting.Style.Type)!.Id ?? 0;
                        }
                        // 指定のスピーカー名が存在しない場合は、デフォルトのスタイルIDを選択
                        else
                        {
                            Style_Id = speakers[0].Styles[0].Id;
                            setting.Style.Name = speakers[0].Name;
                            setting.Style.Type = speakers[0].Styles[0].Name;
                        }

                        // 音声合成用のクエリを作成
                        AudioQuery audioQuery = await apiClient.CreateAudioQueryAsync(Text, Style_Id);

                        //              audioQuery.AccentPhrases
                        audioQuery.SpeedScale = setting.Query.SpeedScale;
                        audioQuery.PitchScale = setting.Query.PitchScale;
                        audioQuery.VolumeScale = setting.Query.VolumeScale;
                        audioQuery.OutputStereo = setting.Query.OutputStereo;
                        audioQuery.OutputSamplingRate = setting.Query.OutputSamplingRate;
                        //                audioQuery.IntonationScale = setting.Query.IntonationScale;
                        audioQuery.PrePhonemeLength = setting.Query.PrePhonemeLength;
                        audioQuery.PostPhonemeLength = setting.Query.PostPhonemeLength;
                        //                audioQuery.PauseLength = setting.Query.PauseLength;
                        //                audioQuery.PauseLengthScale = setting.Query.PauseLengthScale;

                        wav = await apiClient.SynthesisAsync(Style_Id, audioQuery);
                    }
                }
            }
            catch
            {
                speakers = null;
            }
            return wav;
        }

        // 指定のスピーカー名からスタイルIDを取得する、存在しない場合は0を返す
        static public int Speaker_Check(Voice_Setting setting)
        {
            int Style_Id = 0;
            Speaker speaker = speakers.FirstOrDefault(s => s.Name == setting.Style.Name);
            // 指定のスピーカー名からスタイルIDを取得
            if (speaker != null)
            {
                Style_Id = speaker?.Styles.FirstOrDefault(x => x.Name == setting.Style.Type)!.Id ?? 0;
            }
            return Style_Id;
        }

        /*
        // スピーカー一覧を取得する
        static public async Task<Speaker[]> Get_Speakers()
        {
            Speaker[] speakers = null;
            // APIクライアントを生成(CreateForAvisSpeechの使用を許可)
            string uri = "http://localhost:" + Voice_Engine.Connect_Port.ToString();
            using var apiClient = VoicevoxApiClient.CreateForAvisSpeech(baseUrl: uri);
            speakers = await apiClient.GetSpeakersAsync();
            return speakers;
        }
        */

        // Voiceの設定が利用可能な状態か(エラー表示あり)
        static public bool Voice_Ready_Check(bool ErrorDisp)
        {
            switch (Voice_Engine.Process_Step)
            {
                case Proc_step.None:
                    return false;
                case Proc_step.Connect:
                    if (ErrorDisp)
                    {
                        FormCtrl_Wpf.Info_Message("音声Engineの起動中です。\r\n暫く待った後に、もう一度お試しください。", 0);
                    }
                    return false;
                case Proc_step.Ready:

                    break;
                case Proc_step.Port_Error:
                case Proc_step.Engine_Error:
                    if (ErrorDisp)
                    {
                        FormCtrl_Wpf.Error_Message("音声Engineが正常に起動されていません。");
                    }
                    return false;
            }

            if (speakers == null)
            {
                if (ErrorDisp)
                {
                    FormCtrl_Wpf.Error_Message("音声Engineのキャラクタデータが存在しません。");
                }
                return false;
            }
            return true;
        }

        // 日付を音声用に変換する
        static public string Voice_DateTime_Gen(DateTime dt,bool Wareki)
        {
            string Text = "";
            if (Wareki)
            {
                Text = Text + dt.ToString("ggy年M月d日 dddd");
            }
            else
            {
                Int_Dtime_Item dtime_Item = new Int_Dtime_Item();
                dtime_Item = Date_Picker.DateTime_int(dt);
                Text = Text + dtime_Item.Date_Y.ToString() + "年";
                Text = Text + dtime_Item.Date_M.ToString() + "月";
                Text = Text + dtime_Item.Date_D.ToString() + "日";
                Text = Text + " " + dt.ToString("dddd");
            }
            return Text;
        }

        // 特殊文字を含んだメッセージを変換する
        static public string Text_Replace(string str, Voice_Setting Setting)
        {
            WeatherHacks weatherHacks = new WeatherHacks();
            str = str.Replace("[Voiceキャラクタ名]", Voice_ChrName_Gen(Setting));
            str = str.Replace("[挨拶]", Etc.Hellow_Gen());
            str = str.Replace("[時刻(12)]", Etc.NowTime_Gen(false, DateTime.Now));
            str = str.Replace("[時刻(24)]", Etc.NowTime_Gen(true, DateTime.Now));

            str = str.Replace("[天気(簡単)]", weatherHacks.Weather_Gen(false));
            str = str.Replace("[天気(詳細)]", weatherHacks.Weather_Gen(true));
            return str;
        }

        // Voice設定のキャラクタ名を取得する
        static public string Voice_ChrName_Gen(Voice_Setting Setting)
        {
            return Setting.Style.Name;
        }
    }
}