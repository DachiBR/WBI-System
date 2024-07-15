using System;
using System.Drawing;
using System.Windows.Forms;

namespace ERP_Login
{
    internal class QamController
    {
        private Form _mainForm; // Reference to the MainForm
        private Button projectsButton; // Button for Projects

        public QamController(Form mainForm)
        {
            _mainForm = mainForm;
            InitializeProjectsButton(); // Initialize the Projects button
        }

        private void InitializeProjectsButton()
        {
            projectsButton = new Button();
            projectsButton.Text = "Drawings";
            projectsButton.Location = new System.Drawing.Point(30, 120); // Adjust location as needed
            projectsButton.Size = new System.Drawing.Size(100, 30);
            projectsButton.ForeColor = Color.Black;
            projectsButton.BackColor = Color.White;
            projectsButton.Click += new EventHandler(ProjectsButton_Click);
            _mainForm.Controls.Add(projectsButton);
        }

        private void ProjectsButton_Click(object sender, EventArgs e)
        {
            // Handle the click event for Projects button here
            // Example: Opening a ProjectsForm
            ProjectsForm projectsForm = new ProjectsForm();
            projectsForm.Show();
        }

        public void ShowControls()
        {
            projectsButton.Visible = true; // Show the Projects button
        }
    }
}
