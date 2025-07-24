
namespace ClockPlus
{
    partial class Form_Digital1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            PicBox_Disp = new PictureBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)PicBox_Disp).BeginInit();
            SuspendLayout();
            // 
            // PicBox_Disp
            // 
            PicBox_Disp.Dock = DockStyle.Fill;
            PicBox_Disp.Location = new Point(0, 0);
            PicBox_Disp.Margin = new Padding(3, 4, 3, 4);
            PicBox_Disp.Name = "PicBox_Disp";
            PicBox_Disp.Size = new Size(240, 138);
            PicBox_Disp.SizeMode = PictureBoxSizeMode.AutoSize;
            PicBox_Disp.TabIndex = 6;
            PicBox_Disp.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 12;
            label1.Text = "label1";
            label1.Visible = false;
            // 
            // Form_Digital1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(240, 138);
            ControlBox = false;
            Controls.Add(label1);
            Controls.Add(PicBox_Disp);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form_Digital1";
            ShowInTaskbar = false;
            Text = "デジタル表示１";
            FormClosed += Digital_Disp1_FormClosed;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)PicBox_Disp).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public System.Windows.Forms.PictureBox PicBox_Disp;
        private System.Windows.Forms.Label label1;
    }
}

