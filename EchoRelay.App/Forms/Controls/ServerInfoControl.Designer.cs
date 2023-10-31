namespace EchoRelay.App.Forms.Controls
{
    partial class ServerInfoControl
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
            lblConfigPeersHeader = new Label();
            lblLoginPeersHeader = new Label();
            lblMatchingPeersHeader = new Label();
            lblServerDBPeersHeader = new Label();
            lblTransactionPeersHeader = new Label();
            lblConfigPeers = new Label();
            lblLoginPeers = new Label();
            lblMatchingPeers = new Label();
            lblServerDBPeers = new Label();
            lblTransactionPeers = new Label();
            rtbGeneratedServiceConfig = new RichTextBox();
            lblServerPort = new Label();
            lblServerPortHeader = new Label();
            lblServiceConfigHeader = new Label();
            lblServerStatus = new Label();
            lblServerStatusHeader = new Label();
            groupBoxServerInfo = new GroupBox();
            btnCopyServiceConfig = new Button();
            btnSaveServiceConfig = new Button();
            groupBoxServerInfo.SuspendLayout();
            SuspendLayout();
            // 
            // lblConfigPeersHeader
            // 
            lblConfigPeersHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblConfigPeersHeader.Location = new Point(6, 65);
            lblConfigPeersHeader.Name = "lblConfigPeersHeader";
            lblConfigPeersHeader.Size = new Size(147, 23);
            lblConfigPeersHeader.TabIndex = 1;
            lblConfigPeersHeader.Text = "Config Peers:";
            // 
            // lblLoginPeersHeader
            // 
            lblLoginPeersHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblLoginPeersHeader.Location = new Point(6, 88);
            lblLoginPeersHeader.Name = "lblLoginPeersHeader";
            lblLoginPeersHeader.Size = new Size(147, 23);
            lblLoginPeersHeader.TabIndex = 3;
            lblLoginPeersHeader.Text = "Login Peers:";
            // 
            // lblMatchingPeersHeader
            // 
            lblMatchingPeersHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblMatchingPeersHeader.Location = new Point(6, 111);
            lblMatchingPeersHeader.Name = "lblMatchingPeersHeader";
            lblMatchingPeersHeader.Size = new Size(147, 23);
            lblMatchingPeersHeader.TabIndex = 5;
            lblMatchingPeersHeader.Text = "Matching Peers:";
            // 
            // lblServerDBPeersHeader
            // 
            lblServerDBPeersHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblServerDBPeersHeader.Location = new Point(6, 134);
            lblServerDBPeersHeader.Name = "lblServerDBPeersHeader";
            lblServerDBPeersHeader.Size = new Size(147, 23);
            lblServerDBPeersHeader.TabIndex = 7;
            lblServerDBPeersHeader.Text = "ServerDB Peers:";
            // 
            // lblTransactionPeersHeader
            // 
            lblTransactionPeersHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblTransactionPeersHeader.Location = new Point(6, 157);
            lblTransactionPeersHeader.Name = "lblTransactionPeersHeader";
            lblTransactionPeersHeader.Size = new Size(147, 23);
            lblTransactionPeersHeader.TabIndex = 8;
            lblTransactionPeersHeader.Text = "Transaction Peers:";
            // 
            // lblConfigPeers
            // 
            lblConfigPeers.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblConfigPeers.Location = new Point(159, 65);
            lblConfigPeers.Name = "lblConfigPeers";
            lblConfigPeers.Size = new Size(147, 23);
            lblConfigPeers.TabIndex = 10;
            lblConfigPeers.Text = "0";
            // 
            // lblLoginPeers
            // 
            lblLoginPeers.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblLoginPeers.Location = new Point(159, 88);
            lblLoginPeers.Name = "lblLoginPeers";
            lblLoginPeers.Size = new Size(147, 23);
            lblLoginPeers.TabIndex = 11;
            lblLoginPeers.Text = "0";
            // 
            // lblMatchingPeers
            // 
            lblMatchingPeers.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblMatchingPeers.Location = new Point(159, 111);
            lblMatchingPeers.Name = "lblMatchingPeers";
            lblMatchingPeers.Size = new Size(147, 23);
            lblMatchingPeers.TabIndex = 12;
            lblMatchingPeers.Text = "0";
            // 
            // lblServerDBPeers
            // 
            lblServerDBPeers.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblServerDBPeers.Location = new Point(159, 134);
            lblServerDBPeers.Name = "lblServerDBPeers";
            lblServerDBPeers.Size = new Size(147, 23);
            lblServerDBPeers.TabIndex = 13;
            lblServerDBPeers.Text = "0";
            // 
            // lblTransactionPeers
            // 
            lblTransactionPeers.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblTransactionPeers.Location = new Point(159, 157);
            lblTransactionPeers.Name = "lblTransactionPeers";
            lblTransactionPeers.Size = new Size(147, 23);
            lblTransactionPeers.TabIndex = 14;
            lblTransactionPeers.Text = "0";
            // 
            // rtbGeneratedServiceConfig
            // 
            rtbGeneratedServiceConfig.BorderStyle = BorderStyle.None;
            rtbGeneratedServiceConfig.DetectUrls = false;
            rtbGeneratedServiceConfig.Location = new Point(6, 206);
            rtbGeneratedServiceConfig.Name = "rtbGeneratedServiceConfig";
            rtbGeneratedServiceConfig.ReadOnly = true;
            rtbGeneratedServiceConfig.Size = new Size(786, 148);
            rtbGeneratedServiceConfig.TabIndex = 9;
            rtbGeneratedServiceConfig.Text = "";
            // 
            // lblServerPort
            // 
            lblServerPort.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblServerPort.Location = new Point(159, 42);
            lblServerPort.Name = "lblServerPort";
            lblServerPort.Size = new Size(147, 23);
            lblServerPort.TabIndex = 16;
            // 
            // lblServerPortHeader
            // 
            lblServerPortHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblServerPortHeader.Location = new Point(6, 42);
            lblServerPortHeader.Name = "lblServerPortHeader";
            lblServerPortHeader.Size = new Size(147, 23);
            lblServerPortHeader.TabIndex = 15;
            lblServerPortHeader.Text = "Server Port (TCP):";
            // 
            // lblServiceConfigHeader
            // 
            lblServiceConfigHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblServiceConfigHeader.Location = new Point(6, 180);
            lblServiceConfigHeader.Name = "lblServiceConfigHeader";
            lblServiceConfigHeader.Size = new Size(161, 23);
            lblServiceConfigHeader.TabIndex = 19;
            lblServiceConfigHeader.Text = "Service Config (generated):";
            // 
            // lblServerStatus
            // 
            lblServerStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblServerStatus.Location = new Point(159, 19);
            lblServerStatus.Name = "lblServerStatus";
            lblServerStatus.Size = new Size(147, 23);
            lblServerStatus.TabIndex = 21;
            lblServerStatus.Text = "Not started";
            // 
            // lblServerStatusHeader
            // 
            lblServerStatusHeader.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblServerStatusHeader.Location = new Point(6, 19);
            lblServerStatusHeader.Name = "lblServerStatusHeader";
            lblServerStatusHeader.Size = new Size(147, 23);
            lblServerStatusHeader.TabIndex = 20;
            lblServerStatusHeader.Text = "Server Status:";
            // 
            // groupBoxServerInfo
            // 
            groupBoxServerInfo.Controls.Add(btnCopyServiceConfig);
            groupBoxServerInfo.Controls.Add(btnSaveServiceConfig);
            groupBoxServerInfo.Controls.Add(lblServerStatusHeader);
            groupBoxServerInfo.Controls.Add(lblServerStatus);
            groupBoxServerInfo.Controls.Add(lblConfigPeersHeader);
            groupBoxServerInfo.Controls.Add(lblLoginPeersHeader);
            groupBoxServerInfo.Controls.Add(rtbGeneratedServiceConfig);
            groupBoxServerInfo.Controls.Add(lblMatchingPeersHeader);
            groupBoxServerInfo.Controls.Add(lblServiceConfigHeader);
            groupBoxServerInfo.Controls.Add(lblServerDBPeersHeader);
            groupBoxServerInfo.Controls.Add(lblServerPort);
            groupBoxServerInfo.Controls.Add(lblTransactionPeersHeader);
            groupBoxServerInfo.Controls.Add(lblServerPortHeader);
            groupBoxServerInfo.Controls.Add(lblConfigPeers);
            groupBoxServerInfo.Controls.Add(lblTransactionPeers);
            groupBoxServerInfo.Controls.Add(lblLoginPeers);
            groupBoxServerInfo.Controls.Add(lblServerDBPeers);
            groupBoxServerInfo.Controls.Add(lblMatchingPeers);
            groupBoxServerInfo.Dock = DockStyle.Fill;
            groupBoxServerInfo.Location = new Point(0, 0);
            groupBoxServerInfo.Name = "groupBoxServerInfo";
            groupBoxServerInfo.Size = new Size(854, 366);
            groupBoxServerInfo.TabIndex = 22;
            groupBoxServerInfo.TabStop = false;
            groupBoxServerInfo.Text = "Server Info";
            // 
            // btnCopyServiceConfig
            // 
            btnCopyServiceConfig.Location = new Point(798, 206);
            btnCopyServiceConfig.Name = "btnCopyServiceConfig";
            btnCopyServiceConfig.Size = new Size(50, 23);
            btnCopyServiceConfig.TabIndex = 23;
            btnCopyServiceConfig.Text = "Copy";
            btnCopyServiceConfig.UseVisualStyleBackColor = true;
            btnCopyServiceConfig.Click += btnCopyServiceConfig_Click;
            // 
            // btnSaveServiceConfig
            // 
            btnSaveServiceConfig.Location = new Point(798, 235);
            btnSaveServiceConfig.Name = "btnSaveServiceConfig";
            btnSaveServiceConfig.Size = new Size(50, 23);
            btnSaveServiceConfig.TabIndex = 22;
            btnSaveServiceConfig.Text = "Save";
            btnSaveServiceConfig.UseVisualStyleBackColor = true;
            btnSaveServiceConfig.Click += btnSaveServiceConfig_Click;
            // 
            // ServerInfoControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBoxServerInfo);
            Name = "ServerInfoControl";
            Size = new Size(854, 366);
            groupBoxServerInfo.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Label lblConfigPeersHeader;
        private Label lblLoginPeersHeader;
        private Label lblMatchingPeersHeader;
        private Label lblServerDBPeersHeader;
        private Label lblTransactionPeersHeader;
        private Label lblConfigPeers;
        private Label lblLoginPeers;
        private Label lblMatchingPeers;
        private Label lblServerDBPeers;
        private Label lblTransactionPeers;
        private RichTextBox rtbGeneratedServiceConfig;
        private Label lblServerPort;
        private Label lblServerPortHeader;
        private Label lblServiceConfigHeader;
        private Label lblServerStatus;
        private Label lblServerStatusHeader;
        private GroupBox groupBoxServerInfo;
        private Button btnCopyServiceConfig;
        private Button btnSaveServiceConfig;
    }
}
