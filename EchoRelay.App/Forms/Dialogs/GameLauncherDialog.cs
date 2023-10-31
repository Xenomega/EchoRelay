using EchoRelay.App.Settings;
using EchoRelay.Core.Game;

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
                windowed: chkWindowed.Checked,
                spectatorStream: chkSpectatorStream.Checked,
                moderator: chkModerator.Checked,
                noOVR: chkNoOVR.Checked
                );

            // Close the dialog
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
