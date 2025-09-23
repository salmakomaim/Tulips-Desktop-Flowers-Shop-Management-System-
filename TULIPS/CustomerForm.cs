using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;

namespace TULIPS
{
    public partial class CustomerForm : Form
    {
        private string username;
        private int userId;
        private bool isCartVisible;
        private int cartWidth;
        private PrintDocument printDocument1 = new PrintDocument();
        private string receiptText = "";

        public CustomerForm(string user, int cashierId)
        {
            InitializeComponent();
            this.userId = cashierId;      // fix: assign the cashier ID
            this.username = user;
            isCartVisible = false;
            cartWidth = 300;
            lblWelcome.Text = $"Choose Your Happiness Bouquet, {username}";
        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
  
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Disable maximize button
            this.MaximizeBox = true;

            // Optional: allow minimize
            this.MinimizeBox = true;

            // Center the form on the screen
            this.StartPosition = FormStartPosition.CenterScreen;

            // Set the exact size you designed
            this.Width = 900;  // replace with your designer width
            this.Height = 1500;  // replace with your designer height

            // Reset first to avoid duplicates
            dgvCart.Columns.Clear();

            // Setup DataGridView columns
            dgvCart.Columns.Add("Flower", "Flower");
            dgvCart.Columns.Add("Quantity", "Quantity");
            dgvCart.Columns.Add("UnitPrice", "Unit Price");
            dgvCart.Columns.Add("TotalPrice", "Total Price");
            dgvCart.Columns.Add("ProductID", "Product ID");

            // Hide ProductID from user
            dgvCart.Columns["ProductID"].Visible = false;

            // Make columns read-only except Quantity
            dgvCart.Columns["Flower"].ReadOnly = true;
            dgvCart.Columns["UnitPrice"].ReadOnly = true;
            dgvCart.Columns["TotalPrice"].ReadOnly = true;
            dgvCart.Columns["Quantity"].ReadOnly = false;

            // Format prices
            dgvCart.Columns["UnitPrice"].DefaultCellStyle.Format = "C";
            dgvCart.Columns["TotalPrice"].DefaultCellStyle.Format = "C";

            // Attach PrintPage event once
            printDocument1.PrintPage += PrintDocument2_PrintPage;

            // Payment Methods
            cmbPaymentMethod.Items.Clear();
            cmbPaymentMethod.Items.Add("Cash");
            cmbPaymentMethod.Items.Add("Credit Card");
            cmbPaymentMethod.Items.Add("Debit Card");
            cmbPaymentMethod.Items.Add("Mobile Payment");
            cmbPaymentMethod.SelectedIndex = -1; // no default selection

            // Cart Panel
            PanelCart.Width = 500;
            
            PanelCart.Visible = false;
        }

        // ---------------- Cart Logic ----------------
        private void AddToCart(string flowerName, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells["Flower"].Value != null &&
                    row.Cells["Flower"].Value.ToString() == flowerName)
                {
                    int newQty = Convert.ToInt32(row.Cells["Quantity"].Value) + quantity;
                    row.Cells["Quantity"].Value = newQty;
                    row.Cells["TotalPrice"].Value = newQty * unitPrice;
                    UpdateCartTotal();
                    return;
                }
            }

            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice);
            UpdateCartTotal();
        }

        private void UpdateCartTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.IsNewRow) continue;
                total += Convert.ToDecimal(row.Cells["TotalPrice"].Value);
            }
            lblTotal.Text = $"Total: ${total}";
        }

        // ---------------- Cart Panel ----------------
        private void btnCart_Click(object sender, EventArgs e)
        {

            PanelCart.Visible = true;
            CartTimer.Start();
        }

        private void CartTimer_Tick(object sender, EventArgs e)
        {
            if (!isCartVisible)
            {
                PanelCart.Width += 20;
                if (PanelCart.Width >= cartWidth) { CartTimer.Stop(); isCartVisible = true; }
            }
            else
            {
                PanelCart.Width -= 20;
                if (PanelCart.Width <= 0) { CartTimer.Stop(); isCartVisible = false; }
            }
        }

        // PrintPage event handler
        private void PrintDocument2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font font = new Font("Courier New", 10); // Monospaced for alignment
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;

            e.Graphics.DrawString(receiptText, font, Brushes.Black, leftMargin, topMargin);
        }


        // ---------------- Receipt ----------------

        private void btnViewReceipt_Click(object sender, EventArgs e)
        {
            // 1️⃣ Validations
            if (dgvCart.Rows.Count == 0 || dgvCart.Rows[0].IsNewRow)
            {
                MessageBox.Show("Your cart is empty! Please add items to view the receipt.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txbAddress.Text))
            {
                MessageBox.Show("Please enter a delivery address.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cmbPaymentMethod.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a payment method.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2️⃣ Build receipt text
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🌸 Tulips Flower Shop 🌸");
            sb.AppendLine("Order Receipt");
            sb.AppendLine("------------------------------");
            sb.AppendLine($"Cashier: {username}");
            sb.AppendLine($"Address: {txbAddress.Text}");
            sb.AppendLine($"Payment Method: {cmbPaymentMethod.SelectedItem}");
            sb.AppendLine("------------------------------");
            sb.AppendLine("Item\tQty\tUnit Price\tTotal");

            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.IsNewRow) continue;

                string flower = row.Cells["Flower"].Value.ToString();
                int qty = Convert.ToInt32(row.Cells["Quantity"].Value);
                decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                decimal total = Convert.ToDecimal(row.Cells["TotalPrice"].Value);

                sb.AppendLine($"{flower}\t{qty}\t{unitPrice:C}\t{total:C}");
                totalAmount += total;
            }

            sb.AppendLine("------------------------------");
            sb.AppendLine($"Total Amount: {totalAmount:C}");
            sb.AppendLine("Thank you for shopping with us!");

            // Store receipt for printing
            receiptText = sb.ToString();

            // 3️⃣ Show Print Preview Dialog
            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = printDocument1,
                Width = 800,
                Height = 600
            };
            printPreviewDialog1.Document = printDocument1;
            previewDialog.ShowDialog();

            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // 🟢 Step 1: Validate stock for all cart items
                    foreach (DataGridViewRow row in dgvCart.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string flowerName = row.Cells["Flower"].Value.ToString();
                        int productId;

                        using (SqlCommand cmdId = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@name", con, transaction))
                        {
                            cmdId.Parameters.AddWithValue("@name", flowerName);
                            object resultId = cmdId.ExecuteScalar();
                            if (resultId == null)
                            {
                                MessageBox.Show($"Product '{flowerName}' not found in Products table!");
                                transaction.Rollback();
                                return;
                            }
                            productId = Convert.ToInt32(resultId);
                        }
                        int orderedQuantity = Convert.ToInt32(row.Cells["Quantity"].Value);

                        SqlCommand checkStock = new SqlCommand(
                            "SELECT StockQuantity FROM Products WHERE ProductID = @ProductID",
                            con, transaction);
                        checkStock.Parameters.AddWithValue("@ProductID", productId);

                        object result = checkStock.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show($"Product ID {productId} not found!");
                            transaction.Rollback();
                            return;
                        }

                        int stock = Convert.ToInt32(result);
                        if (stock == 0)
                        {
                            MessageBox.Show($"Product ID {productId} is out of stock!");
                            transaction.Rollback();
                            return;
                        }
                        if (stock < orderedQuantity)
                        {
                            MessageBox.Show($"Not enough stock for Product ID {productId}. Available: {stock}, Ordered: {orderedQuantity}");
                            transaction.Rollback();
                            return;
                        }
                    }

                    // 🟢 Step 2: Insert into Orders table (header)
                    SqlCommand insertOrder = new SqlCommand(
                        @"INSERT INTO Orders (CashierID, OrderDate, PaymentMethod, Address, TotalAmount)
                  OUTPUT INSERTED.OrderID
                  VALUES (@CashierID, GETDATE(), @PaymentMethod, @Address, @TotalAmount)",
                        con, transaction);

                    insertOrder.Parameters.AddWithValue("@CashierID", username);
                    insertOrder.Parameters.AddWithValue("@PaymentMethod", cmbPaymentMethod.SelectedItem.ToString());
                    insertOrder.Parameters.AddWithValue("@Address", txbAddress.Text);

                    // calculate total from cart
                    decimal TotalAmount = 0;
                    foreach (DataGridViewRow row in dgvCart.Rows)
                    {
                        if (row.IsNewRow) continue;
                        totalAmount += Convert.ToDecimal(row.Cells["TotalPrice"].Value);
                    }
                    insertOrder.Parameters.AddWithValue("@TotalAmount", totalAmount);

                    int orderId = (int)insertOrder.ExecuteScalar(); // get new OrderID

                    // 🟢 Step 3: Insert into OrderDetails + update stock
                    foreach (DataGridViewRow row in dgvCart.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int productId = Convert.ToInt32(row.Cells["ProductID"].Value);
                        int orderedQuantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                        decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                        decimal totalPrice = Convert.ToDecimal(row.Cells["TotalPrice"].Value);

                        // Insert order detail
                        SqlCommand insertDetail = new SqlCommand(
                            @"INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice, TotalPrice)
                      VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice, @TotalPrice)",
                            con, transaction);
                        insertDetail.Parameters.AddWithValue("@OrderID", orderId);
                        insertDetail.Parameters.AddWithValue("@ProductID", productId);
                        insertDetail.Parameters.AddWithValue("@Quantity", orderedQuantity);
                        insertDetail.Parameters.AddWithValue("@UnitPrice", unitPrice);
                        insertDetail.Parameters.AddWithValue("@TotalPrice", totalPrice);
                        insertDetail.ExecuteNonQuery();

                        // Update stock
                        SqlCommand updateStock = new SqlCommand(
                            "UPDATE Products SET StockQuantity = StockQuantity - @Quantity WHERE ProductID = @ProductID",
                            con, transaction);
                        updateStock.Parameters.AddWithValue("@Quantity", orderedQuantity);
                        updateStock.Parameters.AddWithValue("@ProductID", productId);
                        updateStock.ExecuteNonQuery();
                    }

                    // 🟢 Step 4: Commit transaction
                    transaction.Commit();
                    MessageBox.Show($"Order #{orderId} confirmed! Stock updated.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        // ---------------- Clear Cart ----------------
        private void btnClearCart_Click(object sender, EventArgs e)
        {
            dgvCart.Rows.Clear();
            UpdateCartTotal();
        }

        // ---------------- Remove Selected Item ----------------
        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count > 0)
            {
                dgvCart.Rows.RemoveAt(dgvCart.SelectedRows[0].Index);
                UpdateCartTotal();
            }
            else
            {
                MessageBox.Show("Please select an item to remove.");
            }
        }

        // ---------------- Confirm Order ----------------
        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            // 1️⃣ Validate cart
            if (dgvCart.Rows.Count == 0 || dgvCart.Rows[0].IsNewRow)
            {
                MessageBox.Show("You must add at least one item before confirming the order!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate delivery address and payment method
            string deliveryAddress = txbAddress.Text.Trim();
            string paymentMethod = cmbPaymentMethod.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(deliveryAddress) || string.IsNullOrEmpty(paymentMethod))
            {
                MessageBox.Show("Please enter delivery address and select a payment method.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2️⃣ Calculate total
            decimal totalAmount = 0;
            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.IsNewRow) continue;
                totalAmount += Convert.ToDecimal(row.Cells["TotalPrice"].Value);
            }

            // 3️⃣ Insert order and order details using transaction
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Insert into Orders table
                        string orderQuery = @"
                    INSERT INTO Orders (CashierID, OrderDate, TotalAmount, DeliveryAddress, PaymentMethod, Status)
                    OUTPUT INSERTED.OrderID
                    VALUES (@CashierID, @OrderDate, @TotalAmount, @DeliveryAddress, @PaymentMethod, @Status)";

                        SqlCommand orderCmd = new SqlCommand(orderQuery, con, transaction);
                        orderCmd.Parameters.AddWithValue("@CashierID", this.userId);
                        orderCmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        orderCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        orderCmd.Parameters.AddWithValue("@DeliveryAddress", deliveryAddress);
                        orderCmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                        orderCmd.Parameters.AddWithValue("@Status", "Pending");

                        int orderId = Convert.ToInt32(orderCmd.ExecuteScalar());

                        // Insert each cart item into OrderDetails and update stock
                        foreach (DataGridViewRow row in dgvCart.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string flowerName = row.Cells["Flower"].Value.ToString();
                            int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                            decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);

                            // Get ProductID and current stock
                            int productId;
                            int currentStock;
                            using (SqlCommand cmdId = new SqlCommand("SELECT ProductID, StockQuantity FROM Products WHERE Name=@name", con, transaction))
                            {
                                cmdId.Parameters.AddWithValue("@name", flowerName);
                                using (SqlDataReader reader = cmdId.ExecuteReader())
                                {
                                    if (!reader.Read())
                                    {
                                        MessageBox.Show($"Product '{flowerName}' not found in Products table!");
                                        transaction.Rollback();
                                        return;
                                    }

                                    productId = reader.GetInt32(0);
                                    currentStock = reader.GetInt32(1);
                                }
                            }

                            // Check stock before placing order
                            if (currentStock < quantity)
                            {
                                MessageBox.Show($"Not enough stock for '{flowerName}'. Available: {currentStock}, Requested: {quantity}.", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                transaction.Rollback();
                                return;
                            }

                            // Insert into OrderDetails
                            string detailQuery = @"
                        INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice)
                        VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice)";

                            using (SqlCommand detailCmd = new SqlCommand(detailQuery, con, transaction))
                            {
                                detailCmd.Parameters.AddWithValue("@OrderID", orderId);
                                detailCmd.Parameters.AddWithValue("@ProductID", productId);
                                detailCmd.Parameters.AddWithValue("@Quantity", quantity);
                                detailCmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                                detailCmd.ExecuteNonQuery();
                            }

                            // Update stock
                            string updateStockQuery = "UPDATE Products SET StockQuantity = StockQuantity - @Quantity WHERE ProductID = @ProductID";
                            using (SqlCommand updateStock = new SqlCommand(updateStockQuery, con, transaction))
                            {
                                updateStock.Parameters.AddWithValue("@Quantity", quantity);
                                updateStock.Parameters.AddWithValue("@ProductID", productId);
                                updateStock.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();

                        // 4️⃣ Clear cart and reset form
                        dgvCart.Rows.Clear();
                        UpdateCartTotal();
                        txbAddress.Clear();
                        cmbPaymentMethod.SelectedIndex = -1;

                        MessageBox.Show("Order placed successfully! The admin can now view it.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error placing order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



        // ---------------- Flower Buttons ----------------
        private void btnAddRedRose_Click(object sender, EventArgs e)
        {
            string flowerName = "Red Rose";
            int quantity = (int)numRedRoseQty.Value;
            decimal unitPrice = 9;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
    }
        private void btnAddSunFlower_Click(object sender, EventArgs e)
        {
            string flowerName = "Sun Flower";
            int quantity = (int)numSunFlowerQty.Value;
            decimal unitPrice = 4;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddPinkTulips_Click(object sender, EventArgs e)
        {
            string flowerName = "Pink Tulips";
            int quantity = (int)numPinkTulipsQty.Value;
            decimal unitPrice = 5;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddWhiteRose_Click(object sender, EventArgs e)
        {
            string flowerName = "White Rose";
            int quantity = (int)numWhiteRoseQty.Value;
            decimal unitPrice = 5;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddOrchids_Click(object sender, EventArgs e)
        {
            string flowerName = "Orchids";
            int quantity = (int)numOrchidsQty.Value;
            decimal unitPrice = 15;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddDaisies_Click(object sender, EventArgs e)
        {
            string flowerName = "Daisies";
            int quantity = (int)numDaisiesQty.Value;
            decimal unitPrice = 2;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddPurpleRose_Click(object sender, EventArgs e)
        {
            string flowerName = "Purple Rose";
            int quantity = (int)numPurpleRoseQty.Value;
            decimal unitPrice = 9;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddPinkRose_Click(object sender, EventArgs e)
        {
            string flowerName = "Pink Rose";
            int quantity = (int)numPinkRoseQty.Value;
            decimal unitPrice = 6;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddBlueRose_Click(object sender, EventArgs e)
        {
            string flowerName = "Blue Rose";
            int quantity = (int)numBlueRoseQty.Value;
            decimal unitPrice = 9;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddSpecialTulips_Click(object sender, EventArgs e)
        {
            string flowerName = "Special Tulips";
            int quantity = (int)numSpecialTulipsQty.Value;
            decimal unitPrice = 8;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddLilies_Click(object sender, EventArgs e)
        {
            string flowerName = "Lilies";
            int quantity = (int)numLiliesQty.Value;
            decimal unitPrice = 10;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }
        private void btnAddYellowTulips_Click(object sender, EventArgs e)
        {
            string flowerName = "Yellow Tulips";
            int quantity = (int)numYellowTulipsQty.Value;
            decimal unitPrice = 7;

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }

            // Get ProductID from database
            int productId;
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.Tulips_localDbConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products WHERE Name=@Name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", flowerName);
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // Add to DataGridView including hidden ProductID
            dgvCart.Rows.Add(flowerName, quantity, unitPrice, quantity * unitPrice, productId);

            UpdateCartTotal();
        }

        // ---------------- Optional: Numeric UpDown ValueChanged (can be empty) ----------------
        private void numRedRoseQty_ValueChanged(object sender, EventArgs e) { }
        private void numSunFlowerQty_ValueChanged(object sender, EventArgs e) { }
        private void numPinkTulipsQty_ValueChanged(object sender, EventArgs e) { }
        private void numWhiteRoseQty_ValueChanged(object sender, EventArgs e) { }
        private void numOrchidsQty_ValueChanged(object sender, EventArgs e) { }
        private void numDaisiesQty_ValueChanged(object sender, EventArgs e) { }
        private void numPurpleRoseQty_ValueChanged(object sender, EventArgs e) { }
        private void numPinkRoseQty_ValueChanged(object sender, EventArgs e) { }
        private void numBlueRoseQty_ValueChanged(object sender, EventArgs e) { }
        private void numSpecialTulipsQty_ValueChanged(object sender, EventArgs e) { }
        private void numLiliesQty_ValueChanged(object sender, EventArgs e) { }
        private void numYellowTulipsQty_ValueChanged(object sender, EventArgs e) { }

        // ---------------- Optional: ComboBox / MaskedTextBox Events ----------------
        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txbAddress_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) { }

        private void dgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void printDocument2_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(receiptText, new Font("Courier New", 12), Brushes.Black, new PointF(10, 10));
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void CustomerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void PanelCart_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CustomerForm_Resize(object sender, EventArgs e)
        {
         
        }
    }
}

