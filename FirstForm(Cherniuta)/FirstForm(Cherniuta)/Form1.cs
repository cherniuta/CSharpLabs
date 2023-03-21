using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FirstForm_Cherniuta_
{
    public partial class Form1 : Form
    {
        private List<Person> people = new List<Person>();
        ImageList imageList = new ImageList();
        

        private string path = @"C:\Users\Аня Черняева\Desktop\2 курс\лабы с#\FirstForm(Cherniuta)\fileInfo.txt";
        private bool readFromFile = false;
        private int countFromFile = -1;
        
        public int access;

        DataForm dataForm;

        public Form1()
        {
            InitializeComponent();

            this.BackColor = Color.Violet;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            ListView listView1 = new ListView();
            FileInfo fileInf = new FileInfo(path);

            dataForm = new DataForm(this);

            string[] strok = File.ReadAllLines(path);

            if (strok.Length != 0)
            {
                this.readFile();
            }
            
            
        }
         
        public void AddPerson(Person person)
        {
            if (!readFromFile)
            {
                this.countFromFile++;
            }

            people.Add(person);

            imageList.ImageSize = new Size(50, 20);
            Bitmap ColorImage1 = new Bitmap(50, 20);

            using (Graphics gr=Graphics.FromImage(ColorImage1))
            {
                gr.Clear(person.Color);
            }

            imageList.Images.Add(ColorImage1);
            listView1.SmallImageList = imageList;

            ListViewItem listViewItem = new ListViewItem(new string[] { "", person.Name,person.toString() });
            listViewItem.ImageIndex = this.countFromFile;
            listView1.Items.Add(listViewItem);
            
            if (!readFromFile)
            { 
                File.AppendAllText(path, person.allToString() + "\n");
            }

        }

        public void Change(string newName,DateTime newBirthday,bool selectedAccount, Color color)
        {
            int i = listView1.FocusedItem.Index;
            Person newPeople;

            if ((selectedAccount) == false)
            {

                newPeople = new Person(people[i].CardNumber, newName, people[i].Birthday,color);

            }
            else
            {
                newPeople = new Person(people[i].CardNumber, newName, newBirthday,color);
            }

            string text = File.ReadAllText(path);
            text = text.Replace(people[i].allToString(),newPeople.allToString());
            File.WriteAllText(path, text);

            people.RemoveAt(i);
            people.Insert(i, newPeople);
            Bitmap newColorImage = new Bitmap(50, 20);

            using (Graphics gr = Graphics.FromImage(newColorImage))
            {
                gr.Clear(newPeople.Color);
            }
            
            
            imageList.Images[i] = newColorImage;
            listView1.SmallImageList = this.imageList;
            
            ListViewItem listViewItem = new ListViewItem(new string[] { "",newPeople.Name, newPeople.toString() });
            listViewItem.ImageIndex = i;
            listView1.Items.RemoveAt(i);
            listView1.Items.Insert(i, listViewItem);

        }
        private void readFile()
        {
            StreamReader fs = new StreamReader(path);
            string line;
            Person person;
            
            readFromFile = true;
            
            while ((line = fs.ReadLine()) != null)
            {
                string[] splitLine = line.Split('\t');
                
                string Name = splitLine[0] ;
                DateTime Birthday = Convert.ToDateTime(splitLine[1]);
                int CardNumber = Convert.ToInt32(splitLine[2]);
                Color color = Color.FromArgb(Convert.ToInt32(splitLine[3]));
                
                person = new Person(CardNumber, Name, Birthday, color);
                this.countFromFile++;
                this.AddPerson(person);
                
            }
            
            fs.Close();
            
            readFromFile = false;
        }
        
        public bool compareCardNumber(string cardNumber)
        {
            if((people[listView1.FocusedItem.Index].CardNumber.ToString())==cardNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool uniqueCardNumber(string cardNumber)
        {
            for(int index=0;index<=this.countFromFile;index++)
            {
                if (cardNumber == people[index].CardNumber.ToString())
                    return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataForm.setIsAdmin(false);
            dataForm.BackColor = Color.Pink;
            dataForm.setColorDataForm(Color.White);

            dataForm.Owner = this;// родитель 
            dataForm.ClearForm();
            dataForm.ShowDialog();

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if ((listView1.SelectedItems.Count)!=0)// кол-во выбранных элементов
            {
                dataForm.setIsAdmin(false);
                dataForm.setColorDataForm(people[listView1.FocusedItem.Index].Color);
                
                dataForm.Owner = this;// родитель 
                dataForm.ValueOutput(people[listView1.FocusedItem.Index]);
                dataForm.ShowDialog();

                dataForm.ClearForm();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if ((listView1.SelectedItems.Count) != 0)
            {
               DialogResult result =  MessageBox.Show("Удалить данную запись?",
                                                      "Сообщение",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question,
                                                      MessageBoxDefaultButton.Button2,
                                                      MessageBoxOptions.DefaultDesktopOnly);
                if(result==DialogResult.Yes)
                {
                    string text = File.ReadAllText(path);
                    text = text.Remove(text.IndexOf(people[this.listView1.FocusedItem.Index].allToString()), people[this.listView1.FocusedItem.Index].allToString().Length + 1);
                    File.WriteAllText(path, text);

                    this.people.RemoveAt(this.listView1.FocusedItem.Index);
                    this.listView1.Items.RemoveAt(this.listView1.FocusedItem.Index);

                    this.countFromFile--;
                }
              
            }
        }

        
    }
    
}
