using System;
using System.Collections.Generic;
using System.Linq;

namespace LoLa.IL
{
	public sealed class ScriptFunction : Function
	{
		private readonly Instruction[] code;

        private readonly string[] @params;
        
        public ScriptFunction(Object env, string name, string[] args, IReadOnlyList<Instruction> code, bool isTapFunction) :
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
			this.code = code.ToArray();
            this.IsTapFunction = isTapFunction;
            if (this.IsTapFunction && (this.@params.Length > 0))
                throw new InvalidOperationException("Tap function must not have arguments!");
		}

		protected override Value Execute(Value[] args)
		{
			int ip = 0;
            Scope scope;
            if (this.IsTapFunction)
                scope = this.Environment;
            else
                scope = new Scope(this.Environment);

            for (int i = 0; i < this.@params.Length; i++)
            {
                var param = this.@params[i];
                scope.DeclareVariable(param, args[i]);
            }
			
			var stack = new Stack<Value>();
			while (ip < this.code.Length)
			{
				var instr = this.code[ip];
				ip += 1;
				instr.Execute(stack, scope, ref ip);
				if(ip < 0)
					break;
			}
            if (ip < this.code.Length)
                return stack.Pop();
            else
                return Value.Null;
		}

		public string Name { get; }

		public IReadOnlyList<Instruction> Code { get; }
		
		public IReadOnlyList<string> Parameters => this.@params;
		
		public Object Environment { get; }

        public bool IsTapFunction { get; }
    }
}
