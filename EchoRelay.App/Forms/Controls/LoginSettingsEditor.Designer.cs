namespace EchoRelay.App.Forms.Controls
{
    partial class LoginSettingsEditor
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
            chkIAPUnlocked = new CheckBox();
            chkRemoteLogSocial = new CheckBox();
            chkRemoteLogWarnings = new CheckBox();
            chkRemoteLogErrors = new CheckBox();
            chkRemoteLogRichPresence = new CheckBox();
            chkRemoteLogMetrics = new CheckBox();
            lblEnvironmentHeader = new Label();
            txtEnvironment = new TextBox();
            SuspendLayout();
            // 
            // chkIAPUnlocked
            // 
            chkIAPUnlocked.AutoSize = true;
            chkIAPUnlocked.Location = new Point(128, 39);
            chkIAPUnlocked.Name = "chkIAPUnlocked";
            chkIAPUnlocked.Size = new Size(198, 19);
            chkIAPUnlocked.TabIndex = 0;
            chkIAPUnlocked.Text = "In-app purchases (IAP) unlocked";
            chkIAPUnlocked.UseVisualStyleBackColor = true;
            chkIAPUnlocked.CheckedChanged += onAnyValueChanged;
            // 
            // chkRemoteLogSocial
            // 
            chkRemoteLogSocial.AutoSize = true;
            chkRemoteLogSocial.Location = new Point(128, 64);
            chkRemoteLogSocial.Name = "chkRemoteLogSocial";
            chkRemoteLogSocial.Size = new Size(127, 19);
            chkRemoteLogSocial.TabIndex = 1;
            chkRemoteLogSocial.Text = "Remote Log: Social";
            chkRemoteLogSocial.UseVisualStyleBackColor = true;
            chkRemoteLogSocial.CheckedChanged += onAnyValueChanged;
            // 
            // chkRemoteLogWarnings
            // 
            chkRemoteLogWarnings.AutoSize = true;
            chkRemoteLogWarnings.Location = new Point(128, 89);
            chkRemoteLogWarnings.Name = "chkRemoteLogWarnings";
            chkRemoteLogWarnings.Size = new Size(146, 19);
            chkRemoteLogWarnings.TabIndex = 2;
            chkRemoteLogWarnings.Text = "Remote Log: Warnings";
            chkRemoteLogWarnings.UseVisualStyleBackColor = true;
            chkRemoteLogWarnings.CheckedChanged += onAnyValueChanged;
            // 
            // chkRemoteLogErrors
            // 
            chkRemoteLogErrors.AutoSize = true;
            chkRemoteLogErrors.Location = new Point(128, 114);
            chkRemoteLogErrors.Name = "chkRemoteLogErrors";
            chkRemoteLogErrors.Size = new Size(126, 19);
            chkRemoteLogErrors.TabIndex = 3;
            chkRemoteLogErrors.Text = "Remote Log: Errors";
            chkRemoteLogErrors.UseVisualStyleBackColor = true;
            chkRemoteLogErrors.CheckedChanged += onAnyValueChanged;
            // 
            // chkRemoteLogRichPresence
            // 
            chkRemoteLogRichPresence.AutoSize = true;
            chkRemoteLogRichPresence.Location = new Point(128, 139);
            chkRemoteLogRichPresence.Name = "chkRemoteLogRichPresence";
            chkRemoteLogRichPresence.Size = new Size(169, 19);
            chkRemoteLogRichPresence.TabIndex = 4;
            chkRemoteLogRichPresence.Text = "Remote Log: Rich Presence";
            chkRemoteLogRichPresence.UseVisualStyleBackColor = true;
            chkRemoteLogRichPresence.CheckedChanged += onAnyValueChanged;
            // 
            // chkRemoteLogMetrics
            // 
            chkRemoteLogMetrics.AutoSize = true;
            chkRemoteLogMetrics.Location = new Point(128, 164);
            chkRemoteLogMetrics.Name = "chkRemoteLogMetrics";
            chkRemoteLogMetrics.Size = new Size(135, 19);
            chkRemoteLogMetrics.TabIndex = 5;
            chkRemoteLogMetrics.Text = "Remote Log: Metrics";
            chkRemoteLogMetrics.UseVisualStyleBackColor = true;
            chkRemoteLogMetrics.CheckedChanged += onAnyValueChanged;
            // 
            // lblEnvironmentHeader
            // 
            lblEnvironmentHeader.AutoSize = true;
            lblEnvironmentHeader.Location = new Point(3, 13);
            lblEnvironmentHeader.Name = "lblEnvironmentHeader";
            lblEnvironmentHeader.Size = new Size(119, 15);
            lblEnvironmentHeader.TabIndex = 6;
            lblEnvironmentHeader.Text = "Environment (name):";
            // 
            // txtEnvironment
            // 
            txtEnvironment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEnvironment.Location = new Point(128, 10);
            txtEnvironment.Name = "txtEnvironment";
            txtEnvironment.Size = new Size(375, 23);
            txtEnvironment.TabIndex = 7;
            txtEnvironment.TextChanged += onAnyValueChanged;
            // 
            // LoginSettingsEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(txtEnvironment);
            Controls.Add(lblEnvironmentHeader);
            Controls.Add(chkRemoteLogMetrics);
            Controls.Add(chkRemoteLogRichPresence);
            Controls.Add(chkRemoteLogErrors);
            Controls.Add(chkRemoteLogWarnings);
            Controls.Add(chkRemoteLogSocial);
            Controls.Add(chkIAPUnlocked);
            Name = "LoginSettingsEditor";
            Size = new Size(506, 193);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkIAPUnlocked;
        private CheckBox chkRemoteLogSocial;
        private CheckBox chkRemoteLogWarnings;
        private CheckBox chkRemoteLogErrors;
        private CheckBox chkRemoteLogRichPresence;
        private CheckBox chkRemoteLogMetrics;
        private Label lblEnvironmentHeader;
        private TextBox txtEnvironment;
    }
}
