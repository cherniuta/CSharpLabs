using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace drawing_Functions_2_lb_
{
    class PaintedObject:ICloneable
    {
        private GraphicsPath path;

        public GraphicsPath Path
        {
            get { return path; }
            set { path = value; }
        }
        private Pen pen;

        public Pen @Pen
        {
            get { return pen; }
            set { pen = value; }
        }
        public PaintedObject()
        {
            this.path = new GraphicsPath();
            this.pen = Pens.Black;
        }

        public PaintedObject(Pen pen, GraphicsPath path)
        {
            this.path = path.Clone() as GraphicsPath;
            this.pen = pen.Clone() as Pen;
        }

        #region ICloneable Members

        public object Clone()
        {
            return new PaintedObject(this.Pen.Clone() as Pen, this.Path.Clone() as GraphicsPath);
        }
        #endregion
    }
}

