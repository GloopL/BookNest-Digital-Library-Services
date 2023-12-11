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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BookNest_Digital_Library_Services
{
    public partial class Registeredstudents : Form
    {
        public Registeredstudents()
        {
            InitializeComponent();
            LoadRegisteredStudents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            var adminmenu = new Adminmenu();
            adminmenu.Show();
        }

        private void LoadRegisteredStudents()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    string query = "SELECT * FROM students";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable studentsTable = new DataTable();
                            adapter.Fill(studentsTable);

                            // Bind the DataTable to the DataGridView
                            dataGridView1.DataSource = studentsTable;

                            // Make the DataGridView read-only
                            dataGridView1.ReadOnly = true;

                            // Make all columns read-only
                            foreach (DataGridViewColumn column in dataGridView1.Columns)
                            {
                                column.ReadOnly = true;

                                // Hide the Password and loggedIn columns
                                if (column.Name == "Password" || column.Name == "loggedIn")
                                {
                                    column.Visible = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Search for students based on the entered search term
            string searchTerm = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                SearchStudents(searchTerm);
            }
            else
            {
                MessageBox.Show("Please enter a search term.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchStudents(string searchTerm)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    string query = "SELECT * FROM Students WHERE StudentID = @SearchTerm " +
                                   "OR FirstName LIKE @SearchTerm " +
                                   "OR LastName LIKE @SearchTerm " +
                                   "OR Program LIKE @SearchTerm " +
                                   "OR Section LIKE @SearchTerm " +
                                   "OR ContactNumber LIKE @SearchTerm " +
                                   "OR `SR-Code` LIKE @SearchTerm " +
                                   "OR Username LIKE @SearchTerm";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Convert the search term to an integer for StudentID
                        int searchStudentID;
                        bool isNumeric = int.TryParse(searchTerm, out searchStudentID);

                        if (isNumeric)
                        {
                            command.Parameters.AddWithValue("@SearchTerm", searchStudentID);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                        }

                        

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable searchResultTable = new DataTable();
                            adapter.Fill(searchResultTable);

                            

                            if (searchResultTable.Rows.Count == 0)
                            {
                                MessageBox.Show("No students found with the specified search term.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            // Bind the DataTable to the DataGridView
                            dataGridView1.DataSource = searchResultTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}
