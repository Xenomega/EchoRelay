using EchoRelay.App.Utils;
using EchoRelay.Core.Server.Storage;
using EchoRelay.Core.Server.Storage.Types;

namespace EchoRelay.App.Forms.Controls
{
    public partial class LoginSettingsEditor : StorageEditorBase
    {
        /// <summary>
        /// The login settings the UI control should bind to.
        /// </summary>
        private LoginSettingsResource? _settings;

        public LoginSettingsEditor()
        {
            InitializeComponent();
        }


        protected override void OnStorageLoaded(ServerStorage storage)
        {
            // Subscribe to login settings changes
            storage.LoginSettings.OnChanged += LoginSettings_OnChanged;

            // Load the login settings resource
            _settings = storage.LoginSettings.Get();

            // Refresh UI changes
            RevertChanges();
        }


        protected override void OnStorageUnloaded(ServerStorage storage)
        {
            // Unsubscribe from login settings changes
            storage.LoginSettings.OnChanged -= LoginSettings_OnChanged;

            // Unload the login settings resource
            _settings = null;

            // Refresh UI changes
            RevertChanges();
        }

        private void LoginSettings_OnChanged(ServerStorage storage, LoginSettingsResource resource, StorageChangeType changeType)
        {
            this.InvokeUIThread(() =>
            {
                if (changeType == StorageChangeType.Set)
                    _settings = resource;
                else if (changeType == StorageChangeType.Deleted)
                    _settings = null;

                RevertChanges();
            });
        }

        public override void SaveChanges()
        {
            // Verify settings were set.
            if (_settings == null || !Changed)
                return;

            // Update the properties of the settings.
            _settings.Environment = txtEnvironment.Text;
            _settings.IAPUnlocked = chkIAPUnlocked.Checked;
            _settings.RemoteLogSocial = chkRemoteLogSocial.Checked;
            _settings.RemoteLogWarnings = chkRemoteLogWarnings.Checked;
            _settings.RemoteLogErrors = chkRemoteLogErrors.Checked;
            _settings.RemoteLogRichPresence = chkRemoteLogRichPresence.Checked;
            _settings.RemoteLogMetrics = chkRemoteLogMetrics.Checked;

            // Update the login settings in storage, if storage was provided.
            Storage?.LoginSettings.Set(_settings);
            Changed = false;
        }

        public override void RevertChanges()
        {
            // Update the UI elements from settings properties.
            txtEnvironment.Text = _settings?.Environment ?? "";
            chkIAPUnlocked.Checked = _settings?.IAPUnlocked ?? false;
            chkRemoteLogSocial.Checked = _settings?.RemoteLogSocial ?? false;
            chkRemoteLogWarnings.Checked = _settings?.RemoteLogWarnings ?? false;
            chkRemoteLogErrors.Checked = _settings?.RemoteLogErrors ?? false;
            chkRemoteLogRichPresence.Checked = _settings?.RemoteLogRichPresence ?? false;
            chkRemoteLogMetrics.Checked = _settings?.RemoteLogMetrics ?? false;

            Changed = false;
        }

        private void onAnyValueChanged(object sender, EventArgs e)
        {
            Changed = true;
        }
    }
}
