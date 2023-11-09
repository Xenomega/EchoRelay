using EchoRelay.App.Settings;
using EchoRelay.Core.Game;
using System.Collections.Concurrent;

namespace EchoRelay.App.Forms.Dialogs
{
    public partial class GameLauncherDialog : Form
    {
        /// <summary>
        /// The application settings containing the executable path information used to launch the game.
        /// </summary>
        public AppSettings Settings { get; }

        public GameLauncherDialog(AppSettings settings)
        {
            InitializeComponent();

            // Set our settings
            Settings = settings;

            // Set the dialog result to cancelled, this way only if we launch the process do we return OK.
            DialogResult = DialogResult.Cancel;
        }

        private void btnLaunchGame_Click(object sender, EventArgs e)
        {

            // Obtain the role from the drop down selection.
            var launchRole = radioRoleClient.Checked ? GameLauncher.LaunchRole.Client : (radioRoleServer.Checked ? GameLauncher.LaunchRole.Server : GameLauncher.LaunchRole.Offline);

            // Launch the game with the provided arguments.
            GameLauncher.Launch(
                executableFilePath: Settings.GameExecutableFilePath,
                role: launchRole,
                windowed: radioViewWindowed.Checked,
                headless: radioViewHeadless.Checked,
                noOVR: chkNoOVR.Checked,
                spectatorStream: chkSpectatorStream.Checked,
                moderator: chkModerator.Checked,
                timeStep: chkThrottleTimestep.Checked ? (uint)numThrottleTimestep.Value : null
                );

            // Close the dialog
            DialogResult = DialogResult.OK;
            Close();
        }

        private void radioRoleServer_CheckedChanged(object sender, EventArgs e)
        {
            radioViewVR.Enabled = !radioRoleServer.Checked;
            if (radioRoleServer.Checked && radioViewVR.Checked)
                radioViewHeadless.Checked = true;
        }

        private void chkThrottleTimestep_CheckedChanged(object sender, EventArgs e)
        {
            numThrottleTimestep.Enabled = chkThrottleTimestep.Checked;
        }

        private void radioViewHeadless_CheckedChanged(object sender, EventArgs e)
        {
            chkThrottleTimestep.Enabled = radioViewHeadless.Checked;
            if (!radioViewHeadless.Checked)
                chkThrottleTimestep.Checked = false;
        }
    }
}
