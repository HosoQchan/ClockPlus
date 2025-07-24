using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = System.Windows.Application;

namespace ClockPlus
{
    // WindowManager
    //
    // WPF の Window が開いていれば前面に表示するクラス
    // 以下のような感じで使う
    // 
    // 単純にnewでインスタンス化できるWindowの場合
    // WindowManager.ShowOrActivate<Window1>();
    //
    // ラムダ式で任意のインスタンス生成方法を指定する場合
    // WindowManager.ShowOrActivate<Window1>(() => new Window1());
    //
    public static class WindowManager
    {
        public static void ShowOrActivate<TWindow>()
            where TWindow : Window, new()
        {
            // 対象Windowが開かれているか探す
            var window = Application.Current.Windows.OfType<TWindow>().FirstOrDefault();
            if (window == null)
            {
                // 開かれてなかったら開く
                window = new TWindow();
                window.Show();
            }
            else
            {
                // 既に開かれていたらアクティブにする
                window.Activate();
            }
        }

        // newでインスタンスが作れない時用
        public static void ShowOrActivate<TWindow>(Func<TWindow> factory)
            where TWindow : Window
        {
            // 対象Windowが開かれているか探す
            var window = Application.Current.Windows.OfType<TWindow>().FirstOrDefault();
            if (window == null)
            {
                // 開かれてなかったら開く
                window = factory();
                window.Show();
            }
            else
            {
                // 既に開かれていたらアクティブにする
                window.Activate();
            }
        }
    }
}
