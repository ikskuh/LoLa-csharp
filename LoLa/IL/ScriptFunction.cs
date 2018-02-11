using System;
using System.Collections.Generic;
using System.Linq;
using LoLa.Runtime;

namespace LoLa.IL
{
	public sealed class ScriptFunction : Function
	{
        private readonly string[] @params;
        
        public ScriptFunction(LoLa.Runtime.LoLaObject env, string name, string[] args, IReadOnlyList<Instruction> code, bool isTapFunction) :
			base(args.Length)
		{
			if(env == null)
				throw new ArgumentNullException(nameof(env));
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (code == null)
				throw new ArgumentNullException(nameof(code));
			this.Environment = env;
			this.Name = name;
			this.@params = args.ToArray();
			this.Code = code.ToArray();
            this.IsTapFunction = isTapFunction;
            if (this.IsTapFunction && (this.@params.Length > 0))
                throw new InvalidOperationException("Tap function must not have arguments!");
		}

        public override FunctionCall Call(Value[] args) => new Caller(this, args);

        public string Name { get; }

		public IReadOnlyList<Instruction> Code { get; }
		
		public IReadOnlyList<string> Parameters => this.@params;
		
		public LoLa.Runtime.LoLaObject Environment { get; }

        public bool IsTapFunction { get; }

        private class Caller : FunctionCall
        {
            private readonly Scope scope;
            private readonly Stack<Value> stack = new Stack<Value>();
            private int ip = 0;
            private readonly IL.Instruction[] code;

            private FunctionCall subcall = null;

            public Caller(IL.ScriptFunction fun, Value[] args)
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
                if(subcall != null)
                {
                    if (subcall.Next())
                        return true;
                    stack.Push(subcall.Result);
                    subcall = null;
                }

                if (ip >= this.code.Length)
                    return false;
                
                var instr = this.code[ip];
                ip += 1;
                instr.Execute(stack, scope, ref ip, ref this.subcall);

                if (ip < 0)
                {
                    this.Result = stack.Pop();
                    return false;
                }
                if (ip >= this.code.Length)
                {
                    this.Result = Value.Null;
                    return false;
                }

                return true;
            }
        }
    }
}
