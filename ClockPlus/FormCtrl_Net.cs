using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClockPlus
{
    public class FormCtrl_Net
    {
        static public int Display_Event_Timer = 0;

        // フォームを非表示にする
        static public void Hide_Form(Form form)
        {
            form.Visible = false;
        }
        // フォームを表示する
        static public void valid_Form(Form form)
        {
            if (form.WindowState != FormWindowState.Normal)
            {
                form.WindowState = FormWindowState.Normal;
            }
            form.Visible = true;
        }
    }
}
