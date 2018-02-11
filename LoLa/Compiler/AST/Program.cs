using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLa.Compiler.AST
{
    public sealed class Program : Function
    {
        public Program()
        {
            this.Name = Object.TapFunctionName;
        }

        public List<Function> Functions = new List<Function>();

        public T Instantiate<T>()
            where T : Object, new()
        {
            var obj = new T();

            obj.RegisterFunction(Object.TapFunctionName, this.Compile(obj, true));

            foreach (var fun in this.Functions)
                obj.RegisterFunction(fun.Name, fun.Compile(obj, false));
           
            return obj;
        }

        public Value Evaluate(Object environment)
        {
            var obj = new EvaluationEnvironment(environment);

            obj.RegisterFunction(Object.TapFunctionName, this.Compile(obj, true));

            foreach (var fun in this.Functions)
                obj.RegisterFunction(fun.Name, fun.Compile(obj, false));

            return obj.Tap();
        }
    }
}
