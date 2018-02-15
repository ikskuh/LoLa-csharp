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
            private readonly Instruction.Context context;
            private readonly IL.Instruction[] code;

            public Caller(IL.ScriptFunction fun, Value[] args)
            {
                Scope scope;
                if (fun.IsTapFunction)
                    scope = fun.Environment;
                else
                    scope = new Scope(fun.Environment);
                
                for (int i = 0; i < fun.Parameters.Count; i++)
                {
                    var param = fun.Parameters[i];
                    scope.DeclareVariable(param, args[i]);
                }

                this.context = new Instruction.Context(scope);

                this.code = fun.Code.ToArray();
            }

            public override bool Next()
            {
                if(context.Branch != null)
                {
                    if (context.Branch.Next())
                        return true;
                    context.Stack.Push(context.Branch.Result);
                    context.LeaveFunction();
                }

                if (context.IP >= this.code.Length)
                    return false;
                
                var instr = this.code[context.IP];
                context.IP += 1;
                instr.Execute(context);

                if (context.IP < 0)
                {
                    this.Result = context.Stack.Pop();
                    return false;
                }
                if (context.IP >= this.code.Length)
                {
                    this.Result = Value.Null;
                    return false;
                }

                return true;
            }
        }
    }
}
