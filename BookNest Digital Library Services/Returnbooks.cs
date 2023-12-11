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
    public partial class Returnbooks : Form
    {
        public Returnbooks()
        {
            InitializeComponent();
            LoadBorrowedBooksData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            var usermenu = new Usermenu();
            usermenu.Show();
        }

        private void LoadBorrowedBooksData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    string query = "SELECT borrowedbooks.bookid, " +
                                   "books.title AS BookTitle, " +
                                   "CONCAT(students.firstname, ' ', students.lastname) AS StudentsFullName, " +
                                   "borrowedbooks.borrowdate " +
                                   "FROM borrowedbooks " +
                                   "JOIN books ON borrowedbooks.bookid = books.bookid " +
                                   "JOIN students ON borrowedbooks.studentid = students.studentid " +
                                   "WHERE students.loggedin = true"; 

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable borrowedBooksTable = new DataTable();
                            adapter.Fill(borrowedBooksTable);

                            // Bind the DataTable to the DataGridView
                            dataGridView1.DataSource = borrowedBooksTable;

                            // Make the DataGridView read-only
                            dataGridView1.ReadOnly = true;

                            // Make all columns read-only
                            foreach (DataGridViewColumn column in dataGridView1.Columns)
                            {
                                column.ReadOnly = true;
                            }

                            // Exclude specific columns from the DataGridView display
                            if (dataGridView1.Columns.Contains("BorrowID"))
                                dataGridView1.Columns["BorrowID"].Visible = false;

                            if (dataGridView1.Columns.Contains("StudentID"))
                                dataGridView1.Columns["StudentID"].Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadBorrowedBooksData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    int bookId = Convert.ToInt32(textBox1.Text);

                    // Check if the book is borrowed
                    string checkBorrowedQuery = "SELECT * FROM borrowedbooks WHERE bookid = @BookId";
                    using (MySqlCommand checkBorrowedCommand = new MySqlCommand(checkBorrowedQuery, connection))
                    {
                        checkBorrowedCommand.Parameters.AddWithValue("@BookId", bookId);

                        using (MySqlDataReader reader = checkBorrowedCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int studentId = reader.GetInt32("studentid");
                                DateTime borrowedDate = reader.GetDateTime("borrowdate");

                                // Close the DataReader before executing another query
                                reader.Close();

                                // Increase the quantity in the books table
                                string increaseQuantityQuery = "UPDATE books SET quantity = quantity + 1 WHERE bookid = @BookId";
                                using (MySqlCommand increaseQuantityCommand = new MySqlCommand(increaseQuantityQuery, connection))
                                {
                                    increaseQuantityCommand.Parameters.AddWithValue("@BookId", bookId);
                                    increaseQuantityCommand.ExecuteNonQuery();
                                }

                                // Insert the return transaction into the transactions table
                                string insertTransactionQuery = "INSERT INTO transactions (bookid, studentid, borrowdate, returndate) " +
                                                                "VALUES (@BookId, @StudentId, @BorrowDate, @ReturnDate)";
                                using (MySqlCommand insertTransactionCommand = new MySqlCommand(insertTransactionQuery, connection))
                                {
                                    insertTransactionCommand.Parameters.AddWithValue("@BookId", bookId);
                                    insertTransactionCommand.Parameters.AddWithValue("@StudentId", studentId);
                                    insertTransactionCommand.Parameters.AddWithValue("@BorrowDate", borrowedDate);
                                    insertTransactionCommand.Parameters.AddWithValue("@ReturnDate", DateTime.Now);

                                    insertTransactionCommand.ExecuteNonQuery();
                                }

                                // Remove the book entry from borrowedbooks table
                                string removeBorrowedBookQuery = "DELETE FROM borrowedbooks WHERE bookid = @BookId";
                                using (MySqlCommand removeBorrowedBookCommand = new MySqlCommand(removeBorrowedBookQuery, connection))
                                {
                                    removeBorrowedBookCommand.Parameters.AddWithValue("@BookId", bookId);
                                    removeBorrowedBookCommand.ExecuteNonQuery();
                                }

                                
                                MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                
                                MessageBox.Show("Book is not currently borrowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


    }
}
