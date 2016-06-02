using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Marker : MonoBehaviour {

    public GameObject marker1, marker2, cursor, minPin, maxPin;
    public bool marker1Active = false;
    public bool marker2Active = false;
    private Vector3 cursorPos, CursorWorldPos;
    public raySlicer rayslice;
    public GameObject csvButton;
    public GameObject PlayerController;

    private List<float> DataSlice = new List<float>();

    // Use this for initialization
    void Start () {
        // Add objects to the ignored list
        mouseray.IgnoredObjects.Add(marker1);
        mouseray.IgnoredObjects.Add(marker2);
        mouseray.IgnoredObjects.Add(cursor);
        mouseray.IgnoredObjects.Add(minPin);
        mouseray.IgnoredObjects.Add(maxPin);
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
                    marker1Active = true;
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
                    marker2Active = true;
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

            // Check if there is active markers
            if (marker1.activeSelf && marker2.activeSelf)
            {
                // Place in the check locations
                Vector3 WorldPoint1 = coordsystem.transformToWorld(marker1.transform.position);
                Vector2 CheckPoint1 = new Vector2(WorldPoint1.x, WorldPoint1.z);
                Vector3 WorldPoint2 = coordsystem.transformToWorld(marker2.transform.position);
                Vector2 CheckPoint2 = new Vector2(WorldPoint2.x, WorldPoint2.z);
                Rect BoundingBox = ActiveData.GetBoundingBox(ActiveData.GetCurrentAvtive()[0]);

                if (BoundingBox.Contains(CheckPoint1) && BoundingBox.Contains(CheckPoint2))
                {
                    Vector2 NormalizedPoint1 = TerrainUtils.NormalizePointToTerrain(WorldPoint1, BoundingBox);
                    Vector2 NormalizedPoint2 = TerrainUtils.NormalizePointToTerrain(WorldPoint2, BoundingBox);
                    List<String> tempFrameRef = ActiveData.GetCurrentAvtive();
                    int DataIndex = ActiveData.GetCurrentIndex();
                    int marker1Row = ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(0) - 1 - (int)Math.Min(Math.Round(ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(0) * NormalizedPoint1.x), (double)ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(0) - 1);
                    int marker1Col = ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(1) - 1 - (int)Math.Min(Math.Round(ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(1) * NormalizedPoint1.y), (double)ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(1) - 1);
                    int marker2Row = ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(0) - 1 - (int)Math.Min(Math.Round(ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(0) * NormalizedPoint2.x), (double)ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(0) - 1);
                    int marker2Col = ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(1) - 1 - (int)Math.Min(Math.Round(ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(1) * NormalizedPoint2.y), (double)ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(1) - 1);

                    DataSlice.Clear();
                    BuildSlice(marker1Row, marker1Col, marker2Row, marker2Col);
                    //button.SetActive(true);
                }
                else
                {
                    //button.SetActive(false);
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
        ResizeUniformObjects(minPin);
        ResizeUniformObjects(maxPin);
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

    /// <summary>
    /// Computes a bilinear interoplation off the given initial location, end location, and the point to interpolate on
    /// </summary>
    /// <param name="x1">Initial x point</param>
    /// <param name="y1">Initial y point</param>
    /// <param name="x2">End x point</param>
    /// <param name="y2">End y point</param>
    /// <param name="x">Interpol Point x</param>
    /// <param name="y">Interpol point y</param>
    public float bilinearInterpolation(int x1, int y1, int x2, int y2, float x, float y)
    {
        // Debug log for testing the data
        // Debug.LogError("The value x1, y1, x2, y2, x, y, ts(x1,y1), ts(x2,y1), ts(x1,y2), ts(x2,y2): " + x1 + ", " + y1 + ", " + x2 + ", " + y2 + ", " + x + ", " + y + ", " + ActiveData.GetFrameAt(DataIndex).Data[x1, y1] + ", " + ActiveData.GetFrameAt(DataIndex).Data[x2, y1] + ", " + ActiveData.GetFrameAt(DataIndex).Data[x1, y2] + ", " + ActiveData.GetFrameAt(DataIndex).Data[x2, y2]);

        // Run the Interpolation, and return.
        List<String> tempFrameRef = ActiveData.GetCurrentAvtive();
        int DataIndex = ActiveData.GetCurrentIndex();
        float value = (1 / ((x2 - x1) * (y2 - y1))) * ((ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data[x1, y1] * (x2 - x) * (y2 - y)) + (ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data[x2, y1] * (x - x1) * (y2 - y)) + (ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data[x1, y2] * (x2 - x) * (y - y1)) + (ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data[x2, y2] * (x - x1) * (y - y1)));
        return value;
    }

    /// <summary>
    /// takes the two given points and builds a interpolated slice off of it.
    /// </summary>
    public void BuildSlice(int m1r, int m1c, int m2r, int m2c)
    {
        int sample_rate = 50;
        Vector2 vec = new Vector2(m2r - m1r, m2c - m1c);
        float mag = Mathf.Sqrt((vec.x * vec.x) + (vec.y * vec.y));
        int x1, y1, x2, y2;
        float x, y;

        Vector2 unit = new Vector2((1 / mag) * vec.x, (1 / mag) * vec.y);

        for (int i = 0; i < sample_rate; i++)
        {
            x = m1r + (unit.x * (float)((float)i / (float)sample_rate) * mag);
            y = m1c + (unit.y * (float)((float)i / (float)sample_rate) * mag);

            x1 = (int)Mathf.Floor(x);
            y1 = (int)Mathf.Floor(y);
            x2 = (int)Mathf.Ceil(x);
            y2 = (int)Mathf.Ceil(y);

            if (x1 == x2)
            {
                x2 += 1;
            }
            if (y1 == y2)
            {
                y2 += 1;
            }
            DataSlice.Add(bilinearInterpolation(x1, y1, x2, y2, x, y));
        }

        x = m1r + (unit.x * 1 * mag);
        y = m1c + (unit.y * 1 * mag);

        x1 = m2r - 1;
        y1 = m2c - 1;
        x2 = m2r;
        y2 = m2c;
        DataSlice.Add(bilinearInterpolation(x1, y1, x2, y2, x, y));
    }

    /// <summary>
    /// This will send the data on the slicer to a file
    /// </summary>
    public void SlicerToFile()
    {
        String pathDownload = Utilities.GetFilePath("slicer_data.csv");
        Debug.LogError("The File Path: " + pathDownload);

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@pathDownload))
        {
            file.WriteLine("This file represenets interpolated data that was calculated from a line across the currently shown dataset.");
            //file.WriteLine(variable_name + ": " + unitsLabel);
            List<String> tempFrameRef = ActiveData.GetCurrentAvtive();
            file.WriteLine("Time of Frame: " + ActiveData.GetFrameAt(tempFrameRef[0], ActiveData.GetCurrentIndex()).starttime);
            //file.WriteLine("UTM: (" + WorldPoint1.x + ", " + WorldPoint1.z + ") to (" + WorldPoint2.x + ", " + WorldPoint2.z + ").");
            file.WriteLine("UTM Zone: " + coordsystem.localzone);
            foreach (var i in DataSlice)
            {
                file.Write(i + ", ");
            }
        }
    }
}
