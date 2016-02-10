using System;
using NUnit.Framework;

using Bareplan.Core;

namespace bareplantest {

	[TestFixture]
	public class DocumentTester {

		[Test]
		public static void DocumentAddTasksTest()
		{
			const int Max = 3;
			var doc = new Document();

			for (int i = 0; i < Max; ++i) {
				doc.AddLast();
			} 

			Assert.AreEqual( Max, doc.CountDates );
			Assert.AreEqual( Max, doc.CountTasks );
		}

		[Test]
		public static void DocumentRemoveTasksTest()
		{
			var doc = new Document();

			doc.Remove( 0 );
			doc.RemoveDate( 0 );
			doc.RemoveTask( 0 );
			Assert.AreEqual( 0, doc.CountDates );
		}
	}
}

