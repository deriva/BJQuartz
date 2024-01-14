namespace BJ.Quartz.Fm
{
    partial class FmMsg
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
            lstMsgBox = new ListBox();
            SuspendLayout();
            // 
            // lstMsgBox
            // 
            lstMsgBox.Dock = DockStyle.Fill;
            lstMsgBox.FormattingEnabled = true;
            lstMsgBox.ItemHeight = 20;
            lstMsgBox.Location = new Point(0, 0);
            lstMsgBox.Name = "lstMsgBox";
            lstMsgBox.Size = new Size(800, 450);
            lstMsgBox.TabIndex = 0;
            // 
            // FmMsg
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lstMsgBox);
            Name = "FmMsg";
            Text = "消息窗体";
            Load += FmMsg_Load;
            ResumeLayout(false);
        }

        #endregion

        private ListBox lstMsgBox;
    }
}