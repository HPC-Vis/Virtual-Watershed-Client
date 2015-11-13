using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Collections.Generic;

class WFS_GetCapabilities_Producer : DataProducer
{
            // Fields
    NetworkManager nm;

    // Possible constructor
    public WFS_GetCapabilities_Producer( NetworkManager refToNM ) 
    {
        nm = refToNM;
    }

	public WFS_GetCapabilities_Producer()
	{
	}

    // Need to create a parser stub for this.
    public void ParseWFSCapabilities(DataRecord Record ,string Str)
    {
    	Logger.WriteLine("PARSE WFS CAPABILITIES");
        Record.WFSCapabilities = Str;
        Record.Type = "shapefile";
        //return;
		var reader = System.Xml.XmlTextReader.Create(new System.IO.StringReader(Str));
		Record.WCSCapabilities = Str;
		

		XmlSerializer serial = new XmlSerializer(typeof(WFS_CAPABILITIES_SERVICE.WFS_Capabilities));
		WFS_CAPABILITIES_SERVICE.WFS_CapabilitiesService t = new WFS_CAPABILITIES_SERVICE.WFS_CapabilitiesService ();

		WFS_CAPABILITIES_SERVICE.WFS_Capabilities capabilities = new WFS_CAPABILITIES_SERVICE.WFS_Capabilities();

		if (serial.CanDeserialize (reader)) {
			//WFS_CAPABILITIES_SERVICE.WFS_CapabilitiesFeatureTypeList
			try
			{
				capabilities = (WFS_CAPABILITIES_SERVICE.WFS_Capabilities)serial.Deserialize (reader);
				Record.Identifier = capabilities.FeatureTypeList.FeatureType.Name;
			}
			catch (Exception e)
			{
				Logger.WriteLine (e.Message);
				//Logger.WriteLine (e.StackTrace);
				//Logger.WriteLine (e.InnerException.Message);
				//Logger.WriteLine (e.InnerException.StackTrace);
				//Logger.WriteLine (e.InnerException.InnerException.Message);
				return;
			}
			Logger.WriteLine ("NAME: " + capabilities.FeatureTypeList.FeatureType.Name);
			if (Record.Identifier == null) {
				
			}
		} 
		else 
		{
			Logger.WriteLine ("CANT PARSE");
		}
    }


    void WriteToFile(string Path, string OutputName, string Str)
    {
        // Initialize variables
        var sw = new System.IO.StreamWriter(Path + OutputName + ".xml");
        sw.Write(Str);
        sw.Close();
    }

    ///////////////////////////////////////////////////////////////////////////
    // Overrides Below
    ///////////////////////////////////////////////////////////////////////////
    protected override List<DataRecord> ImportFromURL(List<DataRecord> Records, string path, int priority = 1)
    {
        // Beautiful Lambda here
        // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
        //nc.DownloadBytes(path, ((DownloadBytes) => mp.Parse(Record, DownloadBytes)), priority);
        nm.AddDownload(new DownloadRequest(path, (StringFunction)((DownloadedString) => ParseWFSCapabilities(Records[0], DownloadedString)), priority));

        // Return
        return Records;
    }

    protected override List<DataRecord> ImportFromFile(List<DataRecord> Records, string path)
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
        return Records;
    }

    public override bool ExportToFile(string Path, string outputPath, string outputName)
    {
        // The getType function will determine the type of transfer (file or url) and strip off special tokens to help determine the type.
        Transfer.Type type = Transfer.GetType(ref Path);

        // Put Try Catch HERE
        // If file does not exist 
        if (type == Transfer.Type.URL)
        {
            Logger.WriteLine("URL: " + Path);
            // Beautiful Lambda here
            // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
            // Network Manager download
            nm.AddDownload(new DownloadRequest(Path, (StringFunction)((DownloadedString) => WriteToFile(outputPath, outputName, DownloadedString))));
        }

        // Return
        return true;
    }
}

