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
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.FlowAnalysis.Utilities;
using Mono.Collections.Generic;

namespace Mono.Cecil.FlowAnalysis.ControlFlow {

	/// <summary>
	/// </summary>
	class ControlFlowGraphBuilder {

		MethodBody _body;
		Dictionary<int, InstructionData> _instructionData;
		Dictionary<int, InstructionBlock> _blocks = new Dictionary<int, InstructionBlock> ();

		InstructionBlock [] RegisteredBlocks {
			get { return ToArray (new InstructionBlock [BlockCount]); }
		}

		int BlockCount {
			get { return _blocks.Count; }
		}

		internal ControlFlowGraphBuilder (MethodDefinition method)
		{
			_body = method.Body;
		}

		public ControlFlowGraph BuildGraph (bool computeInstructionData, bool simplifyGraph)
		{
			DelimitBlocks ();
			ConnectBlocks ();
			if (computeInstructionData)
				ComputeInstructionData ();
			if (simplifyGraph)
				SimplifyBlocks ();
			return new ControlFlowGraph (_body, RegisteredBlocks, _instructionData);
		}

		void DelimitBlocks ()
		{
			var instructions = _body.Instructions;

			MarkBlockStarts (instructions);
			MarkBlockEnds (instructions);
		}

		void MarkBlockStarts (Collection<Instruction> instructions)
		{
			// the first instruction starts a block
			MarkBlockStart (instructions[0]);

			foreach (var instruction in instructions)
			{
				if (!IsBlockDelimiter (instruction))
					continue;

				if (HasMultipleBranches (instruction)) {
					// each switch case first instruction starts a block
					foreach (Instruction target in GetBranchTargets (instruction))
						if (target != null)
							MarkBlockStart (target);
				} else {
					// the target of a branch starts a block
					Instruction target = GetBranchTarget (instruction);
					if (null != target) MarkBlockStart (target);
				}

				// the next instruction after a branch starts a block
				if (null != instruction.Next) MarkBlockStart (instruction.Next);
			}
		}

		void MarkBlockEnds (Collection<Instruction> instructions)
		{
			InstructionBlock [] blocks = this.RegisteredBlocks;
			InstructionBlock current = blocks [0];

			for (int i = 1; i < blocks.Length; ++i) {
				InstructionBlock block = blocks [i];
				current.LastInstruction = block.FirstInstruction.Previous;
				current = block;
			}

			current.LastInstruction = instructions [instructions.Count - 1];
		}

		static bool IsBlockDelimiter (Instruction instruction)
		{
			FlowControl control = instruction.OpCode.FlowControl;
			switch (control) {
			case FlowControl.Break:
			case FlowControl.Branch:
			case FlowControl.Return:
			case FlowControl.Cond_Branch:
			case FlowControl.Throw:
				return true;
			}
			return false;
		}

		void MarkBlockStart (Instruction instruction)
		{
			InstructionBlock block = GetBlock (instruction);
			if (null != block) return;

			block = new InstructionBlock (instruction);
			RegisterBlock (block);
		}

		void ComputeInstructionData ()
		{
			_instructionData = new Dictionary<int, InstructionData> ();
			var visited = new HashSet<InstructionBlock> ();
			ComputeInstructionData (visited, 0, GetFirstBlock ());
		}

		InstructionBlock GetFirstBlock ()
		{
			return GetBlock (_body.Instructions [0]);
		}

		void ComputeInstructionData (HashSet<InstructionBlock> visited, int stackHeight, InstructionBlock block)
		{
			if (visited.Contains (block)) return;
			visited.Add (block);

			foreach (Instruction instruction in block) {
				stackHeight = ComputeInstructionData (stackHeight, instruction);
			}

			foreach (InstructionBlock successor in block.Successors) {
				ComputeInstructionData (visited, stackHeight, successor);
			}
		}

		int ComputeInstructionData (int stackHeight, Instruction instruction)
		{
			int before = stackHeight;
			int after = ComputeNewStackHeight (stackHeight, instruction);
			_instructionData.Add (instruction.Offset, new InstructionData (before, after));
			return after;
		}

		int ComputeNewStackHeight (int stackHeight, Instruction instruction)
		{
			return stackHeight + GetPushDelta (instruction) - GetPopDelta (stackHeight, instruction);
		}

		static int GetPushDelta (Instruction instruction)
		{
			OpCode code = instruction.OpCode;
			switch (code.StackBehaviourPush) {
			case StackBehaviour.Push0:
				return 0;

			case StackBehaviour.Push1:
			case StackBehaviour.Pushi:
			case StackBehaviour.Pushi8:
			case StackBehaviour.Pushr4:
			case StackBehaviour.Pushr8:
			case StackBehaviour.Pushref:
				return 1;

			case StackBehaviour.Push1_push1:
				return 2;

			case StackBehaviour.Varpush:
				if (code.FlowControl == FlowControl.Call) {
					MethodReference method = (MethodReference)instruction.Operand;
					return IsVoid (method.ReturnType) ? 0 : 1;
				}

				break;
			}
			throw new ArgumentException (Formatter.FormatInstruction (instruction));
		}

		int GetPopDelta (int stackHeight, Instruction instruction)
		{
			OpCode code = instruction.OpCode;
			switch (code.StackBehaviourPop) {
			case StackBehaviour.Pop0:
				return 0;
			case StackBehaviour.Popi:
			case StackBehaviour.Popref:
			case StackBehaviour.Pop1:
				return 1;

			case StackBehaviour.Pop1_pop1:
			case StackBehaviour.Popi_pop1:
			case StackBehaviour.Popi_popi:
			case StackBehaviour.Popi_popi8:
			case StackBehaviour.Popi_popr4:
			case StackBehaviour.Popi_popr8:
			case StackBehaviour.Popref_pop1:
			case StackBehaviour.Popref_popi:
				return 2;

			case StackBehaviour.Popi_popi_popi:
			case StackBehaviour.Popref_popi_popi:
			case StackBehaviour.Popref_popi_popi8:
			case StackBehaviour.Popref_popi_popr4:
			case StackBehaviour.Popref_popi_popr8:
			case StackBehaviour.Popref_popi_popref:
				return 3;

			case StackBehaviour.PopAll:
				return stackHeight;

			case StackBehaviour.Varpop:
				if (code.FlowControl == FlowControl.Call) {
					MethodReference method = (MethodReference)instruction.Operand;
					int count = method.Parameters.Count;
					if (method.HasThis && OpCodes.Newobj.Value != code.Value)
						++count;

					return count;
				}

				if (code.Value == OpCodes.Ret.Value)
					return IsVoidMethod () ? 0 : 1;

				break;
			}
			throw new ArgumentException (Formatter.FormatInstruction (instruction));
		}

		bool IsVoidMethod ()
		{
			return IsVoid (_body.Method.ReturnType);
		}

		static bool IsVoid (TypeReference type)
		{
			return type.MetadataType == MetadataType.Void;
		}

		InstructionBlock [] ToArray (InstructionBlock [] blocks)
		{
			_blocks.Values.CopyTo (blocks, 0);
			Array.Sort (blocks);
			return blocks;
		}

		void ConnectBlocks ()
		{
			foreach (InstructionBlock block in _blocks.Values)
				ConnectBlock (block);
		}

		void ConnectBlock (InstructionBlock block)
		{
			if (block.LastInstruction == null)
				throw new ArgumentException ("Undelimited block at offset " + block.FirstInstruction.Offset);

			Instruction instruction = block.LastInstruction;
			switch (instruction.OpCode.FlowControl) {
			case FlowControl.Branch:
			case FlowControl.Cond_Branch: {
				if (HasMultipleBranches (instruction))
				{
					AddGraphEdge (block, GetBranchTargetsBlocks (instruction));
					if (instruction.Next != null)
						AddGraphEdge (block, GetBlock (instruction.Next));
					break;
				}

				AddGraphEdge (block, GetBranchTargetBlock (instruction));
				if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch && instruction.Next != null)
					AddGraphEdge (block, GetBlock (instruction.Next));
				break;
			}
			case FlowControl.Call:
			case FlowControl.Next:
				if (null != instruction.Next)
					AddGraphEdge (block, GetBlock (instruction.Next));
				break;
			case FlowControl.Return:
			case FlowControl.Throw:
				break;
			default:
				throw new ArgumentException (
					string.Format ("Unhandled instruction flow behavior {0}: {1}",
					               instruction.OpCode.FlowControl,
					               Formatter.FormatInstruction (instruction)));
			}
		}

		void SimplifyBlocks ()
		{
			if (_blocks.Count < 2)
				return;
			foreach (var pair in _blocks) {
				var b1 = pair.Value;
				var b2 = b1.Successors.FirstOrDefault ();
				// ReSharper disable once PossibleNullReferenceException
				while (b1.Successors.Count == 1 && b2.Predecessors.Count == 1 && b1.LastInstruction == b2.FirstInstruction) {
					_blocks.Remove (b2.FirstInstruction.Offset);
					b1.LastInstruction = b2.LastInstruction;
					b1.Successors = b2.Successors;
					b2 = b1.Successors.FirstOrDefault ();
				}
			}
		}

		static void AddGraphEdge (InstructionBlock source, InstructionBlock target)
		{
			source.Successors.Add (target);
			target.Predecessors.Add (source);
		}

		static void AddGraphEdge (InstructionBlock source, IEnumerable<InstructionBlock> targets)
		{
			foreach (var target in targets)
				AddGraphEdge (source, target);
		}

		static bool HasMultipleBranches (Instruction instruction)
		{
			return instruction.OpCode.Code == Code.Switch;
		}

		IEnumerable<InstructionBlock> GetBranchTargetsBlocks (Instruction instruction)
		{
			return GetBranchTargets (instruction).Select (GetBlock);
		}

		static Instruction [] GetBranchTargets (Instruction instruction)
		{
			return (Instruction []) instruction.Operand;
		}

		InstructionBlock GetBranchTargetBlock (Instruction instruction)
		{
			return GetBlock (GetBranchTarget (instruction));
		}

		static Instruction GetBranchTarget (Instruction instruction)
		{
			return instruction.Operand as Instruction;
		}

		void RegisterBlock (InstructionBlock block)
		{
			_blocks.Add (block.FirstInstruction.Offset, block);
		}

		InstructionBlock GetBlock (Instruction firstInstruction)
		{
			InstructionBlock block;
			_blocks.TryGetValue (firstInstruction.Offset, out block);
			return block;
		}
	}
}
