using System;
using System.Windows.Forms;
using TwitchShoppingNetworkLogger.Auditor.Binding;
using TwitchShoppingNetworkLogger.Auditor.Impl;
using TwitchShoppingNetworkLogger.Auditor.Interfaces;
using TwitchShoppingNetworkLogger.Config;

namespace TwitchShoppingNetworkLogger.WinForm
{
    public partial class AuditorForm : Form
    {
        private IWhisperAuditor _auditor;
        private ListWhisperRepository _listRepository;
        private IUser _loggedInUser;
        private Guid _currentSession;

        private BindingListInvoked<ListUserModel> _boundUsers;

        public AuditorForm(string username, string oAuthToken)
        {
            InitializeComponent();

            /* Initialize the whisper repositories.
               We'll keep a reference to the ListWhisperRepository to update our UI.
               The ExcelWhisperRepository will run in the background and log whispers to an Excel file.
            */
            _listRepository = new ListWhisperRepository(usersListBox, dataGridView1);
            var excelRepository = new ExcelWhisperRepository();
            var aggregateRepository = new AggregateWhisperRepository(new IWhisperRepository[] { _listRepository, excelRepository });

            var userRepository = new UserRepository(ConfigManager.Instance, null);
            _auditor = new AuditorRegistry(userRepository, ConfigManager.Instance)
                .RegisterNewWhisperAuditor(username, oAuthToken, aggregateRepository);
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
            if (currentUser != null)
                dataGridView1.DataSource = _listRepository.GetWhisperListBySessionAndUser(_currentSession.ToString().ToLower(), currentUser.UserId);
            else
                dataGridView1.DataSource = null;
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
                
                _boundUsers = _listRepository.GetUserListBySession(_currentSession.ToString().ToLower());
                usersListBox.ClearSelected();
                usersListBox.DataSource = _boundUsers;
                usersListBox.DisplayMember = "Username";
                usersListBox.ValueMember = "UserId";
                usersListBox.Refresh();
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
