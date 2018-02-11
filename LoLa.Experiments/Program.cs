using System;
using System.Collections.Generic;
using System.IO;
using LoLa.Compiler;
using LoLa.IL;

namespace LoLa.Experiments
{
    class MainClass
    {
        private const string source =
            @"
                var counter = CreateCounter();
                Print(""cnt = "", counter.GetValue());
                Print(""cnt = "", counter.Increment());
                Print(""cnt = "", counter.Increment());
                Print(""cnt = "", counter.Decrement());
            ";

        public static void Main(string[] args)
        {
            var parser = new Parser(new Lexer(new StringReader(source), "source.lola"));

            if (parser.Parse() == false)
                throw new InvalidOperationException();

            var pgm = parser.Result;

            var env = pgm.Instantiate<Object>();

            env.RegisterFunction("Print", new NativeFunction(arglist =>
            {
                Console.WriteLine(string.Join("", arglist));
                return Value.Null;
            }));

            env.RegisterFunction("CreateCounter", new NativeFunction(arglist =>
            {
                var counter = 0;
                var obj = new Object();
                obj.RegisterFunction("GetValue", new NativeFunction(a => counter));
                obj.RegisterFunction("Increment", new NativeFunction(a => ++counter));
                obj.RegisterFunction("Decrement", new NativeFunction(a => --counter));
                return obj;
            }));

            env.Tap();

            Console.WriteLine("Success!");

            Console.ReadLine();
        }
    }
}
