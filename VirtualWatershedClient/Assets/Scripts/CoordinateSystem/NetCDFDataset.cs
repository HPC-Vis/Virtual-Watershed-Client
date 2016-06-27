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

        List<float[,]> outlist = new List<float[,]>();
        var Var = FileHandle.GetVar(VariableName);

        if(Var.Shape.Length < 3)
        {
            int total = 1;
            for(int i =0;i < Var.Shape.Length; i++)
            {
                total *= Var.GetShape()[i];
            }

            float[] arr = new float[total];
            Var.GetVar(arr);
            if (Var.Shape.Length == 1)
            {
                float[,] arr2 = new float[1, total];
                System.Buffer.BlockCopy(arr, 0, arr2, 0, 4*total);
            }
            else
            {
                float[,] arr2 = new float[Var.Shape[0], Var.Shape[1]];
                System.Buffer.BlockCopy(arr, 0, arr2, 0, 4 * total);
            }
        }
        else if(Var.Shape.Length == 3)
        {
            outlist = new List<float[,]>(Var.Shape[0]);
            int total = 1;
            for (int i = 1; i < Var.Shape.Length; i++)
            {
                total *= Var.GetShape()[i];
            }

            var arr = new float[total];
            
            var count = new int[] { 1,Var.GetShape()[1],Var.GetShape()[2] };

            for (int i = 0; i < Var.GetShape()[0]; i++)
            {
                Debug.LogError("DONE: " + i);
                var index = new int[] { i, 0, 0 };
                
                Var.GetVar(index, count, arr);

                //float[,] arr2 = new float[Var.Shape[0], Var.Shape[1]];
                //System.Buffer.BlockCopy(arr, 0, arr2, 0, 4 * total);
                //outlist.Add(arr2);
            }
        }
        else
        {
            throw new System.Exception("Anything beyond 3D is not supported for netcdf");
        }
        

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
