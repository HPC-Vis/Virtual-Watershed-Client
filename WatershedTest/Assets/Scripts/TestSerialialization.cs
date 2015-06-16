using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace NetworkTest
{
    [Serializable]
    public class SerialItem
    {
        public SerialItem()
        {

        }
    }

    [Serializable]
    class TestSerialialization
    {
        SerialItem test = new SerialItem();
        public TestSerialialization()
        {

        }
    }
}
