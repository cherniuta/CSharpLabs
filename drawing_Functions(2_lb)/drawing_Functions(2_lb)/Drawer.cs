using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace drawing_Functions_2_lb_
{
    class Drawer
    {
        public PointF[] Draw(Panel panel,IFunction function,int formWidth,int formHeight)
        {
            Graphics graphics = panel.CreateGraphics();
            Pen pen = new Pen(Color.Black, 2);

            graphics.TranslateTransform(formWidth / 2, formHeight / 2);
            graphics.ScaleTransform(1, -1);
            
            PointF[] pointL = new PointF[2 * (formWidth/15)*100];
            int index = 0;
            for (double i = -formWidth/15; i < formWidth/15; i += 0.01)
            {
                if (index < 2 * (formWidth / 15) * 100)
                {
                    pointL[index].X = (float)i * 15;
                    pointL[index].Y = function.calc((float)i);
                    index++;
                }

            }


            return pointL;
        }
    }
}
