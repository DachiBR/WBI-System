using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ERP_Login
{
    public partial class Login : Form
    {
        private DBManager dBManager = new DBManager();

        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            UserRole? userRole = dBManager.Login(username, password);

            if (userRole.HasValue)
            {
                Main mainPage = new Main(userRole.Value);
                mainPage.Show();
                this.Hide();
                mainPage.FormClosed += (s, args) => this.Close(); // Close the login form when the main page is closed
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {
        }
    }
}
