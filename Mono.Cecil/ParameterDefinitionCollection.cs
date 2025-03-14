//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System.Collections.Generic;
using Mono.Collections.Generic;

namespace Mono.Cecil {

	sealed class ParameterDefinitionCollection : Collection<ParameterDefinition> {

		readonly IMethodSignature method;

		internal ParameterDefinitionCollection (IMethodSignature method)
		{
			this.method = method;
		}

		internal ParameterDefinitionCollection (IMethodSignature method, int capacity)
			: base (capacity)
		{
			this.method = method;
		}

		protected override void OnAdd (ParameterDefinition item, int index)
		{
			item.method = method;
			item.index = index;
		}

		protected override void OnInsert (ParameterDefinition item, int index)
		{
			item.method = method;
			item.index = index;

			for (int i = index; i < Count; i++)
				this [i].index = i + 1;
		}

		protected override void OnInsertRange (IList<ParameterDefinition> items, int index)
		{
			var i = index;
			foreach (var item in items) {
				item.method = method;
				item.index = i++;
			}

			for (i = index; i < Count; i++)
				this [i].index = i + 1;
		}

		protected override void OnSet (ParameterDefinition item, int index)
		{
			item.method = method;
			item.index = index;
		}

		protected override void OnRemove (ParameterDefinition item, int index)
		{
			item.method = null;
			item.index = -1;

			for (int i = index + 1; i < Count; i++)
				this [i].index = i - 1;
		}
	}
}
