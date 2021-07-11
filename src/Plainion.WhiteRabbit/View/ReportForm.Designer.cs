namespace Plainion.WhiteRabbit.View
{
    partial class ReportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.myOkBtn = new System.Windows.Forms.Button();
            this.myBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // myOkBtn
            // 
            this.myOkBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.myOkBtn.Location = new System.Drawing.Point(656, 501);
            this.myOkBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.myOkBtn.Name = "myOkBtn";
            this.myOkBtn.Size = new System.Drawing.Size(88, 27);
            this.myOkBtn.TabIndex = 0;
            this.myOkBtn.Text = "&Ok";
            this.myOkBtn.UseVisualStyleBackColor = true;
            this.myOkBtn.Click += new System.EventHandler(this.myOkBtn_Click);
            // 
            // myBrowser
            // 
            this.myBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myBrowser.Location = new System.Drawing.Point(4, 8);
            this.myBrowser.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.myBrowser.MinimumSize = new System.Drawing.Size(23, 23);
            this.myBrowser.Name = "myBrowser";
            this.myBrowser.Size = new System.Drawing.Size(740, 487);
            this.myBrowser.TabIndex = 1;
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 530);
            this.Controls.Add(this.myBrowser);
            this.Controls.Add(this.myOkBtn);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ReportForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Report";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button myOkBtn;
        private System.Windows.Forms.WebBrowser myBrowser;
    }
}