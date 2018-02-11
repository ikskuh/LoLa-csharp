using LoLa.IL;
using LoLa.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoLa.Runtime
{
	public abstract class Function
	{
		protected readonly int argc;
	
		protected Function() : this(0)
		{
			
		}
	
		protected Function(int argc)
		{
			this.argc = argc;
		}

        public abstract FunctionCall Call(Value[] args);
	}
}
