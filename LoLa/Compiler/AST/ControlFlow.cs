using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoLa.Compiler.AST
{
    public sealed class IfElse : Statement
    {
        public IfElse(Expression condition, IReadOnlyList<Statement> trueBody) :
            this(condition, trueBody, null)
        {

        }

        public IfElse(Expression condition, IReadOnlyList<Statement> trueBody, IReadOnlyList<Statement> falseBody)
        {
            this.Condition = condition;
            this.TrueBody = trueBody.ToArray();
            this.FalseBody = falseBody?.ToArray();
        }

        public override void Emit(CodeWriter writer)
        {
            Condition.Emit(writer);
            var endLabel = new Label();

            if (this.FalseBody == null)
            {
                // single body
                writer.JumpWhenFalse(endLabel);
                foreach (var item in TrueBody)
                    item.Emit(writer);
            }
            else
            {
                // double body
                var falseLabel = new Label();

                writer.JumpWhenFalse(falseLabel);
                foreach (var item in TrueBody)
                    item.Emit(writer);
                writer.Jump(endLabel);

                writer.DefineLabel(falseLabel);

                foreach (var item in FalseBody)
                    item.Emit(writer);
            }
            writer.DefineLabel(endLabel);
        }

        public Expression Condition { get; }
        public IReadOnlyList<Statement> TrueBody { get; }
        public IReadOnlyList<Statement> FalseBody { get; }
    }

    public sealed class WhileLoop : Statement
    {
        public WhileLoop(Expression condition, IReadOnlyList<Statement> body)
        {
            this.Condition = condition;
            this.Body = body.ToArray();
        }

        public override void Emit(CodeWriter writer)
        {
            var loopBegin = new Label();
            var loopEnd = new Label();

            writer.DefineLabel(loopBegin);

            this.Condition.Emit(writer);
            writer.JumpWhenFalse(loopEnd);

            foreach(var item in this.Body)
                item.Emit(writer);
            
            writer.Jump(loopBegin);
            writer.DefineLabel(loopEnd);
        }

        public Expression Condition { get; }
        public IReadOnlyList<Statement> Body { get; }
    }

    public sealed class ForLoop : Statement
    {
        public ForLoop(string variable, Expression array, IReadOnlyList<Statement> body)
        {
            this.Variable = variable;
            this.Array = array;
            this.Body = body?.ToArray();
        }

        public override void Emit(CodeWriter writer)
        {
            var bodyStart = new Label();
            var bodyEnd = new Label();
            
            writer.DeclareVariable(this.Variable);
            
            this.Array.Emit(writer);

            writer.MakeIterator(); // pops array, pushes array iterator value

            writer.DefineLabel(bodyStart);

            writer.IteratorNext(); // peeks iterator, pushes value, then true when there is data, otherwise false

            writer.JumpWhenFalse(bodyEnd);

            writer.Store(this.Variable);
            
            foreach (var item in this.Body)
                item.Emit(writer);

            writer.Jump(bodyStart);
            writer.DefineLabel(bodyEnd);
            writer.Pop();
        }

        public string Variable { get; }
        public Expression Array { get; }
        public IReadOnlyList<Statement> Body { get; }
    }
}
