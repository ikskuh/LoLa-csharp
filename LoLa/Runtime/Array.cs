using System.Collections;
using System.Collections.Generic;

namespace LoLa
{
	public sealed class Array : IList<Value>
	{
		private readonly List<Value> items = new List<Value>();

        public override string ToString() => "[ " + string.Join(", ", this.items) + " ]";

        #region IList<Value>
        public Value this[int index]
		{
			get
			{
				return items[index];
			}

			set
			{
				items[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return items.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((IList<Value>)items).IsReadOnly;
			}
		}

		public void Add(Value item)
		{
			items.Add(item);
		}

		public void Clear()
		{
			items.Clear();
		}

		public bool Contains(Value item)
		{
			return items.Contains(item);
		}

		public void CopyTo(Value[] array, int arrayIndex)
		{
			items.CopyTo(array, arrayIndex);
		}

		public IEnumerator<Value> GetEnumerator()
		{
			return ((IList<Value>)items).GetEnumerator();
		}

		public int IndexOf(Value item)
		{
			return items.IndexOf(item);
		}

		public void Insert(int index, Value item)
		{
			items.Insert(index, item);
		}

		public bool Remove(Value item)
		{
			return items.Remove(item);
		}

		public void RemoveAt(int index)
		{
			items.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList<Value>)items).GetEnumerator();
		}
		
		#endregion
	}
}