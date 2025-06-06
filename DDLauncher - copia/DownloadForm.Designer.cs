namespace Dragones_de_Dojima
{
    partial class DownloadForm
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
            progressBarDownload = new ProgressBar();
            lblDownloadStatus = new Label();
            SuspendLayout();
            // 
            // progressBarDownload
            // 
            progressBarDownload.Location = new Point(12, 38);
            progressBarDownload.Name = "progressBarDownload";
            progressBarDownload.Size = new Size(302, 23);
            progressBarDownload.TabIndex = 0;
            // 
            // lblDownloadStatus
            // 
            lblDownloadStatus.AutoSize = true;
            lblDownloadStatus.Location = new Point(12, 20);
            lblDownloadStatus.Name = "lblDownloadStatus";
            lblDownloadStatus.Size = new Size(0, 15);
            lblDownloadStatus.TabIndex = 1;
            // 
            // DownloadForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(326, 88);
            Controls.Add(lblDownloadStatus);
            Controls.Add(progressBarDownload);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "DownloadForm";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Actualizando";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBarDownload;
        private Label lblDownloadStatus;
    }
}