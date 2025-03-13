
namespace ClockPlus
{
    partial class Digital2_Form
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
            components = new System.ComponentModel.Container();
            PicBox_Disp = new PictureBox();
            label1 = new Label();
            contextMenuStrip1 = new ContextMenuStrip(components);
            設定ToolStripMenuItem = new ToolStripMenuItem();

            toolStripSeparator1 = new ToolStripSeparator();
            閉じるToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)PicBox_Disp).BeginInit();
            contextMenuStrip1.SuspendLayout();
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
            // Digital2_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(240, 138);
            ContextMenuStrip = contextMenuStrip1;
            ControlBox = false;
            Controls.Add(label1);
            Controls.Add(PicBox_Disp);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Digital2_Form";
            ShowInTaskbar = false;
            Text = "デジタル表示２";
            FormClosed += Digital_Disp2_FormClosed;
            Load += Form2_Load;
            ((System.ComponentModel.ISupportInitialize)PicBox_Disp).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public System.Windows.Forms.PictureBox PicBox_Disp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 閉じるToolStripMenuItem;

        private ToolStripSeparator toolStripSeparator1;
    }
}

