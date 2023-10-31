namespace EchoRelay.App.Forms.Controls
{
    partial class AccessControlListEditor
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
            allowRulesEditor = new AccessControlListRuleEditor();
            disallowRulesEditor = new AccessControlListRuleEditor();
            splitContainer1 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // allowRulesEditor
            // 
            allowRulesEditor.Dock = DockStyle.Fill;
            allowRulesEditor.Location = new Point(0, 0);
            allowRulesEditor.Name = "allowRulesEditor";
            allowRulesEditor.RuleSetName = "Allow Rules";
            allowRulesEditor.Size = new Size(914, 379);
            allowRulesEditor.TabIndex = 0;
            allowRulesEditor.RuleSetChanged += rulesEditor_RuleSetChanged;
            // 
            // disallowRulesEditor
            // 
            disallowRulesEditor.Dock = DockStyle.Fill;
            disallowRulesEditor.Location = new Point(0, 0);
            disallowRulesEditor.Name = "disallowRulesEditor";
            disallowRulesEditor.RuleSetName = "Disallow Rules";
            disallowRulesEditor.Size = new Size(914, 375);
            disallowRulesEditor.TabIndex = 1;
            disallowRulesEditor.RuleSetChanged += rulesEditor_RuleSetChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(allowRulesEditor);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(disallowRulesEditor);
            splitContainer1.Size = new Size(914, 758);
            splitContainer1.SplitterDistance = 379;
            splitContainer1.TabIndex = 2;
            // 
            // AccessControlListEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "AccessControlListEditor";
            Size = new Size(914, 758);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AccessControlListRuleEditor allowRulesEditor;
        private AccessControlListRuleEditor disallowRulesEditor;
        private SplitContainer splitContainer1;
    }
}
