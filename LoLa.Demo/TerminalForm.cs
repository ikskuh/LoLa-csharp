using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace LoLa.Demo
{
    public sealed class TerminalForm : Form
    {
        private Font terminalFont;
        private int advanceX = 7;
        private int advanceY = 9;
        private int adjustX = 0;
        private int adjustY = 0;
        private int padding = 3;

        private static readonly FontFamily fontFamily;

        static TerminalForm()
        {
            var col = new PrivateFontCollection();
            col.AddFontFile("Glass_TTY_VT220.ttf");
            fontFamily = col.Families.First();
        }

        private void InitializeFont()
        {
            terminalFont = new Font(fontFamily, 15.0f);
            advanceX = 10;
            advanceY = 16;
            adjustX = -3;
            adjustY = -1;

            //terminalFont = new Font(FontFamily.GenericMonospace, 8.0f, FontStyle.Regular);
        }

        private char[,] screen;
        private int curX, curY;
        private Timer timer;
        private bool cursorVisible;

        public TerminalForm(int w, int h)
        {
            this.InitializeFont();

            this.screen = new char[w, h];

            this.DoubleBuffered = true;
            this.ClientSize = new Size(2 * padding + advanceX * w, 2 * padding + advanceY * h);
            this.BackColor = Color.Black;
            this.ForeColor = Color.Lime;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;

            this.Clear();

            this.timer = new Timer()
            {
                Enabled = true,
                Interval = 350,
            };
            this.timer.Tick += Timer_Tick;
        }

        public void Clear()
        {
            for (int y = 0; y < this.screen.GetLength(1); y++)
            {
                for (int x = 0; x < this.screen.GetLength(0); x++)
                    this.screen[x, y] = ' ';
            }
            this.curX = 0;
            this.curY = 0;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.cursorVisible = !this.cursorVisible;
            this.Invalidate();
        }

        public void Write(string text)
        {
            for (int i = 0; i < text.Length; i++)
                this.Write(text[i]);
        }

        public void WriteLine(string text) => Write(text + "\r\n");

        public void Write(char c)
        {
            switch (c)
            {
                case '\n': this.curY += 1; break;
                case '\r': this.curX = 0; break;
                default:
                    if (curX >= 0 && curY >= 0 && curX < screen.GetLength(0) && curY < screen.GetLength(1))
                        this.screen[this.curX, this.curY] = c;
                    this.curX += 1;
                    if (this.curX >= this.screen.GetLength(0))
                    {
                        this.curX = 0;
                        this.curY += 1;
                    }
                    break;
            }
            if (this.curY >= this.screen.GetLength(1))
                WriteLine();
            this.Invalidate();
        }

        public void WriteLine()
        {
            for (int y = 1; y < this.screen.GetLength(1); y++)
            {
                for (int x = 0; x < this.screen.GetLength(0); x++)
                    this.screen[x, y - 1] = this.screen[x, y];
            }
            for (int x = 0; x < this.screen.GetLength(0); x++)
                this.screen[x, this.screen.GetUpperBound(1)] = ' ';
            this.curY -= 1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                for (int y = 0; y < this.screen.GetLength(1); y++)
                {
                    for (int x = 0; x < this.screen.GetLength(0); x++)
                    {
                        var pt = new Point(padding + advanceX * x, padding + advanceY * y);
                        var c = this.screen[x, y];
                        if (!char.IsControl(c))
                        {
                            e.Graphics.DrawString(
                                    c.ToString(),
                                    terminalFont,
                                    Brushes.Lime,
                                    pt.X + adjustX,
                                    pt.Y + adjustY);
                        }
                        if (x == curX && y == curY && cursorVisible)
                            e.Graphics.FillRectangle(Brushes.Lime, new Rectangle(pt, new Size(advanceX - 1, advanceY - 1)));
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public char this[int x, int y]
        {
            get { return this.screen[x, y]; }
            set { this.screen[x, y] = value; this.Invalidate(); }
        }

        public int CursorX
        {
            get { return this.curX; }
            set { this.curX = value; this.Invalidate(); }
        }

        public int CursorY
        {
            get { return this.curY; }
            set { this.curY = value; this.Invalidate(); }
        }
    }
}