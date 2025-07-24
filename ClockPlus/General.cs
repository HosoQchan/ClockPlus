using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Diagnostics;
using System.Security.Principal;

namespace ClockPlus
{
    static public class Etc
    {
        static public FontConverter converter = new FontConverter();

        // string型のフォントをFont型に変換
        static public Font Font_fromStr(string fnt)
        {
            FontConverter cvt = new FontConverter();
            Font f = cvt.ConvertFromString(fnt) as Font;
            return f;

            //        return (Font)converter.ConvertFromInvariantString(fnt);
        }
        // Font型のフォントをstring型に変換
        static public string Font_fromFnt(Font fnt)
        {
            FontConverter cvt = new FontConverter();
            string s = cvt.ConvertToString(fnt);
            return s;
        }

        // Color型からRGB形式の文字列に変換
        static public string RGB_Code(System.Drawing.Color cor)
        {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(cor.R, cor.G, cor.B);
            return ColorTranslator.ToHtml(color);
        }
        // RGB形式の文字列からColor型に変換
        static public System.Drawing.Color Color_Code(string rgb)
        {
            return ColorTranslator.FromHtml(rgb);
        }

        // 指定の文字、又はバックスペース以外の場合はfalseを返す
        static public bool Keychr_chk(string key, string data)
        {
            if (key == "\b")
            {
                return true;
            }
            else
            {
                return Regex.IsMatch(key, @data);
            }
        }
        // 数値の下限上限のチェック
        static public int KeyMax_chk(int key, int min, int max)
        {
            if (key < min)
            {
                return min;
            }
            if (key > max)
            {
                return max;
            }
            return key;
        }
        //半角英数字チェック関数
        static public bool isFunc(string str)
        {
            // nullの場合はfalseを返す
            if (str == null)
            {
                return false;
            }
            // 半角英数チェック
            return Regex.IsMatch(str, @"^[a-zA-Z0-9]+$");
        }

        // ファイル選択ダイアログ
        static public string Dlg_File_Select(string InitialDir, string Filter, string InitialExt)
        {
            // OpenFileDialogクラスのインスタンスを作成
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // はじめに表示されるフォルダを指定する
            dlg.InitialDirectory = InitialDir;

            // [ファイルの種類]に表示される選択肢を指定する
            // 例:"サウンドファイル(*.wav;*.mp3)|*.wav;*.mp3|すべてのファイル(*.*)|*.*"
            dlg.Filter = Filter;

            //はじめに表示される[ファイルの種類]を指定する 例:".wav"
            dlg.DefaultExt = InitialExt;

            //1番目の「指定された拡張子のファイル」のみが表示されているようにする
            dlg.FilterIndex = 0;

            //タイトルを設定する
            dlg.Title = "ファイルを選択してください";

            //複数ファイル選択を許可するか
            dlg.Multiselect = false;

            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            dlg.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            dlg.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            dlg.CheckPathExists = true;

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;

            }
            return "";
        }

        // JSONを使用したDeepCopy
        static public T JsonSerializerClone<T>(T src)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(src);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        // System.Windows.Media.Color(WPF) とSystem.Drawing.Color(WinForms)の変換
        static public System.Windows.Media.Color ToMediaColor(this System.Drawing.Color dColor) =>
        System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);

        static public System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color mColor) =>
            System.Drawing.Color.FromArgb(mColor.A, mColor.R, mColor.G, mColor.B);

        // BitmapSourceをBitmapに変換
        static public Bitmap ToBitmap(BitmapSource bitmapSource)
        {
            if (bitmapSource == null) return null;

            var bitmap = new Bitmap(
                bitmapSource.PixelWidth,
                bitmapSource.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );
            var bitmapData = bitmap.LockBits(
                new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );
            bitmapSource.CopyPixels(
                Int32Rect.Empty,
                bitmapData.Scan0,
                bitmapData.Height * bitmapData.Stride,
                bitmapData.Stride
            );
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        // BitmapをImageSourceに変換
        static public ImageSource ToImageSource(Bitmap bmp)
        {
            if (bmp == null) return null;

            var hBitmap = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }
        }
        class NativeMethods
        {
            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteObject([In] IntPtr hObject);
        }
    }
}
