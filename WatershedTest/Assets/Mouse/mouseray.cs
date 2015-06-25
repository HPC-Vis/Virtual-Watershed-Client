using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mouseray : MonoBehaviour
{
    public mouselistener ml;
    Camera camera;
    public GameObject cursor;

    public GameObject marker1, marker2;
    //GameObject TheTrailer;

    public List<GameObject> markers = new List<GameObject>();
    Vector3 curpos;
    float timecount;

    public static GameObject[] IgnoredObjects;
    public GameObject NoClipGhostPlayer;
    public GameObject FirstPersonControllerPlayer;
    public GameObject cursorObject;
    public GameObject csvButton;

    public raySlicer rs;

	// Query this position for the world position for checking if the cursor is colliding with a bounding
	public static Vector3 CursorWorldPos = Vector3.zero;

//    Vector3 TrailerPosition;
//    public Material trailMaterial;


    public SlicerPlane slicerPlane;

    float slicerMinScale = 10;
    float slicerDistanceScaleFactor = 20; // larger is smaller, smaller is larger

    // Use this for initialization
    void Start()
    {
        IgnoredObjects = new GameObject[]{ NoClipGhostPlayer,FirstPersonControllerPlayer, cursorObject, marker1, marker2};
        Vector3 Pos = mouseray.raycastHitFurtherest(Vector3.zero, Vector3.up, -10000);
        Pos.y += 50;
        NoClipGhostPlayer.transform.position = Pos;
        csvButton = GameObject.Find("SlicertoCSV");
        ////change model
        //TheTrailer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //TheTrailer.name = "THE TRAILER";
        //var tr = TheTrailer.AddComponent<TrailRenderer>();
        //tr.endWidth = tr.startWidth = 2;
        ////tr.sharedMaterials[0] = trailMaterial;
        ////tr.materials[0] = trailMaterial;
        //tr.material = trailMaterial;
        //Physics.IgnoreCollision(TheTrailer.GetComponent<Collider>(), FirstPersonControllerPlayer.GetComponent<Collider>());


		
	}

    bool previous;
    void activateCursor(bool val)
    {
        if (val != previous)
        {
            cursor.SetActive(val);
            previous = val;
        }
    }

    void setCursor(Vector3 worldp)
    {
        cursor.transform.position = worldp;
		CursorWorldPos = worldp;
        //cursor.transform.eulerAngles = Vector3.zero;
        //Vector3 normal = tm.samepleInterpolatedNormal(worldp);
        //float normAngle = Vector3.Angle (new Vector3(0,1,0),normal);
        //Vector3 rotAxis = Vector3.Cross (new Vector3(0,1,0), normal);
        //cursor.transform.Rotate(rotAxis,normAngle);
        //cursor.transform.RotateAround(worldp,rotAxis,normAngle);
    }
    
    public static Vector3 raycastHit(Vector3 position)
    {
        Camera camera = Camera.main;
        // Raycast check
        Ray ray = camera.ScreenPointToRay(position);
        RaycastHit[] raycasts = Physics.RaycastAll(ray);

        RaycastHit closestObject = new RaycastHit();
        float range = float.MaxValue;
        bool found = false;
        foreach (RaycastHit i in raycasts)
        {
            bool ignore = false;

            if (i.transform.gameObject.name == "Sphere")
            {
                ignore = true;
            }
            if (i.distance < range && !ignore)
            {
                range = i.distance;
                closestObject = i;
                found = true;
            }
        }
        if (!found)
        {
            //Debug.LogError("NOT FOUND");
            return position;
        }
        //Debug.LogError(closestObject.point + " " + position);
        return closestObject.point;
    }

    public static Vector3 raycastHitFurtherest(Vector3 position, Vector3 direction, float ypoint = -1000)
    {
        //Camera camera = Camera.main;
        // Raycast check
        //Ray ray = camera.ScreenPointToRay(position);
        float temp = position.y;
        position.y = ypoint;

        RaycastHit[] raycasts = Physics.RaycastAll(position, direction);

        RaycastHit closestObject = new RaycastHit();
        float range = float.MinValue;
        bool found = false;
        foreach (RaycastHit i in raycasts)
        {
            bool ignore = false;
            if (IgnoredObjects != null)
                for (int j = 0; j < IgnoredObjects.Length; j++)
                {
                    //Debug.LogError(i.transform.gameObject.name);
                    ignore = i.transform.gameObject.name == IgnoredObjects[j].name;
                    if (ignore)
                        break;
                }
            //print(i.distance);
            if (i.distance > range && i.transform.gameObject.GetComponent<Terrain>() != null && !ignore)
            {
                range = i.distance;
                //Debug.LogError(i.transform.gameObject.name);
                closestObject = i;
                found = true;
            }
        }
        if (!found)
        {
            position.y = temp;
            return position;
        }
        //Debug.LogError(closestObject.point + " " + position);

        return closestObject.point;
    }

    float _modf(float k, float bound)
    {
        float r = k / bound;
        return (r - Mathf.Floor(r)) * bound;
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
        //Debug.LogError(sideb + " " + sidec);
        //Debug.LogError((ang + ang2 + ang3));
        ang = invertangle(-sidea, sideb, ang);
        //ang2 = invertangle(-sideb,-sidec,ang2);
        //ang3 = invertangle(sidea,sidec,ang3);

        float bang = ang;
        ang *= (Mathf.PI / 180.0f);
        //Debug.LogError(bang + " " + ang2 + " " + ang3 + " " + Mathf.Abs(sideb.magnitude * Mathf.Sin(ang)));
        //Debug.LogError(sideb.magnitude * Mathf.Sin(ang));

        if (Mathf.Abs(sideb.magnitude * Mathf.Sin(ang)) >= distance || Mathf.Abs(bang) > 120.0f || ang3 > 120.0f)
            return false;

        return true;
    }

    Vector3 mark1posflat
    {
        get
        {
            return new Vector3(marker1.transform.position.x, 0, marker1.transform.position.z);
        }
    }

    Vector3 mark2posflat
    {
        get
        {
            return new Vector3(marker2.transform.position.x, 0, marker2.transform.position.z);
        }
    }

    Vector3 curposflat
    {
        get
        {
            return new Vector3(cursor.transform.position.x, 0, cursor.transform.position.z);
        }
    }

    bool mark1highlighted = false;
    bool mark2highlighted = false;

    // Update is called once per frame
    void Update()
    {
        ////TheTrailer.SetActive(true);
        //TheTrailer.GetComponent<TrailRenderer>().time = 2f;
        //MoveBetween();
        setPoints();
        //System.GC.Collect();
        // Determining what we need to highlight.
        if (marker1 != null && marker2 != null)
        {
           
            if ((curposflat - mark1posflat).magnitude <= 50.0f)
            {
               // marker1.GetComponent<Renderer>().material.SetColor("_Color", new Color(0f, 0f, 1f, 1.0f));
                if (Input.GetMouseButtonDown(1))
                {
                    //marker1.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 0f, 0f, 1.0f));
                    mark1highlighted = !mark1highlighted;
                }
            }
           
            if ((curposflat - mark2posflat).magnitude <= 50.0f)
            {
                //marker2.GetComponent<Renderer>().material.SetColor("_Color", new Color(0f, 0f, 1f, 1.0f));
                if (Input.GetMouseButtonDown(1))
                {
                    //marker2.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 0f, 0f, 1.0f));
                    mark2highlighted = !mark2highlighted;
                }
            }
            
            //Debug.LogError(checkLineDistanceFromPoint(marker1.transform.position, marker2.transform.position, curpos));
            timecount += Time.deltaTime;

            if (timecount > 3.0f)

                timecount = _modf(timecount, 3.0f);



        }

        if (!marker1.activeSelf || !marker2.activeSelf)
            slicerPlane.DisableRendering();

        // First check if the current state of the mouse is terrain
        if (ml.state == mouselistener.mouseState.TERRAIN)
        {
            activateCursor(true);

            // Set Cursor based on check

            //print(closestObject.point);
            curpos = raycastHit(Input.mousePosition);//closestObject.point;

           
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.Z))
            {
                // Place Markers
				if (marker1.transform.position == Vector3.zero)
				{
					marker1.SetActive(true);
					// set first marker
                    //change this to load new model
					//marker1 = (GameObject)Instantiate (Resources.Load("SlicerNode 5 22"));
                    //marker1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //marker1.transform.localScale += new Vector3(100f, 100f, 100f);
					marker1.transform.position = new Vector3(curpos.x, curpos.y - 10.0f, curpos.z);
					//marker1.GetComponent<Renderer>().material.SetColor("_Color", new Color(0f, 1f, 0f, 1.0f));
                    //Physics.IgnoreCollision(marker1.GetComponent<Collider>(), FirstPersonControllerPlayer.GetComponent<Collider>());


					
					//TrailerPosition = marker1.transform.position;
                    //resetTrailer();
                }
				else if (marker2.transform.position == Vector3.zero)
				{
					marker2.SetActive(true);
                    //TheTrailer.SetActive(true);
                    // set first marker
                    //change this to load new model
					//marker2 = (GameObject)Instantiate (Resources.Load("SlicerNode 5 22"));

                    //marker2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //marker2.transform.localScale += new Vector3(100f, 100f, 100f);
                    marker2.transform.position = new Vector3(curpos.x, curpos.y - 10.0f, curpos.z);
                    slicerPlane.Draw();

                    //marker2.GetComponent<Renderer>().material.SetColor("_Color", new Color(0f, 1f, 0f, 1.0f));
                    //Physics.IgnoreCollision(marker2.GetComponent<Collider>(), FirstPersonControllerPlayer.GetComponent<Collider>());

					
					timecount = 0.0f;
                }
                else
                {
					marker1.transform.position = Vector3.zero;
					marker2.transform.position = Vector3.zero;

					marker1.SetActive(false);
					marker2.SetActive(false);

					
					//Destroy(marker1);
                    //Destroy(marker2);
                    
                    mark1highlighted = false;
                    mark2highlighted = false;
                    //resetTrailer();
                    //TheTrailer.GetComponent<TrailRenderer>().time = 0;
                    //TheTrailer.SetActive(false);

                }
            }
            

            curpos.y += 1;
            setCursor(curpos);
            coordsystem.transformToUnity(curpos);

            // Determining if we need to move the markers
            if (Input.GetMouseButton(1))
            {
                if (mark1highlighted && !mark2highlighted)
                {
                    //cursor.active = false;
                    marker1.transform.position = curpos;
                    //resetTrailer();
                    //if (mark1highlighted || mark2highlighted)
                    //{
                        //TheTrailer.GetComponent<TrailRenderer>().time = .0f;
                    //}
                    timecount = 0.0f;

                    //activateCursor(false);
                    return;
                }
                else if (mark2highlighted && !mark1highlighted)
                {
                    //cursor.active = false;
                    marker2.transform.position = curpos;
                    //resetTrailer();
                    //TheTrailer.GetComponent<TrailRenderer>().time = .0f;
                    timecount = 0.0f;
                    activateCursor(true);

                    return;
                }
                else if (mark1highlighted && mark2highlighted)
                {
                    //cursor.active = false;
                    activateCursor(true);
                    //resetTrailer();

                    //TheTrailer.GetComponent<TrailRenderer>().time = .0f;
                    return;
                }
                //else
                //{
                //    TheTrailer.SetActive(true);
                //}
            }
            

            // Check if mouse was clicked
            // set/clear spheres
            // Check if Cursor is between the spheres
            // Draw Arrow -- move cursor and spheres if any movement
        }
        else
        {
            activateCursor(true);
        }
        curpos.y = -10000;
        ResizeObjects(cursor);
        ResizeUniformObjects(marker1);
        ResizeUniformObjects(marker2);
        //ResizeUniformObjects(TheTrailer);
        cursor.transform.Rotate(Vector3.forward, 1);

    }

    void ResizeObjects(GameObject obj)
    {
        GameObject currentController;
        float distance;
        float xyThresh, zThresh;

        if (obj == null)
        {
            return;
        }

        if (NoClipGhostPlayer.activeSelf)
        {
            currentController = NoClipGhostPlayer;
        }
        else
        {
            currentController = FirstPersonControllerPlayer;
        }

        distance = (obj.transform.position - currentController.transform.position).magnitude;

        if (distance/150 < 1)
        {
            xyThresh = 0.1f;
        }
        else
        {
            xyThresh = distance / 150;
        }
        if (distance / 30 < 3)
        {
            zThresh = 0.3f;
        }
        else
        {
            zThresh = distance / 30;
        }

        obj.transform.localScale = new Vector3(xyThresh, xyThresh, zThresh);
    }

    void ResizeUniformObjects(GameObject obj)
    {
        GameObject currentController;
        float distance;
        float xyThresh, zThresh;

        if (obj == null)
        {
            return;
        }

        if (NoClipGhostPlayer.activeSelf)
        {
            currentController = NoClipGhostPlayer;
        }
        else
        {
            currentController = FirstPersonControllerPlayer;
        }

        distance = (obj.transform.position - currentController.transform.position).magnitude;


        if (distance / slicerDistanceScaleFactor < slicerMinScale)
            // Change this
            zThresh = slicerMinScale;
        else
            zThresh = distance / slicerDistanceScaleFactor;

        obj.transform.localScale = new Vector3(zThresh, zThresh, zThresh);
    }

    void OnGUI()
    {
        /*Vector2 posi = tm.ds.UnityToLatLong (new Vector2 (curpos.x, curpos.z));
        string str = "Height: " + tm.sampleHeight (curpos)+"\n LAT_LONG: " + posi.x + ", " + posi.y;
        Vector2 posoffset = GUI.skin.box.CalcSize(new GUIContent(str));
        GUI.Box(new Rect(Screen.width-posoffset.x,Screen.height-posoffset.y,posoffset.x,posoffset.y),str);*/
    }
    void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(curpos,10);
    }

    void getPointInformation()
    {
        // Query the data of the dataset class
        // One function needed in the dataset class
    }

    void setPoints()
    {
        // Here is where the cross section information goes...
        if(marker1.activeSelf && marker2.activeSelf)
        {
            // Call Function that normalize the positions x and y between 0 and 1 for terrain.
            //Vector2 Point1 = TerrainUtils.NormalizePointToTerrain(marker1.transform.position, Terrain.activeTerrain);
            //Vector2 Point2 = TerrainUtils.NormalizePointToTerrain(marker2.transform.position, Terrain.activeTerrain);
            Vector2 Point1 = TerrainUtils.NormalizePointToTerrain(marker1.transform.position, GlobalConfig.TerrainBoundingBox);
            Vector2 Point2 = TerrainUtils.NormalizePointToTerrain(marker2.transform.position, GlobalConfig.TerrainBoundingBox);

            //Debug.LogError(Point1);
            //Debug.LogError(Point2);
            // Pass point 1 and point 2 to shader
            rs.setFirstPoint(Point1);
            rs.setSecondPoint(Point2);
            csvButton.SetActive(true);
        }
        else
        {
            if(csvButton.activeSelf)
            {
                csvButton.SetActive(false);
            }
        }
    }
    //// A variable for switching the direction of the trail renderer
    //bool direction = false;
    //void MoveBetween()
    //{   
    //    // Here is where the cross section information goes...
    //    if (marker1.activeSelf && marker2.activeSelf)
    //    {
    //        //Debug.LogError(TrailerPosition);
    //        Vector3 Direction = marker1.transform.position - marker2.transform.position;
    //        if(Vector3.Distance(TrailerPosition,marker2.transform.position) < 20.0)
    //        {
    //            //TrailerPosition = marker1.transform.position;
    //            direction = true;
    //        }
    //        else if (Vector3.Distance(TrailerPosition, marker1.transform.position) < 20.0)
    //        {
    //            //TrailerPosition = marker2.transform.position;
    //            direction = false;
    //        }
    //        //Debug.LogError(Vector3.Distance(TrailerPosition, marker2.transform.position));
    //        //Debug.LogError(Vector3.Distance(TrailerPosition, marker1.transform.position));
    //        if(!direction)
    //            TrailerPosition = Vector3.MoveTowards(TrailerPosition, marker2.transform.position, Vector3.Distance(marker1.transform.position, marker2.transform.position) * Time.deltaTime/3);
    //        else
    //            TrailerPosition = Vector3.MoveTowards(TrailerPosition, marker1.transform.position, Vector3.Distance(marker1.transform.position, marker2.transform.position) * Time.deltaTime/3);

    //        float ypos = raycastHitFurtherest(TrailerPosition, Vector3.up).y + 5;
    //        TrailerPosition.y = ypos;
    //        TheTrailer.transform.position = TrailerPosition;
    //    }


    //}

    //void resetTrailer()
    //{
    //    direction = true;
    //    //TrailRendererExtensions.Reset(TheTrailer.GetComponent<TrailRenderer>(), this);
    //    if(marker1 != null )
    //    {
    //        TrailerPosition = marker1.transform.position;
    //        TheTrailer.transform.position = TrailerPosition;
    //    }
    //    //TheTrailer.GetComponent<TrailRenderer>().time = 1f;
    //    TheTrailer.GetComponent<TrailRenderer>().time = 0f;
    //    //TheTrailer.SetActive(false);
    //}
}
