using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

/// <summary>
/// WMS_PNG_Producer ... as of current it only handles version 1.1.1 of WMS
/// </summary>
class WMS_PNG_Producer : DataProducer
{
    // Fields
    NetworkManager nm;
    mimeparser mp = new mimeparser();

    // Possible constructor
    public WMS_PNG_Producer( NetworkManager refToNM ) 
    {
        nm = refToNM;
    }
    
    // We should define this somewhere else for other functions to use.
    void SetTexture(DataRecord Record,byte[] bytes)
    {
        Console.WriteLine("TEXTURE: " + bytes);
        //Console.ReadKey();
        Record.texture = bytes;
    }

    // This go somewhere as write binary file
    void WriteTexture(string OutputPath, string OutputName, byte[] byteData)
    {
        // Output to files. Need this error catching here......
        var bw = new System.IO.BinaryWriter(new System.IO.FileStream(OutputPath + OutputName + ".png", System.IO.FileMode.Create));
        bw.Write(byteData, 0, byteData.Length);
        bw.Close();
        Console.WriteLine("WROTE: " + OutputPath + OutputName + ".png");
    }

    ///////////////////////////////////////////////////////////////////////////
    // Overrides Below
    ///////////////////////////////////////////////////////////////////////////
    protected override List<DataRecord> ImportFromURL(List<DataRecord> Records, string path, int priority = 1)
    {
        // Beautiful Lambda here
        // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
        nm.AddDownload(new DownloadRequest(path, (ByteFunction)((DownloadBytes) => SetTexture(Records[0], DownloadBytes)), priority));

        // Return
        return Records;
    }

    protected override List<DataRecord> ImportFromFile(List<DataRecord> Records, string path)
    {
        // Get the file name
        string filename = Path.GetFileNameWithoutExtension(path);
        string fileDirPath = Path.GetDirectoryName(path);
        string pngName = fileDirPath + '\\' + filename + ".png";

        if (File.Exists(pngName))
        {
            // Open the files
            FileInfo pngFI = new FileInfo(pngName);
            BinaryReader pngReader = new BinaryReader(File.Open(pngName, FileMode.Open));

            // Read the header and data
            byte[] dataBytes = pngReader.ReadBytes((int)pngFI.Length);

            // Save into record
            Records[0].texture = dataBytes;

            // Close the files
            pngReader.Close();
        }
        else
        {
            // Throw an exception that the file does not exist
            throw new System.ArgumentException("File does not exist: " + path);
        }

        // Return
        return Records;
    }

    public override bool ExportToFile(string path, string outputPath, string outputName)
    {
        // The getType function will determine the type of transfer (file or url) and strip off special tokens to help determine the type.
        Transfer.Type type = Transfer.GetType(ref path);

        // Put Try Catch HERE
        // If file does not exist 
        if (type == Transfer.Type.URL)
        {
            Console.WriteLine("URL: " + path);
            // Beautiful Lambda here
            // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
            // Network Manager download
            nm.AddDownload(new DownloadRequest(path, (ByteFunction)((DownloadBytes) => WriteTexture(outputPath, outputName, DownloadBytes))));
        }

        // Return
        return true;
    }
}
