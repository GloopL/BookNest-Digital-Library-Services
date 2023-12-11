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
    public partial class Adminform : Form
    {
        public Adminform()
        {
            InitializeComponent();
            LoadBooksData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
            var adminmenu = new Adminmenu();
            adminmenu.Show();
        }

        private void LoadBooksData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    string query = "SELECT * FROM Books";

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


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    string title = textBox2.Text;
                    string author = textBox3.Text;
                    string genre = textBox4.Text;
                    DateTime publicationDate = Convert.ToDateTime(dateTimePicker1.Text);
                    int quantity = Convert.ToInt32(textBox6.Text);

                    // Insert or update the book in the books table using the composite key (Title, Author)
                    string insertOrUpdateQuery = "INSERT INTO books (Title, Author, Genre, PublicationDate, Quantity) " +
                                                 "VALUES (@Title, @Author, @Genre, @PublicationDate, @Quantity) " +
                                                 "ON DUPLICATE KEY UPDATE Quantity = Quantity + @Quantity";

                    using (MySqlCommand command = new MySqlCommand(insertOrUpdateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Author", author);
                        command.Parameters.AddWithValue("@Genre", genre);
                        command.Parameters.AddWithValue("@PublicationDate", publicationDate);
                        command.Parameters.AddWithValue("@Quantity", quantity);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Book added/updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to add/update book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    string title = textBox2.Text;
                    string author = textBox3.Text;
                    string genre = textBox4.Text;
                    DateTime publicationDate;
                    int quantity;

                    // Check if the admin provided any input
                    if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(author) ||
                        !string.IsNullOrEmpty(genre) || DateTime.TryParse(dateTimePicker1.Text, out publicationDate) || int.TryParse(textBox6.Text, out quantity))
                    {
                        // Construct the base query
                        string updateQuery = "UPDATE books SET ";

                        // Add conditions for each provided input
                        if (!string.IsNullOrEmpty(title))
                            updateQuery += "Title = @Title, ";

                        if (!string.IsNullOrEmpty(author))
                            updateQuery += "Author = @Author, ";

                        if (!string.IsNullOrEmpty(genre))
                            updateQuery += "Genre = @Genre, ";

                        if (DateTime.TryParse(dateTimePicker1.Text, out publicationDate))
                            updateQuery += "PublicationDate = @PublicationDate, ";

                        if (int.TryParse(textBox6.Text, out quantity))
                            updateQuery += "Quantity = @Quantity, ";

                        // Remove the trailing comma and space
                        updateQuery = updateQuery.TrimEnd(' ', ',');

                        // Add a condition for updating based on bookID
                        updateQuery += " WHERE BookID = @BookID";

                        using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@BookID", Convert.ToInt32(textBox1.Text));

                            if (!string.IsNullOrEmpty(title))
                                command.Parameters.AddWithValue("@Title", title);

                            if (!string.IsNullOrEmpty(author))
                                command.Parameters.AddWithValue("@Author", author);

                            if (!string.IsNullOrEmpty(genre))
                                command.Parameters.AddWithValue("@Genre", genre);

                            if (DateTime.TryParse(dateTimePicker1.Text, out publicationDate))
                                command.Parameters.AddWithValue("@PublicationDate", publicationDate);

                            if (int.TryParse(textBox6.Text, out quantity))
                                command.Parameters.AddWithValue("@Quantity", quantity);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Book information updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Failed to update book information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No input provided. Please enter information to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //refresh the gridview
        private void button5_Click(object sender, EventArgs e)
        {
            LoadBooksData();
        }

        

        private void SearchBooks(string searchTerm)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=localhost;port=3306;username=root;password=;database=librarymanagementsystem"))
                {
                    connection.Open();

                    
                    string query = "SELECT * FROM Books WHERE BookID LIKE @SearchTerm OR Title LIKE @SearchTerm OR Author LIKE @SearchTerm OR Genre LIKE @SearchTerm OR PublicationDate LIKE @SearchTerm OR Quantity LIKE @SearchTerm";

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

        private void button3_Click(object sender, EventArgs e)
        {
            // Search for books based on the entered search term
            string searchTerm = textBox5.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                SearchBooks(searchTerm);
            }
            else
            {
                MessageBox.Show("Please enter a search term.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
    }
}
