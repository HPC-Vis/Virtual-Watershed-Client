using UnityEngine;
using System.Collections;

/// <summary>
/// A abstract base class for representing World Objects.
/// A World Object is an object that representing an object that has been placed in the Unity simulation that can
/// be saved, modified, moved, and even give data to other requesting classes.
/// </summary>
public abstract class WorldObject : MonoBehaviour
{

    public DataRecord record;

    public abstract bool saveData(string filename, string format="");

    public abstract bool moveObject(GameObject gameobject, Vector3 displacement);

    public abstract bool changeProjection(string projectionString);

    public abstract SessionObjectStructure saveSessionData();

    // To be left alone for now.
    public abstract void alterData();

    public abstract void getData();


}
