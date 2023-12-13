# Cecil.FlowAnalysis

These files are taken from
https://github.com/mono/cecil-old/tree/2d91d044480e7359948ac4b6d0575003228a4b39/flowanalysis/Cecil.FlowAnalysis

## Modifications

* Handle throws (add `FlowControl.Throw` to the switch in `ControlFlowGraphBuilder.ConnectBlock()`)
* Make graph edges bidirectional (every node has a Predecessors collection)
* Graph edges are unique, eliminating duplicates when there are fall-throughs in a switch, or a conditional branch jumps to the next instruction
* Reduce array copying and remove new collection allocation in GetEnumerator
* Make stack height computing optional
* Optional simplify, merging two adjacent blocks that do not connect to anywhere else (common in debug code to have an unconditional branch jumping to the next instruction)

## Room for improvement

* Take ExceptionHandlers into account

## Original license

	(C) db4objects Inc. http://www.db4o.com

	Permission is hereby granted, free of charge, to any person obtaining
	a copy of this software and associated documentation files (the
	"Software"), to deal in the Software without restriction, including
	without limitation the rights to use, copy, modify, merge, publish,
	distribute, sublicense, and/or sell copies of the Software, and to
	permit persons to whom the Software is furnished to do so, subject to
	the following conditions:

	The above copyright notice and this permission notice shall be
	included in all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
	NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
	LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
	OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
	WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
