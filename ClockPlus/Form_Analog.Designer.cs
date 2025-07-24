namespace ClockPlus
{
    partial class Form_Analog
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
            Pbox_Min = new PictureBox();
            Pbox_face = new PictureBox();
            Pbox_Hour = new PictureBox();
            Pbox_Sec = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)Pbox_Min).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_face).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_Hour).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_Sec).BeginInit();
            SuspendLayout();
            // 
            // Pbox_Min
            // 
            Pbox_Min.Location = new Point(12, 121);
            Pbox_Min.Margin = new Padding(3, 4, 3, 4);
            Pbox_Min.Name = "Pbox_Min";
            Pbox_Min.Size = new Size(100, 100);
            Pbox_Min.TabIndex = 15;
            Pbox_Min.TabStop = false;
            // 
            // Pbox_face
            // 
            Pbox_face.Location = new Point(12, 13);
            Pbox_face.Margin = new Padding(3, 4, 3, 4);
            Pbox_face.Name = "Pbox_face";
            Pbox_face.Size = new Size(100, 100);
            Pbox_face.TabIndex = 17;
            Pbox_face.TabStop = false;
            // 
            // Pbox_Hour
            // 
            Pbox_Hour.Location = new Point(118, 13);
            Pbox_Hour.Margin = new Padding(3, 4, 3, 4);
            Pbox_Hour.Name = "Pbox_Hour";
            Pbox_Hour.Size = new Size(100, 100);
            Pbox_Hour.TabIndex = 18;
            Pbox_Hour.TabStop = false;
            // 
            // Pbox_Sec
            // 
            Pbox_Sec.Location = new Point(118, 121);
            Pbox_Sec.Margin = new Padding(3, 4, 3, 4);
            Pbox_Sec.Name = "Pbox_Sec";
            Pbox_Sec.Size = new Size(100, 100);
            Pbox_Sec.TabIndex = 16;
            Pbox_Sec.TabStop = false;
            // 
            // Form_Analog
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(228, 234);
            ControlBox = false;
            Controls.Add(Pbox_Min);
            Controls.Add(Pbox_face);
            Controls.Add(Pbox_Hour);
            Controls.Add(Pbox_Sec);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form_Analog";
            ShowInTaskbar = false;
            Text = "アナログ時計";
            FormClosed += Analog_Disp_FormClosed;
            Load += Analog_Disp_Load;
            ((System.ComponentModel.ISupportInitialize)Pbox_Min).EndInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_face).EndInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_Hour).EndInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_Sec).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public System.Windows.Forms.PictureBox Pbox_Min;
        public System.Windows.Forms.PictureBox Pbox_face;
        public System.Windows.Forms.PictureBox Pbox_Hour;
        public System.Windows.Forms.PictureBox Pbox_Sec;
    }
}