using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleJSON;
using SimpleJson;
using SimpleJson.Reflection;
using Newtonsoft.Json;
[TestFixture]
[Category("Network Tests")]
internal class SerializationTests
{
    // Use JSON.net
    [Test]
    public void SessionDataTest()
    {
        SessionDataStructure test = new SessionDataStructure();
        test.GameObjects = new Dictionary<string, SessionObjectStructure>();
        test.GameObjects.Add("putty", new SessionObjectStructure());
        //test.GameObjects["putty"].DataLocation = "wonder land";
        test.GameObjects["putty"].Name = "apples and sphagetti";
        test.Location = "abc";
        string a = JsonConvert.SerializeObject(test);
        Debug.LogError(a);
        Assert.AreEqual(JsonConvert.DeserializeObject<SessionDataStructure>(a).GameObjects["putty"].Name, test.GameObjects["putty"].Name);
    }
}
