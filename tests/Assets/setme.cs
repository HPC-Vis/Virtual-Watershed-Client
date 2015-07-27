using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class setme : MonoBehaviour {
	public Image image;
	public RawImage image2;
	Texture2D test;
	// Use this for initialization
	void Start () {
		test = new Texture2D (100, 1000);
		Color[] tests = new Color[100 * 1000];
		for (int i =0; i < 100*1000; i++) 
		{
			if(i > (100*1000)/2)
			{
				tests[i] = new Color(0,0,0,0);
			}
			else
			{
				tests[i] = new Color(0,0,0,.9f);
			}
		}
		test.SetPixels (tests);
		test.Apply ();
		if (image != null) 
		{
			image.sprite = Sprite.Create (test, new Rect (0, 0, 100, 1000), Vector2.zero);
		}
		if (image2 != null) 
		{
			image2.texture = test;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
