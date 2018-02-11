using LoLa.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoLa.Demo
{
    public class BasicComputer : LoLa.Runtime.LoLaObject
    {
        private readonly TerminalForm display;

        StringBuilder inputBuffer = new StringBuilder();

        public BasicComputer()
        {
            this.display = new TerminalForm(20,8);
            this.display.Show();

            this.display.KeyDown += (s, e) =>
            {
                switch(e.KeyCode)
                {
                    case Keys.Left: this.display.CursorX--; break;
                    case Keys.Up: this.display.CursorY--; break;
                    case Keys.Right: this.display.CursorX++; break;
                    case Keys.Down: this.display.CursorY++; break;
                }
            };
            this.display.KeyPress += (s, e) =>
            {
                this.display.Write(e.KeyChar);
                e.Handled = true;
            };

            RegisterFunction("PrintLn", args =>
            {
                this.display.WriteLine(string.Join("", args));
                return Value.Null;
            });
            RegisterFunction("Print", args =>
            {
                this.display.Write(string.Join("", args));
                return Value.Null;
            });
            RegisterFunction("SetCursor", 2, args =>
            {
                this.display.CursorX = (int)args[0].ToNumber();
                this.display.CursorY = (int)args[1].ToNumber();
                return Value.Null;
            });
            RegisterFunction("Clear", args =>
            {
                this.display.Clear();
                return Value.Null;
            });
            RegisterFunction("Home", args =>
            {
                this.display.CursorX = 0;
                this.display.CursorY = 0;
                return Value.Null;
            });
        }

        public TerminalForm Display => this.display;
    }
}
