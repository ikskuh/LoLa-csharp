namespace LoLa.Runtime
{
    public sealed class Variable
    {
        // TODO: private readonly Type type;

        public Variable(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public Value Value { get; set; }

        public override string ToString() => $"{Name} = {Value}";
    }
}