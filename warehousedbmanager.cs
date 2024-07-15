using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ERP_Login
{
    internal class WarehouseDBManager
    {
        private const string WarehouseConnectionString = "server=localhost;uid=root;pwd=MITDBR028!WBILUM@2003281955@;database=warehouse";
        private MySqlConnection warehouseConnection;

        public WarehouseDBManager()
        {
            warehouseConnection = new MySqlConnection(WarehouseConnectionString);
        }


        private DataTable GetDataTable(string query)
        {
            DataTable dataTable = new DataTable();

            try
            {
                warehouseConnection.Open();
                using (var cmd = new MySqlCommand(query, warehouseConnection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
            finally
            {
                warehouseConnection.Close();
            }

            return dataTable;
        }



        public DataTable GetTableData(string tableName)
        {
            return GetDataTable($"SELECT * FROM {tableName}");
        }



        public bool DeleteData(string tableName, int productId)
        {
            try
            {
                warehouseConnection.Open();

                // Validate and log the table name
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    Console.WriteLine("Table name is invalid.");
                    return false;
                }

                // Enclose the table name in backticks to handle special characters and reserved words
                string query = $"DELETE FROM `{tableName}` WHERE `ProductID` = @ProductID";
                Console.WriteLine($"Executing query: {query} with ProductID: {productId}");

                using (var cmd = new MySqlCommand(query, warehouseConnection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Product deleted successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("No rows affected. ProductID might not exist.");
                        return false;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Error deleting data: {ex.Message}");
                MessageBox.Show($"MySQL Error deleting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting data: {ex.Message}");
                MessageBox.Show($"Error deleting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                warehouseConnection.Close();
            }
        }



        public bool UpdateData(string tableName, int productId, Dictionary<string, string> updatedData)
        {
            try
            {
                warehouseConnection.Open();

                List<string> setClauses = new List<string>();
                foreach (var kvp in updatedData)
                {
                    // Skip ProductID if it's part of the updated data to avoid duplicate parameter
                    if (kvp.Key != "ProductID")
                    {
                        setClauses.Add($"{kvp.Key} = @{kvp.Key}");
                    }
                }
                string setClause = string.Join(", ", setClauses);
                string query = $"UPDATE `{tableName}` SET {setClause} WHERE ProductID = @ProductID";

                Console.WriteLine($"Executing query: {query}");

                using (var cmd = new MySqlCommand(query, warehouseConnection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    foreach (var kvp in updatedData)
                    {
                        // Skip ProductID if it's part of the updated data to avoid duplicate parameter
                        if (kvp.Key != "ProductID")
                        {
                            cmd.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                        }
                    }

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Data updated successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"No rows updated for ProductID: {productId}");
                        return false;
                    }
                }
            }
            catch (MySqlException ex)
            {
                string errorMessage = $"MySQL Error updating data: {ex.Message}";
                Console.WriteLine(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error updating data: {ex.Message}";
                Console.WriteLine(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                warehouseConnection.Close();
            }
        }


        public DataTable GetOfficeMaterials() => GetDataTable("SELECT * FROM office_materials");
        public DataTable GetSupportMaterials() => GetDataTable("SELECT * FROM support_materials");
        public DataTable GetProductMaterials() => GetDataTable("SELECT * FROM product_materials");
        public DataTable GetSpareParts() => GetDataTable("SELECT * FROM spare_parts");
        public DataTable GetHardware() => GetDataTable("SELECT * FROM hardware");
        public DataTable GetParts() => GetDataTable("SELECT * FROM parts");
        public DataTable GetAssembly() => GetDataTable("SELECT * FROM assembly");

        public DataTable GetTableColumns(string tableName)
        {
            string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
            return GetDataTable(query);
        }

        public DataTable SearchMaterials(string searchBy, string searchTerm)
        {
            string[] tables = { "office_materials", "support_materials", "product_materials", "spare_parts", "hardware", "parts", "assembly" };
            DataTable combinedResults = new DataTable();

            foreach (string table in tables)
            {
                string query = $"SELECT * FROM {table} WHERE {searchBy} LIKE @searchTerm";
                DataTable dataTable = new DataTable();

                try
                {
                    warehouseConnection.Open();
                    using (var cmd = new MySqlCommand(query, warehouseConnection))
                    {
                        cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching data from {table}: {ex.Message}");
                }
                finally
                {
                    warehouseConnection.Close();
                }

                // Combine results from each table into one DataTable
                if (dataTable.Rows.Count > 0)
                {
                    combinedResults.Merge(dataTable);
                }
            }

            return combinedResults;
        }



        public bool InsertData(string tableName, Dictionary<string, string> data)
        {
            try
            {
                warehouseConnection.Open();

                // Create lists for columns and parameters
                var columns = new List<string>();
                var paramNames = new List<string>();

                foreach (var kvp in data)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        columns.Add(kvp.Key);
                        paramNames.Add("@" + kvp.Key);
                    }
                }

                string columnsStr = string.Join(", ", columns);
                string paramNamesStr = string.Join(", ", paramNames);
                string query = $"INSERT INTO {tableName} ({columnsStr}) VALUES ({paramNamesStr})";

                using (var cmd = new MySqlCommand(query, warehouseConnection))
                {
                    foreach (var kvp in data)
                    {
                        if (!string.IsNullOrEmpty(kvp.Value))
                        {
                            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
                        }
                    }

                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"SQL Error inserting data: {ex.Message}");
                MessageBox.Show($"SQL Error inserting data: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error inserting data: {ex.Message}");
                MessageBox.Show($"General Error inserting data: {ex.Message}");
                return false;
            }
            finally
            {
                warehouseConnection.Close();
            }
        }
    }
}
