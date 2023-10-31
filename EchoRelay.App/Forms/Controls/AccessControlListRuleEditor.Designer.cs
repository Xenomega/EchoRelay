namespace EchoRelay.App.Forms.Controls
{
    partial class AccessControlListRuleEditor
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
            lblRulesHeader = new Label();
            txtRule = new TextBox();
            listRules = new ListBox();
            btnUpdate = new Button();
            btnAdd = new Button();
            btnRemove = new Button();
            SuspendLayout();
            // 
            // lblRulesHeader
            // 
            lblRulesHeader.AutoSize = true;
            lblRulesHeader.Location = new Point(3, 6);
            lblRulesHeader.Name = "lblRulesHeader";
            lblRulesHeader.Size = new Size(38, 15);
            lblRulesHeader.TabIndex = 10;
            lblRulesHeader.Text = "Rules:";
            // 
            // txtRule
            // 
            txtRule.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtRule.Location = new Point(137, 136);
            txtRule.Name = "txtRule";
            txtRule.Size = new Size(314, 23);
            txtRule.TabIndex = 11;
            // 
            // listRules
            // 
            listRules.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listRules.FormattingEnabled = true;
            listRules.ItemHeight = 15;
            listRules.Location = new Point(137, 6);
            listRules.Name = "listRules";
            listRules.Size = new Size(385, 124);
            listRules.Sorted = true;
            listRules.TabIndex = 13;
            listRules.SelectedIndexChanged += listRules_SelectedIndexChanged;
            // 
            // btnUpdate
            // 
            btnUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnUpdate.Location = new Point(457, 136);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(65, 23);
            btnUpdate.TabIndex = 14;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.Location = new Point(528, 6);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(25, 25);
            btnAdd.TabIndex = 15;
            btnAdd.Text = "+";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnRemove
            // 
            btnRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemove.Location = new Point(528, 37);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(25, 25);
            btnRemove.TabIndex = 16;
            btnRemove.Text = "-";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // AccessControlListRuleEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnRemove);
            Controls.Add(btnAdd);
            Controls.Add(btnUpdate);
            Controls.Add(listRules);
            Controls.Add(txtRule);
            Controls.Add(lblRulesHeader);
            Name = "AccessControlListRuleEditor";
            Size = new Size(617, 168);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblRulesHeader;
        private TextBox txtRule;
        private ListBox listRules;
        private Button btnUpdate;
        private Button btnAdd;
        private Button btnRemove;
    }
}
