using LoLa.Runtime;
using System;
using System.Collections.Generic;
namespace LoLa.IL.Instructions
{
    public sealed class PushScope : Instruction
    {
        public override void Execute(Context context) => context.PushScope();

        public override string ToString() => "scope_push";
    }

    public sealed class PopScope : Instruction
    {
        public override void Execute(Context context) => context.PopScope();

        public override string ToString() => "scope_pop";
    }

    public sealed class DeclareVariable : Instruction
    {
        public override void Execute(Context context) => context.Scope.DeclareVariable(Variable);

        public string Variable { get; set; }

        public override string ToString() => $"declare {Variable}";
    }

    public sealed class StoreVariable : Instruction
	{
		public override void Execute(Context context)
		{
			context.Scope[Variable] = context.Stack.Pop();
		}

		public string Variable { get; set; }

        public override string ToString() => $"store {Variable}";
    }

	public sealed class LoadVariable : Instruction
	{
		public override void Execute(Context context)
		{
			context.Stack.Push(context.Scope[Variable]);
		}

		public string Variable { get; set; }

        public override string ToString() => $"load {Variable}";
    }


	public sealed class PushValue : Instruction
	{
		public override void Execute(Context context)
		{
			context.Stack.Push(Value);
		}

		public Value Value { get; set; }

        public override string ToString() => $"push {Value}";
    }


	public sealed class ArrayPack : Instruction
	{
		public override void Execute(Context context)
		{
			var array = new LoLa.Runtime.Array();
			for (int i = 0; i < array.Count; i++)
				array[i] = context.Stack.Pop();
			context.Stack.Push(array);
		}

		public int Size { get; set; }

        public override string ToString() => $"array_pack {Size}";
    }


	public sealed class Call : Instruction
	{
        public override void Execute(Context context)
        {
            Function fun = context.Scope.GetFunction(this.Function);

            var args = new Value[this.ArgumentCount];
            for (int i = 0; i < args.Length; i++)
                args[i] = context.Stack.Pop();

            context.EnterFunction(fun.Call(args));
        }

        public string Function { get; set; }
        public int ArgumentCount { get; set; }

        public override string ToString() => $"call {Function} {ArgumentCount}";
    }

    public sealed class CallObject : Instruction
    {
        public override void Execute(Context context)
        {
            var obj = context.Stack.Pop().ToObject();
            var fun = obj.GetFunction(this.Function);
            
            var args = new Value[this.ArgumentCount];
            for (int i = 0; i < args.Length; i++)
                args[i] = context.Stack.Pop();

            context.EnterFunction(fun.Call(args));
        }

        public string Function { get; set; }
        public int ArgumentCount { get; set; }


        public override string ToString() => $"objcall {Function} {ArgumentCount}";
    }

    public sealed class Pop : Instruction
	{
		public override void Execute(Context context)
		{
			context.Stack.Pop();
        }

        public override string ToString() => $"pop";
    }
    
	public sealed class Add : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();

			switch (lhs.Type)
			{
				case LoLa.Runtime.Type.Array:
					{
						var result = new LoLa.Runtime.Array();
						foreach (var val in lhs.ToArray())
							result.Add(val);
						foreach (var val in rhs.ToArray())
							result.Add(val);
                        context.Stack.Push(result);
						break;
					}
				case LoLa.Runtime.Type.Number:
                    context.Stack.Push(lhs.ToNumber() + rhs.ToNumber());
					break;
				case LoLa.Runtime.Type.String:
                    context.Stack.Push(lhs.ToString() + rhs.ToString());
					break;
				default:
					throw new NotSupportedException($"Addition for {lhs.Type} not supported!");
			}
        }

        public override string ToString() => $"add";
    }


	public sealed class Subtract : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
            context.Stack.Push(lhs.ToNumber() - rhs.ToNumber());
        }

        public override string ToString() => $"sub";
    }

	public sealed class Multiply : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.ToNumber() * rhs.ToNumber());
        }

        public override string ToString() => $"mul";
    }

	public sealed class Divide : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.ToNumber() / rhs.ToNumber());
        }

        public override string ToString() => $"div";
    }

	public sealed class Modoluo : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.ToNumber() % rhs.ToNumber());
        }

        public override string ToString() => $"mod";
    }

	public sealed class Negate : Instruction
	{
		public override void Execute(Context context)
		{
			context.Stack.Push(-context.Stack.Pop().ToNumber());
        }

        public override string ToString() => $"negate";
    }

	public sealed class And : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.ToBoolean() & rhs.ToBoolean());
        }

        public override string ToString() => $"and";
    }

	public sealed class Or : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.ToBoolean() | rhs.ToBoolean());
        }

        public override string ToString() => $"or";
    }

	public sealed class Invert : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			context.Stack.Push(!lhs.ToBoolean());
        }

        public override string ToString() => $"invert";
    }

	public sealed class Equals : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.Equals(rhs));
        }

        public override string ToString() => $"equals";
    }

	public sealed class Differs : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(!lhs.Equals(rhs));
        }

        public override string ToString() => $"differs";
    }

	public sealed class LessThan : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.ToNumber() < rhs.ToNumber());
        }

        public override string ToString() => $"less_than";
    }

	public sealed class LessOrEqual : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.ToNumber() <= rhs.ToNumber());
        }

        public override string ToString() => $"less_or_equal";
    }

	public sealed class MoreThan : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.ToNumber() > rhs.ToNumber());
        }

        public override string ToString() => $"more_than";
    }
	public sealed class MoreOrEqual : Instruction
	{
		public override void Execute(Context context)
		{
			var lhs = context.Stack.Pop();
			var rhs = context.Stack.Pop();
			context.Stack.Push(lhs.ToNumber() >= rhs.ToNumber());
        }

        public override string ToString() => $"more_or_equal";
    }


	public sealed class Jump : Instruction
	{
		public override void Execute(Context context)
		{
			context.IP = Target;
		}

		public int Target { get; set; }

        public override string ToString() => $"jmp {Target}";
    }

	public sealed class JumpWhenFalse : Instruction
	{
		public override void Execute(Context context)
		{
			if (context.Stack.Pop().ToBoolean() == false)
				context.IP = Target;
		}

		public int Target { get; set; }

        public override string ToString() => $"jmp_when_false {Target}";
    }

	public sealed class MakeIterator : Instruction
	{
		public override void Execute(Context context)
		{
			var array = context.Stack.Pop().ToArray();
			context.Stack.Push(new Value(array.GetEnumerator()));
        }

        public override string ToString() => $"make_iterator";
    }

	public sealed class IteratorNext : Instruction
	{
		public override void Execute(Context context)
		{
			var e = context.Stack.Peek().ToEnumerator();
			var next = e.MoveNext();
			if(next)
				context.Stack.Push(e.Current);
			context.Stack.Push(next);
        }

        public override string ToString() => $"iterator_next";
    }

	public sealed class ArrayStore : Instruction
	{
		public override void Execute(Context context)
		{
			var array = context.Stack.Pop().ToArray();
			var index = context.Stack.Pop().ToNumber();
			var value = context.Stack.Pop();
			array[(int)index] = value;
        }

        public override string ToString() => $"array_store";
    }

	public sealed class ArrayLoad : Instruction
	{
		public override void Execute(Context context)
        {
            var array = context.Stack.Pop().ToArray();
            var index = context.Stack.Pop().ToNumber();
            context.Stack.Push(array[(int)index]);
        }

        public override string ToString() => $"array_load";
    }

	public sealed class Return : Instruction
	{
		public override void Execute(Context context)
		{
			context.IP = -1;
		}

        public override string ToString() => $"return";
    }
}