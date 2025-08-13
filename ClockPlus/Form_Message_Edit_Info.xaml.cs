using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClockPlus
{
    /// <summary>
    /// Form_MarkdownViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class Form_Message_Edit_Info : MetroWindow
    {
        public class Item
        {
            public string Character1 { get; set; } = "";
            public string Condition1 { get; set; } = "";
            public string Text1 { get; set; } = "";

            public string Character2 { get; set; } = "";
            public string Condition2 { get; set; } = "";
            public string Text2 { get; set; } = "";

            public string Character3 { get; set; } = "";
            public string Condition3 { get; set; } = "";
            public string Text3 { get; set; } = "";
        }
        private ObservableCollection<Item> Item_List = new ObservableCollection<Item>();

        public Form_Message_Edit_Info()
        {
            InitializeComponent();
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            ListView_Gen();
        }

        private void Form_Closed(object sender, EventArgs e)
        {
            
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListView_Gen()
        {
            Item_List.Clear();
            Item item = new Item();
            item.Character1 = "[Voiceキャラクタ名]";
            item.Condition1 = "データ取得に成功";
            item.Text1 = "指定の音声キャラクタ名";
            item.Character2 = "";
            item.Condition2 = "データ取得に失敗";
            item.Text2 = "出力無し";
            Item_List.Add(item);

            item = new Item();
            item.Character1 = "[天気(簡単)]";
            item.Condition1 = "データ取得に成功";
            item.Text1 = "指定地域の天気(簡単版)";
            item.Character2 = "";
            item.Condition2 = "データ取得に失敗";
            item.Text2 = "出力無し";
            Item_List.Add(item);

            item = new Item();
            item.Character1 = "[天気(詳細)]";
            item.Condition1 = "データ取得に成功";
            item.Text1 = "指定地域の天気(詳細版)";
            item.Character2 = "";
            item.Condition2 = "データ取得に失敗";
            item.Text2 = "出力無し";
            Item_List.Add(item);

            item = new Item();
            item.Character1 = "[時刻(12)]";
            item.Condition1 = "";
            item.Text1 = "実行時の時刻(12時間表記)";
            Item_List.Add(item);

            item = new Item();
            item.Character1 = "[時刻(24)]";
            item.Condition1 = "";
            item.Text1 = "実行時の時刻(24時間表記)";
            Item_List.Add(item);

            item = new Item();
            item.Character1 = "[挨拶]";
            item.Condition1 = "05:00～09:59";
            item.Text1 = "お早うございます！";
            item.Character2 = "";
            item.Condition2 = "10:00～16:59";
            item.Text2 = "こんにちは！";
            item.Character3 = "";
            item.Condition3 = "17:00～04:59";
            item.Text3 = "こんばんは！";
            Item_List.Add(item);

            ListView_Template.ItemsSource = Item_List;
            ListView_Template.Items.Refresh();  /// <<<<<<<<<<<<<<<< これが必要
        }
    }
}
