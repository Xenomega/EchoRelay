namespace EchoRelay.App.Forms.Controls
{
    partial class ChannelInfoEditor
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
            lblChannelHeader = new Label();
            comboBoxChannel = new ComboBox();
            lblNameHeader = new Label();
            txtName = new TextBox();
            lblDescriptionHeader = new Label();
            txtDescription = new RichTextBox();
            txtRules = new RichTextBox();
            lblRules = new Label();
            lblLinkHeader = new Label();
            txtLink = new TextBox();
            lblRulesVersionHeader = new Label();
            numRulesVersion = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)numRulesVersion).BeginInit();
            SuspendLayout();
            // 
            // lblChannelHeader
            // 
            lblChannelHeader.AutoSize = true;
            lblChannelHeader.Location = new Point(3, 9);
            lblChannelHeader.Name = "lblChannelHeader";
            lblChannelHeader.Size = new Size(54, 15);
            lblChannelHeader.TabIndex = 0;
            lblChannelHeader.Text = "Channel:";
            // 
            // comboBoxChannel
            // 
            comboBoxChannel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxChannel.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxChannel.FormattingEnabled = true;
            comboBoxChannel.Location = new Point(114, 6);
            comboBoxChannel.Name = "comboBoxChannel";
            comboBoxChannel.Size = new Size(345, 23);
            comboBoxChannel.TabIndex = 1;
            comboBoxChannel.SelectedIndexChanged += comboBoxChannel_SelectedIndexChanged;
            // 
            // lblNameHeader
            // 
            lblNameHeader.AutoSize = true;
            lblNameHeader.Location = new Point(3, 38);
            lblNameHeader.Name = "lblNameHeader";
            lblNameHeader.Size = new Size(42, 15);
            lblNameHeader.TabIndex = 2;
            lblNameHeader.Text = "Name:";
            // 
            // txtName
            // 
            txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtName.Location = new Point(114, 35);
            txtName.Name = "txtName";
            txtName.Size = new Size(345, 23);
            txtName.TabIndex = 3;
            txtName.TextChanged += onAnyValueChanged;
            // 
            // lblDescriptionHeader
            // 
            lblDescriptionHeader.AutoSize = true;
            lblDescriptionHeader.Location = new Point(3, 67);
            lblDescriptionHeader.Name = "lblDescriptionHeader";
            lblDescriptionHeader.Size = new Size(70, 15);
            lblDescriptionHeader.TabIndex = 4;
            lblDescriptionHeader.Text = "Description:";
            // 
            // txtDescription
            // 
            txtDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescription.Location = new Point(114, 67);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(345, 96);
            txtDescription.TabIndex = 5;
            txtDescription.Text = "";
            txtDescription.TextChanged += onAnyValueChanged;
            // 
            // txtRules
            // 
            txtRules.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRules.Location = new Point(114, 169);
            txtRules.Name = "txtRules";
            txtRules.Size = new Size(345, 96);
            txtRules.TabIndex = 7;
            txtRules.Text = "";
            txtRules.TextChanged += onAnyValueChanged;
            // 
            // lblRules
            // 
            lblRules.AutoSize = true;
            lblRules.Location = new Point(3, 169);
            lblRules.Name = "lblRules";
            lblRules.Size = new Size(38, 15);
            lblRules.TabIndex = 6;
            lblRules.Text = "Rules:";
            // 
            // lblLinkHeader
            // 
            lblLinkHeader.AutoSize = true;
            lblLinkHeader.Location = new Point(3, 303);
            lblLinkHeader.Name = "lblLinkHeader";
            lblLinkHeader.Size = new Size(32, 15);
            lblLinkHeader.TabIndex = 8;
            lblLinkHeader.Text = "Link:";
            // 
            // txtLink
            // 
            txtLink.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLink.Location = new Point(114, 300);
            txtLink.Name = "txtLink";
            txtLink.Size = new Size(345, 23);
            txtLink.TabIndex = 9;
            txtLink.TextChanged += onAnyValueChanged;
            // 
            // lblRulesVersionHeader
            // 
            lblRulesVersionHeader.AutoSize = true;
            lblRulesVersionHeader.Location = new Point(3, 273);
            lblRulesVersionHeader.Name = "lblRulesVersionHeader";
            lblRulesVersionHeader.Size = new Size(79, 15);
            lblRulesVersionHeader.TabIndex = 10;
            lblRulesVersionHeader.Text = "Rules Version:";
            // 
            // numRulesVersion
            // 
            numRulesVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numRulesVersion.Location = new Point(114, 271);
            numRulesVersion.Maximum = new decimal(new int[] { -1, -1, 0, 0 });
            numRulesVersion.Name = "numRulesVersion";
            numRulesVersion.Size = new Size(345, 23);
            numRulesVersion.TabIndex = 11;
            numRulesVersion.ValueChanged += onAnyValueChanged;
            // 
            // ChannelInfoEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(numRulesVersion);
            Controls.Add(lblRulesVersionHeader);
            Controls.Add(txtLink);
            Controls.Add(lblLinkHeader);
            Controls.Add(txtRules);
            Controls.Add(lblRules);
            Controls.Add(txtDescription);
            Controls.Add(lblDescriptionHeader);
            Controls.Add(txtName);
            Controls.Add(lblNameHeader);
            Controls.Add(comboBoxChannel);
            Controls.Add(lblChannelHeader);
            Name = "ChannelInfoEditor";
            Size = new Size(462, 331);
            ((System.ComponentModel.ISupportInitialize)numRulesVersion).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblChannelHeader;
        private ComboBox comboBoxChannel;
        private Label lblNameHeader;
        private TextBox txtName;
        private Label lblDescriptionHeader;
        private RichTextBox txtDescription;
        private RichTextBox txtRules;
        private Label lblRules;
        private Label lblLinkHeader;
        private TextBox txtLink;
        private Label lblRulesVersionHeader;
        private NumericUpDown numRulesVersion;
    }
}
