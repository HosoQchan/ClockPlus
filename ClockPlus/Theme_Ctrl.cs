using ControlzEx.Theming;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using MaterialDesignThemes.Wpf;
using MaterialDesignColors;
using MaterialDesignExtensions.Themes;

namespace ClockPlus
{
    static public class Theme_Ctrl
    {
        static MaterialDesignThemes.Wpf.PaletteHelper paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();
        static MaterialDesignThemes.Wpf.Theme theme = new();

        static public Dictionary<string,string> MahApps_color = new Dictionary<string, string>()
        {
                {"Amber",   "#FFF0A30A"},
                {"Blue",    "#FF0078D7"},
                {"Brown",   "#FF825A2C"},
                {"Cobalt",  "#FF0050EF"},
                {"Crimson", "#FFA20025"},
                {"Cyan",    "#FF1BA1E2"},
                {"Emerald", "#FF008A00"},
                {"Green",   "#FF60A917"},
                {"Indigo",  "#FF6A00FF"},
                {"Lime",    "#FFA4C400"},
                {"Magenta", "#FFD80073"},
                {"Mauve",   "#FF76608A"},
                {"Olive",   "#FF6D8764"},
                {"Orange",  "#FFFA6800"},
                {"Pink",    "#FFF472D0"},
                {"Purple",  "#FF6459DF"},
                {"Red",     "#FFE51400"},
                {"Sienna",  "#FFA0522D"},
                {"Steel",   "#FF647687"},
                {"Taupe",   "#FF87794E"},
                {"Teal",    "#FF00ABA9"},
                {"Violet",  "#FFAA00FF"},
                {"Yellow",  "#FFFEDE06"}
        };
        // 色と色名を保持するクラス
        public class MyColor
        {
            public System.Windows.Media.Color Color { get; set; }
            public string Name { get; set; }
        }

        // Windowsテーマ色
        static public int GetSystemUsesLightTheme()
        {
            int nResult = -1;
            string sKeyName = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            string sSubkeyName = "SystemUsesLightTheme";
            Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sKeyName);
            nResult = (int)rKey.GetValue(sSubkeyName);
            rKey.Close();
            return nResult;
        }

        /// <summary>
        /// テーマがライトモードかダークモードかを確認する
        /// </summary>
        /// <returns>
        /// ライトモード: false
        /// ダークモード: true
        /// </returns>
        static public bool Theme_Check()
        {
            if (XML_Main.cnf.Theme.Mode == "Light")
            {
                return false;
            }
            if (XML_Main.cnf.Theme.Mode == "Dark")
            {
                return true;
            }

            // Windowsテーマ色がダークモードの時
            if (Theme_Ctrl.GetSystemUsesLightTheme() == 0)
            {
                return true;
            }
            // Windowsテーマ色がライトモードの時
            else
            {
                return false;
            }
        }

        static public void Change_Theme(string Mode, String Theme_Color)
        {
            /*
            ●モード
            (Auto /Light /Dark)

            ●カラー
            (Red / Green / Blue / Purple / Orange / Lime / Emerald / Teal / Cyan /
            Cobalt / Indigo / Violet / Pink / Magenta / Crimson / Amber / Yellow /
            Brown / Olive / Steel / Mauve / Taupe / Sienna)
            */

            if (Mode == "Auto")
            {
                // Windowsテーマ色がダークモードの時
                if (Theme_Ctrl.GetSystemUsesLightTheme() == 0)
                {
                    Mode = "Dark";
                }
                // Windowsテーマ色がライトモードの時
                else
                {
                    Mode = "Light";
                }
            }

            ///////////  Material Design In XAML ///////
            if (Mode == "Dark")
            {
                // ダークモードへ切り替え
                theme.SetBaseTheme(BaseTheme.Dark);
            }
            else
            {
                // ライトモードへ切り替え
                theme.SetBaseTheme(BaseTheme.Dark);
            }
            // 色設定
            theme.PrimaryMid = new ColorPair(ToColorOrDefault(MahApps_color[Theme_Color]));
            theme.SecondaryMid = new ColorPair(ToColorOrDefault(MahApps_color[Theme_Color]));

            // テーマを適用する
            paletteHelper.SetTheme(theme);


            //////////// MahApps.Metro ///////////
            string Theme = Mode + "." + Theme_Color;
            ThemeManager.Current.ChangeTheme(System.Windows.Application.Current, Theme);
        }

        // System.Drawing.Color(WinForms) ⇒ System.Windows.Media.Colorへの変換
        static public System.Windows.Media.Color ToMediaColor(this System.Drawing.Color dColor) =>
        System.Windows.Media.Color.FromArgb(dColor.A, dColor.R, dColor.G, dColor.B);

        // System.Windows.Media.Color ⇒ System.Drawing.Color(WinForms)への変換
        static public System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color mColor) =>
            System.Drawing.Color.FromArgb(mColor.A, mColor.R, mColor.G, mColor.B);

        // 文字列からColorへ変換するメソッド
        // "#ADFF2F"や"Cyan"といった文字列からColorに変換します。
        // 変換失敗時に、例外ではなくnullかColorの初期値(透明色"#00000000")を返す2つのメソッドです。
        static public System.Windows.Media.Color ToColorOrDefault(this string code) => ToColorOrNull(code) ?? default;
        static public System.Windows.Media.Color? ToColorOrNull(this string code)
        {
            try
            {
                return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(code);
            }
            catch (FormatException _)
            {
                return null;
            }
        }



    }
}
