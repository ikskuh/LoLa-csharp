using System;
using System.Collections.Generic;
namespace LoLa.Runtime
{
	public struct Value : IEquatable<Value>
	{
		public static Value Null { get; } = new Value();
		
		private readonly object value;
		private readonly Type type;

		public Value(string text)
		{
			if(text == null)
				throw new ArgumentNullException(nameof(text));
			this.value = text;
			this.type = Type.String;
		}
		
		public Value(bool boolean)
		{
			this.value = boolean;
			this.type = Type.Boolean;
		}

		public Value(double number)
		{
			this.value = number;
			this.type = Type.Number;
		}

		public Value(System.Object @object)
		{
			if(@object == null)
				throw new ArgumentNullException(nameof(@object));
			this.value = @object;
			this.type = Type.Object;
		}

		public Value(Array array)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));
			this.value = array;
			this.type = Type.Array;
		}

		public Value(IEnumerator<Value> enumerator)
		{
			if(enumerator == null)
				throw new ArgumentNullException(nameof(enumerator));
			this.value = enumerator;
			this.type = Type.Enumerator;
		}
		
		public override int GetHashCode() => this.value?.GetHashCode() ?? 0;
		
		public object Raw => this.value;
		
		public Type Type => this.type;
		
		public LoLaObject ToObject() => (LoLaObject)value;

		public Array ToArray() => (Array)value;

		public bool ToBoolean() => !((value == null) || object.Equals(value, false));

		public double ToNumber() => (double)value;
		
		public int ToInteger() => (int)this.ToNumber();
		
		public IEnumerator<Value> ToEnumerator() => (IEnumerator<Value>)value;

		public override string ToString() => value?.ToString() ?? "null";

		public override bool Equals(object obj) => (obj is Value) ?  Equals((Value)obj) : false;

		public bool Equals(Value other)
		{
			return (this.type == other.type)
				&& object.Equals(this.value, other.value);
		}

		public static implicit operator Value(bool val) => new Value(val);
		public static implicit operator Value(string val) => new Value(val);
		public static implicit operator Value(double val) => new Value(val);
		public static implicit operator Value(Array val) => new Value(val);
		public static implicit operator Value(LoLaObject val) => new Value(val);
		
		public static explicit operator bool(Value val) => val.ToBoolean();
		public static explicit operator string(Value val) => val.ToString();
		public static explicit operator double(Value val) => val.ToNumber();
		public static explicit operator Array(Value val) => val.ToArray();
		public static explicit operator LoLaObject(Value val) => val.ToObject();

        public static bool operator ==(Value lhs, Value rhs) => lhs.Equals(rhs);
        public static bool operator !=(Value lhs, Value rhs) => !lhs.Equals(rhs);
    }
}