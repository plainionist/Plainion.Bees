namespace Plainion.WhiteRabbit
{
    partial class CategoriesForm
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
            if ( disposing && (components != null) )
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
            this.myCategoryList = new System.Windows.Forms.ListBox();
            this.myCategoryTxt = new System.Windows.Forms.TextBox();
            this.myAddBtn = new System.Windows.Forms.Button();
            this.myDelBtn = new System.Windows.Forms.Button();
            this.myOkBtn = new System.Windows.Forms.Button();
            this.myCancelBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.myRenameBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // myCategoryList
            // 
            this.myCategoryList.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.myCategoryList.FormattingEnabled = true;
            this.myCategoryList.Location = new System.Drawing.Point( 3, 96 );
            this.myCategoryList.Name = "myCategoryList";
            this.myCategoryList.Size = new System.Drawing.Size( 237, 121 );
            this.myCategoryList.TabIndex = 0;
            this.myCategoryList.SelectedIndexChanged += new System.EventHandler( this.myCategoryList_SelectedIndexChanged );
            // 
            // myCategoryTxt
            // 
            this.myCategoryTxt.Location = new System.Drawing.Point( 3, 21 );
            this.myCategoryTxt.Name = "myCategoryTxt";
            this.myCategoryTxt.Size = new System.Drawing.Size( 237, 20 );
            this.myCategoryTxt.TabIndex = 1;
            // 
            // myAddBtn
            // 
            this.myAddBtn.Location = new System.Drawing.Point( 3, 47 );
            this.myAddBtn.Name = "myAddBtn";
            this.myAddBtn.Size = new System.Drawing.Size( 60, 23 );
            this.myAddBtn.TabIndex = 2;
            this.myAddBtn.Text = "&Add";
            this.myAddBtn.UseVisualStyleBackColor = true;
            this.myAddBtn.Click += new System.EventHandler( this.myAddBtn_Click );
            // 
            // myDelBtn
            // 
            this.myDelBtn.Location = new System.Drawing.Point( 69, 47 );
            this.myDelBtn.Name = "myDelBtn";
            this.myDelBtn.Size = new System.Drawing.Size( 60, 23 );
            this.myDelBtn.TabIndex = 3;
            this.myDelBtn.Text = "&Delete";
            this.myDelBtn.UseVisualStyleBackColor = true;
            this.myDelBtn.Click += new System.EventHandler( this.myDelBtn_Click );
            // 
            // myOkBtn
            // 
            this.myOkBtn.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.myOkBtn.Location = new System.Drawing.Point( 84, 223 );
            this.myOkBtn.Name = "myOkBtn";
            this.myOkBtn.Size = new System.Drawing.Size( 75, 23 );
            this.myOkBtn.TabIndex = 4;
            this.myOkBtn.Text = "Ok";
            this.myOkBtn.UseVisualStyleBackColor = true;
            this.myOkBtn.Click += new System.EventHandler( this.myOkBtn_Click );
            // 
            // myCancelBtn
            // 
            this.myCancelBtn.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.myCancelBtn.Location = new System.Drawing.Point( 165, 223 );
            this.myCancelBtn.Name = "myCancelBtn";
            this.myCancelBtn.Size = new System.Drawing.Size( 75, 23 );
            this.myCancelBtn.TabIndex = 5;
            this.myCancelBtn.Text = "Cancel";
            this.myCancelBtn.UseVisualStyleBackColor = true;
            this.myCancelBtn.Click += new System.EventHandler( this.myCancelBtn_Click );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 1, 5 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 49, 13 );
            this.label1.TabIndex = 6;
            this.label1.Text = "Category";
            // 
            // label2
            // 
            this.label2.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 0, 80 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 105, 13 );
            this.label2.TabIndex = 7;
            this.label2.Text = "Available categories:";
            // 
            // myRenameBtn
            // 
            this.myRenameBtn.Location = new System.Drawing.Point( 135, 47 );
            this.myRenameBtn.Name = "myRenameBtn";
            this.myRenameBtn.Size = new System.Drawing.Size( 60, 23 );
            this.myRenameBtn.TabIndex = 8;
            this.myRenameBtn.Text = "&Rename";
            this.myRenameBtn.UseVisualStyleBackColor = true;
            this.myRenameBtn.Click += new System.EventHandler( this.myRenameBtn_Click );
            // 
            // CategoriesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 243, 250 );
            this.Controls.Add( this.myRenameBtn );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.myCancelBtn );
            this.Controls.Add( this.myOkBtn );
            this.Controls.Add( this.myDelBtn );
            this.Controls.Add( this.myAddBtn );
            this.Controls.Add( this.myCategoryTxt );
            this.Controls.Add( this.myCategoryList );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CategoriesForm";
            this.Text = "Manage categories";
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox myCategoryList;
        private System.Windows.Forms.TextBox myCategoryTxt;
        private System.Windows.Forms.Button myAddBtn;
        private System.Windows.Forms.Button myDelBtn;
        private System.Windows.Forms.Button myOkBtn;
        private System.Windows.Forms.Button myCancelBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button myRenameBtn;
    }
}