using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TULIPS
{
    public partial class pnlMain : Form
    {
        private Color originalColor;
        private string connectionString = Properties.Settings.Default.Tulips_localDbConnectionString;

        public pnlMain()
        {
            InitializeComponent();
            ;

        }

        private void LoadOrders()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Orders";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvOrders.DataSource = dt;
            }

            dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvOrders.ScrollBars = ScrollBars.Both;
            dgvOrders.AutoSize = false;
            dgvOrders.AllowUserToResizeColumns = true;
        }

        private void LoadProducts()
        {
            string connString = Properties.Settings.Default.Tulips_localDbConnectionString;

            using (SqlConnection con = new SqlConnection(connString))
            {
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Products", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvProducts.DataSource = dt;
            }
        }

        private string receiptText = "";
        private void AdminForm_Load(object sender, EventArgs e)
        {
            LoadProducts();
            LoadOrders();

            // ===== Style dgvProducts =====
            dgvProducts.DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // fills the panel width
            dgvProducts.DefaultCellStyle.Padding = new Padding(5);
            dgvProducts.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProducts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvOrders.DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            dgvOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // fills the panel width
            dgvOrders.DefaultCellStyle.Padding = new Padding(5);
            dgvOrders.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvOrders.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void btnNavProducts_Click(object sender, EventArgs e)
        {
            PanelProducts.BringToFront();

        }

        private void btnNavOrders_Click(object sender, EventArgs e)
        {
            PanelOrders.BringToFront();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            BringToFront();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {



            // Confirm logout
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Close AdminForm
                this.Hide();

                // Show LoginForm
                LoginForm login = new LoginForm();
                login.Show();
            }

        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            int orderID;
            if (dgvOrders.SelectedRows.Count > 0)
            {
                orderID = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
            }
            else
            {
                MessageBox.Show("Please select an order first.");
                return;
            }

            if (dgvOrders.CurrentRow != null)
            {
                int orderId = Convert.ToInt32(dgvOrders.CurrentRow.Cells["OrderID"].Value);
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Orders WHERE OrderID = @OrderID", con);
                    cmd.Parameters.AddWithValue("@OrderID", orderId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadOrders();
                MessageBox.Show($"Order {orderId} has been cancelled!");
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            // Example: Let user select CSV to add multiple products
            openFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                // Load CSV data into dgvProducts or process it for adding products
            }

            // Or continue with normal Add Product functionality
            try
            {
                string Name = InputBox.Show("Enter product name:");
                string Category = InputBox.Show("Enter category:");
                string PriceStr = InputBox.Show("Enter unit price:");
                string StockStr = InputBox.Show("Enter stock quantity:");

                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Category) ||
                    string.IsNullOrWhiteSpace(PriceStr) || string.IsNullOrWhiteSpace(StockStr))
                {
                    MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(PriceStr, out decimal Price))
                {
                    MessageBox.Show("Invalid price entered.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(StockStr, out int Stock))
                {
                    MessageBox.Show("Invalid stock quantity entered.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Products (Name, Category, Price, Stock) VALUES (@Name, @Category, @Price, @Stock)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Category", Category);
                        cmd.Parameters.AddWithValue("@Price", Price);
                        cmd.Parameters.AddWithValue("@Stock", Stock);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadProducts(); // Refresh DataGridView
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }




            string name = InputBox.Show("Enter product name:");
            string category = InputBox.Show("Enter category:");
            string priceStr = InputBox.Show("Enter unit price:");
            string stockStr = InputBox.Show("Enter stock quantity:");

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category)
                || !decimal.TryParse(priceStr, out decimal price)
                || !int.TryParse(stockStr, out int stock))
            {
                MessageBox.Show("Invalid input. Please try again.");
                return;
            }

            string connString = Properties.Settings.Default.Tulips_localDbConnectionString;

            using (SqlConnection con = new SqlConnection(connString))
            {
                con.Open();
                string query = "INSERT INTO Products (Name, Category, UnitPrice, StockQuantity) VALUES (@n, @c, @p, @s)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@c", category);
                cmd.Parameters.AddWithValue("@p", price);
                cmd.Parameters.AddWithValue("@s", stock);
                cmd.ExecuteNonQuery();
            }

        LoadProducts();  // refresh grid
        }

        public static class InputBox
        {
            public static string Show(string prompt, string title = "Input")
            {
                Form form = new Form();
                Label label = new Label();
                TextBox textBox = new TextBox();
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                form.Text = title;
                label.Text = prompt;

                label.SetBounds(9, 20, 372, 13);
                textBox.SetBounds(12, 36, 372, 20);
                buttonOk.Text = "OK";
                buttonCancel.Text = "Cancel";
                buttonOk.SetBounds(228, 72, 75, 23);
                buttonCancel.SetBounds(309, 72, 75, 23);

                buttonOk.DialogResult = DialogResult.OK;
                buttonCancel.DialogResult = DialogResult.Cancel;

                form.ClientSize = new System.Drawing.Size(396, 107);
                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                DialogResult dialogResult = form.ShowDialog();
                return dialogResult == DialogResult.OK ? textBox.Text : null;
            }

            public static string Show(string prompt, string title, string defaultValue)
            {
                Form form = new Form();
                Label label = new Label();
                TextBox textBox = new TextBox();
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                form.Text = title;
                label.Text = prompt;

                textBox.Text = defaultValue; // <-- prefill current value

                label.SetBounds(9, 20, 372, 13);
                textBox.SetBounds(12, 36, 372, 20);
                buttonOk.Text = "OK";
                buttonCancel.Text = "Cancel";
                buttonOk.SetBounds(228, 72, 75, 23);
                buttonCancel.SetBounds(309, 72, 75, 23);

                buttonOk.DialogResult = DialogResult.OK;
                buttonCancel.DialogResult = DialogResult.Cancel;

                form.ClientSize = new Size(396, 107);
                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                DialogResult dialogResult = form.ShowDialog();
                return dialogResult == DialogResult.OK ? textBox.Text : null;
            }
        }

        private void btnEditProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                MessageBox.Show("Please select a product to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get current product values
            int productId = Convert.ToInt32(dgvProducts.CurrentRow.Cells["ProductID"].Value);
            string currentName = dgvProducts.CurrentRow.Cells["Name"].Value.ToString();
            string currentCategory = dgvProducts.CurrentRow.Cells["Category"].Value.ToString();
            string currentPrice = dgvProducts.CurrentRow.Cells["UnitPrice"].Value.ToString();
            string currentStock = dgvProducts.CurrentRow.Cells["StockQuantity"].Value.ToString();

            // Ask user for new values
            string newName = InputBox.Show("Edit product name:", "Edit Product", currentName);
            string newCategory = InputBox.Show("Edit category:", "Edit Product", currentCategory);
            string newPriceStr = InputBox.Show("Edit unit price:", "Edit Product", currentPrice);
            string newStockStr = InputBox.Show("Edit stock quantity:", "Edit Product", currentStock);

            // Handle user canceling input
            if (newName == null || newCategory == null || newPriceStr == null || newStockStr == null)
                return; // exit if user canceled

            // Validate name and category
            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newCategory))
            {
                MessageBox.Show("Name and Category cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate price
            if (!decimal.TryParse(newPriceStr, out decimal newPrice))
            {
                MessageBox.Show("Invalid price entered.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (newPrice <= 0)
            {
                MessageBox.Show("Price must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate stock
            if (!int.TryParse(newStockStr, out int newStock))
            {
                MessageBox.Show("Invalid stock quantity entered.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (newStock < 0)
            {
                MessageBox.Show("Stock cannot be negative.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm before updating
            var confirm = MessageBox.Show("Are you sure you want to update this product?",
                                          "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            // Update the product in the database
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE Products SET Name=@n, Category=@c, UnitPrice=@p, StockQuantity=@s WHERE ProductID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@n", newName);
                    cmd.Parameters.AddWithValue("@c", newCategory);
                    cmd.Parameters.AddWithValue("@p", newPrice);
                    cmd.Parameters.AddWithValue("@s", newStock);
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                }

                // Refresh grid
                LoadProducts();

                // Highlight updated row
                foreach (DataGridViewRow row in dgvProducts.Rows)
                {
                    if (Convert.ToInt32(row.Cells["ProductID"].Value) == productId)
                    {
                        row.Selected = true;
                        dgvProducts.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }

                MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Check if a product is selected
                if (dgvProducts.CurrentRow == null)
                {
                    MessageBox.Show("Please select a product to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Get selected product info
                int productId = Convert.ToInt32(dgvProducts.CurrentRow.Cells["ProductID"].Value);
                string productName = dgvProducts.CurrentRow.Cells["Name"].Value.ToString();

                // 3. Ask for confirmation
                var confirm = MessageBox.Show($"Are you sure you want to delete '{productName}'?",
                                              "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                // 4. Delete from database
                string connString = Properties.Settings.Default.Tulips_localDbConnectionString;
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();
                    string query = "DELETE FROM Products WHERE ProductID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                }

                // 5. Refresh grid
                LoadProducts();

                // 6. Optional: Highlight the next row
                if (dgvProducts.Rows.Count > 0)
                {
                    int rowIndex = Math.Min(dgvProducts.CurrentRow.Index, dgvProducts.Rows.Count - 1);
                    dgvProducts.CurrentCell = dgvProducts[0, rowIndex];
                    dgvProducts.Rows[rowIndex].Selected = true;
                }

                MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while deleting the product:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateStock_Click(object sender, EventArgs e)
        {
            // Import updated stock
            openFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                // Update product stock from CSV
            }

            // Or export current stock
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                // Save current stock to CSV
            }
        

            try
            {
                if (dgvProducts.CurrentRow == null)
                {
                    MessageBox.Show("Please select a product to update stock.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int productId = Convert.ToInt32(dgvProducts.CurrentRow.Cells["ProductID"].Value);
                string productName = dgvProducts.CurrentRow.Cells["Name"].Value.ToString();
                int currentStock = Convert.ToInt32(dgvProducts.CurrentRow.Cells["StockQuantity"].Value);

                string input = InputBox.Show($"Update stock for {productName} (current: {currentStock}):", "Update Stock", currentStock.ToString());

                if (string.IsNullOrWhiteSpace(input)) return; // canceled

                if (!int.TryParse(input, out int newStock) || newStock < 0)
                {
                    MessageBox.Show("Invalid stock quantity!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string connString = Properties.Settings.Default.Tulips_localDbConnectionString;
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();
                    string query = "UPDATE Products SET StockQuantity=@s WHERE ProductID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@s", newStock);
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                }

                LoadProducts();

                MessageBox.Show("Stock updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PanelOrders_Click(object sender, EventArgs e)
        {

        }

        private void btnGenerateInvoice_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                // Load CSV data into dgvProducts or process it for adding products
            }

            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an order first.");
                return;
            }

            // Get order data from selected row
            int orderId = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
            string cashierId = dgvOrders.SelectedRows[0].Cells["CashierID"].Value.ToString();
            string date = dgvOrders.SelectedRows[0].Cells["OrderDate"].Value.ToString();
            string payment = dgvOrders.SelectedRows[0].Cells["PaymentMethod"].Value.ToString();
            string status = dgvOrders.SelectedRows[0].Cells["Status"].Value.ToString();
            string total = dgvOrders.SelectedRows[0].Cells["TotalAmount"].Value.ToString();

            // Store the data to use in PrintPage
            receiptText = $"TULIPS FLOWER SHOP\n";
            receiptText += $"-----------------------------\n";
            receiptText += $"Order ID: {orderId}\n";
            receiptText += $"Cashier ID: {cashierId}\n";
            receiptText += $"Date: {date}\n";
            receiptText += $"Payment: {payment}\n";
            receiptText += $"Status: {status}\n";
            receiptText += $"-----------------------------\n";
            receiptText += $"Total Amount: ${total}\n";
            receiptText += $"-----------------------------\n";
            receiptText += $"Thank you for your order!\n";

            // Show print preview
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }
    
        private void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            int orderID;
            if (dgvOrders.SelectedRows.Count > 0)
            {
                orderID = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
            }
            else
            {
                MessageBox.Show("Please select an order first.");
                return;
            }

            if (dgvOrders.CurrentRow != null)
            {
                int orderId = Convert.ToInt32(dgvOrders.CurrentRow.Cells["OrderID"].Value);
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Orders SET Status='Delivered' WHERE OrderID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", orderId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadOrders();
                MessageBox.Show($"Order {orderId} status updated to Delivered!");
            }
        }

        private void btnViewOrderDetails_Click(object sender, EventArgs e)
        {
            int orderID;
            if (dgvOrders.SelectedRows.Count > 0)
            {
                orderID = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
            }
            else
            {
                MessageBox.Show("Please select an order first.");
                return;
            }

            if (dgvOrders.CurrentRow != null)
            {
                int orderId = Convert.ToInt32(dgvOrders.CurrentRow.Cells["OrderID"].Value);
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Orders WHERE OrderID=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", orderId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        MessageBox.Show(
                            $"OrderID: {reader["OrderID"]}\n" +
                            $"CashierID: {reader["CashierID"]}\n" +
                            $"OrderDate: {reader["OrderDate"]}\n" +
                            $"DeliveryAddress: {reader["DeliveryAddress"]}\n" +
                            $"PaymentMethod: {reader["PaymentMethod"]}\n" +
                            $"Status: {reader["Status"]}\n" +
                            $"TotalAmount: ${reader["TotalAmount"]}"
                        );
                    }
                }
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString(receiptText, new Font("Courier New", 12), Brushes.Black, new PointF(10, 10));
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AdminForm_FormClosing(object sender, FormClosingEventArgs e)
        {
       
            Environment.Exit(0);
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void pnlMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }

        }
    






