namespace BJ.Quartz.Fm
{
    partial class FmHost
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
            btnStart = new Button();
            btnStop = new Button();
            lab1 = new Label();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(26, 60);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(94, 29);
            btnStart.TabIndex = 0;
            btnStart.Text = "启动";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += button1_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(197, 61);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(94, 29);
            btnStop.TabIndex = 1;
            btnStop.Text = "停止";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lab1
            // 
            lab1.AutoSize = true;
            lab1.Location = new Point(59, 117);
            lab1.Name = "lab1";
            lab1.Size = new Size(69, 20);
            lab1.TabIndex = 2;
            lab1.Text = "当前状态";
            lab1.Click += lab1_Click;
            // 
            // FmHost
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(487, 166);
            Controls.Add(lab1);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Name = "FmHost";
            Text = "Http服务";
            Load += FmHost_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private Label lab1;
    }
}