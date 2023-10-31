namespace EchoRelay.App.Forms.Controls
{
    partial class PeerConnectionsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listPeers = new ListView();
            columnHeaderService = new ColumnHeader();
            columnHeaderSourceIP = new ColumnHeader();
            columnHeaderSourcePort = new ColumnHeader();
            columnHeaderUserId = new ColumnHeader();
            columnHeaderInitiatedTime = new ColumnHeader();
            groupBoxConnections = new GroupBox();
            columnHeaderUserDisplayName = new ColumnHeader();
            groupBoxConnections.SuspendLayout();
            SuspendLayout();
            // 
            // listPeers
            // 
            listPeers.Columns.AddRange(new ColumnHeader[] { columnHeaderService, columnHeaderSourceIP, columnHeaderSourcePort, columnHeaderUserId, columnHeaderUserDisplayName, columnHeaderInitiatedTime });
            listPeers.Dock = DockStyle.Fill;
            listPeers.FullRowSelect = true;
            listPeers.GridLines = true;
            listPeers.Location = new Point(3, 19);
            listPeers.Name = "listPeers";
            listPeers.Size = new Size(1276, 794);
            listPeers.TabIndex = 0;
            listPeers.UseCompatibleStateImageBehavior = false;
            listPeers.View = View.Details;
            // 
            // columnHeaderService
            // 
            columnHeaderService.Text = "Service";
            columnHeaderService.Width = 100;
            // 
            // columnHeaderSourceIP
            // 
            columnHeaderSourceIP.Text = "Source IP";
            columnHeaderSourceIP.Width = 150;
            // 
            // columnHeaderSourcePort
            // 
            columnHeaderSourcePort.Text = "Source Port (TCP)";
            columnHeaderSourcePort.Width = 110;
            // 
            // columnHeaderUserId
            // 
            columnHeaderUserId.Text = "User Id";
            columnHeaderUserId.Width = 200;
            // 
            // columnHeaderInitiatedTime
            // 
            columnHeaderInitiatedTime.Text = "Initiated Time (UTC)";
            columnHeaderInitiatedTime.Width = 200;
            // 
            // groupBoxConnections
            // 
            groupBoxConnections.Controls.Add(listPeers);
            groupBoxConnections.Dock = DockStyle.Fill;
            groupBoxConnections.Location = new Point(0, 0);
            groupBoxConnections.Name = "groupBoxConnections";
            groupBoxConnections.Size = new Size(1282, 816);
            groupBoxConnections.TabIndex = 1;
            groupBoxConnections.TabStop = false;
            groupBoxConnections.Text = "Peer connections";
            // 
            // columnHeaderUserDisplayName
            // 
            columnHeaderUserDisplayName.Text = "User Display Name";
            columnHeaderUserDisplayName.Width = 150;
            // 
            // PeerConnectionsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBoxConnections);
            Name = "PeerConnectionsControl";
            Size = new Size(1282, 816);
            groupBoxConnections.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListView listPeers;
        private ColumnHeader columnHeaderService;
        private ColumnHeader columnHeaderSourceIP;
        private ColumnHeader columnHeaderUserId;
        private ColumnHeader columnHeaderInitiatedTime;
        private ColumnHeader columnHeaderSourcePort;
        private GroupBox groupBoxConnections;
        private ColumnHeader columnHeaderUserDisplayName;
    }
}
