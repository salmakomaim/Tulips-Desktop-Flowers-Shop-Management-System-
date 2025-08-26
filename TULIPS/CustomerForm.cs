using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TULIPS
{
    public partial class CustomerForm : Form
    {
        private string username;
        
        public CustomerForm(string user)
        {
            InitializeComponent();
            username = user;
            lblWelcome.Text = "Welcome, " + username;
        }


        private void CustomerForm_Load(object sender, EventArgs e)
        {

        }
    }
}
