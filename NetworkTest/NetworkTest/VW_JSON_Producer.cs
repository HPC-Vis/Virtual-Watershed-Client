﻿using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System;

class VW_JSON_Producer : DataProducer
{
    // Fields
    NetworkManager nm;

    // Possible constructor
    public VW_JSON_Producer( NetworkManager refToNM ) 
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

    // Make a Parser
    void ParseJSON(DataRecord Record ,string Str)
    {

    }

    ///////////////////////////////////////////////////////////////////////////
    // Overrides Below
    ///////////////////////////////////////////////////////////////////////////
    protected override DataRecord ImportFromURL(DataRecord Record, string path, int priority = 1)
    {
        // Beautiful Lambda here
        // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
        //nc.DownloadBytes(path, ((DownloadBytes) => mp.Parse(Record, DownloadBytes)), priority);
        nm.AddDownload(new DownloadRequest(path, (StringFunction) ((DownloadedString) => ParseJSON(Record, DownloadedString)), priority));

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
