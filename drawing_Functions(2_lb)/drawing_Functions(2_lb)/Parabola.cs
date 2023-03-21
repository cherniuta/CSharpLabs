using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drawing_Functions_2_lb_
{
    class Parabola:IFunction
    {
        public float calc(float x)
        {
            float y;

            y = x * x * 15;

            return y;
        }

        public float derivative(float x)
        {
            float y;

            y = 2 * x * 15;

            return y;
        }
    }
}
