using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LoLa.IL;
using LoLa.IL.Instructions;
using LoLa.Runtime;

namespace LoLa.Compiler
{
    public class CodeWriter : IReadOnlyList<Instruction>
    {
        private readonly List<Instruction> code = new List<Instruction>();

        public int Count => code.Count;

        public Instruction this[int index] => code[index];

        public CodeWriter()
        {

        }

        public void EnterScope() => Emit(new PushScope());
        public void LeaveScope() => Emit(new PopScope());

        public void DefineLabel(Label label)
        {
            label.Location = this.code.Count;
        }

        public void DeclareVariable(string variable)
        {
            Emit(new DeclareVariable { Variable = variable });
        }

        public void Jump(Label label)
        {
            var jump = new Jump
            {
                Target = label.Location
            };

            label.LocationChanged += (s, e) =>
            {
                jump.Target = label.Location;
            };

            Emit(jump);
        }

        public void JumpWhenFalse(Label label)
        {
            var jump = new JumpWhenFalse
            {
                Target = label.Location
            };

            label.LocationChanged += (s, e) =>
            {
                jump.Target = label.Location;
            };

            Emit(jump);
        }

        public void Emit(Instruction instr)
        {
            this.code.Add(instr);
        }

        public void Store(string var)
        {
            Emit(new StoreVariable { Variable = var });
        }

        public void Load(string var)
        {
            Emit(new LoadVariable { Variable = var });
        }

        public void Push(Value value)
        {
            Emit(new PushValue { Value = value });
        }

        public void ArrayPack(int size)
        {
            Emit(new ArrayPack { Size = size });
        }

        public void Call(string func, int argc)
        {
            Emit(new Call
            {
                Function = func,
                ArgumentCount = argc,
            });
        }

        public void CallObject(string func, int argc)
        {
            Emit(new CallObject
            {
                Function = func,
                ArgumentCount = argc,
            });
        }

        public void Pop()
        {
            Emit(new Pop());
        }

        public void Add()
        {
            Emit(new Add());
        }

        public void Subtract()
        {
            Emit(new Subtract());
        }

        public void Multiply()
        {
            Emit(new Multiply());
        }

        public void Divide()
        {
            Emit(new Divide());
        }

        public void Modulo()
        {
            Emit(new Modoluo());
        }

        public void Negate()
        {
            Emit(new Negate());
        }

        public void And()
        {
            Emit(new And());
        }

        public void Or()
        {
            Emit(new Or());
        }
        
        public void Invert()
        {
            Emit(new Invert());
        }
        
        public void Equals()
        {
            Emit(new Equals());
        }

        public void Differs()
        {
            Emit(new Differs());
        }

        public void LessThan()
        {
            Emit(new LessThan());
        }

        public void LessOrEqual()
        {
            Emit(new LessOrEqual());
        }
        
        public void MoreThan()
        {
            Emit(new MoreThan());
        }

        public void MoreOrEqual()
        {
            Emit(new MoreOrEqual());
        }

        public void MakeIterator()
        {
            Emit(new MakeIterator());
        }
        public void IteratorNext()
        {
            Emit(new IteratorNext());
        }
        public void ArrayStore()
        {
            Emit(new ArrayStore());
        }
        public void ArrayLoad()
        {
            Emit(new ArrayLoad());
        }
        public void Return()
        {
            Emit(new Return());
        }

        public IEnumerator<Instruction> GetEnumerator()
        {
            return ((IReadOnlyList<Instruction>)code).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyList<Instruction>)code).GetEnumerator();
        }
    }
}
