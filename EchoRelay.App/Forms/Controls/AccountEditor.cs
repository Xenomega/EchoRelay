using EchoRelay.Core.Server.Storage.Types;

namespace EchoRelay.App.Forms.Controls
{
    public partial class AccountEditor : StorageEditorBase
    {
        private AccountResource? _account;
        public AccountResource? Account
        {
            get
            {
                return _account;
            }
            set
            {
                // Check if the account is the same
                bool sameAccount = value?.Key() == _account?.Key();

                // Update the account.
                _account = value;

                // If it's the same account, we don't refresh UI, but only set the resource above.
                // This lets changes be retained even if the account is updated by the server, but they will
                // be saved over the new account object.
                if (!sameAccount)
                    RefreshUI();
            }
        }
        public AccountEditor()
        {
            InitializeComponent();
            chkBanned.CheckedChanged += chkBanned_CheckedChanged;
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (Account == null)
            {
                txtUserId.Text = "";
                txtDisplayName.Text = "";
                txtDisplayName.ReadOnly = true;
                chkBanned.Checked = false;
                bannedUntilDatePicker.Value = DateTime.UtcNow;
                chkModerator.Checked = false;
                chkDisableAFKTimeout.Checked = false;
            }
            else
            {
                txtUserId.Text = Account.AccountIdentifier.ToString();
                txtDisplayName.Text = Account.Profile.Server.DisplayName;
                txtDisplayName.ReadOnly = false;
                chkBanned.Checked = Account.Banned;
                if (chkBanned.Checked)
                    bannedUntilDatePicker.Value = Account.BannedUntil!.Value;
                chkModerator.Checked = Account.IsModerator;
                chkDisableAFKTimeout.Checked = Account.Profile.Server.Developer?.DisableAfkTimeout ?? false;
            }
            Changed = false;
        }

        public override void SaveChanges()
        {
            // If we have no account, there are no changes to be made
            if (Account == null) return;

            // Validate the display name.
            if (txtDisplayName.Text.Length == 0)
            {
                MessageBox.Show("Account display name cannot be saved as a blank name. Account changes have not been saved.");
                return;
            }

            // Update the fields in our account resource
            Account.Profile.SetDisplayName(txtDisplayName.Text);

            // Set our privileges
            Account.IsModerator = chkModerator.Checked;
            Account.BannedUntil = chkBanned.Checked ? bannedUntilDatePicker.Value : null;

            // Set the AFK timeout field in developer settings.
            if (chkDisableAFKTimeout.Checked)
                Account.Profile.Server.Developer = new AccountResource.AccountServerProfile.DeveloperSettings(accountId: Account.AccountIdentifier, disableAfkTimeout: true);
            else
                Account.Profile.Server.Developer = null;

            // Set our account if it is non-null and we have changes
            if (Changed && Account != null)
                Storage?.Accounts.Set(Account);
            Changed = false;
        }
        public override void RevertChanges()
        {
            RefreshUI();
        }

        private void chkBanned_CheckedChanged(object? sender, EventArgs e)
        {
            bannedUntilDatePicker.Enabled = Account != null && chkBanned.Checked;
        }

        private void onAnyValueChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void btnClearAccountLock_Click(object sender, EventArgs e)
        {
            // If we have no account, there are no changes to be made
            if (Account == null) return;

            // Clear the account lock and mark the account changed, if requestd
            if (MessageBox.Show("This will unlock the account, making it available to be locked with a new password. Would you like to continue?", "Echo Relay: Warning",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Account.ClearAccountLock();
                Changed = true;
            }
        }
    }
}
