using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using VTL.ListView;
using SimpleJson;

[Serializable]
public class SessionObjectStructure
{
    public string Name;
    public string DataLocation;
    public SerialVector3 GameObjectPosition;
}

[Serializable]
public class SessionDataStructure
{
    public string Location;
    public SerialVector3 PlayerPosition;

    public Dictionary<string,SessionObjectStructure> GameObjects;
}

/// <summary>
/// The Session Data class is meant to hold any data that is added and *loaded* into the Unity Session.
/// </summary>
public class SessionData
{
    ListViewManager listview;

    public Vector3 PlayerPosition;
    public string Location;

    // json specification -- that may be turned into a schema later.
    // {
    // Player Position
    // Location
    // list of data in scene
    // [
    // Gameobject
    // {
    // name -- name of the data to be placed into the session
    // position -- where was this data located
    // original dataset local: url or file
    // }
    //
    // }

    // Store everything by variable or model run name your choice
    public Dictionary<string, WorldObject> SessionObjects = new Dictionary<string,WorldObject>();

    public SessionData()
    {

    }

    public void SaveSessionData(string filename)
    {
        SessionDataStructure dataStructure = new SessionDataStructure();
        dataStructure.PlayerPosition = PlayerPosition;
        dataStructure.Location = Location;
        foreach (var pair in SessionObjects)
        {
             dataStructure.GameObjects[pair.Key] = pair.Value.saveSessionData();
        }
        string jsonstring = Newtonsoft.Json.JsonConvert.SerializeObject(dataStructure);

        // Save the file out
        String pathDownload = Utilities.GetFilePath(filename);
        using (StreamWriter file = new StreamWriter(@pathDownload))
        {
            file.Write(jsonstring);
        }
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
