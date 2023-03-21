using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace drawing_Functions_2_lb_
{
    public partial class BackgroundStyle : Form
    {
        Form1 form1;
        public BackgroundStyle(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                form1.setСolor(colorDialog.Color);
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();
            ColorDialog colorDialog2 = new ColorDialog();
            Color color1, color2;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                color1 = colorDialog1.Color;
                
                if(colorDialog2.ShowDialog() == DialogResult.OK)
                {
                    color2 = colorDialog2.Color;
                    form1.setGradient(true,color1,color2);
                    form1.Refresh();
                    this.Close();
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            form1.setHatch(true);
            form1.Refresh();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            form1.setImage();
            this.Close();
        }
    }
}
