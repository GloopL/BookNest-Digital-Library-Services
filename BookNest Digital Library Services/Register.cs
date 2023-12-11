using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookNest_Digital_Library_Services
{
    public partial class Register : Form
    {
        private MySqlConnection dbconnection;
        public Register()
        {
            InitializeComponent();
        }

        private void InitializeDatabaseConnection() // --
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
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            var login = new Form1();
            login.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get user input
            string firstName = textBox1.Text;
            string lastName = textBox2.Text;
            string program = textBox3.Text;
            string section = textBox4.Text;
            string contactNo = textBox5.Text;
            string srCode = textBox6.Text;
            string username = textBox7.Text;
            string password = textBox8.Text;
            string reEnterPassword = textBox9.Text;

            // Check if passwords match
            if (password != reEnterPassword)
            {
                MessageBox.Show("Passwords do not match. Please re-enter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Hash the password using SHA256
            string hashedPassword = HashPassword(password);

            // Save the user data to the database
            if (RegisterStudent(firstName, lastName, program, section, contactNo, srCode, username, hashedPassword))
            {
                MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                
                Close();
                var login = new Form1();
                login.Show();
            }
            else
            {
                MessageBox.Show("Registration failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private bool RegisterStudent(string firstName, string lastName, string program, string section, string contactNo, string srCode, string username, string hashedPassword)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    // Insert the student data into the Students table
                    MySqlCommand command = new MySqlCommand("INSERT INTO librarymanagementsystem.students (`FirstName`, `LastName`, `Program`, `Section`, `ContactNumber`, `SR-Code`, `Username`, `Password`) VALUES (@FirstName, @LastName, @Program, @Section, @ContactNo, @SrCode, @Username, @Password)", connection);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Program", program);
                    command.Parameters.AddWithValue("@Section", section);
                    command.Parameters.AddWithValue("@ContactNo", contactNo);
                    command.Parameters.AddWithValue("@SrCode", srCode);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
