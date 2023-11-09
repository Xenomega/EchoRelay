namespace EchoRelay
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            saveChangesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            revertUnsavedChangesToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            startServerToolStripMenuItem = new ToolStripMenuItem();
            launchEchoVRToolStripMenuItem = new ToolStripMenuItem();
            serverHeadlessThrottledToolStripMenuItem = new ToolStripMenuItem();
            serverheadlessUnthrottledHighCPUToolStripMenuItem = new ToolStripMenuItem();
            serverToolStripMenuItem = new ToolStripMenuItem();
            clientWindowedNoOVRToolStripMenuItem = new ToolStripMenuItem();
            clientWindowedOVRToolStripMenuItem = new ToolStripMenuItem();
            clientOVRToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            customToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            rtbLog = new RichTextBox();
            ctxMenuServerLog = new ContextMenuStrip(components);
            clearToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            btnToggleRunningState = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            btnLaunchGameServer = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            btnSaveChanges = new ToolStripButton();
            btnRevertChanges = new ToolStripButton();
            tabControlMain = new TabControl();
            tabServerInfo = new TabPage();
            serverInfoControl = new App.Forms.Controls.ServerInfoControl();
            tabPeers = new TabPage();
            peerConnectionsControl = new App.Forms.Controls.PeerConnectionsControl();
            tabGameServers = new TabPage();
            gameServersControl = new App.Forms.Controls.GameServersControl();
            tabStorage = new TabPage();
            tabControlStorage = new TabControl();
            tabStorageAccessControls = new TabPage();
            accessControlListEditor = new App.Forms.Controls.AccessControlListEditor();
            tabStorageAccounts = new TabPage();
            accountSelector = new App.Forms.Controls.AccountSelector();
            tabStorageChannelInfo = new TabPage();
            channelInfoEditor = new App.Forms.Controls.ChannelInfoEditor();
            tabStorageConfigs = new TabPage();
            tabStorageDocuments = new TabPage();
            tabStorageLoginSettings = new TabPage();
            loginSettingsEditor = new App.Forms.Controls.LoginSettingsEditor();
            statusStripProgress = new StatusStrip();
            lblToolStripPadding = new ToolStripStatusLabel();
            lblStatusHeader = new ToolStripStatusLabel();
            lblStatus = new ToolStripStatusLabel();
            progressBarStatus = new ToolStripProgressBar();
            panelTabStripContainer = new Panel();
            splitContainer1 = new SplitContainer();
            groupBoxLog = new GroupBox();
            menuStrip1.SuspendLayout();
            ctxMenuServerLog.SuspendLayout();
            toolStrip1.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabServerInfo.SuspendLayout();
            tabPeers.SuspendLayout();
            tabGameServers.SuspendLayout();
            tabStorage.SuspendLayout();
            tabControlStorage.SuspendLayout();
            tabStorageAccessControls.SuspendLayout();
            tabStorageAccounts.SuspendLayout();
            tabStorageChannelInfo.SuspendLayout();
            tabStorageLoginSettings.SuspendLayout();
            statusStripProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBoxLog.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1333, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveChangesToolStripMenuItem, toolStripSeparator3, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // saveChangesToolStripMenuItem
            // 
            saveChangesToolStripMenuItem.Name = "saveChangesToolStripMenuItem";
            saveChangesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveChangesToolStripMenuItem.Size = new Size(187, 22);
            saveChangesToolStripMenuItem.Text = "Save Changes";
            saveChangesToolStripMenuItem.Click += btnSaveChanges_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(184, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(187, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { revertUnsavedChangesToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // revertUnsavedChangesToolStripMenuItem
            // 
            revertUnsavedChangesToolStripMenuItem.Name = "revertUnsavedChangesToolStripMenuItem";
            revertUnsavedChangesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.R;
            revertUnsavedChangesToolStripMenuItem.Size = new Size(242, 22);
            revertUnsavedChangesToolStripMenuItem.Text = "Revert unsaved changes";
            revertUnsavedChangesToolStripMenuItem.Click += btnRevertChanges_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { startServerToolStripMenuItem, launchEchoVRToolStripMenuItem, settingsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // startServerToolStripMenuItem
            // 
            startServerToolStripMenuItem.Name = "startServerToolStripMenuItem";
            startServerToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.T;
            startServerToolStripMenuItem.Size = new Size(180, 22);
            startServerToolStripMenuItem.Text = "Start server";
            startServerToolStripMenuItem.Click += btnToggleRunningState_Click;
            // 
            // launchEchoVRToolStripMenuItem
            // 
            launchEchoVRToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { serverHeadlessThrottledToolStripMenuItem, serverheadlessUnthrottledHighCPUToolStripMenuItem, serverToolStripMenuItem, clientWindowedNoOVRToolStripMenuItem, clientWindowedOVRToolStripMenuItem, clientOVRToolStripMenuItem, toolStripSeparator1, customToolStripMenuItem });
            launchEchoVRToolStripMenuItem.Name = "launchEchoVRToolStripMenuItem";
            launchEchoVRToolStripMenuItem.Size = new Size(180, 22);
            launchEchoVRToolStripMenuItem.Text = "Launch Echo VR";
            // 
            // serverHeadlessThrottledToolStripMenuItem
            // 
            serverHeadlessThrottledToolStripMenuItem.Name = "serverHeadlessThrottledToolStripMenuItem";
            serverHeadlessThrottledToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D1;
            serverHeadlessThrottledToolStripMenuItem.Size = new Size(324, 22);
            serverHeadlessThrottledToolStripMenuItem.Text = "Server (headless, default tick rate)";
            serverHeadlessThrottledToolStripMenuItem.Click += serverHeadlessThrottledToolStripMenuItem_Click;
            // 
            // serverheadlessUnthrottledHighCPUToolStripMenuItem
            // 
            serverheadlessUnthrottledHighCPUToolStripMenuItem.Name = "serverheadlessUnthrottledHighCPUToolStripMenuItem";
            serverheadlessUnthrottledHighCPUToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D2;
            serverheadlessUnthrottledHighCPUToolStripMenuItem.Size = new Size(324, 22);
            serverheadlessUnthrottledHighCPUToolStripMenuItem.Text = "Server (headless, unthrottled, high CPU)";
            serverheadlessUnthrottledHighCPUToolStripMenuItem.Click += serverheadlessUnthrottledHighCPUToolStripMenuItem_Click;
            // 
            // serverToolStripMenuItem
            // 
            serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            serverToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D3;
            serverToolStripMenuItem.Size = new Size(324, 22);
            serverToolStripMenuItem.Text = "Server (windowed)";
            serverToolStripMenuItem.Click += serverToolStripMenuItem_Click;
            // 
            // clientWindowedNoOVRToolStripMenuItem
            // 
            clientWindowedNoOVRToolStripMenuItem.Name = "clientWindowedNoOVRToolStripMenuItem";
            clientWindowedNoOVRToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D4;
            clientWindowedNoOVRToolStripMenuItem.Size = new Size(324, 22);
            clientWindowedNoOVRToolStripMenuItem.Text = "Client (windowed, demo profile)";
            clientWindowedNoOVRToolStripMenuItem.Click += clientWindowedNoOVRToolStripMenuItem_Click;
            // 
            // clientWindowedOVRToolStripMenuItem
            // 
            clientWindowedOVRToolStripMenuItem.Name = "clientWindowedOVRToolStripMenuItem";
            clientWindowedOVRToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D5;
            clientWindowedOVRToolStripMenuItem.Size = new Size(324, 22);
            clientWindowedOVRToolStripMenuItem.Text = "Client (windowed)";
            clientWindowedOVRToolStripMenuItem.Click += clientWindowedOVRToolStripMenuItem_Click;
            // 
            // clientOVRToolStripMenuItem
            // 
            clientOVRToolStripMenuItem.Name = "clientOVRToolStripMenuItem";
            clientOVRToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D6;
            clientOVRToolStripMenuItem.Size = new Size(324, 22);
            clientOVRToolStripMenuItem.Text = "Client (VR)";
            clientOVRToolStripMenuItem.Click += clientOVRToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(321, 6);
            // 
            // customToolStripMenuItem
            // 
            customToolStripMenuItem.Name = "customToolStripMenuItem";
            customToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.G;
            customToolStripMenuItem.Size = new Size(324, 22);
            customToolStripMenuItem.Text = "Custom";
            customToolStripMenuItem.Click += customToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(180, 22);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(107, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // rtbLog
            // 
            rtbLog.BackColor = SystemColors.Window;
            rtbLog.ContextMenuStrip = ctxMenuServerLog;
            rtbLog.Dock = DockStyle.Fill;
            rtbLog.Location = new Point(3, 19);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new Size(1327, 180);
            rtbLog.TabIndex = 1;
            rtbLog.Text = "";
            rtbLog.WordWrap = false;
            // 
            // ctxMenuServerLog
            // 
            ctxMenuServerLog.Items.AddRange(new ToolStripItem[] { clearToolStripMenuItem });
            ctxMenuServerLog.Name = "ctxMenuServerLog";
            ctxMenuServerLog.Size = new Size(102, 26);
            // 
            // clearToolStripMenuItem
            // 
            clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            clearToolStripMenuItem.Size = new Size(101, 22);
            clearToolStripMenuItem.Text = "Clear";
            clearToolStripMenuItem.Click += clearToolStripMenuItem_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnToggleRunningState, toolStripSeparator2, btnLaunchGameServer, toolStripSeparator4, btnSaveChanges, btnRevertChanges });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1333, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnToggleRunningState
            // 
            btnToggleRunningState.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnToggleRunningState.Image = App.Properties.Resources.play_button_icon;
            btnToggleRunningState.ImageTransparentColor = Color.Magenta;
            btnToggleRunningState.Name = "btnToggleRunningState";
            btnToggleRunningState.Size = new Size(23, 22);
            btnToggleRunningState.Text = "Toggle server on/off";
            btnToggleRunningState.Click += btnToggleRunningState_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // btnLaunchGameServer
            // 
            btnLaunchGameServer.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnLaunchGameServer.Image = App.Properties.Resources.launch_server_button_icon;
            btnLaunchGameServer.ImageTransparentColor = Color.Magenta;
            btnLaunchGameServer.Name = "btnLaunchGameServer";
            btnLaunchGameServer.Size = new Size(23, 22);
            btnLaunchGameServer.Text = "Launch Game Server";
            btnLaunchGameServer.Click += serverHeadlessThrottledToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // btnSaveChanges
            // 
            btnSaveChanges.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnSaveChanges.Image = App.Properties.Resources.save_button_icon;
            btnSaveChanges.ImageTransparentColor = Color.Magenta;
            btnSaveChanges.Name = "btnSaveChanges";
            btnSaveChanges.Size = new Size(23, 22);
            btnSaveChanges.Text = "Save changes";
            btnSaveChanges.Click += btnSaveChanges_Click;
            // 
            // btnRevertChanges
            // 
            btnRevertChanges.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnRevertChanges.Image = App.Properties.Resources.undo_button_icon;
            btnRevertChanges.ImageTransparentColor = Color.Magenta;
            btnRevertChanges.Name = "btnRevertChanges";
            btnRevertChanges.Size = new Size(23, 22);
            btnRevertChanges.Text = "Revert unsaved changes";
            btnRevertChanges.Click += btnRevertChanges_Click;
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabServerInfo);
            tabControlMain.Controls.Add(tabPeers);
            tabControlMain.Controls.Add(tabGameServers);
            tabControlMain.Controls.Add(tabStorage);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1333, 539);
            tabControlMain.TabIndex = 3;
            // 
            // tabServerInfo
            // 
            tabServerInfo.Controls.Add(serverInfoControl);
            tabServerInfo.Location = new Point(4, 24);
            tabServerInfo.Name = "tabServerInfo";
            tabServerInfo.Padding = new Padding(3);
            tabServerInfo.Size = new Size(1325, 511);
            tabServerInfo.TabIndex = 0;
            tabServerInfo.Text = "Server Info";
            tabServerInfo.UseVisualStyleBackColor = true;
            // 
            // serverInfoControl
            // 
            serverInfoControl.Dock = DockStyle.Fill;
            serverInfoControl.Location = new Point(3, 3);
            serverInfoControl.Name = "serverInfoControl";
            serverInfoControl.Size = new Size(1319, 505);
            serverInfoControl.TabIndex = 0;
            // 
            // tabPeers
            // 
            tabPeers.Controls.Add(peerConnectionsControl);
            tabPeers.Location = new Point(4, 24);
            tabPeers.Name = "tabPeers";
            tabPeers.Padding = new Padding(3);
            tabPeers.Size = new Size(1325, 511);
            tabPeers.TabIndex = 2;
            tabPeers.Text = "Peers";
            tabPeers.UseVisualStyleBackColor = true;
            // 
            // peerConnectionsControl
            // 
            peerConnectionsControl.Dock = DockStyle.Fill;
            peerConnectionsControl.Location = new Point(3, 3);
            peerConnectionsControl.Name = "peerConnectionsControl";
            peerConnectionsControl.Size = new Size(1319, 505);
            peerConnectionsControl.TabIndex = 0;
            // 
            // tabGameServers
            // 
            tabGameServers.Controls.Add(gameServersControl);
            tabGameServers.Location = new Point(4, 24);
            tabGameServers.Name = "tabGameServers";
            tabGameServers.Padding = new Padding(3);
            tabGameServers.Size = new Size(1325, 511);
            tabGameServers.TabIndex = 3;
            tabGameServers.Text = "Game Servers";
            tabGameServers.UseVisualStyleBackColor = true;
            // 
            // gameServersControl
            // 
            gameServersControl.Dock = DockStyle.Fill;
            gameServersControl.Location = new Point(3, 3);
            gameServersControl.Name = "gameServersControl";
            gameServersControl.Size = new Size(1319, 505);
            gameServersControl.TabIndex = 0;
            // 
            // tabStorage
            // 
            tabStorage.Controls.Add(tabControlStorage);
            tabStorage.Location = new Point(4, 24);
            tabStorage.Name = "tabStorage";
            tabStorage.Padding = new Padding(3);
            tabStorage.Size = new Size(1325, 511);
            tabStorage.TabIndex = 1;
            tabStorage.Text = "Storage";
            tabStorage.UseVisualStyleBackColor = true;
            // 
            // tabControlStorage
            // 
            tabControlStorage.Controls.Add(tabStorageAccessControls);
            tabControlStorage.Controls.Add(tabStorageAccounts);
            tabControlStorage.Controls.Add(tabStorageChannelInfo);
            tabControlStorage.Controls.Add(tabStorageConfigs);
            tabControlStorage.Controls.Add(tabStorageDocuments);
            tabControlStorage.Controls.Add(tabStorageLoginSettings);
            tabControlStorage.Dock = DockStyle.Fill;
            tabControlStorage.Location = new Point(3, 3);
            tabControlStorage.Name = "tabControlStorage";
            tabControlStorage.SelectedIndex = 0;
            tabControlStorage.Size = new Size(1319, 505);
            tabControlStorage.TabIndex = 0;
            // 
            // tabStorageAccessControls
            // 
            tabStorageAccessControls.Controls.Add(accessControlListEditor);
            tabStorageAccessControls.Location = new Point(4, 24);
            tabStorageAccessControls.Name = "tabStorageAccessControls";
            tabStorageAccessControls.Padding = new Padding(3);
            tabStorageAccessControls.Size = new Size(1311, 477);
            tabStorageAccessControls.TabIndex = 5;
            tabStorageAccessControls.Text = "Access Controls";
            tabStorageAccessControls.UseVisualStyleBackColor = true;
            // 
            // accessControlListEditor
            // 
            accessControlListEditor.Changed = false;
            accessControlListEditor.Dock = DockStyle.Fill;
            accessControlListEditor.Location = new Point(3, 3);
            accessControlListEditor.Name = "accessControlListEditor";
            accessControlListEditor.Size = new Size(1305, 471);
            accessControlListEditor.Storage = null;
            accessControlListEditor.TabIndex = 0;
            // 
            // tabStorageAccounts
            // 
            tabStorageAccounts.AutoScroll = true;
            tabStorageAccounts.Controls.Add(accountSelector);
            tabStorageAccounts.Location = new Point(4, 24);
            tabStorageAccounts.Name = "tabStorageAccounts";
            tabStorageAccounts.Padding = new Padding(3);
            tabStorageAccounts.Size = new Size(1311, 477);
            tabStorageAccounts.TabIndex = 0;
            tabStorageAccounts.Text = "Accounts";
            tabStorageAccounts.UseVisualStyleBackColor = true;
            // 
            // accountSelector
            // 
            accountSelector.Changed = false;
            accountSelector.Dock = DockStyle.Fill;
            accountSelector.Location = new Point(3, 3);
            accountSelector.Name = "accountSelector";
            accountSelector.Size = new Size(1305, 471);
            accountSelector.Storage = null;
            accountSelector.TabIndex = 0;
            // 
            // tabStorageChannelInfo
            // 
            tabStorageChannelInfo.AutoScroll = true;
            tabStorageChannelInfo.Controls.Add(channelInfoEditor);
            tabStorageChannelInfo.Location = new Point(4, 24);
            tabStorageChannelInfo.Name = "tabStorageChannelInfo";
            tabStorageChannelInfo.Padding = new Padding(3);
            tabStorageChannelInfo.Size = new Size(1311, 477);
            tabStorageChannelInfo.TabIndex = 1;
            tabStorageChannelInfo.Text = "Channels";
            tabStorageChannelInfo.UseVisualStyleBackColor = true;
            // 
            // channelInfoEditor
            // 
            channelInfoEditor.Changed = false;
            channelInfoEditor.Dock = DockStyle.Top;
            channelInfoEditor.Location = new Point(3, 3);
            channelInfoEditor.Name = "channelInfoEditor";
            channelInfoEditor.Size = new Size(1305, 362);
            channelInfoEditor.Storage = null;
            channelInfoEditor.TabIndex = 0;
            // 
            // tabStorageConfigs
            // 
            tabStorageConfigs.Location = new Point(4, 24);
            tabStorageConfigs.Name = "tabStorageConfigs";
            tabStorageConfigs.Padding = new Padding(3);
            tabStorageConfigs.Size = new Size(1311, 477);
            tabStorageConfigs.TabIndex = 2;
            tabStorageConfigs.Text = "Configs";
            tabStorageConfigs.UseVisualStyleBackColor = true;
            // 
            // tabStorageDocuments
            // 
            tabStorageDocuments.Location = new Point(4, 24);
            tabStorageDocuments.Name = "tabStorageDocuments";
            tabStorageDocuments.Padding = new Padding(3);
            tabStorageDocuments.Size = new Size(1311, 477);
            tabStorageDocuments.TabIndex = 3;
            tabStorageDocuments.Text = "Documents";
            tabStorageDocuments.UseVisualStyleBackColor = true;
            // 
            // tabStorageLoginSettings
            // 
            tabStorageLoginSettings.AutoScroll = true;
            tabStorageLoginSettings.Controls.Add(loginSettingsEditor);
            tabStorageLoginSettings.Location = new Point(4, 24);
            tabStorageLoginSettings.Name = "tabStorageLoginSettings";
            tabStorageLoginSettings.Padding = new Padding(3);
            tabStorageLoginSettings.Size = new Size(1311, 477);
            tabStorageLoginSettings.TabIndex = 4;
            tabStorageLoginSettings.Text = "Login Settings";
            tabStorageLoginSettings.UseVisualStyleBackColor = true;
            // 
            // loginSettingsEditor
            // 
            loginSettingsEditor.Changed = false;
            loginSettingsEditor.Dock = DockStyle.Top;
            loginSettingsEditor.Location = new Point(3, 3);
            loginSettingsEditor.Name = "loginSettingsEditor";
            loginSettingsEditor.Size = new Size(1305, 226);
            loginSettingsEditor.Storage = null;
            loginSettingsEditor.TabIndex = 0;
            // 
            // statusStripProgress
            // 
            statusStripProgress.Items.AddRange(new ToolStripItem[] { lblToolStripPadding, lblStatusHeader, lblStatus, progressBarStatus });
            statusStripProgress.Location = new Point(0, 794);
            statusStripProgress.Name = "statusStripProgress";
            statusStripProgress.Size = new Size(1333, 22);
            statusStripProgress.TabIndex = 4;
            statusStripProgress.Text = "statusStrip1";
            // 
            // lblToolStripPadding
            // 
            lblToolStripPadding.Name = "lblToolStripPadding";
            lblToolStripPadding.Size = new Size(1148, 17);
            lblToolStripPadding.Spring = true;
            // 
            // lblStatusHeader
            // 
            lblStatusHeader.Name = "lblStatusHeader";
            lblStatusHeader.Size = new Size(42, 17);
            lblStatusHeader.Text = "Status:";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(26, 17);
            lblStatus.Text = "Idle";
            // 
            // progressBarStatus
            // 
            progressBarStatus.Alignment = ToolStripItemAlignment.Right;
            progressBarStatus.Name = "progressBarStatus";
            progressBarStatus.Size = new Size(100, 16);
            // 
            // panelTabStripContainer
            // 
            panelTabStripContainer.Location = new Point(0, 49);
            panelTabStripContainer.Name = "panelTabStripContainer";
            panelTabStripContainer.Size = new Size(1003, 560);
            panelTabStripContainer.TabIndex = 5;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.Location = new Point(0, 49);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tabControlMain);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(groupBoxLog);
            splitContainer1.Size = new Size(1333, 745);
            splitContainer1.SplitterDistance = 539;
            splitContainer1.TabIndex = 4;
            // 
            // groupBoxLog
            // 
            groupBoxLog.Controls.Add(rtbLog);
            groupBoxLog.Dock = DockStyle.Fill;
            groupBoxLog.Location = new Point(0, 0);
            groupBoxLog.Name = "groupBoxLog";
            groupBoxLog.Size = new Size(1333, 202);
            groupBoxLog.TabIndex = 2;
            groupBoxLog.TabStop = false;
            groupBoxLog.Text = "Server Log";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1333, 816);
            Controls.Add(splitContainer1);
            Controls.Add(panelTabStripContainer);
            Controls.Add(statusStripProgress);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainWindow";
            Text = "Echo Relay";
            FormClosing += MainWindow_FormClosing;
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ctxMenuServerLog.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            tabControlMain.ResumeLayout(false);
            tabServerInfo.ResumeLayout(false);
            tabPeers.ResumeLayout(false);
            tabGameServers.ResumeLayout(false);
            tabStorage.ResumeLayout(false);
            tabControlStorage.ResumeLayout(false);
            tabStorageAccessControls.ResumeLayout(false);
            tabStorageAccounts.ResumeLayout(false);
            tabStorageChannelInfo.ResumeLayout(false);
            tabStorageLoginSettings.ResumeLayout(false);
            statusStripProgress.ResumeLayout(false);
            statusStripProgress.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBoxLog.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private RichTextBox rtbLog;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem launchEchoVRToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem clientOVRToolStripMenuItem;
        private ToolStripMenuItem clientWindowedNoOVRToolStripMenuItem;
        private ToolStripMenuItem serverHeadlessThrottledToolStripMenuItem;
        private ToolStripMenuItem customToolStripMenuItem;
        private ToolStripMenuItem clientWindowedOVRToolStripMenuItem;
        private ToolStripMenuItem serverToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton btnToggleRunningState;
        private TabControl tabControlMain;
        private TabPage tabServerInfo;
        private TabPage tabStorage;
        private TabControl tabControlStorage;
        private TabPage tabStorageAccounts;
        private TabPage tabStorageChannelInfo;
        private TabPage tabStorageConfigs;
        private TabPage tabStorageDocuments;
        private TabPage tabStorageLoginSettings;
        private StatusStrip statusStripProgress;
        private ToolStripProgressBar progressBarStatus;
        private ToolStripStatusLabel lblToolStripPadding;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblStatusHeader;
        private Panel panelTabStripContainer;
        private SplitContainer splitContainer1;
        private EchoRelay.App.Forms.Controls.LoginSettingsEditor loginSettingsEditor;
        private EchoRelay.App.Forms.Controls.ChannelInfoEditor channelInfoEditor;
        private EchoRelay.App.Forms.Controls.ServerInfoControl serverInfoControl;
        private TabPage tabPeers;
        private TabPage tabGameServers;
        private EchoRelay.App.Forms.Controls.PeerConnectionsControl peerConnectionsControl;
        private ToolStripMenuItem startServerToolStripMenuItem;
        private EchoRelay.App.Forms.Controls.GameServersControl gameServersControl;
        private GroupBox groupBoxLog;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btnLaunchGameServer;
        private ToolStripMenuItem saveChangesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem revertUnsavedChangesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton btnSaveChanges;
        private ToolStripButton btnRevertChanges;
        private EchoRelay.App.Forms.Controls.AccountSelector accountSelector;
        private TabPage tabStorageAccessControls;
        private EchoRelay.App.Forms.Controls.AccessControlListEditor accessControlListEditor;
        private ContextMenuStrip ctxMenuServerLog;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem serverheadlessUnthrottledHighCPUToolStripMenuItem;
    }
}