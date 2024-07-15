using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace ERP_Login
{
    internal class DBManager
    {
        private const string AuthConnectionString = "server=localhost;uid=root;pwd=MITDBR028!WBILUM@2003281955@;database=dblog";
        private MySqlConnection authConnection;

        public DBManager()
        {
            authConnection = new MySqlConnection(AuthConnectionString);
        }

        public bool ExecuteNonQuery(string query)
        {
            try
            {
                using (var cmd = new MySqlCommand(query, authConnection))
                {
                    if (authConnection.State != ConnectionState.Open)
                    {
                        authConnection.Open();
                    }
                    int rowsCount = cmd.ExecuteNonQuery();
                    return rowsCount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing non-query: {ex.Message}");
                return false;
            }
            finally
            {
                authConnection.Close();
            }
        }

        public bool RegisterUser(string username, string password, string role)
        {
            string query = "INSERT INTO user (username, password, role) VALUES (@username, @password, @role)";

            try
            {
                using (var cmd = new MySqlCommand(query, authConnection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password); // No hashing
                    cmd.Parameters.AddWithValue("@role", role);

                    if (authConnection.State != ConnectionState.Open)
                    {
                        authConnection.Open();
                    }

                    int rowsCount = cmd.ExecuteNonQuery();
                    return rowsCount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
            finally
            {
                authConnection.Close();
            }
        }

        public UserRole? Login(string username, string password)
        {
            string query = "SELECT role FROM user WHERE username = @username AND password = @password";

            try
            {
                using (var cmd = new MySqlCommand(query, authConnection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    if (authConnection.State != ConnectionState.Open)
                    {
                        authConnection.Open();
                    }

                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string role = result.ToString();
                        if (Enum.TryParse(role, true, out UserRole userRole))
                        {
                            return userRole;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in user: {ex.Message}");
            }
            finally
            {
                authConnection.Close();
            }

            return null;
        }
    }
}
