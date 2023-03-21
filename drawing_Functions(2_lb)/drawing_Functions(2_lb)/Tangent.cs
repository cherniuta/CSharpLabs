using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Data;

namespace drawing_Functions_2_lb_
{
    class Tangent:IFunction
    {
        
        public float calc(float x)
        {
            float y;

            y = (float)(Math.Tan(x) * 15);

            return y;
        }

        public float derivative(float x)
        {
            float y;

            y = (1 / ((float)Math.Cos(x) * (float)Math.Cos(x)))*15;

            return y;
        } 
    }
}
