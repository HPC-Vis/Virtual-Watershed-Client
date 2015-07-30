using System.Collections.Generic;
using System.Threading;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace CACHE_Test
{
	[TestFixture]
	[Category("Cache Tests")]
	internal class CacheTests
	{
		[Test]
		public void CacheTest()
		{
			FileBasedCache.Insert<int> ("test", 1);
			if (FileBasedCache.Exists ("test")) 
			{
				Debug.Log(FileBasedCache.Get<int>("test"));
				Assert.AreEqual(1,FileBasedCache.Get<int>("test"));
				return;
			}
			Assert.Fail ();
		}
	}
}
