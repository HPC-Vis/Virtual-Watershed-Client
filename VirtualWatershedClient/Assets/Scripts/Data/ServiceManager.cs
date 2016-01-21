using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ServiceManager a class meant to be used for the purposes of handing different server downloads and the various different downloads.
/// </summary>
public class ServiceManager 
{
    // All servers will be a sub class of Observer given the current backend.
    /// <summary>
    /// key is server name
    /// value is the server class
    /// </summary>
    public Dictionary<string, Observer> Servers; 

    public ServiceManager()
    {

    }

    public void AddServer(string name, Observer server)
    {
        Servers.Add(name, server);
    }


    // A download has service and operation.
    // service being the server
    // operation being the service or operation that we want to use out of that server. for acquiring the raw data...
    /// <summary>
    /// 
    /// </summary>
    /// <param name="records"></param>
    /// <param name="SettingTheRecord"></param>
    /// <param name="service"></param>
    /// <param name="operation"></param>
    /// <param name="param"></param>
    public void Download(List<DataRecord> records, DataRecordSetter SettingTheRecord, string service = "vwc", string operation = "wcs", SystemParameters param = null)
    {
        // Create param if one does not exist
        /*if (param == null) { param = new SystemParameters(); }

        // TODO 
        if (service == "vwc")
        {
            if (operation == "fgdc")
            {
                client.GetMetaData(SettingTheRecord, records);
            }
            else
            {
                // Start Thread
                new Thread(() =>
                {

                    foreach (var i in records)
                    {
                        //Debug.LogError(i.name);
                        if (operation == "wms" && i.services.ContainsKey("wms"))
                        {
                            Debug.LogError("WMS");
                            // Lets check if it exists in the cache by uuid
                            if (FileBasedCache.Exists(i.id) && i.texture == null)
                            {
                                i.boundingBox = FileBasedCache.Get<DataRecord>(i.id).boundingBox;
                                i.texture = FileBasedCache.Get<DataRecord>(i.id).texture;
                                i.bbox2 = FileBasedCache.Get<DataRecord>(i.id).bbox2;
                                i.bbox = FileBasedCache.Get<DataRecord>(i.id).bbox;
                                SettingTheRecord(new List<DataRecord> { i });
                                continue;
                            }
                            else if (i.texture != null)
                            {
                                //i.texture = FileBasedCache.Get<DataRecord>(i.id).texture;
                                SettingTheRecord(new List<DataRecord> { i });
                                continue;
                            }

                            if (param.width == 0 || param.height == 0)
                            {
                                param.width = 100;
                                param.height = 100;
                            }
                            client.getMap(SettingTheRecord, i, param);
                        }


                        else if (operation == "wcs" && i.services.ContainsKey("wcs"))
                        {
                            // Lets check if it exists in the cache by uuid
                            // Debug.LogError( "DATA: + " + (i.Data == null).ToString())
                            // Debug.LogError("ID: " + i.id);
                            if (FileBasedCache.Exists(i.id) && i.Data.Count == 0)
                            {
                                // Debug.LogError("Recieved the cache for UUDI: " + i.id);
                                i.Data = FileBasedCache.Get<DataRecord>(i.id).Data;
                                i.bbox2 = FileBasedCache.Get<DataRecord>(i.id).bbox2;
                                i.bbox = FileBasedCache.Get<DataRecord>(i.id).bbox;
                                SettingTheRecord(new List<DataRecord> { i });
                                continue;
                            }
                            else if (i.Data.Count != 0)
                            {
                                //Debug.LogError("IN CACHE: " + FileBasedCache.Exists(i.id) + " Data: " + i.Data.GetLength(0) + " ID: " + i.id);
                                SettingTheRecord(new List<DataRecord> { i });
                                continue;
                            }
                            client.getCoverage(SettingTheRecord, i, param);
                        }
                        else if (operation == "wfs" && i.services.ContainsKey("wfs"))
                        {
                            // Lets check if it exists in the cache by uuid
                            if (FileBasedCache.Exists(i.id) && i.Lines == null)
                            {
                                i.Lines = FileBasedCache.Get<DataRecord>(i.id).Lines;
                                SettingTheRecord(new List<DataRecord> { i });
                                continue;
                            }
                            else if (i.Lines != null)
                            {
                                //i.Lines = FileBasedCache.Get<DataRecord>(i.id).Lines;
                                SettingTheRecord(new List<DataRecord> { i });
                            }
                            Debug.LogError("PRIORITY: " + param.Priority);
                            client.getFeatures(SettingTheRecord, i, param);
                        }
                        else if (i.services.ContainsKey("file"))
                        {
                            Debug.LogError("Loading FIle");
                            RasterDataset rd = new RasterDataset(i.services["file"]);
                            if (rd.Open())
                            {

                                var da = rd.GetData();

                                //temporary patch is gross
                                Spooler.TOTAL = da.Count;

                                for (int j = 0; j < da.Count; j++)
                                {
                                    DataRecord recClone = i.Clone();
                                    recClone.Data.Add(da[j]);
                                    recClone.band_id = j + 1;

                                    var TS = i.end.Value - i.start.Value;
                                    double totalhours = TS.TotalHours / da.Count;
                                    recClone.start += new TimeSpan((int)Math.Round((double)j * totalhours), 0, 0);
                                    recClone.end = recClone.start + new TimeSpan((int)Math.Round(totalhours), 0, 0);
                                    SettingTheRecord(new List<DataRecord> { recClone });
                                }
                            }
                        }
                        else if (i.services.ContainsKey("nc"))
                        {
                            Debug.LogError("NCNESSS!!!!");
                        }
                    }
                }).Start();
                // End Thread
            }
        }*/
    }


}
