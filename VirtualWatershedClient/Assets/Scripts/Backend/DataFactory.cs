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

    public virtual List<DataRecord> Parse(string Path)
    {
        throw new System.NotImplementedException();
    }

    public virtual List<DataRecord> Parse(List<DataRecord> record, string Contents)
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

    public virtual List<DataRecord> Parse(List<DataRecord> record, byte[] Contents)
    {
        throw new System.NotImplementedException();
    }

    // For those times when things are being halted...
    public void Halt()
    {
        IsHalting = true;
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

    bool IsHalting = false;
}

/// <summary>
/// This class is the base class describing how DataFactories will be constructed
/// + i.e. FileFactory or NetworkingFactory
/// </summary>
public class DataFactory
{
    // Maps Strings to the their corresponding Producers
    public NetworkManager manager;
    protected Dictionary<String, DataProducer> Products = new Dictionary<String, DataProducer>();

    public DataFactory(NetworkManager networkmanager)
    {
        // Set the network manager
        manager = networkmanager;

        // Initialize the products --- We need to streamline this in someway.....
        Products.Add("WCS_BIL", new WCS_BIL_Producer(manager));
        Products.Add("WMS_PNG", new WMS_PNG_Producer(manager));
        Products.Add("VW_JSON", new VW_JSON_Producer(manager));
        Products.Add("WCS_CAP", new WCS_GetCapabilities_Producer(manager));
        Products.Add("WMS_CAP", new WMS_GetCapabilities_Producer(manager));
        Products.Add("WFS_CAP", new WFS_GetCapabilities_Producer(manager));
        Products.Add("WCS_DC", new WCS_DescribeCoverage_Producer(manager));
        Products.Add("VW_FGDC", new VW_FGDC_XML_Producer(manager));
        Products.Add("WFS_GML", new WFS_GML_Producer(manager));
        Products.Add("VW_MODEL_RUN", new VW_JSON_MODEL_RUN_Producer(manager));
        Products.Add("GDAL", new GDALProducer());
    }

    /// <summary>
    /// Creates a new DataProduct of the specified product type,
    /// with the correct DataProduct class functions and members
    /// </summary>
    /// <returns>A DataProduct of one specified type
    /// + i.e. WCS Product, WMS Product, WFS Product, etc.</returns>
    public List<DataRecord> Import(String type, List<DataRecord> Records, string Path, int priority = 1)
    {
        // Check if the product exists
        if (Products.ContainsKey(type))
        {
            return Products[type].Import(Records, Path, priority);
        }
        else
        {
            // Unsupported type
            throw new System.ArgumentException("Type: " + type + " is not supported.");
        }
    }

    /// <summary>
    /// The Export function takes a downloaded file and spits it out into file(s).
    /// </summary>
    /// <param name="type"></param>
    /// <param name="Path"></param>
    /// <param name="outputPath"></param>
    /// <param name="name"></param>
    public void Export(String type,string Path, string outputPath,string name)
    {
        // Check if the product exists
        if (Products.ContainsKey(type))
        {
            Products[type].ExportToFile(Path,outputPath,name);
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
        Logger.WriteLine(str);
    }
    void PrintBytes(byte[] bytes)
    {
        Logger.WriteLine(bytes.ToString());
    }
    public void TestByteDownload(string url)
    {
        manager.AddDownload(new DownloadRequest(url, PrintBytes));
    }
    public void TestStringDownload(string url)
    {
        manager.AddDownload(new DownloadRequest(url,PrintString));
    }
    public void DownloadString(DownloadRequest Request)
    {
        if (Request != null)
        {
            // Now downloading the request string....
            Logger.WriteLine("Now Downloading: " + Request.Url);
            manager.AddDownload(Request);
        }
    }
}


