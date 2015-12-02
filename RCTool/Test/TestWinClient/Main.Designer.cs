namespace TestWinClient
{
    partial class Main
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
            this.btnDownload = new System.Windows.Forms.Button();
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.lblURL = new System.Windows.Forms.Label();
            this.chkZip = new System.Windows.Forms.CheckBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.txtfileName = new System.Windows.Forms.TextBox();
            this.btnDoPost = new System.Windows.Forms.Button();
            this.btnGetCheckSum = new System.Windows.Forms.Button();
            this.txtReportNumber = new System.Windows.Forms.TextBox();
            this.btnStressChecksum = new System.Windows.Forms.Button();
            this.lblStressTimes = new System.Windows.Forms.Label();
            this.txtStressCycles = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(392, 42);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(126, 23);
            this.btnDownload.TabIndex = 0;
            this.btnDownload.Text = "Download document";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // txtMessages
            // 
            this.txtMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessages.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessages.Location = new System.Drawing.Point(12, 110);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessages.Size = new System.Drawing.Size(1041, 389);
            this.txtMessages.TabIndex = 1;
            // 
            // txtURL
            // 
            this.txtURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtURL.Location = new System.Drawing.Point(50, 16);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(1003, 20);
            this.txtURL.TabIndex = 2;
            this.txtURL.Text = "http://localhost:88/TestRCToolService/Service1.svc/getfilemap/1";
            // 
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new System.Drawing.Point(12, 19);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(32, 13);
            this.lblURL.TabIndex = 3;
            this.lblURL.Text = "URL:";
            // 
            // chkZip
            // 
            this.chkZip.AutoSize = true;
            this.chkZip.Checked = true;
            this.chkZip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkZip.Location = new System.Drawing.Point(288, 46);
            this.chkZip.Name = "chkZip";
            this.chkZip.Size = new System.Drawing.Size(98, 17);
            this.chkZip.TabIndex = 4;
            this.chkZip.Text = "GZip Request?";
            this.chkZip.UseVisualStyleBackColor = true;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(12, 47);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(55, 13);
            this.lblFileName.TabIndex = 5;
            this.lblFileName.Text = "File name:";
            // 
            // txtfileName
            // 
            this.txtfileName.Location = new System.Drawing.Point(73, 44);
            this.txtfileName.Name = "txtfileName";
            this.txtfileName.Size = new System.Drawing.Size(194, 20);
            this.txtfileName.TabIndex = 6;
            this.txtfileName.Text = "download.pdf";
            // 
            // btnDoPost
            // 
            this.btnDoPost.Location = new System.Drawing.Point(524, 42);
            this.btnDoPost.Name = "btnDoPost";
            this.btnDoPost.Size = new System.Drawing.Size(75, 23);
            this.btnDoPost.TabIndex = 7;
            this.btnDoPost.Text = "Do POST";
            this.btnDoPost.UseVisualStyleBackColor = true;
            this.btnDoPost.Click += new System.EventHandler(this.btnDoPost_Click);
            // 
            // btnGetCheckSum
            // 
            this.btnGetCheckSum.Location = new System.Drawing.Point(39, 77);
            this.btnGetCheckSum.Name = "btnGetCheckSum";
            this.btnGetCheckSum.Size = new System.Drawing.Size(125, 23);
            this.btnGetCheckSum.TabIndex = 8;
            this.btnGetCheckSum.Text = "Get CheckSum from:";
            this.btnGetCheckSum.UseVisualStyleBackColor = true;
            this.btnGetCheckSum.Click += new System.EventHandler(this.btnGetCheckSum_Click);
            // 
            // txtReportNumber
            // 
            this.txtReportNumber.Location = new System.Drawing.Point(170, 79);
            this.txtReportNumber.Name = "txtReportNumber";
            this.txtReportNumber.Size = new System.Drawing.Size(74, 20);
            this.txtReportNumber.TabIndex = 9;
            this.txtReportNumber.Text = "1";
            // 
            // btnStressChecksum
            // 
            this.btnStressChecksum.Location = new System.Drawing.Point(274, 77);
            this.btnStressChecksum.Name = "btnStressChecksum";
            this.btnStressChecksum.Size = new System.Drawing.Size(228, 23);
            this.btnStressChecksum.TabIndex = 10;
            this.btnStressChecksum.Text = "Stress Checksum Computation";
            this.btnStressChecksum.UseVisualStyleBackColor = true;
            this.btnStressChecksum.Click += new System.EventHandler(this.btnStressChecksum_Click);
            // 
            // lblStressTimes
            // 
            this.lblStressTimes.AutoSize = true;
            this.lblStressTimes.Location = new System.Drawing.Point(508, 82);
            this.lblStressTimes.Name = "lblStressTimes";
            this.lblStressTimes.Size = new System.Drawing.Size(73, 13);
            this.lblStressTimes.TabIndex = 11;
            this.lblStressTimes.Text = "Stress Cycles:";
            // 
            // txtStressCycles
            // 
            this.txtStressCycles.Location = new System.Drawing.Point(587, 79);
            this.txtStressCycles.Name = "txtStressCycles";
            this.txtStressCycles.Size = new System.Drawing.Size(100, 20);
            this.txtStressCycles.TabIndex = 12;
            this.txtStressCycles.Text = "300";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 511);
            this.Controls.Add(this.txtStressCycles);
            this.Controls.Add(this.lblStressTimes);
            this.Controls.Add(this.btnStressChecksum);
            this.Controls.Add(this.txtReportNumber);
            this.Controls.Add(this.btnGetCheckSum);
            this.Controls.Add(this.btnDoPost);
            this.Controls.Add(this.txtfileName);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.chkZip);
            this.Controls.Add(this.lblURL);
            this.Controls.Add(this.txtURL);
            this.Controls.Add(this.txtMessages);
            this.Controls.Add(this.btnDownload);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.TextBox txtMessages;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.CheckBox chkZip;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox txtfileName;
        private System.Windows.Forms.Button btnDoPost;
        private System.Windows.Forms.Button btnGetCheckSum;
        private System.Windows.Forms.TextBox txtReportNumber;
        private System.Windows.Forms.Button btnStressChecksum;
        private System.Windows.Forms.Label lblStressTimes;
        private System.Windows.Forms.TextBox txtStressCycles;
    }
}

