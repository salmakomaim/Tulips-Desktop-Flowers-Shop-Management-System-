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
            txb_userName.Text = "";
            txb_password.Text = "";
            lblMessage.Text = "";
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

                    if (dbPassword == password)
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
                else
                {
                    reader.Close();

                    if (username.ToLower().Contains("admin"))
                    {
                        lblMessage.Text = "Invalid admin username!";
                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Visible = true;
                        return;
                    }

                    if (Regex.IsMatch(username, @"^\d+$"))
                    {
                        lblMessage.Text = "Invalid username: cannot be only numbers!";
                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Visible = true;
                        return;
                    }

                    // Auto-register customer
                    string insertQuery = "INSERT INTO Users (Username, Password, Role) VALUES (@u, @p, 'Customer')";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@u", username);
                    insertCmd.Parameters.AddWithValue("@p", password);
                    insertCmd.ExecuteNonQuery();

                    CustomerForm customer = new CustomerForm(username);
                    customer.Show();
                    this.Hide();
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
            string error = null;
            if (((Control)sender).Text.Trim().Length == 0)
            {
                error = "This field is required";
                e.Cancel = true;
            }
            errorProvider1.SetError((Control)sender, error);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txb_password_validating(object sender, CancelEventArgs e)
        {
            string error = null;
            if (((Control)sender).Text.Trim().Length == 0)
            {
                error = "This field is required";
                e.Cancel = true;
            }
            errorProvider1.SetError((Control)sender, error);
        }
    }
    }

