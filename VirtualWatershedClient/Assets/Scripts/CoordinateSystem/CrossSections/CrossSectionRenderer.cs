using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CrossSectionRenderer : MonoBehaviour 
{
    private const float nanf = float.NaN;

    // dem parameters
    private float[,] demData; // in meters
    private float[,] htmap; // in scaled units
    private Color dem_c_active = Color.white;
    private Color dem_c = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    private const float dem_w = 0.25f;
    float dem_min = 1593.0f;
    float dem_max = 1653.4f;

    private GameObject demSurf;
    private GameObject demSurfBack;
    public List<Vector3> verts = new List<Vector3>();
    public List<int> frontTris = new List<int>();
    public List<int> backTris = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();

    // z_s parameters
    private GameObject z_sSurf;
    private GameObject z_sSurfSection;
    //private GameObject z_sSurfBack;
    private float[,] z_sData; // 10 241 251
    private Color z_s_c_active = new Color(10 / 255f, 241 / 255f, 251 / 255f, 99 / 255f);
    private Color z_s_c = new Color(10 / 255f, 241 / 255f, 251 / 255f);
    private const float z_s_w = 0.125f;

    private int n = 72;
    private float xres = 2.5f;

    private Slider crossSectionSelector;
    int selected = -1;

    private bool mouseDown = false;
    private Vector3 mouseLastPos;
    private Vector3 transformLastRot;

    private GameObject slider;
    private SliderUpdate sliderUpdateScript;

    private long curStep = -1;

    private Material lineShader;

    void Start()
    {
        slider = GameObject.FindWithTag("TimeSlider");
        sliderUpdateScript = (SliderUpdate)slider.GetComponent<SliderUpdate>();

        crossSectionSelector = GameObject.FindGameObjectWithTag("CrossSectionSelector").GetComponent<Slider>();
        selected = (int)crossSectionSelector.value;

        lineShader = Resources.Load<Material>("Shaders/Particles/Additive");


        load_demdata();
        findDEMlimits();

        build_htmap();

        initialize_demSurf();
        update_demSurf();

        initialize_z_sSurf();

        transform.rotation = Quaternion.Euler(331f, 23f, 0f);
//        OnSelectionChange();
    }


    void findDEMlimits()
    {
        dem_min = 1e38f;
        dem_max = -1e38f;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - 1; j++)
            {
                float val = demData[i, j];
                if (val < dem_min)
                    dem_min = val;

                if (val > dem_max)
                    dem_max = val;
            }
        }
    }

    float scaleY(float val)
    {
        return (val - dem_min) / (n * xres);
        //float aspect = (dem_max - dem_min) / (n * xres);
        //return (val - dem_min) / (dem_max - dem_min) / 2f * aspect - (1f - aspect) / 2;
    }

    void build_htmap() 
    {
        htmap = new float[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                htmap[i, j] = scaleY(demData[i, j]);
            }
        }
    }


    void initialize_demSurf()
    {

        demSurf = new GameObject("DEM Surf");
        demSurf.transform.parent = transform;
        demSurf.transform.localRotation = Quaternion.Euler(0, 0, 0);
        demSurf.transform.localPosition = new Vector3(0, 0, 0);
        demSurf.transform.localScale = new Vector3(1, 1, 1);

        MeshFilter meshFilter = demSurf.AddComponent<MeshFilter>();
        meshFilter.mesh.Clear();

        MeshRenderer meshRenderer = demSurf.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/TransparentBumpedSpecular");
        meshRenderer.material.color = Color.white;
        meshRenderer.material.SetColor("_SpecColor", new Color(0.7f, 0.7f, 0.7f));
        meshRenderer.material.SetFloat("_Shininess", 1f);

        demSurfBack = new GameObject("DEM Surf Back");
        demSurfBack.transform.parent = transform;
        demSurfBack.transform.localRotation = Quaternion.Euler(0, 0, 0);
        demSurfBack.transform.localPosition = new Vector3(0, 0, 0);
        demSurfBack.transform.localScale = new Vector3(1, 1, 1);

        MeshFilter meshFilterBack = demSurfBack.AddComponent<MeshFilter>();
        meshFilterBack.mesh.Clear();

        MeshRenderer meshRendererBack = demSurfBack.AddComponent<MeshRenderer>();
        meshRendererBack.material = Resources.Load<Material>("Materials/TransparentSpecular");
        meshRendererBack.material.color = new Color(245f / 255f, 245f / 255f, 220f / 255f);
        meshRendererBack.material.SetColor("_SpecColor", Color.white);
    }

    void update_demSurf()
    {
        float res = 1f / n;
        float d = res * (n - 1f) - 0.5f;
        float uv_d = 1f / n;
        float uv_offset = uv_d / 2f;

        verts = new List<Vector3>();
        frontTris = new List<int>();
        backTris = new List<int>();
        uvs = new List<Vector2>();

        int k = 0;
        for (int i = 0; i < selected; i++)
        {
            for (int j = 0; j < n - 1; j++)
            {
                if (!float.IsNaN(demData[i, j]) &
                    !float.IsNaN(demData[i, j + 1]) &
                    !float.IsNaN(demData[i + 1, j]) &
                    !float.IsNaN(demData[i + 1, j + 1]))
                {
                    //verts.Add(new Vector3(res * j, 0f, d - res * (i + 1)));
                    //verts.Add(new Vector3(res * j, 0f, d - res * i));
                    //verts.Add(new Vector3(res * (j + 1), 0f, d - res * i));
                    //verts.Add(new Vector3(res * (j + 1), 0f, d - res * (i + 1)));

                    verts.Add(new Vector3(res * j - 0.5f, htmap[i + 1, j], d - res * (i + 1)));
                    verts.Add(new Vector3(res * j - 0.5f, htmap[i, j], d - res * i));
                    verts.Add(new Vector3(res * (j + 1) - 0.5f, htmap[i, j + 1], d - res * i));
                    verts.Add(new Vector3(res * (j + 1) - 0.5f, htmap[i + 1, j + 1], d - res * (i + 1)));

                    frontTris.Add(k);
                    frontTris.Add(k + 1);
                    frontTris.Add(k + 2);

                    frontTris.Add(k + 2);
                    frontTris.Add(k + 3);
                    frontTris.Add(k);

                    backTris.Add(k + 2);
                    backTris.Add(k + 1);
                    backTris.Add(k);

                    backTris.Add(k);
                    backTris.Add(k + 3);
                    backTris.Add(k + 2);

                    uvs.Add(new Vector2(uv_offset + uv_d * j, 1f - (uv_offset + uv_d * (i + 1f))));
                    uvs.Add(new Vector2(uv_offset + uv_d * j, 1f - (uv_offset + uv_d * i)));
                    uvs.Add(new Vector2(uv_offset + uv_d * (j + 1f), 1f - (uv_offset + uv_d * i)));
                    uvs.Add(new Vector2(uv_offset + uv_d * (j + 1f), 1f - (uv_offset + uv_d * (i + 1f))));

                    k += 4;
                }
            }
        }

        MeshFilter meshFilter = demSurf.GetComponent<MeshFilter>();
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = verts.ToArray();
        meshFilter.mesh.triangles = frontTris.ToArray();
        meshFilter.mesh.uv = uvs.ToArray();
        meshFilter.mesh.Optimize();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.tangents = RecalculateTangents(meshFilter.mesh);

        MeshFilter meshFilterBack = demSurfBack.GetComponent<MeshFilter>();
        meshFilterBack.mesh.Clear();
        meshFilterBack.mesh.vertices = verts.ToArray();
        meshFilterBack.mesh.triangles = backTris.ToArray();
        meshFilterBack.mesh.uv = uvs.ToArray();
        meshFilterBack.mesh.Optimize();
        meshFilterBack.mesh.RecalculateNormals();
        meshFilterBack.mesh.tangents = RecalculateTangents(meshFilter.mesh);
    }

    Vector4[] RecalculateTangents(Mesh theMesh)
    {
        int vertexCount = theMesh.vertexCount;
        Vector3[] vertices = theMesh.vertices;
        Vector3[] normals = theMesh.normals;
        Vector2[] texcoords = theMesh.uv;
        int[] triangles = theMesh.triangles;
        int triangleCount = triangles.Length / 3;
        Vector4[] tangents = new Vector4[vertexCount];
        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        int tri = 0;
        int i1, i2, i3;
        Vector3 v1, v2, v3;
        Vector2 w1, w2, w3;
        float x1, x2, y1, y2, z1, z2, s1, s2, t1, t2, r;
        Vector3 sdir, tdir;
        for (int i = 0; i < (triangleCount); i++)
        {
            i1 = triangles[tri];
            i2 = triangles[tri + 1];
            i3 = triangles[tri + 2];

            v1 = vertices[i1];
            v2 = vertices[i2];
            v3 = vertices[i3];

            w1 = texcoords[i1];
            w2 = texcoords[i2];
            w3 = texcoords[i3];

            x1 = v2.x - v1.x;
            x2 = v3.x - v1.x;
            y1 = v2.y - v1.y;
            y2 = v3.y - v1.y;
            z1 = v2.z - v1.z;
            z2 = v3.z - v1.z;

            s1 = w2.x - w1.x;
            s2 = w3.x - w1.x;
            t1 = w2.y - w1.y;
            t2 = w3.y - w1.y;

            r = 1.0f / (s1 * t2 - s2 * t1);
            sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;

            tri += 3;
        }

        Vector3 n;
        Vector3 t;
        for (int i = 0; i < (vertexCount); i++)
        {
            n = normals[i];
            t = tan1[i];

            // Gram-Schmidt orthogonalize
            Vector3.OrthoNormalize(ref n, ref t);

            tangents[i].x = t.x;
            tangents[i].y = t.y;
            tangents[i].z = t.z;

            // Calculate handedness
            tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
        }
        return tangents;
    }

    private static Mesh CloneMesh(Mesh mesh)
    {
        Mesh clone = new Mesh();
        clone.vertices = mesh.vertices;
        clone.normals = mesh.normals;
        clone.tangents = mesh.tangents;
        clone.triangles = mesh.triangles;
        clone.uv = mesh.uv;
        clone.uv2 = mesh.uv2;
        clone.uv2 = mesh.uv2;
        clone.bindposes = mesh.bindposes;
        clone.boneWeights = mesh.boneWeights;
        clone.bounds = mesh.bounds;
        clone.colors = mesh.colors;
        clone.name = mesh.name;
        //TODO : Are we missing anything?
        return clone;
    }

    void initialize_z_sSurf()
    {

        z_sSurf = new GameObject("z_s Surf");
        z_sSurf.transform.parent = transform;
        z_sSurf.transform.localRotation = Quaternion.Euler(0, 0, 0);
        z_sSurf.transform.localPosition = new Vector3(0, 0, 0);
        z_sSurf.transform.localScale = new Vector3(1, 1, 1);

        MeshFilter meshFilter = z_sSurf.AddComponent<MeshFilter>();
        meshFilter.mesh.Clear();

        MeshRenderer meshRenderer = z_sSurf.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/TransparentSpecular");
        meshRenderer.material.color = z_s_c_active;
        meshRenderer.material.SetColor("_SpecColor", Color.white);

        z_sSurfSection = new GameObject("z_s Surf Section");
        z_sSurfSection.transform.parent = transform;
        z_sSurfSection.transform.localRotation = Quaternion.Euler(0, 0, 0);
        z_sSurfSection.transform.localPosition = new Vector3(0, 0, 0);
        z_sSurfSection.transform.localScale = new Vector3(1, 1, 1);

        MeshFilter meshFilterSection = z_sSurfSection.AddComponent<MeshFilter>();
        meshFilterSection.mesh.Clear();

        MeshRenderer meshRendererSection = z_sSurfSection.AddComponent<MeshRenderer>();
        meshRendererSection.material = Resources.Load<Material>("Materials/TransparentSpecular");
        meshRendererSection.material.color = z_s_c;
        meshRendererSection.material.SetColor("_SpecColor", Color.white);

        //z_sSurfBack = new GameObject("z_s Surf Back");
        //z_sSurfBack.transform.parent = transform;
        //z_sSurfBack.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //z_sSurfBack.transform.localPosition = new Vector3(0, 0, 0);
        //z_sSurfBack.transform.localScale = new Vector3(1, 1, 1);

        //MeshFilter meshFilterBack = z_sSurfBack.AddComponent<MeshFilter>();
        //meshFilterBack.mesh.Clear();

        //MeshRenderer meshRendererBack = z_sSurfBack.AddComponent<MeshRenderer>();
        //meshRendererBack.material = Resources.Load<Material>("Materials/TransparentSpecular");
        //meshRendererBack.material.color = z_s_c_active;
        //meshRendererBack.material.SetColor("_SpecColor", Color.white);
    }

    void update_z_s()
    {
        string path = string.Format("ISNOBAL/z_s/z_s.{0:D4}", curStep);
        TextAsset byteData = Resources.Load<TextAsset>(path);

        // no snow at current step
        if (byteData == null)
        {
            z_sSurf.GetComponent<MeshFilter>().mesh.Clear();
            z_sSurfSection.GetComponent<MeshFilter>().mesh.Clear();
            return;
        }


        // snow is present, load data
        z_sData = loadTextAsset(byteData);

        // need to redefine vertices
        float res = 1f / n;
        float d = res * (n - 1f) - 0.5f;
        float uv_d = 1f / n;
        float uv_offset = uv_d / 2f;

        // scale all the data once
        float[,] data = new float[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (float.IsNaN(demData[i, j]))
                {
                    data[i, j] = nanf;
                }
                else
                {
                    data[i, j] = scaleY(demData[i, j] + 10f * z_sData[i, j] - 0.01f);
                }
            }
        }


        // build vertex list
        List<Vector3> newVerts = new List<Vector3>();
        List<int> newTris = new List<int>();
        //List<int> newTriBacks = new List<int>();
        List<Vector2> newUVs = new List<Vector2>();

        int k = 0;
        for (int i = 0; i < selected; i++)
        {
            for (int j = 0; j < n - 1; j++)
            {
                if (!float.IsNaN(data[i, j]) &
                    !float.IsNaN(data[i, j + 1]) &
                    !float.IsNaN(data[i + 1, j]) &
                    !float.IsNaN(data[i + 1, j + 1]))
                {
                    newVerts.Add(new Vector3(res * j - 0.5f, data[i + 1, j], d - res * (i + 1)));
                    newVerts.Add(new Vector3(res * j - 0.5f, data[i, j], d - res * i));
                    newVerts.Add(new Vector3(res * (j + 1) - 0.5f, data[i, j + 1], d - res * i));
                    newVerts.Add(new Vector3(res * (j + 1) - 0.5f, data[i + 1, j + 1], d - res * (i + 1)));

                    newTris.Add(k);
                    newTris.Add(k + 1);
                    newTris.Add(k + 2);

                    newTris.Add(k + 2);
                    newTris.Add(k + 3);
                    newTris.Add(k);

                    //newTriBacks.Add(k + 2);
                    //newTriBacks.Add(k + 1);
                    //newTriBacks.Add(k);

                    //newTriBacks.Add(k);
                    //newTriBacks.Add(k + 3);
                    //newTriBacks.Add(k + 2);

                    newUVs.Add(new Vector2(uv_offset + uv_d * j, 1f - (uv_offset + uv_d * (i + 1f))));
                    newUVs.Add(new Vector2(uv_offset + uv_d * j, 1f - (uv_offset + uv_d * i)));
                    newUVs.Add(new Vector2(uv_offset + uv_d * (j + 1f), 1f - (uv_offset + uv_d * i)));
                    newUVs.Add(new Vector2(uv_offset + uv_d * (j + 1f), 1f - (uv_offset + uv_d * (i + 1f))));

                    k += 4;
                }
            }
        }

//        Debug.Log(newVerts.Count.ToString() + ", " + uvs.Count.ToString());

        // rebuild mesh
        MeshFilter meshFilter = z_sSurf.GetComponent<MeshFilter>();
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = newVerts.ToArray();
        meshFilter.mesh.triangles = newTris.ToArray();
        meshFilter.mesh.uv = newUVs.ToArray();
        meshFilter.mesh.Optimize();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.tangents = RecalculateTangents(meshFilter.mesh);


        // build vertex list
        List<Vector3> sectionVerts = new List<Vector3>();
        List<int> sectionTris = new List<int>();
        //List<int> newTriBacks = new List<int>();
        List<Vector2> sectionUVs = new List<Vector2>();


        float z = d - res * selected;

        k = 0;
        for (int j = 0; j < n - 1; j++) 
        {
            if (!float.IsNaN(data[selected, j]) & !float.IsNaN(data[selected, j+1]))
            {
                float x0 = res * j - 0.5f;
                float x1 = res * (j + 1) - 0.5f;

                sectionVerts.Add(new Vector3(x0, htmap[selected, j], z));
                sectionVerts.Add(new Vector3(x0, data[selected, j], z));
                sectionVerts.Add(new Vector3(x1, data[selected, j + 1], z));
                sectionVerts.Add(new Vector3(x1, htmap[selected, j + 1], z));

                sectionTris.Add(k);
                sectionTris.Add(k + 1);
                sectionTris.Add(k + 2);

                sectionTris.Add(k + 2);
                sectionTris.Add(k + 3);
                sectionTris.Add(k);

                sectionUVs.Add(new Vector2(uv_offset + uv_d * j, 1f - (uv_offset + uv_d * (selected + 1f))));
                sectionUVs.Add(new Vector2(uv_offset + uv_d * j, 1f - (uv_offset + uv_d * selected)));
                sectionUVs.Add(new Vector2(uv_offset + uv_d * (j + 1f), 1f - (uv_offset + uv_d * selected)));
                sectionUVs.Add(new Vector2(uv_offset + uv_d * (j + 1f), 1f - (uv_offset + uv_d * (selected + 1f))));

                k += 4;
            }
        }

        meshFilter = z_sSurfSection.GetComponent<MeshFilter>();
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = sectionVerts.ToArray();
        meshFilter.mesh.triangles = sectionTris.ToArray();
        meshFilter.mesh.uv = sectionUVs.ToArray();
        meshFilter.mesh.Optimize();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.tangents = RecalculateTangents(meshFilter.mesh);
    }



    void Update()
    {
        if (curStep != sliderUpdateScript.SimStepProperty)
        {
            curStep = sliderUpdateScript.SimStepProperty;
            update_z_s();
        }

        if (mouseDown)
        {
            Vector3 curPos = Input.mousePosition;
            transform.rotation = Quaternion.Euler(transformLastRot.x + (curPos.y - mouseLastPos.y),
                                                  transformLastRot.y - (curPos.x - mouseLastPos.x),
                                                  transformLastRot.z);
        }
    }

    public void OnSelectionChange()
    {
        selected = (int)crossSectionSelector.value;
        update_z_s();
        update_demSurf();

        //for (int i = 0; i < n; i++)
        //{
        //    if (selected == i)
        //    {
        //        demLines[i].GetComponent<LineRenderer>().SetColors(dem_c_active, dem_c_active);
        //    }
        //    else
        //    {
        //        demLines[i].GetComponent<LineRenderer>().SetColors(dem_c, dem_c);
        //    }
        //}
    }

    void OnMouseDown()
    {
        mouseDown = true;
        mouseLastPos = Input.mousePosition;
        transformLastRot = transform.eulerAngles;
    }

    void OnMouseUp()
    {
        mouseDown = false;
    }

    float[,] loadTextAsset(TextAsset byteData)
    {
        
        float[,] data = new float[n, n];
        int k = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                data[i, j] = System.BitConverter.ToSingle(byteData.bytes, k);
                k += 4;
            }
        }
        return data;
    }

    void load_demdata()
    {
        demData = new float[,] {
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1652.5f,1652.8f,1652.9f,1652.8f,1652.6f,1652.3f,1652.1f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1652.1f,1652.3f,1652.5f,1652.7f,1652.6f,1652.4f,1652.2f,1651.7f,1651.3f,1651.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1651.6f,1651.7f,1651.9f,1651.9f,1651.9f,1651.8f,1651.6f,1651.2f,1650.7f,1650.4f,1649.9f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1651.1f,1651.2f,1651.3f,1651.4f,1651.4f,1651.4f,1651.3f,1651.2f,1650.7f,1650.4f,1650.1f,1649.6f,1649.4f,1649.0f,1648.8f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1650.4f,1650.6f,1650.5f,1650.6f,1650.4f,1650.3f,1650.0f,1650.0f,1649.7f,1649.5f,1649.2f,1648.6f,1648.6f,1648.5f,1648.3f,1648.2f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1650.1f,1650.1f,1650.2f,1650.1f,1649.8f,1649.6f,1649.3f,1649.1f,1649.0f,1648.7f,1648.6f,1648.0f,1647.8f,1647.8f,1647.9f,1648.0f,1647.4f,1646.7f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1649.3f,1649.4f,1649.4f,1649.4f,1649.2f,1648.8f,1648.4f,1647.7f,1647.3f,1647.4f,1647.2f,1647.0f,1646.8f,1646.5f,1646.8f,1647.2f,1647.4f,1646.9f,1646.1f,1645.8f,1645.2f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1648.8f,1648.9f,1649.0f,1648.9f,1648.8f,1648.6f,1648.3f,1647.7f,1646.9f,1646.6f,1646.5f,1646.4f,1646.2f,1646.0f,1645.8f,1646.1f,1646.4f,1646.6f,1646.4f,1645.7f,1645.3f,1644.9f,1644.4f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1648.4f,1648.4f,1648.5f,1648.4f,1648.3f,1648.0f,1647.7f,1647.2f,1646.2f,1645.8f,1645.7f,1645.5f,1645.4f,1645.1f,1645.2f,1645.3f,1645.4f,1645.7f,1645.5f,1645.1f,1644.9f,1644.5f,1644.1f,1643.8f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1647.6f,1647.7f,1647.8f,1647.8f,1647.6f,1647.4f,1647.1f,1646.6f,1646.2f,1645.4f,1644.7f,1644.1f,1644.0f,1643.6f,1643.6f,1643.6f,1644.0f,1644.2f,1644.3f,1644.3f,1644.1f,1644.0f,1643.8f,1643.4f,1643.3f,1642.9f,1642.6f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1647.3f,1647.3f,1647.4f,1647.4f,1647.2f,1646.9f,1646.5f,1646.1f,1645.5f,1644.8f,1644.0f,1643.3f,1643.0f,1642.7f,1642.6f,1642.6f,1643.0f,1643.4f,1643.6f,1643.6f,1643.5f,1643.3f,1643.0f,1642.9f,1642.8f,1642.6f,1642.3f,1641.6f,1641.1f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1646.5f,1646.5f,1646.7f,1646.7f,1646.6f,1646.3f,1646.1f,1645.6f,1645.1f,1644.5f,1643.7f,1642.8f,1642.0f,1641.5f,1641.3f,1641.2f,1641.1f,1641.5f,1642.0f,1642.4f,1642.4f,1642.1f,1641.9f,1641.7f,1641.8f,1641.8f,1641.8f,1641.5f,1641.1f,1640.8f,1640.2f,1639.7f,1639.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1646.1f,1646.2f,1646.2f,1646.3f,1646.2f,1645.7f,1645.4f,1645.0f,1644.5f,1643.9f,1643.1f,1642.2f,1641.5f,1640.7f,1640.4f,1640.2f,1640.3f,1640.6f,1641.0f,1641.5f,1641.5f,1641.2f,1641.1f,1640.9f,1641.0f,1641.1f,1641.1f,1641.0f,1640.6f,1640.5f,1639.8f,1639.5f,1638.8f,1638.3f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1645.4f,1645.5f,1645.5f,1645.5f,1645.4f,1645.2f,1644.8f,1644.4f,1644.1f,1643.3f,1642.7f,1641.9f,1641.2f,1640.5f,1639.6f,1638.9f,1638.4f,1638.4f,1639.2f,1639.5f,1639.8f,1639.8f,1639.6f,1639.6f,1639.6f,1639.7f,1639.9f,1640.0f,1640.0f,1639.8f,1639.4f,1638.8f,1638.6f,1638.3f,1637.8f,1637.4f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1645.0f,1645.1f,1645.2f,1645.2f,1645.1f,1644.9f,1644.7f,1644.3f,1643.9f,1643.4f,1642.8f,1642.1f,1641.4f,1640.7f,1639.8f,1638.9f,1638.4f,1637.5f,1637.5f,1638.2f,1638.5f,1638.8f,1638.9f,1638.7f,1638.6f,1638.7f,1638.8f,1639.1f,1639.4f,1639.4f,1639.2f,1638.9f,1638.3f,1638.0f,1637.8f,1637.5f,1637.2f,1636.9f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,1644.3f,1644.5f,1644.6f,1644.6f,1644.5f,1644.3f,1644.3f,1644.0f,1643.5f,1643.0f,1642.5f,1641.8f,1641.1f,1640.3f,1639.5f,1638.6f,1637.8f,1636.9f,1636.1f,1636.2f,1637.0f,1637.2f,1637.5f,1637.5f,1637.3f,1637.0f,1637.1f,1637.4f,1638.0f,1638.3f,1638.4f,1638.2f,1638.0f,1637.5f,1637.2f,1636.7f,1636.6f,1636.5f,1636.5f,1636.3f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,1644.2f,1644.2f,1644.2f,1644.2f,1644.0f,1644.0f,1643.8f,1643.4f,1643.0f,1642.6f,1642.0f,1641.3f,1640.6f,1639.8f,1638.9f,1637.9f,1636.9f,1636.2f,1635.2f,1635.5f,1636.2f,1636.4f,1636.7f,1636.7f,1636.4f,1635.9f,1636.0f,1636.7f,1637.2f,1637.5f,1637.7f,1637.7f,1637.5f,1637.0f,1636.7f,1636.3f,1636.1f,1636.1f,1636.1f,1636.1f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,1643.7f,1643.7f,1643.7f,1643.7f,1643.4f,1643.3f,1642.9f,1642.7f,1642.3f,1641.8f,1641.4f,1641.0f,1640.4f,1639.5f,1638.7f,1637.8f,1636.8f,1635.9f,1635.2f,1634.0f,1633.9f,1634.8f,1635.0f,1635.3f,1635.3f,1634.9f,1634.7f,1634.8f,1635.7f,1636.1f,1636.5f,1636.7f,1636.7f,1636.7f,1636.2f,1635.9f,1635.4f,1635.2f,1635.2f,1635.5f,1635.6f,1635.4f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,1643.3f,1643.5f,1643.5f,1643.4f,1643.3f,1642.9f,1642.6f,1642.4f,1641.9f,1641.5f,1640.9f,1640.6f,1640.0f,1639.5f,1638.9f,1638.0f,1637.2f,1636.3f,1635.3f,1634.6f,1633.6f,1633.2f,1633.9f,1634.2f,1634.3f,1634.3f,1634.2f,1634.0f,1634.4f,1635.1f,1635.6f,1635.9f,1636.0f,1636.1f,1636.2f,1635.9f,1635.4f,1634.9f,1634.6f,1634.6f,1635.0f,1635.2f,1635.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,1643.0f,1643.1f,1643.1f,1643.0f,1642.8f,1642.6f,1642.1f,1641.7f,1641.1f,1640.6f,1640.1f,1639.6f,1639.1f,1638.5f,1638.2f,1637.8f,1637.0f,1636.2f,1635.3f,1634.4f,1633.8f,1632.8f,1632.1f,1632.2f,1632.6f,1633.0f,1633.0f,1633.0f,1632.8f,1633.5f,1634.2f,1634.5f,1634.8f,1634.7f,1634.7f,1634.9f,1634.4f,1634.4f,1633.7f,1633.3f,1633.6f,1634.3f,1634.3f,1634.3f,1634.3f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,1643.0f,1643.1f,1643.0f,1642.8f,1642.5f,1642.2f,1641.6f,1641.1f,1640.6f,1639.9f,1639.4f,1638.9f,1638.4f,1637.8f,1637.3f,1637.0f,1636.2f,1635.6f,1634.8f,1633.9f,1633.3f,1632.4f,1631.7f,1631.3f,1631.7f,1632.2f,1632.2f,1632.2f,1632.1f,1633.0f,1633.7f,1634.0f,1634.1f,1634.1f,1633.8f,1634.7f,1633.4f,1633.4f,1633.3f,1632.7f,1633.0f,1633.5f,1633.8f,1633.9f,1633.9f,1633.8f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,1642.9f,1642.9f,1642.7f,1642.5f,1642.0f,1641.7f,1640.9f,1640.5f,1639.7f,1639.2f,1638.6f,1637.9f,1637.2f,1636.5f,1635.8f,1635.5f,1635.0f,1634.3f,1633.6f,1633.0f,1632.5f,1631.5f,1630.9f,1630.0f,1630.1f,1630.7f,1630.8f,1630.8f,1630.9f,1631.9f,1632.8f,1633.0f,1633.0f,1632.8f,1632.3f,1632.0f,1631.7f,1631.4f,1631.4f,1631.6f,1632.0f,1632.4f,1632.9f,1633.3f,1633.4f,1633.4f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,1642.8f,1642.8f,1642.5f,1642.3f,1641.8f,1641.4f,1640.6f,1640.1f,1639.4f,1638.7f,1638.0f,1637.2f,1636.5f,1635.8f,1635.1f,1634.6f,1634.2f,1633.6f,1633.0f,1632.4f,1631.9f,1631.0f,1630.5f,1629.4f,1629.3f,1630.0f,1630.0f,1629.8f,1630.1f,1631.2f,1632.2f,1632.4f,1632.3f,1632.1f,1631.6f,1631.3f,1630.9f,1630.6f,1630.7f,1631.0f,1631.4f,1631.9f,1632.3f,1632.9f,1633.1f,1633.1f,1633.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,1642.7f,1642.6f,1642.3f,1642.1f,1641.7f,1641.2f,1640.3f,1639.7f,1638.8f,1638.1f,1637.2f,1636.4f,1635.6f,1634.8f,1634.0f,1633.4f,1632.9f,1632.4f,1631.9f,1631.4f,1630.9f,1630.2f,1629.8f,1628.8f,1628.3f,1628.6f,1628.8f,1628.6f,1628.9f,1630.0f,1630.7f,1631.0f,1631.1f,1630.8f,1630.3f,1630.0f,1629.6f,1629.5f,1629.5f,1629.9f,1630.5f,1631.0f,1631.5f,1632.1f,1632.4f,1632.5f,1632.4f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,1642.6f,1642.6f,1642.2f,1642.0f,1641.6f,1641.1f,1640.1f,1639.5f,1638.6f,1637.7f,1636.8f,1635.8f,1635.2f,1634.5f,1633.6f,1632.9f,1632.2f,1631.7f,1631.2f,1630.7f,1630.2f,1629.6f,1629.3f,1628.4f,1627.9f,1627.9f,1627.9f,1627.9f,1628.5f,1629.3f,1629.8f,1630.0f,1630.0f,1629.9f,1629.6f,1629.3f,1628.9f,1628.8f,1628.9f,1629.3f,1630.0f,1630.4f,1631.0f,1631.5f,1631.9f,1632.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,1642.6f,1642.5f,1642.2f,1641.8f,1641.3f,1640.9f,1639.9f,1639.2f,1638.4f,1637.5f,1636.5f,1635.4f,1634.7f,1633.7f,1632.9f,1632.1f,1631.4f,1630.8f,1630.0f,1629.5f,1629.1f,1628.6f,1628.3f,1627.6f,1627.1f,1626.4f,1626.7f,1627.0f,1627.5f,1628.2f,1628.6f,1628.7f,1628.7f,1628.5f,1628.1f,1627.9f,1627.7f,1627.5f,1627.6f,1628.1f,1628.8f,1629.5f,1630.0f,1630.8f,1631.0f,1631.2f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,1642.5f,1642.4f,1642.1f,1641.7f,1641.2f,1640.7f,1639.7f,1639.2f,1638.3f,1637.3f,1636.3f,1635.1f,1634.4f,1633.5f,1632.6f,1631.8f,1630.9f,1630.1f,1629.3f,1628.7f,1628.3f,1627.7f,1627.2f,1626.7f,1626.4f,1625.6f,1625.3f,1626.2f,1626.6f,1627.1f,1627.4f,1627.6f,1627.4f,1627.2f,1627.0f,1626.9f,1626.6f,1626.6f,1626.8f,1627.4f,1628.0f,1628.8f,1629.3f,1630.1f,1630.4f,1630.5f,1630.6f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,1642.4f,1642.0f,1641.7f,1641.0f,1640.5f,1639.8f,1639.2f,1638.2f,1637.2f,1636.1f,1635.1f,1634.4f,1633.5f,1632.4f,1631.6f,1630.8f,1629.7f,1628.7f,1627.9f,1627.4f,1626.8f,1626.3f,1625.8f,1625.5f,1624.8f,1624.4f,1625.2f,1625.7f,1626.1f,1626.4f,1626.3f,1626.2f,1626.1f,1625.9f,1625.9f,1625.7f,1625.7f,1626.0f,1626.7f,1627.3f,1628.0f,1628.4f,1629.1f,1629.6f,1629.9f,1629.9f,1629.9f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,1642.1f,1641.8f,1641.5f,1640.9f,1640.5f,1639.7f,1639.2f,1638.2f,1637.1f,1636.1f,1635.0f,1634.4f,1633.4f,1632.5f,1631.5f,1630.6f,1629.6f,1628.6f,1627.6f,1627.0f,1626.1f,1625.7f,1625.0f,1624.7f,1624.0f,1623.5f,1624.3f,1624.6f,1625.0f,1625.1f,1625.1f,1624.9f,1624.7f,1624.6f,1624.7f,1624.6f,1624.6f,1625.3f,1625.9f,1626.5f,1627.0f,1627.4f,1628.2f,1628.7f,1629.1f,1629.2f,1629.2f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,1642.0f,1641.6f,1641.4f,1640.7f,1640.4f,1639.6f,1639.1f,1638.2f,1637.2f,1636.2f,1635.1f,1634.4f,1633.6f,1632.6f,1631.6f,1630.6f,1629.6f,1628.7f,1627.6f,1626.9f,1626.0f,1625.3f,1624.4f,1624.0f,1623.3f,1622.8f,1623.3f,1623.6f,1624.1f,1624.2f,1624.1f,1623.9f,1623.8f,1623.7f,1623.7f,1623.8f,1624.0f,1624.7f,1625.3f,1625.7f,1626.2f,1626.6f,1627.5f,1628.1f,1628.4f,1628.7f,1628.7f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,1641.5f,1641.4f,1641.1f,1640.6f,1640.3f,1639.5f,1639.0f,1638.2f,1637.3f,1636.6f,1635.5f,1634.7f,1633.6f,1632.8f,1631.8f,1630.8f,1629.8f,1628.9f,1628.0f,1627.2f,1625.8f,1625.1f,1623.7f,1622.9f,1622.1f,1621.7f,1621.8f,1622.1f,1622.5f,1622.5f,1622.5f,1622.4f,1622.2f,1622.1f,1622.0f,1622.4f,1622.8f,1623.6f,1624.1f,1624.4f,1625.0f,1625.6f,1626.5f,1627.1f,1627.5f,1627.7f,1627.8f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,1641.2f,1641.0f,1640.4f,1640.1f,1639.4f,1639.0f,1638.2f,1637.4f,1636.6f,1635.7f,1634.9f,1633.8f,1632.9f,1631.9f,1630.9f,1630.0f,1629.2f,1628.2f,1627.3f,1625.8f,1624.9f,1623.6f,1622.8f,1621.5f,1620.9f,1620.9f,1621.2f,1621.5f,1621.4f,1621.5f,1621.3f,1621.3f,1621.1f,1621.0f,1621.7f,1622.3f,1623.0f,1623.3f,1623.5f,1624.3f,1625.1f,1626.1f,1626.6f,1627.1f,1627.3f,1627.3f,1627.2f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,1640.7f,1640.5f,1640.1f,1639.9f,1639.3f,1638.9f,1638.2f,1637.4f,1636.4f,1635.5f,1634.9f,1633.9f,1633.1f,1632.1f,1631.0f,1630.2f,1629.3f,1628.3f,1627.4f,1625.9f,1625.0f,1623.5f,1622.6f,1621.5f,1620.8f,1619.7f,1619.7f,1619.9f,1619.9f,1620.0f,1619.9f,1619.9f,1619.5f,1619.6f,1620.9f,1621.3f,1622.1f,1622.3f,1622.7f,1623.6f,1624.4f,1625.3f,1625.8f,1626.3f,1626.6f,1626.6f,1626.5f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,1640.4f,1640.0f,1639.8f,1639.3f,1638.9f,1638.3f,1637.4f,1636.3f,1635.4f,1634.8f,1633.8f,1633.1f,1632.2f,1631.2f,1630.2f,1629.4f,1628.3f,1627.3f,1625.9f,1625.0f,1623.3f,1622.6f,1621.4f,1620.8f,1619.2f,1618.9f,1619.0f,1619.2f,1619.2f,1619.2f,1619.1f,1618.8f,1619.2f,1620.4f,1620.8f,1621.5f,1621.8f,1622.4f,1623.4f,1624.1f,1625.1f,1625.4f,1625.7f,1626.0f,1626.1f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,1640.0f,1639.7f,1639.5f,1639.1f,1638.8f,1638.2f,1637.3f,1636.5f,1635.4f,1634.6f,1633.8f,1633.2f,1632.3f,1631.3f,1630.2f,1629.2f,1628.1f,1627.1f,1625.8f,1624.9f,1623.3f,1622.4f,1621.2f,1620.5f,1619.1f,1618.1f,1617.4f,1618.0f,1618.1f,1617.9f,1617.8f,1617.7f,1618.5f,1619.5f,1619.8f,1620.1f,1620.5f,1621.6f,1622.8f,1623.4f,1624.3f,1624.6f,1625.0f,1625.2f,1625.3f,1625.2f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1639.5f,1639.3f,1639.0f,1638.6f,1638.2f,1637.3f,1636.4f,1635.4f,1634.7f,1633.7f,1633.1f,1632.3f,1631.3f,1630.2f,1629.1f,1628.0f,1627.0f,1625.6f,1624.6f,1623.1f,1622.3f,1620.9f,1620.0f,1618.9f,1618.0f,1616.5f,1617.0f,1617.2f,1617.2f,1617.0f,1617.3f,1618.1f,1618.8f,1619.0f,1619.2f,1620.0f,1621.3f,1622.4f,1623.1f,1623.8f,1624.1f,1624.4f,1624.7f,1624.8f,1624.7f,1624.5f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1639.0f,1638.8f,1638.4f,1638.1f,1638.0f,1636.9f,1636.2f,1635.2f,1634.5f,1633.8f,1632.9f,1632.0f,1631.0f,1629.8f,1628.7f,1627.7f,1626.8f,1625.2f,1624.4f,1622.7f,1621.9f,1620.3f,1619.4f,1618.2f,1617.4f,1616.0f,1615.3f,1615.7f,1615.6f,1615.5f,1616.6f,1617.2f,1617.7f,1617.6f,1618.3f,1619.4f,1620.6f,1621.7f,1622.4f,1623.0f,1623.2f,1623.5f,1623.7f,1623.7f,1623.6f,1623.5f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1638.7f,1638.5f,1638.0f,1637.8f,1637.5f,1636.8f,1636.0f,1635.2f,1634.5f,1633.7f,1632.8f,1631.8f,1630.8f,1629.6f,1628.6f,1627.5f,1626.6f,1625.7f,1624.4f,1622.3f,1621.3f,1619.9f,1618.9f,1617.7f,1617.0f,1615.7f,1614.5f,1614.5f,1614.6f,1615.0f,1616.2f,1616.6f,1616.8f,1616.9f,1617.8f,1619.0f,1620.2f,1621.2f,1621.7f,1622.4f,1622.7f,1622.9f,1623.1f,1623.1f,1623.0f,1622.9f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1637.8f,1637.4f,1637.2f,1636.7f,1636.3f,1635.7f,1635.0f,1634.3f,1633.5f,1632.6f,1631.5f,1630.5f,1629.3f,1628.3f,1627.3f,1626.5f,1625.3f,1624.0f,1622.0f,1620.8f,1619.3f,1618.5f,1617.3f,1616.4f,1615.3f,1614.2f,1613.5f,1613.7f,1614.3f,1615.3f,1615.5f,1615.6f,1616.0f,1616.8f,1618.0f,1619.1f,1620.2f,1620.8f,1621.5f,1621.8f,1622.0f,1622.1f,1622.2f,1622.2f,1622.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1637.1f,1636.9f,1636.4f,1636.0f,1635.5f,1634.8f,1634.2f,1633.4f,1632.5f,1631.4f,1630.3f,1629.1f,1628.2f,1627.1f,1626.2f,1624.9f,1623.7f,1621.8f,1620.6f,1619.1f,1618.3f,1617.0f,1616.3f,1615.1f,1613.9f,1613.4f,1613.1f,1613.9f,1614.5f,1614.6f,1614.8f,1615.3f,1616.3f,1617.5f,1618.6f,1619.4f,1620.0f,1620.8f,1621.2f,1621.4f,1621.6f,1621.7f,1621.5f,1621.3f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1636.8f,1636.5f,1636.2f,1635.7f,1635.2f,1634.6f,1634.1f,1633.3f,1632.4f,1631.4f,1630.1f,1628.8f,1627.8f,1626.5f,1625.8f,1624.8f,1623.5f,1621.5f,1620.4f,1618.8f,1618.1f,1616.9f,1616.2f,1615.0f,1613.7f,1613.0f,1612.4f,1613.2f,1613.7f,1613.8f,1614.1f,1614.7f,1615.8f,1617.0f,1618.0f,1618.7f,1619.3f,1620.2f,1620.6f,1620.9f,1621.1f,1621.1f,1620.9f,1620.6f,1620.3f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1635.9f,1635.6f,1635.2f,1634.7f,1634.2f,1633.6f,1633.1f,1632.2f,1631.3f,1629.6f,1628.1f,1627.2f,1625.9f,1625.2f,1623.6f,1622.8f,1621.2f,1620.3f,1618.7f,1618.0f,1616.7f,1616.1f,1614.7f,1613.6f,1612.7f,1611.6f,1611.6f,1612.4f,1612.5f,1613.3f,1614.0f,1615.3f,1616.4f,1617.1f,1617.8f,1618.3f,1619.0f,1619.4f,1619.6f,1619.9f,1619.8f,1619.8f,1619.5f,1619.1f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1635.3f,1634.9f,1634.4f,1633.9f,1633.4f,1632.8f,1632.0f,1630.9f,1629.4f,1628.1f,1626.9f,1625.7f,1625.0f,1623.4f,1622.4f,1621.1f,1620.2f,1618.6f,1617.9f,1616.6f,1615.9f,1614.6f,1613.5f,1612.7f,1611.4f,1611.0f,1611.7f,1611.8f,1612.7f,1613.6f,1614.6f,1615.6f,1616.5f,1617.1f,1617.5f,1618.3f,1618.5f,1618.8f,1618.9f,1619.0f,1619.0f,1618.8f,1618.4f,1618.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1634.3f,1633.8f,1633.3f,1632.9f,1632.2f,1631.3f,1630.3f,1629.4f,1628.1f,1626.6f,1625.5f,1624.5f,1623.1f,1622.3f,1621.1f,1620.2f,1618.8f,1618.0f,1616.6f,1615.8f,1614.7f,1613.4f,1612.6f,1611.1f,1610.2f,1610.5f,1610.9f,1611.8f,1612.4f,1613.4f,1614.2f,1615.0f,1615.8f,1616.3f,1616.9f,1617.2f,1617.5f,1617.6f,1617.6f,1617.5f,1617.3f,1617.1f,1616.8f,1616.6f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1633.5f,1632.9f,1632.5f,1631.9f,1631.0f,1630.2f,1629.1f,1627.8f,1626.5f,1625.4f,1624.6f,1623.2f,1622.4f,1621.0f,1620.2f,1618.9f,1618.1f,1616.7f,1616.1f,1614.8f,1613.5f,1612.7f,1611.1f,1610.0f,1609.9f,1610.3f,1611.0f,1611.6f,1612.4f,1613.4f,1614.1f,1614.9f,1615.4f,1616.0f,1616.3f,1616.6f,1616.8f,1616.7f,1616.5f,1616.4f,1616.3f,1616.1f,1616.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1632.3f,1631.9f,1631.2f,1630.5f,1629.6f,1628.8f,1627.8f,1626.7f,1625.6f,1624.7f,1623.3f,1622.5f,1621.2f,1620.5f,1619.1f,1618.4f,1617.1f,1616.5f,1615.1f,1613.8f,1612.9f,1611.6f,1610.5f,1609.2f,1609.1f,1609.5f,1610.0f,1610.7f,1611.6f,1612.4f,1613.1f,1613.8f,1614.6f,1614.9f,1615.2f,1615.3f,1615.3f,1615.1f,1615.0f,1614.9f,1614.7f,1614.6f,1614.1f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1631.5f,1630.9f,1630.1f,1629.5f,1628.6f,1627.7f,1626.7f,1625.7f,1624.9f,1623.5f,1622.7f,1621.4f,1620.7f,1619.5f,1618.6f,1617.3f,1616.6f,1615.4f,1614.1f,1613.3f,1611.9f,1610.9f,1609.0f,1608.4f,1608.3f,1608.8f,1609.7f,1610.7f,1611.4f,1612.2f,1612.9f,1613.8f,1614.0f,1614.3f,1614.4f,1614.4f,1614.3f,1614.2f,1614.1f,1613.8f,1613.7f,1613.2f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1630.4f,1629.8f,1629.1f,1628.4f,1627.6f,1626.7f,1625.7f,1625.1f,1624.0f,1623.3f,1622.1f,1621.3f,1620.1f,1619.3f,1617.9f,1617.1f,1615.8f,1614.6f,1613.7f,1612.5f,1611.6f,1610.1f,1609.0f,1607.3f,1606.8f,1607.8f,1609.0f,1609.8f,1610.6f,1611.2f,1612.1f,1612.5f,1612.8f,1612.9f,1612.9f,1612.9f,1612.8f,1612.6f,1612.4f,1612.1f,1611.8f,1611.5f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1629.6f,1628.9f,1628.2f,1627.5f,1626.7f,1625.9f,1625.3f,1624.1f,1623.5f,1622.4f,1621.8f,1620.5f,1619.8f,1618.4f,1617.6f,1616.2f,1615.1f,1614.2f,1612.9f,1612.0f,1610.7f,1609.8f,1608.1f,1607.0f,1606.6f,1607.9f,1608.8f,1609.6f,1610.2f,1611.0f,1611.4f,1611.6f,1611.7f,1611.9f,1611.9f,1611.9f,1611.7f,1611.6f,1611.4f,1611.1f,1610.8f,1610.2f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1627.1f,1626.6f,1625.9f,1625.3f,1624.4f,1623.8f,1622.8f,1622.1f,1620.9f,1620.3f,1619.0f,1618.3f,1617.0f,1615.6f,1614.8f,1613.6f,1612.8f,1611.4f,1610.6f,1609.2f,1608.3f,1606.9f,1606.0f,1607.0f,1607.9f,1608.5f,1609.2f,1609.6f,1610.0f,1610.1f,1610.1f,1610.1f,1610.1f,1610.0f,1610.0f,1609.9f,1609.7f,1609.4f,1609.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1626.3f,1625.8f,1625.1f,1624.3f,1623.9f,1622.8f,1622.1f,1621.1f,1620.4f,1619.3f,1618.6f,1617.4f,1615.9f,1615.2f,1614.1f,1613.2f,1611.8f,1611.0f,1609.8f,1608.9f,1607.6f,1606.0f,1606.0f,1606.9f,1607.5f,1608.4f,1608.8f,1609.1f,1609.3f,1609.1f,1608.9f,1608.8f,1608.8f,1608.8f,1608.7f,1608.6f,1608.4f,1608.0f,1607.9f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1624.8f,1624.1f,1623.7f,1622.9f,1622.3f,1621.2f,1620.7f,1619.7f,1619.0f,1617.9f,1616.5f,1615.7f,1614.7f,1613.9f,1612.6f,1611.7f,1610.4f,1609.6f,1608.5f,1607.1f,1605.6f,1605.1f,1605.9f,1606.8f,1607.2f,1607.4f,1607.3f,1607.1f,1606.7f,1606.6f,1606.6f,1606.7f,1606.7f,1606.8f,1606.7f,1606.2f,1606.0f,1605.6f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1623.9f,1623.5f,1622.8f,1622.1f,1621.3f,1620.8f,1619.8f,1619.2f,1618.1f,1616.8f,1616.1f,1614.9f,1614.2f,1613.1f,1612.2f,1610.9f,1610.1f,1608.9f,1607.7f,1606.4f,1605.1f,1604.8f,1605.7f,1606.1f,1606.2f,1606.1f,1605.8f,1605.4f,1605.2f,1605.2f,1605.4f,1605.6f,1605.6f,1605.5f,1605.2f,1605.0f,1604.6f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1623.0f,1622.4f,1621.9f,1621.2f,1620.8f,1620.0f,1619.3f,1618.3f,1617.2f,1616.5f,1615.4f,1614.8f,1613.5f,1612.8f,1611.5f,1610.7f,1609.7f,1608.7f,1607.6f,1606.1f,1605.0f,1604.1f,1604.3f,1604.4f,1604.2f,1603.7f,1603.3f,1603.1f,1603.1f,1603.3f,1603.5f,1603.6f,1603.7f,1603.5f,1603.5f,1603.0f,1602.8f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1622.0f,1621.7f,1621.1f,1620.7f,1619.9f,1619.4f,1618.4f,1617.3f,1616.7f,1615.7f,1615.0f,1613.7f,1613.1f,1611.9f,1611.1f,1610.2f,1609.2f,1607.9f,1606.7f,1605.7f,1603.9f,1603.6f,1603.4f,1603.2f,1602.7f,1602.1f,1601.9f,1601.9f,1602.0f,1602.1f,1602.4f,1602.5f,1602.6f,1602.5f,1602.3f,1601.9f,1601.5f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1621.2f,1620.8f,1620.6f,1619.9f,1619.5f,1618.7f,1617.7f,1617.0f,1616.1f,1615.5f,1614.4f,1613.7f,1612.7f,1612.0f,1611.0f,1609.9f,1608.9f,1607.7f,1606.8f,1605.0f,1604.1f,1603.1f,1602.3f,1601.5f,1601.0f,1600.4f,1600.1f,1600.2f,1600.3f,1600.3f,1600.6f,1600.9f,1601.0f,1600.9f,1600.8f,1600.3f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1620.7f,1620.4f,1620.3f,1619.7f,1619.5f,1618.8f,1617.9f,1617.3f,1616.4f,1615.8f,1614.7f,1614.2f,1613.3f,1612.6f,1611.6f,1610.6f,1609.6f,1608.5f,1607.7f,1606.0f,1605.1f,1604.3f,1603.4f,1602.5f,1601.9f,1601.0f,1599.8f,1598.7f,1599.0f,1598.8f,1599.1f,1599.4f,1599.7f,1599.7f,1599.5f,1599.2f,1598.7f,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1619.9f,1619.6f,1619.4f,1618.8f,1618.1f,1617.7f,1616.6f,1616.1f,1615.0f,1614.5f,1613.6f,1613.1f,1612.2f,1611.2f,1610.2f,1609.2f,1608.5f,1607.0f,1606.1f,1605.2f,1604.3f,1603.5f,1602.8f,1601.9f,1600.8f,1599.5f,1598.7f,1597.3f,1597.5f,1598.0f,1598.2f,1598.4f,1598.4f,1598.2f,1597.9f,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1619.4f,1619.4f,1619.3f,1618.8f,1618.2f,1617.8f,1616.9f,1616.3f,1615.4f,1614.8f,1614.1f,1613.5f,1612.6f,1611.7f,1610.8f,1609.9f,1609.2f,1607.9f,1607.0f,1606.1f,1605.3f,1604.3f,1603.5f,1602.5f,1601.5f,1600.4f,1599.4f,1598.2f,1597.0f,1596.7f,1596.8f,1597.1f,1597.3f,1597.2f,1597.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1619.2f,1619.2f,1619.0f,1618.4f,1617.9f,1617.2f,1616.6f,1615.7f,1615.3f,1614.5f,1613.9f,1613.0f,1612.2f,1611.3f,1610.5f,1609.9f,1608.8f,1608.0f,1607.1f,1606.0f,1605.1f,1604.1f,1603.2f,1602.1f,1601.3f,1600.4f,1598.7f,1597.4f,1596.1f,1595.7f,1595.9f,1596.1f,1596.2f,1596.1f,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.9f,1619.0f,1618.6f,1618.2f,1617.5f,1617.0f,1616.1f,1615.6f,1614.8f,1614.3f,1613.6f,1612.8f,1611.9f,1611.0f,1610.5f,1609.4f,1608.6f,1607.8f,1606.7f,1605.8f,1604.8f,1603.9f,1603.0f,1602.0f,1601.2f,1599.6f,1598.6f,1596.8f,1595.8f,1595.1f,1595.1f,1595.0f,1595.1f,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.7f,1619.0f,1618.6f,1618.4f,1617.7f,1617.3f,1616.4f,1615.9f,1615.1f,1614.5f,1613.8f,1613.1f,1612.3f,1611.5f,1610.9f,1609.8f,1609.1f,1608.2f,1607.3f,1606.2f,1605.2f,1604.3f,1603.4f,1602.5f,1601.7f,1600.1f,1599.2f,1597.5f,1596.4f,1595.2f,1594.4f,1594.2f,1594.3f,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.6f,1618.8f,1618.6f,1617.9f,1617.6f,1616.9f,1616.5f,1615.6f,1615.1f,1614.3f,1613.6f,1612.8f,1612.0f,1611.5f,1610.4f,1609.8f,1608.9f,1607.9f,1606.9f,1605.8f,1605.0f,1604.1f,1603.2f,1602.5f,1601.1f,1600.1f,1598.5f,1597.6f,1596.0f,1595.1f,1593.4f,1593.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.6f,1618.1f,1617.8f,1617.2f,1616.8f,1616.0f,1615.4f,1614.5f,1613.7f,1613.0f,1612.3f,1611.8f,1610.8f,1610.2f,1609.3f,1608.3f,1607.3f,1606.4f,1605.4f,1604.5f,1603.6f,1603.0f,1601.6f,1600.6f,1599.0f,1598.1f,1596.6f,1595.6f,1593.9f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.0f,1617.5f,1617.2f,1616.5f,1615.9f,1615.1f,1614.1f,1613.3f,1612.7f,1612.2f,1611.4f,1610.8f,1609.7f,1608.9f,1607.9f,1607.0f,1606.1f,1605.3f,1604.2f,1603.6f,1602.1f,1601.3f,1599.9f,1598.9f,1597.5f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.3f,1618.1f,1617.6f,1617.2f,1616.8f,1616.2f,1615.4f,1614.5f,1613.6f,1612.8f,1612.3f,1611.5f,1611.0f,1610.0f,1609.1f,1608.2f,1607.3f,1606.5f,1605.4f,1604.5f,1603.8f,1602.5f,1601.7f,1600.3f,1599.5f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.3f,1618.3f,1617.9f,1617.6f,1617.0f,1616.6f,1615.8f,1614.9f,1614.1f,1613.2f,1612.7f,1611.8f,1611.3f,1610.4f,1609.5f,1608.7f,1607.8f,1606.9f,1606.0f,1604.9f,1604.3f,1603.1f,1602.4f,1601.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.3f,1618.3f,1618.1f,1617.8f,1617.3f,1616.9f,1616.0f,1615.2f,1614.5f,1613.6f,1613.0f,1612.0f,1611.4f,1610.6f,1609.8f,1608.9f,1608.1f,1607.1f,1606.1f,1605.3f,1604.6f,1603.4f,1602.7f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.0f,1617.5f,1617.1f,1616.6f,1615.7f,1614.9f,1614.2f,1613.5f,1612.5f,1611.8f,1611.0f,1610.1f,1609.4f,1608.5f,1607.7f,1606.6f,1605.7f,1605.0f,1603.8f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.5f,1618.3f,1617.6f,1617.3f,1616.8f,1616.0f,1615.2f,1614.5f,1613.8f,1612.8f,1612.1f,1611.2f,1610.3f,1609.5f,1608.7f,1607.9f,1607.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.6f,1618.5f,1617.7f,1617.4f,1616.9f,1616.2f,1615.4f,1614.7f,1614.0f,1613.0f,1612.4f,1611.5f,1610.6f,1609.7f,1608.8f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
    {nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,1618.8f,1618.6f,1618.1f,1617.7f,1617.1f,1616.4f,1615.7f,1614.9f,1614.5f,1613.4f,1612.9f,1612.0f,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf,nanf},
};
    }
}