using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace ClockPlus
{

    public class FormCtrl_Wpf
    {
        // ComBoxのTextデータに一致する項目(Index値)を取得する
        static public int ComBox_Index_Get(System.Windows.Controls.ComboBox Cbox)
        {
            int index = 0;
            for (int i = 0; i < Cbox.Items.Count; ++i)
            {
                if (Cbox.Items[i].Equals(Cbox.Text))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        // フォームを非表示にする
        static public void Hide_Form(Form form)
        {
            if (form.WindowState != FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Minimized;
                form.Visible = false;
                form.TransparencyKey = form.BackColor;
                form.FormBorderStyle = FormBorderStyle.None;
            }
        }
        // フォームを表示する
        static public void valid_Form(Form form)
        {
            if (form.WindowState != FormWindowState.Normal)
            {
                form.WindowState = FormWindowState.Normal;
                form.Visible = true;
                form.TransparencyKey = form.BackColor;
                form.FormBorderStyle = FormBorderStyle.None;
            }
        }
        // ファイルが存在するか
        static public bool File_Check(System.Windows.Window form, string FileName)
        {
            if ((FileName == "") || (System.IO.File.Exists(FileName) == false))
            {
                Error_Message("ファイルが見つかりません。");
                return false;
            }
            return true;
        }

        // メッセージを表示する
        static public void Info_Message(string Text)
        {
            var Result = new MessageBox_Ex(Text, MessageBox_Ex.MessageType.Info, MessageBox_Ex.MessageButtons.Ok).ShowDialog();
        }

        // エラーメッセージを表示する
        static public void Error_Message(string Text)
        {
            var Result = new MessageBox_Ex(Text, MessageBox_Ex.MessageType.Error, MessageBox_Ex.MessageButtons.Ok).ShowDialog();
        }

        // 確認メッセージを表示する
        static public bool YesNo_Message(string Text)
        {
            bool Result = (bool)new MessageBox_Ex(Text, MessageBox_Ex.MessageType.Confirmation, MessageBox_Ex.MessageButtons.YesNo).ShowDialog();
            return Result;
        }
    }
}
