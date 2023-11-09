using EchoRelay.App.Settings;
using System.Security.Cryptography;

namespace EchoRelay.App.Forms.Dialogs
{
    public partial class SettingsDialog : Form
    {
        /// <summary>
        /// The application settings that are presented for modification.
        /// </summary>
        public AppSettings Settings { get; }
        /// <summary>
        /// The file path to which the <see cref="Settings"/> should be saved.
        /// </summary>
        public string SettingsFilePath { get; }

        /// <summary>
        /// Initializes a new <see cref="SettingsDialog"/>.
        /// </summary>
        /// <param name="settings">The application settings to be presented for modification.</param>
        public SettingsDialog(AppSettings settings, string settingsFilePath)
        {
            InitializeComponent();

            // Load our settings into the UI
            Settings = settings;
            SettingsFilePath = settingsFilePath;

            txtExecutablePath.Text = Settings.GameExecutableFilePath;
            numericTCPPort.Value = Settings.Port;
            if (Settings.FilesystemDatabaseDirectory != null)
                txtDbFolder.Text = Settings.FilesystemDatabaseDirectory;
            chkStartServerOnStartup.Checked = Settings.StartServerOnStartup;
            chkPopulationOverPing.Checked = Settings.MatchingPopulationOverPing;
            chkForceIntoAnySession.Checked = Settings.MatchingForceIntoAnySessionOnFailure;
            chkValidateGameServers.Checked = Settings.ServerDBValidateGameServers ?? false;
            numValidateGameServersTimeout.Value = (int)(Settings?.ServerDBValidateGameServersTimeout ?? numValidateGameServersTimeout.Value);

            // Set the server DB api key
            txtServerDBApiKey.Text = Settings.ServerDBApiKey ?? "";
            chkUseServerDBApiKeys.Checked = !string.IsNullOrEmpty(Settings.ServerDBApiKey);

            // Set the dialog result to cancelled, this way only if we successfully save settings, do we return OK.
            DialogResult = DialogResult.Cancel;
        }

        private void btnOpenGameFolder_Click(object sender, EventArgs e)
        {
            // Create a folder browser dialog to let the user select the game executable.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable Files (.exe)|*.exe";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Set the executable path.
                txtExecutablePath.Text = openFileDialog.FileName;
            }
        }

        private void btnOpenDbFolder_Click(object sender, EventArgs e)
        {
            // Create a folder browser dialog to let the user select the database folder.
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                // Set the folder path.
                txtDbFolder.Text = folderBrowser.SelectedPath;
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            // Validate the settings provided.
            if (!Directory.Exists(txtDbFolder.Text))
            {
                MessageBox.Show("The provided filesystem database folder could not be found.", "Error");
                return;
            }
            if (!File.Exists(txtExecutablePath.Text))
            {
                MessageBox.Show("The provided game executable could not be found.", "Error");
                return;
            }
            if (!File.Exists(txtExecutablePath.Text))
            {
                MessageBox.Show("The specified executable path does not exist.", "Error");
                return;
            }

            // Obtain the new server DB API key.
            string? newServerDbApiKey = chkUseServerDBApiKeys.Checked ? txtServerDBApiKey.Text.Trim() : null;

            // Display a confirmation dialog if we're about to change an existing API key.
            if (newServerDbApiKey != null && Settings.ServerDBApiKey != null && newServerDbApiKey != Settings.ServerDBApiKey)
            {
                if (MessageBox.Show("Resetting the API key used to access ServerDB may invalidate authentication for game servers which do not update their service configs, would you like to continue?", "Echo Relay: Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
            }

            // Set the provided in our settings object.
            Settings.Port = (ushort)numericTCPPort.Value;
            Settings.GameExecutableFilePath = txtExecutablePath.Text;
            Settings.FilesystemDatabaseDirectory = txtDbFolder.Text;
            Settings.MongoDBConnectionString = null; // TODO: currently unsupported
            Settings.StartServerOnStartup = chkStartServerOnStartup.Checked;
            Settings.ServerDBApiKey = newServerDbApiKey;
            Settings.ServerDBValidateGameServers = chkValidateGameServers.Checked;
            Settings.ServerDBValidateGameServersTimeout = (int)numValidateGameServersTimeout.Value;
            Settings.MatchingPopulationOverPing = chkPopulationOverPing.Checked;
            Settings.MatchingForceIntoAnySessionOnFailure = chkForceIntoAnySession.Checked;

            // Save the settings.
            Settings.Save(SettingsFilePath);

            // Set our result and close the dialog.
            DialogResult = DialogResult.OK;
            Close();
        }

        private void RegenerateServerDBApiKey()
        {
            txtServerDBApiKey.Text = Convert.ToBase64String(RandomNumberGenerator.GetBytes(0x20));
        }

        private void RefreshServerDBApiKey()
        {
            if (!chkUseServerDBApiKeys.Checked)
            {
                chkUseServerDBApiKeys.Checked = false;
                txtServerDBApiKey.ReadOnly = true;
            }
            else
            {
                chkUseServerDBApiKeys.Checked = true;
                if (string.IsNullOrEmpty(txtServerDBApiKey.Text.Trim()))
                    RegenerateServerDBApiKey();
                txtServerDBApiKey.ReadOnly = false;
            }
        }
        private void chkUseServerDBApiKeys_CheckedChanged(object sender, EventArgs e)
        {
            RefreshServerDBApiKey();
        }

        private void btnRegenerateAPIKey_Click(object sender, EventArgs e)
        {
            // Regenerate the key.
            RegenerateServerDBApiKey();
        }

        private void chkValidateGameServers_CheckedChanged(object sender, EventArgs e)
        {
            numValidateGameServersTimeout.Enabled = chkValidateGameServers.Checked;
        }
    }
}
