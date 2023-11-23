//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mono.Collections.Generic {
	public class Collection<T> : IList<T> {

		private readonly List<T> items;

		public Collection ()
		{
			items = new List<T> ();
		}

		public Collection (int capacity)
		{
			items = new List<T> (capacity);
		}

		public Collection (IEnumerable<T> items)
		{
			this.items = new List<T> (items);
		}

		public void Add (T item)
		{
			OnAdd (item, Count);
			items.Add (item);
		}

		public void AddRange (IEnumerable<T> collection)
		{
			var as_list = collection as IList<T> ?? collection.ToList ();
			OnAddRange (as_list, Count);
			items.AddRange (as_list);
		}

		public void Clear ()
		{
			OnClear ();
			items.Clear ();
		}

		public bool Contains (T item)
		{
			return items.Contains (item);
		}

		public void CopyTo (T [] array, int arrayIndex)
		{
			items.CopyTo (array, arrayIndex);
		}

		public bool Remove (T item)
		{
			var index = items.IndexOf (item);
			if (index < 0)
				return false;
			OnRemove (item, index);
			items.RemoveAt (index);
			return true;
		}

		public int Count => items.Count;

		public bool IsReadOnly => ((IList<T>)items).IsReadOnly;

		public int IndexOf (T item)
		{
			return items.IndexOf (item);
		}

		public void Insert (int index, T item)
		{
			OnInsert (item, index);
			items.Insert (index, item);
		}

		public void InsertRange (int index, IEnumerable<T> items)
		{
			var as_list = items as IList<T> ?? items.ToList ();
			OnInsertRange (as_list, index);
			this.items.InsertRange (index, as_list);
		}

		public void RemoveAt (int index)
		{
			OnRemove (this [index], index);
			items.RemoveAt (index);
		}

		public T this [int index] {
			get => items [index];
			set {
				OnSet (value, index);
				items [index] = value;
			}
		}

		protected virtual void OnAdd (T item, int index)
		{
		}

		protected virtual void OnAddRange (IList<T> items, int index)
		{
			OnInsertRange (items, index);
		}

		protected virtual void OnInsert (T item, int index)
		{
		}

		protected virtual void OnInsertRange (IList<T> items, int index)
		{
			var i = index;
			foreach (var item in items) {
				OnInsert (item, i);
			}
		}

		protected virtual void OnSet (T item, int index)
		{
		}

		protected virtual void OnRemove (T item, int index)
		{
		}

		protected virtual void OnClear ()
		{
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return items.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((IEnumerable)items).GetEnumerator ();
		}

		public void Sort ()
		{
			items.Sort ();
		}

		public void Sort (IComparer<T> comparer)
		{
			items.Sort (comparer);
		}

		public void Sort (int index, int count, IComparer<T> comparer)
		{
			items.Sort (index, count, comparer);
		}

		public void Sort (Comparison<T> comparison)
		{
			items.Sort (comparison);
		}

		public int Capacity {
			get => items.Capacity;
			set => items.Capacity = value;
		}

		public T [] ToArray ()
		{
			return items.ToArray ();
		}
	}
}
