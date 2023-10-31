namespace EchoRelay.App.Forms.Controls
{
    partial class AccountSelector
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            listAccounts = new ListView();
            columnHeaderUserId = new ColumnHeader();
            accountEditor = new AccountEditor();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(listAccounts);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(accountEditor);
            splitContainer1.Size = new Size(829, 474);
            splitContainer1.SplitterDistance = 176;
            splitContainer1.TabIndex = 0;
            // 
            // listAccounts
            // 
            listAccounts.Columns.AddRange(new ColumnHeader[] { columnHeaderUserId });
            listAccounts.Dock = DockStyle.Fill;
            listAccounts.FullRowSelect = true;
            listAccounts.GridLines = true;
            listAccounts.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listAccounts.LabelWrap = false;
            listAccounts.Location = new Point(0, 0);
            listAccounts.MultiSelect = false;
            listAccounts.Name = "listAccounts";
            listAccounts.Size = new Size(176, 474);
            listAccounts.Sorting = SortOrder.Ascending;
            listAccounts.TabIndex = 2;
            listAccounts.UseCompatibleStateImageBehavior = false;
            listAccounts.View = View.Details;
            listAccounts.SelectedIndexChanged += listAccounts_SelectedIndexChanged;
            // 
            // columnHeaderUserId
            // 
            columnHeaderUserId.Text = "User Identifier";
            columnHeaderUserId.Width = 150;
            // 
            // accountEditor
            // 
            accountEditor.Account = null;
            accountEditor.Dock = DockStyle.Top;
            accountEditor.Location = new Point(0, 0);
            accountEditor.Name = "accountEditor";
            accountEditor.Size = new Size(649, 177);
            accountEditor.TabIndex = 0;
            // 
            // AccountSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "AccountSelector";
            Size = new Size(829, 474);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private ListView listAccounts;
        private ColumnHeader columnHeaderUserId;
        private AccountEditor accountEditor;
    }
}
