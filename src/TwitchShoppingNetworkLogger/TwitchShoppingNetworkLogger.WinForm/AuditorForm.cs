using System;
using System.Windows.Forms;
using TwitchShoppingNetworkLogger.Auditor.Impl;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Config;

namespace TwitchShoppingNetworkLogger.WinForm
{
    public partial class AuditorForm : Form
    {
        private IWhisperAuditor _auditor;
        private ListWhisperRepository _repository;
        private IUser _loggedInUser;
        private Guid _currentSession;
        
        public AuditorForm(string username, string oAuthToken)
        {
            InitializeComponent();

            _repository = new ListWhisperRepository();

            var userRepository = new UserRepository(ConfigManager.Instance);
            _auditor = new AuditorRegistry(userRepository).RegisterNewWhisperAuditor(username, oAuthToken, _repository);
            _loggedInUser = userRepository.GetUserByUsername(username);
        }

        private void AuditorForm_Load(object sender, EventArgs e)
        {
            linkLabel1.Text = $"https://www.twitch.tv/{_loggedInUser.Username}";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try {
                System.Diagnostics.Process.Start($"https://www.twitch.tv/{_loggedInUser.Username}");
            }
            catch (Exception) {
                // do nothing
            }
        }

        private void usersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentUser = (ListUserModel) usersListBox.SelectedItem;
            dataGridView1.DataSource = _repository.GetWhisperListBySessionAndUser(_currentSession.ToString().ToLower(), currentUser.UserId);
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            if (_auditor.IsAuditing())
            {
                _auditor.EndAuditing();
                startStopButton.Text = "Start Logging";
            }
            else
            {
                _auditor.StartAuditing();
                _currentSession = _auditor.CurrentSessionId;
                startStopButton.Text = "Stop Logging";

                usersListBox.DataSource = _repository.GetUserListBySession(_currentSession.ToString().ToLower());
                usersListBox.ClearSelected();
            }
        }

        private void AuditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_auditor.IsAuditing())
                _auditor.EndAuditing();
            Application.Exit();
        }
    }
}
