using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TULIPS
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
       
        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Disable maximize button
            this.MaximizeBox = true;

            // Optional: allow minimize
            this.MinimizeBox = true;

            // Center the form on the screen
            this.StartPosition = FormStartPosition.CenterScreen;

            // Set the exact size you designed
            this.Width =900;  // replace with your designer width
            this.Height = 600;  // replace with your designer height
        }

        private void lblMessage_Click(object sender, EventArgs e)
        {

        }
        private void but_reset_Click(object sender, EventArgs e)
        {
            this.AutoValidate = AutoValidate.Disable;
            txb_userName.Text = "";
            txb_password.Text = "";
            lblMessage.Text = "";
            errorProvider1.Clear();
            this.AutoValidate = AutoValidate.EnableAllowFocusChange;
        }

        private void but_login_Click(object sender, EventArgs e)
        {
            string username = txb_userName.Text.Trim();
            string password = txb_password.Text.Trim();

            // 1. Validate inputs
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Please enter username and password.";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
                return;
            }

            if (password.Length < 4 || !Regex.IsMatch(password, @"\d"))
            {
                lblMessage.Text = "Password must be at least 4 characters and contain a number!";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
                return;
            }

            if (username.Length < 3)
            {
                lblMessage.Text = "Username must be at least 3 characters long.";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
                return;
            }

            if (username.Length > 30)
            {
                lblMessage.Text = "Username cannot be longer than 30 characters.";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
                return;
            }

            if (!Regex.IsMatch(username, @"^(?=.*[A-Za-z])[A-Za-z0-9]+$"))
            {
                lblMessage.Text = "Username must contain only letters or a mix of letters and numbers.";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
                return;
            }

            string connString = Properties.Settings.Default.Tulips_localDbConnectionString;

            using (SqlConnection con = new SqlConnection(connString))
            {
                con.Open();

                string query = "SELECT UserID, Password, Role FROM Users WHERE Username=@u";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@u", username);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) // user exists
                {
                    string dbPassword = reader["Password"].ToString();
                    string role = reader["Role"].ToString();
                    int userId = Convert.ToInt32(reader["UserID"]); // ✅ fetch ID here
                    reader.Close();

                    if (dbPassword == password)
                    {
                        if (role == "Admin")
                        {
                            pnlMain admin = new pnlMain();
                            admin.Show();
                        }
                        else
                        {
                            CustomerForm cashier = new CustomerForm(username, userId);
                            cashier.Show();
                        }
                        this.Hide();
                    }
                    else
                    {
                        lblMessage.Text = "Invalid password!";
                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Visible = true;
                    }
                }
                else // auto-register new customer
                {
                    reader.Close();

                    if (username.ToLower() == "admin")
                    {
                        lblMessage.Text = "This username is reserved. Please choose another.";
                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Visible = true;
                        return;
                    }

                    string insertQuery = "INSERT INTO Users (Username, Password, Role) OUTPUT INSERTED.UserID VALUES (@u, @p, 'Cashier')";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@u", username);
                    insertCmd.Parameters.AddWithValue("@p", password);

                    try
                    {
                        int newCashierId = Convert.ToInt32(insertCmd.ExecuteScalar()); // ✅ capture new ID

                        lblMessage.Text = "Registration successful! Logging you in...";
                        lblMessage.ForeColor = Color.Green;
                        lblMessage.Visible = true;

                        CustomerForm cashier = new CustomerForm(username, newCashierId);
                        cashier.Show();
                        this.Hide();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627) // unique constraint violation
                        {
                            lblMessage.Text = "Username already exists. Please choose another.";
                            lblMessage.ForeColor = Color.Red;
                            lblMessage.Visible = true;
                        }
                        else
                        {
                            lblMessage.Text = "Database error: " + ex.Message;
                            lblMessage.ForeColor = Color.Red;
                            lblMessage.Visible = true;
                        }

                        

                    }
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void but_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txb_userName_TextChanged(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        private void txb_password_TextChanged(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        private void txb_userName_Validating(object sender, CancelEventArgs e)
        {
            if (this.AutoValidate == AutoValidate.Disable) return;

            string error = null;
            if (((Control)sender).Text.Trim().Length == 0)
            {
                error = "This field is required";
                e.Cancel = true;
            }
            errorProvider1.SetError((Control)sender, error);
        }

        private void txb_password_validating(object sender, CancelEventArgs e)
        {
            if (this.AutoValidate == AutoValidate.Disable) return;

            string error = null;
            if (((Control)sender).Text.Trim().Length == 0)
            {
                error = "This field is required";
                e.Cancel = true;
            }
            errorProvider1.SetError((Control)sender, error);
        }

        private void LoginForm_Resize(object sender, EventArgs e)
        {
            this.Resize += LoginForm_Resize;
            loginPanel.Left = (this.ClientSize.Width - loginPanel.Width) / 2;
            loginPanel.Top = (this.ClientSize.Height - loginPanel.Height) / 2;
        }
    }
}