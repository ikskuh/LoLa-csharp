namespace LoLa.Compiler.AST
{
    public abstract class Statement
    {
        public abstract void Emit(CodeWriter writer);
    }
}