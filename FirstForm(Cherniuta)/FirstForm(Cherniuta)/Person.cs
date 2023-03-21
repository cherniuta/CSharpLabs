using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FirstForm_Cherniuta_
{
    public class Person : IPerson
    {
        private Color color;
        public Color Color
        {
            get { return this.color; }
        }

        private readonly int cardNumber;
        public int CardNumber
        {
            get { return this.cardNumber; }
        }

        private readonly string name;
        public string Name
        {
            get { return this.name; }
        }

        private readonly DateTime birthday;
        public DateTime Birthday
        {
            get { return this.birthday; }
        }

        public Person(int cardNumber, string name, DateTime birthday, Color color)
        {
            this.cardNumber = cardNumber;
            this.name = name;
            this.birthday = birthday;
            this.color = color;
        }
        public int calcAge(DateTime date)
        {
            return date.Year - this.Birthday.Year - 1 +
        ((date.Month > this.Birthday.Month || date.Month == this.Birthday.Month && date.Day >= this.Birthday.Day) ? 1 : 0);
        }

        public string toString()
        {
            return calcAge(DateTime.Today).ToString();
        }

        public string allToString()
        {
            return Name + "\t" + Birthday + "\t" + CardNumber + "\t" + Color.ToArgb();
        }
    }
}
