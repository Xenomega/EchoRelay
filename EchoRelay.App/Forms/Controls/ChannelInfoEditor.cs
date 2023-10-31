using EchoRelay.App.Utils;
using EchoRelay.Core.Server.Storage;
using EchoRelay.Core.Server.Storage.Types;

namespace EchoRelay.App.Forms.Controls
{
    public partial class ChannelInfoEditor : StorageEditorBase
    {
        /// <summary>
        /// The channel info the UI control should bind to.
        /// </summary>
        private ChannelInfoResource? _channelInfo;
        private int _lastIndex = -1;

        public ChannelInfoEditor()
        {
            InitializeComponent();
        }

        private void RefreshChannelInfo()
        {
            // Reset our state
            Changed = false;
            _lastIndex = -1;

            // Update the UI elements from channel info properties.
            comboBoxChannel.Items.Clear();
            if (_channelInfo != null)
            {
                foreach (ChannelInfoResource.Channel channel in _channelInfo.Group)
                {
                    comboBoxChannel.Items.Add(channel.ChannelUUID);
                }
            }

            // Select the item to trigger its refreshing.
            comboBoxChannel.SelectedIndex = Math.Min(0, comboBoxChannel.Items.Count - 1);
        }

        protected override void OnStorageLoaded(ServerStorage storage)
        {
            // Subscribe to channel info changes
            storage.ChannelInfo.OnChanged += ChannelInfo_OnChanged;

            // Load the channel info resource
            _channelInfo = storage.ChannelInfo.Get();

            // Refresh UI changes
            RefreshChannelInfo();
        }


        protected override void OnStorageUnloaded(ServerStorage storage)
        {
            // Unsubscribe from channel info changes
            storage.ChannelInfo.OnChanged -= ChannelInfo_OnChanged;

            // Unload the channel info resource
            _channelInfo = null;

            // Refresh UI changes
        }

        private void ChannelInfo_OnChanged(ServerStorage storage, ChannelInfoResource resource, StorageChangeType changeType)
        {
            this.InvokeUIThread(() =>
            {
                if (changeType == StorageChangeType.Set)
                    _channelInfo = resource;
                else if (changeType == StorageChangeType.Deleted)
                    _channelInfo = null;

                RefreshChannelInfo();
            });
        }

        public override void SaveChanges()
        {
            // Verify channel info changes were made.
            if (_channelInfo == null || comboBoxChannel.SelectedIndex < 0 || !Changed)
                return;

            // Obtain the channel.
            ChannelInfoResource.Channel channel = _channelInfo.Group[comboBoxChannel.SelectedIndex];

            // Update the channel info.
            channel.Name = txtName.Text;
            channel.Description = txtDescription.Text;
            channel.Rules = txtRules.Text;
            channel.RulesVersion = (ulong)numRulesVersion.Value;
            channel.Link = txtLink.Text;

            // Update the channel info in storage, if storage was provided.
            Storage?.ChannelInfo.Set(_channelInfo);
            Changed = false;
        }

        public override void RevertChanges()
        {
            // Update the UI elements from channel info properties.
            if (_channelInfo == null || comboBoxChannel.SelectedIndex == -1)
            {
                txtName.Text = "";
                txtDescription.Text = "";
                txtRules.Text = "";
                numRulesVersion.Value = 0;
                txtLink.Text = "";
            }
            else
            {
                // Obtain the channel
                ChannelInfoResource.Channel channel = _channelInfo.Group[comboBoxChannel.SelectedIndex];
                txtName.Text = channel.Name;
                txtDescription.Text = channel.Description;
                txtRules.Text = channel.Rules;
                numRulesVersion.Value = channel.RulesVersion;
                txtLink.Text = channel.Link;
            }
            Changed = false;
        }

        private void comboBoxChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If we have changes on this current channel, but the user is trying to view another channel, ask them to save changes.
            // If they wish to, we restore the selected index from before, if not, we allow it to change.
            if (Changed)
            {
                // If we just rolled back to the index we restored, do nothing (do not revert changes, leave them as they are, we don't want to reset any UI value state).
                if (comboBoxChannel.SelectedIndex == _lastIndex)
                    return;

                // Warn the user if they'd like to proceed. If they do, we simply set our index and reload changes. If they don't, we restore our previous selected item (the if statement above will avoid reloading changes
                // when this event is triggered again).
                if (MessageBox.Show("Unsaved changes for the current channel will be lost if you proceed without saving. Would you like to proceed?", "Echo Relay: Warning", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    comboBoxChannel.SelectedIndex = _lastIndex;
                    return;
                }
            }

            // Store the last selected index so the mechanism used to cancel channel item selection when you have unsaved changes can avoid reloading values when the index is reset to this value.
            _lastIndex = comboBoxChannel.SelectedIndex;

            // Reload changes for the selected item.
            RevertChanges();
        }

        private void onAnyValueChanged(object sender, EventArgs e)
        {
            Changed = true;
        }
    }
}
