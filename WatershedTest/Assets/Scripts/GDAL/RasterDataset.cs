using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using OSGeo.GDAL;

// Gdal class for pulling data out
public class RasterDataset
{
    string File;
    Dataset dataset = null;
    List<string> Subdatasets = new List<string>();
    int MAX_WIDTH = 1024, MAX_HEIGHT = 1024;
    public RasterDataset(string file)
    {
        File = file;
    }
    public bool Open()
    {
        Gdal.AllRegister();
        System.Console.WriteLine(File);
        try 
        {
            dataset = Gdal.Open(File, Access.GA_ReadOnly);
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
        return false;
    }

    public bool HasSubDatasets(out int numsubdatasets)
    {
        numsubdatasets = 0;
        var sub = dataset.GetMetadata("SUBDATASETS");

        if(sub != null)
        {
            populateSubDatasets(sub);
        }
        numsubdatasets = Subdatasets.Count;
        return sub != null;
    }

    public List<string> GetSubDatasets()
    {
        if(Subdatasets.Count == 0)
        {
            var sub = dataset.GetMetadata("SUBDATASETS");
            if (sub != null)
            {
                populateSubDatasets(sub);
            }
        }
        return Subdatasets;
    }

    void populateSubDatasets(string[] SubMetadata)
    {
        // Parse the subdatasets strings
        int count = SubMetadata.Length;
        for (int i = 0; i < count; i += 2)
        {
            System.Console.WriteLine(i);
            Subdatasets.Add(SubMetadata[i].Replace("SUBDATASET_" + ((i / 2)+1) + "_NAME=", ""));
            System.Console.WriteLine(SubMetadata[i].Replace("SUBDATASET_"+ ((i/2)+1) + "_NAME=",""));
        }
    }

    public List<float[,]> GetData()
    {
        List<float[,]> data = new List<float[,]>();
        int width = dataset.RasterXSize;
        int height = dataset.RasterYSize;
        width = Math.Min(width, MAX_WIDTH);
        height = Math.Min(height, MAX_HEIGHT);
        float[] DataF = new float[width*height];
        Console.WriteLine(width + " " + height);
        for(int i =0;i < dataset.RasterCount; i++)
        {
            float[,] Data = new float[width, height];
            var band = dataset.GetRasterBand(i + 1);
            try
            {
                band.ReadRaster(0, 0, width, height, DataF, width, height, 0, 0);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return data;
            }

            for(int k = 0; k < width; k++)
            {
                for(int j = 0; j < height; j++)
                {
                    Data[k, j] = DataF[k * height + j];
                }
            }

            data.Add(Data);
        }
        return data;
    }
}
