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
            this.label1 = new System.Windows.Forms.Label();
            this.myReports = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // myOkBtn
            // 
            this.myOkBtn.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.myOkBtn.Location = new System.Drawing.Point( 562, 434 );
            this.myOkBtn.Name = "myOkBtn";
            this.myOkBtn.Size = new System.Drawing.Size( 75, 23 );
            this.myOkBtn.TabIndex = 0;
            this.myOkBtn.Text = "&Ok";
            this.myOkBtn.UseVisualStyleBackColor = true;
            this.myOkBtn.Click += new System.EventHandler( this.myOkBtn_Click );
            // 
            // myBrowser
            // 
            this.myBrowser.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.myBrowser.Location = new System.Drawing.Point( 3, 32 );
            this.myBrowser.MinimumSize = new System.Drawing.Size( 20, 20 );
            this.myBrowser.Name = "myBrowser";
            this.myBrowser.Size = new System.Drawing.Size( 634, 396 );
            this.myBrowser.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 0, 8 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 82, 13 );
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose a report";
            // 
            // myReports
            // 
            this.myReports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.myReports.FormattingEnabled = true;
            this.myReports.Location = new System.Drawing.Point( 88, 5 );
            this.myReports.Name = "myReports";
            this.myReports.Size = new System.Drawing.Size( 275, 21 );
            this.myReports.TabIndex = 3;
            this.myReports.SelectionChangeCommitted += new System.EventHandler( this.comboBox1_SelectionChangeCommitted );
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 639, 459 );
            this.Controls.Add( this.myReports );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.myBrowser );
            this.Controls.Add( this.myOkBtn );
            this.Name = "ReportForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Report";
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button myOkBtn;
        private System.Windows.Forms.WebBrowser myBrowser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox myReports;
    }
}