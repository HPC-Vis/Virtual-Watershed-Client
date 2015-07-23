/*
 * Copyright (c) 2014, Roger Lew (rogerlew.gmail.com)
 * Date: 5/20/2015
 * License: BSD (3-clause license)
 * 
 * The project described was supported by NSF award number IIA-1301792
 * from the NSF Idaho EPSCoR Program and by the National Science Foundation.
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VTL.TrendGraph
{
    public struct TimeseriesRecord
    {
        public DateTime time;
        public float value;
        public float[,] Data;
        public TimeseriesRecord(DateTime time, float value,float[,] data)
        {
            this.time = time;
            this.value = value;
            this.Data = data;
        }
    }

    public class TrendGraphController : MonoBehaviour
    {
        public Color lineColor = Color.white;
        public float lineWidth = 1f;
        public float yMax = 1;
        public float yMin = 0;
        public float timebase = 300; // in seconds
        public string minTime = ""; // Its on the developer to make sure 
                                                // this makes sense with the timebase
        public string maxTime = ""; // Its on the developer to make sure 
        // this makes sense with the timebase
        public string unitsLabel = "F"; // the units label
        public string variable_name = "";
        public string valueFormatString = "D3";
        private DateTime lastDraw;
		public bool Keep = true;
        List<TimeseriesRecord> timeseries;
        Text valueText;
        Vector3 origin;
        float w;
        float h;
        public int row = 0;
        public int col = 0;
        RectTransform rectTransform;
        RectTransform lineAnchor;
        Canvas parentCanvas;
        public float easting;
        public float northing;
        public Material GraphMaterial;
        public Image GraphImage;
        private Texture2D TrendTexture = null;

        public void OnValidate()
        {
            transform.Find("Ymax")
                     .GetComponent<Text>()
                     .text = yMax.ToString();

            transform.Find("Ymin")
                     .GetComponent<Text>()
                     .text = yMin.ToString();

            transform.Find("MinHour")
                     .GetComponent<Text>()
                     .text = minTime;

            transform.Find("MaxHour")
                     .GetComponent<Text>()
                     .text = maxTime;

            transform.Find("Units")
                     .GetComponent<Text>()
                     .text = unitsLabel;
        }

        public void SetTime(string min, string max)
        {
            transform.Find("MinHour")
                     .GetComponent<Text>()
                     .text = min;
            minTime = min;

            transform.Find("MaxHour")
                     .GetComponent<Text>()
                     .text = max;
            maxTime = max;
        }

        public void SetMinMax(int min, int max)
        {
            transform.Find("Ymax")
                     .GetComponent<Text>()
                     .text = max.ToString();
            yMax = max;

            transform.Find("Ymin")
                     .GetComponent<Text>()
                     .text = min.ToString();
            yMin = min;
        }

        // Use this for initialization
        void Start()
        {
            timeseries = new List<TimeseriesRecord>();
            rectTransform = transform.Find("Graph") as RectTransform;
            lineAnchor = transform.Find("Graph")
                                  .Find("LineAnchor") as RectTransform;

            // Walk up the Hierarchy to find the parent canvas.
            var parent = transform.parent;
            while(parent != null)
            {
                parentCanvas = parent.GetComponent<Canvas>();
                if (parentCanvas != null)
                    parent = null; // stop condition
                else
                    parent = parent.parent;
            }

            valueText = transform.Find("Value").GetComponent<Text>();

            // Set image material
            GraphImage.material = GraphMaterial;
        }

        // The Drawing.DrawLine method using the GL and GUI class and 
        // has to be drawn in draw line
        void OnGUI()
        {
            if(timeseries.Count < 1)
            {
                Debug.LogError("There was no data in the time series");
                return;
            }
            // sort the records incase they are out of order
            timeseries.Sort((s1, s2) => s1.time.CompareTo(s2.time));
			//Debug.LogError (timeseries.Count);
            // cull old records

			if (!Keep) {
				var elapsed = (float)(lastDraw - timeseries [0].time).TotalSeconds;
				while (elapsed > timebase && elapsed > 0) { 
					timeseries.RemoveAt (0);
					if (timeseries.Count == 0)
						return;
					elapsed = (float)(lastDraw - timeseries [0].time).TotalSeconds;
				}

				// cull future records
				// e.g. SimTimeControl, user scrubbing backwards
				int m = timeseries.Count - 1;
				if (m == -1)
					return;
				while (timeseries[m].time > (DateTime)lastDraw) {
					timeseries.RemoveAt (m);
					m = timeseries.Count - 1;
					if (m == -1)
						return;
				}
			}

            // return if there are less than 2 records after culling
            int n = timeseries.Count;
            if (n < 2)
                return;

            // Need to check the origin and the width and height every draw
            // just in case the panel has been resized
            switch (parentCanvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    origin = rectTransform.position;
                    origin.y = Screen.height - origin.y;
                    break;
                default:
                    origin = RectTransformUtility.WorldToScreenPoint(parentCanvas.worldCamera, lineAnchor.position);
                    origin.y = Screen.height - origin.y;
                    break;
            }
            w = rectTransform.rect.width * transform.localScale.x;
            h = rectTransform.rect.height * transform.localScale.y;

            /* OLD
            // Iterate through the timeseries and draw the trend segment
            // by segment.
            var prev = Record2PixelCoords(timeseries[0]);
            for (int i = 1; i < n; i+=4)
            {
                var next = Record2PixelCoords2(timeseries[i]);
                Drawing.DrawLine(prev, next, lineColor, lineWidth, false);
                prev = next;
            }
            */
        }

        public void Clear()
        {
            timeseries.Clear();
        }

        public void Compute()
        {
            // Set the width and height to integers
            int width = (int)w;
            int height = (int)h;

            // Build the new texture
            if(TrendTexture != null)
            {
                Texture2D.Destroy(TrendTexture);
            }
            TrendTexture = new Texture2D(width, height);            
            
            // Loop through all the time series
            Vector2 prev = Record2PixelCoords2(timeseries[0]);
            prev.y = prev.y - (int)origin.y - 1;
            int counter = 0;
            for (int i = 1; i < timeseries.Count; i++)
            {
                Vector2 next = Record2PixelCoords2(timeseries[i]);
                next.y = next.y - (int)origin.y -1 ;
                TrendTexture.SetPixel((int)next.x, (int)next.y, Color.blue);
                /*
                if (next.y != origin.y + h || prev.y != origin.y + h)
                {
                    counter++;
                    next.y = next.y - (int)origin.y - 1;
                    if(counter < 500)
                    {
                        DrawLine(TrendTexture, (int)prev.x, (int)prev.y, (int)next.x, (int)next.y, Color.white);
                    }
                    else 
                    {
                        TrendTexture.SetPixel((int)next.x, (int)next.y, Color.blue);
                    }
                    
                }
                else
                {
                    TrendTexture.SetPixel((int)next.x, (int)next.y, Color.blue);
                }
                */
                prev = next;
            }
            
            // Apply to the world
            TrendTexture.wrapMode = TextureWrapMode.Clamp;

            TrendTexture.Apply();
            GraphImage.sprite = Sprite.Create(TrendTexture, new Rect(0, 0, width, height), new Vector2(0, 0));
        }

        void Drawline(Vector2 previous, Vector2 next)
        {
            float x, y;
            float dy = next.y - previous.y;
            float dx = next.x - previous.x;
            float m = dy / dx;
            float dy_inc = -1;
            float dx_inc = 1;

            // Check if the dy is a negative
            if(dy < 0)
            {
                dy = 1;
            }

            // check the dx value
            if(dx < 0)
            {
                dx = -1;
            }

            // rende the line for the streatch of the points
            if(Mathf.Abs(dy) > Mathf.Abs(dx))
            {
                for (y = next.y; y < previous.y; y += dy_inc)
                {
                    x = previous.x + (y - previous.y) * m;
                    TrendTexture.SetPixel((int)x, (int)y, Color.white);
                }
            }
            else
            {
                for (x = previous.x; x < next.x; x += dx_inc)
                {
                    y = previous.y + (x - previous.x) * m;
                    TrendTexture.SetPixel((int)x, (int)y, Color.white);
                }
            }
        }

        void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
{
 	int dy = (int)(y1-y0);
	int dx = (int)(x1-x0);
 	int stepx, stepy;
 
	if (dy < 0) {dy = -dy; stepy = -1;}
	else {stepy = 1;}
	if (dx < 0) {dx = -dx; stepx = -1;}
	else {stepx = 1;}
	dy <<= 1;
	dx <<= 1;
 
	float fraction = 0;
 
	tex.SetPixel(x0, y0, col);
	if (dx > dy) {
		fraction = dy - (dx >> 1);
		while (Mathf.Abs(x0 - x1) > 1) {
			if (fraction >= 0) {
				y0 += stepy;
				fraction -= dx;
			}
			x0 += stepx;
			fraction += dy;
			tex.SetPixel(x0, y0, col);
		}
	}
	else {
		fraction = dx - (dy >> 1);
		while (Mathf.Abs(y0 - y1) > 1) {
			if (fraction >= 0) {
				x0 += stepx;
				fraction -= dy;
			}
			y0 += stepy;
			fraction += dx;
			tex.SetPixel(x0, y0, col);
		}
	}
}


        // converts a TimeseriesRecord to screen pixel coordinates for plotting
        Vector2 Record2PixelCoords(TimeseriesRecord record)
        {
            float s = (float)(lastDraw - record.time).TotalSeconds;
            float normTime = Mathf.Clamp01(1 - s / timebase);
            float normHeight = Mathf.Clamp01((record.Data[row,col] - yMin) / (yMax - yMin));
            //float normHeight = Mathf.Clamp01((record.value - yMin) / (yMax - yMin));
            return new Vector2(origin.x + w * normTime,
                               origin.y + h * (1 - normHeight));
        }


		Vector2 Record2PixelCoords2(TimeseriesRecord record)
		{
			//float s = (float)(lastDraw - record.time).TotalSeconds;
			//float s = (float)(lastDraw - record.time).TotalSeconds;
			//float normTime = Mathf.Clamp01(1 - s / timebase);
			float normTime = (float)(record.time - Start2).TotalSeconds / (float)(End - Start2).TotalSeconds;
            float normHeight = 0;
            if (record.Data != null)
            {
                normHeight = Mathf.Clamp01((record.Data[row, col] - yMin) / (yMax - yMin));
            }
            //float normHeight = Mathf.Clamp01((record.value - yMin) / (yMax - yMin));
			return new Vector2(origin.x + w * normTime,
			                   origin.y + h * (1 - normHeight));
		}

		public DateTime Start2 = DateTime.MaxValue,End=DateTime.MinValue;

        // add a time series to the trend graph
        public void Add(TimeseriesRecord record)
        {
            timeseries.Add(record);
			if (Start2 > record.time) 
			{
				Start2 = record.time;
			}
		    if (End < record.time) {
				End = record.time;
			}
            lastDraw = record.time;
            valueText.text = record.value.ToString(valueFormatString);
            OnValidate();
        }

        public void SetUnit(string unit)
        {
            variable_name = unit;
            VariableReference newVariable = new VariableReference();
            unitsLabel = newVariable.GetDescription(unit);
            OnValidate();
        }
        
        public void Add(DateTime time, float value,float[,] data)
        {
            Add(new TimeseriesRecord(time, value,data));
        }

        public void SetCoordPoint(Vector3 point)
        {
            easting = point.x;
            northing = point.z;
        }

        public void dataToFile()
        {
            // Temp patch to the OS dependen Compute Shader
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string pathDownload = pathUser + "\\graph.txt";
#else
		string pathDownload = pathUser + "/slicer_path.txt";
#endif

            float[] csv_file = new float[timeseries.Count];

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@pathDownload))
            {
                file.WriteLine(variable_name + ": " + unitsLabel);
                file.WriteLine("Time Frame: " + minTime + " to " + maxTime);
                file.WriteLine("UTM: (" + easting + ", " + northing + ")");
                file.WriteLine("UTM Zone: " + coordsystem.localzone);
                foreach (var i in timeseries)
                {
                    file.Write(i.Data[row, col] + ", ");
                }
            }
        }
    }
}
