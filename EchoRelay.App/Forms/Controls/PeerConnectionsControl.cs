using EchoRelay.Core.Server.Services;
using System.Globalization;

namespace EchoRelay.App.Forms.Controls
{
    public partial class PeerConnectionsControl : UserControl
    {
        /// <summary>
        /// A lookup of <see cref="Peer.Id"/> to <see cref="ListViewItem"/>s used in this control.
        /// </summary>
        private Dictionary<string, ListViewItem> _items;

        /// <summary>
        /// The total amount of peer connections across all services.
        /// </summary>
        public int PeerCount
        {
            get
            {
                return listPeers.Items.Count;
            }
        }

        public PeerConnectionsControl()
        {
            InitializeComponent();
            _items = new Dictionary<string, ListViewItem>();
        }

        public void AddOrUpdatePeer(Peer peer)
        {
            // If the peer isn't connected, this might've been triggered out of order, do nothing.
            if (!peer.Connected)
                return;

            // Obtain an existing list view item for this peer, or create one.
            ListViewItem? listItem = null;
            if (!_items.TryGetValue(peer.Id, out listItem))
            {
                listItem = new ListViewItem();
                listItem.SubItems.Add("");
                listItem.SubItems.Add("");
                listItem.SubItems.Add("");
                listItem.SubItems.Add("");
                listItem.SubItems.Add("");
                listPeers.Items.Add(listItem);
                _items[peer.Id] = listItem;
            }

            // Update the subitems of the list.
            listItem.SubItems[0].Text = peer.Service.Name;
            listItem.SubItems[1].Text = peer.Address.ToString();
            listItem.SubItems[2].Text = peer.Port.ToString();
            listItem.SubItems[3].Text = peer.UserId?.ToString() ?? "-";
            listItem.SubItems[4].Text = peer.UserDisplayName?.ToString() ?? "-";
            listItem.SubItems[5].Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
        }

        public void RemovePeer(Peer peer)
        {
            // If we have a list item for this peer, remove it.
            if (_items.TryGetValue(peer.Id, out var listItem))
            {
                listPeers.Items.Remove(listItem!);
                _items.Remove(peer.Id);
            }
        }
    }
}
