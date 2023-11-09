namespace EchoRelay.App.Forms.Dialogs
{
    partial class SettingsDialog
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
            lblDatabaseType = new Label();
            radioDbTypeFilesystem = new RadioButton();
            radioButton1 = new RadioButton();
            groupBoxGame = new GroupBox();
            btnOpenGameFolder = new Button();
            txtExecutablePath = new TextBox();
            label1 = new Label();
            groupBoxServer = new GroupBox();
            chkStartServerOnStartup = new CheckBox();
            numericTCPPort = new NumericUpDown();
            label2 = new Label();
            btnOpenDbFolder = new Button();
            txtDbFolder = new TextBox();
            lblDbFolder = new Label();
            btnSaveSettings = new Button();
            groupBoxMatching = new GroupBox();
            chkForceIntoAnySession = new CheckBox();
            chkPopulationOverPing = new CheckBox();
            groupBox1 = new GroupBox();
            numValidateGameServersTimeout = new NumericUpDown();
            chkValidateGameServers = new CheckBox();
            btnRegenerateAPIKey = new Button();
            txtServerDBApiKey = new TextBox();
            chkUseServerDBApiKeys = new CheckBox();
            groupBoxGame.SuspendLayout();
            groupBoxServer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericTCPPort).BeginInit();
            groupBoxMatching.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numValidateGameServersTimeout).BeginInit();
            SuspendLayout();
            // 
            // lblDatabaseType
            // 
            lblDatabaseType.AutoSize = true;
            lblDatabaseType.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblDatabaseType.Location = new Point(6, 52);
            lblDatabaseType.Name = "lblDatabaseType";
            lblDatabaseType.Size = new Size(85, 15);
            lblDatabaseType.TabIndex = 0;
            lblDatabaseType.Text = "Database Type:";
            // 
            // radioDbTypeFilesystem
            // 
            radioDbTypeFilesystem.AutoSize = true;
            radioDbTypeFilesystem.Checked = true;
            radioDbTypeFilesystem.Location = new Point(106, 50);
            radioDbTypeFilesystem.Name = "radioDbTypeFilesystem";
            radioDbTypeFilesystem.Size = new Size(80, 19);
            radioDbTypeFilesystem.TabIndex = 2;
            radioDbTypeFilesystem.TabStop = true;
            radioDbTypeFilesystem.Text = "Filesystem";
            radioDbTypeFilesystem.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Enabled = false;
            radioButton1.Location = new Point(192, 50);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(79, 19);
            radioButton1.TabIndex = 3;
            radioButton1.Text = "MongoDB";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBoxGame
            // 
            groupBoxGame.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxGame.Controls.Add(btnOpenGameFolder);
            groupBoxGame.Controls.Add(txtExecutablePath);
            groupBoxGame.Controls.Add(label1);
            groupBoxGame.Location = new Point(12, 12);
            groupBoxGame.Name = "groupBoxGame";
            groupBoxGame.Size = new Size(500, 61);
            groupBoxGame.TabIndex = 4;
            groupBoxGame.TabStop = false;
            groupBoxGame.Text = "Game Settings";
            // 
            // btnOpenGameFolder
            // 
            btnOpenGameFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenGameFolder.Location = new Point(464, 22);
            btnOpenGameFolder.Name = "btnOpenGameFolder";
            btnOpenGameFolder.Size = new Size(30, 23);
            btnOpenGameFolder.TabIndex = 6;
            btnOpenGameFolder.Text = "...";
            btnOpenGameFolder.UseVisualStyleBackColor = true;
            btnOpenGameFolder.Click += btnOpenGameFolder_Click;
            // 
            // txtExecutablePath
            // 
            txtExecutablePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtExecutablePath.Location = new Point(106, 22);
            txtExecutablePath.Name = "txtExecutablePath";
            txtExecutablePath.ReadOnly = true;
            txtExecutablePath.Size = new Size(352, 23);
            txtExecutablePath.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 25);
            label1.Name = "label1";
            label1.Size = new Size(67, 15);
            label1.TabIndex = 7;
            label1.Text = "Executable:";
            // 
            // groupBoxServer
            // 
            groupBoxServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxServer.Controls.Add(chkStartServerOnStartup);
            groupBoxServer.Controls.Add(numericTCPPort);
            groupBoxServer.Controls.Add(label2);
            groupBoxServer.Controls.Add(btnOpenDbFolder);
            groupBoxServer.Controls.Add(txtDbFolder);
            groupBoxServer.Controls.Add(lblDbFolder);
            groupBoxServer.Controls.Add(radioDbTypeFilesystem);
            groupBoxServer.Controls.Add(lblDatabaseType);
            groupBoxServer.Controls.Add(radioButton1);
            groupBoxServer.Location = new Point(12, 79);
            groupBoxServer.Name = "groupBoxServer";
            groupBoxServer.Size = new Size(500, 137);
            groupBoxServer.TabIndex = 5;
            groupBoxServer.TabStop = false;
            groupBoxServer.Text = "Server Settings";
            // 
            // chkStartServerOnStartup
            // 
            chkStartServerOnStartup.AutoSize = true;
            chkStartServerOnStartup.Checked = true;
            chkStartServerOnStartup.CheckState = CheckState.Checked;
            chkStartServerOnStartup.Location = new Point(6, 108);
            chkStartServerOnStartup.Name = "chkStartServerOnStartup";
            chkStartServerOnStartup.Size = new Size(141, 19);
            chkStartServerOnStartup.TabIndex = 8;
            chkStartServerOnStartup.Text = "Start server on startup";
            chkStartServerOnStartup.UseVisualStyleBackColor = true;
            // 
            // numericTCPPort
            // 
            numericTCPPort.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numericTCPPort.Location = new Point(106, 21);
            numericTCPPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericTCPPort.Name = "numericTCPPort";
            numericTCPPort.Size = new Size(352, 23);
            numericTCPPort.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 23);
            label2.Name = "label2";
            label2.Size = new Size(55, 15);
            label2.TabIndex = 6;
            label2.Text = "TCP Port:";
            // 
            // btnOpenDbFolder
            // 
            btnOpenDbFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenDbFolder.Location = new Point(464, 79);
            btnOpenDbFolder.Name = "btnOpenDbFolder";
            btnOpenDbFolder.Size = new Size(30, 23);
            btnOpenDbFolder.TabIndex = 0;
            btnOpenDbFolder.Text = "...";
            btnOpenDbFolder.UseVisualStyleBackColor = true;
            btnOpenDbFolder.Click += btnOpenDbFolder_Click;
            // 
            // txtDbFolder
            // 
            txtDbFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDbFolder.Location = new Point(106, 79);
            txtDbFolder.Name = "txtDbFolder";
            txtDbFolder.ReadOnly = true;
            txtDbFolder.Size = new Size(352, 23);
            txtDbFolder.TabIndex = 5;
            // 
            // lblDbFolder
            // 
            lblDbFolder.AutoSize = true;
            lblDbFolder.Location = new Point(6, 81);
            lblDbFolder.Name = "lblDbFolder";
            lblDbFolder.Size = new Size(94, 15);
            lblDbFolder.TabIndex = 4;
            lblDbFolder.Text = "Database Folder:";
            // 
            // btnSaveSettings
            // 
            btnSaveSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSaveSettings.Location = new Point(12, 392);
            btnSaveSettings.Name = "btnSaveSettings";
            btnSaveSettings.Size = new Size(500, 23);
            btnSaveSettings.TabIndex = 6;
            btnSaveSettings.Text = "Save Settings";
            btnSaveSettings.UseVisualStyleBackColor = true;
            btnSaveSettings.Click += btnSaveSettings_Click;
            // 
            // groupBoxMatching
            // 
            groupBoxMatching.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxMatching.Controls.Add(chkForceIntoAnySession);
            groupBoxMatching.Controls.Add(chkPopulationOverPing);
            groupBoxMatching.Location = new Point(12, 309);
            groupBoxMatching.Name = "groupBoxMatching";
            groupBoxMatching.Size = new Size(500, 79);
            groupBoxMatching.TabIndex = 7;
            groupBoxMatching.TabStop = false;
            groupBoxMatching.Text = "Matching Settings";
            // 
            // chkForceIntoAnySession
            // 
            chkForceIntoAnySession.AutoSize = true;
            chkForceIntoAnySession.Checked = true;
            chkForceIntoAnySession.CheckState = CheckState.Checked;
            chkForceIntoAnySession.Location = new Point(6, 47);
            chkForceIntoAnySession.Name = "chkForceIntoAnySession";
            chkForceIntoAnySession.Size = new Size(432, 19);
            chkForceIntoAnySession.TabIndex = 1;
            chkForceIntoAnySession.Text = "Force into most populated session on matching failure / lack of game servers";
            chkForceIntoAnySession.UseVisualStyleBackColor = true;
            // 
            // chkPopulationOverPing
            // 
            chkPopulationOverPing.AutoSize = true;
            chkPopulationOverPing.Checked = true;
            chkPopulationOverPing.CheckState = CheckState.Checked;
            chkPopulationOverPing.Location = new Point(6, 22);
            chkPopulationOverPing.Name = "chkPopulationOverPing";
            chkPopulationOverPing.Size = new Size(275, 19);
            chkPopulationOverPing.TabIndex = 0;
            chkPopulationOverPing.Text = "Prioritize population over ping (recommended)";
            chkPopulationOverPing.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(numValidateGameServersTimeout);
            groupBox1.Controls.Add(chkValidateGameServers);
            groupBox1.Controls.Add(btnRegenerateAPIKey);
            groupBox1.Controls.Add(txtServerDBApiKey);
            groupBox1.Controls.Add(chkUseServerDBApiKeys);
            groupBox1.Location = new Point(12, 222);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(500, 81);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "ServerDB Settings";
            // 
            // numValidateGameServersTimeout
            // 
            numValidateGameServersTimeout.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numValidateGameServersTimeout.Location = new Point(334, 48);
            numValidateGameServersTimeout.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            numValidateGameServersTimeout.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numValidateGameServersTimeout.Name = "numValidateGameServersTimeout";
            numValidateGameServersTimeout.Size = new Size(160, 23);
            numValidateGameServersTimeout.TabIndex = 12;
            numValidateGameServersTimeout.Value = new decimal(new int[] { 3000, 0, 0, 0 });
            // 
            // chkValidateGameServers
            // 
            chkValidateGameServers.AutoSize = true;
            chkValidateGameServers.Checked = true;
            chkValidateGameServers.CheckState = CheckState.Checked;
            chkValidateGameServers.Location = new Point(6, 49);
            chkValidateGameServers.Name = "chkValidateGameServers";
            chkValidateGameServers.Size = new Size(329, 19);
            chkValidateGameServers.TabIndex = 11;
            chkValidateGameServers.Text = "Validate game servers with ping request (w/ timeout, ms):\r\n";
            chkValidateGameServers.UseVisualStyleBackColor = true;
            chkValidateGameServers.CheckedChanged += chkValidateGameServers_CheckedChanged;
            // 
            // btnRegenerateAPIKey
            // 
            btnRegenerateAPIKey.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRegenerateAPIKey.BackgroundImageLayout = ImageLayout.Stretch;
            btnRegenerateAPIKey.Location = new Point(428, 20);
            btnRegenerateAPIKey.Name = "btnRegenerateAPIKey";
            btnRegenerateAPIKey.Size = new Size(66, 23);
            btnRegenerateAPIKey.TabIndex = 10;
            btnRegenerateAPIKey.Text = "Generate";
            btnRegenerateAPIKey.UseVisualStyleBackColor = true;
            btnRegenerateAPIKey.Click += btnRegenerateAPIKey_Click;
            // 
            // txtServerDBApiKey
            // 
            txtServerDBApiKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtServerDBApiKey.Location = new Point(182, 20);
            txtServerDBApiKey.Name = "txtServerDBApiKey";
            txtServerDBApiKey.Size = new Size(240, 23);
            txtServerDBApiKey.TabIndex = 9;
            // 
            // chkUseServerDBApiKeys
            // 
            chkUseServerDBApiKeys.AutoSize = true;
            chkUseServerDBApiKeys.Checked = true;
            chkUseServerDBApiKeys.CheckState = CheckState.Checked;
            chkUseServerDBApiKeys.Location = new Point(6, 22);
            chkUseServerDBApiKeys.Name = "chkUseServerDBApiKeys";
            chkUseServerDBApiKeys.Size = new Size(170, 19);
            chkUseServerDBApiKeys.TabIndex = 0;
            chkUseServerDBApiKeys.Text = "Use API key authentication:";
            chkUseServerDBApiKeys.UseVisualStyleBackColor = true;
            chkUseServerDBApiKeys.CheckedChanged += chkUseServerDBApiKeys_CheckedChanged;
            // 
            // SettingsDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 423);
            Controls.Add(groupBox1);
            Controls.Add(groupBoxMatching);
            Controls.Add(btnSaveSettings);
            Controls.Add(groupBoxServer);
            Controls.Add(groupBoxGame);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "SettingsDialog";
            Text = "Application Settings";
            groupBoxGame.ResumeLayout(false);
            groupBoxGame.PerformLayout();
            groupBoxServer.ResumeLayout(false);
            groupBoxServer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericTCPPort).EndInit();
            groupBoxMatching.ResumeLayout(false);
            groupBoxMatching.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numValidateGameServersTimeout).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label lblDatabaseType;
        private RadioButton radioDbTypeFilesystem;
        private RadioButton radioButton1;
        private GroupBox groupBoxGame;
        private Button btnOpenDbFolder;
        private GroupBox groupBoxServer;
        private TextBox txtDbFolder;
        private Label lblDbFolder;
        private Button btnOpenGameFolder;
        private TextBox txtExecutablePath;
        private Label label1;
        private Button btnSaveSettings;
        private Label label2;
        private NumericUpDown numericTCPPort;
        private GroupBox groupBoxMatching;
        private CheckBox chkForceIntoAnySession;
        private CheckBox chkPopulationOverPing;
        private CheckBox chkStartServerOnStartup;
        private GroupBox groupBox1;
        private TextBox txtServerDBApiKey;
        private CheckBox chkUseServerDBApiKeys;
        private Button btnRegenerateAPIKey;
        private CheckBox chkValidateGameServers;
        private NumericUpDown numValidateGameServersTimeout;
    }
}