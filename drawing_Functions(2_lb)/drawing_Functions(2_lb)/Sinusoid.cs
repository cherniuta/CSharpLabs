using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace drawing_Functions_2_lb_
{
     class Sinusoid:IFunction
    {
        public float calc(float x)
        {
            float y;

            y = (float)(Math.Sin(x) * 15);

            return y;
        }

        public float derivative(float x)
        {
            float y;

            y = (float)Math.Cos(x)*15;

            return y;
        }
    }
}
