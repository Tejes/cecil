#region license
//
// (C) db4objects Inc. http://www.db4o.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;

using Mono.Collections.Generic;

namespace Mono.Cecil.FlowAnalysis.ControlFlow {

	public class InstructionBlock : IComparable, IEnumerable<Instruction> {
		Instruction _firstInstruction;
		Instruction _lastInstruction;
		IList<InstructionBlock> _successors = new UniqueList<InstructionBlock> ();
		IList<InstructionBlock> _predecessors = new UniqueList<InstructionBlock> ();

		public Instruction FirstInstruction {
			get { return _firstInstruction; }
		}

		public Instruction LastInstruction {
			get { return _lastInstruction; }
			internal set {
				_lastInstruction = value ?? throw new ArgumentNullException ("last");
			}
		}

		public IList<InstructionBlock> Successors {
			get { return _successors; }
			internal set { _successors = value; }
		}

		public IList<InstructionBlock> Predecessors
		{
			get { return _predecessors; }
		}

		internal InstructionBlock (Instruction first)
		{
			if (null == first) throw new ArgumentNullException ("first");
			_firstInstruction = first;
		}

		public int CompareTo (object obj)
		{
			return _firstInstruction.Offset.CompareTo (((InstructionBlock)obj).FirstInstruction.Offset);
		}

		public IEnumerator<Instruction> GetEnumerator ()
		{
			Instruction instruction = _firstInstruction;
			while (true) {
				yield return instruction;
				if (instruction == _lastInstruction)
					break;
				instruction = instruction.Next;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
