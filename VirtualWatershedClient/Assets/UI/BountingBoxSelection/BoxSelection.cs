using UnityEngine;
using System.Collections;

public class BoxSelection : MonoBehaviour {

    public raySlicer rayslice;
    public Vector3 cursorPos, startPos, endPos;
    public GameObject marker1, marker2, marker3, marker4;
    public bool marker1Active = false;
    public bool marker2Active = false;
    Vector3 topLeft, topRight, bottomLeft, bottomRight;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        cursorPos = mouseray.UpdateCursor();
        if (mouselistener.state == mouselistener.mouseState.TERRAIN)
        {
            if (Input.GetMouseButtonDown(0))
            {
               //marker1Active = true;
                startPos = cursorPos;
                Debug.Log("start position");
                Debug.Log(startPos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                //marker2Active = true;
                endPos = cursorPos;
                Debug.Log("Endposition");
                Debug.Log(endPos);
                CalcPosition(startPos, endPos);
            }
        }
	
	}
    void AddVertex( Vector3 points)
    {
       
        if (points == topLeft)
        {
            
            marker1.SetActive(true);
            marker1.transform.position = new Vector3(points.x, points.y, points.z);
            marker1.transform.localScale = new Vector3(0.1f, 6f, 0.1f);
        }
        else if (points == topRight)
        {
            
            marker2.SetActive(true);
            marker2.transform.position = new Vector3(points.x, points.y, points.z);
            marker2.transform.localScale = new Vector3(.1f, 6f, .1f);
        }
        else if (points == bottomLeft)
        {
           
            marker3.SetActive(true);
            marker3.transform.position = new Vector3(points.x, points.y, points.z);
            marker3.transform.localScale = new Vector3(.1f, 6f, .1f);
        }
        else if (points == bottomRight)
        {
            
            marker4.SetActive(true);
            marker4.transform.position = new Vector3(points.x, points.y, points.z);
            marker4.transform.localScale = new Vector3(.1f, 6f, .1f);
        }
    }

    void CalcPosition(Vector3 start, Vector3 end)
    {

        topLeft = new Vector3(start.x, 230, start.z);
        Debug.Log(topLeft);
        AddVertex(topLeft);
        topRight = new Vector3(end.x, 230, start.z);
        Debug.Log(topRight);
        AddVertex(topRight);
        bottomLeft = new Vector3(start.x, 230, end.z);
        Debug.Log(bottomLeft);
        AddVertex(bottomLeft);
        bottomRight = new Vector3(end.x, 230, end.z);
        Debug.Log(bottomRight);
        AddVertex(bottomRight);

    }
    void SetPoints()
    {
        if(marker1.activeSelf && marker2.activeSelf)
        {
            Vector2 Point1 = TerrainUtils.NormalizePointToTerrain(marker1.transform.position, GlobalConfig.TerrainBoundingBox);
            Vector2 Point2 = TerrainUtils.NormalizePointToTerrain(marker2.transform.position, GlobalConfig.TerrainBoundingBox);

            rayslice.setFirstPoint(Point1);
            rayslice.setSecondPoint(Point2);
        }
    }

    public void BoundingBoxSelection()
    {
        if (marker1.activeSelf && marker2.activeSelf)
        {
            Vector3 WorldPoint1 = marker1.transform.position;
            Vector2 CheckPoint1 = new Vector2(WorldPoint1.x, WorldPoint1.z);
            Vector3 WorldPoint2 = marker2.transform.position;
            Vector2 CheckPoint2 = new Vector2(WorldPoint2.x, WorldPoint2.z);
            Rect BoundingBox;
            Vector2 marker1Points, marker2Points;
            float[,] data;
            foreach (string activedata in ActiveData.GetCurrentAvtive())
            {
                BoundingBox = ActiveData.GetBoundingBox(activedata);
                if (BoundingBox.Contains(CheckPoint1) && BoundingBox.Contains(CheckPoint2))
                {
                    int DataIndex = ActiveData.GetCurrentIndex();
                    data = ActiveData.GetFrameAt(activedata, DataIndex).Data;

                    marker1Points = Utilities.GetDataPointFromWorldPoint(data, BoundingBox, WorldPoint1);
                    marker2Points = Utilities.GetDataPointFromWorldPoint(data, BoundingBox, WorldPoint2);

                }
            }
            data = TerrainUtils.GetHeightmap(Terrain.activeTerrain);
            BoundingBox = GlobalConfig.TerrainBoundingBox;
            marker1Points = Utilities.GetDataPointFromWorldPoint(data, BoundingBox, WorldPoint1);
            marker2Points = Utilities.GetDataPointFromWorldPoint(data, BoundingBox, WorldPoint2);

        }
    }
}
