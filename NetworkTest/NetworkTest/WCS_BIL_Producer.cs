using System;
// We will need to change this when the time comes around to do the networking version...
using System.IO;

/// <summary>
/// The following WCS Product is used to download any data and populate the passed in DataRecord.
/// </summary>
public class WCS_BIL_Producer : DataProducer
{
    // Fields
    NetworkManager nm;
    mimeparser mp = new mimeparser();

    // Possible constructor
    public WCS_BIL_Producer( NetworkManager refToNM ) 
    {
        nm = refToNM;
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

            nm.AddDownload(new DownloadRequest(path, (ByteFunction) ((DownloadBytes) => mp.Parse(Record, DownloadBytes)), priority));
        }
        else if (type == TransferType.FILE)
        {
            // Get the file name
            string filename = Path.GetFileNameWithoutExtension(path);
            string fileDirPath = Path.GetDirectoryName(path);
            string hdrName = fileDirPath + '\\' + filename + ".hdr";
            string bilName = fileDirPath + '\\' + filename + ".bil";

            if (File.Exists(hdrName) && File.Exists(bilName))
            {
                // Open the files
                StreamReader hdrReader = new StreamReader(hdrName);
                FileInfo bilFI = new FileInfo(bilName);
                BinaryReader bilReader = new BinaryReader(File.Open(bilName, FileMode.Open));

                // Read the header and data
                string header = hdrReader.ReadToEnd();
                byte[] dataBytes = bilReader.ReadBytes((int)bilFI.Length);

                // Save into record
                Record.Data = bilreader.parse(header, dataBytes);

                // Close the files
                hdrReader.Dispose();
                bilReader.Close();
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
            //nc.DownloadBytes(path, ((DownloadBytes) => mp.Parse(outputPath,name,DownloadBytes)));
            nm.AddDownload(new DownloadRequest(path, (ByteFunction)((DownloadBytes) => mp.Parse(outputPath,name,DownloadBytes))));
        }

        // Return
        return true;
    }
}
