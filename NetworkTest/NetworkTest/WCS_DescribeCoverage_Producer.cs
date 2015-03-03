using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

class WCS_DescribeCoverage_Producer : DataProducer
{
               // Fields
    NetworkManager nm;

    // Need to build a parser for this....
    void parseDescribeCoverage(string str, string key) 
    {
        var reader = System.Xml.XmlTextReader.Create(new System.IO.StringReader(str));

        XmlSerializer serial = new XmlSerializer(typeof(DescribeCoverageWCS.CoverageDescriptions));
        DescribeCoverageWCS.CoverageDescriptions testc = new DescribeCoverageWCS.CoverageDescriptions();

        if (serial.CanDeserialize(reader))
        {
            testc = (DescribeCoverageWCS.CoverageDescriptions)serial.Deserialize(reader);
        }

        string bbox = (testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox.LowerCorner.Replace(" ", ",") + "," + testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox.UpperCorner.Replace(" ", ","));

        /*int[] dim = grab_dimensions(testc.CoverageDescription.Domain.SpatialDomain.BoundingBox[0].LowerCorner, testc.CoverageDescription.Domain.SpatialDomain.BoundingBox[0].UpperCorner);
        for (int i = 0; i < 2; i++)

            manager.records[key].WGS84origin = grab_dimensions(testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox.UpperCorner);

        /// This should be passed to GetCoverage
        int width = dim[0];
        int height = dim[1];

        Vector2[] utmWorldDimensions = grab_dimensions_float(testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox.LowerCorner, testc.CoverageDescription.Domain.SpatialDomain.WGS84BoundingBox.UpperCorner);

        //Debug.LogError(bbox + " "  + manager.records[key].bbox);
        string epsg = "EPSG:" + "4326";
        manager.records[key].boundingBox = new Rect(utmWorldDimensions[1].x, utmWorldDimensions[0].y - Mathf.Abs(utmWorldDimensions[0].y - utmWorldDimensions[1].y), Mathf.Abs(utmWorldDimensions[0].x - utmWorldDimensions[1].x), Mathf.Abs(utmWorldDimensions[0].y - utmWorldDimensions[1].y));
        // Debug.LogError("Bounding BOX: " + manager.records[key].boundingBox);
        int pot = nearestPowerOfTwo(width);
        int pot2 = nearestPowerOfTwo(height);
        pot = Mathf.Min(new int[] { pot, pot2 });

        // This is a hard fixed addition.
        if (pot >= 2048)
        {
            pot = 1024;
        }
        pot++;
        manager.records[key].resolution = new Vector2(Mathf.Abs(utmWorldDimensions[0].x - utmWorldDimensions[1].x) / dim[0], -Mathf.Abs(utmWorldDimensions[0].y - utmWorldDimensions[1].y) / dim[1]);//toVector2(testc.CoverageDescription.Domain.SpatialDomain.GridCRS.GridOffsets,new char[]{' '});
        GetCoverage(key, epsg, bbox, pot, pot, interpolation: "bilinear");*/

    }

    // Possible constructor
    public WCS_DescribeCoverage_Producer (NetworkManager refToNM) 
    {
        nm = refToNM;
    }

    void ParseWFSCapabilities(DataRecord Record ,string Str)
    {
        Record.WFSCapabilities = Str;
    }
    public override DataRecord Import(DataRecord Record, string path, int priority = 1)
    {
        // Create a record if one does not exist
        if( Record == null )
        {
            Record = new DataRecord();
        }

        // The getType function will determine the type of transfer (file or url) and strip off special tokens to help determine the type.
        TransferType type = getType( ref path );
        Console.WriteLine("Path = " + path);

        // If import type is URL or FILE
        if (type == TransferType.URL)
        {
            // Beautiful Lambda here
            // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.

            //nc.DownloadBytes(path, ((DownloadBytes) => mp.Parse(Record, DownloadBytes)), priority);

            //nm.AddDownload(new DownloadRequest(path, (StringFunction) ((DownloadedString) => ParseWFSCapabilities(Record, DownloadedString)), priority));
        }
        else if (type == TransferType.FILE)
        {
            // Get the file name
            string filename = Path.GetFileNameWithoutExtension(path);
            string fileDirPath = Path.GetDirectoryName(path);
            string capabilities = fileDirPath + '\\' + filename + ".xml";

            if (File.Exists(capabilities))
            {
                // Open the files
                StreamReader capReader = new StreamReader(capabilities);

                // Read the header and data
                string header = capReader.ReadToEnd();

                // Close the files
                capReader.Dispose();
            }
            else
            {
                // Throw an exception that the file does not exist
                throw new System.ArgumentException("File does not exist: " + path);
            }
        }
        else
        {
            // Unhandled file types, error messages, invalid formatting
            throw new System.ArgumentException("Invalid Path: " + path);
        }

        // Return
        return Record;
    }

    public override bool Export(string path,string outputPath,string name)
    {
        // The getType function will determine the type of transfer (file or url) and strip off special tokens to help determine the type.
        TransferType type = getType(ref path);

        // Put Try Catch HERE
        // If file does not exist 
        if (type == TransferType.URL)
        {
            Console.WriteLine("URL: " + path);
            // Beautiful Lambda here
            // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
            // Network Manager download
            //nm.AddDownload(new DownloadRequest(path, (ByteFunction)((DownloadBytes) => mp.Parse(outputPath,name,DownloadBytes))));
        }

        // Return
        return true;
    }
}
