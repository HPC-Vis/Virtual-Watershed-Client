using UnityEngine;
using System.Collections;

public class TerrainTestVegetation : MonoBehaviour {

    public Terrain terrain;
    bool first = false;
    TerrainData td;
	// Use this for initialization
	void Start () {
        td = terrain.terrainData;
	}
	
	// Update is called once per frame
	void Update () {
        if (!first)
        {
            var test = new DetailPrototype();
            if (td.detailPrototypes.Length > 0)
            {
                terrain.terrainData.RefreshPrototypes();
                Debug.LogError("Vegatating!!");
                Debug.LogError(td.detailHeight);
                Debug.LogError(td.detailWidth);
                Debug.LogError("TREE INSTANCE COUNT: " + td.treeInstanceCount);
                int[,] detailmap = new int[td.detailWidth, td.detailHeight];

                for (int i = 0; i < td.detailPrototypes.Length; i++)
                {
                    for (int k = 0; k < td.detailWidth; k++)
                    {
                        for (int j = 0; j < td.detailHeight; j++)
                        {
                            detailmap[k, j] = k%10 + i;
                        }
                    }
                    Debug.LogError(i);
                    terrain.terrainData.SetDetailLayer(0, 0, i, detailmap);
                }
            }
            
            terrain.terrainData = td;
            
            first = true;

            TreeInstance[] testtreepop = new TreeInstance[10];
            for (int i = 0; i < td.treeInstanceCount; i++)
            {
                Debug.LogError(td.GetTreeInstance(i).prototypeIndex);
                Debug.LogError(td.GetTreeInstance(i).position);
                Debug.LogError("ROTATION: " + td.GetTreeInstance(i).rotation);
                Debug.LogError("HEIGHT: " + td.GetTreeInstance(i).heightScale);
                Debug.LogError("WIDTH: " + td.GetTreeInstance(i).widthScale);
                Debug.LogError("COLOR: " + td.GetTreeInstance(i).color);
                Debug.LogError("COLOR: " + td.GetTreeInstance(i).lightmapColor);
            }
            for (int i = 0; i < 10; i++)
            {
                var ti = new TreeInstance();
                
                ti.position = new Vector3(i/10.0f, 0, i/10.0f);
                ti.widthScale = 1;
                ti.heightScale = 1;
                ti.lightmapColor = new Color32(255, 255, 255, 255);
                ti.color = new Color32(168, 168, 168, 255);
                testtreepop[i] = ti;
            }
            terrain.terrainData.treeInstances = testtreepop;
            terrain.terrainData.RefreshPrototypes();
            Debug.LogError("TREE INSTANCE COUNT: " + td.treeInstanceCount);
            Debug.LogError("TREE INSTANCE COUNT: " + terrain.terrainData.treeInstanceCount);
            //terrain.terrainData.SetTreeInstance(0, new TreeInstance());

        }
	}
}
