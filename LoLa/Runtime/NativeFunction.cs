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

        /*
		
		public NativeFunction(Delegate fun) : base(fun.Method.GetParameters().Length)
		{
			var n_return = fun.Method.ReturnType;
			var n_params = fun.Method.GetParameters();
			if(n_params.Any(p => p.IsOut))
				throw new ArgumentException("All parameters must be [In]!");
			
			if(n_params.Any(p => Map(p.ParameterType) == Type.Void))
				throw new ArgumentException("A parameter does not match a supported type!");
			
			var l_return = Map(n_return);
			
			this.fun = new NativeFunctionDelegate((l_args) => 
			{
				object[] n_args = new object[n_params.Length];
				for(int i = 0; i < n_args.Length; i++)
				{
					n_args[i] = Convert(l_args[i], n_params[i].ParameterType);
				}
				
				object result = fun.DynamicInvoke(n_args);
				
				if(n_return != typeof(void))
					return Convert(result, l_return);
				else
					return Value.Null;
			});
		}
		
		private static readonly Tuple<LoLa.Type, System.Type>[] mapping = new []
		{
			Tuple.Create(Type.Void, typeof(void)),
			Tuple.Create(Type., typeof(void)),
			Tuple.Create(Type.Void, typeof(void)),
			Tuple.Create(Type.Void, typeof(void)),
			Tuple.Create(Type.Void, typeof(void)),
			Tuple.Create(Type.Void, typeof(void)),
			Tuple.Create(Type.Void, typeof(void)),
			Tuple.Create(Type.Void, typeof(void)),
			Tuple.Create(Type.Void, typeof(void)),
		};
		
		private static LoLa.Type Map(System.Type type)
		{
			if(type == typeof(void))
				return Type.Void;
			else if(
		}
		
		private static object Convert(Value val, System.Type target)
		{
		
		}
		
		private static Value Convert(object val, LoLa.Type type)
		{
			
		}
		*/
    }
}
