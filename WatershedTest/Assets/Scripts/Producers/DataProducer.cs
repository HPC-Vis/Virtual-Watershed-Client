using System;
using System.Collections.Generic;
using System.Collections;


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
    /// <param name="Record">The list of DataRecord being loaded into and returned</param>
    /// <param name="Path">The location of the desired data</param>
    /// <returns>A new or updated list of DataRecord including the newly acquired data</returns>
    /// For path include file://"path" for files and for urls include url://"path"
    public List<DataRecord> Import(List<DataRecord> Records, string Path, int priority = 1)
    {
        // Allocate a data record if one does not exist
        if (Records == null) { Records = new List<DataRecord>(); }
        //DataTracker.updateJob(Path, DataTracker.Status.RUNNING);
        // Check the path
        //Logger.WriteLine("Data Producer Path = " + Path);
        Transfer.Type type = Transfer.GetType(ref Path);
        if (type == Transfer.Type.URL)
        {
            // Call the url import function
            //DataTracker.submitJob(Path, Records);
            return ImportFromURL(Records, Path, priority);
        }
        else if (type == Transfer.Type.FILE)
        {
            // Call the file import function
            //DataTracker.submitJob(Path, Records);
            return ImportFromFile(Records, Path);
        }
        else
        {
            // Throw an exception for unsupported operation
            throw new System.ArgumentException("File type is not \'file\' or \'url\': " + Path);
        }
    }

    protected abstract List<DataRecord> ImportFromURL(List<DataRecord> Records, string path, int priority = 1);

    protected abstract List<DataRecord> ImportFromFile(List<DataRecord> Records, string path);

    /// <summary>
    /// Exports to the file drive from a download.
    /// Pass in the path to get data typically the url.
    /// Pass in outputPath to specify the output file path
    /// Pass in the outputName to specify the name that will be used for all files that are created. 
    /// </summary>
    /// <param name="Path"></param>
    /// <returns></returns>
    public abstract bool ExportToFile(string Path, string outputPath, string outputName);
}