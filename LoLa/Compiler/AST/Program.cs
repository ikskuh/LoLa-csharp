using LoLa.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoLa.IL;

namespace LoLa.Compiler.AST
{
    /// <summary>
    /// A compiled, but not instantiated LoLa program.
    /// </summary>
	public sealed class Program : Function
	{
		public Program()
		{
			this.Name = LoLa.Runtime.LoLaObject.TapFunctionName;
		}

		public List<Function> Functions = new List<Function>();

        public List<Statement> Statements = new List<Statement>();

        public override Statement Body { get => new SubScope(this.Statements); set => throw new NotSupportedException("Main Program does not support setting the body."); }

        /// <summary>
        /// Instantiate this program into the given lola object.
        /// </summary>
        /// <param name="obj">The target object that should receive the compiled code</param>
        /// <param name="removePreviousScripts">Remove all previously created script functions.</param>
        public void InstantiateInto(LoLaObject obj, bool removePreviousScripts)
		{
			if (removePreviousScripts)
			{
				foreach (var fun in obj.Functions.Where(f => f.Value is ScriptFunction).ToArray())
					obj.RemoveFunction(fun.Key);
			}
			obj.RegisterFunction(LoLa.Runtime.LoLaObject.TapFunctionName, this.Compile(obj, true));
			foreach (var fun in this.Functions)
				obj.RegisterFunction(fun.Name, fun.Compile(obj, false));
		}

        /// <summary>
        /// Instantiate a new LolaObject out of this program.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		public T Instantiate<T>()
			where T : LoLa.Runtime.LoLaObject, new()
		{
			var obj = new T();
			InstantiateInto(obj, true);
			return obj;
		}

        /// <summary>
        /// Evaluate this program for the given LoLaObject. Useful for REPL programs.
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
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
