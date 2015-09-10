using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using OSGeo.GDAL;

// Gdal class for pulling data out
public class RasterDataset
{
    string File;
    Dataset dataset = null;
    List<string> Subdatasets = new List<string>();
    // Assuming this for the window size...
    int MAX_WIDTH = 100, MAX_HEIGHT = 100;
    public RasterDataset(string file)
    {
        File = file;
    }
    public bool Open()
    {
        Gdal.AllRegister();
        System.Console.WriteLine(File);
        try 
        {
            dataset = Gdal.Open(File, Access.GA_ReadOnly);
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
        return false;
    }

    public bool HasSubDatasets(out int numsubdatasets)
    {
        numsubdatasets = 0;
        var sub = dataset.GetMetadata("SUBDATASETS");

        if(sub != null)
        {
            populateSubDatasets(sub);
        }
        numsubdatasets = Subdatasets.Count;
        return sub != null && sub.Length != 0;
    }

    public List<string> GetSubDatasets()
    {
        if(Subdatasets.Count == 0)
        {
            var sub = dataset.GetMetadata("SUBDATASETS");
            if (sub != null && sub.Length != 0 )
            {
                populateSubDatasets(sub);
            }
        }
        return Subdatasets;
    }

    void populateSubDatasets(string[] SubMetadata)
    {
        // Parse the subdatasets strings
        int count = SubMetadata.Length;
        for (int i = 0; i < count; i += 2)
        {
            //System.Console.WriteLine(i);
            Subdatasets.Add(SubMetadata[i].Replace("SUBDATASET_" + ((i / 2)+1) + "_NAME=", ""));
            //System.Console.WriteLine(SubMetadata[i].Replace("SUBDATASET_"+ ((i/2)+1) + "_NAME=",""));
        }
    }

    public void GetMetaData()
    {
        var lis = dataset.GetMetadataDomainList();
        Debug.LogError("METADATA");
        foreach(var i in lis)
        {
            Debug.LogError(i);
            var lis2 = dataset.GetMetadata(i);
            foreach(var j in lis2)
            {
                Debug.LogError("SUBDATA: " + j);
            }
        }
    }

    // A quick dirty parser for netcdf times.. in the metadata
    public void GetTimes(out DateTime begin, out TimeSpan timespan)
    {
        begin = DateTime.MinValue;
        timespan = TimeSpan.MaxValue;
        // Get Metadata
        var md = dataset.GetMetadata("");
        List<string> strings = new List<string>();
        string substring = "";
        string matched = "";
        foreach (var i in md)
        {
            Debug.LogError(i);
            var match = Regex.Match(i, "time#.*=");
            if (match.Success && i.ToLower().Contains("since"))
            {
                matched = i;
                substring = match.Value;
                break;
            }
        }

        Debug.LogError("SUBSTRING: " + substring);


        string time = matched.Replace(substring, "").Replace(" since ", " ");
        if (time == "")
            return;

        //Debug.LogError(time);
        var timeinfo = time.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (timeinfo.Length != 2)
            return;
        Debug.LogError(timeinfo[0]);
        Debug.LogError(timeinfo[1]);
        TimeSpan ts = new TimeSpan();
        if (timeinfo[0].ToLower() == "hours")
        {
            ts = new TimeSpan(dataset.RasterCount, 0, 0);
            timespan = ts;
            Debug.LogError("HOURS: " + dataset.RasterCount);
        }
        else if(timeinfo[0].ToLower() == "days")
        {
            ts = new TimeSpan(dataset.RasterCount, 0, 0, 0);
            timespan = ts;
            Debug.LogError("DAYS: " + dataset.RasterCount);
        }

        var times = timeinfo[1].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

        if (times.Length != 3)
            return;
        times[2] = times[2].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];
        DateTime dt = new DateTime(int.Parse(times[0]), int.Parse(times[1]), int.Parse(times[2]));
        begin = dt;


    }

    public int GetRasterCount()
    {
        return dataset.RasterCount;
    }

    public List<float[,]> GetData()
    {
        List<float[,]> data = new List<float[,]>();
        int width = dataset.RasterXSize;
        int height = dataset.RasterYSize;
        //width = Math.Min(width, MAX_WIDTH);
        //height = Math.Min(height, MAX_HEIGHT);
        width = 100;
        height = 100;
        width = height = Math.Max(width, height);
        Console.WriteLine(width + " " + height);
        for (int i = 0; i < dataset.RasterCount; i++)
        {
            float[] DataF = new float[width * height];
            float[,] Data = new float[width, height];
            var band = dataset.GetRasterBand(i + 1);
            try
            {
                band.ReadRaster(0, 0, band.XSize, band.YSize, DataF, width, height, 0, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return data;
            }
            Debug.LogError(width + " " + height);
            
            for (int k = 0; k < width; k++)
            {
                for (int j = 0; j < height; j++)
                {
                    Data[k, j] = DataF[(k) * height + height-1-j];
                }
            }



            data.Add(Data);

        }
        Console.WriteLine(dataset.RasterCount);
        return data;
    }

    public static string GetGdalPath(string path)
    {
        string extension = Path.GetExtension(path);
        if(extension.Contains("nc"))
        {
            return "NETCDF:" + '"'+path + '"';
        }
        else if(extension.Contains("tif"))
        {
            return path;
        }
        else if(path.ToLower().Contains("wms"))
        {
            return "WMS:" + path;
        }
        else if(path.ToLower().Contains("wcs"))
        {
            return "WCS:" + path;
        }
        return path;
    }

    static string XMLElement(string name, string value)
    {
        return "<" + name + ">" + value + "</" + name + ">";

    }


    /// <summary>
    /// Returns a bounding box string of the form upperleft x upper y botttomright x bottom y (or l t r b or minx maxy maxx miny or ..
    /// )
    /// </summary>
    /// <returns></returns>
    public string GetBoundingBox()
    {
        double[] geoTransform = new double[6];
        dataset.GetGeoTransform(geoTransform);
        double minx = geoTransform[0];
        double maxy = geoTransform[3];
        double maxx = minx + geoTransform[1]*dataset.RasterXSize;
        double miny = maxy - geoTransform[5]*dataset.RasterYSize;
        Debug.LogError(dataset.GetProjection());
        Debug.LogError(minx + " " + maxy);
        OSGeo.OSR.SpatialReference sr1 = new OSGeo.OSR.SpatialReference(dataset.GetProjection());
        sr1.ImportFromEPSG(26911);
        OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference("");
        sr2.ImportFromEPSG(4326);

        OSGeo.OSR.CoordinateTransformation ct = new OSGeo.OSR.CoordinateTransformation(sr1, sr2);
        double[] upperleft = new double[] {minx,maxy};
        double[] lowerright = new double[] {maxx,miny};
        ct.TransformPoint(lowerright);
        ct.TransformPoint(upperleft);

        return upperleft[0] + " " + upperleft[1] + " " + lowerright[0] + " " + lowerright[1];
    }

    public string ReturnProjection()
    {
        OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference("");
        sr2.ImportFromEPSG(26911);
        string wktstring = "";
        sr2.ExportToWkt(out wktstring);
        return wktstring;//dataset.GetProjection();
    }

    public static string buildXMLString(DataRecord record)
    {
        if (record.WCSOperations == null)
        {
            return "";
        }

        // Grab the identifier of the record
        string identifer = record.Identifier;

        // Grab the service url of the record
        string ServiceUrl = record.WCSCap.OperationsMetadata[0].DCP.HTTP.Get.href;

        // produce a xml file.
        string wcs_xml_string = XMLElement("WCS_GDAL", XMLElement("ServiceURL", ServiceUrl) + "\n" + XMLElement("CoverageName", identifer));
        return wcs_xml_string;
    }

    public bool IsTemporal()
    {
        return dataset.RasterCount > 1;
    }
}
