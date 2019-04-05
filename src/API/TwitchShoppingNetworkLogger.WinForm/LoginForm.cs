using System;
using System.Windows.Forms;

namespace TwitchShoppingNetworkLogger.WinForm
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string tokenUrl = "https://twitchtokengenerator.com/quick/g8SD5tBBBA";
            try
            {
                System.Diagnostics.Process.Start(tokenUrl);
            }
            catch (Exception) {
                MessageBox.Show($"Unable to open link. Please go to {tokenUrl} to generate your token.");
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void usernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Login();
        }

        private void tokenTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Login();
        }

        private void Login()
        {
            var auditorForm = new AuditorForm(usernameTextBox.Text, tokenTextBox.Text);
            auditorForm.Show();
            this.Hide();
        }
    }
}
