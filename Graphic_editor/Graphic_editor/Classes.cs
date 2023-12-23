using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Graphic_editor
{
    public interface Figure
    {
        void Draw(Graphics g);
    }
    public abstract class Figures:Figure
    {
        public abstract void Draw(Graphics g);
    }
    public class ArrayPoints
    {
        int index = 0;
        Point[] points;
        public ArrayPoints(int size)
        {
            if (size <= 0)
            {
                size = 2;
            }
            points = new Point[size];
        }
        public void SetPoint(int x, int y)
        {
            if (index >= points.Length)
            {
                index = 0;
            }
            points[index] = new Point(x, y);
            index++;
        }
        public void ResetPoints()
        {
            index = 0;
        }
        public int GetCount()
        {
            return index;
        }
        public Point[] GetPoints()
        {
            return points;
        }
    }
    public class Line: Figures
    {
        public Point startPoint, endPoint;
        public Pen pen;
        public Line(Point startP, Point endP, Pen pen)
        {
            this.startPoint = startP;
            this.endPoint = endP;
            this.pen = pen;
        }
        public Line()
        {
            startPoint.X = 0;
            startPoint.Y = 0;
            endPoint.X = 0;
            endPoint.Y = 0;
        }
        public override void Draw(Graphics g)
        {
            g.DrawLine(pen, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
        }
    }
    public class CircleStatic : Figures
    {
        public Point pos;
        public float size;
        public Pen pen;
        public CircleStatic(Pen pen)
        {
            this.pen = pen;
        }
        public CircleStatic()
        {
            pos.X = 0;
            pos.Y = 0;
        }
        public override void Draw(Graphics g)
        {
            g.DrawEllipse(pen,new RectangleF(pos.X-size/2, pos.Y-size/2,size,size));
        }
    }
    public class SquareStatic : Figures
    {
        public Point pos;
        public int size;
        public Pen pen;
        public int rotate=0;
        public SquareStatic(Pen pen)
        {
            this.pen = pen;
        }
        public SquareStatic()
        {
            pos.X = 0;
            pos.Y = 0;
        }
        public override void Draw(Graphics g)
        {
            Rectangle squareRect = new Rectangle(pos.X - size / 2, pos.Y - size / 2, size, size);
            Matrix matrix = new Matrix();
            matrix.RotateAt(rotate, new PointF(squareRect.Left + squareRect.Width / 2, squareRect.Top + squareRect.Height / 2));
            g.Transform = matrix;
            g.DrawRectangle(pen, squareRect);
        }
    }
    public class RectStatic : Figures
    {
        public Point pos;
        public Size size;
        public Pen pen;
        public int rotate = 0;
        public RectStatic(Pen pen)
        {
            this.pen = pen;
        }
        public RectStatic()
        {
            pos.X = 0;
            pos.Y = 0;
        }
        public override void Draw(Graphics g)
        {
            Rectangle Rect = new Rectangle(pos.X - size.Width / 2, pos.Y - size.Height / 2, size.Width, size.Height);
            Matrix matrix = new Matrix();
            matrix.RotateAt(rotate, new PointF(Rect.Left + Rect.Width / 2, Rect.Top + Rect.Height / 2));
            g.Transform = matrix;
            g.DrawRectangle(pen, Rect);
        }
    }
    public class Ex1 : Exception
    {
        public Ex1() { }
        public Ex1(string message) : base(message) { }
        public Ex1(string message, Exception e) : base(message, e) { }
    }

}
