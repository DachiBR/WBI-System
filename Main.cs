using System;
using System.Windows.Forms;

namespace ERP_Login
{
    public partial class Main : Form
    {
        private AdminController adminController;
        private UserController userController;
        private QamController qamController;

        public Main(UserRole role)
        {
            InitializeComponent();
            InitializeControllers(role);
        }

        private void InitializeControllers(UserRole role)
        {
            if (role == UserRole.ADMIN)
            {
                adminController = new AdminController(this);
                adminController.ShowControls();
            }
            else if (role == UserRole.USER)
            {
                userController = new UserController(this);
                userController.ShowControls();
            }
            else if (role == UserRole.QAM)
            {
                qamController = new QamController(this);
                qamController.ShowControls();
            }
            else
            {
                throw new ArgumentException("Unsupported role: " + role);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }
}
