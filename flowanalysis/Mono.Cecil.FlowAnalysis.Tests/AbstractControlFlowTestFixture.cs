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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil.FlowAnalysis.Utilities;
using Mono.Cecil.FlowAnalysis.ControlFlow;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Mono.Cecil.FlowAnalysis.Tests {

	public class AbstractControlFlowTestFixture : AbstractFlowAnalysisTestFixture {

		protected void RunTestCase (string name)
		{
			MethodDefinition method = LoadTestCaseMethod (name);
			ControlFlowGraph cfg = FlowGraphFactory.CreateControlFlowGraph (method);
			Assert.AreEqual (Normalize (LoadExpectedControlFlowString (name)), Normalize (ToString (cfg)));
		}

		public static string ToString (ControlFlowGraph cfg)
		{
			StringWriter writer = new StringWriter ();
			FormatControlFlowGraph (writer, cfg);
			return writer.ToString ();
		}

		public static void FormatControlFlowGraph (TextWriter writer, ControlFlowGraph cfg)
		{
			int id = 1;
			foreach (InstructionBlock block in cfg.Blocks) {
				writer.WriteLine ("block {0}:", id);
				writer.WriteLine ("\tbody:");
				foreach (Instruction instruction in block) {
					writer.Write ("\t\t");
					Formatter.WriteInstruction (writer, instruction);
					writer.WriteLine ();
				}
				IList<InstructionBlock> successors = block.Successors;
				if (successors.Count > 0) {
					writer.WriteLine ("\tsuccessors:");
					foreach (InstructionBlock successor in successors) {
						writer.WriteLine ("\t\tblock {0}", GetBlockId (cfg, successor));
					}
				}

				++id;
			}
		}

		private static int GetBlockId (ControlFlowGraph cfg, InstructionBlock block)
		{
			return ((IList) cfg.Blocks).IndexOf (block) + 1;
		}

		private string LoadExpectedControlFlowString (string name)
		{
			return LoadTestCaseFile (name + "-cfg.txt");
		}
	}
}
