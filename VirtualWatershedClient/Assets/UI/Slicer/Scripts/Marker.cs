using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Marker : MonoBehaviour {

    public GameObject marker1, marker2, cursor;
    public bool marker1Active = false;
    public bool marker2Active = false;
    private Vector3 cursorPos, CursorWorldPos;
    public raySlicer rayslice;
    public GameObject csvButton;
    public GameObject PlayerController;

    // What uses this??
    float timecount;

    // Use this for initialization
    void Start () {
        // Add objects to the ignored list
        mouseray.IgnoredObjects.Add(marker1);
        mouseray.IgnoredObjects.Add(marker2);
        mouseray.IgnoredObjects.Add(cursor);
        mouseray.IgnoredObjects.Add(PlayerController);

        // Set objects
        Vector3 Pos = mouseray.raycastHitFurtherest(Vector3.zero, Vector3.up, -10000);
        Pos.y += 50;
        PlayerController.transform.position = Pos;
        marker1.transform.position = Vector3.zero;
        marker2.transform.position = Vector3.zero;
        activateCursor(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Get the new cursor positions
        cursorPos = mouseray.UpdateCursor();

        // First check if the current state of the mouse is terrain
        if (mouselistener.state == mouselistener.mouseState.TERRAIN)
        {
            // See if the markers are selected to move
            if ((curposflat - mark1posflat).magnitude <= 50.0f)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    marker1Active = !marker1Active;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    marker1Active = false;
                }
            }

            if ((curposflat - mark2posflat).magnitude <= 50.0f)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    marker2Active = !marker2Active;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    marker2Active = false;
                }
            }

            // Update the markser positions
            if(marker1Active)
            {
                marker1.transform.position = new Vector3(cursorPos.x, cursorPos.y - 10.0f, cursorPos.z);
            }
            else if(marker2Active)
            {
                marker2.transform.position = new Vector3(cursorPos.x, cursorPos.y - 10.0f, cursorPos.z);
            }


            // Set Cursor based on check
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.Z))
            {
                // Place Markers
                if (!marker1.activeSelf)
                {
                    // Activate
                    marker1.SetActive(true);
                    rayslice.marker1 = true;

                    // set first marker position
                    marker1.transform.position = new Vector3(cursorPos.x, cursorPos.y - 10.0f, cursorPos.z);
                }
                else if (!marker2.activeSelf)
                {
                    // Activate
                    marker2.SetActive(true);
                    rayslice.marker2 = true;

                    // set second marker position
                    marker2.transform.position = new Vector3(cursorPos.x, cursorPos.y - 10.0f, cursorPos.z);
                }
                else
                {
                    marker1.transform.position = Vector3.zero;
                    marker2.transform.position = Vector3.zero;

                    marker1.SetActive(false);
                    marker2.SetActive(false);

                    marker1Active = false;
                    marker2Active = false;

                    rayslice.marker1 = false;
                    rayslice.marker2 = false;
                }
            }

            // Update the cursor
            activateCursor(true);
            cursorPos.y += 1;
            setCursor(cursorPos);
            coordsystem.transformToUnity(cursorPos);
        }

        
        cursorPos.y = -10000;
        ResizeObjects(cursor);
        ResizeUniformObjects(marker1);
        ResizeUniformObjects(marker2);
        setPoints();

        cursor.transform.Rotate(Vector3.forward, 1);
    }

    float _modf(float k, float bound)
    {
        float r = k / bound;
        return (r - Mathf.Floor(r)) * bound;
    }

    //self explanatory function
    public void activateCursor(bool val)
    {
        cursor.SetActive(val);
    }

    void setCursor(Vector3 worldp)
    {
        cursor.transform.position = worldp;
        CursorWorldPos = worldp;
    }


    void setPoints()
    {
        // Here is where the cross section information goes...
        if (marker1.activeSelf && marker2.activeSelf)
        {
            // Call Function that normalize the positions x and y between 0 and 1 for terrain.
            Vector2 Point1 = TerrainUtils.NormalizePointToTerrain(marker1.transform.position, GlobalConfig.TerrainBoundingBox);
            Vector2 Point2 = TerrainUtils.NormalizePointToTerrain(marker2.transform.position, GlobalConfig.TerrainBoundingBox);

            // Pass point 1 and point 2 to shader
            rayslice.setFirstPoint(Point1);
            rayslice.setSecondPoint(Point2);
            csvButton.SetActive(true);
        }
        else
        {
            if (csvButton.activeSelf)
            {
                csvButton.SetActive(false);
            }
        }
    }

    void ResizeObjects(GameObject obj)
    {
        float distance;
        float xyThresh, zThresh;

        if (obj == null)
        {
            return;
        }

        distance = (obj.transform.position - PlayerController.transform.position).magnitude;

        if (distance / 150 < 1)
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
        float distance;
        float zThresh;

        if (obj == null)
        {
            return;
        }

        distance = (obj.transform.position - PlayerController.transform.position).magnitude;


        if (distance / mouseray.slicerDistanceScaleFactor < mouseray.slicerMinScale)
        {
            // Change this
            zThresh = mouseray.slicerMinScale;
        }
        else
        {
            zThresh = distance / mouseray.slicerDistanceScaleFactor;
        }

        obj.transform.localScale = new Vector3(zThresh, zThresh, zThresh);
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
}
