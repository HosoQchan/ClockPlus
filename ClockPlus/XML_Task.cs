using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;
using static ClockPlus.Date_Picker;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace ClockPlus
{
    public class ListView_Task_Item
    {
        public int List_No { get; set; } = -1;
        public bool Enable { get; set; } = false;
        public string Name { get; set; } = "";
        public string Cycle { get; set; } = "";
        public string Year { get; set; } = "";
        public string Month { get; set; } = "";
        public string Day { get; set; } = "";
        public string Hour { get; set; } = "";
        public string Min { get; set; } = "";
        public string Alarm { get; set; } = "";
        public string App { get; set; } = "";
        public string Power { get; set; } = "";

        public bool Mon { get; set; } = false;
        public bool Tue { get; set; } = false;
        public bool Wed { get; set; } = false;
        public bool Thu { get; set; } = false;
        public bool Fri { get; set; } = false;
        public bool Sat { get; set; } = false;
        public bool Sun { get; set; } = false;

        public Visibility Grid_Week { get; set; } = Visibility.Hidden;
        public Visibility Grid_Date { get; set; } = Visibility.Hidden;
        public Visibility Grid_Year { get; set; } = Visibility.Hidden;
        public Visibility Grid_Month { get; set; } = Visibility.Hidden;

        public Visibility Grid_AlarmApp { get; set; } = Visibility.Hidden;
        public Visibility Grid_Power { get; set; } = Visibility.Hidden;
    }

    // タスク
    [XmlRoot("Task_List")]
    public class Task_List
    {
        public List<Task> TaskList {  get; set; } = new List<Task>();
    }

    [XmlRoot("Task")]
    public class Task
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; } = false;
        [XmlElement("Type")]
        public string Type { get; set; } = "Alarm";
        [XmlElement("Name")]
        public string Name { get; set; } = "";
        [XmlElement("Cycle")]
        public string Cycle { get; set; } = "一回";
        [XmlElement("Dtime")]
        public DateTime DateTime { get; set; } = DateTime.MinValue;
        [XmlElement("Timer")]
        public DateTime Timer { get; set; } = DateTime.MinValue;
        [XmlElement("Day_List")]
        public Week_List Week_List { get; set; } = new Week_List();
        [XmlElement("Sound")]
        public Task_Sound Sound { get; set; } = new Task_Sound();
        [XmlElement("App")]
        public Task_Program App { get; set; } = new Task_Program();
        [XmlElement("Power")]
        public Task_Power Power { get; set; } = new Task_Power();
    }

    // 曜日リスト
    [XmlRoot("Week_List")]
    public class Week_List          
    {
        [XmlElement("Mon")]
        public bool Mon { get; set; } = false;
        [XmlElement("Tue")]
        public bool Tue { get; set; } = false;
        [XmlElement("Wed")]
        public bool Wed { get; set; } = false;
        [XmlElement("Thu")]
        public bool Thu { get; set; } = false;
        [XmlElement("Fri")]
        public bool Fri { get; set; } = false;
        [XmlElement("Sat")]
        public bool Sat { get; set; } = false;
        [XmlElement("Sun")]
        public bool Sun { get; set; } = false;
    }

    // 音声設定
    [XmlRoot("Task_Sound")]
    public class Task_Sound
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; } = true;
        [XmlElement("FileName")]
        public string FileName { get; set; } = "";
        [XmlElement("Device")]
        public int Device { get; set; } = 0;
        [XmlElement("Volume")]
        public int Volume { get; set; } = 100;
        [XmlElement("Repeat")]
        public int Repeat { get; set; } = 0;
        [XmlElement("Password_Enable")]
        public bool Password_Enable { get; set; } = false;
    }

    // プログラム設定 
    [XmlRoot("Task_Program")]
    public class Task_Program
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; } = false;
        [XmlElement("FileName")]
        public string FileName { get; set; } = "";
        [XmlElement("Option")]
        public string Option { get; set; } = "";
    }

    // 電源制御 
    [XmlRoot("Task_Power")]
    public class Task_Power
    {
        [XmlElement("Enable")]
        public bool Enable { get; set; } = false;
        [XmlElement("Mode")]
        public string Mode { get; set; } = "";
    }

    public class XML_Task
    {
        // 実行ファイルが存在するフォルダ
        static public string XML_Task_File =
            AppDomain.CurrentDomain.BaseDirectory + "Task.xml";

        // クラス化した設定値を、どのクラスからも読み書き出来るようにする
        static public Task_List Task = new Task_List();

        public void XML_Task_Create()
        {
            if (File.Exists(XML_Task_File))
            {
                XML_Task_load();        // 設定を読み込む
            }
            else
            {
                XML_Task_Initialize();  // 設定の初期化
            }

        }

        // タスクリストの初期化
        public void XML_Task_Initialize()
        {
            Task.TaskList.Clear();
        }

        public void XML_Task_save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Task_List));
            using (FileStream fileStream = new FileStream(XML_Task_File, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    serializer.Serialize(writer, Task);
                }
            }
        }

        public void XML_Task_load()
        {
            try
            {
                // XmlSerializerのインスタンスを作成（デシリアライズ対象の型を指定）
                XmlSerializer serializer = new XmlSerializer(typeof(Task_List));

                // FileStreamを作成（読み込みモード）
                using (FileStream fileStream = new FileStream(XML_Task_File, FileMode.Open))
                {
                    // デシリアライズを実行
                    Task = (Task_List)serializer.Deserialize(fileStream);
                }
            }
            catch
            {
                FormCtrl_Wpf.Error_Message("データの読み込みに失敗しました。\r\nデータが初期化されました。");
                XML_Task_Initialize();  // タスクリストの初期化
                return;
            }
        }

        static public bool Task_Name_Check(string Type,string Name)
        {
            bool Flag = false;
            foreach (Task task in Task.TaskList)
            {
                if ((task.Type == Type) && (task.Name == Name))
                {
                    Flag = true;
                    break;
                }
            }
            return Flag;
        }
    }
}
