using System;
using System.Linq;
using LoLa.Runtime;

namespace LoLa.Runtime
{
	public delegate Value NativeFunctionDelegate(Value[] args);

	public sealed class NativeFunction : Function
	{
		private readonly NativeFunctionDelegate fun;
		
		public NativeFunction(NativeFunctionDelegate fun) : this(fun, 0)
		{
		
		}
		
		public NativeFunction(NativeFunctionDelegate fun, int argc) : base(argc)
		{
			if(fun == null)
				throw new ArgumentNullException(nameof(fun));
			this.fun = fun;
		}

        public override FunctionCall Call(Value[] args) => new SynchronousCaller(this, args);

        class SynchronousCaller : FunctionCall
        {
            private NativeFunction nativeFunction;
            private Value[] args;

            public SynchronousCaller(NativeFunction nativeFunction, Value[] args)
            {
                this.nativeFunction = nativeFunction;
                this.args = args;
            }

            public override bool Next()
            {
                this.Result = this.nativeFunction.fun(this.args);
                return false;
            }
        }
    }
}
