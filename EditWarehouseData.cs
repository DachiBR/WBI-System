using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ERP_Login
{
    public partial class EditWarehouseData : Form
    {
        private string _tableName;
        private Dictionary<string, object> _rowData;
        private Dictionary<string, TextBox> _textBoxes;

        public EditWarehouseData(string tableName, Dictionary<string, object> rowData)
        {
            InitializeComponent();
            _tableName = tableName;
            _rowData = rowData;
            _textBoxes = new Dictionary<string, TextBox>();
        }

        private void EditWarehouseData_Load(object sender, EventArgs e)
        {
            GenerateFields();
        }

        private void GenerateFields()
        {
            int y = 20; // starting y position for the first field
            foreach (var item in _rowData)
            {
                // Skip unwanted fields
                if (item.Key.Equals("Edit", StringComparison.OrdinalIgnoreCase) ||
                    item.Key.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Label label = new Label
                {
                    Text = item.Key,
                    Location = new Point(20, y),
                    AutoSize = true
                };
                this.Controls.Add(label);

                TextBox textBox = new TextBox
                {
                    Name = "txt" + item.Key,
                    Text = item.Value?.ToString(),
                    Location = new Point(150, y),
                    Width = 200
                };
                this.Controls.Add(textBox);

                _textBoxes.Add(item.Key, textBox);
                y += 30;
            }

            Button saveButton = new Button
            {
                Text = "Save",
                Location = new Point(150, y + 20),
                Width = 100
            };
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> updatedData = new Dictionary<string, string>();
            foreach (var textBox in _textBoxes)
            {
                updatedData.Add(textBox.Key, textBox.Value.Text);
            }

            WarehouseDBManager dbManager = new WarehouseDBManager();
            bool success = dbManager.UpdateData(_tableName, Convert.ToInt32(_rowData["ProductID"]), updatedData);

            if (success)
            {
                MessageBox.Show("Data updated successfully!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to update data. Check logs for details.");
            }
        }
    }
}
