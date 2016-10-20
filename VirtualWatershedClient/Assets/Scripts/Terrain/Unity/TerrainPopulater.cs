using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using VTL;
using VTL.ListView;
using VTL.ProceduralTerrain;
public class TerrainPopulater : MonoBehaviour
{
    public ListViewManager TerrainList;
    List<object[]> Lists = new List<object[]>();
    List<string> PreviousLists = new List<string>();
    List<DataRecord> Records = new List<DataRecord>();
    string TerrainListStr = "terrainlist";
    public GameObject Terrains;

    public System.Net.WebClient client = new System.Net.WebClient();
    public FileBrowse fb = new FileBrowse();
    public FileListViewManager fileview;

    Texture2D LoadSatelliteImage(string bbox, int width = 1024, int height= 1024)
    {
        Texture2D temp = new Texture2D(width, height);
        bbox = bbox.Replace(' ', ',');
        
        try
        {
            var bytes = client.DownloadData("http://irs.gis-lab.info/?layers=landsat&SERVICE=WMS&format=image/png&" + "bbox=" + bbox + "&width=" + width + "&height=" + height);
            if (bytes != null)
            {
                temp.LoadImage(bytes);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Satellite Imagery Failed: " + e.Message);
        }
        return temp;
    }

    void GetTerrainList(List<DataRecord> Terrains)
    {
        Records = Terrains;
        foreach (var rec in Terrains)
        {
            Logger.Log("Got the record named: " + rec.name);
            if (!PreviousLists.Contains(rec.name) && !Lists.Contains(new object[]{rec.name,
                rec.location}))
            {
                Lists.Add(new object[]{rec.name,
                rec.location});
            }
        }
        //return;
        if ( !FileBasedCache.Exists(TerrainListStr))
        {
            FileBasedCache.Insert<List<DataRecord>>(TerrainListStr, Terrains);
        }
        else
        {
            var DEMs = FileBasedCache.Get<List<DataRecord>>(TerrainListStr);
            /// Merge Collections
            foreach (var i in DEMs)
            {
                if (!Terrains.Contains(i))
                {
                    Terrains.Add(i);
                }
            }
            FileBasedCache.Insert<List<DataRecord>>(TerrainListStr, Terrains);
        }
    }

    public ToggleObjects to;
    VWClient vwc;
    NetworkManager nm;
    List<DataRecord> recordToBuild = new List<DataRecord>();
    public Texture2D BaseMap;
    // Use this for initialization
    void Start()
    {
        // Gdal Initialization is needed here...
        OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DATA", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\data");
        System.Environment.SetEnvironmentVariable("GDAL_DATA", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\data\");

        if (ModelRunManager.client == null)
        {
            Logger.WriteLine("Does not have client.");
            nm = new NetworkManager();
            vwc = new VWClient(new DataFactory(nm), nm);
            nm.Subscribe(vwc);
            ModelRunManager.client = vwc;
        }
        else
        {
            Debug.LogError("Does Have Client");
            vwc = ModelRunManager.client;
        }
        // Start downloading terrains
        SystemParameters sp = new SystemParameters();
        sp.query = "DEM";
        sp.limit = 100;
        sp.offset = 0;
        vwc.RequestRecords(GetTerrainList, sp);

        System.GC.Collect();
    }

    // Update is called once per frame
    void Update()
    {
        //var jobs = nm.CurrentJobs();
        //for(int i = 0; i < jobs.Count; i++)
        //{
        //    Debug.LogError(i + " " + jobs[i]);
        //}
        if (Lists.Count > 0)
        {
            for (int i = 0; i < Lists.Count; i++)
            {
                if (!PreviousLists.Contains((string)Lists[i][0]))
                {
                    PreviousLists.Add((string)Lists[i][0]);
                    TerrainList.AddRow(Lists[i], Records[i]);
                }
            }
            Records.Clear();
            Lists.Clear();
        }
        if (recordToBuild.Count > 0)
        {
            LoadTerrain(recordToBuild[0]);
            recordToBuild.Clear();
        }
    }

    // Assuming that bbox part of datarecord is lat long...
    // Consequently our globalprojection needs to be a utm projection
    public void LoadTerrain(DataRecord record)
    {
        // Cache the record
        if (!FileBasedCache.Exists(record.name))
        {
            FileBasedCache.Insert<DataRecord>(record.name, record);
        }

        record.boundingBox = new SerialRect(Utilities.bboxSplit3(record.bbox2));
        //WorldTransform tran = new WorldTransform(record.projection);

        Rect abc = record.boundingBox;
        int zone = CoordinateUtils.GetZone(abc.center.y, abc.center.x);

        // EPSG code
        string EPSG = record.projection.Replace("epsg:", "");
        GlobalConfig.GlobalProjection = int.Parse(EPSG);
        GlobalConfig.GlobalProjection = 26900 + zone;

        GlobalConfig.Zone = zone;//CoordinateUtils.GetZone(record.boundingBox.y, record.boundingBox.x);

        //tran.createCoordSystem(record.projection);
        OSGeo.OSR.SpatialReference source = new OSGeo.OSR.SpatialReference("");
        source.ImportFromEPSG(int.Parse(EPSG));

        OSGeo.OSR.SpatialReference dest = new OSGeo.OSR.SpatialReference("");
        dest.ImportFromEPSG(GlobalConfig.GlobalProjection);

        var tran = coordsystem.createTransform(source, dest);
        
        // Calculate UTM boundingbox
        try
        {
            Vector2 point = CoordinateUtils.TransformPoint(tran,new Vector2(record.boundingBox.x, record.boundingBox.y));
            Vector2 upperLeft = CoordinateUtils.TransformPoint(tran, new Vector2(record.boundingBox.x, record.boundingBox.y));
            //Vector2 upperRight = CoordinateUtils.TransformPoint(tran, new Vector2(record.boundingBox.x + record.boundingBox.width, record.boundingBox.y));
            // float Distance = (float)CoordinateUtils.SimponsRule(record.boundingBox.x, record.boundingBox.y, record.boundingBox.x + record.boundingBox.width, record.boundingBox.y - record.boundingBox.height, CoordinateUtils.GetZone(record.boundingBox.y, record.boundingBox.x));
            //Vector2 lowerRight = upperLeft + new Vector2(record.boundingBox.width,-record.boundingBox.height).normalized *Distance;
            Vector2 lowerRight = CoordinateUtils.TransformPoint(tran, new Vector2(record.boundingBox.x + record.boundingBox.width, record.boundingBox.y - record.boundingBox.height)); ;
            //Vector2 lowerLeft = CoordinateUtils.TransformPoint(tran, new Vector2(record.boundingBox.x, record.boundingBox.y - record.boundingBox.height));

            // Calculate average xres
            //Debug.LogError("UPPER RIGHT: " + upperRight);
            GlobalConfig.BoundingBox = new Rect(upperLeft.x, upperLeft.y, Mathf.Abs(upperLeft.x - lowerRight.x), Mathf.Abs(upperLeft.y - lowerRight.y));
            //GlobalConfig.BoundingBox = new Rect(upperLeft.x, upperLeft.y, CoordinateUtils.SimpleRuleXDist(record.boundingBox.x, record.boundingBox.y, record.boundingBox.x + record.boundingBox.width, record.boundingBox.y - record.boundingBox.height, CoordinateUtils.GetZone(record.boundingBox.y, record.boundingBox.x)), Mathf.Abs(upperLeft.y - lowerRight.y));
            float XRes = GlobalConfig.BoundingBox.width / record.Data[0].GetLength(0);
            float YRes = GlobalConfig.BoundingBox.height / record.Data[0].GetLength(1);
            //StartupConfiguration.LoadConfig();
            record.Data[0] = Utilities.reflectData(record.Data[0]);
            GlobalConfig.Location = record.location;

            BaseMap = LoadSatelliteImage(record.bbox2);
            var GO = ProceduralTerrain.BuildTerrain(record.Data[0], XRes, YRes, BaseMap);
            GO.transform.position = new Vector3(-GlobalConfig.BoundingBox.width / 2, 0, -GlobalConfig.BoundingBox.height / 2);

            var heightmap = TerrainUtils.GetHeightMapAsTexture(record.Data[0]);
            var bytesofBasemap = heightmap.EncodeToPNG();
            File.WriteAllBytes(raySlicer.ImageLoc, bytesofBasemap);

            GO.transform.SetParent(Terrains.transform);

            to.toggleObjects();
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }

    public void loadTerrain(List<DataRecord> Record)
    {
        // Debug.LogError ("SETTING THE TERRAIN");
        recordToBuild.Add(Record[0]);
    }

    public void onDestroy()
    {
        nm.Halt();
    }

    public void LoadSelected()
    {
        var Runs = TerrainList.GetSelected();
        if (Runs.Count > 0)
        {
            SystemParameters sp = new SystemParameters();
            sp.width = 1025;
            sp.height = 1025;
            if (FileBasedCache.Exists(Runs[0].name))
            {
                loadTerrain(new List<DataRecord> { FileBasedCache.Get<DataRecord>(Runs[0].name) });
            }

            ModelRunManager.Download(new List<DataRecord> { Runs[0] }, loadTerrain, param: sp);
        }
    }

    public void ActivateFileListView()
    {

        fileview.gameObject.SetActive(true);
        fileview.SetSearchPattern(new string[] { "tif", "dem" });
        fileview.action = LoadDemFromFile;
        TerrainList.gameObject.SetActive(false);
    }

    /// <summary>
    /// Assumptions in this function are that we are using gdal.
    /// The data has a projection..
    /// Data will be reprojected into epsg:4326... by rasterdataset.
    /// </summary>
    public void LoadDemFromFile()
    {
        string file = fileview.GetCurrentSelection();
        // Loading Dem.
        if (file != "" || file != null)
        {
            Debug.Log("Loading DEM");
            RasterDataset Dem = new RasterDataset(file);
            if(Dem.Open())
            {
                DataRecord record = new DataRecord("somename.tif");
                string boundingbox = Dem.GetBoundingBox();
                var Data = Dem.GetData(513,513);
                

                record.bbox = record.bbox2 = boundingbox;
                record.Data = Data;
                record.projection = "epsg:4326";
                LoadTerrain(record);
                //coordsystem.createTransform(new OSGEOProjection
            }
        }
    }
}
