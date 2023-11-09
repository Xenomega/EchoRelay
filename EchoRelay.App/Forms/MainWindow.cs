using EchoRelay.App.Forms.Controls;
using EchoRelay.App.Forms.Dialogs;
using EchoRelay.App.Properties;
using EchoRelay.App.Settings;
using EchoRelay.App.Utils;
using EchoRelay.Core.Game;
using EchoRelay.Core.Server;
using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Server.Services;
using EchoRelay.Core.Server.Storage;
using EchoRelay.Core.Server.Storage.Filesystem;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EchoRelay
{
    public partial class MainWindow : Form
    {
        #region Properties
        /// <summary>
        /// The original window title set for the app, to be restored if changed, e.g. when appending annotation for unsaved changes, then saving.
        /// </summary>
        private string OriginalWindowTitle { get; }
        /// <summary>
        /// The file path which the <see cref="AppSettings"/> are loaded from.
        /// </summary>
        public string SettingsFilePath { get; }
        /// <summary>
        /// The application settings loaded from the <see cref="SettingsFilePath"/>.
        /// </summary>
        public AppSettings Settings { get; }

        /// <summary>
        /// The websocket server used to power central game services.
        /// </summary>
        public Server Server { get; set; }

        /// <summary>
        /// The UI editors for different storage resources.
        /// </summary>
        public StorageEditorBase[] StorageEditors { get; }
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            // Append the assembly version to the window title.
            Version? assemblyVersion = this.GetType().Assembly.GetName().Version;
            if (assemblyVersion != null)
                Text += $" v{assemblyVersion.ToString(3)}";

            // Store the original window title
            OriginalWindowTitle = this.Text;

            // Set our settings file path to be within the current directory.
            SettingsFilePath = Path.Join(Environment.CurrentDirectory, "settings.json");

            // Try to load our application settings.
            AppSettings? settings = AppSettings.Load(SettingsFilePath);

            // Validate the settings.
            if (settings == null || !settings.Validate())
            {
                // Show our initial message describing what is about to happen.
                MessageBox.Show("Application settings have not been correctly configured. Please configure them now.", "Echo Relay: Settings");

                // If the settings weren't initialized, do so with some default values.
                settings ??= new AppSettings(port: 777);

                // Display our settings dialog and expect an OK result (meaning the settings were saved, the dialog was not simply closed).
                SettingsDialog settingsDialog = new SettingsDialog(settings, SettingsFilePath);
                if (settingsDialog.ShowDialog() != DialogResult.OK)
                {
                    // Close this window and return.
                    Environment.Exit(1);
                    return;
                }
            }

            // Set our loaded/created settings
            Settings = settings;

            // Create our file system storage and open it.
            ServerStorage serverStorage = new FilesystemServerStorage(Settings.FilesystemDatabaseDirectory!);
            serverStorage.Open();

            // Perform initial deployment
            bool allCriticalResourcesExist = serverStorage.AccessControlList.Exists() && serverStorage.ChannelInfo.Exists() && serverStorage.LoginSettings.Exists() && serverStorage.SymbolCache.Exists();
            bool anyCriticalResourcesExist = serverStorage.AccessControlList.Exists() || serverStorage.ChannelInfo.Exists() || serverStorage.LoginSettings.Exists() || serverStorage.SymbolCache.Exists();
            bool performInitialSetup = !allCriticalResourcesExist;
            if (performInitialSetup && anyCriticalResourcesExist)
            {
                performInitialSetup = MessageBox.Show("Critical resources are missing from storage, but storage is non-empty.\n" +
                    "Would you like to re-deploy initial setup resources? Warning: this will clear all storage except accounts!", "Redeployment", MessageBoxButtons.YesNo) == DialogResult.Yes;
            }
            if (performInitialSetup)
                InitialDeployment.PerformInitialDeployment(serverStorage, Settings.GameExecutableDirectory, false);

            // Create our list of storage editors and set the storage for each.
            StorageEditors = new StorageEditorBase[] { accessControlListEditor, accountSelector, channelInfoEditor, loginSettingsEditor };
            foreach (StorageEditorBase storageEditor in StorageEditors)
            {
                storageEditor.Storage = serverStorage;
                storageEditor.OnUnsavedChangesStateChange += StorageEditor_OnUnsavedChangesStateChange;
            }

            // Create a server instance and set up our event handlers
            Server = new Server(serverStorage,
                new ServerSettings(
                    port: Settings.Port,
                    serverDbApiKey: Settings.ServerDBApiKey,
                    serverDBValidateServerEndpoint: Settings.ServerDBValidateGameServers ?? false,
                    serverDBValidateServerEndpointTimeout: Settings.ServerDBValidateGameServersTimeout ?? 3000,
                    favorPopulationOverPing: Settings.MatchingPopulationOverPing,
                    forceIntoAnySessionIfCreationFails: Settings.MatchingForceIntoAnySessionOnFailure
                    )
                );
            Server.OnServerStarted += Server_OnServerStarted;
            Server.OnServerStopped += Server_OnServerStopped;
            Server.OnAuthorizationResult += Server_OnAuthorizationResult;
            Server.OnServicePeerConnected += Server_OnServicePeerConnected;
            Server.OnServicePeerDisconnected += Server_OnServicePeerDisconnected;
            Server.OnServicePeerAuthenticated += Server_OnServicePeerAuthenticated;
            Server.OnServicePacketSent += Server_OnServicePacketSent;
            Server.OnServicePacketReceived += Server_OnServicePacketReceived;
            Server.ServerDBService.Registry.OnGameServerRegistered += Registry_OnGameServerRegistered;
            Server.ServerDBService.Registry.OnGameServerUnregistered += Registry_OnGameServerUnregistered;
            Server.ServerDBService.OnGameServerRegistrationFailure += ServerDBService_OnGameServerRegistrationFailure; ;
        }
        #endregion

        #region Functions
        private string getPacketDisplayString(Packet packet)
        {
            string result = "";
            foreach (var message in packet)
            {
                result += $"\t{message}\n";
            }
            return result;
        }
        #endregion

        #region Event Handlers
        private async void Form1_Load(object sender, EventArgs e)
        {
            // Start the server if it is configured to start on startup.
            if (Settings.StartServerOnStartup)
                await Server.Start();
        }

        private void Server_OnServerStarted(Server server)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                btnToggleRunningState.Checked = true;
                btnToggleRunningState.Image = Resources.stop_button_icon;
                lblStatus.Text = "Running";
                startServerToolStripMenuItem.Text = "Stop server";
                progressBarStatus.Style = ProgressBarStyle.Marquee;
                serverInfoControl.UpdateServerInfo(Server, true);
            });
        }
        private void Server_OnServerStopped(Server server)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Reset our UI state
                btnToggleRunningState.Checked = false;
                btnToggleRunningState.Image = Resources.play_button_icon;
                lblStatus.Text = "Idle";
                startServerToolStripMenuItem.Text = "Start server";
                progressBarStatus.Style = ProgressBarStyle.Continuous;
                serverInfoControl.UpdateServerInfo(null, true);
            });
        }

        private void Server_OnAuthorizationResult(Server server, IPEndPoint client, bool authorized)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Add to our log
                if (!authorized)
                    AppendLogText($"[SERVER] client({client.Address}:{client.Port}) failed authorization\n");
            });
        }

        private async void btnToggleRunningState_Click(object sender, EventArgs e)
        {
            // If the server is running
            if (Server.Running)
                Server.Stop();
            else
                await Server.Start();
        }

        private void StorageEditor_OnUnsavedChangesStateChange(StorageEditorBase storageEditor, bool hasUnsavedChanges)
        {
            RefreshUnsavedChangesState();
        }

        private void RefreshUnsavedChangesState()
        {
            // Evaluate whether there are unsaved changes.
            bool changed = false;
            foreach (StorageEditorBase storageEditor in StorageEditors)
            {
                changed |= storageEditor.Changed;
            }

            // Update the window title accordingly.
            if (changed)
                this.Text = OriginalWindowTitle + " [unsaved changes]**";
            else
                this.Text = OriginalWindowTitle;
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            // Save changes in all editors
            foreach (StorageEditorBase storageEditor in StorageEditors)
                storageEditor.SaveChanges();

            // Refresh our window indicators for unsaved changes.
            RefreshUnsavedChangesState();
        }

        private void btnRevertChanges_Click(object sender, EventArgs e)
        {
            // Provide a confirmation window.
            if (MessageBox.Show("You are about to undo all unsaved changes in all editors. Would you like to continue?", "Echo Relay: Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Revert changes in all editors
                foreach (StorageEditorBase storageEditor in StorageEditors)
                    storageEditor.RevertChanges();

                // Refresh our window indicators for unsaved changes.
                RefreshUnsavedChangesState();
            }
        }

        private void Server_OnServicePeerConnected(Service service, Peer peer)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Add to our log
                AppendLogText($"[{service.Name}] client({peer.Address}:{peer.Port}) connected\n");

                // Update server info
                serverInfoControl.UpdateServerInfo(Server, false);

                // Add our peer to the peers list
                peerConnectionsControl.AddOrUpdatePeer(peer);

                // Update peer count on the peers tab.
                tabPeers.Text = $"Peers ({peerConnectionsControl.PeerCount})";
            });
        }

        private void Server_OnServicePeerDisconnected(Service service, Peer peer)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Add to our log
                AppendLogText($"[{service.Name}] client({peer.Address}:{peer.Port}) disconnected\n");

                // Update server info
                serverInfoControl.UpdateServerInfo(Server, false);

                // Remove a peer from the peers list.
                peerConnectionsControl.RemovePeer(peer);

                // Update peer count on the peers tab.
                tabPeers.Text = peerConnectionsControl.PeerCount > 0 ? $"Peers ({peerConnectionsControl.PeerCount})" : "Peers";
            });
        }

        private void Server_OnServicePeerAuthenticated(Service service, Peer peer, XPlatformId userId)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Update our peer.
                peerConnectionsControl.AddOrUpdatePeer(peer);
            });
        }

        private void Server_OnServicePacketReceived(EchoRelay.Core.Server.Services.Service service, EchoRelay.Core.Server.Services.Peer sender, EchoRelay.Core.Server.Messages.Packet packet)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Add to our log
                AppendLogText($"[{service.Name}] client({sender.Address}:{sender.Port})->server:\n" + getPacketDisplayString(packet));
            });
        }

        private void Server_OnServicePacketSent(EchoRelay.Core.Server.Services.Service service, EchoRelay.Core.Server.Services.Peer sender, EchoRelay.Core.Server.Messages.Packet packet)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Add to our log
                AppendLogText($"[{service.Name}] server->client({sender.Address}:{sender.Port}\n" + getPacketDisplayString(packet));
            });
        }
        private void AppendLogText(string text)
        {
            // Check if the cursor is at the end of the textbox
            bool endSelected = rtbLog.SelectionStart >= text.Length - 1;

            // Ensure the log buffer does not exceed the given number of lines.
            int maxLineCount = 200;
            if (rtbLog.Lines.Length > maxLineCount)
            {
                // Obtain the line start and length.
                int startLineCharIndex = rtbLog.GetFirstCharIndexFromLine(0);
                int endLineCharIndex = rtbLog.GetFirstCharIndexFromLine(rtbLog.Lines.Length - maxLineCount);
                rtbLog.Text = rtbLog.Text.Remove(startLineCharIndex, endLineCharIndex - startLineCharIndex);
            }

            // Append our log text
            rtbLog.AppendText(text);

            // Scroll to the end again if the end was previously selected
            if (endSelected)
            {
                rtbLog.SelectionStart = rtbLog.Text.Length;
                rtbLog.ScrollToCaret();
            }
        }

        private void Registry_OnGameServerRegistered(EchoRelay.Core.Server.Services.ServerDB.RegisteredGameServer gameServer)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Unregister any existing events
                gameServer.OnPlayersAdded += GameServer_OnPlayersAdded;
                gameServer.OnPlayerRemoved += GameServer_OnPlayerRemoved;
                gameServer.OnSessionStateChanged += GameServer_OnSessionStateChanged;

                // Register our event handlers for any newly registered server.
                gameServer.OnPlayersAdded += GameServer_OnPlayersAdded;
                gameServer.OnPlayerRemoved += GameServer_OnPlayerRemoved;
                gameServer.OnSessionStateChanged += GameServer_OnSessionStateChanged;

                // Add the game server to our UI control
                gameServersControl.AddOrUpdateGameServer(gameServer);

                // Update server count on the game server tab.
                tabGameServers.Text = $"Game Servers ({gameServersControl.GameServerCount})";

                AppendLogText($"[{gameServer.Peer.Service.Name}] client({gameServer.Peer.Address}:{gameServer.Peer.Port}) registered game server (server_id={gameServer.ServerId}, region_symbol={gameServer.RegionSymbol}, version_lock={gameServer.VersionLock}, endpoint=<{gameServer.ExternalAddress}:{gameServer.Port}>)\n");
            });
        }

        private void Registry_OnGameServerUnregistered(EchoRelay.Core.Server.Services.ServerDB.RegisteredGameServer gameServer)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Remove the game server from our UI control
                gameServersControl.RemoveGameServer(gameServer);

                // Update server count on the game server tab.
                tabGameServers.Text = gameServersControl.GameServerCount > 0 ? $"Game Servers ({gameServersControl.GameServerCount})" : "Game Servers";

                // Add to our log
                AppendLogText($"[{gameServer.Peer.Service.Name}] client({gameServer.Peer.Address}:{gameServer.Peer.Port}) unregistered game server (server_id={gameServer.ServerId}, region_symbol={gameServer.RegionSymbol}, version_lock={gameServer.VersionLock}, endpoint=<{gameServer.ExternalAddress}:{gameServer.Port}>)\n");
            });
        }

        private void ServerDBService_OnGameServerRegistrationFailure(Peer peer, Core.Server.Messages.ServerDB.ERGameServerRegistrationRequest registrationRequest, string failureMessage)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Add to our log
                AppendLogText($"[{peer.Service.Name}] client({peer.Address}:{peer.Port}) failed to register game server: \"{failureMessage}\"\n");
            });
        }

        private void GameServer_OnPlayersAdded(EchoRelay.Core.Server.Services.ServerDB.RegisteredGameServer gameServer, (Guid playerSession, Peer? peer)[] players)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Update the state of the game server in our UI control
                gameServersControl.AddOrUpdateGameServer(gameServer);
            });
        }

        private void GameServer_OnPlayerRemoved(EchoRelay.Core.Server.Services.ServerDB.RegisteredGameServer gameServer, Guid playerSession, Peer? peer)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Update the state of the game server in our UI control
                gameServersControl.AddOrUpdateGameServer(gameServer);
            });
        }

        private void GameServer_OnSessionStateChanged(EchoRelay.Core.Server.Services.ServerDB.RegisteredGameServer gameServer)
        {
            // Invoke the UI thread to perform updates.
            this.InvokeUIThread(() =>
            {
                // Update the state of the game server in our UI control
                gameServersControl.AddOrUpdateGameServer(gameServer);
            });
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Begin closing this window.
            Close();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop any server operations.
            Server.Stop();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("For personal education/research purposes only.");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Display our settings dialog. If the user saved settings, we warn the application will be restarted.
            SettingsDialog settingsDialog = new SettingsDialog(Settings, SettingsFilePath);
            if (settingsDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Application settings have been changed, the application will now restart.");
                Application.Restart();
                return;
            }
        }

        private void serverHeadlessThrottledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Launch game as a headless, no OVR server with the default timestep.
            GameLauncher.Launch(Settings.GameExecutableFilePath, GameLauncher.LaunchRole.Server, noOVR: true, headless: true);
        }

        private void serverheadlessUnthrottledHighCPUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Launch game as a headless, no OVR server with an unthrottled timestep (zero).
            GameLauncher.Launch(Settings.GameExecutableFilePath, GameLauncher.LaunchRole.Server, noOVR: true, headless: true, timeStep: 0);
        }

        private void serverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Launch game as a no OVR server.
            GameLauncher.Launch(Settings.GameExecutableFilePath, GameLauncher.LaunchRole.Server, noOVR: true);
        }

        private void clientWindowedNoOVRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Launch game as a windowed no OVR client.
            GameLauncher.Launch(Settings.GameExecutableFilePath, GameLauncher.LaunchRole.Client, windowed: true, noOVR: true);
        }

        private void clientWindowedOVRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Launch game as a windowed OVR client.
            GameLauncher.Launch(Settings.GameExecutableFilePath, GameLauncher.LaunchRole.Client, windowed: true);
        }

        private void clientOVRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Launch game as a VR OVR client.
            GameLauncher.Launch(Settings.GameExecutableFilePath, GameLauncher.LaunchRole.Client);
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a game launcher dialog and show it
            GameLauncherDialog gameLauncherDialog = new GameLauncherDialog(Settings);
            gameLauncherDialog.ShowDialog();
        }

        private void controlRuntimeGeneral_Load(object sender, EventArgs e)
        {

        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbLog.Text = "";
        }
        #endregion
    }
}
