using LoLa.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoLa.Compiler.AST
{
    public class Function
    {
        public string Name;

        public List<string> Parameters = new List<string>();

        public virtual Statement Body { get; set; }

        public ScriptFunction Compile(LoLa.Runtime.LoLaObject env, bool isTapFunction)
        {
            var code = new CodeWriter();
            Body.Emit(code);
            return new ScriptFunction(env, this.Name, this.Parameters.ToArray(), code, isTapFunction);
        }
    }
}
