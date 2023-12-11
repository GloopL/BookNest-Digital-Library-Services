using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookNest_Digital_Library_Services
{
    public partial class Form1 : Form
    {
        private MySqlConnection dbconnection;
        
        
        private void InitializeDatabaseConnection() 
        {
            
            try
            {
                // Set up the connection string with MySQL configuration
                dbconnection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=''");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in database connection: " + ex.Message);
            }
        }
        public Form1()
        {
            InitializeComponent();
        }




        private void button2_Click(object sender, EventArgs e)
        {
            // Retrieve username and password from textboxes
            string username = textBox1.Text;
            string password = textBox2.Text;

            bool isLoggedIn;

            // Check if the user is a student and update the 'LoggedIn' status in the database
            if (IsStudent(username, password, out isLoggedIn))
            {
                if (isLoggedIn)
                {
                    MessageBox.Show("Student login successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Hide();
                    var student = new Usermenu();
                    student.Show();
                    
                }
                else
                {
                    MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
            else
            if (IsAdmin(username, password))
            {
                MessageBox.Show("Admin login successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Hide();
                var admin = new Adminmenu();
                admin.Show();
                
            }
            else
                {
                    MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            
        }

        private bool IsAdmin(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            // Check admin credentials in the database
            using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM admins WHERE Username = @Username AND Password = @Password";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the hashed bytes to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private bool IsStudent(string username, string password, out bool isLoggedIn)
        {
            // Hash the entered password using SHA256
            string hashedPassword = HashPassword(password);

            // Check student credentials in the database
            using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM students WHERE Username = @Username AND Password = @Password";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // Check if the user is logged in and update the database accordingly
                    if (count > 0)
                    {
                        isLoggedIn = true;
                        UpdateLoggedInStatus(username, true);
                    }
                    else
                    {
                        isLoggedIn = false;
                    }

                    return count > 0;
                }
            }
        }

        private void UpdateLoggedInStatus(string username, bool isLoggedIn)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();
                    string query = "UPDATE students SET loggedIn = @LoggedIn WHERE Username = @Username";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@LoggedIn", isLoggedIn);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();
            var register = new Register();
            register.Show();
        }
    }
}
    

    

