using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    //void giveFeature(string text, string key)
    //{
    //    var n = System.Xml.Linq.XDocument.Parse(text);
        
    //    System.Xml.Linq.XNamespace gml = "http://www.opengis.net/gml";
    //    System.Xml.Linq.XNamespace ms = "http://mapserver.gis.umn.edu/mapserver";
    //    var query = n.Root.Descendants(gml + "featureMember");

        
    //    manager.records[key].Lines = new List<List<Vector2>>();
    //    foreach (var c in query)
    //    {
    //       //var d = c.Elements();

    //        foreach (var e in c.Descendants(ms + "msGeometry").Elements())
    //        {
    //            manager.records[key].Lines.Add(getPoints(e.Value));
    //        }

    //    }
    //    manager.DataBuilder.addShape(key);
    //    //Debug.LogError(text);

    //}

class WFS_GML_Parser : Parser
{
    List<Vector2> getPoints(string points)
    {
        string[] pts = points.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        List<Vector2> pointList = new List<Vector2>();

        for (int i = 0; i < pts.Length; i++)
        {
            string[] temp = pts[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //Debug.LogError(temp[0] + " " + temp[1]);
            try
            {
                pointList.Add(new Vector2(float.Parse(temp[0]), float.Parse(temp[1])));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                // Debug.LogError(temp.Length);
            }
        }
        return pointList;
    }

    public override DataRecord Parse(DataRecord record, string Contents)
    {
        var n = System.Xml.Linq.XDocument.Parse(Contents);

        System.Xml.Linq.XNamespace gml = "http://www.opengis.net/gml";
        System.Xml.Linq.XNamespace ms = "http://mapserver.gis.umn.edu/mapserver";
        var query = n.Root.Descendants(gml + "featureMember");


        record.Lines = new List<List<SerialVector2>>();
        foreach (var c in query)
        {
            //var d = c.Elements();
            
            foreach (var e in c.Descendants(ms + "msGeometry").Elements())
            {
                record.Lines.Add(SerialVector2.ToVector2Array(getPoints(e.Value).ToArray()).ToList());
            }

        }
        return record;
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
        var sw = new System.IO.StreamWriter(Path + OutputName + ".gml");
        sw.Write(Str);
        sw.Close();
    }

}

