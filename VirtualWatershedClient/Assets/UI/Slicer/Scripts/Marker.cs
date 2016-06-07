using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Marker : MonoBehaviour {

    public GameObject marker1, marker2, cursor, minPin, maxPin;
    public bool marker1Active = false;
    public bool marker2Active = false;
    private Vector3 cursorPos, CursorWorldPos;
    public raySlicer rayslice;
    public GameObject PlayerController;
    public Button SavePlacementButton;
    public MarkerListView listView;

    Vector3 mark1posflat
    {
        get { return new Vector3(marker1.transform.position.x, 0, marker1.transform.position.z); }
    }

    Vector3 mark2posflat
    {
        get { return new Vector3(marker2.transform.position.x, 0, marker2.transform.position.z); }
    }

    Vector3 curposflat
    {
        get { return new Vector3(cursor.transform.position.x, 0, cursor.transform.position.z); }
    }

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
                    SavePlacementButton.interactable = true;
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
                    SavePlacementButton.interactable = true;
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

                    // Set the save button to interactiable
                    SavePlacementButton.interactable = true;
                }
                else
                {
                    Clear();
                }
            }            

            // Update the cursor
            activateCursor(true);
            cursorPos.y += 1;
            setCursor(cursorPos);
            coordsystem.transformToUnity(cursorPos);
        }

        
        cursorPos.y = -10000;
        Utilities.ResizeObjects(cursor, PlayerController.transform.position);
        Utilities.ResizeUniformObjects(marker1, PlayerController.transform.position);
        Utilities.ResizeUniformObjects(marker2, PlayerController.transform.position);
        Utilities.ResizeUniformObjects(minPin, PlayerController.transform.position);
        Utilities.ResizeUniformObjects(maxPin, PlayerController.transform.position);
        setPoints();

        cursor.transform.Rotate(Vector3.forward, 1);
    }

    public void Clear()
    {
        marker1.transform.position = Vector3.zero;
        marker2.transform.position = Vector3.zero;

        marker1.SetActive(false);
        marker2.SetActive(false);

        marker1Active = false;
        marker2Active = false;

        rayslice.marker1 = false;
        rayslice.marker2 = false;

        SavePlacementButton.interactable = false;
        listView.ClearSelected();
    }

    /// <summary>
    /// Adds the current markers to the table.
    /// </summary>
    public void AddToTable()
    {
        Vector3 WorldPoint1 = coordsystem.transformToWorld(marker1.transform.position);
        Vector3 WorldPoint2 = coordsystem.transformToWorld(marker2.transform.position);
        listView.AddRow(new object [] { "UTM: (" + WorldPoint1.x + ", " + WorldPoint1.z + ") to (" + WorldPoint2.x + ", " + WorldPoint2.z + ")", marker1.transform.position, marker2.transform.position });
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
        }
    }

    /// <summary>
    /// Used to set the markers by code in a specific spot
    /// </summary>
    /// <param name="m1">The marker1 position to move to.</param>
    /// <param name="m2">The marker2 position to move to.</param>
    public void SetMarkers(Vector3 m1, Vector3 m2)
    {
        // Activate
        marker1.SetActive(true);
        rayslice.marker1 = true;
        marker2.SetActive(true);
        rayslice.marker2 = true;

        // set markers positions
        marker1.transform.position = m1;
        marker2.transform.position = m2;

        // Make sure the user will not save the same values
        SavePlacementButton.interactable = false;
    }
    
    
    private void DataToFile(List<float> DataSlice, string filename, string name, string variable, DateTime time)
    {
        String pathDownload = Utilities.GetFilePath(filename + "_slicer_data.csv");
        Debug.LogError("The File Path: " + pathDownload);

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@pathDownload))
        {
            file.WriteLine("This file represenets interpolated data that was calculated from a line across the currently shown dataset.");
            file.WriteLine("Variable, " + variable);
            if (time == DateTime.MinValue)
            {
                file.WriteLine("Time of Data: UNKNOWN");
            }
            else
            {
                file.WriteLine("Time of Data: " + time);
            }
            file.WriteLine(name);
            file.WriteLine("UTM Zone: " + coordsystem.localzone);
            foreach (var i in DataSlice)
            {
                file.Write(i + ", ");
            }
        }
    }

    public void MarkerOutputToFile()
    {
        if(marker1.activeSelf && marker2.activeSelf)
        {
            // Place in the check locations
            Vector3 WorldPoint1 = coordsystem.transformToWorld(marker1.transform.position);
            Vector2 CheckPoint1 = new Vector2(WorldPoint1.x, WorldPoint1.z);
            Vector3 WorldPoint2 = coordsystem.transformToWorld(marker2.transform.position);
            Vector2 CheckPoint2 = new Vector2(WorldPoint2.x, WorldPoint2.z);
            string UTMName = "UTM, " + WorldPoint1.x + " - " + WorldPoint1.z + ", " + WorldPoint2.x + " - " + WorldPoint2.z;
            List<float> dataslice = new List<float>();
            float[,] data;

            // Run this for the loaded in data
            foreach (string activedata in ActiveData.GetCurrentAvtive())
            {
                Rect BoundingBox = ActiveData.GetBoundingBox(activedata);
                if (BoundingBox.Contains(CheckPoint1) && BoundingBox.Contains(CheckPoint2))
                {
                    Vector2 NormalizedPoint1 = TerrainUtils.NormalizePointToTerrain(WorldPoint1, BoundingBox);
                    Vector2 NormalizedPoint2 = TerrainUtils.NormalizePointToTerrain(WorldPoint2, BoundingBox);
                    int DataIndex = ActiveData.GetCurrentIndex();
                    data = ActiveData.GetFrameAt(activedata, DataIndex).Data;
                    
                    int marker1Row = data.GetLength(0) - 1 - (int)Math.Min(Math.Round(data.GetLength(0) * NormalizedPoint1.x), (double)data.GetLength(0) - 1);
                    int marker1Col = data.GetLength(1) - 1 - (int)Math.Min(Math.Round(data.GetLength(1) * NormalizedPoint1.y), (double)data.GetLength(1) - 1);
                    int marker2Row = data.GetLength(0) - 1 - (int)Math.Min(Math.Round(data.GetLength(0) * NormalizedPoint2.x), (double)data.GetLength(0) - 1);
                    int marker2Col = data.GetLength(1) - 1 - (int)Math.Min(Math.Round(data.GetLength(1) * NormalizedPoint2.y), (double)data.GetLength(1) - 1);                    

                    dataslice = Utilities.BuildSlice(marker1Row, marker1Col, marker2Row, marker2Col, data);                    
                    DataToFile(dataslice, activedata, activedata + ", " + VariableReference.GetDescription(activedata), UTMName, ActiveData.GetFrameAt(activedata, ActiveData.GetCurrentIndex()).starttime);
                }
            }

            // Do this for the terrain data
            data = TerrainUtils.GetHeightmap(Terrain.activeTerrain);
            Vector2 NormalizedPoint1 = TerrainUtils.NormalizePointToTerrain(WorldPoint1, Terrain.activeTerrain.);
            Vector2 NormalizedPoint2 = TerrainUtils.NormalizePointToTerrain(WorldPoint2, BoundingBox);
            int marker1Row = data.GetLength(0) - 1 - (int)Math.Min(Math.Round(data.GetLength(0) * NormalizedPoint1.x), (double)data.GetLength(0) - 1);
            int marker1Col = data.GetLength(1) - 1 - (int)Math.Min(Math.Round(data.GetLength(1) * NormalizedPoint1.y), (double)data.GetLength(1) - 1);
            int marker2Row = data.GetLength(0) - 1 - (int)Math.Min(Math.Round(data.GetLength(0) * NormalizedPoint2.x), (double)data.GetLength(0) - 1);
            int marker2Col = data.GetLength(1) - 1 - (int)Math.Min(Math.Round(data.GetLength(1) * NormalizedPoint2.y), (double)data.GetLength(1) - 1);
            dataslice = Utilities.BuildSlice(marker1Row, marker1Col, marker2Row, marker2Col, data);
            DataToFile(dataslice, "Terrain", "Terrain", UTMName, DateTime.MinValue);
        }
    }
}
