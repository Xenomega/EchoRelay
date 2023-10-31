namespace EchoRelay.App.Forms.Controls
{
    partial class AccountEditor
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
            lblUserIdHeader = new Label();
            txtDisplayName = new TextBox();
            lblDisplayNameHeader = new Label();
            chkBanned = new CheckBox();
            chkModerator = new CheckBox();
            bannedUntilDatePicker = new DateTimePicker();
            chkDisableAFKTimeout = new CheckBox();
            btnClearAccountLock = new Button();
            txtUserId = new TextBox();
            SuspendLayout();
            // 
            // lblUserIdHeader
            // 
            lblUserIdHeader.AutoSize = true;
            lblUserIdHeader.Location = new Point(7, 6);
            lblUserIdHeader.Name = "lblUserIdHeader";
            lblUserIdHeader.Size = new Size(83, 15);
            lblUserIdHeader.TabIndex = 4;
            lblUserIdHeader.Text = "User Identifier:";
            // 
            // txtDisplayName
            // 
            txtDisplayName.Location = new Point(141, 32);
            txtDisplayName.Name = "txtDisplayName";
            txtDisplayName.Size = new Size(385, 23);
            txtDisplayName.TabIndex = 7;
            txtDisplayName.TextChanged += onAnyValueChanged;
            // 
            // lblDisplayNameHeader
            // 
            lblDisplayNameHeader.AutoSize = true;
            lblDisplayNameHeader.Location = new Point(7, 35);
            lblDisplayNameHeader.Name = "lblDisplayNameHeader";
            lblDisplayNameHeader.Size = new Size(83, 15);
            lblDisplayNameHeader.TabIndex = 8;
            lblDisplayNameHeader.Text = "Display Name:";
            // 
            // chkBanned
            // 
            chkBanned.AutoSize = true;
            chkBanned.Location = new Point(7, 63);
            chkBanned.Name = "chkBanned";
            chkBanned.Size = new Size(128, 19);
            chkBanned.TabIndex = 9;
            chkBanned.Text = "Banned until (UTC):";
            chkBanned.UseVisualStyleBackColor = true;
            chkBanned.CheckedChanged += onAnyValueChanged;
            // 
            // chkModerator
            // 
            chkModerator.AutoSize = true;
            chkModerator.Location = new Point(7, 90);
            chkModerator.Name = "chkModerator";
            chkModerator.Size = new Size(135, 19);
            chkModerator.TabIndex = 10;
            chkModerator.Text = "Moderator privileges";
            chkModerator.UseVisualStyleBackColor = true;
            chkModerator.CheckedChanged += onAnyValueChanged;
            // 
            // bannedUntilDatePicker
            // 
            bannedUntilDatePicker.CustomFormat = "MM/dd/yyyy @ hh:mm:ss tt";
            bannedUntilDatePicker.Format = DateTimePickerFormat.Custom;
            bannedUntilDatePicker.Location = new Point(141, 61);
            bannedUntilDatePicker.Name = "bannedUntilDatePicker";
            bannedUntilDatePicker.Size = new Size(385, 23);
            bannedUntilDatePicker.TabIndex = 11;
            bannedUntilDatePicker.ValueChanged += onAnyValueChanged;
            // 
            // chkDisableAFKTimeout
            // 
            chkDisableAFKTimeout.AutoSize = true;
            chkDisableAFKTimeout.Location = new Point(7, 115);
            chkDisableAFKTimeout.Name = "chkDisableAFKTimeout";
            chkDisableAFKTimeout.Size = new Size(192, 19);
            chkDisableAFKTimeout.TabIndex = 12;
            chkDisableAFKTimeout.Text = "Disable AFK (inactivity) timeout";
            chkDisableAFKTimeout.UseVisualStyleBackColor = true;
            chkDisableAFKTimeout.CheckedChanged += onAnyValueChanged;
            // 
            // btnClearAccountLock
            // 
            btnClearAccountLock.Location = new Point(6, 140);
            btnClearAccountLock.Name = "btnClearAccountLock";
            btnClearAccountLock.Size = new Size(520, 23);
            btnClearAccountLock.TabIndex = 13;
            btnClearAccountLock.Text = "Clear password/account lock";
            btnClearAccountLock.UseVisualStyleBackColor = true;
            btnClearAccountLock.Click += btnClearAccountLock_Click;
            // 
            // txtUserId
            // 
            txtUserId.Location = new Point(141, 3);
            txtUserId.Name = "txtUserId";
            txtUserId.ReadOnly = true;
            txtUserId.Size = new Size(385, 23);
            txtUserId.TabIndex = 14;
            // 
            // AccountEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(txtUserId);
            Controls.Add(btnClearAccountLock);
            Controls.Add(chkDisableAFKTimeout);
            Controls.Add(bannedUntilDatePicker);
            Controls.Add(chkModerator);
            Controls.Add(chkBanned);
            Controls.Add(lblDisplayNameHeader);
            Controls.Add(txtDisplayName);
            Controls.Add(lblUserIdHeader);
            Name = "AccountEditor";
            Size = new Size(529, 179);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblUserIdHeader;
        private TextBox txtDisplayName;
        private Label lblDisplayNameHeader;
        private CheckBox chkBanned;
        private CheckBox chkModerator;
        private DateTimePicker bannedUntilDatePicker;
        private CheckBox chkDisableAFKTimeout;
        private Button btnClearAccountLock;
        private TextBox txtUserId;
    }
}
