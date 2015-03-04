using System;
using System.Collections.Generic;
using System.Collections;
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
    public DataRecord Import(DataRecord Record, string Path, int priority = 1)
    {
        // Allocate a data record if one does not exist
        if (Record == null) { Record = new DataRecord(); }

        // Check the path
        Console.WriteLine("Path = " + Path);
        TransferType type = getType(ref Path);
        if (type == TransferType.URL)
        {
            // Call the url import function
            return ImportFromURL(Record, Path, priority);
        }
        else if (type == TransferType.FILE)
        {
            // Call the file import function
            return ImportFromFile(Record, Path);
        }
        else
        {
            // Throw an exception for unsupported operation
            throw new System.ArgumentException("File type is not \'file\' or \'url\': " + Path);
        }
    }

    public List<DataRecord> Import(List<DataRecord> Records, string Path, int priority = 1)
    {
        // Allocate a data record if one does not exist
        if (Records == null) { Records = new List<DataRecord>(); }

        // Check the path
        Console.WriteLine("Path = " + Path);
        TransferType type = getType(ref Path);
        if (type == TransferType.URL)
        {
            // Call the url import function
            return ImportFromURL(Records, Path, priority);
        }
        else if (type == TransferType.FILE)
        {
            // Call the file import function
            return ImportFromFile(Records, Path);
        }
        else
        {
            // Throw an exception for unsupported operation
            throw new System.ArgumentException("File type is not \'file\' or \'url\': " + Path);
        }
    }

    protected abstract DataRecord ImportFromURL(DataRecord Record, string path, int priority = 1);

    protected abstract DataRecord ImportFromFile(DataRecord Record, string path);

    protected abstract List<DataRecord> ImportFromURL(List<DataRecord> Record, string path, int priority = 1);

    protected abstract List<DataRecord> ImportFromFile(List<DataRecord> Record, string path);
    /// <summary>
    /// Exports to the file drive from a download.
    /// </summary>
    /// <param name="Path"></param>
    /// <returns></returns>
    public abstract bool ExportToFile(string Path, string outputPath, string outputName);

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