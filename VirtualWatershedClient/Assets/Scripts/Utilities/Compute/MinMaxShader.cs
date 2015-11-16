using UnityEngine;
using System.Collections;
using System;

public class MinMaxShader
{
	// The buffer to hold the min and max
	ComputeBuffer buffer;
	
	// For the csv file build
	float Max = 0;
	ComputeBuffer csvDump;
	
	// The Texture to find the min and max
	Texture2D DataArray;

	uint[] da = new uint[2];
	int sampleRate = 100;
	public ComputeShader cs;
	Vector2 first = Vector2.zero;
	Vector2 second = Vector2.zero;
	public float min = 0.0f;
	public float max = 0.0f;
	int kernelHandle;

	/// <summary>
	/// Initializes a new instance of the <see cref="MinMaxShader"/> class.
	/// Sets the necessary data on the Compute Shader.
	/// </summary>
	/// <param name="CS">The compute shader to use with this program.</param>
	public MinMaxShader (ComputeShader CS) 
	{
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

	/// <summary>
	/// Sets the first point to sample from.
	/// </summary>
	/// <param name="point">A vector2 point.</param>
	public void SetFirstPoint(Vector2 point)
	{
		first = point;
	}

	/// <summary>
	/// Sets the last point to take a sample from.
	/// </summary>
	/// <param name="point">A vector2 point.</param>
	public void SetSecondPoint(Vector2 point)
	{
		second = point;
	}

	/// <summary>
	/// Assigns a prebuild texture2D to the one to search.
	/// </summary>
	/// <param name="Tex">The texture2D to search.</param>
	public void SetDataArray(Texture2D Tex)
	{
		DataArray = Tex;
	}

	/// <summary>
	/// Sets the max.
	/// </summary>
	/// <param name="max">The max float value.</param>
    public void SetMax(float max)
    {
        Max = max;
    }

	/// <summary>
	/// Will set up the data of the min max computation, call the compute shader, 
	/// and gets the data from the compute shader to be set to the world.
	/// </summary>
	public void FindMinMax()
	{
		// Inits with a min and max value
		da [0] = uint.MinValue;
		da [1] = uint.MaxValue;

		// Checks that the buffer has been built
		if (buffer != null) 
		{
            // Send data to the shader
            cs.SetTexture(kernelHandle, "PassedInData", DataArray);
            cs.SetFloat("normalizeValue", Max);
			cs.SetInt ("sampleRate", sampleRate);
            if (first.y > second.y)
            {
                cs.SetFloats("from", new float[] { first.x, first.y+0.05f });
                cs.SetFloats("to", new float[] { second.x, second.y-0.05f });
            }
            else
            {
                cs.SetFloats("from", new float[] { first.x, first.y - 0.05f });
                cs.SetFloats("to", new float[] { second.x, second.y + 0.05f });
            }
			buffer.SetData (da);

			cs.Dispatch (kernelHandle, sampleRate, 1, 1);
            buffer.GetData(da);
			max = BitConverter.ToSingle (BitConverter.GetBytes (da [0]),0);
			min = BitConverter.ToSingle (BitConverter.GetBytes (da [1]), 0);
		}

	}

	/// <summary>
	/// This will not call a compute shader to do the min/max, it will just set 0 and 1.
	/// Used for operating systems without Compute Shader options.
	/// </summary>
    public void FindMinMaxCPU()
    {
        max = 1;
        min = 0;
    }

	/// <summary>
	/// Writes the data that is along the path of the slicer and send it to a file.
	/// This data will come from the Computer Shader as it samples across the line of the ray.
	/// </summary>
    public void WriteSlicerToFile()
    {
            String pathDownload = Utilities.GetFilePath("slicer_path.csv");
            float[] csv_file = new float[sampleRate];
            csvDump.GetData(csv_file);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathDownload))
            {

                Vector2 from = TerrainUtils.TerrainToNormalizedPoint(new Vector3(first.x, 0, first.y+0.1f), GlobalConfig.TerrainBoundingBox);
                Vector2 to = TerrainUtils.TerrainToNormalizedPoint(new Vector3(second.x, 0, second.y-0.1f), GlobalConfig.TerrainBoundingBox);
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
	
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy()
	{
		buffer.Release();
		buffer.Dispose();
		buffer = null;
	}
}
