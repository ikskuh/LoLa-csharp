using LoLa.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoLa.Compiler.AST
{
    public abstract class Expression : Statement
    {
        protected Expression() { }
    }

    public abstract class RValue : Expression
    {
        protected RValue() { }
    }

    public abstract class LValue : Expression
    {
        protected LValue() { }

        public abstract void EmitStore(CodeWriter writer);
    }

    public sealed class VariableRef : LValue
    {
        public VariableRef(string name)
        {
            this.Variable = name;
        }

        public override void Emit(CodeWriter writer)
        {
            writer.Load(this.Variable);
        }

        public override void EmitStore(CodeWriter writer)
        {
            writer.Store(this.Variable);
        }

        public string Variable { get; }

        public override string ToString() => $"{Variable}";
    }

    public sealed class ArrayIndexer : LValue
    {
        public ArrayIndexer(string variable, Expression index)
        {
            this.Variable = variable;
            this.Index = index;
        }

        public override void Emit(CodeWriter writer)
        {
            this.Index.Emit(writer);
            writer.Load(this.Variable);
            writer.ArrayLoad();
        }

        public override void EmitStore(CodeWriter writer)
        {
            this.Index.Emit(writer);
            writer.Load(this.Variable);
            writer.ArrayStore();
        }

        public string Variable { get; }
        public Expression Index { get; }

        public override string ToString() => $"{Variable}[{Index}]";
    }

    public sealed class Literal : RValue
    {
        public Literal(Value value)
        {
            this.Value = value;
        }

        public override void Emit(CodeWriter writer)
        {
            writer.Push(this.Value);
        }

        public Value Value { get; }

        public override string ToString() => $"{Value}";
    }

    public sealed class BinaryOperator : RValue
    {
        public BinaryOperator(string @operator, Expression lhs, Expression rhs)
        {
            this.Operator = @operator;
            this.LeftHandSide = lhs;
            this.RightHandSide = rhs;
        }

        public override void Emit(CodeWriter writer)
        {
            this.RightHandSide.Emit(writer);
            this.LeftHandSide.Emit(writer);
            switch (this.Operator)
            {
                case "+": writer.Add(); break;
                case "-": writer.Subtract(); break;
                case "*": writer.Multiply(); break;
                case "/": writer.Divide(); break;
                case "%": writer.Modulo(); break;
                case "and": writer.And(); break;
                case "or": writer.Or(); break;

                case ">": writer.MoreThan(); break;
                case "<": writer.LessThan(); break;
                case "==": writer.Equals(); break;
                case "!=": writer.Differs(); break;
                case ">=": writer.MoreOrEqual(); break;
                case "<=": writer.LessOrEqual(); break;

                default: throw new NotSupportedException($"{Operator} is not a supported binary operator.");
            }
        }

        public string Operator { get; }
        public Expression LeftHandSide { get; }
        public Expression RightHandSide { get; }

        public override string ToString() => $"({LeftHandSide} {Operator} {RightHandSide})";
    }

    public sealed class UnaryOperator : RValue
    {
        public UnaryOperator(string @operator, Expression value)
        {
            this.Operator = @operator;
            this.Value = value;
        }

        public override void Emit(CodeWriter writer)
        {
            this.Value.Emit(writer);
            switch (this.Operator)
            {
                case "-": writer.Negate(); break;
                case "not": writer.Invert(); break;

                default: throw new NotSupportedException($"{Operator} is not a supported unary operator.");
            }
        }

        public string Operator { get; }
        public Expression Value { get; }

        public override string ToString() => $"{Operator}({Value})";
    }

    public sealed class FunctionCall : RValue
    {
        public FunctionCall(string name) : this(null, name, new Expression[0])
        {

        }

#if NETFX_35
        public FunctionCall(string name, IEnumerable<Expression> args) : this(null, name, args)
#else
        public FunctionCall(string name, IReadOnlyList<Expression> args) : this(null, name, args)
#endif
        {

        }

        public FunctionCall(string objectvar, string name) : this(objectvar, name, new Expression[0])
        {

        }

#if NETFX_35
        public FunctionCall(string objectvar, string name, IEnumerable<Expression> args)
#else
        public FunctionCall(string objectvar, string name, IReadOnlyList<Expression> args)
#endif
        {
            this.ObjectName = objectvar;
            this.Function = name;
            this.Arguments = args.ToList();
        }

        public override void Emit(CodeWriter writer)
        {
            for (int i = (this.Arguments.Count - 1); i >= 0; i--)
            {
                this.Arguments[i].Emit(writer);
            }
            if (this.ObjectName != null)
            {
                writer.Load(this.ObjectName);
                writer.CallObject(this.Function, this.Arguments.Count);
            }
            else
            {
                writer.Call(this.Function, this.Arguments.Count);
            }
        }

        public string Function { get; }
        public string ObjectName { get; }

#if NETFX_35
        public List<Expression> Arguments { get; }
#else
        public IReadOnlyList<Expression> Arguments { get; }
#endif
    }

    public sealed class ArrayLiteral : RValue
    {
        public ArrayLiteral() : this(new Expression[0])
        {

        }

#if NETFX_35
        public ArrayLiteral(IEnumerable<Expression> items)
#else
        public ArrayLiteral(IReadOnlyList<Expression> items)
#endif
        {
            this.Items = items.ToList();
        }


        public override void Emit(CodeWriter writer)
        {
            for (int i = (this.Items.Count - 1); i > 0; i++)
            {
                this.Items[i].Emit(writer);
            }
            writer.ArrayPack(this.Items.Count);
        }

#if NETFX_35
        public List<Expression> Items { get; }
#else
        public IReadOnlyList<Expression> Items { get; }
#endif
    }
}
