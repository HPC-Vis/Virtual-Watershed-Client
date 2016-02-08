using UnityEngine;
using System.Collections;

public abstract class WorldObject : MonoBehaviour
{

    public DataRecord record;

    public abstract void saveData();

    public abstract void moveObject();

    public abstract void alterData();

    public abstract void getData();
}
