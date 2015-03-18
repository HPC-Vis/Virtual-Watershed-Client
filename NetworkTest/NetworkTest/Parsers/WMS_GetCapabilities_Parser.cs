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
        System.Xml.Linq.XDocument document = System.Xml.Linq.XDocument.Load(reader);

        var query = document.Descendants("LatLonBoundingBox");//.Descendants("WMT_MS_Capabilities");
        var title = document.Descendants("Title");


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


        // Coordinate System CODE!!!!!!!
        /*int zone = coordsystem.GetZone(maxy, minx);
        int zone2 = coordsystem.GetZone(miny, maxx);
        Debug.LogError(zone + " " + zone2);
        Vector2 upperleft = coordsystem.transformToUTM(minx, maxy);
        Vector2 lowerright = coordsystem.transformToUTM(maxx, miny);
        if (zone != zone2)
        {
            // Thanks to https://www.maptools.com/tutorials/utm/details
            if (zone < zone2)
                upperleft.x -= Mathf.Abs(zone - zone2) * 674000f;
            else
                lowerright.x -= Mathf.Abs(zone - zone2) * 674000f;
        }
        //Debug.LogError(upperleft);
        manager.records[key].boundingBox = new Rect(upperleft.x, upperleft.y - Mathf.Abs(upperleft.y - lowerright.y), Mathf.Abs(upperleft.x - lowerright.x), Mathf.Abs(upperleft.y - lowerright.y));*/
       // We need to figure out what to assign the rect -- for now it is the logical thing to do 
        Record.boundingBox = new Rect(minx, maxy, Mathf.Abs(maxx - minx), Mathf.Abs(maxy - miny));

    }

    public override DataRecord Parse(DataRecord record, string Contents)
    {
        ParseWMSCapabilities(record, Contents);
        return record;
    }
}

