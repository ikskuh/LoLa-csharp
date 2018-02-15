%start program

%parsertype Parser
%tokentype LoLaTokenType
%visibility public

%output="Parser.cs"

%YYSTYPE AstNode

// Brackets
%token <Token> CURLY_O, CURLY_C, ROUND_O, ROUND_C, SQUARE_O, SQUARE_C

// Keywords
%token <String> VAR, FOR, WHILE, IF, ELSE, FUNCTION
%token <String> BREAK, CONTINUE, RETURN, IN

// Operators
%left <String> LEQUAL, GEQUAL, EQUALS, DIFFERS, LESS, MORE
%left <String> IS, DOT, COMMA, TERMINATOR
%left <String> PLUS, MINUS, MULT, DIV, MOD, AND, OR, INVERT
%token <String> IDENT

// Literals
%token <String> NUMBER, STRING

// lexer ignored tokens:
%token Comment,	LongComment, Whitespace

%namespace LoLa.Compiler

%type <Program> program
%type <Statement> statement body decl ass for while conditional expression return
%type <Function> function
%type <Statements> stmtlist
%type <Expression> expr_0 expr_1 expr_2 expr_3 expr_4
%type <NameList> plist
%type <RValue> rvalue call array
%type <LValue> lvalue
%type <String> expr_0_op expr_1_op expr_2_op expr_3_op
%type <Expressions> arglist 

%using LoLa.Compiler.AST;

%%

program		: /* empty */								{ $$ = new Program(); }
			| program function 							{ $$ = $1; $$.Functions.Add($2); }
			| program statement 						{ $$ = $1; $$.Statements.Add($2); }
			;

function	: FUNCTION IDENT ROUND_O plist ROUND_C body 
			{
				$$ = new AST.Function();
				$$.Name = $2;
				$$.Parameters = $4;
				$$.Body = $6;
			}
			| FUNCTION IDENT ROUND_O ROUND_C body 
			{
				$$ = new AST.Function();
				$$.Name = $2;
				$$.Parameters = new List<string>();
				$$.Body = $5;
			}
			;

plist       : IDENT
			{ 
				$$ = new List<string>(); 
				$$.Add($1);
			}
			| plist COMMA IDENT
			{
				$$ = $1;
				$$.Add($3);
			}
			;

body		: CURLY_O stmtlist CURLY_C					{ $$ = new SubScope($2); }
			;

stmtlist	: /* */										{ $$ = new List<AST.Statement>();  }
			| stmtlist statement						{ $$ = $1; $$.Add($2); }
			;

statement	: decl | ass | for | while | conditional | expression | return;

decl		: VAR IDENT IS expr_0 TERMINATOR			{ $$ = new Declaration($2, $4); }
			| VAR IDENT TERMINATOR						{ $$ = new Declaration($2); }
			;

ass			: lvalue IS expr_0 TERMINATOR				{ $$ = new Assignment($1, $3); }
			;

for			: FOR ROUND_O IDENT IN expr_0 ROUND_C body	{ $$ = new ForLoop($3,$5,$7); }
			;

while		: WHILE ROUND_O expr_0 ROUND_C body			{ $$ = new WhileLoop($3, $5); }
			;

return		: RETURN expr_0 TERMINATOR					{ $$ = new Return($2); }
			| RETURN TERMINATOR							{ $$ = new Return(); }
			;

conditional : IF ROUND_O expr_0 ROUND_C body ELSE body	{ $$ = new IfElse($3, $5, $7); }
			| IF ROUND_O expr_0 ROUND_C body			{ $$ = new IfElse($3, $5); }
			;

expression	: call TERMINATOR							{ $$ = new DiscardResult($1); }
			;
			
expr_0_op	: EQUALS|DIFFERS|LEQUAL|GEQUAL|MORE|LESS;
expr_0		: expr_0 expr_0_op expr_0					{ $$ = new BinaryOperator($2, $1, $3); }
			| expr_1									{ $$ = $1; }
			;


expr_1_op	: PLUS | MINUS ;
expr_1		: expr_1 expr_1_op expr_1					{ $$ = new BinaryOperator($2, $1, $3); }
			| expr_2									{ $$ = $1; }
			;

		
expr_2_op	: MULT | DIV | MOD | AND | OR;	
expr_2		: expr_2 expr_2_op expr_2					{ $$ = new BinaryOperator($2, $1, $3); }
			| expr_3									{ $$ = $1; }
			;


expr_3_op	: MINUS | INVERT;
expr_3		: expr_3_op expr_3							{ $$ = new UnaryOperator($1, $2); }
			| expr_4									{ $$ = $1; }
			;


expr_4		: ROUND_O expr_0 ROUND_C					{ $$ = $2; }
			| rvalue									{ $$ = $1; }
			| lvalue									{ $$ = $1; }
			;

rvalue		: call										{ $$ = $1; }
			| array										{ $$ = $1; }
			| STRING									{ $$ = new AST.Literal($1); }
			| NUMBER									{ $$ = new AST.Literal(Convert.ToDouble($1, CultureInfo.InvariantCulture)); }
			;

call		: IDENT DOT IDENT ROUND_O ROUND_C			{ $$ = new FunctionCall($1, $3); }
			| IDENT DOT IDENT ROUND_O arglist ROUND_C	{ $$ = new FunctionCall($1, $3, $5); }
			| IDENT ROUND_O ROUND_C						{ $$ = new FunctionCall($1); }
			| IDENT ROUND_O arglist ROUND_C				{ $$ = new FunctionCall($1, $3); }
			;

array		: SQUARE_O SQUARE_C							{ $$ = new ArrayLiteral(); }
			| SQUARE_O arglist SQUARE_C					{ $$ = new ArrayLiteral($2); }
			;

arglist		: arglist COMMA expr_0						{ $$ = $1; $$.Add($3); }
			| expr_0									{ $$ = new List<AST.Expression>(); $$.Add($1); }
			;

lvalue		: IDENT SQUARE_O expr_0 SQUARE_C			{ $$ = new ArrayIndexer($1, $3); }
			| IDENT										{ $$ = new AST.VariableRef($1); }
			;

%%

public Parser(Lexer lexer) : base(lexer) 
{ 
	
}

public Program Result => this.CurrentSemanticValue.Program;
