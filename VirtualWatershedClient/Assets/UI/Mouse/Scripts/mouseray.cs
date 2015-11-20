using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mouseray : MonoBehaviour
{
    public static List<GameObject> IgnoredObjects = new List<GameObject>();
    public static float slicerMinScale = 10;
    public static float slicerDistanceScaleFactor = 20; // larger is smaller, smaller is larger
    public static Vector3 CursorPosition;

    // Use this for initialization
    void Start()
    {
    }
  
    /// <summary>
    /// raycastHit
    /// Description: Casts a ray at a specified screen position and returns the closest intersection.
    /// </summary>
    /// <param name="position">A screen position.</param>
    /// <returns>Returns an intersection with a Unity Object.</returns>
    public static Vector3 raycastHit(Vector3 position)
    {
        Camera camera = Camera.main;
        float range = float.MaxValue;
        bool found = false;
        RaycastHit closestObject = new RaycastHit();
        /// Cast a ray into the screen at the specified position
        Ray ray = camera.ScreenPointToRay(position);

        // Get all the intersections
        RaycastHit[] raycasts = Physics.RaycastAll(ray);
        
        // Search the intersections
        foreach (RaycastHit i in raycasts)
        {

            bool ignore = false;
            // Check if distance is greater than current range 
            if (i.distance < range && !ignore)
            {
                range = i.distance;
                closestObject = i;
                found = true;
            }
        }
        //case: no object intersection found
        if (!found)
        {
            //return the passed in position
            return position;
        }
        //return the position of the raycast intersection
        return closestObject.point;
    }

    /// <summary>
    /// RaycastHitFurtherest
    /// Descripton: Casts a ray at position based on a specified direction 
    /// and finds the highest point on a terrain.
    /// ypoint is the starting position for the ray.
    /// </summary>
    /// <param name="position"> The position to start casting a ray at. </param>
    /// <param name="direction"> The direction that the ray is projected towards.</param>
    /// <param name="ypoint"> The starting height for the ray. </param>
    /// <returns>The position of intersection. </returns>
    public static Vector3 raycastHitFurtherest(Vector3 position, Vector3 direction, float ypoint = -1000)
    {
        // Variable declaration
        float tempheight = position.y;
        position.y = ypoint;
        float range = float.MinValue; // this variable will be maximized
        bool found = false;
        RaycastHit closestObject = new RaycastHit();

        // Do the raycasting.
        RaycastHit[] raycasts = Physics.RaycastAll(position, direction);
        
        // Search through all raycasts for a possible match
        foreach (RaycastHit i in raycasts)
        {
            // Check ignore list 
            bool ignore = false;
            if (IgnoredObjects != null)
            {
                for (int j = 0; j < IgnoredObjects.Count && !ignore; j++)
                {
                    ignore = i.transform.gameObject.name == IgnoredObjects[j].name;
                }
            }

            // Check if distance is greater than current range and check if the object has terrain
            if (i.distance > range && i.transform.gameObject.GetComponent<Terrain>() != null && !ignore)
            {
                range = i.distance;
                closestObject = i;
                found = true;
            }
        }
        if (!found)
        {
            position.y = tempheight;
            return position;
        }
 
        return closestObject.point;
    }

    // This function will rotate a 2D vector.
    Vector2 rotate(Vector2 vec, float radians)
    {
        return new Vector2(Mathf.Cos(radians) * vec.x - vec.y * Mathf.Sin(radians), Mathf.Sin(radians) * vec.x + vec.y * Mathf.Cos(radians));
    }

    float invertangle(Vector3 a, Vector3 b, float angle)
    {
        var cross = Vector3.Cross(a, b);
        if (cross.y < 0)
            angle = -angle;
        return angle;
    }

    bool checkLineDistanceFromPoint(Vector3 a, Vector3 b, Vector3 point, float distance = 10.0f)
    {
        a.y = b.y = point.y = 0;
        Vector3 sidea = a - b;
        Vector3 sideb = point - a;
        Vector3 sidec = point - b;
        float ang = Vector3.Angle(-sidea, sideb);
        float ang2 = Vector3.Angle(-sideb, -sidec);
        float ang3 = Vector3.Angle(sidea, sidec);
      
        ang = invertangle(-sidea, sideb, ang);

        float bang = ang;
        ang *= (Mathf.PI / 180.0f);
        

        if (Mathf.Abs(sideb.magnitude * Mathf.Sin(ang)) >= distance || Mathf.Abs(bang) > 120.0f || ang3 > 120.0f)
            return false;

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //MOVE TO DATASELECT CLASS!
    void getPointInformation()
    {
        // Query the data of the dataset class
        // One function needed in the dataset class
    }

}
