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

		public BasicComputer()
		{		
			this.timer = new Timer
			{
				Interval = 10,
			};
			this.timer.Tick += Timer_Tick;

			this.display = new TerminalForm(20, 8);
			this.display.Show();
			
			this.display.MouseClick += (s,e) =>
			{
				if(e.Button != MouseButtons.Right)
					return;
				var frm = new Form
				{
					Text = "Code Editor (" + display.Text + ")",
					ClientSize = new Size(640, 480),
					StartPosition = FormStartPosition.CenterParent
				};
				var edit = new TextBox
				{
					Multiline = true,
					Font = new Font(FontFamily.GenericMonospace, 16.0f),
					Dock = DockStyle.Fill,
					Text = this.tapCode,
					AcceptsTab = true,
					AcceptsReturn = true,
				};
				
				edit.KeyUp += (se,ea) =>
				{
					if(ea.Control && ea.KeyCode == Keys.R)
					{
						ea.Handled = true;
						frm.DialogResult = DialogResult.OK;
					}
					if(ea.KeyCode == Keys.Escape)
						frm.DialogResult = DialogResult.Cancel;
				};
				
				frm.Controls.Add(edit);
				
				this.timer.Stop();
				
				if(frm.ShowDialog(this.display) != DialogResult.OK)
					return;
				
				try
				{
					var pgm = Compiler.Compiler.Compile(edit.Text);
					pgm.InstantiateInto(this, true);
					
					this.tapFun = this.Tap();
					this.timer.Start();
					
					this.tapCode = edit.Text;
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
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
				if (e.KeyChar == '\n')
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
