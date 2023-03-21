using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Drawing.Imaging;
using System.IO;


namespace drawing_Functions_2_lb_
{

    public partial class Form1 : Form
    {
        BackgroundStyle backgroundStyle;
        List<PaintedObject> list;//Список с объектами для прорисовки
        PaintedObject currObj;//Объект, который в данный момент перемещается

        Point oldPoint;
        
        Bitmap bmp;
        private Random rng;

      
        private List<IFunction> functions = new List<IFunction>();
        private Drawer drawer;

        private int index;
        private int oldIndex = 5;
        private int deltaX, deltaY;
        private bool gradient = false;
        private bool hatch = false;
        
        private Color color1, color2;
        
        int PIX_IN_ONE = 15;
        int ARR_LEN = 10;
        int SIZE_BRUSH = 6;
        const float SCALE_MUL = 2;
        int scale = 1;
        

        int formWidth;
        int formHeight;

        int lx = 0, ly = 0;
        int cx = 0, cy = 0;
        int k = 1;

        List<Drop> rain = new List<Drop>(); // keeps all drops in one place
        Random rnd = new Random();          // for generating random numbers
        Timer t = new Timer();
        private bool isRain = false;

        
        public Form1()
        {
            InitializeComponent();

            drawer = new Drawer();
            backgroundStyle = new BackgroundStyle(this);
            
            functions.Add(new Sinusoid());
            functions.Add(new StraightLine());
            functions.Add(new Parabola());
            functions.Add(new CubicParabola());
            functions.Add(new Tangent());

            list = new List<PaintedObject>();
            bmp = new Bitmap(panel1.Width+1000,panel1.Height+1000);
           

            rng = new Random();

            this.DoubleBuffered = true;

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty
            | BindingFlags.Instance | BindingFlags.NonPublic, null,
            panel1, new object[] { true });

            typeof(GroupBox).InvokeMember("DoubleBuffered", BindingFlags.SetProperty
            | BindingFlags.Instance | BindingFlags.NonPublic, null,
            groupBox1, new object[] { true });

           
            panel1.MouseDown += Panel1_MouseDown;
            panel1.MouseUp += Panel1_MouseUp;
            panel1.MouseMove += Panel1_MouseMove;
            panel1.MouseWheel += Panel1_MouseWheel;

            label1.Text = "";


            for (int i = 0; i < 150; i++) // creates 100 drops at random position and with random speed
                rain.Add(CreateRandomDrop());

            t.Interval = 10; // specify interval time as you want
            t.Tick += new EventHandler(timer_Tick);
            t.Start();

        }

        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            var K = e.Delta > 0 ? SCALE_MUL : 1 / SCALE_MUL;
            
            
            if (e.Delta>0)
            {
                if (PIX_IN_ONE < 120)
                {
                    PIX_IN_ONE = PIX_IN_ONE * 2;
                    SIZE_BRUSH = SIZE_BRUSH * 2;
                    scale += 1;
                    if (list.Count() > 0)
                    {
                        graphScale((float)K);
                    }
                    
                }
            }
            else
            {
                if (PIX_IN_ONE > 15)
                {
                    PIX_IN_ONE = PIX_IN_ONE / 2;
                    SIZE_BRUSH = SIZE_BRUSH / 2;
                    scale -= 1;
                    if(list.Count()>0)
                    {
                        graphScale((float)K);
                    }
                    
                }
            }

            if (scale !=1) 
            {
                label1.Text = "scale" + "=" + scale.ToString();
            }
            else
            {
                label1.Text = "";
            }
            
            this.Refresh();

        }
        private void graphScale(float K)
        {
            PaintedObject po = new PaintedObject();
            po = (PaintedObject)list[0].Clone();
            list.RemoveAt(0);

            po.Path.Transform(new Matrix(1, 0, 0, 1, -panel1.Width / 2, -panel1.Height / 2));
            po.Path.Transform(new Matrix(K, 0, 0, K, 0, 0));
            po.Path.Transform(new Matrix(1, 0, 0, 1, panel1.Width / 2, panel1.Height / 2));
            list.Add(po);
            RefreshBitmap();

        }

        private void panel1_Paint(Graphics e)
        {
            
            if (gradient==true)
            {
                e.Clear(Color.White);
                LinearGradientBrush gradBrush = new LinearGradientBrush(new Rectangle(0, 0, panel1.Width, panel1.Height), color1, color2, LinearGradientMode.Horizontal);
                e.FillRectangle(gradBrush, 0, 0, panel1.Width, panel1.Height); //с градиентной заливкой
                
            }
            
           if(hatch==true)
            {
                HatchBrush hBrush = new HatchBrush(HatchStyle.Horizontal,Color.Red,Color.White);
                
                e.FillRectangle(hBrush, 0, 0, panel1.Width, panel1.Height);
                
            }

            if (isRain == true)
            {
                UpdateRain();
                RenderRain(e);
            }

            Pen pen = new Pen(Color.Black, 1);

            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            int w = panel1.ClientSize.Width / 2;
            int h = panel1.ClientSize.Height / 2;

            
            e.TranslateTransform(w, h);
            

            DrawXAxis(new Point(k*-w, 0), new Point(k*w, 0), e);
            DrawYAxis(new Point(0,k* h), new Point(0,k* -h), e);

            
            e.FillEllipse(Brushes.Red, -2, -2, 4, 4);
            e.DrawEllipse(pen, -PIX_IN_ONE, -PIX_IN_ONE, 2*PIX_IN_ONE, 2*PIX_IN_ONE);


        }


        private void DrawXAxis(Point start, Point end, Graphics g)
        {

            //Деления в положительном направлении оси
            for (int i = PIX_IN_ONE; i < end.X - ARR_LEN; i += PIX_IN_ONE)
            {
                g.DrawLine(Pens.Black, i, -5, i, 5);
                DrawText(new Point(i, 5), (i / PIX_IN_ONE).ToString(), g);
            }
            //Деления в отрицательном направлении оси
            for (int i = -PIX_IN_ONE; i > start.X; i -= PIX_IN_ONE)
            {
                g.DrawLine(Pens.Black, i, -5, i, 5);
                DrawText(new Point(i, 5), (i / PIX_IN_ONE).ToString(), g);
            }
            //Ось
            g.DrawLine(Pens.Black, start, end);
            //Стрелка
            g.DrawLines(Pens.Black, GetArrow(start.X, start.Y, end.X, end.Y, ARR_LEN));
        }

        private void DrawYAxis(Point start, Point end, Graphics g)
        {
            //Деления в отрицательном направлении оси
            for (int i = PIX_IN_ONE; i < start.Y; i += PIX_IN_ONE)
            {
                g.DrawLine(Pens.Black, -5, i, 5, i);
                DrawText(new Point(5, i), (-i / PIX_IN_ONE).ToString(), g, true);
            }
            //Деления в положительном направлении оси
            for (int i = -PIX_IN_ONE; i > end.Y + ARR_LEN; i -= PIX_IN_ONE)
            {
                g.DrawLine(Pens.Black, -5, i, 5, i);
                DrawText(new Point(5, i), (-i / PIX_IN_ONE).ToString(), g, true);
            }
            //Ось
            g.DrawLine(Pens.Black, start, end);
            //Стрелка
            g.DrawLines(Pens.Black, GetArrow(start.X, start.Y, end.X, end.Y, ARR_LEN));
        }

        private void DrawText(Point point, string text, Graphics g, bool isYAxis = false)
        {
            var f = new Font(Font.FontFamily, SIZE_BRUSH);
            var size = g.MeasureString(text, f);
            var pt = isYAxis
                ? new PointF(point.X + 1, point.Y - size.Height / 2)
                : new PointF(point.X - size.Width / 2, point.Y + 1);
            var rect = new RectangleF(pt, size);
            
            g.DrawString(text, f, Brushes.Black, rect);
        }

        private static PointF[] GetArrow(float x1, float y1, float x2, float y2, float len = 10, float width = 4)
        {
            PointF[] result = new PointF[3];
            //направляющий вектор отрезка
            var n = new PointF(x2 - x1, y2 - y1);
            //Длина отрезка
            var l = (float)Math.Sqrt(n.X * n.X + n.Y * n.Y);
            //Единичный вектор
            var v1 = new PointF(n.X / l, n.Y / l);
            //Длина стрелки
            n.X = x2 - v1.X * len;
            n.Y = y2 - v1.Y * len;
            result[0] = new PointF(n.X + v1.Y * width, n.Y - v1.X * width);
            result[1] = new PointF(x2, y2);
            result[2] = new PointF(n.X - v1.Y * width, n.Y + v1.X * width);
            return result;
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            PIX_IN_ONE = 15;
            SIZE_BRUSH = 6;
            scale = 1;
            label1.Text = "";
            if (list.Count()>0)
            {
                list.RemoveAt(0);
            }
           
            
            this.index = rng.Next(5);
            while(index==oldIndex)
            {
                this.index = rng.Next(5);
            }
            Init();
            RefreshBitmap();
            this.Refresh();
            oldIndex = index;
            

        }



        private void Form1_Resize(object sender, EventArgs e)
        {
            if(isRain==true)
            {
                rain.Clear();
                for (int i = 0; i < 200; i++) // creates 100 drops at random position and with random speed
                    rain.Add(CreateRandomDrop());
            }

            int resW = panel1.Width/2 - formWidth/2;
            int resH = panel1.Height/2 - formHeight/2;
            PaintedObject po = new PaintedObject();
            
            Graphics graphics = panel1.CreateGraphics();
            graphics.Clear(BackColor);
            
            if (list.Count() > 0)
            {
                po = (PaintedObject)list[0].Clone();
                
                po.Path.Transform(new Matrix(1, 0, 0 ,1, resW,resH));
                list.RemoveAt(0);
                list.Add(po); 
            }
            
            this.Refresh();
            formWidth = panel1.Width;
            formHeight = panel1.Height;
        }
       

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            

            PaintedObject po = new PaintedObject();
            if ((Control.ModifierKeys) != 0)
            {
                e.Graphics.TranslateTransform(cx, cy);
                k = 8;
            }
            else
                k = 1;

            panel1_Paint(e.Graphics);
            if ((Control.ModifierKeys) != 0)
            {
                if (list.Count() > 0)
                {
                    po = (PaintedObject)list[0].Clone();
                    po.Path.Transform(new Matrix(1, 0, 0, 1, -panel1.Width / 2, -panel1.Height / 2));
                    e.Graphics.DrawPath(po.Pen, po.Path);
                }
            }
            else
            {


                if (bmp == null) return;
                RefreshBitmap();
                e.Graphics.DrawImage(bmp, -panel1.Width / 2, -panel1.Height / 2);
            }

            
        }
        void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys ) != 0)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:

                        int dx = e.X - lx;
                        int dy = e.Y - ly;

                        cx += dx;
                        cy += dy;

                        lx = e.X;
                        ly = e.Y;

                        break;

                    default:
                        break;


                }

                this.Refresh();

            }
            else
            {
                cx = 0;
                cy = 0;
                switch (e.Button)
                {
                    case MouseButtons.Left:

                        if (currObj != null)
                        {
                            deltaX = e.Location.X - oldPoint.X;
                            deltaY = e.Location.Y - oldPoint.Y;


                            currObj.Path.Transform(new Matrix(1, 0, 0, 1, deltaX, deltaY));


                            oldPoint = e.Location;
                            break;
                        }
                        else
                        {
                            deltaX = e.Location.X - oldPoint.X;
                            deltaY = e.Location.Y - oldPoint.Y;
                            break;
                        }
                    default:
                        break;
                }

                this.Refresh();
            }
            
        }
        void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys) != 0)
            {
                lx = e.X;
                ly = e.Y;


            }
            else
            {
                
                //Запоминаем положение курсора
                oldPoint = e.Location;
                //Ищем объект, в который попала точка. Если таких несколько, то найден будет первый по списку
                foreach (PaintedObject po in list)
                {
                    if (po.Path.GetBounds().Contains(e.Location))
                    {
                        if (po != null)
                        {
                            currObj = po;//Запоминаем найденный объект
                            return;
                        }
                    }
                }
            }
        }
        void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (currObj != null)
            {
                currObj = null;//Убираем ссылку на объект
            }

        }

        void RefreshBitmap()
        {
            if (bmp != null) bmp.Dispose();
            bmp = new Bitmap(this.ClientSize.Width,this.ClientSize.Height);
            //Прорисовка всех объектов из списка
            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (PaintedObject po in list)
                {
                    g.DrawPath(po.Pen, po.Path);
                }
            }
        }
        void Init()
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            PaintedObject po;

            formWidth = this.panel1.Width;
            formHeight = this.panel1.Height;
           
            //graphicsPath.Transform(new Matrix(1,0,0,1,formWidth,formHeight));
            po = new PaintedObject(new Pen(Color.Black,2), graphicsPath);
            
            if(index==0|| index==4)
            {
                po.Path.AddLines(drawer.Draw(panel1, functions[index], 2*formWidth,2*formHeight));
            }
            else
            {
                
                po.Path.AddLines(drawer.Draw(panel1, functions[index],formWidth,formHeight));
            }
            
            po.Path.Transform(new Matrix(1,0,0,-1, formWidth / 2, formHeight / 2));
            list.Add(po);
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            backgroundStyle.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (list.Count() > 0)
            {
                ColorDialog colorDialog = new ColorDialog();
                PaintedObject po = new PaintedObject();



                if (colorDialog.ShowDialog() == DialogResult.OK)
                {

                    po = (PaintedObject)list[0].Clone();
                    list.RemoveAt(0);
                    po.Pen.Color = colorDialog.Color;


                    list.Add(po);


                    this.Refresh();
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("График не создан",
                                                      "Ошибка",
                                                      MessageBoxButtons.OK,
                                                      MessageBoxIcon.Error,
                                                      MessageBoxDefaultButton.Button1,
                                                      MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        public void setСolor(Color color)
        {
            panel1.BackgroundImage = null;
            panel1.BackColor = color;
            gradient = false;
            this.hatch = false;
            panel1.Refresh();
        }

        public void setGradient(bool gradient,Color color1,Color color2)
        {
            this.gradient = gradient;
            
            this.color1 = color1;
            this.color2 = color2;

            this.hatch = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            save();
            DialogResult result = MessageBox.Show("Изображение сохранино",
                                                      "Сообщение",
                                                      MessageBoxButtons.OK,
                                                      MessageBoxIcon.Information,
                                                      MessageBoxDefaultButton.Button2,
                                                      MessageBoxOptions.DefaultDesktopOnly);
        }

        public void setHatch(bool hatch)
        {
            this.hatch = hatch;
        }


        public void setImage()
        {
            panel1.BackgroundImage = Properties.Resources.fon;
            gradient = false;
            this.hatch = false;
            panel1.Refresh();
        }
        private void save()
        {
            int width = panel1.Size.Width;
            int height = panel1.Size.Height;

            using (Bitmap bmp = new Bitmap(width, height))
            {
                panel1.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save("test.bmp", ImageFormat.Bmp);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            isRain = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            panel1.Refresh();
        }


        private Drop CreateRandomDrop()
        {
            return new Drop
            {
                Position = new PointF(rnd.Next(panel1.Width), rnd.Next(panel1.Height)),
                YSpeed = (float)rnd.NextDouble() * 6 + 2
            };
        }


        private void UpdateRain()
        {
            
            foreach (var drop in rain)
            {
                
                float currentY = functions[index].calc(drop.Position.X / 15 - formWidth / 30);
                if (formHeight / 2 - currentY <= drop.Position.Y + 2 && currentY <= formHeight / 2 )
                {
                    float derivative = functions[index].derivative(drop.Position.X  / 15 - formWidth / 30);
                    if (derivative > 0)
                    {
                        if (Math.Sign(derivative) == Math.Sign(functions[index].derivative((drop.Position.X - 1) / 15 - formWidth / 30 )))
                        {
                            drop.Position.X -= 1;
                            drop.Position.Y = formHeight / 2 - functions[index].calc(drop.Position.X / 15 - formWidth / 30);
                        }
                        else
                        {
                            drop.Position.Y = 0; ;
                            drop.Position.X = rnd.Next(panel1.Width);
                        }


                    }
                    if (derivative < 0)
                    {
                        if (Math.Sign(derivative) == Math.Sign(functions[index].derivative((drop.Position.X + 1) / 15 - formWidth / 30)))
                        {
                            drop.Position.X += 1;
                            drop.Position.Y = formHeight / 2 - functions[index].calc(drop.Position.X / 15 - formWidth / 30);
                        }
                        else
                        {
                            drop.Position.Y = 0; ;
                            drop.Position.X = rnd.Next(panel1.Width);
                        }


                    }
                    if (derivative == 0)
                    {
                        drop.Position.Y = 0; ;
                        drop.Position.X = rnd.Next(panel1.Width);
                    }

                }
                else
                {
                    drop.Fall();
                }


                if (drop.Position.Y > panel1.Height)
                {
                    drop.Position.Y = 0;
                    drop.Position.X = rnd.Next(panel1.Width);
                }


            }
        }


        private void RenderRain(Graphics graphics)
        {
            //graphics.Clear(Color.White);
            PointF poin = new PointF();
            float y = 0;
            Pen pen = new Pen(Color.Blue);
            int i = 0;
            foreach (var drop in rain)
            {
                i++;
                graphics.DrawLine(pen, drop.Position, new PointF(drop.Position.X, drop.Position.Y + 2));
                if(i>70)
                {
                    pen = new Pen(Color.Red);

                }
                
            }
        }

        
    }
}
class Drop
{
    public PointF Position;
    public float YSpeed;

    public void Fall() => Position.Y += YSpeed;
}
