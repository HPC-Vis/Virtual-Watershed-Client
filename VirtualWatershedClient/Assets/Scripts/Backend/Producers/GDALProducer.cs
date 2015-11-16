using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GDALProducer : DataProducer
{

    bool GDALOpen(string resource,DataRecord dr)
    {
        RasterDataset rd = new RasterDataset(resource);
        int numSubData = 0;

        if (rd.Open())
        {
            if (rd.HasSubDatasets(out numSubData))
            {
                Logger.WriteLine("Error this has subdatasets");
                return false;
            }
            dr.Data = rd.GetData();
            dr.numbands = dr.Data.Count;
            dr.multiLayered = dr.numbands > 0 ? "layered" : null;
        }

        return true;
    }

    ///////////////////////////////////////////////////////////////////////////
    // Overrides Below
    ///////////////////////////////////////////////////////////////////////////
    protected override List<DataRecord> ImportFromURL(List<DataRecord> Records, string path, int priority = 1)
    {
        // Beautiful Lambda here
        // Downloads the bytes and uses the ByteFunction lambda described in the passed parameter which will call the mime parser and populate the record.
        //nc.DownloadBytes(path, ((DownloadBytes) => mp.Parse(Record, DownloadBytes)), priority);
        GDALOpen(path, Records[0]);

        // Return
        return Records;
    }

    protected override List<DataRecord> ImportFromFile(List<DataRecord> Records, string path)
    {
        if (File.Exists(path))
        {
            GDALOpen(path, Records[0]);
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

        }

        // Return
        return true;
    }
}
