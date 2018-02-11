using LoLa.IL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoLa
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
		
		public Value Call(Value[] args)
		{
			if(args.Length < this.argc)
				System.Array.Resize(ref args, this.argc);
			return this.Execute(args);
		}
		
		protected abstract Value Execute(Value[] args);
	}

    public sealed class ExecutionContext
    {
        private readonly Stack<Call> calls = new Stack<Call>();

        public ExecutionContext()
        {

        }


        abstract class Call
        {
            protected readonly Function fun;

            protected Call(Function fun)
            {
                this.fun = fun;
            }

            public abstract bool Next();

            public Value ReturnValue { get; protected set; }
        }

        class ScriptCall : Call
        {
            private readonly Scope scope;
            private readonly Stack<Value> stack = new Stack<Value>();
            private int ip = 0;
            private readonly Instruction[] code;

            public ScriptCall(ScriptFunction fun, Value[] args) : base(fun)
            {
                if (fun.IsTapFunction)
                    this.scope = fun.Environment;
                else
                    this.scope = new Scope(fun.Environment);

                for (int i = 0; i < fun.Parameters.Count; i++)
                {
                    var param = fun.Parameters[i];
                    scope.DeclareVariable(param, args[i]);
                }

                this.code = fun.Code.ToArray();
            }

            public override bool Next()
            {
                if (ip >= this.code.Length)
                    return false;

                var instr = this.code[ip];
                ip += 1;
                instr.Execute(stack, scope, ref ip);

                if(ip < 0)
                {
                    this.ReturnValue = stack.Pop();
                    return false;
                }
                if(ip >= this.code.Length)
                {
                    this.ReturnValue = Value.Null;
                    return false;
                }

                return true;
            }
        }
    }
}
