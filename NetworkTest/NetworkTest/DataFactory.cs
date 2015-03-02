using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Defines a general interface for parsing different file formats
/// </summary>
public abstract class Parser
{
    private string name = "Unnamed Parser";

    /// <summary>
    /// This function parses any dataset given to it and populates the DataRecord with the results
    /// </summary>
    /// <param name="record">The DataRecord to be populated</param>
    /// <param name="Contents">The file to be parsed into the DataRecord</param>
    /// <returns>DataRecord containing information of the parsed "Contents" file</returns>
    public virtual DataRecord Parse(DataRecord record, string Contents)
    {
        throw new System.NotImplementedException();
    }

    // Define custom functionality here (i.e. writing to files)
    public virtual void Parse(string Path, string OutputName, string Contents)
    {
        throw new System.NotImplementedException();
    }

    // Define custom functionality here (i.e. writing to files)
    public virtual void Parse(string Path, string OutputName, byte[] Contents)
    {
        throw new System.NotImplementedException();
    }

    public virtual DataRecord Parse(DataRecord record, byte[] Contents)
    {
        throw new System.NotImplementedException();
    }

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }
}

/// <summary>
/// Defines the different types of paths that can be used.
/// </summary>
public enum TransferType
{
    URL, FILE, UNKNOWN
};

/// <summary>
/// This class defines a general interface between the DataFactory and the 
/// services used to acquire data.
/// </summary>
public abstract class DataProducer
{
    /// <summary>
    /// This function imports the desired DataRecord specified by the Path.
    /// This could be loading data from a url, or file.
    /// </summary>
    /// <param name="Record">The DataRecord being loaded into and returned</param>
    /// <param name="Path">The location of the desired data</param>
    /// <returns>A new or updated DataRecord including the newly acquired data</returns>
    /// For path include file://"path" for files and for urls include url://"path"
    public abstract DataRecord Import(DataRecord Record, string Path, int priority = 1);

    /// <summary>
    /// Exports to the file drive from a download.
    /// </summary>
    /// <param name="Path"></param>
    /// <returns></returns>
    public abstract bool Export(string Path,string outputPath, string OutputName);

    protected TransferType getType(ref String str)
    {
        if (str.StartsWith("url://"))
        {
            str = str.Substring(6);
            return TransferType.URL;
        }
        else if (str.StartsWith("file://"))
        {
            str = str.Substring(7);
            return TransferType.FILE;
        }

        // Else
        return TransferType.UNKNOWN;
    }
}

/// <summary>
/// This class is the base class describing how DataFactories will be constructed
/// + i.e. FileFactory or NetworkingFactory
/// </summary>
public class DataFactory
{
    // Maps Strings to the their corresponding Producers
    private NetworkManager manager = new NetworkManager(4);
    protected Dictionary<String, DataProducer> Products = new Dictionary<String, DataProducer>();

    public DataFactory()
    {
        // Initialize the products --- We need to streamline this in someway.....
        Products.Add("WCS_BIL", new WCS_BIL_Producer(manager));
        Products.Add("WMS_PNG", new WMS_PNG_Producer(manager));
        Products.Add("WCS_1", null);
        Products.Add("WCS_2", null);
        Products.Add("WCS_3", null);
    }

    /// <summary>
    /// Creates a new DataProduct of the specified product type,
    /// with the correct DataProduct class functions and members
    /// </summary>
    /// <returns>A DataProduct of one specified type
    /// + i.e. WCS Product, WMS Product, WFS Product, etc.</returns>
    public DataRecord Import(String type, DataRecord Record, string Path, int priority = 1)
    {
        // Check if the product exists
        if( Products.ContainsKey( type ) )
        {
            return Products[type].Import(Record, Path, priority);
        }
        else
        {
            // Unsupported type
            throw new System.ArgumentException("Type: " + type + " is not supported.");
        }
    }

    public void Export(String type,string Path, string outputPath,string name)
    {
        // Check if the product exists
        if (Products.ContainsKey(type))
        {
            Products[type].Export(Path,outputPath,name);
        }
        else
        {
            // Unsupported type
            throw new System.ArgumentException("Type: " + type + " is not supported.");
        }
    }

    /// Some test download functions 
    void PrintString(string str)
    {
        Console.WriteLine(str);
    }
    void PrintBytes(byte[] bytes)
    {
        Console.WriteLine(bytes);
    }
    public void TestByteDownload(string url)
    {
        manager.AddDownload(new DownloadRequest(url, PrintBytes));
    }
    public void TestStringDownload(string url)
    {
        manager.AddDownload(new DownloadRequest(url,PrintString));
    }
}


