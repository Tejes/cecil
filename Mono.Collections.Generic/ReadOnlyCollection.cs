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
using System.Threading;

namespace Mono.Collections.Generic {

	public sealed class ReadOnlyCollection<T> : Collection<T>, ICollection<T> {

		static ReadOnlyCollection<T> empty;

		public static ReadOnlyCollection<T> Empty {
			get {
				if (empty != null)
					return empty;

				Interlocked.CompareExchange (ref empty, new ReadOnlyCollection<T> (), null);
				return empty;
			}
		}

		bool ICollection<T>.IsReadOnly {
			get { return true; }
		}

		ReadOnlyCollection ()
		{
		}

		public ReadOnlyCollection (T [] array) : base(array)
		{
		}

		public ReadOnlyCollection (Collection<T> collection) : base(collection)
		{
		}

		protected override void OnAdd (T item, int index)
		{
			throw new InvalidOperationException ();
		}

		protected override void OnClear ()
		{
			throw new InvalidOperationException ();
		}

		protected override void OnInsert (T item, int index)
		{
			throw new InvalidOperationException ();
		}

		protected override void OnInsertRange (IList<T> items, int index)
		{
			throw new InvalidOperationException ();
		}

		protected override void OnRemove (T item, int index)
		{
			throw new InvalidOperationException ();
		}

		protected override void OnSet (T item, int index)
		{
			throw new InvalidOperationException ();
		}
	}
}
