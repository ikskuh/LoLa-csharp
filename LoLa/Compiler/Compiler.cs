using LoLa.Runtime;
using System;
using System.IO;

namespace LoLa.Compiler
{
	public static class Compiler
	{
		public static AST.Program Compile(string source, string fileName = "source.lola")
        {
            var parser = new Parser(new Lexer(new StringReader(source), fileName));
            if (parser.Parse() == false)
                throw new InvalidOperationException("Failed to parse source code!");

            return parser.Result;
        }

        public static FunctionCall Evaluate(LoLa.Runtime.LoLaObject environment, string source)
        {
            var pgm = Compile(source);
            return pgm.Evaluate(environment);
        }
	}
}
