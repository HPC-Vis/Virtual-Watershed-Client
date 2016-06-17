using UnityEngine;
using System.Collections;
using ASA.NetCDF4; // https://github.com/lukecampbell/netcdf4.net
using System.Collections.Generic;

public class NetCDFDataset : FileDataset
{
    NcFile FileHandle;
    string projection = "";
    bool IsGeospatial = false;
    public string boundingBox = "";

    public NetCDFDataset(string filename)
    {
        FileName = filename;
    }

    public override bool Open()
    {
        FileHandle = new NcFile(FileName, NcFileMode.read);
        return !FileHandle.IsNull();
    }

    int GetTotalSize(NcVar variable)
    {
        int product = 1;
        for(int i = 0; i < variable.Shape.Length; i++)
        {
            product *= variable.Shape[i];
        }

        return product;
    }

    public float[,] GetFloats(NcVar variable,int index,int count , int width, int height)
    {
        float[] arr = new float[count];
        float[,] arr2 = new float[width,height];
        
        variable.GetVar(new int[] { index }, new int[] { count } , arr);
        //System.Buffer.BlockCopy(arr)
        return arr2;
    }

    public double[] GetDoubles(NcVar variable)
    {
        double[] arr = new double[GetTotalSize(variable)];
        variable.GetVar(arr);
        return arr;
    }

    public int[] GetInts(NcVar variable)
    {
        int[] arr = new int[GetTotalSize(variable)];
        variable.GetVar(arr);
        return arr;
    }

    //public GetData()

    public List<float[,]> GetVariableData(string VariableName)
    {
        Debug.LogError("CRINGE!!! WORKS");
        List<float[,]> outlist = new List<float[,]>();
        var Var = FileHandle.GetVar(VariableName);
        //var Floats = GetFloats(Var);
        for (int i = 0; i < Var.Shape.Length; i++)
        {
            Debug.LogError(Var.Shape[i]);
        }

        Debug.LogError(Var.GetName());
        Debug.LogError(Var.GetNcType().GetTypeClassName());
        var fc = new int[5]; // new float[5];
        var index = new System.Int32[] { 0,0,0};
        //var counts = new System.Int32[] { 1,1,1 };
        Var.CheckData();
        //Var.GetVar();
        Var.GetVar(index, fc);
        

        return outlist;
    }

    public override string GetBoundingBox()
    {
        return boundingBox;
    }

    public override List<DataRecord> Parse()
    {
        float maxx = float.MinValue, maxy = float.MinValue, minx = float.MaxValue, miny = float.MaxValue;
        List<DataRecord> records = new List<DataRecord>();

        List<string> Dims = new List<string>();

        // Get Dims first
        foreach (var i in FileHandle.GetDims())
        {
            Dims.Add(i.Key);
        }

        // Gether dimension values...

        // check for latitude and longitude -- calculate bounding box
        if(Dims.Contains("lon") && Dims.Contains("lat") )
        {
            IsGeospatial = true;
            var Lat = FileHandle.GetVar("lat");
            var Lon = FileHandle.GetVar("lon");

            //var Lats = GetFloats(Lat);
            //var Longs = GetFloats(Lon);
            
            //Utilities.findMinMax(Lats, ref miny, ref maxy);
            //Utilities.findMinMax(Longs, ref minx, ref maxx);

            boundingBox = minx + "," + miny + "," + maxx + "," + maxy;
        }


        // Determine the variables
        var Variables = FileHandle.GetVars();

        if (!FileHandle.GetVar("crs").IsNull())
        {
            projection = FileHandle.GetVar("crs").GetAtt("spatial_ref").GetValues();
        }

        foreach (var Variable in Variables)
        {
            string name = Variable.Value.GetName();

            // Check if variable is dim ..
            if ( name != "lat" && name != "lon" && name != "crs" && name != "time" )
            {
                DataRecord record = new DataRecord(name);

                record.id = System.Guid.NewGuid().ToString();
                record.projection = projection;

                record.bbox2 = record.bbox = boundingBox;

                records.Add(record);

                // Grab the attributes and set their fields
                //record

                // set services
                record.services.Add("nc4", FileName);
            }
        }

        return records;
    }



}
