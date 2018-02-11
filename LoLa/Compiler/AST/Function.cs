using LoLa.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLa.Compiler.AST
{
    public class Function
    {
        public string Name;

        public List<string> Parameters = new List<string>();

        public List<Statement> Statements = new List<Statement>();

        public ScriptFunction Compile(LoLa.Runtime.LoLaObject env, bool isTapFunction)
        {
            var code = new CodeWriter();
            foreach (var item in this.Statements)
                item.Emit(code);
            return new ScriptFunction(env, this.Name, this.Parameters.ToArray(), code, isTapFunction);
        }
    }
}
