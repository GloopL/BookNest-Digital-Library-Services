using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookNest_Digital_Library_Services
{
    public partial class Usermenu : Form
    {
        public Usermenu()
        {
            InitializeComponent();
        }

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

        private void UpdateLoggedInStatus(bool isLoggedIn)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();
                    string query = "UPDATE students SET loggedIn = @LoggedIn WHERE loggedIn = True";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
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

        public static class CurrentUser
        {
            public static bool loggedin { get; private set; }

            public static void SetUser(bool isLoggedIn)
            {
                loggedin = isLoggedIn;
            }

            public static void ClearUser()
            {
                loggedin = false;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            // Get the username of the currently logged-in user
            bool isloggedIn = CurrentUser.loggedin;
            // Check if the user is logged in before attempting to log out
            if (isloggedIn = true)
            {
                // Update the 'LoggedIn' status to false in the database
                UpdateLoggedInStatus(false);

                // Clear the current user information
                CurrentUser.ClearUser();

                // Close the current form and show the login form
                Close();
                var login = new Form1();
                login.Show();
                MessageBox.Show("Logged out successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No user is currently logged in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            var books = new Userform();
            books.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            var returnbook = new Returnbooks();
            returnbook.Show();
        }
    }
}
