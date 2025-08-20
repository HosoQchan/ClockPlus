using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ClockPlus
{
    // 基本設定
    [XmlRoot("Cnf")]
    public class Cnf
    {
        [XmlElement("AutoStart")]
        public bool AutoStart { get; set; }
        [XmlElement("Password")]
        public string Password { get; set; }
        [XmlElement("Theme")]
        public Theme Theme { get; set; }
        [XmlElement("Weather")]
        public Weather Weather { get; set; }
        [XmlElement("Signal")]
        public Signal Signal { get; set; }
        [XmlElement("Voice")]
        public VoiceEngine VoiceEngine { get; set; }
        [XmlElement("Display")]
        public Display Display { get; set; }
    }

    [XmlRoot("Theme")]
    public class Theme          // テーマ設定
    {
        [XmlElement("Mode")]
        public string Mode { get; set; }
        [XmlElement("Color")]
        public string Color { get; set; }
    }

    [XmlRoot("Weather")]
    public class Weather        // 天気
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; }
        [XmlElement("Area")]
        public string AreaCode { get; set; }
        [XmlElement("Acquisition")]
        public DateTime Acquisition { get; set; }
        [XmlElement("Voice")]
        public Voice_Setting Voice { get; set; }
        [XmlElement("Weather_Disp")]
        public List<Weather_Disp> Weather_Disp { get; set; }
    }

    [XmlRoot("Weather_Disp")]
    public class Weather_Disp
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; }
        [XmlElement("Font_Color")]
        public string Font_Color { get; set; }
        [XmlElement("Font_Color1")]
        public string Font_Color1 { get; set; }
        [XmlElement("Font_Color2")]
        public string Font_Color2 { get; set; }
        [XmlElement("Back_Color")]
        public string Back_Color { get; set; }
        [XmlElement("Clear_Background")]
        public bool Clear_Background { get; set; }
        [XmlElement("PosX")]
        public int PosX { get; set; }
        [XmlElement("PosY")]
        public int PosY { get; set; }
        [XmlElement("Opacity")]
        public double Opacity { get; set; }
        [XmlElement("Zoom")]
        public double Zoom { get; set; }
        [XmlElement("TopMost")]
        public bool TopMost { get; set; }
    }

    [XmlRoot("Signal")]
    public class Signal         // 時報設定
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; }
        [XmlElement("FileName")]
        public string FileName { get; set; }
        [XmlElement("Device")]
        public int Device { get; set; }
        [XmlElement("Volume")]
        public int Volume { get; set; }
        [XmlElement("Cycle")]
        public int Cycle { get; set; }
        [XmlElement("Voice")]
        public Voice_Setting Voice { get; set; }
    }

    [XmlRoot("VoiceEngine")]
    public class VoiceEngine     // 音声合成設定
    {
        [XmlElement("Engine_Name")]
        public string Engine_Name { get; set; }
        [XmlElement("Path")]
        public string Path { get; set; }
    }

    [XmlRoot("Display")]
    public class Display         // 表示設定
    {
        [XmlElement("Analog")]
        public Analog Analog { get; set; }
        [XmlElement("Digtal")]
        public List<Digital> Digtal { get; set; }
    }

    [XmlRoot("Analog")]
    public class Analog         // アナログ時計表示設定
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; }
        [XmlElement("Skin")]
        public string Skin { get; set; }
        [XmlElement("PosX")]
        public int PosX { get; set; }
        [XmlElement("PosY")]
        public int PosY { get; set; }
        [XmlElement("Opacity")]
        public double Opacity { get; set; }
        [XmlElement("Zoom")]
        public double Zoom { get; set; }
        [XmlElement("Clear_Background")]
        public bool Clear_Background { get; set; }
        [XmlElement("Display_Event")]
        public bool Display_Event { get; set; }
        [XmlElement("TopMost")]
        public bool TopMost { get; set; }
    }
    [XmlRoot("Digital")]
    public class Digital        // デジタル表示設定
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; }
        [XmlElement("PosX")]
        public int PosX { get; set; }
        [XmlElement("PosY")]
        public int PosY { get; set; }
        [XmlElement("Format")]
        public string Format { get; set; }
        [XmlElement("Font")]
        public string Font { get; set; }
        [XmlElement("Font_Color")]
        public string Font_Color { get; set; }
        [XmlElement("Back_Color")]
        public string Back_Color { get; set; }
        [XmlElement("Outline_Color")]
        public string Outline_Color { get; set; }
        [XmlElement("Outline_Thick")]
        public int Outline_Thick { get; set; }
        [XmlElement("Paint_Style")]
        public string Paint_Style { get; set; }
        [XmlElement("LGB_Mode")]
        public int LGB_Mode { get; set; }
        [XmlElement("LGB_Color1")]
        public string LGB_Color1 { get; set; }
        [XmlElement("LGB_Color2")]
        public string LGB_Color2 { get; set; }
        [XmlElement("HB_Style")]
        public int HB_Style { get; set; }
        [XmlElement("HB_Color1")]
        public string HB_Color1 { get; set; }
        [XmlElement("HB_Color2")]
        public string HB_Color2 { get; set; }
        [XmlElement("Opacity")]
        public double Opacity { get; set; }
        [XmlElement("Zoom")]
        public double Zoom { get; set; }
        [XmlElement("Clear_Background")]
        public bool Clear_Background { get; set; }
        [XmlElement("Display_Event")]
        public bool Display_Event { get; set; }
        [XmlElement("TopMost")]
        public bool TopMost { get; set; }
    }

    public class XML_Main
    {
        // アプリケーションのフォルダ
        static public string App_Path;

        // 設定保存用ファイル名
        static public string XML_Main_File = "";

        // Soundフォルダ
        static public string Sound_Dir = "";

        // skinフォルダ
        static public string Skin_Dir = "";

        //ディスプレイの高さ
        static public int Display_Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        //ディスプレイの幅
        static public int Display_Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

        // クラス化した設定値を、どのクラスからも読み書き出来るようにする
        static public Cnf cnf = new Cnf();

        public void XML_Main_Create()
        {
            DirectoryInfo Info;
            // アプリケーションのフォルダ
            App_Path = System.AppDomain.CurrentDomain.BaseDirectory;

            // 設定保存用ファイル名
            XML_Main_File = App_Path + "ClockPlus.xml";

            // Soundフォルダを作成
            Info = Directory.CreateDirectory(XML_Main.App_Path + "sound");
            XML_Main.Sound_Dir = Info.FullName;

            // skinフォルダを作成
            Info = Directory.CreateDirectory(App_Path + "skin");
            Skin_Dir = Info.FullName;

            if (File.Exists(XML_Main_File))
            {
                XML_Main_load();        // 設定を読み込む
            }
            else
            {
                XML_Main_Initialize();  // 設定の初期化
            }

            // スタートアップフォルダに登録済みか確認
            if (File.Exists(App.LnkFile))
            {
                cnf.AutoStart = true;
            }
            else
            {
                cnf.AutoStart = false;

            }
        }

        public void XML_Main_save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Cnf));
            using (FileStream fileStream = new FileStream(XML_Main_File, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    serializer.Serialize(writer, cnf);
                }
            }
        }

        public void XML_Main_load()
        {
            try
            {
                // XmlSerializerのインスタンスを作成（デシリアライズ対象の型を指定）
                XmlSerializer serializer = new XmlSerializer(typeof(Cnf));

                // FileStreamを作成（読み込みモード）
                using (FileStream fileStream = new FileStream(XML_Main_File, FileMode.Open))
                {
                    // デシリアライズを実行
                    cnf = (Cnf)serializer.Deserialize(fileStream);
                }
            }
            catch
            {
                FormCtrl_Wpf.Error_Message("設定ファイルの読み込みに失敗しました。\r\n設定が初期化されました。");
                XML_Main_Initialize();  // 設定の初期化
                return;
            }

            // 天気予報の表示位置が範囲内かの確認
            cnf.Weather.Weather_Disp[0].PosX = PosX_Check(cnf.Weather.Weather_Disp[0].PosX);
            cnf.Weather.Weather_Disp[0].PosY = PosY_Check(cnf.Weather.Weather_Disp[0].PosY);

            cnf.Weather.Weather_Disp[1].PosX = PosX_Check(cnf.Weather.Weather_Disp[1].PosX);
            cnf.Weather.Weather_Disp[1].PosY = PosY_Check(cnf.Weather.Weather_Disp[1].PosY);

            // 時計の表示位置が範囲内かの確認
            cnf.Display.Analog.PosX = PosX_Check(cnf.Display.Analog.PosX);
            cnf.Display.Analog.PosY = PosY_Check(cnf.Display.Analog.PosY);

            cnf.Display.Digtal[0].PosX = PosX_Check(cnf.Display.Digtal[0].PosX);
            cnf.Display.Digtal[0].PosY = PosY_Check(cnf.Display.Digtal[0].PosY);

            cnf.Display.Digtal[1].PosX = PosX_Check(cnf.Display.Digtal[1].PosX);
            cnf.Display.Digtal[1].PosY = PosY_Check(cnf.Display.Digtal[1].PosY);
        }

        private int PosX_Check(int PosX)
        {
            if (PosX < 0)
            {
                return 0;
            }
            if (PosX > Display_Width)
            {
                return XML_Main.Display_Width / 2;
            }
            return PosX;
        }
        private int PosY_Check(int PosY)
        {
            if (PosY < 0)
            {
                return 0;
            }
            if (PosY > Display_Height)
            {
                return XML_Main.Display_Height / 2;
            }
            return PosY;
        }

        // 初期化
        public void XML_Main_Initialize()
        {
            XML_Main.cnf.AutoStart = false;
            XML_Main.cnf.Password = "";

            Theme theme = new Theme();
            theme.Mode = "Auto";
            theme.Color = "Steel";
            XML_Main.cnf.Theme = theme;

            Weather weather = new Weather();
            weather.Enable = false;
            weather.AreaCode = "130010";
            weather.Acquisition = DateTime.MinValue;
            weather.Voice = new Voice_Setting();
            Weather_Disp weather_Disp1 = new Weather_Disp();
            weather_Disp1 = XML_Display_Weather_Initialize();
            Weather_Disp weather_Disp2 = new Weather_Disp();
            weather_Disp2 = XML_Display_Weather_Initialize();
            weather_Disp2.Enable = false;
            weather.Weather_Disp = new List<Weather_Disp>();
            weather.Weather_Disp.Clear();
            weather.Weather_Disp.Add(weather_Disp1);
            weather.Weather_Disp.Add(weather_Disp2);
            XML_Main.cnf.Weather = weather;

            Signal signal = new Signal();
            signal.Enable = true;
            signal.FileName = Sound_Dir + "\\Jihou.wav";
            signal.Device = 0;
            signal.Volume = 100;
            signal.Cycle = 60;
            signal.Voice = new Voice_Setting();
            XML_Main.cnf.Signal = signal;

            VoiceEngine VoiceEngine = new VoiceEngine();
            VoiceEngine.Engine_Name = "無し";
            VoiceEngine.Path = "";
            XML_Main.cnf.VoiceEngine = VoiceEngine;

            Display display = new Display();
            Analog analog = new Analog();
            Digital digital1 = new Digital();
            Digital digital2 = new Digital();

            analog = XML_Display_Analog_Initialize();
            digital1 = XML_Display_Digital_Initialize(0);
            digital2 = XML_Display_Digital_Initialize(1);
            digital2.Enable = false;

            display.Analog = analog;
            display.Digtal = new List<Digital>();
            display.Digtal.Clear();
            display.Digtal.Add(digital1);
            display.Digtal.Add(digital2);
            XML_Main.cnf.Display = display;
        }

        // 初期化
        static public Analog XML_Display_Analog_Initialize()
        {
            Analog analog = new Analog();
            analog.Enable = true;
            analog.Skin = "default";
            analog.PosX = XML_Main.Display_Width / 2;
            analog.PosY = XML_Main.Display_Height / 2;
            analog.Opacity = 1.0;
            analog.Zoom = 0.4;
            analog.Clear_Background = true;
            analog.Display_Event = false;
            analog.TopMost = false;
            return analog;
        }
        static public Digital XML_Display_Digital_Initialize(int DisplayNo)
        {
            System.Drawing.Font font = new System.Drawing.Font(SystemFonts.DefaultFont.FontFamily, 36);
            string Font_Color = Etc.RGB_Code(Color.Violet);
            string Back_Color = Etc.RGB_Code(Color.Green);

            string OutlineColor = Etc.RGB_Code(Color.White);
            string Colo1 = Etc.RGB_Code(Color.White);
            string Colo2 = Etc.RGB_Code(Color.Black);

            Digital digital = new Digital();
            digital.Enable = true;
            digital.PosX = XML_Main.Display_Width / 2;
            digital.PosY = XML_Main.Display_Height / 2;
//            digital.Format = "yyyy/MM/dd (ddd)";
            digital.Font = Etc.Font_fromFnt(font);
//            digital.Font_Color = Font_Color;
            digital.Back_Color = Back_Color;
            digital.Outline_Color = OutlineColor;
            digital.Outline_Thick = 0;
            digital.Paint_Style = "Off";
            digital.LGB_Mode = 0;
            digital.LGB_Color1 = Colo1;
            digital.LGB_Color2 = Colo2;
            digital.HB_Style = 0;
            digital.HB_Color1 = Colo1;
            digital.HB_Color2 = Colo2;
            digital.Opacity = 1.0;
            digital.Zoom = 1.0;
            digital.Clear_Background = true;
            digital.Display_Event = false;
            digital.TopMost = false;

            if (DisplayNo == 0)
            {
                digital.Font_Color = Etc.RGB_Code(Color.Violet);
                digital.Format = "yyyy/MM/dd (ddd)";
            }
            else
            {
                digital.Font_Color = Etc.RGB_Code(Color.LimeGreen);
                digital.Format = "HH:mm:ss";
            }
            return digital;
        }
        static public Weather_Disp XML_Display_Weather_Initialize()
        {
            Weather_Disp disp = new Weather_Disp();
            disp.Enable = true;
            disp.Font_Color = Etc.RGB_Code(Color.White);
            disp.Font_Color1 = Etc.RGB_Code(Color.DeepPink);
            disp.Font_Color2 = Etc.RGB_Code(Color.DodgerBlue);
            disp.Back_Color = Etc.RGB_Code(Color.Black);
            disp.Clear_Background = true;
            disp.PosX = XML_Main.Display_Width / 2;
            disp.PosY = XML_Main.Display_Height / 2;
            disp.Opacity = 1.0;
            disp.Zoom = 1.0;
            disp.TopMost = false;
            return disp;
        }

        // 設定されているスピーカー名が存在しない場合、デフォルト値に戻す処理
        static public void Voice_Setting_Check()
        {
            Voice_Setting setting = new Voice_Setting();
            setting = XML_Main.cnf.Signal.Voice;
            if (Voice_Ctrl.Speaker_Check(setting) == 0)
            {
                XML_Main.cnf.Signal.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                XML_Main.cnf.Signal.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
            }
            setting = new Voice_Setting();
            setting = XML_Main.cnf.Weather.Voice;
            if (Voice_Ctrl.Speaker_Check(setting) == 0)
            {
                XML_Main.cnf.Weather.Voice.Style.Name = Voice_Ctrl.speakers[0].Name;
                XML_Main.cnf.Weather.Voice.Style.Type = Voice_Ctrl.speakers[0].Styles[0].Name;
            }
        }
    }
}
