using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace TULIPS
{
    public partial class CustomerForm : Form
    {
        private int currentCustomerID;
        private string username;
        private bool isCartVisible;
        private int cartWidth;
        private string connectionString = Properties.Settings.Default.Tulips_localDbConnectionString;

        public CustomerForm(string user)
        {
            InitializeComponent();
            username = user;
            isCartVisible = false;
            cartWidth = 300;
            lblWelcome.Text = "Welcome, " + username;
        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
            // Start with cart collapsed
            PanelCart.Width = 0;
            PanelCart.Dock = DockStyle.Right;
            PanelCart.Visible = true;

            // Setup DataGridView columns
            dgvCart.ColumnCount = 4;
            dgvCart.Columns[0].Name = "Flower Name";
            dgvCart.Columns[1].Name = "Price";
            dgvCart.Columns[2].Name = "Quantity";
            dgvCart.Columns[3].Name = "Subtotal";

            dgvCart.Columns[1].DefaultCellStyle.Format = "C";
            dgvCart.Columns[3].DefaultCellStyle.Format = "C";

            }

           
       

           
        private void UpdateCartTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.Cells["Subtotal"].Value != null)
                    total += Convert.ToDecimal(row.Cells["Subtotal"].Value);
            }
            lblTotal.Text = "Total: $" + total.ToString("0.00");
        }


        // Place order
        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
        }

        // Cart toggle
        private void btnCart_Click(object sender, EventArgs e)
        {
            CartTimer.Start();
        }

        private void CartTimer_Tick(object sender, EventArgs e)
        {
            if (!isCartVisible)
            {
                PanelCart.Width += 20;
                if (PanelCart.Width >= cartWidth)
                {
                    CartTimer.Stop();
                    isCartVisible = true;
                }
            }
            else
            {
                PanelCart.Width -= 20;
                if (PanelCart.Width <= 0)
                {
                    CartTimer.Stop();
                    isCartVisible = false;
                }
            }
        }
    }
}
