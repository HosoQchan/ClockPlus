using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClockPlus
{
    public partial class MessageBox_Ex : MetroWindow
    {
        public enum MessageType
        {
            Info,
            Confirmation,
            Success,
            Warning,
            Error,
        }
        public enum MessageButtons
        {
            OkCancel,
            YesNo,
            Ok,
        }

        public MessageBox_Ex(string Message, MessageType Type, MessageButtons Buttons)
        {
            InitializeComponent();
            txtMessage.Text = Message;
            switch (Type)
            {
                case MessageType.Info:
                    this.Title = "情報";
                    break;
                case MessageType.Confirmation:
                    this.Title = "確認";
                    break;
                case MessageType.Success:
                    this.Title = "確認";
                    break;
                case MessageType.Warning:
                    this.Title = "警告";
                    break;
                case MessageType.Error:
                    this.Title = "エラー";
                    break;
            }
            switch (Buttons)
            {
                case MessageButtons.OkCancel:
                    btnYes.Visibility = Visibility.Collapsed; btnNo.Visibility = Visibility.Collapsed;
                    break;
                case MessageButtons.YesNo:
                    btnOk.Visibility = Visibility.Collapsed; btnCancel.Visibility = Visibility.Collapsed;
                    break;
                case MessageButtons.Ok:
                    btnOk.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnYes.Visibility = Visibility.Collapsed; btnNo.Visibility = Visibility.Collapsed;
                    break;
            }
            SetupButtonImage(Type);
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        //---------------------------------------------------------------------------------------------
        // MessageType プロパティに基づいてアイコンを設定する
        private void SetupButtonImage(MessageType Type)
        {
            StockIconId id = StockIconId.SIID_INFO;
            switch (Type)
            {
                case MessageType.Info:
                    id = StockIconId.SIID_INFO;
                    // メッセージ（情報）を鳴らす
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case MessageType.Confirmation:
                    id = StockIconId.SIID_HELP;
                    // メッセージ（情報）を鳴らす
                    System.Media.SystemSounds.Asterisk.Play();
                    /*
                    // メッセージ（問い合わせ）を鳴らす
                    System.Media.SystemSounds.Question.Play();
                    */
                    break;
                case MessageType.Success:
                    id = StockIconId.SIID_HELP;
                    // メッセージ（情報）を鳴らす
                    System.Media.SystemSounds.Asterisk.Play();
                    /*
                    // メッセージ（問い合わせ）を鳴らす
                    System.Media.SystemSounds.Question.Play();
                    */
                    break;
                case MessageType.Warning:
                    id = StockIconId.SIID_WARNING;
                    // メッセージ（警告）を鳴らす
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case MessageType.Error:
                    id = StockIconId.SIID_ERROR;
                    // システムエラーを鳴らす
                    System.Media.SystemSounds.Hand.Play();
                    break;
            }
            PART_Image.Source = this.GetStockIconById(id, StockIconFlags.Large);
        }

        //*********************************************************************************************
        // ダイアログ用アイコンを Windows ストックアイコンから取得する

        [DllImport("User32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("Shell32.dll")]
        private static extern IntPtr SHGetStockIconInfo(
            StockIconId siid,   // 取得するアイコンの IDを指定する StockIconId enum 型
            StockIconFlags uFlags, // 取得するアイコンの種類を指定する StockIconFlags enum 型
            ref StockIconInfo psii    //（戻り値）StockIconInfo 型
        );

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct StockIconInfo
        {
            public uint cbSize;        // 構造体のサイズ（バイト数）
            public IntPtr hIcon;       //（戻り値）アイコンへのハンドル
            public int iSysImageIndex; //（戻り値）システムアイコンキャッシュ内のアイコンのインデックス
            public int iIcon;          //（戻り値）取り出したアイコンのインデックス
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;      //（戻り値）アイコンリソースを保持するファイル名 
        }

        [Flags]
        private enum StockIconFlags
        {
            Large = 0x000000000,       // 大きなアイコン
            Small = 0x000000001,       // 小さなアイコン
            ShellSize = 0x000000004,   // シェルのサイズのアイコン
            Handle = 0x000000100,      // 指定のアイコンのハンドル
            SystemIndex = 0x000004000, // システムのイメージリストのインデックス
            LinkOverlay = 0x000008000, // リンクを示すオーバーレイ付きのアイコン
            Selected = 0x000010000     // 選択状態のアイコン
        }

        private enum StockIconId
        {
            SIID_HELP = 23,
            SIID_WARNING = 78,
            SIID_INFO = 79,
            SIID_ERROR = 80
        }

        //---------------------------------------------------------------------------------------------
        // 指定のストックアイコンを取得する
        // id   : アイコンの種類を指定する StockIconId enum 型
        // flag : アイコンのサイズを指定する StockIconFlags enum 型
        private BitmapSource GetStockIconById(StockIconId id, StockIconFlags flag)
        {
            BitmapSource bitmapSource = null;
            StockIconFlags flags = StockIconFlags.Handle | flag;

            var info = new StockIconInfo();
            info.cbSize = (uint)Marshal.SizeOf(typeof(StockIconInfo));

            IntPtr result = SHGetStockIconInfo(id, flags, ref info);

            if (info.hIcon != IntPtr.Zero)
            {
                bitmapSource = Imaging.CreateBitmapSourceFromHIcon(info.hIcon, Int32Rect.Empty, null);
                DestroyIcon(info.hIcon);
            }

            return bitmapSource;
        }
    }
}
