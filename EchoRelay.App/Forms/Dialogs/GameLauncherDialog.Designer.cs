namespace EchoRelay.App.Forms.Dialogs
{
    partial class GameLauncherDialog
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
            cmbGametype = new ComboBox();
            lblGametype = new Label();
            cmbLevel = new ComboBox();
            lblLevel = new Label();
            chkSpectatorStream = new CheckBox();
            chkModerator = new CheckBox();
            chkNoOVR = new CheckBox();
            btnLaunchGame = new Button();
            radioRoleClient = new RadioButton();
            radioRoleServer = new RadioButton();
            radioRoleOffline = new RadioButton();
            numThrottleTimestep = new NumericUpDown();
            chkThrottleTimestep = new CheckBox();
            radioViewVR = new RadioButton();
            radioViewHeadless = new RadioButton();
            radioViewWindowed = new RadioButton();
            groupRole = new GroupBox();
            groupViewMode = new GroupBox();
            groupBox1 = new GroupBox();
            groupAdditional = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)numThrottleTimestep).BeginInit();
            groupRole.SuspendLayout();
            groupViewMode.SuspendLayout();
            groupBox1.SuspendLayout();
            groupAdditional.SuspendLayout();
            SuspendLayout();
            // 
            // cmbGametype
            // 
            cmbGametype.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGametype.Enabled = false;
            cmbGametype.FormattingEnabled = true;
            cmbGametype.Location = new Point(76, 22);
            cmbGametype.Name = "cmbGametype";
            cmbGametype.Size = new Size(294, 23);
            cmbGametype.TabIndex = 3;
            // 
            // lblGametype
            // 
            lblGametype.AutoSize = true;
            lblGametype.Location = new Point(6, 25);
            lblGametype.Name = "lblGametype";
            lblGametype.Size = new Size(64, 15);
            lblGametype.TabIndex = 2;
            lblGametype.Text = "Gametype:";
            // 
            // cmbLevel
            // 
            cmbLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLevel.Enabled = false;
            cmbLevel.FormattingEnabled = true;
            cmbLevel.Location = new Point(76, 51);
            cmbLevel.Name = "cmbLevel";
            cmbLevel.Size = new Size(294, 23);
            cmbLevel.TabIndex = 5;
            // 
            // lblLevel
            // 
            lblLevel.AutoSize = true;
            lblLevel.Location = new Point(6, 54);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(37, 15);
            lblLevel.TabIndex = 4;
            lblLevel.Text = "Level:";
            // 
            // chkSpectatorStream
            // 
            chkSpectatorStream.AutoSize = true;
            chkSpectatorStream.Location = new Point(6, 47);
            chkSpectatorStream.Name = "chkSpectatorStream";
            chkSpectatorStream.Size = new Size(76, 19);
            chkSpectatorStream.TabIndex = 6;
            chkSpectatorStream.Text = "Spectator";
            chkSpectatorStream.UseVisualStyleBackColor = true;
            // 
            // chkModerator
            // 
            chkModerator.AutoSize = true;
            chkModerator.Location = new Point(6, 72);
            chkModerator.Name = "chkModerator";
            chkModerator.Size = new Size(82, 19);
            chkModerator.TabIndex = 7;
            chkModerator.Text = "Moderator";
            chkModerator.UseVisualStyleBackColor = true;
            // 
            // chkNoOVR
            // 
            chkNoOVR.AutoSize = true;
            chkNoOVR.Checked = true;
            chkNoOVR.CheckState = CheckState.Checked;
            chkNoOVR.Location = new Point(6, 22);
            chkNoOVR.Name = "chkNoOVR";
            chkNoOVR.Size = new Size(147, 19);
            chkNoOVR.TabIndex = 8;
            chkNoOVR.Text = "No OVR (demo profile)";
            chkNoOVR.UseVisualStyleBackColor = true;
            // 
            // btnLaunchGame
            // 
            btnLaunchGame.Location = new Point(12, 361);
            btnLaunchGame.Name = "btnLaunchGame";
            btnLaunchGame.Size = new Size(376, 23);
            btnLaunchGame.TabIndex = 9;
            btnLaunchGame.Text = "Launch Game";
            btnLaunchGame.UseVisualStyleBackColor = true;
            btnLaunchGame.Click += btnLaunchGame_Click;
            // 
            // radioRoleClient
            // 
            radioRoleClient.AutoSize = true;
            radioRoleClient.Location = new Point(107, 22);
            radioRoleClient.Name = "radioRoleClient";
            radioRoleClient.Size = new Size(56, 19);
            radioRoleClient.TabIndex = 10;
            radioRoleClient.Text = "Client";
            radioRoleClient.UseVisualStyleBackColor = true;
            // 
            // radioRoleServer
            // 
            radioRoleServer.AutoSize = true;
            radioRoleServer.Checked = true;
            radioRoleServer.Location = new Point(6, 22);
            radioRoleServer.Name = "radioRoleServer";
            radioRoleServer.Size = new Size(57, 19);
            radioRoleServer.TabIndex = 11;
            radioRoleServer.TabStop = true;
            radioRoleServer.Text = "Server";
            radioRoleServer.UseVisualStyleBackColor = true;
            radioRoleServer.CheckedChanged += radioRoleServer_CheckedChanged;
            // 
            // radioRoleOffline
            // 
            radioRoleOffline.AutoSize = true;
            radioRoleOffline.Location = new Point(204, 22);
            radioRoleOffline.Name = "radioRoleOffline";
            radioRoleOffline.Size = new Size(61, 19);
            radioRoleOffline.TabIndex = 12;
            radioRoleOffline.Text = "Offline";
            radioRoleOffline.UseVisualStyleBackColor = true;
            // 
            // numThrottleTimestep
            // 
            numThrottleTimestep.Enabled = false;
            numThrottleTimestep.Location = new Point(176, 96);
            numThrottleTimestep.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numThrottleTimestep.Name = "numThrottleTimestep";
            numThrottleTimestep.Size = new Size(194, 23);
            numThrottleTimestep.TabIndex = 8;
            numThrottleTimestep.Value = new decimal(new int[] { 120, 0, 0, 0 });
            // 
            // chkThrottleTimestep
            // 
            chkThrottleTimestep.AutoSize = true;
            chkThrottleTimestep.Enabled = false;
            chkThrottleTimestep.Location = new Point(6, 97);
            chkThrottleTimestep.Name = "chkThrottleTimestep";
            chkThrottleTimestep.Size = new Size(165, 19);
            chkThrottleTimestep.TabIndex = 7;
            chkThrottleTimestep.Text = "Headless timestep (tick/s):";
            chkThrottleTimestep.UseVisualStyleBackColor = true;
            chkThrottleTimestep.CheckedChanged += chkThrottleTimestep_CheckedChanged;
            // 
            // radioViewVR
            // 
            radioViewVR.AutoSize = true;
            radioViewVR.Enabled = false;
            radioViewVR.Location = new Point(204, 22);
            radioViewVR.Name = "radioViewVR";
            radioViewVR.Size = new Size(39, 19);
            radioViewVR.TabIndex = 16;
            radioViewVR.Text = "VR";
            radioViewVR.UseVisualStyleBackColor = true;
            // 
            // radioViewHeadless
            // 
            radioViewHeadless.AutoSize = true;
            radioViewHeadless.Location = new Point(107, 22);
            radioViewHeadless.Name = "radioViewHeadless";
            radioViewHeadless.Size = new Size(72, 19);
            radioViewHeadless.TabIndex = 15;
            radioViewHeadless.Text = "Headless";
            radioViewHeadless.UseVisualStyleBackColor = true;
            radioViewHeadless.CheckedChanged += radioViewHeadless_CheckedChanged;
            // 
            // radioViewWindowed
            // 
            radioViewWindowed.AutoSize = true;
            radioViewWindowed.Checked = true;
            radioViewWindowed.Location = new Point(6, 22);
            radioViewWindowed.Name = "radioViewWindowed";
            radioViewWindowed.Size = new Size(82, 19);
            radioViewWindowed.TabIndex = 14;
            radioViewWindowed.TabStop = true;
            radioViewWindowed.Text = "Windowed";
            radioViewWindowed.UseVisualStyleBackColor = true;
            // 
            // groupRole
            // 
            groupRole.Controls.Add(radioRoleClient);
            groupRole.Controls.Add(radioRoleServer);
            groupRole.Controls.Add(radioRoleOffline);
            groupRole.Location = new Point(12, 12);
            groupRole.Name = "groupRole";
            groupRole.Size = new Size(375, 51);
            groupRole.TabIndex = 17;
            groupRole.TabStop = false;
            groupRole.Text = "Role";
            // 
            // groupViewMode
            // 
            groupViewMode.Controls.Add(radioViewWindowed);
            groupViewMode.Controls.Add(radioViewVR);
            groupViewMode.Controls.Add(radioViewHeadless);
            groupViewMode.Location = new Point(12, 69);
            groupViewMode.Name = "groupViewMode";
            groupViewMode.Size = new Size(375, 51);
            groupViewMode.TabIndex = 18;
            groupViewMode.TabStop = false;
            groupViewMode.Text = "View Mode";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblGametype);
            groupBox1.Controls.Add(cmbGametype);
            groupBox1.Controls.Add(lblLevel);
            groupBox1.Controls.Add(cmbLevel);
            groupBox1.Location = new Point(12, 130);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(376, 90);
            groupBox1.TabIndex = 19;
            groupBox1.TabStop = false;
            groupBox1.Text = "Match settings";
            // 
            // groupAdditional
            // 
            groupAdditional.Controls.Add(chkNoOVR);
            groupAdditional.Controls.Add(chkModerator);
            groupAdditional.Controls.Add(chkSpectatorStream);
            groupAdditional.Controls.Add(chkThrottleTimestep);
            groupAdditional.Controls.Add(numThrottleTimestep);
            groupAdditional.Location = new Point(12, 226);
            groupAdditional.Name = "groupAdditional";
            groupAdditional.Size = new Size(376, 129);
            groupAdditional.TabIndex = 20;
            groupAdditional.TabStop = false;
            groupAdditional.Text = "Additional settings";
            // 
            // GameLauncherDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(399, 393);
            Controls.Add(groupAdditional);
            Controls.Add(groupBox1);
            Controls.Add(groupViewMode);
            Controls.Add(groupRole);
            Controls.Add(btnLaunchGame);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "GameLauncherDialog";
            Text = "Game Launcher";
            ((System.ComponentModel.ISupportInitialize)numThrottleTimestep).EndInit();
            groupRole.ResumeLayout(false);
            groupRole.PerformLayout();
            groupViewMode.ResumeLayout(false);
            groupViewMode.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupAdditional.ResumeLayout(false);
            groupAdditional.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private ComboBox cmbGametype;
        private Label lblGametype;
        private ComboBox cmbLevel;
        private Label lblLevel;
        private CheckBox chkSpectatorStream;
        private CheckBox chkModerator;
        private CheckBox chkNoOVR;
        private Button btnLaunchGame;
        private RadioButton radioRoleClient;
        private RadioButton radioRoleServer;
        private RadioButton radioRoleOffline;
        private CheckBox chkThrottleTimestep;
        private NumericUpDown numThrottleTimestep;
        private RadioButton radioViewVR;
        private RadioButton radioViewHeadless;
        private RadioButton radioViewWindowed;
        private GroupBox groupRole;
        private GroupBox groupViewMode;
        private GroupBox groupBox1;
        private GroupBox groupAdditional;
    }
}