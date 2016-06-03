using UnityEngine;
using System.Collections;
using ASA.NetCDF4;

public class NetCDFTests : MonoBehaviour {
    // compressed file
    string CompressedFile = @"C:\Users\ccarthen\Downloads\animation.nc";

    // uncompressed file
    string UncompressedFile = @"C:\Users\ccarthen\Downloads\animation2.nc";

    public string ToNetCDFFile(string filepath)
    {
        return "NETCDF:" + '"' + filepath + '"';
    }

    // Use this for initialization
    void Start() {
        System.GC.Collect();
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        LoadNetCDF2(CompressedFile);
        timer.Stop();
        Debug.LogError("TIME it to load compressed with NetCDF4: " + timer.ElapsedMilliseconds / 1000);

        System.GC.Collect();
        timer.Reset();
        timer.Start();
        LoadNetCDF2(UncompressedFile);
        timer.Stop();
        Debug.LogError("TIME it to load uncompressed with NetCDF4: " + timer.ElapsedMilliseconds / 1000);


    }

    public void LoadStuff()
    {
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        System.GC.Collect();
        timer.Reset();
        
        OSGeo.GDAL.Gdal.AllRegister();
        RasterDataset rd = new RasterDataset(ToNetCDFFile(UncompressedFile));
        rd.Open();

        timer.Start();
        string sub = rd.GetSubDatasets()[1];
        rd = new RasterDataset(sub);

        if (rd.Open())
        {
            rd.GetData();
        }
        timer.Stop();

        Debug.LogError("TIME it to load uncompressed with GDAL: " + timer.ElapsedMilliseconds / 1000);
    }

    bool once = false;
	// Update is called once per frame
	void Update () {
        if (!once)
        {
            System.Threading.Thread thread = new System.Threading.Thread(() => LoadStuff());
            thread.Start();
            once = true;
        }
	}

    public void LoadNetCDF2(string filename)
    {
        NcFile file = new NcFile(filename, NcFileMode.read);

        Debug.LogError("FILENESS");
        bool once = true;
        var nothing = 1;
        Debug.LogError(nothing);
        foreach (var i in file.GetAtts())
        {
            Debug.LogError(i.Value.GetName());
        }

        foreach (var i in file.GetDims())
        {
            Debug.LogError(i.Value.GetName());
        }

        var vars = file.GetVars();
        Debug.LogError("--------");
        foreach (var i in vars)
        {
            if (!i.Value.IsNull())
            {
                if (i.Value.GetNcType() != null && i.Value.GetNcType().GetTypeClass() == NcTypeEnum.NC_DOUBLE && once && i.Value.Shape.Length > 1)
                {
                    //float[] values = new float[1];
                    //i.Value.GetVar(new int[] { 1},values);
                    //i.Value.Shape
                    Debug.LogError(i.Value.GetName());
                    int length = 1;
                    for (int j = 0; j < i.Value.Shape.Length; j++)
                    {
                        Debug.LogError("SHAPE: " + i + " _ " + i.Value.Shape[j].ToString());
                        length *= i.Value.Shape[j];
                    }
                    var doubleness = new double[length];
                    i.Value.GetVar(doubleness);
                    doubleness = null;
                    //Debug.LogError(data.GetValueAt(new int[] { 0,0,0}));
                    //data = null;
                    //Debug.LogError(values[0]);
                    // Debug.LogError(i.Value.GetName());
                    // Debug.LogError(values[0]);
                    once = false;
                }
                else if (i.Value.GetNcType() != null)
                {
                    //string[] values = new string[i.Value.GetVar().Length];
                    //var v = i.Value.GetVar();
                    //Debug.LogError("ATTR: " + i.Value.GetName());
                    Debug.LogError(i.Value.GetNcType().GetTypeClassName());
                    //Debug.LogError(i.Value.GetAttCount());
                    foreach (var k in i.Value.GetAtts())
                    {
                        // k.Value.GetValues()
                        //Debug.LogError(k.Value.GetValues());
                    }
                    //i.Value.GetVar(values);
                    //Debug.LogError(values.Length);
                    //Debug.LogError(i.Value.GetName());
                    //Debug.LogError(values[0]);

                }
            }
        }
        //Debug.LogError(file.GetVarCount());
        //NCFile file = new NNcFileMode.read
        file.Close();
        //file = null;
        Debug.LogError(file.IsNull());
        System.GC.Collect();
    }
}
