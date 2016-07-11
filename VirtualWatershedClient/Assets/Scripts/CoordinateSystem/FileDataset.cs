using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A base class for classes such as NetCDF Dataset and Raster Dataset.
/// A future consideration is to add Writing to this generic class
/// </summary>
public class FileDataset : Parser
{
    public bool IsOpen = false;
    public int Progress;
    public int Total;

    public string ModelRunUUID = "";
    public string FileName;

    public void SetModelRunUUID(string someUniqueCommonIdentifier)
    {
        ModelRunUUID = someUniqueCommonIdentifier;
    }

    public virtual bool Open()
    {
        throw new System.NotImplementedException();
    }
    
    public virtual string GetBoundingBox()
    {
        throw new System.NotImplementedException();
    }

    public virtual string GetProjection()
    {
        throw new System.NotImplementedException();
    }

    public override List<DataRecord> Parse(string Path)
    {
        return Parse();
    }

    public virtual List<DataRecord> Parse()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// A function to determine how far this loader has progressed in loading a particular dataset.
    /// </summary>
    /// <returns>The percentage finished by this class in loading stuff.</returns>
    public virtual float CurrentProgress()
    {
        return Progress / Total;
    }

}
