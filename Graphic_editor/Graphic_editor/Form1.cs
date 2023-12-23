using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Graphic_editor
{
    public partial class Form1 : Form
    {
        private List<Line> MyLines = new List<Line>();
        private List<CircleStatic> MyStaticCircles = new List<CircleStatic>();
        private List<SquareStatic> MyStaticSquares = new List<SquareStatic>();
        private List<RectStatic> MyStaticRects = new List<RectStatic>();
        private int toolIndex=0;
        private string DrawCase = "Line";
        private Point startPoint, currentPoint, dragStart, dragEnd, startDownLocation, posPoint;
        private Button currentButton = null;
        private int initialBorderSize = 1;
        private bool mooved = false;
        private bool isPressed = false;
        private bool isStartPressed = false;
        private bool isEndPressed = false;
        private bool cleared = false;
        private ArrayPoints arrayPoints = new ArrayPoints(2);
        Bitmap map = new Bitmap(100, 100);
        Graphics graphics;
        Pen pen = new Pen(Color.Black, 3f);
        Pen eraser = new Pen(Color.White, 3f);
        private void SetSize()
        {
            map = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            graphics = Graphics.FromImage(map);
            graphics.Clear(pictureBox1.BackColor);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            eraser.StartCap = LineCap.Round;
            eraser.EndCap =LineCap.Round;
        }
        public Form1()
        {
            InitializeComponent();
            Titul t = new Titul();
            t.Show();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ControlExtension.Draggable(panel7, true);
            SetSize();
            initialBorderSize = button3.FlatAppearance.BorderSize;
            pen.Width = thickBar.Value;
            button11.BackColor = pen.Color;
            label2.Text = thickBar.Value.ToString();
            toolTip1.SetToolTip(pencilBtn, "Кисть");
            toolTip1.SetToolTip(eraserBtn, "Ластик");
            toolTip1.SetToolTip(fillBtn, "Заливка");
            toolTip1.SetToolTip(pipetteBtn, "Пипетка");
            toolTip1.SetToolTip(lineBtn, "Прямая линия");
            toolTip1.SetToolTip(circleBtn, "Окружность");
            toolTip1.SetToolTip(squareBtn, "Квадрат");
            toolTip1.SetToolTip(rectangleBtn, "Прямоугольник");
            toolTip1.SetToolTip(button10, "Градиент");
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isPressed = true;
            mooved = false;
            if (toolIndex == 2)
            {
                Fill(e.X, e.Y, pen.Color);
            }
            if (toolIndex == 3)
            {
                pen.Color = map.GetPixel(e.X, e.Y);
                button11.BackColor = pen.Color;
            }
            if (toolIndex==4 && MyLines.Count >= 1)
            {
                int k = MyLines.Count - 1;
                if (IsBetween(MyLines[k].startPoint.X, MyLines[k].startPoint.Y, MyLines[k].endPoint.X, MyLines[k].endPoint.Y, e.X, e.Y))
                {
                    DrawCase = "MoveLine";
                }
                else
                {
                    DrawCase = "Line";
                }
                if (IsOnEdge(MyLines[k].startPoint.X, MyLines[k].startPoint.Y, MyLines[k].endPoint.X, MyLines[k].endPoint.Y, e.X, e.Y, pen.Width))
                {
                    DrawCase = "ResizeLine";
                    isStartPressed = IsOnStart(MyLines[k].startPoint.X, MyLines[k].startPoint.Y, e.X, e.Y, pen.Width);
                    isEndPressed = IsOnEnd(MyLines[k].endPoint.X, MyLines[k].endPoint.Y, e.X, e.Y, pen.Width);
                }
            }
            if (toolIndex==5 && MyStaticCircles.Count >= 1)
            {
                int k = MyStaticCircles.Count - 1;
                if (Math.Pow(e.X - MyStaticCircles[k].pos.X, 2) + Math.Pow(e.Y - MyStaticCircles[k].pos.Y, 2) <= Math.Pow((MyStaticCircles[k].size / 2) + MyStaticCircles[k].pen.Width / 2, 2) && Math.Pow(e.X - MyStaticCircles[k].pos.X, 2) + Math.Pow(e.Y - MyStaticCircles[k].pos.Y, 2) >= Math.Pow((MyStaticCircles[k].size / 2) - MyStaticCircles[k].pen.Width / 2, 2))
                {
                    DrawCase = "MoveCircle";
                } else
                {
                    DrawCase = "Circle";
                }
            }
            if (toolIndex == 6 && MyStaticSquares.Count >= 1)
            {
                int k = MyStaticSquares.Count - 1;
                if (IsCursorOnSquare(MyStaticSquares[k].pos, MyStaticSquares[k].size, e.Location, MyStaticSquares[k].pen) )
                {
                    DrawCase = "MoveSquare";
                }
                else
                {
                    DrawCase = "Square";
                }
            }
            if (toolIndex == 7 && MyStaticRects.Count >= 1)
            {
                int k = MyStaticRects.Count - 1;
                if (IsCursorOnRect(MyStaticRects[k].pos, MyStaticRects[k].size, e.Location, MyStaticRects[k].pen))
                {
                    DrawCase = "MoveRect";
                }
                else
                {
                    DrawCase = "Rect";
                }
            }
            startPoint = e.Location;
            currentPoint = e.Location;
            startDownLocation = e.Location;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mooved = true;
            if (!isPressed) {
                if (MyLines.Count>=1 && toolIndex==4)
                {
                    int k = MyLines.Count - 1;
                    if (IsBetween(MyLines[k].startPoint.X, MyLines[k].startPoint.Y, MyLines[k].endPoint.X, MyLines[k].endPoint.Y, e.X, e.Y))
                    {
                        pictureBox1.Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        pictureBox1.Cursor = Cursors.Default;
                    }
                    if (IsOnEdge(MyLines[k].startPoint.X, MyLines[k].startPoint.Y, MyLines[k].endPoint.X, MyLines[k].endPoint.Y, e.X, e.Y, pen.Width))
                    {
                        pictureBox1.Cursor = Cursors.SizeNS;
                    }
                }
                if (MyStaticCircles.Count>=1 && toolIndex == 5)
                {
                    int k = MyStaticCircles.Count - 1;
                    if (Math.Pow(e.X - MyStaticCircles[k].pos.X, 2) + Math.Pow(e.Y - MyStaticCircles[k].pos.Y, 2) <=Math.Pow((MyStaticCircles[k].size / 2) + MyStaticCircles[k].pen.Width/2, 2) && Math.Pow(e.X - MyStaticCircles[k].pos.X, 2) + Math.Pow(e.Y - MyStaticCircles[k].pos.Y, 2) >= Math.Pow((MyStaticCircles[k].size / 2) - MyStaticCircles[k].pen.Width / 2, 2))
                    {
                        pictureBox1.Cursor = Cursors.SizeAll;
                    }else
                    {
                        pictureBox1.Cursor = Cursors.Default;
                    }
                }
                if (MyStaticSquares.Count>=1 && toolIndex== 6)
                {
                    int k = MyStaticSquares.Count - 1;
                    if (IsCursorOnSquare(MyStaticSquares[k].pos, MyStaticSquares[k].size, e.Location, MyStaticSquares[k].pen))
                    {
                        pictureBox1.Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        pictureBox1.Cursor = Cursors.Default;
                    }
                }
                if (MyStaticRects.Count >= 1 && toolIndex == 7)
                {
                    int k = MyStaticRects.Count - 1;
                    if (IsCursorOnRect(MyStaticRects[k].pos, MyStaticRects[k].size, e.Location, MyStaticRects[k].pen))
                    {
                        pictureBox1.Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        pictureBox1.Cursor = Cursors.Default;
                    }
                }
                return; 
            }
            currentPoint = e.Location;
            if (toolIndex == 0)
            {
                arrayPoints.SetPoint(e.X, e.Y);
                if (arrayPoints.GetCount() >= 2)
                {
                    graphics.DrawLines(pen, arrayPoints.GetPoints());
                    pictureBox1.Image = map;
                    arrayPoints.SetPoint(e.X, e.Y);
                }
            }
            if (toolIndex == 1)
            {
                arrayPoints.SetPoint(e.X, e.Y);
                if (arrayPoints.GetCount() >= 2)
                {
                    graphics.DrawLines(eraser, arrayPoints.GetPoints());
                    pictureBox1.Image = map;
                    arrayPoints.SetPoint(e.X, e.Y);
                }
            }
            switch (DrawCase)
            {
                case "Line":
                    {
                        break;
                    }
                case "MoveLine":
                    {
                        int i;
                        i = MyLines.Count - 1;
                        if (i >= 0)
                        {
                            dragStart.X = e.X + MyLines[i].startPoint.X - startDownLocation.X;
                            dragStart.Y = e.Y + MyLines[i].startPoint.Y - startDownLocation.Y;
                            dragEnd.X = e.X + MyLines[i].endPoint.X - startDownLocation.X;
                            dragEnd.Y = e.Y + MyLines[i].endPoint.Y - startDownLocation.Y;
                        }
                        break;
                    }
                case "ResizeLine":
                    {
                        int i;
                        i = MyLines.Count - 1;
                        if (i >= 0) 
                        {
                            if (isStartPressed)
                            {
                                MyLines[i].startPoint.X = e.X;
                                MyLines[i].startPoint.Y = e.Y;
                            }
                            if (isEndPressed)
                            {
                                MyLines[i].endPoint.X = e.X;
                                MyLines[i].endPoint.Y = e.Y;
                            }
                        }
                        break;
                    }
                case "MoveCircle":
                    {
                        int i;
                        i = MyStaticCircles.Count - 1;
                        if (i >= 0)
                        {
                            dragStart.X = e.X + MyStaticCircles[i].pos.X - startDownLocation.X;
                            dragStart.Y = e.Y + MyStaticCircles[i].pos.Y - startDownLocation.Y;
                        }
                        break;
                    }
                case "MoveSquare":
                    {
                        int i;
                        i = MyStaticSquares.Count - 1;
                        if (i >= 0)
                        {
                            dragStart.X = e.X + MyStaticSquares[i].pos.X - startDownLocation.X;
                            dragStart.Y = e.Y + MyStaticSquares[i].pos.Y - startDownLocation.Y;
                        }
                        break;
                    }
                case "MoveRect":
                    {
                        int i;
                        i = MyStaticRects.Count - 1;
                        if (i >= 0)
                        {
                            dragStart.X = e.X + MyStaticRects[i].pos.X - startDownLocation.X;
                            dragStart.Y = e.Y + MyStaticRects[i].pos.Y - startDownLocation.Y;
                        }
                        break;
                    }
            }
            pictureBox1.Invalidate();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (cleared)
            {
                cleared = false;
                ClearLists();
                e.Graphics.Clear(pictureBox1.BackColor);
            }
            Graphics g = e.Graphics;
            foreach (Line line in MyLines)
            {
                line.Draw(g);
            }
            foreach (CircleStatic circle in MyStaticCircles)
            {
                circle.Draw(g);
            }
            foreach (SquareStatic square in MyStaticSquares)
            {
                square.Draw(g);
            }
            foreach (RectStatic rect in MyStaticRects)
            {
                rect.Draw(g);
            }
            if (mooved)
            {
                if (toolIndex == 4)
                {
                    switch (DrawCase)
                    {
                        case "Line":
                            {
                                e.Graphics.DrawLine(pen, startPoint.X, startPoint.Y, currentPoint.X, currentPoint.Y);
                                break;
                            }
                        case "MoveLine":
                            {
                                e.Graphics.DrawLine(pen, dragStart.X, dragStart.Y, dragEnd.X, dragEnd.Y);
                                break;
                            }
                    }
                }
                if (toolIndex == 5)
                {
                    switch (DrawCase)
                    {
                        case "MoveCircle":
                            {
                                if (MyStaticCircles.Count >= 1)
                                {
                                    float size = MyStaticCircles[MyStaticCircles.Count - 1].size;
                                    e.Graphics.DrawEllipse(pen, dragStart.X - size / 2, dragStart.Y - size / 2, size, size);
                                }
                                break;
                            }
                    }
                }
                if (toolIndex == 6)
                {
                    switch (DrawCase)
                    {
                        case "MoveSquare":
                            {
                                if (MyStaticSquares.Count >= 1)
                                {
                                    float size = MyStaticSquares[MyStaticSquares.Count - 1].size;
                                    e.Graphics.DrawRectangle(pen, dragStart.X - size / 2, dragStart.Y - size / 2, size, size);
                                }
                                break;
                            }
                    }
                }
                if (toolIndex == 7)
                {
                    switch (DrawCase)
                    {
                        case "MoveRect":
                            {
                                if (MyStaticRects.Count >= 1)
                                {
                                    Size size = MyStaticRects[MyStaticRects.Count - 1].size;
                                    e.Graphics.DrawRectangle(pen, dragStart.X - size.Width / 2, dragStart.Y - size.Height / 2, size.Width, size.Height);
                                }
                                break;
                            }
                    }
                }
            }
        }
        
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isPressed = false;
            if (!mooved)
            {
                if (toolIndex == 0)
                {
                    SolidBrush br = new SolidBrush(pen.Color);
                    graphics.FillEllipse(br, e.X - pen.Width / 2, e.Y - pen.Width / 2, pen.Width, pen.Width);
                    pictureBox1.Refresh();

                }
                if (toolIndex == 1)
                {
                    SolidBrush br = new SolidBrush(eraser.Color);
                    graphics.FillEllipse(br, e.X - eraser.Width / 2, e.Y - eraser.Width / 2, eraser.Width, eraser.Width);
                    pictureBox1.Refresh();

                }
                
            }
            else
            {
                if (toolIndex == 4)
                {
                    switch (DrawCase)
                    {
                        case "Line":
                            {
                                Line DrawLine = new Line();
                                DrawLine.startPoint = startPoint;
                                DrawLine.endPoint = currentPoint;
                                DrawLine.pen = (Pen)pen.Clone();
                                MyLines.Add(DrawLine);
                                break;
                            }
                        case "MoveLine":
                            {
                                Line DrawLine = new Line();
                                DrawLine.startPoint = dragStart;
                                DrawLine.endPoint = dragEnd;
                                if (MyLines.Count >= 1)
                                {
                                    DrawLine.pen = (Pen)MyLines[MyLines.Count - 1].pen.Clone();
                                    MyLines.RemoveAt(MyLines.Count - 1);
                                    MyLines.Add(DrawLine);
                                }
                                break;
                            }
                    }
                }
                if (toolIndex == 5)
                {
                    switch (DrawCase)
                    {
                        case "MoveCircle":
                            {
                                posPoint = e.Location;
                                CircleStatic DrawCircle = new CircleStatic();
                                DrawCircle.pos = dragStart;
                                DrawCircle.size = MyStaticCircles[MyStaticCircles.Count - 1].size;
                                if (MyStaticCircles.Count >= 1)
                                {
                                    DrawCircle.pen = (Pen)MyStaticCircles[MyStaticCircles.Count - 1].pen.Clone();
                                    MyStaticCircles.RemoveAt(MyStaticCircles.Count - 1); 
                                    MyStaticCircles.Add(DrawCircle);
                                    DrawCase = "Line";
                                }
                                break;
                            }
                    }
                }
                if (toolIndex == 6)
                {
                    switch (DrawCase)
                    {
                        case "MoveSquare":
                            {
                                posPoint = e.Location;
                                SquareStatic DrawSquare = new SquareStatic();
                                DrawSquare.pos = dragStart;
                                DrawSquare.size = MyStaticSquares[MyStaticSquares.Count - 1].size;
                                if (MyStaticSquares.Count >= 1)
                                {
                                    DrawSquare.pen = (Pen)MyStaticSquares[MyStaticSquares.Count - 1].pen.Clone();
                                    MyStaticSquares.RemoveAt(MyStaticSquares.Count - 1);
                                    MyStaticSquares.Add(DrawSquare);
                                    DrawCase = "Line";
                                }
                                break;
                            }
                    }
                }
                if (toolIndex == 7)
                {
                    switch (DrawCase)
                    {
                        case "MoveRect":
                            {
                                posPoint = e.Location;
                                RectStatic DrawRect = new RectStatic();
                                DrawRect.pos = dragStart;
                                DrawRect.size = MyStaticRects[MyStaticRects.Count - 1].size;
                                if (MyStaticRects.Count >= 1)
                                {
                                    DrawRect.pen = (Pen)MyStaticRects[MyStaticRects.Count - 1].pen.Clone();
                                    MyStaticRects.RemoveAt(MyStaticRects.Count - 1);
                                    MyStaticRects.Add(DrawRect);
                                    DrawCase = "Line";
                                }
                                break;
                            }
                    }
                }
            }
            arrayPoints.ResetPoints();
            pictureBox1.Refresh();
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (toolIndex == 5 && DrawCase == "Circle" && DrawCase!="MoveCircle")
            {
                posPoint = e.Location;
                CircleStatic DrawCircle = new CircleStatic();
                DrawCircle.pos = posPoint;
                if (!String.IsNullOrEmpty(textBox1.Text) && float.Parse(textBox1.Text) >= 10)
                {
                    DrawCircle.size = float.Parse(textBox1.Text);
                }
                else
                {
                    DrawCircle.size = 10;
                }
                DrawCircle.pen = (Pen)pen.Clone();
                MyStaticCircles.Add(DrawCircle);
                textBox1.Text = DrawCircle.size.ToString();
                pictureBox1.Refresh();
                DrawCase = "Circle";
            }
            if (toolIndex == 6 && DrawCase == "Square" && DrawCase != "MoveSquare")
            {
                posPoint = e.Location;
                SquareStatic DrawSquare = new SquareStatic();
                DrawSquare.pos = posPoint;
                if (!String.IsNullOrEmpty(textBox1.Text) && int.Parse(textBox1.Text) >= 10)
                {
                    DrawSquare.size = int.Parse(textBox1.Text);
                }
                else
                {
                    DrawSquare.size = 10;
                }
                if (!String.IsNullOrEmpty(textBox2.Text) && int.Parse(textBox2.Text) >= 0)
                {
                    DrawSquare.rotate = int.Parse(textBox2.Text);
                }
                DrawSquare.pen = (Pen)pen.Clone();
                MyStaticSquares.Add(DrawSquare);
                textBox1.Text = DrawSquare.size.ToString();
                pictureBox1.Refresh();
                DrawCase = "Square";
            }
            if (toolIndex == 7 && DrawCase == "Rect" && DrawCase != "MoveRect")
            {
                posPoint = e.Location;
                RectStatic DrawRect = new RectStatic();
                DrawRect.pos = posPoint;
                if (!String.IsNullOrEmpty(textBox1.Text) && int.Parse(textBox1.Text) >= 10 && !String.IsNullOrEmpty(textBox3.Text) && int.Parse(textBox3.Text) >= 10)
                {
                    DrawRect.size.Width = int.Parse(textBox1.Text);
                    DrawRect.size.Height = int.Parse(textBox3.Text);
                }
                else
                {
                    DrawRect.size.Width = 10;
                    DrawRect.size.Height = 10;
                }
                DrawRect.pen = (Pen)pen.Clone();
                MyStaticRects.Add(DrawRect);
                textBox1.Text = DrawRect.size.Width.ToString();
                textBox3.Text = DrawRect.size.Height.ToString();
                pictureBox1.Refresh();
                DrawCase = "Rect";
            }
        }
        public Bitmap SavePictureBoxContent()
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                pictureBox1.DrawToBitmap(bitmap, pictureBox1.ClientRectangle);
                pictureBox1.Image = bitmap;
            }
            return bitmap;
        }
        private void button3_Click(object sender, EventArgs e)  //выбор цвета
        {
            pen.Color = ((Button)sender).BackColor;
            button11.BackColor = pen.Color;
        }
        private void button10_Click(object sender, EventArgs e) //установка пользовательского цвета
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pen.Color = colorDialog1.Color;
                ((Button)sender).BackColor = colorDialog1.Color;
                button11.BackColor = pen.Color;
            }
        }
        private void thickBar_ValueChanged(object sender, EventArgs e) //изменение толщины кисти
        {
            pen.Width = thickBar.Value;
            eraser.Width = thickBar.Value;
            label2.Text = thickBar.Value.ToString();
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Title = "Сохранить картинку как ...";
                saveFileDialog1.Filter = "PNG File(*.png)|*.png|" + "JPEG File(*.jpg)|*.jpg";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog1.FileName;
                    if (string.IsNullOrEmpty(fileName))
                    {
                        throw (new Ex1("Имя файла не может быть пустым!"));
                    }
                    string strFilExtn = fileName.Remove(0, fileName.Length - 3);
                    switch (strFilExtn)
                    {
                        case "jpg": SavePictureBoxContent().Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                        case "png": SavePictureBoxContent().Save(fileName, System.Drawing.Imaging.ImageFormat.Png); break;
                        default: break;
                    }
                }
                panel7.Visible = false;
            } catch (Ex1 ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
        private void clearBtn_Click(object sender, EventArgs e)
        {
            cleared = true;
            mooved = false;
            ClearLists();
            graphics.Clear(pictureBox1.BackColor);
            panel7.Visible = false;
            switch (toolIndex)
            {
                case 4: DrawCase = "Line"; break;
                case 5: DrawCase = "Circle"; break;
                case 6: DrawCase = "Square"; break;
                case 7: DrawCase = "Rect"; break;
            }
            pictureBox1.Refresh();
        }
        public void Clear()
        {
            mooved = false;
            map = SavePictureBoxContent();
            graphics = Graphics.FromImage(map);
            pictureBox1.Image = map;
            ClearLists();
        }
        public void ClearLists()
        {
            MyLines.Clear();
            MyStaticCircles.Clear();
            MyStaticRects.Clear();
            MyStaticSquares.Clear();
        }
        private void button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (currentButton != null)
            {
                currentButton.FlatAppearance.BorderSize = initialBorderSize;
            }
            button.FlatAppearance.BorderSize = 3;
            currentButton = button;
        }
        private void eraserBtn_Click(object sender, EventArgs e)
        {
            SetToolIndex(1, false, true, false, sender, e);
        }

        private void pencilBtn_Click(object sender, EventArgs e)
        {
            SetToolIndex(0, true, false, false, sender, e);
        }

        private void fillBtn_Click(object sender, EventArgs e)
        {
            SetToolIndex(2, true, false, false, sender, e);
        }

        private void pipetteBtn_Click(object sender, EventArgs e)
        {
            SetToolIndex(3, true, false, false, sender, e);
        }

        private void lineBtn_Click(object sender, EventArgs e)
        {
            Clear();
            mooved = false;
            DrawCase = "Line";
            SetToolIndex(4, true, false, false, sender, e);
        }

        public void SetToolIndex(int index, bool button11Visible, bool label10Visible, bool panel7Visible, object sender, EventArgs e)
        {
            Clear();
            toolIndex = index;
            button_Click(sender, e);
            button11.Visible = button11Visible;
            label10.Visible = label10Visible;
            panel7.Visible = panel7Visible;
        }
        private void circleBtn_Click(object sender, EventArgs e)
        {
            SetToolIndex(5, "Диаметр(px):", false, false, false, sender, e);
            textBox1.Text = GetLastCircleSize();
            DrawCase = "Circle";
        }

        private void squareBtn_Click(object sender, EventArgs e)
        {
            SetToolIndex(6, "Ширина(px):", true, true, false, sender, e, "Поворот:", "0");
            DrawCase = "Square";
        }

        private void rectangleBtn_Click(object sender, EventArgs e)
        {
            SetToolIndex(7, "Ширина(px):", true, true, true, sender, e, "Поворот:", "0", "Высота(px):");
            DrawCase = "Rect";
        }

        public void SetToolIndex(int index, string label11Text, bool label12Visible, bool textBox2Visible, bool label13Visible, object sender, EventArgs e, string label12Text = "", string textBox2Text = "", string label13Text = "")
        {
            Clear();
            pictureBox1.Refresh();
            toolIndex = index;
            button_Click(sender, e);
            button11.Visible = true;
            label10.Visible = false;
            panel7.Visible = true;
            label11.Text = label11Text;
            textBox1.Text = "";
            label12.Visible = label12Visible;
            label12.Text = label12Text;
            textBox2.Visible = textBox2Visible;
            textBox2.Text = textBox2Text;
            label13.Visible = label13Visible;
            label13.Text = label13Text;
            textBox3.Visible = label13Visible;
            textBox3.Text = "";
        }
        public void Fill(int x, int y, Color new_color)
        {
            Color old_color = map.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x,y));
            map.SetPixel(x,y,new_color);
            if (old_color == new_color) return;
            while (pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if (pt.X>0 && pt.Y>0 && pt.X<map.Width-1 && pt.Y < map.Height - 1)
                {
                    validate(pixel, pt.X - 1, pt.Y, old_color, new_color);
                    validate(pixel, pt.X , pt.Y - 1, old_color, new_color);
                    validate(pixel, pt.X + 1, pt.Y, old_color, new_color);
                    validate(pixel, pt.X, pt.Y + 1, old_color, new_color);
                }
            }
            pictureBox1.Invalidate();
            pictureBox1.Image = map;
        }
        public void validate(Stack<Point> st, int x, int y, Color old_color, Color new_color)
        {
            Color cx = map.GetPixel(x, y);
            if (cx == old_color)
            {
                st.Push(new Point(x, y));
                map.SetPixel(x, y, new_color);
            }
        }
        public string GetLastCircleSize()
        {
            if (MyStaticCircles.Count >= 1)
            {
                return MyStaticCircles[MyStaticCircles.Count - 1].size.ToString();
            }
            return "";
        }
        private void closeBtn_Click(object sender, EventArgs e)
        {
            panel7.Visible = false;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            if (toolIndex == 5)
            {
                if (!String.IsNullOrEmpty(textBox1.Text) && float.Parse(textBox1.Text) > 1)
                {
                    float size = float.Parse(textBox1.Text);
                    if (MyStaticCircles.Count >= 1)
                    {
                        MyStaticCircles[MyStaticCircles.Count - 1].size = size;
                    }
                    pictureBox1.Invalidate();
                }
            }
            if (toolIndex == 6)
            {
                if (!String.IsNullOrEmpty(textBox1.Text) && int.Parse(textBox1.Text) > 1 && !String.IsNullOrEmpty(textBox2.Text) && int.Parse(textBox2.Text) >= 0)
                {
                    int size = int.Parse(textBox1.Text);
                    if (MyStaticSquares.Count >= 1)
                    {
                        MyStaticSquares[MyStaticSquares.Count - 1].size = size;
                        MyStaticSquares[MyStaticSquares.Count - 1].rotate = int.Parse(textBox2.Text);
                    }
                    pictureBox1.Invalidate();
                }
            }
            if (toolIndex == 7)
            {
                if (!String.IsNullOrEmpty(textBox1.Text) && int.Parse(textBox1.Text) >= 10 && !String.IsNullOrEmpty(textBox3.Text) && int.Parse(textBox3.Text) >= 10)
                {
                    if (MyStaticRects.Count >= 1)
                    {
                        if (!String.IsNullOrEmpty(textBox2.Text) && int.Parse(textBox2.Text) >= 0)
                        {
                            MyStaticRects[MyStaticRects.Count - 1].rotate = int.Parse(textBox2.Text);
                        }
                        MyStaticRects[MyStaticRects.Count - 1].size.Width = int.Parse(textBox1.Text);
                        MyStaticRects[MyStaticRects.Count - 1].size.Height = int.Parse(textBox3.Text);
                    }
                }
                else
                {
                    if (MyStaticRects.Count >= 1)
                    {
                        MyStaticRects[MyStaticRects.Count - 1].size.Width = 10;
                        MyStaticRects[MyStaticRects.Count - 1].size.Height = 10;
                        if (!String.IsNullOrEmpty(textBox2.Text) && int.Parse(textBox2.Text) >= 0)
                        {
                            MyStaticRects[MyStaticRects.Count - 1].rotate = int.Parse(textBox2.Text);
                        }
                    }
                }
                pictureBox1.Invalidate();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '\r')
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '\r')
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '\r')
            {
                e.Handled = true;
            }
        }
        public bool IsBetween(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            double distance1 = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            double distance2 = Math.Sqrt(Math.Pow(x3 - x1, 2) + Math.Pow(y3 - y1, 2));
            double distance3 = Math.Sqrt(Math.Pow(x3 - x2, 2) + Math.Pow(y3 - y2, 2));
            return ApproximatelyEqual(distance1, distance2 + distance3) ||
                   ApproximatelyEqual(distance1, distance2) ||
                   ApproximatelyEqual(distance1, distance3);
        }
        public bool ApproximatelyEqual(double x, double y, double epsilon = 0.1)
        {
            return Math.Abs(x - y) < epsilon;
        }
        public bool IsOnEdge(double x1, double y1, double x2, double y2, double x3, double y3, float i)
        {
            if (Math.Pow(x3-x1,2)+Math.Pow(y3-y1,2)>Math.Pow(i/2,2) && Math.Pow(x3 - x2, 2) + Math.Pow(y3 - y2, 2) > Math.Pow(i/2,2))
            {
                return false;
            }else
            {
                return true;
            }
        }
        public bool IsOnStart(double x1, double y1, double x3, double y3, float i)
        {
            if (Math.Pow(x3 - x1, 2) + Math.Pow(y3 - y1, 2) > Math.Pow(i / 2, 2))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool IsOnEnd(double x2, double y2, double x3, double y3, float i)
        {
            if (Math.Pow(x3 - x2, 2) + Math.Pow(y3 - y2, 2) > Math.Pow(i / 2, 2))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool IsCursorOnSquare(Point pos, int size, Point e, Pen pen)
        {
            Point cursorPos = e;
            int left = pos.X - size / 2;
            int right = pos.X + size / 2;
            int top = pos.Y - size / 2;
            int bottom = pos.Y + size / 2;
            int halfPenWidth = (int)(pen.Width / 2);
            bool isOnBorder = (cursorPos.X >= left - halfPenWidth && cursorPos.X <= left + halfPenWidth &&
                               cursorPos.Y >= top - halfPenWidth && cursorPos.Y <= bottom + halfPenWidth) ||
                              (cursorPos.X >= right - halfPenWidth && cursorPos.X <= right + halfPenWidth &&
                               cursorPos.Y >= top - halfPenWidth && cursorPos.Y <= bottom + halfPenWidth) ||
                              (cursorPos.Y >= top - halfPenWidth && cursorPos.Y <= top + halfPenWidth &&
                               cursorPos.X >= left - halfPenWidth && cursorPos.X <= right + halfPenWidth) ||
                              (cursorPos.Y >= bottom - halfPenWidth && cursorPos.Y <= bottom + halfPenWidth &&
                               cursorPos.X >= left - halfPenWidth && cursorPos.X <= right + halfPenWidth);
            return isOnBorder;
        }
        public bool IsCursorOnRect(Point pos, Size size,Point e, Pen pen)
        {
            Point cursorPos = e;
            int left = pos.X - size.Width / 2;
            int right = pos.X + size.Width / 2;
            int top = pos.Y - size.Height / 2;
            int bottom = pos.Y + size.Height / 2;
            int halfPenWidth = (int)(pen.Width / 2);
            bool isOnBorder = (cursorPos.X >= left - halfPenWidth && cursorPos.X <= left + halfPenWidth &&
                               cursorPos.Y >= top - halfPenWidth && cursorPos.Y <= bottom + halfPenWidth) ||
                              (cursorPos.X >= right - halfPenWidth && cursorPos.X <= right + halfPenWidth &&
                               cursorPos.Y >= top - halfPenWidth && cursorPos.Y <= bottom + halfPenWidth) ||
                              (cursorPos.Y >= top - halfPenWidth && cursorPos.Y <= top + halfPenWidth &&
                               cursorPos.X >= left - halfPenWidth && cursorPos.X <= right + halfPenWidth) ||
                              (cursorPos.Y >= bottom - halfPenWidth && cursorPos.Y <= bottom + halfPenWidth &&
                               cursorPos.X >= left - halfPenWidth && cursorPos.X <= right + halfPenWidth);
            return isOnBorder;
        }
    }
}
