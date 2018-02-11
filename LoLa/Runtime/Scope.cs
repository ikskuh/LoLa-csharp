using System.Collections.Generic;
using System;
namespace LoLa
{
    public class Scope
    {
        private readonly Scope shadow;
        protected readonly Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

        protected Scope()
        {
            this.shadow = null;
        }

        public Scope(Scope shadow)
        {
            if (shadow == null)
                throw new ArgumentNullException(nameof(shadow));
            this.shadow = shadow;
        }

        public virtual Function GetFunction(string name)
        {
            if (this.shadow != null)
                return this.shadow.GetFunction(name);
            else
                throw new NotSupportedException("This scope does not support free functions.");
        }

        public void DeclareVariable(string name, Value value = default(Value))
        {
            this.variables.Add(name, new Variable(name)
            {
                Value = value
            });
        }

        public Variable GetVariable(string name)
        {
            Variable var;
            if (this.variables.TryGetValue(name, out var))
                return var;
            else if (this.shadow != null)
                return this.shadow.GetVariable(name);
            else
                throw new InvalidOperationException($"The variable {name} does not exist!");
        }

        public Value this[string name]
        {
            get { return this.GetVariable(name).Value; }
            set { this.GetVariable(name).Value = value; }
        }
    }
}