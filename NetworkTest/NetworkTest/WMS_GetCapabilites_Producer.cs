using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System;

class WMS_GetCapabilities_Producer : DataProducer
{
               // Fields
    NetworkManager nm;

    // Possible constructor
    public WMS_GetCapabilities_Producer( NetworkManager refToNM ) 
    {
        nm = refToNM;
    }

    XmlReader createStringReader(string sr)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.XmlResolver = null;
        settings.ProhibitDtd = false;
        return XmlReader.Create(new System.IO.StringReader(sr), settings);
    }

    void ParseWMSCapabilities(DataRecord Record ,string Str)
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

    }

    ///////////////////////////////////////////////////////////////////////////
    // Overrides Below
    ///////////////////////////////////////////////////////////////////////////
    protected override DataRecord ImportFromURL(DataRecord Record, string path, int priority = 1)
    {
        // Beautiful Lambda here
        // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
        //nc.DownloadBytes(path, ((DownloadBytes) => mp.Parse(Record, DownloadBytes)), priority);
        nm.AddDownload(new DownloadRequest(path, (StringFunction)((DownloadedString) => ParseWMSCapabilities(Record, DownloadedString)), priority));

        // Return
        return Record;
    }

    protected override DataRecord ImportFromFile(DataRecord Record, string path)
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

        // Return
        return Record;
    }

    public override bool ExportToFile(string Path, string outputPath, string outputName)
    {
        // The getType function will determine the type of transfer (file or url) and strip off special tokens to help determine the type.
        TransferType type = getType(ref Path);

        // Put Try Catch HERE
        // If file does not exist 
        if (type == TransferType.URL)
        {
            Console.WriteLine("URL: " + Path);
            // Beautiful Lambda here
            // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
            // Network Manager download
            //nm.AddDownload(new DownloadRequest(path, (ByteFunction)((DownloadBytes) => mp.Parse(outputPath,name,DownloadBytes))));
        }

        // Return
        return true;
    }
}
