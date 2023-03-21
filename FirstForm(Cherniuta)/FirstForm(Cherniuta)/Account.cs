using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace FirstForm_Cherniuta_
{
    public partial class Account : Form
    {
        DataForm dataForm;

        public string result = "";

        private bool selectedAccount;
        
        public Account(DataForm dataForm)
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.comboBox1.Items.Add("User");
            this.comboBox1.Items.Add("Admin");

            this.comboBox1.SelectedIndex = 0;
            this.textBox1.PasswordChar = '*';

            
            this.result = "db8f188d203f0c6167013c57604274d5";

            this.dataForm = dataForm;

        }
        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex == 0)
            {
                this.textBox1.Clear();
                this.textBox1.Enabled = false;
            }
            else
            {
                this.textBox1.Enabled = true;
            }
        }
        public int comparisonHash(string result)
        {
            byte[] hash = Encoding.ASCII.GetBytes(this.textBox1.Text);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashenc = md5.ComputeHash(hash);

            string resultInput = "";

            foreach (var b in hashenc)
            {
                resultInput += b.ToString("x2");
            }

            if (resultInput== result)
            {
                return 1;
            }
            else
            {
                return 0;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if ((comparisonHash(this.result))==1)
            {
                dataForm.setSelectedAccount(true);
                dataForm.setIsAdmin(true);
                dataForm.unblock();
                Close();
                
                
            }
            else
            {
                MessageBox.Show("Неверный пароль",
                                 "Сообщение",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error,
                                 MessageBoxDefaultButton.Button1,
                                 MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}
