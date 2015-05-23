using UnityEngine;
using System.Collections;

public class SlideShaper : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Color[] TextureReshape(Color[] OldColors, int OldWidth, int OldHeight, int NewWidth)
	{
		Texture2D texture = new Texture2D (NewWidth, NewWidth, TextureFormat.ARGB32, false);
		Color[] NewColors = new Color[NewWidth*NewWidth];
		if (OldWidth == NewWidth) 
		{
			int offset = NewWidth - OldHeight;
			for(int i =0; i < NewWidth; i++)
			{
				for(int j = 0; j < NewWidth; j++)
				{
					if(j < offset)
					{
						// Zero out
						NewColors[i*NewWidth+j] = Color.clear;
					}
					else
					{
						NewColors[i*NewWidth+j] = OldColors[i*NewWidth + j-offset];
					}
				}
			}
		}

		else if (OldHeight == NewWidth)
		{

		}
		return OldColors;
	}
}
