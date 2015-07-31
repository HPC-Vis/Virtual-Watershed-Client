using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// WCS_DescribeCoverage_Parser
/// This parser will parse a DescribeCoverage xml from a WCS Request.
/// </summary>
class WCS_DescribeCoverage_Parser :Parser
{

    int[] grab_dimensions(string lower, string upper)
    {
        string[] l = lower.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string[] u = upper.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        return new[] { int.Parse(u[0]) - int.Parse(l[0]) + 1, int.Parse(u[1]) - int.Parse(l[1]) + 1 };
    }

    Vector2[] grab_dimensions_float(string lower, string upper)
    {
        string[] l = lower.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string[] u = upper.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //int zone = coordsystem.GetZone(float.Parse(l[1]), float.Parse(l[0]));
        //int zone2 = coordsystem.GetZone(float.Parse(u[1]), float.Parse(u[0]));
        //Vector2 upperleft = coordsystem.transformToUTM(float.Parse(l[0]), float.Parse(l[1]));
        //Vector2 lowerright = coordsystem.transformToUTM(float.Parse(u[0]), float.Parse(u[1]));
        //Debug.LogError(lowerright);
        //Debug.LogError(upperleft);
        //Debug.LogError(zone2);
        //Debug.Log(zone);
        /*if (zone != zone2)
        {
            // Thanks to https://www.maptools.com/tutorials/utm/details
            if (zone < zone2)
                upperleft.x -= Mathf.Abs(zone - zone2) * 774000f;
            else
                lowerright.x -= Mathf.Abs(zone - zone2) * 774000f;
        }*/
		float u1 = float.Parse(u[0]);
		float u2 = float.Parse(u[1]);
		float l1 = float.Parse(l[0]);
		float l2 = float.Parse(l[1]);
		return new[] { new Vector2(u1,u2), new Vector2(l1,l2) };
    }


    public void parseDescribeCoverage(DataRecord Record, string Str)
    {
        // Logger.WriteLine(Str);
        var reader = System.Xml.XmlTextReader.Create(new System.IO.StringReader(Str));

        XmlSerializer serial = new XmlSerializer(typeof(DescribeCoverageWCS.CoverageDescriptions));
        DescribeCoverageWCS.CoverageDescriptions testc = new DescribeCoverageWCS.CoverageDescriptions();
		
        if (serial.CanDeserialize(reader))
        {
            testc = (DescribeCoverageWCS.CoverageDescriptions)serial.Deserialize(reader);
			Logger.WriteLine ("PARSED: " + testc.CoverageDescription.Range.Field.Axis.AvailableKeys.Key.Count().ToString());
        }
        else
        {
        	Debug.LogError("NOOOOOOOOOOOOOOOOOOOO! Failed to parse!!");
        	return;
        }

        Record.CoverageDescription = testc;

		Record.numbands = testc.CoverageDescription.Range.Field.Axis.AvailableKeys.Key.Count();
        Debug.LogError(testc.CoverageDescription.Domain.SpatialDomain != null);
        if (testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox != null)
        {
            Record.bbox2 = (testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox.LowerCorner.Replace(" ", ",") + "," + testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox.UpperCorner.Replace(" ", ","));
            Vector2[] utmWorldDimensions = grab_dimensions_float(testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox.LowerCorner, testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox.UpperCorner);
            Record.boundingBox = new Rect(utmWorldDimensions[0].x, utmWorldDimensions[1].y, Mathf.Abs(utmWorldDimensions[0].x - utmWorldDimensions[1].x), Mathf.Abs(utmWorldDimensions[0].y - utmWorldDimensions[1].y));
        }
        else
        {
             var boundingbox = testc.CoverageDescription.Domain.SpatialDomain.BoundingBox[1];
             var lowerSplit = boundingbox.LowerCorner.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
             var upperSplit = boundingbox.UpperCorner.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);

             Record.bbox2 = lowerSplit[0] + "," + lowerSplit[1] + "," + upperSplit[0] + "," + upperSplit[1];
        }
        int[] dim = grab_dimensions(testc.CoverageDescription.Domain.SpatialDomain.BoundingBox[0].LowerCorner, testc.CoverageDescription.Domain.SpatialDomain.BoundingBox[0].UpperCorner);
        Record.width = dim[0];
        Record.height = dim[1];

        
        

        //string epsg = "EPSG:" + "4326";
        
    }


    public override DataRecord Parse(DataRecord record, string Contents)
    {
        try
        {
            parseDescribeCoverage(record, Contents);
        }
        catch(Exception e)
        {
            Logger.WriteLine(e.Message);
            Logger.WriteLine(e.StackTrace);
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
        var sw = new System.IO.StreamWriter(Path + OutputName + ".xml");
        sw.Write(Str);
        sw.Close();
    }
}
