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
            label1 = new Label();
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
            chkWindowed = new CheckBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 15);
            label1.Name = "label1";
            label1.Size = new Size(33, 15);
            label1.TabIndex = 0;
            label1.Text = "Role:";
            // 
            // cmbGametype
            // 
            cmbGametype.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGametype.Enabled = false;
            cmbGametype.FormattingEnabled = true;
            cmbGametype.Location = new Point(83, 41);
            cmbGametype.Name = "cmbGametype";
            cmbGametype.Size = new Size(305, 23);
            cmbGametype.TabIndex = 3;
            // 
            // lblGametype
            // 
            lblGametype.AutoSize = true;
            lblGametype.Location = new Point(13, 44);
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
            cmbLevel.Location = new Point(83, 70);
            cmbLevel.Name = "cmbLevel";
            cmbLevel.Size = new Size(305, 23);
            cmbLevel.TabIndex = 5;
            // 
            // lblLevel
            // 
            lblLevel.AutoSize = true;
            lblLevel.Location = new Point(13, 73);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(37, 15);
            lblLevel.TabIndex = 4;
            lblLevel.Text = "Level:";
            // 
            // chkSpectatorStream
            // 
            chkSpectatorStream.AutoSize = true;
            chkSpectatorStream.Location = new Point(12, 124);
            chkSpectatorStream.Name = "chkSpectatorStream";
            chkSpectatorStream.Size = new Size(76, 19);
            chkSpectatorStream.TabIndex = 6;
            chkSpectatorStream.Text = "Spectator";
            chkSpectatorStream.UseVisualStyleBackColor = true;
            // 
            // chkModerator
            // 
            chkModerator.AutoSize = true;
            chkModerator.Location = new Point(12, 149);
            chkModerator.Name = "chkModerator";
            chkModerator.Size = new Size(82, 19);
            chkModerator.TabIndex = 7;
            chkModerator.Text = "Moderator";
            chkModerator.UseVisualStyleBackColor = true;
            // 
            // chkNoOVR
            // 
            chkNoOVR.AutoSize = true;
            chkNoOVR.Location = new Point(12, 174);
            chkNoOVR.Name = "chkNoOVR";
            chkNoOVR.Size = new Size(147, 19);
            chkNoOVR.TabIndex = 8;
            chkNoOVR.Text = "No OVR (demo profile)";
            chkNoOVR.UseVisualStyleBackColor = true;
            // 
            // btnLaunchGame
            // 
            btnLaunchGame.Location = new Point(12, 199);
            btnLaunchGame.Name = "btnLaunchGame";
            btnLaunchGame.Size = new Size(375, 23);
            btnLaunchGame.TabIndex = 9;
            btnLaunchGame.Text = "Launch Game";
            btnLaunchGame.UseVisualStyleBackColor = true;
            btnLaunchGame.Click += btnLaunchGame_Click;
            // 
            // radioRoleClient
            // 
            radioRoleClient.AutoSize = true;
            radioRoleClient.Checked = true;
            radioRoleClient.Location = new Point(83, 13);
            radioRoleClient.Name = "radioRoleClient";
            radioRoleClient.Size = new Size(56, 19);
            radioRoleClient.TabIndex = 10;
            radioRoleClient.TabStop = true;
            radioRoleClient.Text = "Client";
            radioRoleClient.UseVisualStyleBackColor = true;
            // 
            // radioRoleServer
            // 
            radioRoleServer.AutoSize = true;
            radioRoleServer.Location = new Point(145, 13);
            radioRoleServer.Name = "radioRoleServer";
            radioRoleServer.Size = new Size(57, 19);
            radioRoleServer.TabIndex = 11;
            radioRoleServer.Text = "Server";
            radioRoleServer.UseVisualStyleBackColor = true;
            // 
            // radioRoleOffline
            // 
            radioRoleOffline.AutoSize = true;
            radioRoleOffline.Location = new Point(208, 13);
            radioRoleOffline.Name = "radioRoleOffline";
            radioRoleOffline.Size = new Size(61, 19);
            radioRoleOffline.TabIndex = 12;
            radioRoleOffline.Text = "Offline";
            radioRoleOffline.UseVisualStyleBackColor = true;
            // 
            // chkWindowed
            // 
            chkWindowed.AutoSize = true;
            chkWindowed.Checked = true;
            chkWindowed.CheckState = CheckState.Checked;
            chkWindowed.Location = new Point(13, 99);
            chkWindowed.Name = "chkWindowed";
            chkWindowed.Size = new Size(83, 19);
            chkWindowed.TabIndex = 13;
            chkWindowed.Text = "Windowed";
            chkWindowed.UseVisualStyleBackColor = true;
            // 
            // GameLauncherDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(399, 230);
            Controls.Add(chkWindowed);
            Controls.Add(radioRoleOffline);
            Controls.Add(radioRoleServer);
            Controls.Add(radioRoleClient);
            Controls.Add(btnLaunchGame);
            Controls.Add(chkNoOVR);
            Controls.Add(chkModerator);
            Controls.Add(chkSpectatorStream);
            Controls.Add(cmbLevel);
            Controls.Add(lblLevel);
            Controls.Add(cmbGametype);
            Controls.Add(lblGametype);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "GameLauncherDialog";
            Text = "Game Launcher";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
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
        private CheckBox chkWindowed;
    }
}