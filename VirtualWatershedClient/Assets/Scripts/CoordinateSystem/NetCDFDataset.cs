using UnityEngine;
using System.Collections;
using ASA.NetCDF4; // https://github.com/lukecampbell/netcdf4.net
using System;
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

    public float[] GetFloats(NcVar variable)
    {
        float[] Arr = new float[GetTotalSize(variable)];
        variable.GetVar(Arr);
        return Arr;
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
                float min = 0.0f;
                float max = 0.0f;
                var index = new int[] { i, 0, 0 };
                
                Var.GetVar(index, count, arr);

                float[,] arr2 = new float[Var.Shape[0], Var.Shape[1]];
                System.Buffer.BlockCopy(arr, 0, arr2, 0, 4 * total);
                Utilities.findMinMax(arr,ref min,ref max);
                Debug.LogError(min + "_____" + max);
                Utilities.findMinMax(arr2, ref min, ref max);
                Debug.LogError(min + "_____" + max);
                outlist.Add(arr2);
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
        System.DateTime start = new System.DateTime();
        DateTime end = new System.DateTime();
        start = System.DateTime.MinValue;
        end = System.DateTime.MaxValue;
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

            var Lats = GetFloats(Lat);
            var Longs = GetFloats(Lon);
            
            Utilities.findMinMax(Lats, ref miny, ref maxy);
            Utilities.findMinMax(Longs, ref minx, ref maxx);

            boundingBox = minx + "," + miny + "," + maxx + "," + maxy;
            Debug.LogError(boundingBox);
        }

        if(Dims.Contains("time"))
        {
            var Time = FileHandle.GetVar("time");
            var Delta = "days";


            foreach(var Attribute in Time.GetAtts())
            {
                string Att = Attribute.Value.GetValues();
                Debug.LogError("SPLIT: " + Att);
                if (Att.Contains("since"))
                {
                    var splits = Att.Split(new string[] { "since"},System.StringSplitOptions.RemoveEmptyEntries);
                    splits[0] = splits[0].Split(' ')[0];
                    splits[1] = splits[1].Split(' ')[1];

                    Delta = splits[0];
                    try
                    {
                        start = System.DateTime.ParseExact(splits[1], "yyyy-MM-dd:hh:mm:ss", null);
                    }
                    catch
                    {
                        Debug.LogError("Was unable to parse the datetime...");
                    }

                    TimeSpan ts = new TimeSpan();
                    if (splits[0].ToLower() == "hours")
                    {
                        ts = new TimeSpan(Time.Shape[0], 0, 0);
                        //Debug.LogError("HOURS: " + dataset.RasterCount);
                    }
                    else if (splits[0].ToLower() == "days")
                    {
                        ts = new TimeSpan(Time.Shape[0], 0, 0, 0);
                        Debug.LogError("DAYS: " );
                    }
                    else if(splits[0].ToLower() == "years")
                    {
                        ts = TimeSpan.FromDays(365 * Time.Shape[0]);
                    }
                    Debug.LogError(Time.Shape[0] + "TOTAL");
                    end = start + ts;


                    Debug.LogError(ts);
                    Debug.LogError(start);
                    Debug.LogError(splits[1]);
                    Debug.LogError(end);
                    Debug.LogError("======");
                }

            }
            
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
                record.Temporal = false;
                record.start = start;
                record.end = end;
                foreach (var dim in Variable.Value.GetDims())
                {
                    if(dim.GetName().ToLower() == "time")
                    {
                        record.Temporal = true;
                    }
                }
                // Grab the attributes and set their fields
                //record

                // set services
                record.services.Add("nc4", FileName);
            }
        }

        return records;
    }



}
