using UnityEngine;
using System.Collections;

public class SlicerPlane : MonoBehaviour
{
    public GameObject marker1;
    public GameObject marker2;

    LayerMask terrainLayerMask;

    Transform m1Top;
    Transform m2Top;

    Mesh mesh;
    MeshRenderer renderer;

    LineRenderer topLineRenderer;
    LineRenderer bottomLineRenderer;
    int lineRendererPositionCount = 64;

    void Start()
    {
        terrainLayerMask = 1 << LayerMask.NameToLayer("Terrain");

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        renderer = GetComponent<MeshRenderer>();
        topLineRenderer = GetComponent<LineRenderer>();
        bottomLineRenderer = transform.Find("BottomLineRenderer")
                                      .GetComponent<LineRenderer>();

        m1Top = marker1.transform.Find("Top");
        m2Top = marker2.transform.Find("Top");

    }

    void Update()
    {
        // Check if this should be drawn or not
        if(marker1.activeSelf && marker2.activeSelf)
        {
            Draw();
        }
        else
        {
            DisableRendering();
        }
    }

    public void DisableRendering()
    {
        renderer.enabled = false;
        topLineRenderer.enabled = false;
        bottomLineRenderer.enabled = false;
    }

    public void Draw()
    {
        renderer.enabled = true;
        topLineRenderer.enabled = true;
        bottomLineRenderer.enabled = false;


        // build mesh
        Vector3 m1bottom = marker1.transform.position;
        m1bottom.y -= 300;

        Vector3 m2bottom = marker2.transform.position;
        m2bottom.y -= 300;

        Vector3[] newVertices = { m1Top.position, m2Top.position, m1bottom, m2bottom };

        Vector2[] newUV = {new Vector2(0,1),
                           new Vector2(1,1),
                           new Vector2(0,0),
                           new Vector2(1,0)};

        int[] newTriangles = { 0, 1, 2, 2, 1, 3 };

        mesh.Clear();
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;

        // Draw top line renderer
        topLineRenderer.SetPosition(0, m1Top.position);
        topLineRenderer.SetPosition(1, m2Top.position);

        // Draw bottom line renderer
        var m1_to_m2 = (m1Top.position - m2Top.position) / (lineRendererPositionCount - 1);

        for (int i = 0; i < lineRendererPositionCount; i++)
        {
            var newPos = m1Top.position - m1_to_m2 * i;

            RaycastHit hitInfo;
            if (Physics.Raycast(newPos, Vector3.down, out hitInfo, terrainLayerMask))
            {
                newPos.y -= hitInfo.distance - 0.3f;
                bottomLineRenderer.SetPosition(i, newPos);
            }
        }

    }
}
