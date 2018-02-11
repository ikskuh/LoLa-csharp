using System;
using System.Collections.Generic;
using CompilerKit;
namespace LoLa.Compiler
{
	public struct AstNode
	{
		public string String;
		
		public Token<LoLaTokenType> Token;

        public AST.Function   Function;
        public AST.Statement  Statement;
        public AST.Program    Program;
        public AST.Expression Expression;
        public AST.RValue     RValue;
        public AST.LValue     LValue;

        public List<AST.Statement> Statements;
        public List<AST.Expression> Expressions;

        public List<string> NameList;
    }
}
