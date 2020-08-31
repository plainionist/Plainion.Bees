using System;
using System.Drawing;

namespace Plainion.WhiteRabbit.View
{
    partial class SlimForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose( bool disposing )
        {
            if( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlimForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.myTimeElapsed = new System.Windows.Forms.Label();
            this.myStopRecordBtn = new System.Windows.Forms.Button();
            this.myCommentTxt = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.myTimeElapsed);
            this.panel1.Controls.Add(this.myStopRecordBtn);
            this.panel1.Controls.Add(this.myCommentTxt);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(223, 29);
            this.panel1.TabIndex = 17;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // myTimeElapsed
            // 
            this.myTimeElapsed.AutoSize = true;
            this.myTimeElapsed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.myTimeElapsed.Location = new System.Drawing.Point(166, 8);
            this.myTimeElapsed.Name = "myTimeElapsed";
            this.myTimeElapsed.Size = new System.Drawing.Size(51, 15);
            this.myTimeElapsed.TabIndex = 20;
            this.myTimeElapsed.Text = "00:00:00";
            // 
            // myStopRecordBtn
            // 
            this.myStopRecordBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.myStopRecordBtn.Image = ((System.Drawing.Image)(resources.GetObject("myStopRecordBtn.Image")));
            this.myStopRecordBtn.Location = new System.Drawing.Point(3, 3);
            this.myStopRecordBtn.Name = "myStopRecordBtn";
            this.myStopRecordBtn.Size = new System.Drawing.Size(23, 23);
            this.myStopRecordBtn.TabIndex = 18;
            this.myStopRecordBtn.UseVisualStyleBackColor = true;
            this.myStopRecordBtn.Click += new System.EventHandler(this.myStopRecordBtn_Click);
            // 
            // myCommentTxt
            // 
            this.myCommentTxt.Location = new System.Drawing.Point(32, 5);
            this.myCommentTxt.Name = "myCommentTxt";
            this.myCommentTxt.Size = new System.Drawing.Size(128, 20);
            this.myCommentTxt.TabIndex = 21;
            // 
            // SlimForm
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(223, 29);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SlimForm";
            this.ControlBox = false;
            this.Text = String.Empty;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SlimForm_MouseDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label myTimeElapsed;
        private System.Windows.Forms.Button myStopRecordBtn;
        private System.Windows.Forms.TextBox myCommentTxt;

    }
}