using EchoRelay.App.Utils;
using EchoRelay.Core.Server.Storage;
using EchoRelay.Core.Server.Storage.Types;

namespace EchoRelay.App.Forms.Controls
{
    public partial class AccessControlListEditor : StorageEditorBase
    {
        private AccessControlListResource? _accessControlList;
        public AccessControlListEditor()
        {
            InitializeComponent();
        }

        private void RefreshACL()
        {
            allowRulesEditor.RuleSet = _accessControlList?.AllowRules.ToArray() ?? Array.Empty<string>();
            disallowRulesEditor.RuleSet = _accessControlList?.DisallowRules.ToArray() ?? Array.Empty<string>();
            Changed = false;
        }

        protected override void OnStorageLoaded(ServerStorage storage)
        {
            // Subscribe to ACL changes
            storage.AccessControlList.OnChanged += AccessControlList_OnChanged;

            // Load the ACL
            _accessControlList = storage.AccessControlList.Get();

            // Refresh UI changes
            RefreshACL();
        }


        protected override void OnStorageUnloaded(ServerStorage storage)
        {
            // Unsubscribe from ACL changes
            storage.AccessControlList.OnChanged -= AccessControlList_OnChanged;

            // Unload the ACL
            _accessControlList = null;

            // Refresh UI changes
        }

        private void AccessControlList_OnChanged(ServerStorage storage, AccessControlListResource resource, StorageChangeType changeType)
        {
            this.InvokeUIThread(() =>
            {
                if (changeType == StorageChangeType.Set)
                    _accessControlList = resource;
                else if (changeType == StorageChangeType.Deleted)
                    _accessControlList = null;

                RefreshACL();
            });
        }

        public override void SaveChanges()
        {
            // If there are no changes, stop
            if (!Changed || _accessControlList == null)
                return;

            // Update our rule sets
            _accessControlList.AllowRules = new HashSet<string>(allowRulesEditor.RuleSet);
            _accessControlList.DisallowRules = new HashSet<string>(disallowRulesEditor.RuleSet);
            Storage?.AccessControlList.Set(_accessControlList);
            Changed = false;
        }

        public override void RevertChanges()
        {
            RefreshACL();
        }

        private void rulesEditor_RuleSetChanged(object sender, EventArgs e)
        {
            Changed = true;
        }
    }
}
