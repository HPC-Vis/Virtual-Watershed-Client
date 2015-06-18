using UnityEngine;
using System.Collections;
using System;
public class MinMaxShader
{
	public ComputeShader cs;
	Vector2 first = Vector2.zero;
	Vector2 second = Vector2.zero;
	public void SetFirstPoint(Vector2 point)
	{
		first = point;
	}
	public void SetSecondPoint(Vector2 point)
	{
		second = point;
	}


	public void SetDataArray(Texture2D Tex)
	{
		DataArray = Tex;
		cs.SetTexture (kernelHandle, "PassedInData", DataArray);
	}
	// The buffer to hold the min and max....
	ComputeBuffer buffer;

	// The Texture to find hte min and max
	Texture2D DataArray;
	uint[] da = new uint[2];
	int sampleRate = 100;

	public void FindMinMax()
	{
		//Debug.LogError (first);
		//Debug.LogError (second);
		da [0] = uint.MinValue;
		da [1] = uint.MaxValue;
		if (buffer != null) {
			cs.SetInts ("direction", new int[]{1,0});
			cs.SetInt ("fromx", 98);
			cs.SetInt ("tox", 99);
			cs.SetInt ("fromy", 98);
			cs.SetInt ("toy", 99);
			
			cs.SetInt ("sampleRate", sampleRate);
			cs.SetFloats ("from", new float[]{first.x,first.y});
			cs.SetFloats ("to", new float[]{second.x,second.y});
			buffer.SetData (da);
			//cs.Dispatch (kernelHandle, sampleRate, 1, 1);
			
			buffer.GetData (da);
			//Debug.LogError ("KERNEL: " + kernelHandle);
			//int ii = BitConverter.ToInt32(BitConverter.GetBytes(ff), 0);
			//Debug.LogError ("Converted value x: " + BitConverter.ToSingle (BitConverter.GetBytes (da [0]), 0));
			//Debug.LogError ("Converted value y: " + BitConverter.ToSingle (BitConverter.GetBytes (da [1]), 0));
			//Debug.LogError ("x: " + da [0]);
			//Debug.LogError ("y: " + da [1]);
			max = BitConverter.ToSingle (BitConverter.GetBytes (da [0]),0);
			min = BitConverter.ToSingle (BitConverter.GetBytes (da [1]), 0);
		}

	}

	public float min = 0.0f;
	public float max = 0.0f;
	int kernelHandle;
	// Update is called once per frame
	public MinMaxShader (ComputeShader CS) {
		cs = CS;

		if (buffer != null) {
			buffer.Release();
			buffer.Dispose();
			buffer = null;
		}
		DataArray = new Texture2D(100,100);
		buffer = new ComputeBuffer(2,sizeof(float));
		
		kernelHandle = cs.FindKernel("NormalizedSampler");
		

		Color[] colors = new Color[100 * 100];
		for (int i = 0; i < 100; i++) 
		{
			for(int j = 0; j < 100; j++)
			{
				//Debug.LogError((float)i/(float)100);
				colors[i*100 + j] = new Color((float)(i*100 + j)/(float)(100*100),0,0);
			}
		}

		DataArray.SetPixels (colors);
		DataArray.Apply ();

		cs.SetTexture (kernelHandle, "PassedInData", DataArray);
		cs.SetBuffer (kernelHandle, "MinMax", buffer);

		da [0] = 0;
		da [1] = 1;
	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (0, 0, 100, 100), DataArray);
	}

	void OnDestroy()
	{
		buffer.Release();
		buffer.Dispose();
		buffer = null;
	}
}
