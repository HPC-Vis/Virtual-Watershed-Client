using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/// <summary>
/// This Producer handles the FGDC xml data from the virtual watershed.
/// </summary>
class VW_FGDC_XML_Producer : DataProducer
{
        // Fields
    NetworkManager nm;
    VW_FGDC_XML_Parser parser = new VW_FGDC_XML_Parser();

    // Possible constructor
    public VW_FGDC_XML_Producer( NetworkManager refToNM ) 
    {
        nm = refToNM;
    }
    


    ///////////////////////////////////////////////////////////////////////////
    // Overrides Below
    ///////////////////////////////////////////////////////////////////////////
    protected override List<DataRecord> ImportFromURL(List<DataRecord> Records, string path, int priority = 1)
    {
        // Beautiful Lambda here
        // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
        //nc.DownloadBytes(path, ((DownloadBytes) => mp.Parse(Record, DownloadBytes)), priority);
        nm.AddDownload(new DownloadRequest(path, (ByteFunction) ((DownloadedBytes) => parser.Parse(Records[0],DownloadedBytes)), priority));

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
            // Beautiful Lambda here
            // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
            // Network Manager download
            //Console.WriteLine("URL: " + Path);
            //nm.AddDownload(new DownloadRequest(Path, (ByteFunction)((DownloadBytes) => mp.Parse(outputPath, outputName, DownloadBytes))));
        }

        // Return
        return true;
    }
}

