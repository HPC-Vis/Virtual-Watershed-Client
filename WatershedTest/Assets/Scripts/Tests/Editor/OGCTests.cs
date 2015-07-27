using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace OGC_Tests
{
	[TestFixture]
	[Category("OGC Tests")]
	internal class OGCTests
	{
		[Test]
		public void TestTest()
		{
			Assert.That (1 == 1);
		}
		
		// The very first utm test :D.
		[Test, TestCaseSource("VectorCases")]
        public void UTMTransformationTest(Vector2 testvec,Vector2 Result)
        {
			var testvec2 = coordsystem.transformToUTM (testvec.x, testvec.y);
			double[] a = coordsystem.transformToUTMDouble (testvec.x, testvec.y);
			Assert.AreEqual (Result.x, a [0], 1);
			Assert.AreEqual (Result.y, a [1], 1);
        }
		static object[] VectorCases =
		{
			new object[] {new Vector2(-119.8152367f,39.5436008f), new Vector2(258081.4f,4380889f)},
			new object[] {new Vector2(-119.8152367f,39.5436008f-2), new Vector2(251279.8f,4158903.7f)},
			new object[] {new Vector2(-119.8152367f+2,39.5436008f),  new Vector2(429950.1f,4377420.7f)},
			new object[] {new Vector2(-119.8152367f+2,39.5436008f-2), new Vector2(427982.5f,4155490.8f)}
		};
	}
}