using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

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
            


        }

        private void welcome_label_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void but_reset_Click(object sender, EventArgs e)
        {
            this.AutoValidate = AutoValidate.Disable; // Temporarily disable validation
            txb_userName.Text = "";
            txb_password.Text = "";
            lblMessage.Text = "";
            errorProvider1.Clear(); // clear all error messages
            this.AutoValidate = AutoValidate.EnableAllowFocusChange; // Re-enable validation
        }

        private void but_login_Click(object sender, EventArgs e)
        {
            string username = txb_userName.Text.Trim();
            string password = txb_password.Text.Trim();

            // Check empty fields
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Please enter username and password.";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
                return;
            }

            // length password rules
            if (password.Length < 4 || !Regex.IsMatch(password, @"\d"))
            {
                lblMessage.Text = "Password must be at least 4 characters and contain a number!";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
                return;
            }

            // Username validation
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
                string query = "SELECT Password, Role FROM Users WHERE Username=@u";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@u", username);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string dbPassword = reader["Password"].ToString();
                    string role = reader["Role"].ToString();

                    reader.Close();

                    // Check if password is hashed (simple check)
                    bool isHashed = dbPassword.Length == 64 && Regex.IsMatch(dbPassword, @"\A\b[0-9a-fA-F]+\b\Z");

                    if (!isHashed)
                    {
                        // Hash the existing password
                        string hashedAdminPassword = PasswordHasher.HashPassword(dbPassword);

                        // Update database
                        string updateQuery = "UPDATE Users SET Password=@p WHERE Username=@u";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                        {
                            updateCmd.Parameters.AddWithValue("@p", hashedAdminPassword);
                            updateCmd.Parameters.AddWithValue("@u", username);
                            updateCmd.ExecuteNonQuery();
                        }

                        dbPassword = hashedAdminPassword; // update the variable to continue login
                    }

                    // Now check login
                    if (PasswordHasher.VerifyPassword(password, dbPassword))
                    {
                        if (role == "Admin")
                        {
                            pnlMain admin = new pnlMain();
                            admin.Show();
                        }
                        else
                        {
                            CustomerForm customer = new CustomerForm(username);
                            customer.Show();
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

       

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void lblMessage_Click(object sender, EventArgs e)
        {

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
            if (this.AutoValidate == AutoValidate.Disable) return; // allow reset to skip validation

            string error = null;
            if (((Control)sender).Text.Trim().Length == 0)
            {
                error = "This field is required";
                e.Cancel = true; // stop leaving if empty
            }
            errorProvider1.SetError((Control)sender, error);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txb_password_validating(object sender, CancelEventArgs e)
        {
            if (this.AutoValidate == AutoValidate.Disable) return; // allow reset to skip validation

            string error = null;
            if (((Control)sender).Text.Trim().Length == 0)
            {
                error = "This field is required";
                e.Cancel = true; // stop leaving if empty
            }
            errorProvider1.SetError((Control)sender, error);
        }
    }
    }
    

