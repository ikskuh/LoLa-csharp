using System;
using QUT.Gppg;
using System.IO;

namespace LoLa.Compiler
{
	public sealed class Lexer: AbstractScanner<AstNode, LexLocation>, IDisposable
	{
		private readonly LoLaTokenizer tokenizer;

        LexLocation currentLocation;

        public override LexLocation yylloc {
            get { return currentLocation; }
            set { throw new NotSupportedException(); }
        }

        public Lexer(System.IO.TextReader reader, string fileName)
		{
			this.tokenizer = new LoLaTokenizer(reader, fileName);
		}

		public Lexer(string fileName)
		{
			this.tokenizer = new LoLaTokenizer(new StreamReader(fileName), fileName, true);
		}

		~Lexer()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.tokenizer.Dispose();
		}

		public override int yylex()
		{
			if (this.tokenizer.EndOfText)
				return (int)LoLaTokenType.EOF;

			var token = this.tokenizer.Read();

            var loc = token.Location;

            currentLocation = new LexLocation(
                loc.Line, loc.Column,
                loc.Line, loc.Column);
                
			this.yylval = new AstNode() { String = token.Text };

			return (int)token.Type;
		}

		public override void yyerror(string format, params object[] args)
		{
			Console.Error.WriteLine(format, args);
		}
	}
}
