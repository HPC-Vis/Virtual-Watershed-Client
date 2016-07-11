using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class BoundingBoxSelection : MonoBehaviour
{
   
    public Vector3 cursorPos, startPos, endPos, offset;
    Vector3 topLeft, topRight, bottomLeft, bottomRight;
   
    

    // Use this for initialization
    void Start()
    {

    }


    void Update()
    {
        
        cursorPos = mouseray.UpdateCursor();

        if (mouselistener.state == mouselistener.mouseState.TERRAIN)
        {

            if (Input.GetMouseButtonDown(0))
            {

                startPos = cursorPos;
                //AddMarker(startPos);
                Debug.Log("Start Position");
                Debug.Log(startPos);
            }

            if (Input.GetMouseButtonUp(0))
            {

                endPos = cursorPos;
                //AddMarker(endPos);
                Debug.Log("End Position");
                Debug.Log(endPos);
                CalcPosition(startPos, endPos);
            }
            
        }
    }

    // Marking start and end positions
    void AddMarker(Vector3 pos)
    {
        //Debug.LogError("BB: Addmarker");
        GameObject Marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Marker.transform.position = new Vector3(pos.x, pos.y, pos.z);
        Marker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        return;
    }

    //Marking vertex of the bounding box
    void AddVertex(Vector3 points)
    {
        if (points == topLeft)
        {
           GameObject marker1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker1.transform.position = new Vector3(points.x, points.y, points.z);
            marker1.transform.localScale = new Vector3(0.1f, 6f, 0.1f);
            //rayslice.marker1 = true;
            
        }
        else if (points == topRight)
        {
           GameObject marker2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker2.transform.position = new Vector3(points.x, points.y, points.z);
            marker2.transform.localScale = new Vector3(0.1f, 6f, 0.1f);
            //rayslice.marker2 = true;
           // marker2.GetComponent<Projector>();
           // Projector pro = GetComponent<Projector>();
          //  pro.farClipPlane = 1;
        }
        else if(points == bottomLeft)
        {
           GameObject marker3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker3.transform.position = new Vector3(points.x, points.y, points.z);
            marker3.transform.localScale = new Vector3(0.1f, 6f, 0.1f);
            //rayslice.marker3 = true;
        }
        else if (points == bottomRight)
        {
           GameObject marker4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker4.transform.position = new Vector3(points.x, points.y, points.z);
            marker4.transform.localScale = new Vector3(0.1f, 6f, 0.1f);
            //rayslice.marker4 = true;
        }

        //GameObject Cap = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Cap.transform.position = new Vector3(points.x, points.y, points.z);
        //Cap.transform.Translate(d/2, 0, 0);
        //Cap.transform.localScale = new Vector3(0.1f, 6f, 0.1f);

       // pillar.SetActive(true);
       // pillar.transform.position = new Vector3(points.x, points.y, points.z);
       // pillar.transform.localScale = new Vector3(1f, 7f, 0.1f);
    }


    //Calculating vertex of the bounding box
    void CalcPosition(Vector3 start, Vector3 end)
    {
        Debug.LogError("Calcposition");
        topLeft = new Vector3(start.x, 230, start.z);
        float d1 = Mathf.Abs(end.x - start.x);
        AddVertex(topLeft);
        topRight = new Vector3(end.x, 230, start.z);
        float d2 = Mathf.Abs(end.y - start.y);
        AddVertex(topRight);
        bottomLeft = new Vector3(start.x, 230, end.z);
        AddVertex(bottomLeft);
        bottomRight = new Vector3(end.x, 230, end.z);
        AddVertex(bottomRight);

        Debug.Log(topLeft);
        Debug.Log(topRight);
        Debug.Log(bottomLeft);
        Debug.Log(bottomRight);

    }
  
}
