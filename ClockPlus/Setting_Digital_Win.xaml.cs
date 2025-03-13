using ControlzEx.Standard;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace ClockPlus
{
    public partial class Setting_Digital_Win : MetroWindow
    {
        private bool FormLoad_Flag = false;
        static public int Setting_Dno = 0;
        public Digital _digital = new Digital();

        public Setting_Digital_Win()
        {
            InitializeComponent();
            Height_Slider.Maximum = Etc.Disp_Height;
            Width_Slider.Maximum = Etc.Disp_Width;

            // 時計表示フォーマットの種類
            Format_ComboBox.Items.Add("yyyy/MM/dd");
            Format_ComboBox.Items.Add("yyyy/MM/dd (ddd)");
            Format_ComboBox.Items.Add("yyyy/MM/dd dddd");
            Format_ComboBox.Items.Add("yy/M/d");
            Format_ComboBox.Items.Add("yy/M/d (ddd)");
            Format_ComboBox.Items.Add("ggy年M月d日");
            Format_ComboBox.Items.Add("ggy年M月d日 (ddd)");
            Format_ComboBox.Items.Add("ggy年M月d日 dddd");
            Format_ComboBox.Items.Add("HH:mm");
            Format_ComboBox.Items.Add("HH:mm:ss");
            Format_ComboBox.Items.Add("H時m分");
            Format_ComboBox.Items.Add("H時m分s秒");
            Format_ComboBox.Items.Add("tth時m分");
            Format_ComboBox.Items.Add("tth時m分s秒");

            // 色塗種類の種類
            Style_ComboBox.Items.Add("Off");
            Style_ComboBox.Items.Add("HatchBrush");
            Style_ComboBox.Items.Add("LinearGradientBrush");

            // HatchStyleの種類
            Get_Hblush_List();

            // LinearGradientBrushの種類
            LgBrush_ComboBox.Items.Add("左から右へ");
            LgBrush_ComboBox.Items.Add("上から下へ");
            LgBrush_ComboBox.Items.Add("左上から右下へ");
            LgBrush_ComboBox.Items.Add("右上から左下へ");

            OneColor_Grid.Visibility = Visibility.Visible;         // 表示する
            HbBrush_Grid.Visibility = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)
            LgBrush_Grid.Visibility = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)
        }

        // HatchStyleのリストを作成
        private void Get_Hblush_List()
        {
            foreach (string hs in Enum.GetNames(typeof(HatchStyle)))
            {
                HbBrush_ComboBox.Items.Add(hs);
            }
        }

        private void Setting_Digital_Window_Loaded(object sender, RoutedEventArgs e)
        {
            // キャンセルされた時のために、設定値をバックアップしておく
            _digital = Etc.JsonSerializerClone(XML_Main_IO.Main.Display.Digtal[Setting_Dno]);

            Get_Setting_Data();             // 設定データの取り込み
            FormLoad_Flag = true;
        }

        private void Setting_Font_Button_Click(object sender, RoutedEventArgs e)
        {
            //            System.Windows.Controls.Label Work_label = new System.Windows.Controls.Label();
            Font font = System.Drawing.SystemFonts.DefaultFont;
            font = Etc.Font_fromStr(XML_Main_IO.Main.Display.Digtal[Setting_Dno].Font);

            var dlg = new System.Windows.Forms.FontDialog();
            dlg.Font = font;
            dlg.MaxSize = 1638;
            dlg.FontMustExist = true;       // エラーメッセージを表示
            dlg.ShowColor = false;          // 色を選択出来ないようにする
            dlg.ShowEffects = true;         // 取り消し線、下線、テキストの色などのオプションを選択出来ないようにする

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].Font = Etc.Font_fromFnt(dlg.Font);
                System.Drawing.FontFamily FontFamily = new System.Drawing.FontFamily(dlg.Font.FontFamily.Name);
                txtFamilyName.Text = FontFamily.Name;
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void Position_Ini_Button_Click(object sender, RoutedEventArgs e)
        {
            XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosX = Etc.Disp_Width / 2;
            XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosY = Etc.Disp_Height / 2;
            XML_Main_IO.Main.Display.Digtal[Setting_Dno].TopMost = false;

            Height_Slider.Value = XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosY;
            Height_Label.Content = Height_Slider.Value.ToString();
            Width_Slider.Value = XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosX;
            Width_Label.Content = Width_Slider.Value.ToString();
            TopMost_CheckBox.IsChecked = XML_Main_IO.Main.Display.Digtal[Setting_Dno].TopMost;

            Location_Change();     // 位置変更処理
            TopMost_Change();      // フォーム優先度の変更
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            // バックアップしておいた設定を読み込む
            XML_Main_IO.Main.Display.Digtal[Setting_Dno] = _digital;
            Digital_Disp_ReDrow();          // デジタル表示の再描画
            Location_Change();     // 位置変更処理
            this.Close();
        }

        private void Outline_ColorPicker_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                System.Windows.Media.Color Mcolor = Outline_ColorPicker.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].Outline_Color = Etc.RGB_Code(Dcolor);
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void Opacity_Slider_ValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].Opacity = (double)Opacity_Slider.Value / 100;
                Opacity_Label.Content = Opacity_Slider.Value.ToString() + "%";
                Opacity_Change();           // 不透明度変更処理
            }
        }

        private void Size_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag == true)
            {
                Size_Label.Content = Size_Slider.Value.ToString("P0");
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].Zoom = Size_Slider.Value;
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void Edge_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag == true)
            {
                Edge_Label.Content = Edge_Slider.Value.ToString();
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].Outline_Thick = (int)Edge_Slider.Value;
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void Style_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].Paint_Style = Style_ComboBox.SelectedItem.ToString();
                Style_Changed();        // 色塗種類の項目が変更された場合に更新
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void Format_ComboBox_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                string Item = "";
                Item = Format_ComboBox.SelectedItem.ToString();
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].Format = Item;
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void OneColor_ColorPicker_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                System.Windows.Media.Color Mcolor = OneColor_ColorPicker.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].Font_Color = Etc.RGB_Code(Dcolor);
                Style_Changed();                // 色塗種類の項目が変更された場合に更新
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void HbBrush_ComboBox_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                string Item = "";
                Item = HbBrush_ComboBox.SelectedItem.ToString();

                /*
                switch (Item)
                {
                    case "左から右へ":
                        break;
                    case "上から下へ":
                        break;
                    case "左上から右下へ":
                        break;
                    case "右上から左下へ":
                        break;
                }
                */
                XML_Main_IO.Main.Display.Digtal[0].HB_Style = HbBrush_ComboBox.SelectedIndex;
                Style_Changed();            // 色塗種類の項目が変更された場合に更新
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void HbBrush1_ColorPicker_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                System.Windows.Media.Color Mcolor = HbBrush1_ColorPicker.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].HB_Color1 = Etc.RGB_Code(Dcolor);
                Style_Changed();            // 色塗種類の項目が変更された場合に更新
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void HbBrush2_ColorPicker_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                System.Windows.Media.Color Mcolor = HbBrush2_ColorPicker.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].HB_Color2 = Etc.RGB_Code(Dcolor);
                Style_Changed();            // 色塗種類の項目が変更された場合に更新
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }
        private void LgBrush_ComboBox_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Digtal[0].LGB_Mode = LgBrush_ComboBox.SelectedIndex;
                Style_Changed();            // 色塗種類の項目が変更された場合に更新
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void LgBrush1_ColorPicker_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                System.Windows.Media.Color Mcolor = LgBrush1_ColorPicker.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].LGB_Color1 = Etc.RGB_Code(Dcolor);
                Style_Changed();            // 色塗種類の項目が変更された場合に更新
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        private void LgBrush2_ColorPicker_SelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                System.Windows.Media.Color Mcolor = LgBrush2_ColorPicker.SelectedColor;
                System.Drawing.Color Dcolor = Etc.ToDrawingColor(Mcolor);
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].LGB_Color2 = Etc.RGB_Code(Dcolor);
                Style_Changed();            // 色塗種類の項目が変更された場合に更新
                Digital_Disp_ReDrow();          // デジタル表示の再描画
            }
        }

        // 設定データの取り込み
        private void Get_Setting_Data()
        {
            // フォント
            Font fnt = Etc.Font_fromStr(XML_Main_IO.Main.Display.Digtal[Setting_Dno].Font);
            System.Drawing.FontFamily FontFamily = new System.Drawing.FontFamily(fnt.FontFamily.Name);
            txtFamilyName.Text = FontFamily.Name;

            // フォーマット
            Format_ComboBox.Text = XML_Main_IO.Main.Display.Digtal[Setting_Dno].Format;
            Format_ComboBox.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(Format_ComboBox);

            // 淵
            System.Drawing.Color Dcolor = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].Outline_Color);
            System.Windows.Media.Color Mcolor = Etc.ToMediaColor(Dcolor);
            Outline_ColorPicker.SelectedColor = Mcolor;

            Edge_Label.Content = XML_Main_IO.Main.Display.Digtal[Setting_Dno].Outline_Thick.ToString();
            Edge_Slider.Value = (double)XML_Main_IO.Main.Display.Digtal[Setting_Dno].Outline_Thick;

            // 色塗の種類
            Style_ComboBox.Text = XML_Main_IO.Main.Display.Digtal[Setting_Dno].Paint_Style;
            Style_ComboBox.SelectedIndex = FormCtrl_Wpf.ComBox_Index_Get(Style_ComboBox);
            Style_Changed();            // 色塗種類の項目が変更された場合に更新

            // 位置
            Height_Slider.Value = XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosY;
            Height_Label.Content = Height_Slider.Value.ToString();
            Width_Slider.Value = XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosX;
            Width_Label.Content = Width_Slider.Value.ToString();
            TopMost_CheckBox.IsChecked = XML_Main_IO.Main.Display.Digtal[Setting_Dno].TopMost;

            // 不透明度
            Opacity_Slider.Value = XML_Main_IO.Main.Display.Digtal[Setting_Dno].Opacity * 100;
            Opacity_Label.Content = Opacity_Slider.Value.ToString() + "%";

            // 倍率
            Size_Slider.Value = XML_Main_IO.Main.Display.Digtal[Setting_Dno].Zoom;
            Size_Label.Content = Size_Slider.Value.ToString("P0");
        }

        // 色塗種類の項目が変更された場合に更新
        private void Style_Changed()
        {
            string Item = "";
            Item = Style_ComboBox.SelectedItem.ToString();
            if (Item != null)
            {
                OneColor_Grid.Visibility = Visibility.Collapsed;       // 非表示にする(コンポーネントの場所を詰める)
                HbBrush_Grid.Visibility = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)
                LgBrush_Grid.Visibility = Visibility.Collapsed;        // 非表示にする(コンポーネントの場所を詰める)

                switch (Item)
                {
                    case "HatchBrush":
                        System.Drawing.Color Dcolor;
                        System.Windows.Media.Color Mcolor;

                        Dcolor = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].HB_Color1);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        HbBrush1_ColorPicker.SelectedColor = Mcolor;
                        Dcolor = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].HB_Color2);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        HbBrush2_ColorPicker.SelectedColor = Mcolor;

                        //描画先とするImageオブジェクトを作成する
                        Bitmap bitmap = new Bitmap(120, 50);        // コントロールのサイズに合わせる
                        HbBrush_Image.Source = Brush_GrdDraw(Item, bitmap);

                        OneColor_Grid.Visibility = Visibility.Collapsed;
                        HbBrush_Grid.Visibility = Visibility.Visible;         // 表示する
                        LgBrush_Grid.Visibility = Visibility.Collapsed;
                        break;
                    case "LinearGradientBrush":
                        Dcolor = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].LGB_Color1);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        LgBrush1_ColorPicker.SelectedColor = Mcolor;
                        Dcolor = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].LGB_Color2);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        LgBrush2_ColorPicker.SelectedColor = Mcolor;

                        //描画先とするImageオブジェクトを作成する
                        bitmap = new Bitmap(120, 50);               // コントロールのサイズに合わせる
                        LgBrush_Image.Source = Brush_GrdDraw(Item, bitmap);

                        OneColor_Grid.Visibility = Visibility.Collapsed;
                        HbBrush_Grid.Visibility = Visibility.Collapsed;
                        LgBrush_Grid.Visibility = Visibility.Visible;         // 表示する
                        break;
                    default:
                        Dcolor = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].Font_Color);
                        Mcolor = Etc.ToMediaColor(Dcolor);
                        OneColor_ColorPicker.SelectedColor = Mcolor;

                        OneColor_Grid.Visibility = Visibility.Visible;         // 表示する
                        HbBrush_Grid.Visibility = Visibility.Collapsed;
                        LgBrush_Grid.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private ImageSource Brush_GrdDraw(string brush, Bitmap bitmap)
        {
            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics graphics = Graphics.FromImage(bitmap);

            HatchStyle style = 0;
            LinearGradientMode mode = 0;

            System.Drawing.Color HB_Color1 = System.Drawing.Color.White;
            System.Drawing.Color HB_Color2 = System.Drawing.Color.Black;
            System.Drawing.Color LGB_Color1 = System.Drawing.Color.White;
            System.Drawing.Color LGB_Color2 = System.Drawing.Color.Black;

            style = (HatchStyle)XML_Main_IO.Main.Display.Digtal[0].HB_Style;
            HB_Color1 = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].HB_Color1);
            HB_Color2 = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].HB_Color2);
            mode = (LinearGradientMode)XML_Main_IO.Main.Display.Digtal[Setting_Dno].LGB_Mode;
            LGB_Color1 = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].LGB_Color1);
            LGB_Color2 = Etc.Color_Code(XML_Main_IO.Main.Display.Digtal[Setting_Dno].LGB_Color2);

            switch (brush)
            {
                case "HatchBrush":
                    HatchBrush hb = new HatchBrush(style, HB_Color1, HB_Color2);
                    //四角を描く
                    graphics.FillRectangle(hb, graphics.VisibleClipBounds);
                    //リソースを解放する
                    hb.Dispose();
                    graphics.Dispose();
                    break;
                case "LinearGradientBrush":
                    System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(
                    graphics.VisibleClipBounds, LGB_Color1, LGB_Color2, mode);
                    //四角を描く
                    graphics.FillRectangle(gb, graphics.VisibleClipBounds);
                    //リソースを解放する
                    gb.Dispose();
                    graphics.Dispose();
                    break;
            }
            return Etc.ToImageSource(bitmap);
        }

        private void Height_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosY = (int)Height_Slider.Value;
                Height_Label.Content = Height_Slider.Value.ToString();
                Location_Change();     // 位置変更処理
            }
        }

        private void Width_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosX = (int)Width_Slider.Value;
                Width_Label.Content = Width_Slider.Value.ToString();
                Location_Change();     // 位置変更処理
            }
        }

        private void TopMost_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].TopMost = true;
                TopMost_Change();           // フォーム優先度の変更
            }
        }

        private void TopMost_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (FormLoad_Flag == true)
            {
                XML_Main_IO.Main.Display.Digtal[Setting_Dno].TopMost = false;
                TopMost_Change();           // フォーム優先度の変更
            }
        }

        // デジタル表示の再描画
        private void Digital_Disp_ReDrow()
        {
            switch (Setting_Dno)
            {
                case 0:
                    App._Digital1_Form.Disp1_Drow();       // デジタル時計１を描画
                    break;
                case 1:
                    App._Digital2_Form.Disp2_Drow();       // デジタル時計２を描画
                    break;
            }
        }

        // 位置変更処理
        private void Location_Change()
        {
            switch (Setting_Dno)
            {
                case 0:
                    App._Digital1_Form.Location = new System.Drawing.Point(XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosX, XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosY);
                    break;
                case 1:
                    App._Digital2_Form.Location = new System.Drawing.Point(XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosX, XML_Main_IO.Main.Display.Digtal[Setting_Dno].PosY);
                    break;
            }
        }

        // フォーム優先度の変更
        private void TopMost_Change()
        {
            switch (Setting_Dno)
            {
                case 0:
                    App._Digital1_Form.TopMost = XML_Main_IO.Main.Display.Digtal[Setting_Dno].TopMost;
                    break;
                case 1:
                    App._Digital2_Form.TopMost = XML_Main_IO.Main.Display.Digtal[Setting_Dno].TopMost;
                    break;
            }
        }
        
        // 不透明度変更処理
        private void Opacity_Change()
        {
            switch (Setting_Dno)
            {
                case 0:
                    App._Digital1_Form.Opacity = XML_Main_IO.Main.Display.Digtal[Setting_Dno].Opacity;
                    break;
                case 1:
                    App._Digital2_Form.Opacity = XML_Main_IO.Main.Display.Digtal[Setting_Dno].Opacity;
                    break;
            }
        }
    }
}
