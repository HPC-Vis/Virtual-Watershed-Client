using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        /*System.GC.Collect();
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
        Debug.LogError("TIME it to load uncompressed with NetCDF4: " + timer.ElapsedMilliseconds / 1000);*/


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
            System.Threading.Thread thread = new System.Threading.Thread(() => LoadNetCDF2());
            //System.Threading.Thread thread2 = new System.Threading.Thread(() => LoadStuff());
            thread.Start();
            //thread2.Start();
            once = true;
        }
	}

    public void LoadNetCDF2()
    {
        bool once2 = false;
        NetCDFDataset ncfile = new NetCDFDataset(CompressedFile);
        if (ncfile.Open())
        {
            List<DataRecord> records = new List<DataRecord>();
            records = ncfile.Parse();
            Debug.LogError(records.Count);
            Debug.LogError(ncfile.GetBoundingBox());
            foreach (var i in records)
            {
                Debug.LogError(i.name);
                if (!once2)
                {
                    Debug.LogError(ncfile.GetVariableData(i.name).Count);
                    once2 = true;
                }
            }
        }
    }

}
