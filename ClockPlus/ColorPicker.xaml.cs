﻿using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using UserControl = System.Windows.Controls.UserControl;

namespace ClockPlus
{
    public class ColorPickerHeaderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Template1 { get; set; }
        public DataTemplate Template2 { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            CollectionViewGroup group = (CollectionViewGroup)item;

            if ("カスタム" != group.Name.ToString())
                return Template1;
            else
                return Template2;
        }
    }

    public class ColorPickerItem : INotifyPropertyChanged
    {
        public string CategoryName { get; set; }

        public int Column { get; set; }

        public int Row { get; set; }

        private Color _ItemColor;
        public Color ItemColor
        {
            get { return _ItemColor; }
            set
            {
                _ItemColor = value;
                _ItemBrush = new SolidColorBrush(_ItemColor);
                OnPropertyChanged("ItemColor");
                OnPropertyChanged("ItemBrush");
            }
        }

        private SolidColorBrush _ItemBrush;
        public Brush ItemBrush
        {
            get { return _ItemBrush; }
        }

        public string ToolTip
        {
            get { return string.Format("#{0:X2}{1:X2}{2:X2}", _ItemColor.R, _ItemColor.G, _ItemColor.B); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (null == this.PropertyChanged) return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    };

    public partial class ColorPicker : UserControl
    {
        private bool IsPropertyChanging;
        private byte FixedAlpha;
        private ColorPickerItem CustomItem;


        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                "SelectedColor", // プロパティ名を指定
                typeof(Color), // プロパティの型を指定
                typeof(ColorPicker), // プロパティを所有する型を指定
                new FrameworkPropertyMetadata(Colors.White,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) => { (d as ColorPicker).OnColorPropertyChanged(e); }));
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public event SelectionChangedEventHandler SelectionChanged;

        public ColorPicker()
        {
            InitializeComponent();

            float r, g, b;
            var items = new List<ColorPickerItem>();

            for (int i = 0; i < 12; ++i)
            {
                Cnv.HSVtoRGB(0.0f, 0.0f, (float)i / 11.0f, out r, out g, out b);
                byte ba = 255;
                byte br = (byte)(r * 255.0f);
                byte bg = (byte)(g * 255.0f);
                byte bb = (byte)(b * 255.0f);
                items.Add(new ColorPickerItem()
                {
                    CategoryName = "グレースケール",
                    Column = i,
                    Row = 0,
                    ItemColor = Color.FromArgb(ba, br, bg, bb),
                });
            }
            for (int i = 0; i < 12; ++i)
            {
                Cnv.HSVtoRGB((float)i / 12.0f, 1.0f, 1.0f, out r, out g, out b);
                byte ba = 255;
                byte br = (byte)(r * 255.0f);
                byte bg = (byte)(g * 255.0f);
                byte bb = (byte)(b * 255.0f);
                items.Add(new ColorPickerItem()
                {
                    CategoryName = "基本の色",
                    Column = i,
                    Row = 1,
                    ItemColor = Color.FromArgb(ba, br, bg, bb),
                });
            }

            for (int j = 1; j < 5; ++j)
            {
                float s = (float)j / 5.0f;
                for (int i = 0; i < 12; ++i)
                {
                    Cnv.HSVtoRGB((float)i / 12.0f, s, 1.0f, out r, out g, out b);
                    byte ba = 255;
                    byte br = (byte)(r * 255.0f);
                    byte bg = (byte)(g * 255.0f);
                    byte bb = (byte)(b * 255.0f);
                    items.Add(new ColorPickerItem()
                    {
                        CategoryName = "その他の色",
                        Column = i,
                        Row = 2 + j - 1,
                        ItemColor = Color.FromArgb(ba, br, bg, bb),
                    });
                }
            }
            for (int j = 1; j < 4; ++j)
            {
                float v = 1.0f - (float)j / 5.0f;
                for (int i = 0; i < 12; ++i)
                {
                    Cnv.HSVtoRGB((float)i / 12.0f, 1.0f, v, out r, out g, out b);
                    byte ba = 255;
                    byte br = (byte)(r * 255.0f);
                    byte bg = (byte)(g * 255.0f);
                    byte bb = (byte)(b * 255.0f);
                    items.Add(new ColorPickerItem()
                    {
                        CategoryName = "その他の色",
                        Column = i,
                        Row = 6 + j - 1,
                        ItemColor = Color.FromArgb(ba, br, bg, bb),
                    });
                }
            }
            CustomItem = new ColorPickerItem()
            {
                CategoryName = "カスタム",
                Column = 0,
                Row = 9,
                ItemColor = Colors.Black,
            };
            items.Add(CustomItem);

            IsPropertyChanging = true;
            var src = new ListCollectionView(items);
            src.GroupDescriptions.Add(new PropertyGroupDescription("CategoryName"));
            Main.ItemsSource = src;

            FixedAlpha = SelectedColor.A;
            CustomItem.ItemColor = Color.FromArgb(255, SelectedColor.R, SelectedColor.G, SelectedColor.B);
            Main.SelectedItem = CustomItem;
            IsPropertyChanging = false;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            dialog.FullOpen = true;
            dialog.Color = System.Drawing.Color.FromArgb(SelectedColor.A, SelectedColor.R, SelectedColor.G, SelectedColor.B);
            if (System.Windows.Forms.DialogResult.OK == dialog.ShowDialog())
            {
                SelectedColor = Color.FromArgb(FixedAlpha, dialog.Color.R, dialog.Color.G, dialog.Color.B);
            }
        }

        private void Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = Main.SelectedItem as ColorPickerItem;
            if (null == item) return;

            if (!IsPropertyChanging)
            {
                SelectedColor = Color.FromArgb(FixedAlpha, item.ItemColor.R, item.ItemColor.G, item.ItemColor.B);

                if (null != SelectionChanged)
                    SelectionChanged(this, e);
            }
        }

        public void OnColorPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var item = Main.SelectedItem as ColorPickerItem;

            Color col = Colors.Black;
            col = (Color)e.NewValue;

            if (null != item)
                if (item.ItemColor == col) return;
//            IsPropertyChanging = true;
            FixedAlpha = col.A;
            CustomItem.ItemColor = col;

            // 以前もカスタム選択された後だと、SelectionChangedイベントが発生しないので、
            // 一時的にnullをセットして強制的に変化させる
            Main.SelectedItem = null;

            Main.SelectedItem = CustomItem;
//            IsPropertyChanging = false;
        }
    }

    public partial class Cnv
    {
        public static void RGBtoHSV(float r, float g, float b, out float h, out float s, out float v)
        {
            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);

            h = max - min;
            if (0.0f < h)
            {
                if (max == r)
                {
                    h = (g - b) / h;
                    if (h < 0.0f)
                    {
                        h += 6.0f;
                    }
                }
                else if (max == g)
                {
                    h = 2.0f + (b - r) / h;
                }
                else
                {
                    h = 4.0f + (r - g) / h;
                }
            }
            h /= 6.0f;
            s = (max - min);
            if (0.0 < max) s /= max;
            v = max;
        }

        public static void HSVtoRGB(float h, float s, float v, out float r, out float g, out float b)
        {
            r = v;
            g = v;
            b = v;
            if (0.0f < s)
            {
                h *= 6.0f;
                int i = (int)h;
                float f = h - (float)i;
                switch (i)
                {
                    default:
                    case 0:
                        g *= 1 - s * (1 - f);
                        b *= 1 - s;
                        break;
                    case 1:
                        r *= 1 - s * f;
                        b *= 1 - s;
                        break;
                    case 2:
                        r *= 1 - s;
                        b *= 1 - s * (1 - f);
                        break;
                    case 3:
                        r *= 1 - s;
                        g *= 1 - s * f;
                        break;
                    case 4:
                        r *= 1 - s * (1 - f);
                        g *= 1 - s;
                        break;
                    case 5:
                        g *= 1 - s;
                        b *= 1 - s * f;
                        break;
                }
            }
        }
    }

}
