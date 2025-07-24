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
        [XmlElement("Theme")]
        public Theme Theme { get; set; }
        [XmlElement("AutoStart")]
        public bool AutoStart { get; set; }
        [XmlElement("Password")]
        public string Password { get; set; }
        [XmlElement("Signal")]
        public Signal Signal { get; set; }
        [XmlElement("Voice")]
        public Voice Voice { get; set; }
        [XmlElement("Display")]
        public Display Display { get; set; }
    }

    [XmlRoot("Theme")]
    public class Theme         // テーマ設定
    {
        [XmlElement("Mode")]
        public string Mode { get; set; }
        [XmlElement("Color")]
        public string Color { get; set; }
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
    }

    [XmlRoot("Voice")]
    public class Voice          // 音声合成設定
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; }
        [XmlElement("Volume")]
        public int Volume { get; set; }
        [XmlElement("Rate")]
        public int Rate { get; set; }
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

            // リソースからサウンドファイルを作成する
            Sound_Ctrl sound_ctrl = new Sound_Ctrl();
            sound_ctrl.Create_Sound_Files();

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

            // 設定値が範囲外だった時は初期化する
            if (cnf.AutoStart)
            {
                // スタートアップフォルダに登録済みか確認
                if (!File.Exists(App.LnkFile))
                {
                    cnf.AutoStart = false;
                }
            }

            // 時計の表示位置が範囲内かの確認
            if ((cnf.Display.Analog.PosX > Display_Width) || (cnf.Display.Analog.PosY > Display_Height))
            {
                cnf.Display.Analog.PosX = XML_Main.Display_Width / 2;
                cnf.Display.Analog.PosY = XML_Main.Display_Height / 2;
            }

            if ((cnf.Display.Digtal[0].PosX > Display_Width) || (cnf.Display.Digtal[0].PosY > Display_Height))
            {
                cnf.Display.Digtal[0].PosX = XML_Main.Display_Width / 2;
                cnf.Display.Digtal[0].PosY = XML_Main.Display_Height / 2;
            }

            if ((cnf.Display.Digtal[1].PosX > Display_Width) || (cnf.Display.Digtal[1].PosY > Display_Height))
            {
                cnf.Display.Digtal[1].PosX = XML_Main.Display_Width / 2;
                cnf.Display.Digtal[1].PosY = XML_Main.Display_Height / 2;
            }
        }

        // 初期化
        public void XML_Main_Initialize()
        {
            Theme theme = new Theme();
            theme.Mode = "Auto";
            theme.Color = "Steel";
            XML_Main.cnf.Theme = theme;

            XML_Main.cnf.AutoStart = false;
            XML_Main.cnf.Password = "";

            Signal signal = new Signal();
            signal.Enable = true;
            signal.FileName = Sound_Dir + "\\Jihou.wav";
            signal.Device = 0;
            signal.Volume = 100;
            signal.Cycle = 60;
            XML_Main.cnf.Signal = signal;

            Voice voice = new Voice();
            voice.Enable = false;
            voice.Volume = 100;
            voice.Rate = -2;
            XML_Main.cnf.Voice = voice;

            Display display = new Display();
            Analog analog = new Analog();
            Digital digital1 = new Digital();
            Digital digital2 = new Digital();

            analog = XML_Display_Analog_Initialize();
            digital1 = XML_Display_Digital_Initialize(0);
            digital2 = XML_Display_Digital_Initialize(1);

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
            analog.Zoom = 0.5;
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
    }
}
