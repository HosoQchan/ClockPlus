namespace ClockPlus
{
    partial class Form_Weather_Backup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Label_TmpMin = new Label();
            Label_TmpMax = new Label();
            Label_Date = new Label();
            Label_Title = new Label();
            Label_Ttmpmin = new Label();
            PictureBox1 = new PictureBox();
            Label_Ttmpmax = new Label();
            Label_Rain1 = new Label();
            Label_Telop = new Label();
            Label_Rain2 = new Label();
            Label_Rain = new Label();
            Label_Rain3 = new Label();
            Label_T1824 = new Label();
            Label_Rain4 = new Label();
            Label_T1218 = new Label();
            Label_T0006 = new Label();
            Label_T0612 = new Label();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
            SuspendLayout();
            // 
            // Label_TmpMin
            // 
            Label_TmpMin.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_TmpMin.ForeColor = Color.Blue;
            Label_TmpMin.Location = new Point(178, 82);
            Label_TmpMin.Name = "Label_TmpMin";
            Label_TmpMin.Size = new Size(50, 23);
            Label_TmpMin.TabIndex = 35;
            Label_TmpMin.Text = "15";
            Label_TmpMin.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Label_TmpMax
            // 
            Label_TmpMax.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_TmpMax.ForeColor = Color.Red;
            Label_TmpMax.Location = new Point(178, 59);
            Label_TmpMax.Name = "Label_TmpMax";
            Label_TmpMax.Size = new Size(50, 23);
            Label_TmpMax.TabIndex = 34;
            Label_TmpMax.Text = "15℃";
            Label_TmpMax.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Label_Date
            // 
            Label_Date.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Date.Location = new Point(212, 9);
            Label_Date.Name = "Label_Date";
            Label_Date.Size = new Size(50, 23);
            Label_Date.TabIndex = 33;
            Label_Date.Text = "今日";
            Label_Date.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label_Title
            // 
            Label_Title.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Title.Location = new Point(12, 9);
            Label_Title.Name = "Label_Title";
            Label_Title.Size = new Size(194, 23);
            Label_Title.TabIndex = 19;
            Label_Title.Text = "群馬県 南部";
            Label_Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Label_Ttmpmin
            // 
            Label_Ttmpmin.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Ttmpmin.ForeColor = Color.Blue;
            Label_Ttmpmin.ImageAlign = ContentAlignment.MiddleRight;
            Label_Ttmpmin.Location = new Point(98, 82);
            Label_Ttmpmin.Name = "Label_Ttmpmin";
            Label_Ttmpmin.Size = new Size(74, 23);
            Label_Ttmpmin.TabIndex = 32;
            Label_Ttmpmin.Text = "最低気温";
            Label_Ttmpmin.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // PictureBox1
            // 
            PictureBox1.Location = new Point(12, 59);
            PictureBox1.Margin = new Padding(3, 4, 3, 4);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new Size(80, 60);
            PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureBox1.TabIndex = 20;
            PictureBox1.TabStop = false;
            // 
            // Label_Ttmpmax
            // 
            Label_Ttmpmax.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Ttmpmax.ForeColor = Color.Red;
            Label_Ttmpmax.ImageAlign = ContentAlignment.MiddleRight;
            Label_Ttmpmax.Location = new Point(98, 59);
            Label_Ttmpmax.Name = "Label_Ttmpmax";
            Label_Ttmpmax.Size = new Size(74, 23);
            Label_Ttmpmax.TabIndex = 31;
            Label_Ttmpmax.Text = "最高気温";
            Label_Ttmpmax.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Label_Rain1
            // 
            Label_Rain1.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Rain1.Location = new Point(12, 146);
            Label_Rain1.Name = "Label_Rain1";
            Label_Rain1.Size = new Size(58, 23);
            Label_Rain1.TabIndex = 21;
            Label_Rain1.Text = "00-06";
            Label_Rain1.TextAlign = ContentAlignment.BottomCenter;
            // 
            // Label_Telop
            // 
            Label_Telop.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Telop.Location = new Point(12, 32);
            Label_Telop.Name = "Label_Telop";
            Label_Telop.Size = new Size(250, 23);
            Label_Telop.TabIndex = 30;
            Label_Telop.Text = "くもり海上海岸は霧か霧雨";
            Label_Telop.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Label_Rain2
            // 
            Label_Rain2.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Rain2.Location = new Point(76, 146);
            Label_Rain2.Name = "Label_Rain2";
            Label_Rain2.Size = new Size(58, 23);
            Label_Rain2.TabIndex = 22;
            Label_Rain2.Text = "06-12";
            Label_Rain2.TextAlign = ContentAlignment.BottomCenter;
            // 
            // Label_Rain
            // 
            Label_Rain.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Rain.Location = new Point(12, 123);
            Label_Rain.Name = "Label_Rain";
            Label_Rain.Size = new Size(250, 23);
            Label_Rain.TabIndex = 29;
            Label_Rain.Text = "降水確率";
            Label_Rain.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label_Rain3
            // 
            Label_Rain3.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Rain3.Location = new Point(140, 146);
            Label_Rain3.Name = "Label_Rain3";
            Label_Rain3.Size = new Size(58, 23);
            Label_Rain3.TabIndex = 23;
            Label_Rain3.Text = "12-18";
            Label_Rain3.TextAlign = ContentAlignment.BottomCenter;
            // 
            // Label_T1824
            // 
            Label_T1824.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_T1824.Location = new Point(204, 169);
            Label_T1824.Name = "Label_T1824";
            Label_T1824.Size = new Size(58, 23);
            Label_T1824.TabIndex = 28;
            Label_T1824.Text = "20%";
            Label_T1824.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label_Rain4
            // 
            Label_Rain4.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_Rain4.Location = new Point(204, 146);
            Label_Rain4.Name = "Label_Rain4";
            Label_Rain4.Size = new Size(58, 23);
            Label_Rain4.TabIndex = 24;
            Label_Rain4.Text = "18-24";
            Label_Rain4.TextAlign = ContentAlignment.BottomCenter;
            // 
            // Label_T1218
            // 
            Label_T1218.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_T1218.Location = new Point(140, 169);
            Label_T1218.Name = "Label_T1218";
            Label_T1218.Size = new Size(58, 23);
            Label_T1218.TabIndex = 27;
            Label_T1218.Text = "20%";
            Label_T1218.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label_T0006
            // 
            Label_T0006.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_T0006.Location = new Point(12, 169);
            Label_T0006.Name = "Label_T0006";
            Label_T0006.Size = new Size(58, 23);
            Label_T0006.TabIndex = 25;
            Label_T0006.Text = "2%";
            Label_T0006.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label_T0612
            // 
            Label_T0612.Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label_T0612.Location = new Point(76, 169);
            Label_T0612.Name = "Label_T0612";
            Label_T0612.Size = new Size(58, 23);
            Label_T0612.TabIndex = 26;
            Label_T0612.Text = "20%";
            Label_T0612.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form_Weather
            // 
            AutoScaleDimensions = new SizeF(10F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(278, 194);
            Controls.Add(Label_TmpMin);
            Controls.Add(Label_TmpMax);
            Controls.Add(Label_Date);
            Controls.Add(Label_Title);
            Controls.Add(Label_Ttmpmin);
            Controls.Add(PictureBox1);
            Controls.Add(Label_Ttmpmax);
            Controls.Add(Label_Rain1);
            Controls.Add(Label_Telop);
            Controls.Add(Label_Rain2);
            Controls.Add(Label_Rain);
            Controls.Add(Label_Rain3);
            Controls.Add(Label_T1824);
            Controls.Add(Label_Rain4);
            Controls.Add(Label_T1218);
            Controls.Add(Label_T0006);
            Controls.Add(Label_T0612);
            Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form_Weather";
            ShowInTaskbar = false;
            Text = "Form_Weather_Base";
            FormClosed += Form_Closed;
            Load += Form_Load;
            ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label Label_TmpMin;
        private Label Label_TmpMax;
        private Label Label_Date;
        private Label Label_Title;
        private Label Label_Ttmpmin;
        private PictureBox PictureBox1;
        private Label Label_Ttmpmax;
        private Label Label_Rain1;
        private Label Label_Telop;
        private Label Label_Rain2;
        private Label Label_Rain;
        private Label Label_Rain3;
        private Label Label_T1824;
        private Label Label_Rain4;
        private Label Label_T1218;
        private Label Label_T0006;
        private Label Label_T0612;
    }
}