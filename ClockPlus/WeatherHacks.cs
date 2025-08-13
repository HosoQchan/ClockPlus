using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static System.Net.WebRequestMethods;

namespace ClockPlus
{
    public class WeatherHacks
    {
        private const string url = "https://weather.tsukumijima.net/api/forecast/city/";
        private const int Access_Wait = 1;      // URLアクセス制限時間(1時間)

        public WaveOutEvent outputDevice;
        public WaveFileReader wfr;
        public bool Voice_Play_Flag = false;

        static public string[][] AreaCode =
        {
            new string [] { "011000", "宗谷地方", "宗谷地方"},
            new string [] { "012010", "上川・留萌地方", "上川地方"},
            new string [] { "012020", "上川・留萌地方", "留萌地方" },
            new string [] { "013010", "網走・北見・紋別地方", "網走地方" },
            new string [] { "013020", "網走・北見・紋別地方", "北見地方" },
            new string [] { "013030", "網走・北見・紋別地方", "紋別地方" },
            new string [] { "014010", "釧路・根室・十勝地方", "根室地方" },
            new string [] { "014020", "釧路・根室・十勝地方", "釧路地方" },
            new string [] { "014030", "釧路・根室・十勝地方", "十勝地方" },
            new string [] { "015010", "胆振・日高地方", "胆振地方" },
            new string [] { "015020", "胆振・日高地方", "日高地方" },
            new string [] { "016010", "石狩・空知・後志地方", "石狩地方" },
            new string [] { "016020", "石狩・空知・後志地方", "空知地方" },
            new string [] { "016030", "石狩・空知・後志地方", "後志地方" },
            new string [] { "017010", "渡島・檜山地方", "渡島地方" },
            new string [] { "017020", "渡島・檜山地方", "檜山地方" },
            new string [] { "020010", "青森県", "津軽" },
            new string [] { "020020", "青森県", "下北" },
            new string [] { "020030", "青森県", "三八上北" },
            new string [] { "030010", "岩手県", "内陸" },
            new string [] { "030020", "岩手県", "沿岸北部" },
            new string [] { "030030", "岩手県", "沿岸南部" },
            new string [] { "040010", "宮城県", "東部" },
            new string [] { "040020", "宮城県", "西部" },
            new string [] { "050010", "秋田県", "沿岸" },
            new string [] { "050020", "秋田県", "内陸" },
            new string [] { "060010", "山形県", "村山" },
            new string [] { "060020", "山形県", "置賜" },
            new string [] { "060030", "山形県", "庄内" },
            new string [] { "060040", "山形県", "最上" },
            new string [] { "070010", "福島県", "中通り" },
            new string [] { "070020", "福島県", "浜通り" },
            new string [] { "070030", "福島県", "会津" },
            new string [] { "080010", "茨城県", "北部" },
            new string [] { "080020", "茨城県", "南部" },
            new string [] { "090010", "栃木県", "南部" },
            new string [] { "090020", "栃木県", "北部" },
            new string [] { "100010", "群馬県", "南部" },
            new string [] { "100020", "群馬県", "北部" },
            new string [] { "110010", "埼玉県", "南部" },
            new string [] { "110020", "埼玉県", "北部" },
            new string [] { "110030", "埼玉県", "秩父地方" },
            new string [] { "120010", "千葉県", "北西部" },
            new string [] { "120020", "千葉県", "北東部" },
            new string [] { "120030", "千葉県", "南部" },
            new string [] { "130010", "東京都", "東京地方" },
            new string [] { "130020", "東京都", "伊豆諸島北部" },
            new string [] { "130030", "東京都", "伊豆諸島南部" },
            new string [] { "130040", "東京都", "小笠原諸島" },
            new string [] { "140010", "神奈川県", "東部" },
            new string [] { "140020", "神奈川県", "西部" },
            new string [] { "150010", "新潟県", "下越" },
            new string [] { "150020", "新潟県", "中越" },
            new string [] { "150030", "新潟県", "上越" },
            new string [] { "150040", "新潟県", "佐渡" },
            new string [] { "160010", "富山県", "東部" },
            new string [] { "160020", "富山県", "西部" },
            new string [] { "170010", "石川県", "加賀" },
            new string [] { "170020", "石川県", "能登" },
            new string [] { "180010", "福井県", "嶺北" },
            new string [] { "180020", "福井県", "嶺南" },
            new string [] { "190010", "山梨県", "中・西部" },
            new string [] { "190020", "山梨県", "東部・富士五湖" },
            new string [] { "200010", "長野県", "北部" },
            new string [] { "200020", "長野県", "中部" },
            new string [] { "200030", "長野県", "南部" },
            new string [] { "210010", "岐阜県", "美濃地方" },
            new string [] { "210020", "岐阜県", "飛騨地方" },
            new string [] { "220010", "静岡県", "中部" },
            new string [] { "220020", "静岡県", "伊豆" },
            new string [] { "220030", "静岡県", "東部" },
            new string [] { "220040", "静岡県", "西部" },
            new string [] { "230010", "愛知県", "西部" },
            new string [] { "230020", "愛知県", "東部" },
            new string [] { "240010", "三重県", "北中部" },
            new string [] { "240020", "三重県", "南部" },
            new string [] { "250010", "滋賀県", "南部" },
            new string [] { "250020", "滋賀県", "北部" },
            new string [] { "260010", "京都府", "南部" },
            new string [] { "260020", "京都府", "北部" },
            new string [] { "270000", "大阪府", "大阪府" },
            new string [] { "280010", "兵庫県", "南部" },
            new string [] { "280020", "兵庫県", "北部" },
            new string [] { "290010", "奈良県", "北部" },
            new string [] { "290020", "奈良県", "南部" },
            new string [] { "300010", "和歌山県", "北部" },
            new string [] { "300020", "和歌山県", "南部" },
            new string [] { "310010", "鳥取県", "東部" },
            new string [] { "310020", "鳥取県", "中・西部" },
            new string [] { "320010", "島根県", "東部" },
            new string [] { "320020", "島根県", "西部" },
            new string [] { "320030", "島根県", "隠岐" },
            new string [] { "330010", "岡山県", "南部" },
            new string [] { "330020", "岡山県", "北部" },
            new string [] { "340010", "広島県", "南部" },
            new string [] { "340020", "広島県", "北部" },
            new string [] { "350010", "山口県", "西部" },
            new string [] { "350020", "山口県", "中部" },
            new string [] { "350030", "山口県", "東部" },
            new string [] { "350040", "山口県", "北部" },
            new string [] { "360010", "徳島県", "北部" },
            new string [] { "360020", "徳島県", "南部" },
            new string [] { "370000", "香川県", "香川県" },
            new string [] { "380010", "愛媛県", "中予" },
            new string [] { "380020", "愛媛県", "東予" },
            new string [] { "380030", "愛媛県", "南予" },
            new string [] { "390010", "高知県", "中部" },
            new string [] { "390020", "高知県", "東部" },
            new string [] { "390030", "高知県", "西部" },
            new string [] { "400010", "福岡県", "福岡地方" },
            new string [] { "400020", "福岡県", "北九州地方" },
            new string [] { "400030", "福岡県", "筑豊地方" },
            new string [] { "400040", "福岡県", "筑後地方" },
            new string [] { "410010", "佐賀県", "南部" },
            new string [] { "410020", "佐賀県", "北部" },
            new string [] { "420010", "長崎県", "南部" },
            new string [] { "420020", "長崎県", "北部" },
            new string [] { "420030", "長崎県", "壱岐・対馬" },
            new string [] { "420040", "長崎県", "五島" },
            new string [] { "430010", "熊本県", "熊本地方" },
            new string [] { "430020", "熊本県", "阿蘇地方" },
            new string [] { "430030", "熊本県", "天草・芦北地方" },
            new string [] { "430040", "熊本県", "球磨地方" },
            new string [] { "440010", "大分県", "中部" },
            new string [] { "440020", "大分県", "北部" },
            new string [] { "440030", "大分県", "西部" },
            new string [] { "440040", "大分県", "南部" },
            new string [] { "450010", "宮崎県", "南部平野部" },
            new string [] { "450020", "宮崎県", "北部平野部" },
            new string [] { "450030", "宮崎県", "南部山沿い" },
            new string [] { "450040", "宮崎県", "北部山沿い" },
            new string [] { "460010", "鹿児島県", "薩摩地方" },
            new string [] { "460020", "鹿児島県", "大隅地方" },
            new string [] { "460030", "鹿児島県", "種子島・屋久島地方" },
            new string [] { "460040", "鹿児島県", "奄美地方" },
            new string [] { "471010", "沖縄県", "本島中南部" },
            new string [] { "471020", "沖縄県", "本島北部" },
            new string [] { "471030", "沖縄県", "久米島" },
            new string [] { "472000", "沖縄県", "大東島地方" },
            new string [] { "473000", "沖縄県", "宮古島地方" },
            new string [] { "474010", "沖縄県", "石垣島地方" },
            new string [] { "474020", "沖縄県", "与那国島地方" },
        };
      
        public class CityCode
        {
            public string city { get; set; }
            public string area { get; set; }
            public string Code { get; set; }
        }

        public class Tenki
        {
            public DateTime publicTime { get; set; }
            public string publicTime_format { get; set; }
            public string title { get; set; }
            public string link { get; set; }
            public Description description { get; set; }
            public Forecast[] forecasts { get; set; }
            public Location location { get; set; }
            public Copyright copyright { get; set; }
        }

        public class Description
        {
            public string text { get; set; }
            public DateTime publicTime { get; set; }
            public string publicTime_format { get; set; }
        }

        public class Location
        {
            public string city { get; set; }
            public string area { get; set; }
            public string prefecture { get; set; }
        }

        public class Copyright
        {
            public string link { get; set; }
            public string title { get; set; }
            public Image image { get; set; }
            public Provider[] provider { get; set; }
        }

        public class Image
        {
            public int width { get; set; }
            public int height { get; set; }
            public string link { get; set; }
            public string url { get; set; }
            public string title { get; set; }
        }

        public class Provider
        {
            public string link { get; set; }
            public string name { get; set; }
            public string note { get; set; }
        }

        public class Forecast
        {
            public string date { get; set; }
            public string dateLabel { get; set; }
            public string telop { get; set; }
            public Temperature temperature { get; set; }
            public Chanceofrain chanceOfRain { get; set; }
            public Image1 image { get; set; }
        }

        public class Temperature
        {
            public Min min { get; set; }
            public Max max { get; set; }
        }

        public class Min
        {
            public string celsius { get; set; }
            public string fahrenheit { get; set; }
        }

        public class Max
        {
            public string celsius { get; set; }
            public string fahrenheit { get; set; }
        }

        public class Chanceofrain
        {
            public string _0006 { get; set; }
            public string _0612 { get; set; }
            public string _1218 { get; set; }
            public string _1824 { get; set; }
            public string T00_06 { get; set; }
            public string T06_12 { get; set; }
            public string T12_18 { get; set; }
            public string T18_24 { get; set; }
        }

        public class Image1
        {
            public string title { get; set; }
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }
        static public Tenki? tenki = null;
//        static public bool Read_Flag = false;

        // 今日の天気を生成する(true=詳細版,false=簡単版)
        public string Weather_Gen(bool Detail)
        {
            // 指定されたcityCodeの天気予報データを取得する
            Get_Weather(XML_Main.cnf.Weather.AreaCode);

            string weather = "";
            if (tenki != null)
            {
                // 開始文
                weather = tenki.forecasts[0].dateLabel + "の" + tenki.title + "は、";

                // 天気
                weather = weather + tenki.forecasts[0].telop + "。";
                // 最高気温
                string tempmax = "";
                if (tenki.forecasts[0].temperature.max.celsius != null)
                {
                    tempmax = "最高気温は" + tenki.forecasts[0].temperature.max.celsius + "度";
                }
                // 最低気温
                string tempmin = "";
                if (tenki.forecasts[0].temperature.min.celsius != null)
                {
                    tempmin = "最低気温は" + tenki.forecasts[0].temperature.min.celsius + "度";
                }

                // 終了文
                weather = weather + tempmax + tempmin + "です。";

                if (Detail)
                {
                    // 詳細
                    weather = weather + tenki.description.text;
                }
            }
            return weather;
        }

        // 指定されたcityCodeの天気予報データを取得する
        public void Get_Weather(string cityCode)
        {
            DateTime get_dt = XML_Main.cnf.Weather.Acquisition.AddHours(Access_Wait);
            DateTime now_dt = DateTime.Now;

            if (tenki != null)
            {
                if (now_dt.Date == get_dt.Date)
                {
                    // 前回の取得時間から日付が同じで且、アクセス制限時間を過ぎていない場合は取得しない
                    if (now_dt <= get_dt)
                    {
                        return;
                    }
                }
            }
            XML_Main.cnf.Weather.Acquisition = now_dt;

            // 天気予報データを取得する
            string jsonurl = url + cityCode;
            try
            {
                string jsonStr = Get_StringFromUrl(jsonurl);
                tenki = JsonSerializer.Deserialize<Tenki>(jsonStr);
            }
            catch
            {
                Debug.WriteLine("天気予報データ取得エラー");
            }
        }

        public string Get_StringFromUrl(string url)
        {
            string data = "";
            using (WebClient wc = new WebClient())
            {
                try
                {
                    data = wc.DownloadString(url);
                }
                catch (WebException exc)
                {
                    Debug.WriteLine("天気予報Web取得エラー");
                }
            }
            return data;
        }

        // 天気予報(気象庁)のHPを開く
        public async void Weather_HP()
        {
            try
            {
                string URL = $"https://www.data.jma.go.jp/multi/yoho/yoho_detail.html?code={XML_Main.cnf.Weather.AreaCode}&lang=jp";
                Form_About.Link_Display(URL);

                if (Voice_Play_Flag)
                {
                    Voice_Stop();
                }
                else
                {
                    // 予報を音声で知らせるかどうか
                    if (XML_Main.cnf.Weather.Voice.Enable)
                    {
                        // Voiceの設定が利用可能な状態か
                        if (!Voice_Ctrl.Voice_Ready_Check(false))
                        {
                            return;
                        }

                        Voice_Setting voice_Setting = new Voice_Setting();
                        voice_Setting = XML_Main.cnf.Weather.Voice;

                        // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
                        if (Voice_Ctrl.Speaker_Check(voice_Setting) == 0)
                        {
                            XML_Main.cnf.Weather.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                            XML_Main.cnf.Weather.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
                            voice_Setting.Style.Name = XML_Main.cnf.Weather.Voice.Style.Name;
                            voice_Setting.Style.Type = XML_Main.cnf.Weather.Voice.Style.Type;
                        }

                        // 特殊文字を含んだメッセージを変換する
                        string text = Voice_Ctrl.Text_Replace("[天気(詳細)]", voice_Setting);

                        Voice_Play_Flag = true;
                        byte[] wav = await Voice_Ctrl.Voice_Gen(text, voice_Setting);
                        Voice_Play(wav, 100, voice_Setting.Query.Device);
                    }
                }
            }
            catch
            {
                FormCtrl_Wpf.Error_Message("リンクが開けませんでした。");
            }
        }

        public void Voice_Stop()
        {
            if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                wfr.Close();
                outputDevice.Dispose();
            }
            Voice_Play_Flag = false;
        }

        public void Voice_Play(byte[] wavBytes, int Vol, int DeviceNo)
        {
            try
            {
                Voice_Stop();
                outputDevice = new WaveOutEvent();
                wfr = new WaveFileReader(new MemoryStream(wavBytes));
                outputDevice.DeviceNumber = DeviceNo;
                outputDevice.Init(wfr);
                outputDevice.Volume = (float)(Vol / 100f);
                outputDevice.Play();
                outputDevice.PlaybackStopped += Playback_Complete;
            }
            catch
            {
                Voice_Play_Flag = false;
            }
        }
        // 再生完了のイベント処理
        public void Playback_Complete(object sender, EventArgs e)
        {
            outputDevice.Dispose();
            wfr.Dispose();

            Voice_Play_Flag = false;
        }
    }
}
