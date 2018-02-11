using LoLa.Runtime;
using System;
using System.Collections.Generic;

namespace LoLa.IL
{
	public abstract class Instruction 
	{
		protected Instruction() { }
		
		/// <summary>
		/// Execute on the given stack/scope.
		/// </summary>
		/// <param name="stack">Stack.</param>
		/// <param name="scope">Scope.</param>
		/// <param name="ip>Instruction Pointer</param>
		public abstract void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch);
	}
}
