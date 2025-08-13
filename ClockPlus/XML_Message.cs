using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace ClockPlus
{
    [XmlRoot("Msg_Template")]
    public class Message
    {
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
    }

    public class XML_Message
    {
        static public Dictionary<string, string> Msg_Temp = new Dictionary<string, string>()
        {
            { "アラーム", "時間になりました！只今の時刻は、[時刻(12)]です。" },
            { "目覚まし", "[挨拶][時刻(12)]になりました！今日も一日頑張りましょう！" },
            { "お出かけ", "お出かけの時間です！[天気(簡単)]お忘れ物の無い様に注意しましょう。" },
            { "就寝", "就寝の時間です！今日も一日お疲れさまでした。" },
            { "挨拶", "[挨拶][Voiceキャラクタ名]です。只今の時刻は、[時刻(12)]です。"},
            { "時報", "只今の時刻は、[時刻(12)]です。"},
        };

        // 実行ファイルが存在するフォルダ
        static public string XML_Message_File =
            AppDomain.CurrentDomain.BaseDirectory + "Message.xml";

        // クラス化した設定値を、どのクラスからも読み書き出来るようにする
        static public List<Message> Message_List { get; set; } = new List<Message>();

        public void XML_Message_Create()
        {
            if (File.Exists(XML_Message_File))
            {
                XML_Message_load();        // 設定を読み込む
            }
            else
            {
                XML_Message_Initialize();  // 設定の初期化
            }

        }

        public void XML_Message_save()
        {
            // XMLコード用にエスケープ(変換)する
            Text_To_XML();

            XmlSerializer serializer = new XmlSerializer(typeof(List<Message>));
            using (FileStream fileStream = new FileStream(XML_Message_File, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    serializer.Serialize(writer, Message_List);
                }
            }
        }

        public void XML_Message_load()
        {
            try
            {
                // XmlSerializerのインスタンスを作成（デシリアライズ対象の型を指定）
                XmlSerializer serializer = new XmlSerializer(typeof(List<Message>));

                // FileStreamを作成（読み込みモード）
                using (FileStream fileStream = new FileStream(XML_Message_File, FileMode.Open))
                {
                    // デシリアライズを実行
                    Message_List = (List<Message>)serializer.Deserialize(fileStream);
                }
                // XMLコード用にエスケープ(変換)された文字列を元に戻す
                XML_To_Text();
            }
            catch
            {
                FormCtrl_Wpf.Error_Message("メッセージファイルの読み込みに失敗しました。\r\nメッセージファイルが初期化されました。");
                XML_Message_Initialize();  // 設定の初期化
            }
        }

        // 初期化
        public void XML_Message_Initialize()
        {
            Message_List.Clear();
            foreach ((string Name,string Text) in Msg_Temp)
            {
                Message message = new Message();
                message.Name = Name;
                message.Text = Text;
                Message_List.Add(message);
            }
        }

        // XMLコード用にエスケープ(変換)する
        private void Text_To_XML()
        {
            for (int i = 0; i < Message_List.Count; i ++)
            {
                Message message = new Message();
                message.Name = SecurityElement.Escape(Message_List[i].Name);
                message.Text = SecurityElement.Escape(Message_List[i].Text);
                Message_List[i] = message;
            }
        }
        // XMLコード用にエスケープ(変換)された文字列を元に戻す
        private void XML_To_Text()
        {
            for (int i = 0; i < Message_List.Count; i++)
            {
                Message message = new Message();
                message.Name = System.Net.WebUtility.HtmlDecode(Message_List[i].Name);
                message.Text = System.Net.WebUtility.HtmlDecode(Message_List[i].Text);
                Message_List[i] = message;
            }
        }
    }
}
