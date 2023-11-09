using EchoRelay.Core.Server.Services;
using EchoRelay.Core.Server.Services.ServerDB;

namespace EchoRelay.App.Forms.Controls
{
    public partial class GameServersControl : UserControl
    {
        /// <summary>
        /// A lookup of <see cref="RegisteredGameServer.ServerId"/> to <see cref="ListViewItem"/>s used in this control.
        /// </summary>
        private Dictionary<ulong, ListViewItem> _items;

        /// <summary>
        /// The total amount of peer connections across all services.
        /// </summary>hold on
        public int GameServerCount
        {
            get
            {
                return listGameServers.Items.Count;
            }
        }

        public GameServersControl()
        {
            InitializeComponent();
            _items = new Dictionary<ulong, ListViewItem>();
        }

        public void AddOrUpdateGameServer(RegisteredGameServer gameServer)
        {
            // If the peer isn't connected, this might've been triggered out of order, do nothing.
            if (!gameServer.Peer.Connected)
                return;

            // Obtain an existing list view item for this game server, or create one.
            ListViewItem? listItem = null;
            if (!_items.TryGetValue(gameServer.ServerId, out listItem))
            {
                listItem = new ListViewItem();
                for (int i = 0; i < 9; i++)
                    listItem.SubItems.Add("");
                listGameServers.Items.Add(listItem);
                _items[gameServer.ServerId] = listItem;
            }

            // Update the subitems of the list.
            listItem.SubItems[0].Text = gameServer.ServerId.ToString();
            listItem.SubItems[1].Text = gameServer.ExternalAddress.ToString();
            listItem.SubItems[2].Text = gameServer.Port.ToString();
            if (gameServer.SessionGameTypeSymbol != null)
            {
                listItem.SubItems[3].Text = gameServer.Peer.Service.Server.SymbolCache.GetName(gameServer.SessionGameTypeSymbol.Value) ?? $"unknown({gameServer.SessionGameTypeSymbol.Value})";
            }
            else
            {
                listItem.SubItems[3].Text = "-";
            }
            if (gameServer.SessionLevelSymbol != null)
            {
                listItem.SubItems[4].Text = gameServer.Peer.Service.Server.SymbolCache.GetName(gameServer.SessionLevelSymbol.Value) ?? $"unknown({gameServer.SessionLevelSymbol.Value})";
            }
            else
            {
                listItem.SubItems[4].Text = "-";
            }
            listItem.SubItems[5].Text = $"{gameServer.SessionPlayerCount}/{gameServer.SessionPlayerLimits.TotalPlayerLimit}";
            listItem.SubItems[6].Text = gameServer.SessionLobbyType.ToString();
            listItem.SubItems[7].Text = gameServer.SessionLocked.ToString();
            listItem.SubItems[8].Text = gameServer.SessionChannel?.ToString() ?? "-";
            listItem.SubItems[9].Text = gameServer.SessionId?.ToString() ?? "-";
            listItem.Tag = gameServer;

            // Update the selected item
            RefreshSelectedGameServer();
        }

        public void RemoveGameServer(RegisteredGameServer gameServer)
        {
            // If we have a list item for this game server, remove it.
            if (_items.TryGetValue(gameServer.ServerId, out var listItem))
            {
                listGameServers.Items.Remove(listItem!);
                _items.Remove(gameServer.ServerId);
            }
        }

        private void listGameServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Refresh the game server UI
            RefreshSelectedGameServer();

            // If we have no selected items, remove the context menu. Otherwise, set it appropriately.
            listGameServers.ContextMenuStrip = listGameServers.SelectedItems.Count > 0 ? contextMenuGameServers : null;
        }

        private async void RefreshSelectedGameServer()
        {
            // Obtain the selected item, if any.
            ListViewItem? selectedItem = null;
            if (listGameServers.SelectedItems.Count > 0)
                selectedItem = listGameServers.SelectedItems[0];

            // Clear our players table information.
            listPlayers.Items.Clear();

            // Obtain the game server associated with the list item
            if (selectedItem != null)
            {
                // Create items for every player in the game server.
                RegisteredGameServer selectedGameServer = (RegisteredGameServer)selectedItem.Tag;
                var playersInfo = await selectedGameServer.GetPlayers();
                foreach (var playerInfo in playersInfo)
                {
                    // Create a list item for this player
                    ListViewItem playerListItem = new ListViewItem(playerInfo.Peer?.UserId?.ToString() ?? "-");
                    playerListItem.SubItems.Add(playerInfo.Peer?.UserDisplayName?.ToString() ?? "-");
                    playerListItem.SubItems.Add(playerInfo.Peer?.Address.ToString() ?? "-");
                    playerListItem.SubItems.Add(playerInfo.PlayerSession.ToString());

                    // Set the tag as the player session identifier.
                    playerListItem.Tag = playerInfo.PlayerSession;

                    // Add the item to our players list
                    listPlayers.Items.Add(playerListItem);
                }
            }
        }

        private void listPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If we have no selected items, remove the context menu. Otherwise, set it appropriately.
            listPlayers.ContextMenuStrip = listPlayers.SelectedItems.Count > 0 ? contextMenuPlayers : null;
        }

        private async void kickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Obtain the selected game server, if any.
            if (listGameServers.SelectedItems.Count <= 0)
                return;
            RegisteredGameServer selectedGameServer = (RegisteredGameServer)listGameServers.SelectedItems[0].Tag;

            // Loop for all selected players
            foreach (ListViewItem item in listPlayers.SelectedItems)
            {
                // Obtain the player session
                Guid playerSession = (Guid)item.Tag;

                // Try to send a rejection message for this player session to the game server.
                await selectedGameServer.KickPlayer(playerSession);
            }
        }

        private void copySessionLobbyIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Obtain the selected game server, if any.
            if (listGameServers.SelectedItems.Count <= 0)
                return;
            RegisteredGameServer selectedGameServer = (RegisteredGameServer)listGameServers.SelectedItems[0].Tag;

            // Set the copied text if the have a lobby identifier.
            if (selectedGameServer.SessionId != null)
                Clipboard.SetText(selectedGameServer.SessionId.Value.ToString());
        }

        private async void copyUserIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Obtain the selected game server, if any.
            if (listGameServers.SelectedItems.Count <= 0 || listPlayers.SelectedItems.Count <= 0)
                return;
            RegisteredGameServer selectedGameServer = (RegisteredGameServer)listGameServers.SelectedItems[0].Tag;

            // Obtain the selected peer
            Peer? selectedPeer = await selectedGameServer.GetPeer((Guid)listPlayers.SelectedItems[0].Tag);

            // If the field exists, set it to the clipboard.
            if (selectedPeer?.UserId != null)
                Clipboard.SetText(selectedPeer.UserId.ToString());
        }

        private async void copyIPAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Obtain the selected game server, if any.
            if (listGameServers.SelectedItems.Count <= 0 || listPlayers.SelectedItems.Count <= 0)
                return;
            RegisteredGameServer selectedGameServer = (RegisteredGameServer)listGameServers.SelectedItems[0].Tag;

            // Obtain the selected peer
            Peer? selectedPeer = await selectedGameServer.GetPeer((Guid)listPlayers.SelectedItems[0].Tag);

            // If the peer exists, set it to the clipboard.
            if (selectedPeer != null)
                Clipboard.SetText(selectedPeer.Address.ToString());
        }
    }
}
