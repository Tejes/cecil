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

using Mono.Cecil.FlowAnalysis.ActionFlow;
using Mono.Cecil.Cil;
using Mono.Cecil.FlowAnalysis.CodeStructure;

namespace Mono.Cecil.FlowAnalysis.ActionFlow {

	public class InvokeActionBlock : AbstractFallThroughActionBlock {

		MethodInvocationExpression _expression;

		public override ActionType ActionType {
			get {
				return ActionType.Invoke;
			}
		}

		public MethodInvocationExpression Expression {
			get {
				return _expression;
			}
		}

		public InvokeActionBlock (Instruction sourceInstruction, MethodInvocationExpression expression)
			: base (sourceInstruction)
		{
			_expression = expression;
		}
	}
}
