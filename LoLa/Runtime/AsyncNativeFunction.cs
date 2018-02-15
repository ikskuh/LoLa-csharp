using System;
using System.Collections.Generic;

namespace LoLa.Runtime
{
	public delegate IEnumerable<Value?> AsyncNativeFunctionDelegate(Value[] args);

	public sealed class AsyncNativeFunction : Function
	{
		private readonly AsyncNativeFunctionDelegate fun;

		public AsyncNativeFunction(AsyncNativeFunctionDelegate fun) : this(fun, 0)
		{

		}

		public AsyncNativeFunction(AsyncNativeFunctionDelegate fun, int argc) : base(argc)
		{
			if (fun == null)
				throw new ArgumentNullException(nameof(fun));
			this.fun = fun;
		}

		public override FunctionCall Call(Value[] args) => new SynchronousCaller(this, args);

		class SynchronousCaller : FunctionCall
		{
			private readonly IEnumerator<Value?> task;

			public SynchronousCaller(AsyncNativeFunction nativeFunction, Value[] args)
			{
				this.task = nativeFunction.fun(args).GetEnumerator();
			}

			public override bool Next()
			{
				if (this.task.MoveNext() == false)
					return false;

				if (this.task.Current != null)
				{
					this.Result = this.task.Current.Value;
					return false;
				}

				return true;
			}
		}
	}
}
