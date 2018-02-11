using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoLa.Compiler.AST
{
    public sealed class Declaration : Statement
    {
        public Declaration(string name)
        {
            this.Name = name;
        }

        public Declaration(string name, Expression value) : this(name)
        {
            this.Value = value;
        }

        public override void Emit(CodeWriter writer)
        {
            if (this.Value == null)
                return;
            
            // First get value, then declare, then store:
            // Allows var x = x; to shadow another variable

            this.Value.Emit(writer);

            writer.DeclareVariable(this.Name);

            writer.Store(this.Name);
        }

        public string Name { get; }

        public Expression Value { get; }
    }

    public sealed class Assignment : Statement
    {
        public Assignment(LValue target, Expression value)
        {
            this.Target = target;
            this.Value = value;
        }

        public override void Emit(CodeWriter writer)
        {
            this.Value.Emit(writer);
            this.Target.EmitStore(writer);
        }

        public LValue Target { get; }

        public Expression Value { get; }
    }

    public sealed class Return : Statement
    {
        public Return() : this(null)
        {

        }

        public Return(Expression value)
        {
            this.Value = value;
        }

        public override void Emit(CodeWriter writer)
        {
            if (this.Value != null)
                this.Value.Emit(writer);
            else
                writer.Push(LoLa.Runtime.Value.Null);
            writer.Return();
        }

        public Expression Value { get; }
    }

    public sealed class DiscardResult : Statement
    {
        public DiscardResult(Expression value)
        {
            this.Value = value;
        }

        public override void Emit(CodeWriter writer)
        {
            this.Value.Emit(writer);
            writer.Pop();
        }

        public Expression Value { get; }
    }
}
