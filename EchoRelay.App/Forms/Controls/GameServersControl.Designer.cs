namespace EchoRelay.App.Forms.Controls
{
    partial class GameServersControl
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
            components = new System.ComponentModel.Container();
            listGameServers = new ListView();
            columnHeaderServerId = new ColumnHeader();
            columnHeaderIP = new ColumnHeader();
            columnHeaderBroadcastPort = new ColumnHeader();
            columnHeaderGametype = new ColumnHeader();
            columnHeaderLevel = new ColumnHeader();
            columnHeaderPlayerCount = new ColumnHeader();
            columnHeaderLobbyType = new ColumnHeader();
            columnHeaderLocked = new ColumnHeader();
            columnHeaderChannel = new ColumnHeader();
            columnHeaderSessionId = new ColumnHeader();
            splitContainer1 = new SplitContainer();
            groupBoxGameServers = new GroupBox();
            groupBoxPlayers = new GroupBox();
            listPlayers = new ListView();
            columnHeaderUserId = new ColumnHeader();
            columnHeaderUserDisplayName = new ColumnHeader();
            columnHeaderUserIP = new ColumnHeader();
            columnHeaderPlayerSession = new ColumnHeader();
            contextMenuPlayers = new ContextMenuStrip(components);
            copyUserIdToolStripMenuItem = new ToolStripMenuItem();
            copyIPAddressToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            kickToolStripMenuItem = new ToolStripMenuItem();
            contextMenuGameServers = new ContextMenuStrip(components);
            copySessionLobbyIdToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBoxGameServers.SuspendLayout();
            groupBoxPlayers.SuspendLayout();
            contextMenuPlayers.SuspendLayout();
            contextMenuGameServers.SuspendLayout();
            SuspendLayout();
            // 
            // listGameServers
            // 
            listGameServers.Columns.AddRange(new ColumnHeader[] { columnHeaderServerId, columnHeaderIP, columnHeaderBroadcastPort, columnHeaderGametype, columnHeaderLevel, columnHeaderPlayerCount, columnHeaderLobbyType, columnHeaderLocked, columnHeaderChannel, columnHeaderSessionId });
            listGameServers.Dock = DockStyle.Fill;
            listGameServers.FullRowSelect = true;
            listGameServers.GridLines = true;
            listGameServers.Location = new Point(3, 19);
            listGameServers.MultiSelect = false;
            listGameServers.Name = "listGameServers";
            listGameServers.Size = new Size(1276, 386);
            listGameServers.TabIndex = 1;
            listGameServers.UseCompatibleStateImageBehavior = false;
            listGameServers.View = View.Details;
            listGameServers.SelectedIndexChanged += listGameServers_SelectedIndexChanged;
            // 
            // columnHeaderServerId
            // 
            columnHeaderServerId.Text = "Server Identifier";
            columnHeaderServerId.Width = 150;
            // 
            // columnHeaderIP
            // 
            columnHeaderIP.Text = "IP";
            columnHeaderIP.Width = 110;
            // 
            // columnHeaderBroadcastPort
            // 
            columnHeaderBroadcastPort.Text = "Port (UDP)";
            columnHeaderBroadcastPort.Width = 75;
            // 
            // columnHeaderGametype
            // 
            columnHeaderGametype.Text = "Gametype";
            columnHeaderGametype.Width = 150;
            // 
            // columnHeaderLevel
            // 
            columnHeaderLevel.Text = "Level";
            columnHeaderLevel.Width = 150;
            // 
            // columnHeaderPlayerCount
            // 
            columnHeaderPlayerCount.Text = "Players";
            // 
            // columnHeaderLobbyType
            // 
            columnHeaderLobbyType.Text = "Type";
            columnHeaderLobbyType.Width = 80;
            // 
            // columnHeaderLocked
            // 
            columnHeaderLocked.Text = "Is Locked";
            columnHeaderLocked.Width = 80;
            // 
            // columnHeaderChannel
            // 
            columnHeaderChannel.Text = "Channel";
            columnHeaderChannel.Width = 120;
            // 
            // columnHeaderSessionId
            // 
            columnHeaderSessionId.Text = "Session/Lobby Identifier";
            columnHeaderSessionId.Width = 300;
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
            splitContainer1.Panel1.Controls.Add(groupBoxGameServers);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(groupBoxPlayers);
            splitContainer1.Size = new Size(1282, 816);
            splitContainer1.SplitterDistance = 408;
            splitContainer1.TabIndex = 2;
            // 
            // groupBoxGameServers
            // 
            groupBoxGameServers.Controls.Add(listGameServers);
            groupBoxGameServers.Dock = DockStyle.Fill;
            groupBoxGameServers.Location = new Point(0, 0);
            groupBoxGameServers.Name = "groupBoxGameServers";
            groupBoxGameServers.Size = new Size(1282, 408);
            groupBoxGameServers.TabIndex = 2;
            groupBoxGameServers.TabStop = false;
            groupBoxGameServers.Text = "Game servers";
            // 
            // groupBoxPlayers
            // 
            groupBoxPlayers.Controls.Add(listPlayers);
            groupBoxPlayers.Dock = DockStyle.Fill;
            groupBoxPlayers.Location = new Point(0, 0);
            groupBoxPlayers.Name = "groupBoxPlayers";
            groupBoxPlayers.Size = new Size(1282, 404);
            groupBoxPlayers.TabIndex = 3;
            groupBoxPlayers.TabStop = false;
            groupBoxPlayers.Text = "Players in selected game server";
            // 
            // listPlayers
            // 
            listPlayers.Columns.AddRange(new ColumnHeader[] { columnHeaderUserId, columnHeaderUserDisplayName, columnHeaderUserIP, columnHeaderPlayerSession });
            listPlayers.Dock = DockStyle.Fill;
            listPlayers.FullRowSelect = true;
            listPlayers.GridLines = true;
            listPlayers.Location = new Point(3, 19);
            listPlayers.Name = "listPlayers";
            listPlayers.Size = new Size(1276, 382);
            listPlayers.TabIndex = 2;
            listPlayers.UseCompatibleStateImageBehavior = false;
            listPlayers.View = View.Details;
            listPlayers.SelectedIndexChanged += listPlayers_SelectedIndexChanged;
            // 
            // columnHeaderUserId
            // 
            columnHeaderUserId.Text = "User Id";
            columnHeaderUserId.Width = 200;
            // 
            // columnHeaderUserDisplayName
            // 
            columnHeaderUserDisplayName.Text = "User Display Name";
            columnHeaderUserDisplayName.Width = 200;
            // 
            // columnHeaderUserIP
            // 
            columnHeaderUserIP.Text = "IP";
            columnHeaderUserIP.Width = 120;
            // 
            // columnHeaderPlayerSession
            // 
            columnHeaderPlayerSession.Text = "Player Session";
            columnHeaderPlayerSession.Width = 300;
            // 
            // contextMenuPlayers
            // 
            contextMenuPlayers.Items.AddRange(new ToolStripItem[] { copyUserIdToolStripMenuItem, copyIPAddressToolStripMenuItem, toolStripSeparator1, kickToolStripMenuItem });
            contextMenuPlayers.Name = "contextMenuPlayers";
            contextMenuPlayers.Size = new Size(161, 76);
            // 
            // copyUserIdToolStripMenuItem
            // 
            copyUserIdToolStripMenuItem.Name = "copyUserIdToolStripMenuItem";
            copyUserIdToolStripMenuItem.Size = new Size(160, 22);
            copyUserIdToolStripMenuItem.Text = "Copy User Id";
            copyUserIdToolStripMenuItem.Click += copyUserIdToolStripMenuItem_Click;
            // 
            // copyIPAddressToolStripMenuItem
            // 
            copyIPAddressToolStripMenuItem.Name = "copyIPAddressToolStripMenuItem";
            copyIPAddressToolStripMenuItem.Size = new Size(160, 22);
            copyIPAddressToolStripMenuItem.Text = "Copy IP Address";
            copyIPAddressToolStripMenuItem.Click += copyIPAddressToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(157, 6);
            // 
            // kickToolStripMenuItem
            // 
            kickToolStripMenuItem.Name = "kickToolStripMenuItem";
            kickToolStripMenuItem.Size = new Size(160, 22);
            kickToolStripMenuItem.Text = "Kick";
            kickToolStripMenuItem.Click += kickToolStripMenuItem_Click;
            // 
            // contextMenuGameServers
            // 
            contextMenuGameServers.Items.AddRange(new ToolStripItem[] { copySessionLobbyIdToolStripMenuItem });
            contextMenuGameServers.Name = "contextMenuGameServers";
            contextMenuGameServers.Size = new Size(202, 26);
            // 
            // copySessionLobbyIdToolStripMenuItem
            // 
            copySessionLobbyIdToolStripMenuItem.Name = "copySessionLobbyIdToolStripMenuItem";
            copySessionLobbyIdToolStripMenuItem.Size = new Size(201, 22);
            copySessionLobbyIdToolStripMenuItem.Text = "Copy Session / Lobby Id";
            copySessionLobbyIdToolStripMenuItem.Click += copySessionLobbyIdToolStripMenuItem_Click;
            // 
            // GameServersControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "GameServersControl";
            Size = new Size(1282, 816);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBoxGameServers.ResumeLayout(false);
            groupBoxPlayers.ResumeLayout(false);
            contextMenuPlayers.ResumeLayout(false);
            contextMenuGameServers.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView listGameServers;
        private ColumnHeader columnHeaderIP;
        private ColumnHeader columnHeaderGametype;
        private ColumnHeader columnHeaderLevel;
        private SplitContainer splitContainer1;
        private ColumnHeader columnHeaderBroadcastPort;
        private ColumnHeader columnHeaderServerId;
        private ColumnHeader columnHeaderPlayerCount;
        private ColumnHeader columnHeaderLobbyType;
        private ColumnHeader columnHeaderSessionId;
        private ListView listPlayers;
        private ColumnHeader columnHeaderUserId;
        private ColumnHeader columnHeaderUserDisplayName;
        private ColumnHeader columnHeaderUserIP;
        private ColumnHeader columnHeaderPlayerSession;
        private GroupBox groupBoxGameServers;
        private GroupBox groupBoxPlayers;
        private ContextMenuStrip contextMenuPlayers;
        private ToolStripMenuItem kickToolStripMenuItem;
        private ColumnHeader columnHeaderLocked;
        private ColumnHeader columnHeaderChannel;
        private ContextMenuStrip contextMenuGameServers;
        private ToolStripMenuItem copySessionLobbyIdToolStripMenuItem;
        private ToolStripMenuItem copyUserIdToolStripMenuItem;
        private ToolStripMenuItem copyIPAddressToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
    }
}
