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
		public abstract void Execute(Context context);
        
        public sealed class Context
        {
            private readonly Stack<Scope> scopes = new Stack<Scope>();

            private FunctionCall branch;
            
            public Context(Scope scope)
            {
                this.scopes.Push(scope);
            }

            public void EnterFunction(FunctionCall call) => this.branch = call;

            public void LeaveFunction() => this.branch = null;

            public void PushScope() => this.scopes.Push(new Scope(this.Scope));

            public void PopScope() => this.scopes.Pop();

            public Stack<Value> Stack { get; } = new Stack<Value>();
            public Scope Scope => this.scopes.Peek();
            public int IP { get; set; } = 0;
            public FunctionCall Branch => this.branch;
        }
    }
}
