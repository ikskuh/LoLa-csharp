using LoLa.Compiler.AST;
using System;
using System.IO;

namespace LoLa.Compiler
{
	public static class Compiler
	{
		public static Program Compile(string source, string fileName = "source.lola")
        {
            var parser = new Parser(new Lexer(new StringReader(source), fileName));

            if (parser.Parse() == false)
                throw new InvalidOperationException();

            return parser.Result;
        }

        public static Value Evaluate(Object environment, string source)
        {
            var pgm = Compile(source);
            return pgm.Evaluate(environment);
        }
	}
}
