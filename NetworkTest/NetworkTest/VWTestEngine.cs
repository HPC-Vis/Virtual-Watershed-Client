using UnityEngine;
using System.Collections;

namespace NUnit.Tests
{
    using System;
    using NUnit.Framework;
    [TestFixture]
    public class VWTestEngine
    {
        [SetUp] 
        public void Init()
        { /* ... */ }

        [TearDown] 
        public void Dispose()
        { /* ... */ }

        [Test] // this is just a test that doesn't do anything useful
        public void dataRecordTest()
        {
            dataRecord record = new dataRecord();
            record.invokeTest();
            Assert.AreEqual(4, record.test);
        }

        [Test]
        public void dataDownloadTest()
        {
           // need to merge Thomas's test in program.cs into here
        }
    }
}