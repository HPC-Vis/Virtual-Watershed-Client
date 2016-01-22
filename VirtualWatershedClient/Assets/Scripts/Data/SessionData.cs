using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;

public class DataObject
{
    public DataObject()
    {

    }
    public DataRecord record;
    public Vector3 position;
    public GameObject gameobject;
}

public class SessionData
{
    ListViewManager listview;

    // Store everything by variable or model run name your choice
    public Dictionary<string, DataObject> SessionObjects = new Dictionary<string,DataObject>();

    public SessionData()
    {

    }

    public int NumberOfObjects()
    {
        Debug.LogError(SessionObjects.Count);
        return SessionObjects.Count;
    }

    public void InsertSessionData(DataRecord dr)
    {
        DataObject Do = new DataObject();

        Do.record = dr;
        if (!SessionObjects.ContainsKey(dr.name))
        {
            SessionObjects[dr.name] = Do;
        }
    }

    public void updatePosition(string name,Vector3 position)
    {
        if (SessionObjects.ContainsKey(name))
        {
            SessionObjects[name].position = new Vector3();
        }
    }

    public void updateGameObject(string name,GameObject gameobject)
    {
        if (SessionObjects.ContainsKey(name))
        {
            SessionObjects[name].gameobject = gameobject;
        }
    }

    public void Clear()
    {
        SessionObjects.Clear();
    }
}
