using LoLa.Runtime;
using System;
using System.Collections.Generic;
namespace LoLa.IL.Instructions
{
    public sealed class DeclareVariable : Instruction
    {
        public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
        {
            scope.DeclareVariable(Variable);
        }

        public string Variable { get; set; }

        public override string ToString() => $"declare {Variable}";
    }

    public sealed class StoreVariable : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			scope[Variable] = stack.Pop();
		}

		public string Variable { get; set; }

        public override string ToString() => $"store {Variable}";
    }

	public sealed class LoadVariable : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			stack.Push(scope[Variable]);
		}

		public string Variable { get; set; }

        public override string ToString() => $"load {Variable}";
    }


	public sealed class PushValue : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			stack.Push(Value);
		}

		public Value Value { get; set; }

        public override string ToString() => $"push {Value}";
    }


	public sealed class ArrayPack : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var array = new LoLa.Runtime.Array();
			for (int i = 0; i < array.Count; i++)
				array[i] = stack.Pop();
			stack.Push(array);
		}

		public int Size { get; set; }

        public override string ToString() => $"array_pack {Size}";
    }


	public sealed class Call : Instruction
	{
        public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
        {
            Function fun = scope.GetFunction(this.Function);

            var args = new Value[this.ArgumentCount];
            for (int i = 0; i < args.Length; i++)
                args[i] = stack.Pop();

            branch = fun.Call(args);
        }

        public string Function { get; set; }
        public int ArgumentCount { get; set; }

        public override string ToString() => $"call {Function} {ArgumentCount}";
    }

    public sealed class CallObject : Instruction
    {
        public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
        {
            var obj = stack.Pop().ToObject();
            var fun = obj.GetFunction(this.Function);
            
            var args = new Value[this.ArgumentCount];
            for (int i = 0; i < args.Length; i++)
                args[i] = stack.Pop();

            branch = fun.Call(args);
        }

        public string Function { get; set; }
        public int ArgumentCount { get; set; }


        public override string ToString() => $"objcall {Function} {ArgumentCount}";
    }

    public sealed class Pop : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			stack.Pop();
        }

        public override string ToString() => $"pop";
    }
    
	public sealed class Add : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();

			switch (lhs.Type)
			{
				case LoLa.Runtime.Type.Array:
					{
						var result = new LoLa.Runtime.Array();
						foreach (var val in lhs.ToArray())
							result.Add(val);
						foreach (var val in rhs.ToArray())
							result.Add(val);
						stack.Push(result);
						break;
					}
				case LoLa.Runtime.Type.Number:
					stack.Push(lhs.ToNumber() + rhs.ToNumber());
					break;
				case LoLa.Runtime.Type.String:
					stack.Push(lhs.ToString() + rhs.ToString());
					break;
				default:
					throw new NotSupportedException($"Addition for {lhs.Type} not supported!");
			}
        }

        public override string ToString() => $"add";
    }


	public sealed class Subtract : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToNumber() - rhs.ToNumber());
        }

        public override string ToString() => $"sub";
    }

	public sealed class Multiply : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToNumber() * rhs.ToNumber());
        }

        public override string ToString() => $"mul";
    }

	public sealed class Divide : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToNumber() / rhs.ToNumber());
        }

        public override string ToString() => $"div";
    }

	public sealed class Modoluo : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToNumber() % rhs.ToNumber());
        }

        public override string ToString() => $"mod";
    }

	public sealed class Negate : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			stack.Push(-stack.Pop().ToNumber());
        }

        public override string ToString() => $"negate";
    }

	public sealed class And : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToBoolean() & rhs.ToBoolean());
        }

        public override string ToString() => $"and";
    }

	public sealed class Or : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToBoolean() | rhs.ToBoolean());
        }

        public override string ToString() => $"or";
    }

	public sealed class Invert : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			stack.Push(!lhs.ToBoolean());
        }

        public override string ToString() => $"invert";
    }

	public sealed class Equals : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.Equals(rhs));
        }

        public override string ToString() => $"equals";
    }

	public sealed class Differs : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(!lhs.Equals(rhs));
        }

        public override string ToString() => $"differs";
    }

	public sealed class LessThan : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToNumber() < rhs.ToNumber());
        }

        public override string ToString() => $"less_than";
    }

	public sealed class LessOrEqual : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToNumber() <= rhs.ToNumber());
        }

        public override string ToString() => $"less_or_equal";
    }

	public sealed class MoreThan : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToNumber() > rhs.ToNumber());
        }

        public override string ToString() => $"more_than";
    }
	public sealed class MoreOrEqual : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var lhs = stack.Pop();
			var rhs = stack.Pop();
			stack.Push(lhs.ToNumber() >= rhs.ToNumber());
        }

        public override string ToString() => $"more_or_equal";
    }


	public sealed class Jump : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			ip = Target;
		}

		public int Target { get; set; }

        public override string ToString() => $"jmp {Target}";
    }

	public sealed class JumpWhenFalse : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			if (stack.Pop().ToBoolean() == false)
				ip = Target;
		}

		public int Target { get; set; }

        public override string ToString() => $"jmp_when_false {Target}";
    }

	public sealed class MakeIterator : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var array = stack.Pop().ToArray();
			stack.Push(new Value(array.GetEnumerator()));
        }

        public override string ToString() => $"make_iterator";
    }

	public sealed class IteratorNext : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var e = stack.Peek().ToEnumerator();
			var next = e.MoveNext();
			if(next)
				stack.Push(e.Current);
			stack.Push(next);
        }

        public override string ToString() => $"iterator_next";
    }

	public sealed class ArrayStore : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			var array = stack.Pop().ToArray();
			var index = stack.Pop().ToNumber();
			var value = stack.Pop();
			array[(int)index] = value;
        }

        public override string ToString() => $"array_store";
    }

	public sealed class ArrayLoad : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
        {
            var array = stack.Pop().ToArray();
            var index = stack.Pop().ToNumber();
            stack.Push(array[(int)index]);
        }

        public override string ToString() => $"array_load";
    }

	public sealed class Return : Instruction
	{
		public override void Execute(Stack<Value> stack, Scope scope, ref int ip, ref LoLa.Runtime.FunctionCall branch)
		{
			ip = -1;
		}

        public override string ToString() => $"return";
    }
}