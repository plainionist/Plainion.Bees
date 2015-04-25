namespace Plainion.Scripts.Loc
{
    partial class MainForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.myDir = new System.Windows.Forms.TextBox();
            this.myTree = new System.Windows.Forms.TreeView();
            this.myRunBtn = new System.Windows.Forms.Button();
            this.myTable = new System.Windows.Forms.DataGridView();
            this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PSLOC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PCLOC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PELOC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GLOC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TSLOC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TCLOC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TELOC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.myDumpBtn = new System.Windows.Forms.Button();
            ( (System.ComponentModel.ISupportInitialize)( this.myTable ) ).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 5, 10 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 40, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Project";
            // 
            // myDir
            // 
            this.myDir.Location = new System.Drawing.Point( 51, 7 );
            this.myDir.Name = "myDir";
            this.myDir.Size = new System.Drawing.Size( 326, 20 );
            this.myDir.TabIndex = 1;
            // 
            // myTree
            // 
            this.myTree.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.myTree.Location = new System.Drawing.Point( 4, 21 );
            this.myTree.Name = "myTree";
            this.myTree.Size = new System.Drawing.Size( 224, 330 );
            this.myTree.TabIndex = 2;
            this.myTree.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler( this.myTree_AfterCollapse );
            this.myTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.myTree_AfterSelect );
            this.myTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler( this.myTree_AfterExpand );
            // 
            // myRunBtn
            // 
            this.myRunBtn.Location = new System.Drawing.Point( 383, 5 );
            this.myRunBtn.Name = "myRunBtn";
            this.myRunBtn.Size = new System.Drawing.Size( 39, 23 );
            this.myRunBtn.TabIndex = 3;
            this.myRunBtn.Text = "Run";
            this.myRunBtn.UseVisualStyleBackColor = true;
            this.myRunBtn.Click += new System.EventHandler( this.myRunBtn_Click );
            // 
            // myTable
            // 
            this.myTable.AllowUserToAddRows = false;
            this.myTable.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.myTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myTable.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.Total,
            this.PSLOC,
            this.PCLOC,
            this.PELOC,
            this.GLOC,
            this.TSLOC,
            this.TCLOC,
            this.TELOC} );
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.myTable.DefaultCellStyle = dataGridViewCellStyle1;
            this.myTable.Location = new System.Drawing.Point( 3, 3 );
            this.myTable.Name = "myTable";
            this.myTable.ReadOnly = true;
            this.myTable.RowHeadersVisible = false;
            this.myTable.Size = new System.Drawing.Size( 404, 348 );
            this.myTable.TabIndex = 4;
            // 
            // Total
            // 
            this.Total.HeaderText = "Total";
            this.Total.Name = "Total";
            this.Total.ReadOnly = true;
            this.Total.Width = 50;
            // 
            // PSLOC
            // 
            this.PSLOC.HeaderText = "PSLOC";
            this.PSLOC.Name = "PSLOC";
            this.PSLOC.ReadOnly = true;
            this.PSLOC.Width = 50;
            // 
            // PCLOC
            // 
            this.PCLOC.HeaderText = "PCLOC";
            this.PCLOC.Name = "PCLOC";
            this.PCLOC.ReadOnly = true;
            this.PCLOC.Width = 50;
            // 
            // PELOC
            // 
            this.PELOC.HeaderText = "PELOC";
            this.PELOC.Name = "PELOC";
            this.PELOC.ReadOnly = true;
            this.PELOC.Width = 50;
            // 
            // GLOC
            // 
            this.GLOC.HeaderText = "GLOC";
            this.GLOC.Name = "GLOC";
            this.GLOC.ReadOnly = true;
            this.GLOC.Width = 50;
            // 
            // TSLOC
            // 
            this.TSLOC.HeaderText = "TSLOC";
            this.TSLOC.Name = "TSLOC";
            this.TSLOC.ReadOnly = true;
            this.TSLOC.Width = 50;
            // 
            // TCLOC
            // 
            this.TCLOC.HeaderText = "TCLOC";
            this.TCLOC.Name = "TCLOC";
            this.TCLOC.ReadOnly = true;
            this.TCLOC.Width = 50;
            // 
            // TELOC
            // 
            this.TELOC.HeaderText = "TELOC";
            this.TELOC.Name = "TELOC";
            this.TELOC.ReadOnly = true;
            this.TELOC.Width = 50;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.splitContainer1.Location = new System.Drawing.Point( -1, 37 );
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add( this.myTree );
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add( this.myTable );
            this.splitContainer1.Size = new System.Drawing.Size( 645, 354 );
            this.splitContainer1.SplitterDistance = 231;
            this.splitContainer1.TabIndex = 5;
            // 
            // myDumpBtn
            // 
            this.myDumpBtn.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.myDumpBtn.Location = new System.Drawing.Point( 578, 5 );
            this.myDumpBtn.Name = "myDumpBtn";
            this.myDumpBtn.Size = new System.Drawing.Size( 63, 23 );
            this.myDumpBtn.TabIndex = 6;
            this.myDumpBtn.Text = "Snapshot";
            this.myDumpBtn.UseVisualStyleBackColor = true;
            this.myDumpBtn.Click += new System.EventHandler( this.myDumpBtn_Click );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 645, 392 );
            this.Controls.Add( this.myDumpBtn );
            this.Controls.Add( this.splitContainer1 );
            this.Controls.Add( this.myRunBtn );
            this.Controls.Add( this.myDir );
            this.Controls.Add( this.label1 );
            this.Name = "MainForm";
            this.Text = "myLOC";
            this.Load += new System.EventHandler( this.MainForm_Load );
            ( (System.ComponentModel.ISupportInitialize)( this.myTable ) ).EndInit();
            this.splitContainer1.Panel1.ResumeLayout( false );
            this.splitContainer1.Panel2.ResumeLayout( false );
            this.splitContainer1.ResumeLayout( false );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox myDir;
        private System.Windows.Forms.TreeView myTree;
        private System.Windows.Forms.Button myRunBtn;
        private System.Windows.Forms.DataGridView myTable;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Total;
        private System.Windows.Forms.DataGridViewTextBoxColumn PSLOC;
        private System.Windows.Forms.DataGridViewTextBoxColumn PCLOC;
        private System.Windows.Forms.DataGridViewTextBoxColumn PELOC;
        private System.Windows.Forms.DataGridViewTextBoxColumn GLOC;
        private System.Windows.Forms.DataGridViewTextBoxColumn TSLOC;
        private System.Windows.Forms.DataGridViewTextBoxColumn TCLOC;
        private System.Windows.Forms.DataGridViewTextBoxColumn TELOC;
        private System.Windows.Forms.Button myDumpBtn;
    }
}

