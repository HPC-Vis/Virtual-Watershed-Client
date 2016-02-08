using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;

public class SessionData
{
    ListViewManager listview;

    // Store everything by variable or model run name your choice
    public Dictionary<string, WorldObject> SessionObjects = new Dictionary<string,WorldObject>();

    public SessionData()
    {

    }

    public int NumberOfObjects()
    {
        return SessionObjects.Count;
    }

    public void InsertSessionData(WorldObject worldObject)
    {
        if (!SessionObjects.ContainsKey(worldObject.record.name))
        {
            SessionObjects[worldObject.record.name] = worldObject;
        }
    }

    public void Clear()
    {
        SessionObjects.Clear();
    }


    public WorldObject GetSessionObject(string name)
    {
        return SessionObjects[name];
    }

}
