using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using ExcelDataReader;
using System.Collections.Generic;
using System.Reflection;

namespace ERP_Login
{
    public partial class WarehouseForm : Form
    {
        private WarehouseDBManager warehouseDBManager;

        public WarehouseForm()
        {
            InitializeComponent();
            warehouseDBManager = new WarehouseDBManager();
            warehousedgv1.DataError += warehousedgv1_DataError;
            warehousedgv1.AutoGenerateColumns = true;
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, warehousedgv1, new object[] { true });
            warehousedgv1.ScrollBars = ScrollBars.Both;
        }



        /*-------------------------------------------------------------------*/
        /* Button Fucntions To Load Specific Table Data to DataGridView*/

        private void button2_Click(object sender, EventArgs e)
        {
            LoadData(warehouseDBManager.GetOfficeMaterials());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadData(warehouseDBManager.GetSupportMaterials());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadData(warehouseDBManager.GetProductMaterials());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadData(warehouseDBManager.GetSpareParts());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LoadData(warehouseDBManager.GetHardware());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LoadData(warehouseDBManager.GetParts());
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LoadData(warehouseDBManager.GetAssembly());
        }


        private void LoadData(DataTable dtbl)
        {
            // Display data in DataGridView
            warehousedgv1.DataSource = dtbl;
        }


        /*-------------------------------------------------------------------*/
        /* Error Message for Datagridview Bugs */
        private void warehousedgv1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Log the error or show a message to the user
            Console.WriteLine($"DataGridView Error: {e.Exception.Message}");
            e.ThrowException = false; // Prevent the exception from being thrown
        }


        /*-------------------------------------------------------------------*/
        /* Functional Button For Adding Information In the specific Table, Which takes data from the Combo Box */
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please choose a category before adding a product.");
            }
            else
            {
                string selectedCategory = comboBox2.SelectedItem.ToString();
                InsertWarehouseData insertWarehouseDataForm = new InsertWarehouseData(selectedCategory);
                insertWarehouseDataForm.ShowDialog();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        /*-------------------------------------------------------------------*/



        private void WarehouseForm_Load(object sender, EventArgs e)
        {
            // Add Edit button column
            DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn();
            editButtonColumn.Name = "Edit";
            editButtonColumn.Text = "Edit";
            editButtonColumn.UseColumnTextForButtonValue = true;
            warehousedgv1.Columns.Add(editButtonColumn);

            // Add Delete button column
            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.Name = "Delete";
            deleteButtonColumn.Text = "Delete";
            deleteButtonColumn.UseColumnTextForButtonValue = true;
            warehousedgv1.Columns.Add(deleteButtonColumn);

            // Hook up the CellFormatting event (if needed)
            warehousedgv1.CellFormatting += warehousedgv1_CellFormatting;

            // Hook up the CellContentClick event for handling button clicks

            // Load data into DataGridView (assuming you have a method like LoadData)
            // LoadData();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            comboBox2.Items.AddRange(new string[] { "office_materials", "support_materials", "product_materials", "spare_parts", "hardware", "parts", "assembly" });
        }

        /* Delete function */
        private void warehousedgv1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Prevent the event from triggering for header or invalid rows/columns
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Check if the "Edit" button is clicked
            if (warehousedgv1.Columns[e.ColumnIndex].Name == "Edit")
            {
                DataGridViewRow row = warehousedgv1.Rows[e.RowIndex];
                if (row.Cells["ProductID"].Value != null)
                {
                    int productId;
                    if (int.TryParse(row.Cells["ProductID"].Value.ToString(), out productId))
                    {
                        string tableName = comboBox2.SelectedItem?.ToString();
                        if (string.IsNullOrWhiteSpace(tableName))
                        {
                            MessageBox.Show("Please select a category before attempting to edit a product.", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        Dictionary<string, object> rowData = new Dictionary<string, object>();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            rowData.Add(cell.OwningColumn.Name, cell.Value);
                        }

                        EditWarehouseData editForm = new EditWarehouseData(tableName, rowData);
                        editForm.ShowDialog();

                        // Optionally, refresh or reload your DataGridView after editing
                        LoadData(warehouseDBManager.GetTableData(tableName));
                    }
                }
            }

            // Check if the "Delete" button is clicked
            if (warehousedgv1.Columns[e.ColumnIndex].Name == "Delete")
            {
                // Ensure the row index and column index are valid and prevent multiple dialogs
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // Get the TableName from the ComboBox
                    string tableName = comboBox2.SelectedItem?.ToString();

                    // Log the table name for debugging
                    Console.WriteLine($"Selected table name: {tableName}");

                    // Check if table name is not selected or invalid
                    if (string.IsNullOrWhiteSpace(tableName))
                    {
                        MessageBox.Show("Please select a category before attempting to delete a product.", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Exit the method to prevent further execution
                    }

                    // Get the ProductID from the selected row
                    DataGridViewRow row = warehousedgv1.Rows[e.RowIndex];
                    if (row.Cells["ProductID"].Value == null)
                    {
                        MessageBox.Show("Product ID is invalid.");
                        return;
                    }

                    int productId;
                    if (!int.TryParse(row.Cells["ProductID"].Value.ToString(), out productId))
                    {
                        MessageBox.Show("Invalid Product ID format.");
                        return;
                    }

                    // Confirm deletion
                    DialogResult result = MessageBox.Show($"Are you sure you want to delete Product ID: {productId} from {tableName}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Perform delete operation
                        bool success = warehouseDBManager.DeleteData(tableName, productId);

                        if (success)
                        {
                            MessageBox.Show("Product deleted successfully!");
                            // Optionally, refresh or reload your DataGridView after deletion
                            // LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete product.");
                        }
                    }
                }
            }
        }




        /*-------------------------------------------------------------------*/

        /* Searching Functionality */
        private void Search_Click(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text.Trim();
            string searchBy = comboBox1.SelectedItem?.ToString();

            // Provide a default search criterion if none is selected
            if (string.IsNullOrEmpty(searchBy))
            {
                MessageBox.Show("Please select a search criterion from the dropdown.");
                return;
            }

            WarehouseDBManager dbManager = new WarehouseDBManager();
            DataTable searchResults = dbManager.SearchMaterials(searchBy, searchTerm);

            if (searchResults.Rows.Count == 0)
            {
                MessageBox.Show("No matching materials found.");
            }
            else
            {
                try
                {
                    warehousedgv1.DataSource = searchResults;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error displaying data: {ex.Message}");
                }
            }
        }

        /*-------------------------------------------------------------------*/






        /*-------------------------------------------------------------------*/

        /* Cell Formatting Algorithm*/

        private void warehousedgv1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = warehousedgv1.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];


                /* Min/Max Algorithm for applying Colors */
                if (cell.OwningColumn.Name == "Quantity")
                {
                    var quantityCell = cell;

                    if (warehousedgv1.Columns.Contains("MinQuantity"))
                    {
                        var minQuantityCell = row.Cells["MinQuantity"];

                        if (minQuantityCell != null && minQuantityCell.Value != null)
                        {
                            if (int.TryParse(quantityCell.Value.ToString(), out int quantity) &&
                                int.TryParse(minQuantityCell.Value.ToString(), out int minQuantity))
                            {
                                if (quantity < minQuantity)
                                {
                                    quantityCell.Style.BackColor = Color.Red;
                                }
                                else if (quantity > minQuantity * 3)
                                {
                                    quantityCell.Style.BackColor = Color.Green;
                                }
                                else if (quantity > minQuantity * 2)
                                {
                                    quantityCell.Style.BackColor = Color.Yellow;
                                }
                                else if (quantity > minQuantity)
                                {
                                    quantityCell.Style.BackColor = Color.Orange;
                                }
                                else
                                {
                                    quantityCell.Style.BackColor = Color.White; 
                                }
                            }
                        }
                    }
                }



                /* Expiration Date Colors Algorithm */
                else if (cell.OwningColumn.Name == "ExpirationDate")
                {
                    if (cell.Value != null)
                    {
                        if (DateTime.TryParse(cell.Value.ToString(), out DateTime expirationDate))
                        {
                            // Calculate days until expiration
                            int daysUntilExpiration = (expirationDate - DateTime.Today).Days;

                            // Set background color based on days until expiration
                            if (daysUntilExpiration < 0 )
                            {
                                cell.Style.BackColor = Color.Red; // Expired
                            }
                            else if (daysUntilExpiration <= 30)
                            {
                                cell.Style.BackColor = Color.Red; // Less than or equal to 1 month
                            }
                            else if (daysUntilExpiration <= 45)
                            {
                                cell.Style.BackColor = Color.Orange; // Less than or equal to 2 months
                            }
                            else if (daysUntilExpiration <= 60)
                            {
                                cell.Style.BackColor = Color.Yellow; // Less than or equal to 3 months
                            }
                            else if (daysUntilExpiration >= 90)
                            {
                                cell.Style.BackColor = Color.Green; // Less than or equal to 3 months
                            }
                            // Otherwise, keep the default color
                        }
                    }
                }
            }
        }






        /*Import Export Functionality for Handling import export data with excel files */




        /* Import Clicking Function  */       
        private void Import_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please choose a category before importing.");
                return;
            }

            string selectedCategory = comboBox2.SelectedItem.ToString();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    DataTable dataTable = ExcelToDataTable(openFileDialog.FileName);
                    if (dataTable != null)
                    {
                        WarehouseDBManager dbManager = new WarehouseDBManager();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var data = new Dictionary<string, string>();
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                data[column.ColumnName] = row[column].ToString();
                            }
                            dbManager.InsertData(selectedCategory, data);
                        }
                        MessageBox.Show("Data imported successfully.");
                    }
                }
            }
        }


        /* Export Clicking Function  */

        private void Export_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please choose a category before exporting.");
                return;
            }

            string selectedCategory = comboBox2.SelectedItem.ToString();
            WarehouseDBManager dbManager = new WarehouseDBManager();
            DataTable dataTable = dbManager.GetTableData(selectedCategory);

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files|*.xlsx;*.xlsm;*.xltx;*.xltm";
                saveFileDialog.DefaultExt = "xlsx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    string extension = Path.GetExtension(filePath).ToLower();
                    if (extension != ".xlsx" && extension != ".xlsm" && extension != ".xltx" && extension != ".xltm")
                    {
                        filePath = Path.ChangeExtension(filePath, ".xlsx");
                    }

                    DataTableToExcel(dataTable, filePath);
                    MessageBox.Show("Data exported successfully.");
                }
            }
        }

        /* Transfering Excel Data to Database Table */
        private DataTable ExcelToDataTable(string filePath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    var dataTable = result.Tables[0];
                    dataTable.TableName = Path.GetFileNameWithoutExtension(filePath); 
                    return dataTable;
                }
            }
        }
        /* Transfering Database Table to Excel File data */

        private void DataTableToExcel(DataTable dataTable, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var sheetName = !string.IsNullOrWhiteSpace(dataTable.TableName) ? dataTable.TableName : "Sheet1";
                var worksheet = workbook.Worksheets.Add(sheetName);

                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = dataTable.Columns[i].ColumnName;
                }

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        worksheet.Cell(i + 2, j + 1).Value = dataTable.Rows[i][j]?.ToString();
                    }
                }

                workbook.SaveAs(filePath);
            }
        }

  
    }
}


/*-------------------------------------------------------------------*/


/* converting a byte array (byte[]) containing image data into an Image object*/
/*   
 private Image ByteArrayToImage(byte[] byteArray)
   {
       try
       {
           if (byteArray == null || byteArray.Length == 0)
           {
               return null;
           }

           using (MemoryStream ms = new MemoryStream(byteArray))
           {
               return Image.FromStream(ms);
           }
       }
       catch (Exception ex)
       {
           Console.WriteLine($"Error converting byte array to image: {ex.Message}");
           return null;
       }
   }

   */

/*-------------------------------------------------------------------*/