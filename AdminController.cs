using System;
using System.Drawing;
using System.Windows.Forms;

namespace ERP_Login
{
    public class AdminController
    {
        private Form mainForm;
        private Button warehouseButton;

        public AdminController(Form form)
        {
            mainForm = form;
            InitializeWarehouseButton();
        }

        private void InitializeWarehouseButton()
        {
            warehouseButton = new Button();
            warehouseButton.Text = "Warehouse";
            warehouseButton.Location = new System.Drawing.Point(30, 80);
            warehouseButton.Size = new System.Drawing.Size(100, 30);
            warehouseButton.ForeColor = Color.Black;
            warehouseButton.BackColor = Color.White;
            warehouseButton.Click += new EventHandler(WarehouseButton_Click);
            mainForm.Controls.Add(warehouseButton);
        }

        private void WarehouseButton_Click(object sender, EventArgs e)
        {
            WarehouseForm warehouseForm = new WarehouseForm();
            warehouseForm.Show();
        }

        public void ShowControls()
        {
            warehouseButton.Visible = true;
        }
    }
}
