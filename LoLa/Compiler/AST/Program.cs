using LoLa.Runtime;
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
            this.Name = LoLa.Runtime.LoLaObject.TapFunctionName;
        }

        public List<Function> Functions = new List<Function>();

        public T Instantiate<T>()
            where T : LoLa.Runtime.LoLaObject, new()
        {
            var obj = new T();

            obj.RegisterFunction(LoLa.Runtime.LoLaObject.TapFunctionName, this.Compile(obj, true));

            foreach (var fun in this.Functions)
                obj.RegisterFunction(fun.Name, fun.Compile(obj, false));
           
            return obj;
        }

        public LoLa.Runtime.FunctionCall Evaluate(LoLa.Runtime.LoLaObject environment)
        {
            var obj = new EvaluationEnvironment(environment);

            obj.RegisterFunction(LoLa.Runtime.LoLaObject.TapFunctionName, this.Compile(obj, true));

            foreach (var fun in this.Functions)
                obj.RegisterFunction(fun.Name, fun.Compile(obj, false));

            return obj.Tap();
        }
    }
}
