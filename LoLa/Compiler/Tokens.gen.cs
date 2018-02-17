









using System;
using System.IO;
using System.Text.RegularExpressions;
using CompilerKit;

namespace LoLa.Compiler
{
	
	







sealed partial class LoLaTokenizer : Tokenizer<LoLaTokenType>
{
	public LoLaTokenizer(TextReader reader, string fileName) : this(reader, fileName, true)
	{
	
	}

	public LoLaTokenizer(TextReader reader, string fileName, bool closeOnDispose) : base(reader, fileName, closeOnDispose)
	{

		this.RegisterToken(LoLaTokenType.Comment, new Regex(@"\/\/.*", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.LongComment, new Regex(@"\/\*.*?\*\/", RegexOptions.Compiled|RegexOptions.Singleline));

		this.RegisterToken(LoLaTokenType.Whitespace, new Regex(@"\s+", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.CURLY_O, new Regex(@"\{", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.CURLY_C, new Regex(@"\}", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.ROUND_O, new Regex(@"\(", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.ROUND_C, new Regex(@"\)", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.SQUARE_O, new Regex(@"\[", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.SQUARE_C, new Regex(@"\]", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.VAR, new Regex(@"\bvar\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.FOR, new Regex(@"\bfor\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.WHILE, new Regex(@"\bwhile\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.IF, new Regex(@"\bif\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.ELSE, new Regex(@"\belse\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.FUNCTION, new Regex(@"\bfunction\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.IN, new Regex(@"\bin\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.BREAK, new Regex(@"\bbreak\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.CONTINUE, new Regex(@"\bcontinue\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.RETURN, new Regex(@"\breturn\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.LEQUAL, new Regex(@"\<\=", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.GEQUAL, new Regex(@"\>\=", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.EQUALS, new Regex(@"\=\=", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.DIFFERS, new Regex(@"\!\=", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.LESS, new Regex(@"\<", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.MORE, new Regex(@"\>", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.IS, new Regex(@"\=", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.DOT, new Regex(@"\.", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.COMMA, new Regex(@"\,", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.TERMINATOR, new Regex(@"\;", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.PLUS, new Regex(@"\+", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.MINUS, new Regex(@"\-", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.MULT, new Regex(@"\*", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.MOD, new Regex(@"\%", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.DIV, new Regex(@"\/", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.AND, new Regex(@"\band\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.OR, new Regex(@"\bor\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.INVERT, new Regex(@"\bnot\b", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.NUMBER, new Regex(@"\d+(\.\d+)?", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.STRING, new Regex(@"""(?:\\""|.)*?""", RegexOptions.Compiled));

		this.RegisterToken(LoLaTokenType.IDENT, new Regex(@"\w\w*", RegexOptions.Compiled));

			
		this.Initialize();
	}

	partial void Initialize();

	protected override Func<string,string> GetPostProcessor(LoLaTokenType type)
	{
		switch(type)
		{

			case LoLaTokenType.Comment:
				return (text) => null;

			case LoLaTokenType.LongComment:
				return (text) => null;

			case LoLaTokenType.Whitespace:
				return (text) => null;

			case LoLaTokenType.STRING:
				return (text) => text.Substring(1, text.Length - 2);

			default:
				return (text) => text;
		}
	}
}

}
