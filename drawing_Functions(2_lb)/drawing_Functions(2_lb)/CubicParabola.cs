using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drawing_Functions_2_lb_
{
    class CubicParabola:IFunction
    {
        public float calc(float x)
        {
            float y;

            y = x * x * x * 15;

            return y;
        }

        public float derivative(float x)
        {
            float y;

            y = 3 * x * x * 15;

            return y;
        }
    }
}
