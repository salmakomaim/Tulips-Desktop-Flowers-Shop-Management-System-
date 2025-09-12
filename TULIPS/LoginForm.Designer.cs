
namespace TULIPS
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.but_close = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.but_reset = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txb_userName = new System.Windows.Forms.TextBox();
            this.txb_password = new System.Windows.Forms.TextBox();
            this.but_login = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.but_close);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(381, 126);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1057, 764);
            this.panel1.TabIndex = 1;
            // 
            // but_close
            // 
            this.but_close.BackColor = System.Drawing.Color.Transparent;
            this.but_close.Font = new System.Drawing.Font("Tahoma", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_close.ForeColor = System.Drawing.Color.Red;
            this.but_close.Location = new System.Drawing.Point(964, 17);
            this.but_close.Name = "but_close";
            this.but_close.Size = new System.Drawing.Size(80, 73);
            this.but_close.TabIndex = 4;
            this.but_close.Text = "X";
            this.but_close.UseVisualStyleBackColor = false;
            this.but_close.Click += new System.EventHandler(this.but_close_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.lblMessage);
            this.groupBox1.Controls.Add(this.but_reset);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txb_userName);
            this.groupBox1.Controls.Add(this.txb_password);
            this.groupBox1.Controls.Add(this.but_login);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(44, 162);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(965, 552);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(60, 332);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 33);
            this.lblMessage.TabIndex = 9;
            this.lblMessage.Click += new System.EventHandler(this.lblMessage_Click);
            // 
            // but_reset
            // 
            this.but_reset.CausesValidation = false;
            this.but_reset.Font = new System.Drawing.Font("Tahoma", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_reset.ForeColor = System.Drawing.Color.HotPink;
            this.but_reset.Location = new System.Drawing.Point(484, 410);
            this.but_reset.Name = "but_reset";
            this.but_reset.Size = new System.Drawing.Size(254, 71);
            this.but_reset.TabIndex = 6;
            this.but_reset.Text = "Reset";
            this.but_reset.UseVisualStyleBackColor = true;
            this.but_reset.Click += new System.EventHandler(this.but_reset_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Tahoma", 11.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.HotPink;
            this.label2.Location = new System.Drawing.Point(58, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 46);
            this.label2.TabIndex = 1;
            this.label2.Text = "User Name";
            // 
            // txb_userName
            // 
            this.errorProvider1.SetError(this.txb_userName, "User Name is required !");
            this.txb_userName.Font = new System.Drawing.Font("Tahoma", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txb_userName.Location = new System.Drawing.Point(66, 110);
            this.txb_userName.Name = "txb_userName";
            this.txb_userName.Size = new System.Drawing.Size(672, 47);
            this.txb_userName.TabIndex = 1;
            this.txb_userName.TextChanged += new System.EventHandler(this.txb_userName_TextChanged);
            this.txb_userName.Validating += new System.ComponentModel.CancelEventHandler(this.txb_userName_Validating);
            // 
            // txb_password
            // 
            this.errorProvider1.SetError(this.txb_password, "Password is required !");
            this.txb_password.Font = new System.Drawing.Font("Tahoma", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txb_password.Location = new System.Drawing.Point(66, 261);
            this.txb_password.Name = "txb_password";
            this.txb_password.PasswordChar = '*';
            this.txb_password.Size = new System.Drawing.Size(672, 47);
            this.txb_password.TabIndex = 2;
            this.txb_password.TextChanged += new System.EventHandler(this.txb_password_TextChanged);
            this.txb_password.Validating += new System.ComponentModel.CancelEventHandler(this.txb_password_validating);
            // 
            // but_login
            // 
            this.but_login.Font = new System.Drawing.Font("Tahoma", 9.900001F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.but_login.ForeColor = System.Drawing.Color.HotPink;
            this.but_login.Location = new System.Drawing.Point(66, 410);
            this.but_login.Name = "but_login";
            this.but_login.Size = new System.Drawing.Size(265, 72);
            this.but_login.TabIndex = 3;
            this.but_login.Text = "Login";
            this.but_login.UseVisualStyleBackColor = true;
            this.but_login.Click += new System.EventHandler(this.but_login_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.HotPink;
            this.label3.Location = new System.Drawing.Point(58, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(215, 48);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Mistral", 26.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.HotPink;
            this.label1.Location = new System.Drawing.Point(409, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 105);
            this.label1.TabIndex = 0;
            this.label1.Text = "Login";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.but_login;
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 33F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1873, 1177);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoginForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txb_password;
        private System.Windows.Forms.TextBox txb_userName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button but_close;
        private System.Windows.Forms.Button but_login;
        private System.Windows.Forms.Button but_reset;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}