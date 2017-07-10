namespace MultiRunner
{
    partial class TestSetForm
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
            this.btSave = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.testSetTable = new System.Windows.Forms.DataGridView();
            this.TestSets = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.testSetTable)).BeginInit();
            this.SuspendLayout();
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(55, 235);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 1;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(147, 235);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // testSetTable
            // 
            this.testSetTable.AllowUserToResizeColumns = false;
            this.testSetTable.AllowUserToResizeRows = false;
            this.testSetTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.testSetTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TestSets});
            this.testSetTable.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.testSetTable.Location = new System.Drawing.Point(12, 12);
            this.testSetTable.Name = "testSetTable";
            this.testSetTable.RowHeadersVisible = false;
            this.testSetTable.Size = new System.Drawing.Size(266, 192);
            this.testSetTable.TabIndex = 3;
            // 
            // TestSets
            // 
            this.TestSets.HeaderText = "Tests Sets";
            this.TestSets.Name = "TestSets";
            this.TestSets.Width = 262;
            // 
            // btReset
            // 
            this.btReset.Location = new System.Drawing.Point(11, 206);
            this.btReset.Name = "btReset";
            this.btReset.Size = new System.Drawing.Size(76, 23);
            this.btReset.TabIndex = 4;
            this.btReset.Text = "Use Defaults";
            this.btReset.UseVisualStyleBackColor = true;
            this.btReset.Click += new System.EventHandler(this.btReset_Click);
            // 
            // TestSetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btReset);
            this.Controls.Add(this.testSetTable);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestSetForm";
            this.Text = "Tests Sets";
            this.Load += new System.EventHandler(this.TestSetForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.testSetTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.DataGridView testSetTable;
        private System.Windows.Forms.Button btReset;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestSets;

    }
}