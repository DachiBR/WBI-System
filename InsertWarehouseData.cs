using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ERP_Login
{
    public partial class InsertWarehouseData : Form
    {
        private string selectedCategory;
        private WarehouseDBManager warehouseDBManager;

        public InsertWarehouseData(string category)
        {
            InitializeComponent();
            selectedCategory = category;
            warehouseDBManager = new WarehouseDBManager();
            LoadFields();
        }

        private void InsertWarehouseData_Load(object sender, EventArgs e)
        {
        }

        private void LoadFields()
        {
            DataTable columns = warehouseDBManager.GetTableColumns(selectedCategory);

            int yPos = 20;
            foreach (DataRow row in columns.Rows)
            {
                string columnName = row["COLUMN_NAME"].ToString();

                Label label = new Label
                {
                    Text = columnName,
                    Location = new Point(20, yPos),
                    ForeColor = Color.White
                };
                Controls.Add(label);

                TextBox textBox = new TextBox
                {
                    Name = $"txt{columnName}",
                    Location = new Point(150, yPos),
                    Width = 200,
                    ForeColor = Color.Black,  // Text color inside the TextBox
                    BackColor = Color.White   // Background color of the TextBox
                };
                Controls.Add(textBox);

                yPos += 30;
            }

            Button submitButton = new Button
            {
                Text = "Submit",
                Location = new Point(150, yPos),
                ForeColor = Color.Red   // Text color of the button
            };
            submitButton.Click += SubmitButton_Click;
            Controls.Add(submitButton);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> formData = new Dictionary<string, string>();
            DataTable columns = warehouseDBManager.GetTableColumns(selectedCategory);

            foreach (Control control in Controls)
            {
                if (control is TextBox textBox)
                {
                    string columnName = textBox.Name.Substring(3); // Remove "txt" prefix
                    string value = textBox.Text;
                    string dataType = columns.Select($"COLUMN_NAME = '{columnName}'")[0]["DATA_TYPE"].ToString();

                    if (!ValidateData(columnName, value, dataType))
                    {
                        return;
                    }

                    formData.Add(columnName, value);
                }
            }

            if (warehouseDBManager.InsertData(selectedCategory, formData))
            {
                MessageBox.Show("Data inserted successfully!");
            }
            else
            {
                MessageBox.Show("Error inserting data.");
            }

            this.Close();
        }

        private bool ValidateData(string columnName, string value, string dataType)
        {
            try
            {
                switch (dataType.ToLower())
                {
                    case "int":
                    case "integer":
                        if (!int.TryParse(value, out _))
                        {
                            MessageBox.Show($"{columnName} must be an integer.");
                            return false;
                        }
                        break;
                    case "float":
                    case "double":
                        if (!double.TryParse(value, out _))
                        {
                            MessageBox.Show($"{columnName} must be a number.");
                            return false;
                        }
                        break;
                    case "datetime":
                    case "date":
                        if (!DateTime.TryParse(value, out _))
                        {
                            MessageBox.Show($"{columnName} must be a valid date.");
                            return false;
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(value))
                        {
                            MessageBox.Show($"{columnName} cannot be empty.");
                            return false;
                        }
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating data for {columnName}: {ex.Message}");
                return false;
            }
        }
    }
}
