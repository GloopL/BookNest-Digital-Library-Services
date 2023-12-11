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
    public partial class Borrowedbooks : Form
    {
        public Borrowedbooks()
        {
            InitializeComponent();
            LoadTransactionsBooks();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            var adminmenu = new Adminmenu();
            adminmenu.Show();
        }

        private void LoadTransactionsBooks()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    string query = "SELECT transactions.TransactionId, " +
                                   "transactions.BookId, " +
                                   "books.title AS BookTitle, " +
                                   "transactions.StudentID, " +
                                   "CONCAT(students.firstname, ' ', students.lastname) AS StudentsFullName, " +
                                   "transactions.BorrowDate, " +
                                   "transactions.ReturnDate " +
                                   "FROM transactions " +
                                   "JOIN books ON transactions.bookid = books.bookid " +
                                   "JOIN students ON transactions.studentid = students.studentid";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable borrowedbooksTable = new DataTable();
                            adapter.Fill(borrowedbooksTable);

                            // Bind the DataTable to the DataGridView
                            dataGridView1.DataSource = borrowedbooksTable;

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
            // Search for transactions based on the entered search term
            string searchTerm = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                SearchTransactions(searchTerm);
            }
            else
            {
                MessageBox.Show("Please enter a search term.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchTransactions(string searchTerm)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    string query = "SELECT transactions.TransactionId, " +
                                   "transactions.BookId, " +
                                   "books.title AS BookTitle, " +
                                   "transactions.StudentID, " +
                                   "CONCAT(students.firstname, ' ', students.lastname) AS StudentsFullName, " +
                                   "transactions.BorrowDate, " +
                                   "transactions.ReturnDate " +
                                   "FROM transactions " +
                                   "JOIN books ON transactions.BookId = books.BookId " +
                                   "JOIN students ON transactions.StudentId = students.StudentId " +
                                   "WHERE transactions.TransactionID = @SearchTerm " +
                                   "   OR books.title LIKE @SearchTerm " +
                                   "   OR CONCAT(students.firstname, ' ', students.lastname) LIKE @SearchTerm " +
                                   "   OR DATE(transactions.BorrowDate) = DATE(@SearchTerm) " +
                                   "   OR DATE(transactions.ReturnDate) = DATE(@SearchTerm)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", searchTerm);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable searchResultTable = new DataTable();
                            adapter.Fill(searchResultTable);

                            if (searchResultTable.Rows.Count == 0)
                            {
                                MessageBox.Show("No transactions found with the specified search term.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            // Preserve existing columns in the DataGridView
                            foreach (DataGridViewColumn column in dataGridView1.Columns)
                            {
                                if (!searchResultTable.Columns.Contains(column.Name))
                                {
                                    Type colType = column.ValueType ?? typeof(string);
                                    searchResultTable.Columns.Add(column.Name, colType);
                                }
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
