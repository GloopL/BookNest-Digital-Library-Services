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
    public partial class Userform : Form
    {

        private MySqlConnection dbconnection;


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

        public Userform()
        {
            InitializeComponent();
            LoadBooksData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
            var usermenu = new Usermenu();
            usermenu.Show();
        }

        private void LoadBooksData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                   
                    string query = "SELECT * FROM Books WHERE Quantity > 0";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable booksTable = new DataTable();
                            adapter.Fill(booksTable);

                            // Bind the DataTable to the DataGridView
                            dataGridView1.DataSource = booksTable;

                            // Make the DataGridView read-only
                            dataGridView1.ReadOnly = true;

                            // Make all columns read-only
                            foreach (DataGridViewColumn column in dataGridView1.Columns)
                            {
                                column.ReadOnly = true;
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
            // Search for books based on the entered search term
            string searchTerm = textBox2.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                SearchBooks(searchTerm);
            }
            else
            {
                MessageBox.Show("Please enter a search term.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            }
        }
        private void SearchBooks(string searchTerm)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    string query = "SELECT * FROM Books WHERE (Title LIKE @SearchTerm OR Author LIKE @SearchTerm OR BookID LIKE @SearchTerm OR Genre LIKE @SearchTerm OR PublicationDate LIKE @SearchTerm) AND Quantity > 0";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable searchResultTable = new DataTable();
                            adapter.Fill(searchResultTable);

                            if (searchResultTable.Rows.Count == 0)
                            {
                                MessageBox.Show("No books found with the specified search term.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
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


        private void button4_Click(object sender, EventArgs e)
        {
            LoadBooksData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    // Get student ID where loggedin is true
                    int studentId;
                    string getStudentIdQuery = "SELECT StudentId FROM students WHERE loggedIn = true";

                    using (MySqlCommand getStudentIdCmd = new MySqlCommand(getStudentIdQuery, connection))
                    {
                        object result = getStudentIdCmd.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out studentId))
                        {
                            // Get book ID from the textbox
                            int bookId;

                            if (int.TryParse(textBox1.Text, out bookId))
                            {
                                // Check book quantity
                                string checkQuantityQuery = "SELECT quantity FROM books WHERE BookId = @bookId";

                                using (MySqlCommand checkQuantityCmd = new MySqlCommand(checkQuantityQuery, connection))
                                {
                                    checkQuantityCmd.Parameters.AddWithValue("@bookId", bookId);

                                    object quantityResult = checkQuantityCmd.ExecuteScalar();

                                    if (quantityResult != null && int.Parse(quantityResult.ToString()) > 0)
                                    {
                                        // Insert borrow record
                                        string insertBorrowQuery = "INSERT INTO borrowedbooks (bookid, studentid, borrowdate, Borrowedqty) " +
                                                                   "VALUES (@bookId, @studentId, @borrowDate, 1)";

                                        using (MySqlCommand insertBorrowCmd = new MySqlCommand(insertBorrowQuery, connection))
                                        {
                                            insertBorrowCmd.Parameters.AddWithValue("@bookId", bookId);
                                            insertBorrowCmd.Parameters.AddWithValue("@studentId", studentId);
                                            insertBorrowCmd.Parameters.AddWithValue("@borrowDate", DateTime.Now);

                                            insertBorrowCmd.ExecuteNonQuery();

                                            MessageBox.Show("Book borrowed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }

                                        // Update book quantity
                                        string updateQuantityQuery = "UPDATE books SET quantity = quantity - 1 WHERE bookid = @bookId";

                                        using (MySqlCommand updateQuantityCmd = new MySqlCommand(updateQuantityQuery, connection))
                                        {
                                            updateQuantityCmd.Parameters.AddWithValue("@bookId", bookId);

                                            updateQuantityCmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Book Unavailable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid book ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Student not found or not logged in.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
           
        }
    }
    
}
