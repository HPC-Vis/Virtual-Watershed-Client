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
	}

    public void SetMax(float max)
    {
        Max = max;
    }

	// The buffer to hold the min and max....
	ComputeBuffer buffer;

    // For the csv file build
    float Max = 0;
    ComputeBuffer csvDump;

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
            // Send data to the shader
            cs.SetTexture(kernelHandle, "PassedInData", DataArray);
            cs.SetFloat("normalizeValue", Max);
			cs.SetInt ("sampleRate", sampleRate);
			cs.SetFloats ("from", new float[]{first.x,first.y+0.1f});
			cs.SetFloats ("to", new float[]{second.x,second.y+0.1f});
			buffer.SetData (da);

			cs.Dispatch (kernelHandle, sampleRate, 1, 1);
            buffer.GetData(da);
			max = BitConverter.ToSingle (BitConverter.GetBytes (da [0]),0);
			min = BitConverter.ToSingle (BitConverter.GetBytes (da [1]), 0);
		}

	}

    public void FindMinMaxCPU()
    {
        max = 1;
        min = 0;
    }

    public void WriteSlicerToFile()
    {
        // Temp patch to the OS dependen Compute Shader
		string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        string pathDownload = pathUser + "\\slicer_path.txt";
#else
		string pathDownload = pathUser + "/slicer_path.txt";
#endif


            float[] csv_file = new float[sampleRate];
            csvDump.GetData(csv_file);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@pathDownload))
            {
                Vector2 from = TerrainUtils.TerrainToNormalizedPoint(new Vector3(first.x, 0, first.y), GlobalConfig.TerrainBoundingBox);
                Vector2 to = TerrainUtils.TerrainToNormalizedPoint(new Vector3(second.x, 0, second.y), GlobalConfig.TerrainBoundingBox);
                Vector3 utm_from = coordsystem.transformToWorld(new Vector3(from.x, 0, from.y));
                Vector3 utm_to = coordsystem.transformToWorld(new Vector3(to.x, 0, to.y));
                file.WriteLine("UTM From: (" + utm_from.x + ", " + utm_from.z + ")");
                file.WriteLine("UTM To: (" + utm_to.x +", " + utm_to.z +")");
                file.WriteLine("UTM Zone: " + coordsystem.localzone);
                foreach (var i in csv_file)
                {
                    file.Write(i + ", ");
                }
            }
    }

	public float min = 0.0f;
	public float max = 0.0f;
	int kernelHandle;
	// Update is called once per frame
	public MinMaxShader (ComputeShader CS) {
		cs = CS;

		if (buffer != null)
        {
			buffer.Release();
			buffer.Dispose();
			buffer = null;
		}
		DataArray = new Texture2D(100,100);
		buffer = new ComputeBuffer(2,sizeof(float));
        csvDump = new ComputeBuffer(sampleRate, sizeof(float));
		
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
        cs.SetBuffer(kernelHandle, "SampleLine", csvDump);

		da [0] = 0;
		da [1] = 1;
	}

	void OnDestroy()
	{
		buffer.Release();
		buffer.Dispose();
		buffer = null;
	}
}
