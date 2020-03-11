using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Configuration;

using Brandeis.AGSOL.Network;

namespace Brandeis.AGSOL.Hulkamania
{
    partial class SendReconnectCommandToNetworkClients : Form
    {
        #region Fields
        private Button okButton;
        private Button cancelButton;
        private Label serverAddressLabel;
        private Label serverPortLabel;
        private ComboBox serverAddressComboBox;
        private TextBox serverPortTextBox;
        #endregion

        #region GUI Creation

        /// <summary>
        /// Populates the GUI with controls
        /// </summary>
        private void InitializeGUI()
        {
            BackColor = ColorScheme.Instance.PanelBackground;

            okButton = new Button();
            cancelButton = new Button();
            serverAddressLabel = new Label();
            serverPortLabel = new Label();
            serverAddressComboBox = new ComboBox();
            serverPortTextBox = new TextBox();
            SuspendLayout();

            okButton.DialogResult = DialogResult.Cancel;
            okButton.Location = new System.Drawing.Point(132, 62);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 24;
            okButton.Text = "&OK";
            okButton.Click += new System.EventHandler(okButton_Click);
            okButton.BackColor = ColorScheme.Instance.ButtonBackground;
            okButton.FlatStyle = FlatStyle.Flat;
            okButton.ForeColor = ColorScheme.Instance.ButtonForeground;
            okButton.FlatAppearance.BorderSize = 0;

            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(51, 62);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 25;
            cancelButton.Text = "&Cancel";
            cancelButton.BackColor = ColorScheme.Instance.ButtonBackground;
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.ForeColor = ColorScheme.Instance.ButtonForeground;
            cancelButton.FlatAppearance.BorderSize = 0;

            serverAddressLabel.AutoSize = true;
            serverAddressLabel.Location = new System.Drawing.Point(13, 13);
            serverAddressLabel.Name = "serverAddressLabel";
            serverAddressLabel.Size = new System.Drawing.Size(81, 13);
            serverAddressLabel.TabIndex = 26;
            serverAddressLabel.Text = "Server address:";
            serverAddressLabel.ForeColor = ColorScheme.Instance.LabelForeground;
            serverAddressLabel.TextAlign = ContentAlignment.MiddleLeft;
          
            serverPortLabel.AutoSize = true;
            serverPortLabel.Location = new System.Drawing.Point(13, 39);
            serverPortLabel.Name = "serverPortLabel";
            serverPortLabel.Size = new System.Drawing.Size(62, 13);
            serverPortLabel.TabIndex = 27;
            serverPortLabel.Text = "Server port:";
            serverPortLabel.ForeColor = ColorScheme.Instance.LabelForeground;
            serverPortLabel.TextAlign = ContentAlignment.MiddleLeft;

            serverAddressComboBox.Location = new System.Drawing.Point(100, 10);
            serverAddressComboBox.Name = "serverAddressTextBox";
            serverAddressComboBox.Size = new System.Drawing.Size(146, 20);
            serverAddressComboBox.TabIndex = 28;
            serverAddressComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            serverAddressComboBox.BackColor = ColorScheme.Instance.ListBoxBackground;
            serverAddressComboBox.ForeColor = ColorScheme.Instance.ListBoxForeground;
            serverAddressComboBox.FlatStyle = FlatStyle.Flat;

            serverPortTextBox.Location = new System.Drawing.Point(100, 36);
            serverPortTextBox.Name = "serverPortTextBox";
            serverPortTextBox.Size = new System.Drawing.Size(146, 20);
            serverPortTextBox.TabIndex = 29;
            serverPortTextBox.ForeColor = ColorScheme.Instance.TextBoxForeground;
            serverPortTextBox.BackColor = ColorScheme.Instance.TextBoxBackground;
            serverPortTextBox.BorderStyle = BorderStyle.None;
            
            AcceptButton = okButton;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(256, 94);
            Controls.Add(serverPortTextBox);
            Controls.Add(serverAddressComboBox);
            Controls.Add(serverPortLabel);
            Controls.Add(serverAddressLabel);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SendReconnectCommandToNetworkClients";
            Padding = new Padding(9);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Reconnect Network Clients To";
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SendReconnectCommandToNetworkClients()
        {
            InitializeGUI();

            // get hostname for current machine
            string hostName = System.Net.Dns.GetHostName();

            int serverIndex = 1;
            while(_hasVRServerInConfiguration("VRServer"+serverIndex)){
                serverAddressComboBox.Items.Add(ConfigurationManager.AppSettings["VRServer"+serverIndex]);

                // select the last added item if it is not the local machine
                if (hostName.ToLower() != ConfigurationManager.AppSettings["VRServer" + serverIndex].ToLower())
                {
                    serverAddressComboBox.SelectedIndex = serverAddressComboBox.Items.Count - 1;
                }

                serverIndex+=1;
            }

            serverPortTextBox.Text = ConfigurationManager.AppSettings["VRServerPort"];
            serverPortTextBox.TextChanged += new EventHandler(serverPortTextBox_TextChanged);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Checks whether the app.config file holds the specified key
        /// </summary>
        /// <param name="key">the key to check</param>
        /// <returns>true if the key is present</returns>
        private bool _hasVRServerInConfiguration(string key)
        {
            foreach (string s in ConfigurationManager.AppSettings.AllKeys) {
                if (s.ToLower() == key.ToLower()) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Called when the text in the serverport textbox changes. Checks if a valid integer has been entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void serverPortTextBox_TextChanged(object sender, EventArgs e)
        {
            int port = 0;
            bool validInt = int.TryParse(serverPortTextBox.Text, out port);
            serverPortTextBox.BackColor = validInt ? ColorScheme.Instance.TextBoxBackground : Color.Red;
            okButton.Enabled = validInt;
        }

        /// <summary>
        /// Called when the user clicks the OK button. Sends the actual command to the connected client(s)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            ICommand c = new ICommand();
            c.CommandType = (int)(eCommands.User) + 2;
            c.addParameter(0, serverAddressComboBox.SelectedItem as string);
            c.addParameter(1, serverPortTextBox.Text);
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);
        }
        #endregion
    }
}
