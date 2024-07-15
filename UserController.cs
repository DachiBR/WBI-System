using System;
using System.Drawing;
using System.Windows.Forms;

namespace ERP_Login
{
    public class UserController
    {
        private Form mainForm;
        private Button projectsButton;

        public UserController(Form form)
        {
            mainForm = form;
            InitializeProjectsButton();
        }

        private void InitializeProjectsButton()
        {
            projectsButton = new Button();
            projectsButton.Text = "Projects";
            projectsButton.Location = new System.Drawing.Point(160, 80);
            projectsButton.Size = new System.Drawing.Size(100, 30);
            projectsButton.ForeColor = Color.Black;
            projectsButton.BackColor = Color.White;
            projectsButton.Click += new EventHandler(ProjectsButton_Click);
            mainForm.Controls.Add(projectsButton);
        }

        private void ProjectsButton_Click(object sender, EventArgs e)
        {
            ProjectsForm projectsForm = new ProjectsForm();
            projectsForm.Show();
        }

        public void ShowControls()
        {
            projectsButton.Visible = true;
        }
    }
}