namespace TUKE_Recognition_of_speech
{
    partial class TUKEasr
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TUKEasr));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.Record = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Record)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "TUKE asr has been minimized!";
            this.notifyIcon1.BalloonTipTitle = "TUKEasr";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "TUKE Recognition of speech";
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1_MouseClick);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // Record
            // 
            this.Record.BackColor = System.Drawing.Color.Khaki;
            this.Record.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Record.BackgroundImage")));
            this.Record.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Record.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Record.ErrorImage = null;
            this.Record.InitialImage = null;
            this.Record.Location = new System.Drawing.Point(0, 0);
            this.Record.Margin = new System.Windows.Forms.Padding(0);
            this.Record.Name = "Record";
            this.Record.Size = new System.Drawing.Size(100, 103);
            this.Record.TabIndex = 1;
            this.Record.TabStop = false;
            this.Record.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Record_Click);
            this.Record.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove);
            // 
            // TUKEasr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Khaki;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(100, 103);
            this.Controls.Add(this.Record);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TUKEasr";
            this.ShowInTaskbar = false;
            this.Text = "TUKE asr";
            this.Resize += new System.EventHandler(this.TUKEasr_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.Record)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.PictureBox Record;
    }
}

