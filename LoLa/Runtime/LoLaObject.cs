using LoLa.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using LoLa.IL;

namespace LoLa.Runtime
{
    public class LoLaObject : Scope, IObject
    {
        public const string TapFunctionName = "$main";

        private Dictionary<string, Function> functions = new Dictionary<string, Function>();

        private static readonly Scope ultraGlobals;

        static LoLaObject()
        {
            ultraGlobals = new Scope();
            ultraGlobals.DeclareVariable("true", true);
            ultraGlobals.DeclareVariable("false", false);
        }

        public LoLaObject() : base(ultraGlobals)
        {
            this.CreateDefaultLibrary();
        }

        internal LoLaObject(LoLaObject env) :
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

        public bool RemoveFunction(string name) => this.functions.Remove(name);

        public void RegisterFunction(string name, NativeFunctionDelegate fun) => RegisterFunction(name, new NativeFunction(fun));

        public void RegisterFunction(string name, int argc, NativeFunctionDelegate fun) => RegisterFunction(name, new NativeFunction(fun, argc));

        public void RegisterAsyncFunction(string name, AsyncNativeFunctionDelegate fun) => RegisterFunction(name, new AsyncNativeFunction(fun));

        public void RegisterAsyncFunction(string name, int argc, AsyncNativeFunctionDelegate fun) => RegisterFunction(name, new AsyncNativeFunction(fun, argc));

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
        public FunctionCall Tap()
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
                if (arglist[0].Type == Type.String)
                    return arglist[0].ToString().Length;
                if (arglist[0].Type == Type.String)
                    return arglist[0].ToArray().Count;
                throw new LoLaException("Length() expects either an array or a string!");
            }, 1));

            this.RegisterFunction("InStr", new NativeFunction(arglist =>
            {
                var str = arglist[0].ToString();
                var sub = arglist[1].ToString();
                return str.IndexOf(sub, StringComparison.Ordinal);
            }, 3));

            this.RegisterFunction("SubStr", new NativeFunction(arglist =>
            {
                var str = arglist[0].ToString();
                var idx = arglist[1].ToInteger();
                var len = (arglist[2].Type == Type.Number) ? arglist[2].ToInteger() : (str.Length - idx);
                return str.Substring(idx, len);
            }, 3));

            this.RegisterFunction("Chr", new NativeFunction(arglist =>
            {
                var cp = arglist[0].ToInteger();
                return char.ConvertFromUtf32(cp);
            }, 1));

            this.RegisterFunction("Asc", new NativeFunction(arglist =>
            {
                var txt = arglist[0].ToString();
                return char.ConvertToUtf32(txt, 0);
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

#if NETFX_35
        public IDictionary<string, Function> Functions => this.functions;
#else
        public IReadOnlyDictionary<string, Function> Functions => this.functions;
#endif
    }
}
