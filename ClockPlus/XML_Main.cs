using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ClockPlus
{
    // 基本設定
    [XmlRoot("Main")]
    public class XML_Main
    {
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

    [XmlRoot("Signal")]
    public class Signal          // 時報設定
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
        [XmlElement("Enable")]
        public bool Enable { get; set; }
        [XmlElement("Event_Timer")]
        public int Event_Timer { get; set; }
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

    public class XML_Main_IO
    {
        // 設定保存用フォルダ
        static public string App_Data_Path = "";

        // 設定保存用ファイル名
        static public string XML_Main_File = "";

        // skinフォルダ
        static public string Skin_Dir = "";

        // Soundフォルダ
        static public string Sound_Dir = "";

        // クラス化した設定値を、どのクラスからも読み書き出来るようにする
        static public XML_Main Main = new XML_Main();

        XmlSerializer serializer = new XmlSerializer(typeof(XML_Main));
        FontConverter converter = new FontConverter();

        public void XML_Main_Create()
        {
            DirectoryInfo Info;
            // ローカルのApplication Dataフォルダのパスを取得
            string Path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // 設定保存用フォルダ
            Info = Directory.CreateDirectory(Path + "\\ClockPlus");
            App_Data_Path = Info.FullName + "\\";

            // 設定保存用ファイル名
            XML_Main_File = App_Data_Path + App.Title + ".xml";

            // Soundフォルダを作成
            Info = Directory.CreateDirectory(App_Data_Path + "sound");
            Sound_Dir = Info.FullName;

            // skinフォルダを作成
            Info = Directory.CreateDirectory(App_Data_Path + "skin");
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
            FileStream save_data = new FileStream(XML_Main_File, FileMode.Create);
            StreamWriter writer = new StreamWriter(save_data, Encoding.UTF8);
            serializer.Serialize(writer, Main);
            writer.Flush();
            writer.Close();
        }

        public void XML_Main_load()
        {
            FileStream load_Data = new FileStream(XML_Main_File, FileMode.Open);
            Main = (XML_Main)serializer.Deserialize(load_Data);
            load_Data.Close();
        }

        // 初期化
        public void XML_Main_Initialize()
        {
            Font font = new Font(SystemFonts.DefaultFont.FontFamily, 36);
            string Font_Color = Etc.RGB_Code(Color.Violet);
            string Back_Color = Etc.RGB_Code(Color.Green);

            string OutlineColor = Etc.RGB_Code(Color.White);
            string Colo1 = Etc.RGB_Code(Color.White);
            string Colo2 = Etc.RGB_Code(Color.Black);

            XML_Main_IO.Main.AutoStart = false;
            XML_Main_IO.Main.Password = "";

            Signal _Signal = new Signal();
            _Signal.Enable = true;
            _Signal.FileName = Sound_Dir + "\\Jihou.wav";
            _Signal.Device = 0;
            _Signal.Volume = 100;
            _Signal.Cycle = 60;
            XML_Main_IO.Main.Signal = _Signal;

            Voice _Voice = new Voice();
            _Voice.Enable = true;
            _Voice.Volume = 100;
            _Voice.Rate = -2;
            XML_Main_IO.Main.Voice = _Voice;

            Display _Display = new Display();
            _Display.Enable = true;
            _Display.Event_Timer = 0;

            Analog _Analog = new Analog();
            _Analog.Enable = true;
            _Analog.Skin = "default";
            _Analog.PosX = Etc.Disp_Width / 2;
            _Analog.PosY = Etc.Disp_Height / 2;
            _Analog.Zoom = 0.3;
            _Analog.Opacity = 1.0;
            _Analog.Clear_Background = true;
            _Analog.Display_Event = false;
            _Analog.TopMost = false;
            _Display.Analog = _Analog;

            Digital _Digital1 = new Digital();
            _Digital1.Enable = true;
            _Digital1.PosX = Etc.Disp_Width / 2;
            _Digital1.PosY = Etc.Disp_Height / 2;
            _Digital1.Font = Etc.Font_fromFnt(font);
            _Digital1.Font_Color = Etc.RGB_Code(Color.Violet);
            _Digital1.Back_Color = Back_Color;
            _Digital1.Outline_Color = OutlineColor;
            _Digital1.Outline_Thick = 0;
            _Digital1.Font_Color = Font_Color;
            _Digital1.Back_Color = Back_Color;
            _Digital1.Outline_Color = OutlineColor;
            _Digital1.Paint_Style = "Off";
            _Digital1.LGB_Mode = 0;
            _Digital1.LGB_Color1 = Colo1;
            _Digital1.LGB_Color2 = Colo2;
            _Digital1.HB_Style = 0;
            _Digital1.HB_Color1 = Colo1;
            _Digital1.HB_Color2 = Colo2;
            _Digital1.Opacity = 1.0;
            _Digital1.Zoom = 1.0;
            _Digital1.Clear_Background = true;
            _Digital1.Display_Event = false;
            _Digital1.TopMost = false;

            Digital _Digital2 = new Digital();
            _Digital2 = Etc.JsonSerializerClone(_Digital1);

            _Digital1.Format = "yyyy/MM/dd (ddd)";
            _Digital2.Format = "HH:mm:ss";
            _Digital2.Font_Color = Etc.RGB_Code(Color.LimeGreen);
            _Display.Digtal = new List<Digital>();
            _Display.Digtal.Add(_Digital1);
            _Display.Digtal.Add(_Digital2);
            XML_Main_IO.Main.Display = _Display;
        }
    }
}
