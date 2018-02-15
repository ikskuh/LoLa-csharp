using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoLa.Compiler.AST
{
    public sealed class IfElse : Statement
    {
        public IfElse(Expression condition, Statement trueBody) :
            this(condition, trueBody, null)
        {

        }

        public IfElse(Expression condition, Statement trueBody, Statement falseBody)
        {
            this.Condition = condition;
            this.TrueBody = trueBody;
            this.FalseBody = falseBody;
        }

        public override void Emit(CodeWriter writer)
        {
            Condition.Emit(writer);
            var endLabel = new Label();

            if (this.FalseBody == null)
            {
                // single body
                writer.JumpWhenFalse(endLabel);
                TrueBody.Emit(writer);
            }
            else
            {
                // double body
                var falseLabel = new Label();

                writer.JumpWhenFalse(falseLabel);
                TrueBody.Emit(writer);
                writer.Jump(endLabel);

                writer.DefineLabel(falseLabel);

                FalseBody.Emit(writer);
            }
            writer.DefineLabel(endLabel);
        }

        public Expression Condition { get; }
        public Statement TrueBody { get; }
        public Statement FalseBody { get; }
    }

    public sealed class WhileLoop : Statement
    {
        public WhileLoop(Expression condition, Statement body)
        {
            this.Condition = condition;
            this.Body = body;
        }

        public override void Emit(CodeWriter writer)
        {
            var loopBegin = new Label();
            var loopEnd = new Label();

            writer.DefineLabel(loopBegin);

            this.Condition.Emit(writer);
            writer.JumpWhenFalse(loopEnd);

            this.Body.Emit(writer);

            writer.Jump(loopBegin);
            writer.DefineLabel(loopEnd);
        }

        public Expression Condition { get; }
        public Statement Body { get; }
    }

    public sealed class ForLoop : Statement
    {
        public ForLoop(string variable, Expression array, Statement body)
        {
            this.Variable = variable;
            this.Array = array;
            this.Body = body;
        }

        public override void Emit(CodeWriter writer)
        {
            var bodyStart = new Label();
            var bodyEnd = new Label();

            writer.EnterScope();
            writer.DeclareVariable(this.Variable);

            this.Array.Emit(writer);

            writer.MakeIterator(); // pops array, pushes array iterator value

            writer.DefineLabel(bodyStart);

            writer.IteratorNext(); // peeks iterator, pushes value, then true when there is data, otherwise false

            writer.JumpWhenFalse(bodyEnd);

            writer.Store(this.Variable);

            this.Body.Emit(writer);

            writer.Jump(bodyStart);
            writer.DefineLabel(bodyEnd);
            writer.Pop();
            writer.LeaveScope();
        }

        public string Variable { get; }
        public Expression Array { get; }
        public Statement Body { get; }
    }
}
