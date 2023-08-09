using System.Collections;
using System.Collections.Generic;

namespace Mono.Collections.Generic {

	/// <summary>
	/// A List that contains every item only once. Suitable for small datasets, because every insertion is O(n).
	/// Does not use a Set internally, so memory usage is the same as a System.Collections.Generic.List&lt;T&gt;.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list</typeparam>
	public class UniqueList<T> : IList<T> {

		private readonly List<T> list;

		public UniqueList ()
		{
			list = new List<T> ();
		}

		public UniqueList (IEnumerable<T> enumerable)
		{
			if (enumerable is ICollection<T> collection) {
				list = new List<T> (collection.Count);
			} else {
				list = new List<T> ();
			}

			AddRange (enumerable);
		}

		public UniqueList (int capacity)
		{
			list = new List<T> (capacity);
		}

		public int Count => list.Count;

		public bool IsReadOnly => false;

		public T this [int index] {
			get => list [index];
			set {
				if (!Contains (value))
					list [index] = value;
			}
		}

		public void Add (T item)
		{
			if (!Contains (item))
				list.Add (item);
		}

		public void AddRange (IEnumerable<T> enumerable)
		{
			foreach (var item in enumerable) {
				Add (item);
			}
		}

		public void Insert (int index, T item)
		{
			if (!Contains (item))
				list.Insert (index, item);
		}

		public void Clear ()
		{
			list.Clear ();
		}

		public bool Remove (T item)
		{
			return list.Remove (item);
		}

		public void RemoveAt (int index)
		{
			list.RemoveAt (index);
		}

		public bool Contains (T item)
		{
			return list.Contains (item);
		}

		public int IndexOf (T item)
		{
			return list.IndexOf (item);
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return list.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return list.GetEnumerator ();
		}

		public void CopyTo (T [] array, int arrayIndex)
		{
			list.CopyTo (array, arrayIndex);
		}
	}
}


