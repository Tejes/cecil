using System.Collections.Generic;
using Mono.Collections.Generic;
using NUnit.Framework;

namespace Mono.Cecil.Tests {
	[TestFixture]
	public class CollectionTests : BaseTestFixture {

		[Test]
		public static void TestInsertRange ()
		{
			var instance = new Collection<int> { 0, 1, 2, 3, 4, 5 };
			instance.InsertRange (2, new [] { 6, 7, 8 });
			Assert.AreEqual (new Collection<int> { 0, 1, 6, 7, 8, 2, 3, 4, 5 }, instance);

			instance = new Collection<int> { 0, 1, 2, 3, 4, 5 };
			instance.InsertRange (0, new [] { 6, 7, 8 });
			Assert.AreEqual (new Collection<int> { 6, 7, 8, 0, 1, 2, 3, 4, 5 }, instance);

			instance = new Collection<int> { 0, 1, 2, 3, 4, 5 };
			instance.InsertRange (6, new [] { 6, 7, 8 });
			Assert.AreEqual (new Collection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, instance);
		}

		[Test]
		public static void TestNewMethods ()
		{
			TestCollection instance;

			instance = new TestCollection { 0, 1, 2, 3, 4 };
			AsInterface ().Add (10);
			Assert.True (instance.add_called);
			Assert.AreEqual (new TestCollection { 0, 1, 2, 3, 4, 10 }, instance);

			instance = new TestCollection { 0, 1, 2, 3, 4 };
			instance.AddRange (new [] { 6, 7, 8 });
			Assert.True (instance.add_range_called);
			Assert.AreEqual (new TestCollection { 0, 1, 2, 3, 4, 6, 7, 8 }, instance);

			instance = new TestCollection { 0, 1, 2, 3, 4 };
			AsInterface ().Clear ();
			Assert.True (instance.clear_called);
			Assert.AreEqual (new TestCollection (), instance);

			instance = new TestCollection { 0, 1, 2, 3, 4 };
			AsInterface ().Insert (1, 10);
			Assert.True (instance.insert_called);
			Assert.AreEqual (new TestCollection { 0, 10, 1, 2, 3, 4 }, instance);

			instance = new TestCollection { 0, 1, 2, 3, 4 };
			instance.InsertRange (0, new [] { 6, 7, 8 });
			Assert.True (instance.insert_range_called);
			Assert.AreEqual (new TestCollection { 6, 7, 8, 0, 1, 2, 3, 4 }, instance);

			instance = new TestCollection { 0, 1, 2, 3, 4 };
			AsInterface ().Remove (2);
			Assert.True (instance.remove_called);
			Assert.AreEqual (new TestCollection { 0, 1, 3, 4 }, instance);

			instance = new TestCollection { 0, 1, 2, 3, 4 };
			AsInterface () [2] = 12;
			Assert.True (instance.set_called);
			Assert.AreEqual (new TestCollection { 0, 1, 12, 3, 4 }, instance);

			IList<int> AsInterface () => instance;
		}

		private class TestCollection : Collection<int> {
			public bool add_called;
			public bool add_range_called;
			public bool clear_called;
			public bool insert_called;
			public bool insert_range_called;
			public bool remove_called;
			public bool set_called;

			protected override void OnAdd (int item, int index)
			{
				add_called = true;
			}

			protected override void OnAddRange (IList<int> items, int index)
			{
				add_range_called = true;
			}

			protected override void OnClear ()
			{
				clear_called = true;
			}

			protected override void OnInsert (int item, int index)
			{
				insert_called = true;
			}

			protected override void OnInsertRange (IList<int> items, int index)
			{
				insert_range_called = true;
			}

			protected override void OnRemove (int item, int index)
			{
				remove_called = true;
			}

			protected override void OnSet (int item, int index)
			{
				set_called = true;
			}
		}
	}
}
