using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FirstForm_Cherniuta_
{
    public partial class DataForm : Form
    {
        Form1 form1;

        private bool isAdmin;
        private bool selectedAccount;
        private string patternName;
        private string patternСardNumber;
        private Color ColorDataForm;
        int count = -1;


        public DataForm(Form1 form1)
        {
            InitializeComponent();

            this.BackColor = Color.Pink;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.textBox2.MaxLength = 5;
            this.dateTimePicker1.MaxDate = DateTime.Today;

            this.form1 = form1;
            
            isAdmin = false;
            selectedAccount = false;
            ColorDataForm = Color.White;

            patternName = @"([A-ZА-Я][a-zа-я]*\s{0,1}){3}";
            patternСardNumber = @"[0-9]{5}";

        }
        public void setColorDataForm(Color ColorDataForm)
        {
            this.ColorDataForm = ColorDataForm;
            this.pictureBox1.BackColor = ColorDataForm;
        }
        public void setIsAdmin(bool isAdmin)
        {
            this.isAdmin = isAdmin;
        }

        public void setSelectedAccount(bool selectedAccount)
        {
            this.selectedAccount = selectedAccount;
        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void ValueOutput(Person person)
        {
            textBox1.Text = person.Name;

            dateTimePicker1.Value = person.Birthday;
            dateTimePicker1.Enabled = false;

            textBox2.Text = person.CardNumber.ToString();
            textBox2.Enabled = false;


            this.BackColor = person.Color;

            
        }

        public void unblock()
        {
            this.dateTimePicker1.Enabled = true;
            this.textBox2.Enabled = true;

        }

        public void ClearForm()
        {
            textBox1.Enabled = true;
            textBox1.Clear();

            dateTimePicker1.Enabled = true;
            DateTime now = DateTime.Today;
            dateTimePicker1.Value = now;

            textBox2.Enabled = true;
            textBox2.Clear();
        }

        private Color usingColor()
        {
            ColorDialog colorDialog= new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.BackColor = colorDialog.Color;
                return colorDialog.Color;
            }
            else
                return this.ColorDataForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {  
            if ((dateTimePicker1.Enabled) == false)
            {
                if (Regex.IsMatch(textBox1.Text,patternName))
                {
                    
                    if (form1 != null)
                    {
                        
                        form1.Change(textBox1.Text,dateTimePicker1.Value,isAdmin,ColorDataForm);

                        Close();
                       

                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("Неверно введенные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                if ((this.isAdmin) == true)
                {
                    if (Regex.IsMatch(textBox1.Text, patternName))
                    {
                        if (form1 != null)
                        {
                            form1.Change(textBox1.Text, dateTimePicker1.Value, isAdmin,ColorDataForm);
                            Close();
                        }
                    }
                }
                else
                {

                    if (Regex.IsMatch(textBox1.Text, patternName) && Regex.IsMatch(textBox2.Text, patternСardNumber)&&((form1.uniqueCardNumber(textBox2.Text))==true))
                    {

                        Person person = new Person(Int32.Parse(textBox2.Text), textBox1.Text, dateTimePicker1.Value,ColorDataForm);

                        if (form1 != null)
                        {
                            this.count++;
                            form1.AddPerson(person);
                        }

                        Close();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("Неверно введенные данные", "Ошибка", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
            
            
            
        }


        private void textBox2_KeyPressed(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if ((e.KeyChar <= 47 || e.KeyChar >= 58)&& e.KeyChar!=8)
            {
                e.Handled = true;
                
            }
            
        }

        private void textBox1_KeyPressed(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if ((e.KeyChar > 47 && e.KeyChar < 58) && e.KeyChar != 8)
            {
                e.Handled = true;

            }
        }

        private void DataForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (dateTimePicker1.Enabled == false)
            {
                if (e.Control && e.Shift && e.KeyCode == Keys.L)
                {
                    Account account = new Account(this);
                    account.Show();
                }
            }
        }

        private void button1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            button1.Left = 255;
            button1.Top = 453;

            if ((this.isAdmin) == true)
            {
                if ((form1.compareCardNumber(textBox2.Text))==false)
                {
                    Random r = new Random();
                    button1.Left = r.Next(0, this.ClientSize.Width - button1.Width);
                    button1.Top = r.Next(0, this.ClientSize.Height - button1.Height);
                }
            }

            
        }

        private void dateTimePicker1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
            {
                if (DateTime.Parse(Clipboard.GetText()) <= DateTime.Today)
                {
                    dateTimePicker1.Value = DateTime.Parse(Clipboard.GetText());
                    e.Handled = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           this.ColorDataForm= this.usingColor();
        }
    }
}
