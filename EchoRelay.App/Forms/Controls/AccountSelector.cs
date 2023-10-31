using EchoRelay.App.Utils;
using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Storage;
using EchoRelay.Core.Server.Storage.Types;

namespace EchoRelay.App.Forms.Controls
{
    public partial class AccountSelector : StorageEditorBase
    {
        public AccountEditor AccountEditor
        {
            get { return accountEditor; }
        }
        private Dictionary<XPlatformId, ListViewItem> _items;

        public AccountSelector()
        {
            InitializeComponent();
            _items = new Dictionary<XPlatformId, ListViewItem>();
            accountEditor.OnUnsavedChangesStateChange += AccountEditor_OnUnsavedChangesStateChange;
        }

        private void AccountEditor_OnUnsavedChangesStateChange(StorageEditorBase storageEditor, bool hasUnsavedChanges)
        {
            // Forward the changed state from the editor to the selector, so state/events propagate upwards.
            Changed = hasUnsavedChanges;
        }

        private void UpdateItem(XPlatformId accountId)
        {
            // If we don't have an item for this account, create one.
            if (!_items.TryGetValue(accountId, out _))
            {
                ListViewItem item = new ListViewItem(accountId.ToString());
                item.Tag = accountId;
                listAccounts.Items.Add(item);
                _items[accountId] = item;
            }
        }

        private void DeleteItem(XPlatformId accountId)
        {
            // If we don't have an item for this account, create one.
            if (_items.TryGetValue(accountId, out ListViewItem? listItem))
            {
                listAccounts.Items.Remove(listItem);
                _items.Remove(accountId);
            }
        }

        protected override void OnStorageLoaded(ServerStorage storage)
        {
            // Set the storage for our child editor control
            accountEditor.Storage = storage;

            // Subscribe to account changes
            storage.Accounts.OnChanged += Accounts_OnChanged;

            // Clear our list.
            listAccounts.Items.Clear();

            // Verify our storage is not null
            if (Storage == null)
                return;

            // Obtain the list of user accounts.
            XPlatformId[] accountIds = Storage.Accounts.Keys() ?? Array.Empty<XPlatformId>();

            // Load our lookup with it.
            listAccounts.Items.Clear();
            foreach (XPlatformId accountId in accountIds)
                UpdateItem(accountId);

            // If no item is selected, select the first item, if available.
            if(listAccounts.SelectedItems.Count == 0 && listAccounts.Items.Count > 0)
            {
                listAccounts.Items[0].Selected = true;
                listAccounts.Select();
            }
        }

        protected override void OnStorageUnloaded(ServerStorage storage)
        {
            // Unsubscribe from account changes
            storage.Accounts.OnChanged -= Accounts_OnChanged;

            // Update UI
            listAccounts.Items.Clear();
            _items.Clear();
            accountEditor.Account = null;
        }

        public override void SaveChanges()
        {
            accountEditor.SaveChanges();
        }

        public override void RevertChanges()
        {
            accountEditor.RevertChanges();
        }



        private void Accounts_OnChanged(ServerStorage storage, AccountResource resource, StorageChangeType changeType)
        {
            this.InvokeUIThread(() =>
            {
                // Update the UI with our change.
                if (changeType == StorageChangeType.Set)
                    UpdateItem(resource.Key());
                else if (changeType == StorageChangeType.Deleted)
                    DeleteItem(resource.Key());

                // If the account changed was the one selected, update the account editor's account reference.
                if (listAccounts.SelectedItems.Count > 0 && (XPlatformId)listAccounts.SelectedItems[0].Tag == resource.Key())
                    accountEditor.Account = resource;
            });
        }

        private void listAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listAccounts.SelectedItems.Count <= 0)
                accountEditor.Account = null;
            else
                accountEditor.Account = Storage?.Accounts.Get((XPlatformId)listAccounts.SelectedItems[0].Tag);
        }
    }
}
