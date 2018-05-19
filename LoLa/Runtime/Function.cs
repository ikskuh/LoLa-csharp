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

        public int MinArgumentCount => this.argc;

        /// <summary>
        /// Creates a function call that can evaluate the function result over time.
        /// </summary>
        /// <param name="args">Argument list which contains at least MinArgumentCount arguments.</param>
        /// <returns></returns>
        public abstract FunctionCall Call(Value[] args);
	}
}
