using UnityEngine;
using System.Collections;
using System;
public class terrainDetailTest : MonoBehaviour {
	public Texture2D tex;
	Terrain t;
	TerrainData td;
	
	public static string ByteArrayToString(byte[] ba)
	{
		string hex = BitConverter.ToString(ba);
		return hex.Replace("-","");
	}
	
	// Use this for initialization
	void Start () {
		float a = 1.1f;
		int b = 1;
		//Debug.LogError();
		Color d = Color.red;
		
		var c = BitConverter.GetBytes(a);
		Color32 da = new Color32(c[3],c[2],c[1],c[0]);
		Texture2D testTex = new Texture2D(1,1);
		testTex.SetPixels32(new Color32[]{da});
		Debug.LogError(testTex.GetPixel(0,0));
		Debug.LogError(ByteArrayToString(c));
		Debug.LogError(c[0]);
		Debug.LogError(((int)a >> 24) & 0xFF);
		Debug.LogError(((int)a >> 16) & 0xFF);
		Debug.LogError(((int)a >> 8) & 0xFF);
		Debug.LogError((int)a & 0xFF);
		//Color a = //(Color)1.1f;
		t = gameObject.GetComponent<Terrain>();
		td = t.terrainData;
		var DP = new DetailPrototype[1];
		var D = new DetailPrototype();
		D.prototypeTexture = tex;
		DP[0] = D;
		td.detailPrototypes = DP;
		
		// The tex layer 
		var texlayer = td.GetDetailLayer(0,0,td.detailWidth,td.detailHeight,0);
		
		for(int i = 0; i < td.detailWidth; i++)
		{
			for(int j = 0; j < td.detailHeight; j++)
			{
				texlayer[i,j] = 1;
			}
		}

		td.SetDetailLayer(0,0,0,texlayer);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
