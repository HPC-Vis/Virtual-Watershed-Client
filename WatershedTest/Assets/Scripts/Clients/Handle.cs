using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// This for handling one variable or a set of related data records that are related.
/// </summary>
public class Handle
{
    // We need someway of building multiple clients.
    // Ouch!!!!

    List<DataRecord> records;
    string service;
    string operation;
    
    public Handle(List<DataRecord> recs, string serv = "vwc", string oper = "wcs")
    {
        if(recs == null || recs.Count == 0)
        {
            throw new Exception("Error you passed in zero or null records");
        }
        if(recs.Count >= 1)
        {
            foreach(var i in recs)
            {
                if (i.numbands > 1)
                {
                    throw new Exception("Error there must be only one band in this datarecord. ");
                }
            }
        }
        records = recs;
        Service = serv;
        Operation = oper;
    }

    // We may need to redesign the service and operation fields a bit.
    public string Service
    {
        get { return service; }
        set { service = value; }
    }

    public string Operation
    {
        get { return operation; }
        set { operation = value; }
    }

    // A list of datarecords is assumed to be single entities
    public int NumRecords()
    {
        if(records.Count > 0)
        {
            return records.Count;
        }
        return records[0].numbands;
    }

    // Download functions -- We need some way specifing the service!
    void Download(DateTime dt)
    {

    }

    void Download(int index)
    {

    }


    // The function to download datarecords
    void HandleDownload()
    {

    }

    // Clean up functions

    // Remove the nearest datarecord with date -- blash
    void Dispose(DateTime dt)
    {

    }

    // Remove the data records that with the certain index
    void Dispose(int index)
    {

    }
}
