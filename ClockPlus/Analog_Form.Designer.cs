namespace ClockPlus
{
    partial class Analog_Form
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
            components = new System.ComponentModel.Container();
            Pbox_Min = new PictureBox();
            Pbox_face = new PictureBox();
            Pbox_Hour = new PictureBox();
            Pbox_Sec = new PictureBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            設定ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            閉じるToolStripMenuItem = new ToolStripMenuItem();

            ((System.ComponentModel.ISupportInitialize)Pbox_Min).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_face).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_Hour).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_Sec).BeginInit();
            contextMenuStrip1.SuspendLayout();
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
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 設定ToolStripMenuItem, toolStripSeparator1, 閉じるToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(241, 107);

            // 
            // 設定ToolStripMenuItem
            // 
            設定ToolStripMenuItem.Name = "設定ToolStripMenuItem";
            設定ToolStripMenuItem.Size = new Size(240, 32);
            設定ToolStripMenuItem.Text = "設定";
            設定ToolStripMenuItem.Click += 設定ToolStripMenuItem_Click;
            // 

            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(237, 6);
            // 

            // 閉じるToolStripMenuItem
            // 
            閉じるToolStripMenuItem.Name = "閉じるToolStripMenuItem";
            閉じるToolStripMenuItem.Size = new Size(240, 32);
            閉じるToolStripMenuItem.Text = "閉じる";
            閉じるToolStripMenuItem.Click += 閉じるToolStripMenuItem_Click;
            // 

            // Analog_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(228, 234);
            ContextMenuStrip = contextMenuStrip1;
            ControlBox = false;
            Controls.Add(Pbox_Min);
            Controls.Add(Pbox_face);
            Controls.Add(Pbox_Hour);
            Controls.Add(Pbox_Sec);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Analog_Form";
            ShowInTaskbar = false;
            Text = "アナログ時計";
            FormClosed += Analog_Disp_FormClosed;
            Load += Analog_Disp_Load;
            ((System.ComponentModel.ISupportInitialize)Pbox_Min).EndInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_face).EndInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_Hour).EndInit();
            ((System.ComponentModel.ISupportInitialize)Pbox_Sec).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        public System.Windows.Forms.PictureBox Pbox_Min;
        public System.Windows.Forms.PictureBox Pbox_face;
        public System.Windows.Forms.PictureBox Pbox_Hour;
        public System.Windows.Forms.PictureBox Pbox_Sec;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 閉じるToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;

    }
}