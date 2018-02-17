using LoLa.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace LoLa.Demo
{
	public class BasicComputer : LoLa.Runtime.LoLaObject
	{
		private readonly TerminalForm display;

		bool inputValid = false;
		bool allowInput = false;
		StringBuilder inputBuffer = new StringBuilder();

		Timer timer;
		FunctionCall tapFun;
		string tapCode;

        private readonly CodeEditorForm editor;

		public BasicComputer()
		{		
			this.timer = new Timer
			{
				Interval = 10,
                Enabled = true,
			};
			this.timer.Tick += Timer_Tick;

			this.display = new TerminalForm(20, 8);
			this.display.Show();
			
			this.display.MouseClick += (s,e) =>
			{
                if (e.Button != MouseButtons.Right)
                    return;
                this.editor.Show();
			};

			this.display.KeyDown += (s, e) =>
			{
				/*
                switch(e.KeyCode)
                {
                    case Keys.Left: this.display.CursorX--; break;
                    case Keys.Up: this.display.CursorY--; break;
                    case Keys.Right: this.display.CursorX++; break;
                    case Keys.Down: this.display.CursorY++; break;
                }
                */
			};
			this.display.KeyPress += (s, e) =>
			{
				if (!this.allowInput)
					return;
				if (e.KeyChar == '\r')
				{
					this.inputValid = true;
				}
				else
				{
					this.display.Write(e.KeyChar);
					this.inputBuffer.Append(e.KeyChar);
				}
				e.Handled = true;
			};
            
            this.editor = new CodeEditorForm
            {
                Owner = this.display
            };
            this.editor.Run += (s, e) =>
            {   
                try
                {
                    var pgm = Compiler.Compiler.Compile(this.editor.Code);
                    pgm.InstantiateInto(this, true);

                    this.tapFun = this.Tap();
                    this.timer.Start();

                    this.tapCode = this.editor.Code;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this.editor, ex.ToString());
                }
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

			RegisterAsyncFunction("ReadLine", 0, ReadLine);
		}

		void Timer_Tick(object sender, EventArgs e)
		{
			if(this.tapFun == null)
				return;
			if(this.tapFun.Next() == false)
				this.tapFun = null;
		}
		
		private IEnumerable<Value?> ReadLine(Value[] args)
		{
			this.allowInput = true;
			this.inputValid = false;
			this.inputBuffer.Clear();
			while (!this.inputValid)
				yield return null;
			this.allowInput = false;
			yield return this.inputBuffer.ToString();
		}

		public TerminalForm Display => this.display;
	}
}
