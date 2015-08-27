using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class tempColorMapData : MonoBehaviour {

    System.Random r = new System.Random();
    float[,] data = new float[256, 256];
    public Image temp;
    public Texture2D text;
    
	// Use this for initialization
	void Start () {
        for (int i = 0; i < 256; i++)
        {
            for (int j = 0; j < 256; j++)
            {
                data[i, j] = r.Next(10,100);//(float)((float)(j) / (float)(256));
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("t"))
        {
            float dummy1, dummy2;
            text = Utilities.BuildDataTexture(data, out dummy1, out dummy2);
            temp.sprite = Sprite.Create(text, new Rect(0, 0, 256, 256), Vector2.zero);
            Debug.Log("Applying sprite");
        }
	}


}
