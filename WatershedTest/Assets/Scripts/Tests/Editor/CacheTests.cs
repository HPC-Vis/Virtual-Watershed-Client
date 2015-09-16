using System.Collections.Generic;
using System.Threading;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using System;

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

        [Test]
        public void TestModelRun()
        {
            // Build the model run
            ModelRun deltarun = new ModelRun("Nolan fake data.", "FAKE_TWO");
            deltarun.Location = GlobalConfig.Location;
            ModelRunManager.InsertModelRun("FAKE_TWO", deltarun);



            DataRecord insert = new DataRecord("SOME Fake Names");
            insert.location = GlobalConfig.Location;
            insert.modelRunUUID = "FAKE_TWO";
            insert.id = Guid.NewGuid().ToString();
            insert.variableName = "Delta_NOALN_FAKE";

            insert.Min = float.MaxValue;
            insert.Max = float.MinValue;

            insert.start = DateTime.Now;
            insert.end = DateTime.Now;

            // Add the Datarecord to the ModelRunManager
            ModelRunManager.InsertDataRecord(insert.Clone(), new List<DataRecord>());


            //Cache the ModelRuns again
            Debug.LogError("Adding the comparison to the filebased cache.");
            //FileBasedCache.Insert<ModelRun>(deltarun.ModelRunUUID, deltarun);
            
            
            try
            {
                var startCache = FileBasedCache.Get<Dictionary<string, ModelRun>>("startup");
                startCache.Add(deltarun.ModelRunUUID, deltarun);
                FileBasedCache.Insert<Dictionary<string, ModelRun>>("startup", startCache);
            }
            catch(System.Exception a)
            {
                Debug.LogError("This was the error: " + a.Message);
                Debug.LogError(a.StackTrace);
            }

        }
	}
}
