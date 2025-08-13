namespace ClockPlus
{
    partial class Form_Weather2
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
            PictureBox_Form = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)PictureBox_Form).BeginInit();
            SuspendLayout();
            // 
            // PictureBox_Form
            // 
            PictureBox_Form.Dock = DockStyle.Fill;
            PictureBox_Form.Location = new Point(0, 0);
            PictureBox_Form.Name = "PictureBox_Form";
            PictureBox_Form.Size = new Size(278, 194);
            PictureBox_Form.SizeMode = PictureBoxSizeMode.Zoom;
            PictureBox_Form.TabIndex = 0;
            PictureBox_Form.TabStop = false;
            // 
            // Form_Weather2
            // 
            AutoScaleDimensions = new SizeF(10F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(278, 194);
            Controls.Add(PictureBox_Form);
            Font = new Font("メイリオ", 8F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form_Weather2";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Form_Weather2";
            FormClosed += Form_Closed;
            Load += Form_Load;
            ((System.ComponentModel.ISupportInitialize)PictureBox_Form).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox PictureBox_Form;
    }
}