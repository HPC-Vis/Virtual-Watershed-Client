using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using UnityEngine;


/// <summary>
/// This Parser handles the WMSCapabilities from a wms capabilities string.
/// </summary>
class WMS_GetCapabilities_Parser : Parser
{
    XmlReader createStringReader(string sr)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.XmlResolver = null;
        settings.ProhibitDtd = false;
        return XmlReader.Create(new System.IO.StringReader(sr), settings);
    }

    /// <summary>
    /// This version of parse parses the given input and outputs it to the file directory.
    /// </summary>
    /// <param name="Path"></param>
    /// <param name="OutputName"></param>
    /// <param name="str"></param>
    public override void Parse(string Path, string OutputName, string Str)
    {

        // Initialize variables
        var sw = new System.IO.StreamWriter(Path + OutputName + ".xml");
        sw.Write(Str);
        sw.Close();
    }

    void ParseWMSCapabilities(DataRecord Record, string Str)
    {
        Record.WMSCapabilities = Str;

        // if(!manager.records.ContainsKey(key) || !manager.records[key].services.ContainsKey("wms"))
        //Debug.LogError(key);
        XmlReader reader = createStringReader(Str);

		XmlSerializer serial = new XmlSerializer(typeof(WMS_CAPABILITIES.WMT_MS_Capabilities));

		WMS_CAPABILITIES.WMT_MS_Capabilities capabilities = new WMS_CAPABILITIES.WMT_MS_Capabilities();

		if (serial.CanDeserialize(reader))
		{
			try
			{
				capabilities = ((WMS_CAPABILITIES.WMT_MS_Capabilities)serial.Deserialize(reader));
				Record.wmslayers = capabilities.Capability.Layer.Layer;
				Logger.WriteLine(Record.wmslayers.Count().ToString());
			}
			catch(Exception e) 
			{
				Logger.WriteLine (e.Message);
			}

		}
		return;
        System.Xml.Linq.XDocument document = System.Xml.Linq.XDocument.Load(reader);

        var query = document.Descendants("LatLonBoundingBox");//.Descendants("WMT_MS_Capabilities");
        var title = document.Descendants("Title");
		//var layers = document.Descendants("Layer");

        foreach (var t in title)
        {
            //Debug.LogError(t.Name + " " + t.Value);//t.t;
            Record.title = t.Value;
            break;
        }

        int count = 0;
        float minx = 0, miny = 0, maxx = 0, maxy = 0;

        foreach (var c in query.Attributes())
        {

            switch (c.Name.ToString())
            {
                case "minx":
                    minx = float.Parse(c.Value);
                    break;
                case "maxx":
                    maxx = float.Parse(c.Value);
                    break;
                case "miny":
                    miny = float.Parse(c.Value);
                    break;
                case "maxy":
                    maxy = float.Parse(c.Value);
                    break;
            }
            count++;
            if (count == 4)
                break;
        }
			
        Record.boundingBox = new Rect(minx, maxy, Mathf.Abs(maxx - minx), Mathf.Abs(maxy - miny));

    }

    public override DataRecord Parse(DataRecord record, string Contents)
    {
        ParseWMSCapabilities(record, Contents);
        return record;
    }
}

