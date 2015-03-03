using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;

class WCS_GetCapabilities_Producer : DataProducer
{
    // Fields
    NetworkManager nm;

    // Possible constructor
    public WCS_GetCapabilities_Producer( NetworkManager refToNM ) 
    {
        nm = refToNM;
    }

    void ParseWCSCapabilities(DataRecord Record ,string Str)
    {
        var reader = System.Xml.XmlTextReader.Create(Str);
        Record.WCSCapabilities = Str;


        XmlSerializer serial = new XmlSerializer(typeof(GetCapabilites.Capabilities));
        GetCapabilites.Capabilities capabilities = new GetCapabilites.Capabilities();

        if (serial.CanDeserialize(reader))
        {
            capabilities = ((GetCapabilites.Capabilities)serial.Deserialize(reader));
            capabilities.OperationsMetadata = capabilities.OperationsMetadata;
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    // Overrides Below
    ///////////////////////////////////////////////////////////////////////////
    protected override DataRecord ImportFromURL(DataRecord Record, string path, int priority = 1)
    {
        // Beautiful Lambda here
        // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
        //nc.DownloadBytes(path, ((DownloadBytes) => mp.Parse(Record, DownloadBytes)), priority);
        nm.AddDownload(new DownloadRequest(path, (StringFunction) ((DownloadedString) => ParseWCSCapabilities(Record, DownloadedString)), priority));

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

    public override bool ExportToFile(string path,string outputPath,string outputName)
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

