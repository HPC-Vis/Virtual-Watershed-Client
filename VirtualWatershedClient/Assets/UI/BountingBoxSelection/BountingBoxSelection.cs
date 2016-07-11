using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class BountingBoxSelection : MonoBehaviour {
    public GameObject point1, point2, cursor, PlayerController,maxPin, minPin;
    public bool point1Active = false;
    public bool point2Active = false;
    public raySlicer rayslice;
    private Vector3 cursorPos, startPos,offset;
    public Button SavePlacementButton;
  
    Vector3 point1posflat
    {
        get { return new Vector3(point1.transform.position.x, 0, point1.transform.position.z); }
    }

    Vector3 point2posflat
    {
        get { return new Vector3(point2.transform.position.x, 0, point2.transform.position.z); }
    }

    Vector3 cursorposflat
    {
        get { return new Vector3(cursor.transform.position.x, 0, cursor.transform.position.z); }
    }


	// Use this for initialization
	void Start () {
        Vector3 Pos = mouseray.raycastHitFurtherest(Vector3.zero, Vector3.up, -10000);
        Pos.y += 50;
        PlayerController.transform.position = Pos;
        point1.transform.position = Vector3.zero;
        point2.transform.position = Vector3.zero;
        activateCursor(true);
      
	}

    // Update is called once per frame
    void Update()
    {
        cursorPos = mouseray.UpdateCursor();

        if (mouselistener.state == mouselistener.mouseState.TERRAIN)
        {
            if ((cursorposflat - point1posflat).magnitude <= 50.0f)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    point1Active = true;
                    SavePlacementButton.interactable = true;
                    startPos = Input.mousePosition;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    point1Active = false;
                    offset = Input.mousePosition - startPos; 
                }
            }

            if ((cursorposflat - point2posflat).magnitude <= 50.0f)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    point2Active = true;
                    SavePlacementButton.interactable = true;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    point2Active = false;
                }
            }

            if (point1Active)
            {
                point1.transform.position = new Vector3(cursorPos.x, cursorPos.y - 10.0f, cursorPos.z);
            }
            else if (point2Active)
            {
                point2.transform.position = new Vector3(cursorPos.x, cursorPos.y - 10.0f, cursorPos.z);
            }

            if(Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.Z))
            {
                if (!point1.activeSelf)
                {
                    point1.SetActive(true);
                   // rayslice.point1 = true;

                    point2.transform.position = new Vector3(cursorPos.x, cursorPos.y - 10.0f, cursorPos.z);
                    SavePlacementButton.interactable = true;
                }
                else
                {
                    clear();
                }
            }

            activateCursor(true);
            cursorPos.y += 1;
            setCursor(cursorPos);
            coordsystem.transformToUnity(cursorPos);

        }

        cursorPos.y = -10000;
        Utilities.ResizeObjects(cursor, PlayerController.transform.position);
        Utilities.ResizeUniformObjects(point1, PlayerController.transform.position);
        Utilities.ResizeUniformObjects(point2, PlayerController.transform.position);
        Utilities.ResizeUniformObjects(minPin, PlayerController.transform.position);
        Utilities.ResizeUniformObjects(maxPin, PlayerController.transform.position);
        setPoints();

        cursor.transform.Rotate(Vector3.forward, 1);
    }

    public void clear()
    {
        point1.transform.position = Vector3.zero;
        point2.transform.position = Vector3.zero;
        point1.SetActive(false);
        point2.SetActive(false);

        point1Active = false;
        point2Active = false;

       // rayslice.point1 = false;
       // rayslice.point2 = false;

        SavePlacementButton.interactable = false;
        
    }


    public void activateCursor(bool val)
    {
        cursor.SetActive(val);
    }

    void setCursor(Vector3 worldp)
    {
        cursor.transform.position = worldp;
    }

    void setPoints()
    {
        if (point1.activeSelf && point2.activeSelf)
        {
            Vector2 Point1 = TerrainUtils.NormalizePointToTerrain(point1.transform.position, GlobalConfig.TerrainBoundingBox);
            Vector2 Point2 = TerrainUtils.NormalizePointToTerrain(point2.transform.position, GlobalConfig.TerrainBoundingBox);

            rayslice.setFirstPoint(Point1);
            rayslice.setSecondPoint(Point2); 
        }
    }

    public void DrawBox()
    {

    }
}
