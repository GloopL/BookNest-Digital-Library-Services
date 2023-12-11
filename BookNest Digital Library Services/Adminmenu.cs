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
    public partial class Adminmenu : Form
    {
        public Adminmenu()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
            var login = new Form1();
            login.Show();
            MessageBox.Show("Logged out successfully!.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            var books = new Adminform();
            books.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
            var borrowedbooks = new Borrowedbooks();
            borrowedbooks.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            var registeredstudents = new Registeredstudents();
            registeredstudents.Show();
        }
    }
}
