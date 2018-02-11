using System;
using System.Collections.Generic;

namespace LoLa
{
    public class Object : Scope, IObject
    {
        public const string TapFunctionName = "$main";

        private Dictionary<string, Function> functions = new Dictionary<string, Function>();

        public Object()
        {
            this.CreateDefaultLibrary();
        }

        internal Object(Object env) : 
            base(env)
        {

        }

        /// <summary>
        /// Registers a new function for this object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fun"></param>
        public void RegisterFunction(string name, Function fun)
        {
            this.functions.Add(name, fun);
        }

        public void RegisterFunction(string name, NativeFunctionDelegate fun) => RegisterFunction(name, new NativeFunction(fun));

        public void RegisterFunction(string name, int argc, NativeFunctionDelegate fun) => RegisterFunction(name, new NativeFunction(fun, argc));

        /// <summary>
        /// Gets a function of this object.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Function GetFunction(string name)
        {
            if (this.functions.TryGetValue(name, out var fun))
                return fun;
            else
                throw new InvalidOperationException($"The function {name} does not exist!");
        }

        /// <summary>
        /// Executes the tap function of this object.
        /// </summary>
        /// <returns>Return value of the tap</returns>
        public Value Tap()
        {
            this.variables.Clear(); // Tapping clears all global variables so they can be created anew
            return this.functions[TapFunctionName].Call(new Value[0]);
        }

        private void CreateDefaultLibrary()
        {
            this.RegisterFunction("Range", new NativeFunction(arglist =>
            {
                var items = new Array();
                int min = (int)arglist[0].ToNumber();
                int count = (int)arglist[1].ToNumber();
                for (int i = 0; i < count; i++)
                    items.Add(min + i);
                return items;
            }, 2));

            this.RegisterFunction("Length", new NativeFunction(arglist =>
            {
                return arglist[0].ToArray().Count;
            }, 1));

            this.RegisterFunction("Sqrt", new NativeFunction(arglist =>
            {
                return Math.Sqrt(arglist[0].ToNumber());
            }, 1));

            this.RegisterFunction("Pow", new NativeFunction(arglist =>
            {
                return Math.Pow(arglist[0].ToNumber(), arglist[1].ToNumber());
            }, 2));

            this.RegisterFunction("Sin", new NativeFunction(arglist =>
            {
                return Math.Sin(arglist[0].ToNumber());
            }, 1));

            this.RegisterFunction("Cos", new NativeFunction(arglist =>
            {
                return Math.Cos(arglist[0].ToNumber());
            }, 1));

            this.RegisterFunction("Tan", new NativeFunction(arglist =>
            {
                return Math.Tan(arglist[0].ToNumber());
            }, 1));

            this.RegisterFunction("Abs", new NativeFunction(arglist =>
            {
                return Math.Abs(arglist[0].ToNumber());
            }, 1));

            this.RegisterFunction("Ceiling", new NativeFunction(arglist =>
            {
                return Math.Ceiling(arglist[0].ToNumber());
            }, 1));

            this.RegisterFunction("Floor", new NativeFunction(arglist =>
            {
                return Math.Floor(arglist[0].ToNumber());
            }, 1));

            this.RegisterFunction("Log", new NativeFunction(arglist =>
            {
                return Math.Log(arglist[0].ToNumber());
            }, 1));

            this.RegisterFunction("Max", new NativeFunction(arglist =>
            {
                return Math.Max(arglist[0].ToNumber(), arglist[1].ToNumber());
            }, 2));

            this.RegisterFunction("Min", new NativeFunction(arglist =>
            {
                return Math.Min(arglist[0].ToNumber(), arglist[1].ToNumber());
            }, 2));
        }

        public IReadOnlyDictionary<string, Function> Functions => this.functions;
    }
}
