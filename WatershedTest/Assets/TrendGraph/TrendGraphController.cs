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
        public TimeseriesRecord(DateTime time, float value, float[,] data)
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
        public int Row = 0;
        public int Col = 0;
        RectTransform rectTransform;
        RectTransform lineAnchor;
        Canvas parentCanvas;
        public float easting;
        public float northing;
        public Material GraphMaterial;
        public Image GraphImage;
        private Texture2D TrendTexture = null;
        public DateTime Begin = DateTime.MaxValue;
        public DateTime End = DateTime.MinValue;

        // Used for the data slicer
        public GameObject marker1, marker2;
        Vector3 marker1Position = Vector3.zero, marker2Position = Vector3.zero;
        int marker1Row = 0, marker1Col = 0, marker2Row = 0, marker2Col = 0;
        public Rect BoundingBox;
        public int DataIndex = 0;
        List<float> DataSlice;
        Vector3 WorldPoint1, WorldPoint2;
        public GameObject button;

        /// <summary>
        /// Called to update the fields on the trend graph.
        /// </summary>
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
                     .text = Begin.ToString();

            transform.Find("MaxHour")
                     .GetComponent<Text>()
                     .text = End.ToString();

            transform.Find("Units")
                     .GetComponent<Text>()
                     .text = unitsLabel;
        }

        /// <summary>
        /// This sets the min value of the data. This is for the X axis.
        /// </summary>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        public void SetMin(int min)
        {
            yMin = min;
            OnValidate();
        }

        /// <summary>
        /// This sets max value of the data. This is for the X axis.
        /// </summary>
        /// <param name="max">Max.</param>
        public void SetMax(int max)
        {
            yMax = max;
            OnValidate();
        }

        /// <summary>
        /// Set the Bounding box to the trend graph for checking
        /// </summary>
        /// <param name="bb">The bounding box to store the position.</param>
        public void SetBoundingBox(Rect bb)
        {
            BoundingBox = bb;
        }

        /// <summary>
        /// Use this for initialization of the trend graph and the locations on the scene.
        /// </summary>
        void Start()
        {
            button.SetActive(false);
            DataSlice = new List<float>();
            timeseries = new List<TimeseriesRecord>();
            rectTransform = transform.Find("Graph") as RectTransform;
            lineAnchor = transform.Find("Graph")
                                  .Find("LineAnchor") as RectTransform;

            // Walk up the Hierarchy to find the parent canvas.
            var parent = transform.parent;
            while (parent != null)
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

        /// <summary>
        /// This will handle the slicer of the data in the bounding box.
        /// </summary>
        void Update()
        {

            if(Input.GetKeyDown(KeyCode.X))
            {
                Debug.LogError("Current Res: " + Screen.currentResolution);
                Debug.LogError("Scale factor: " + parentCanvas.scaleFactor);
                Debug.LogError("Origin: " + origin);
                Debug.LogError("Width, Height: " + w + ", " + h);
            }

            // Check if there is active markers
            if(marker1.activeSelf && marker2.activeSelf)
            {
                if(marker1.transform.position != marker1Position || marker2.transform.position != marker2Position)
                {
                    // Set the marker positions
                    marker1Position = new Vector3(marker1.transform.position.x, marker1.transform.position.y, marker1.transform.position.z);
                    marker2Position = new Vector3(marker2.transform.position.x, marker2.transform.position.y, marker2.transform.position.z);

                    // Place in the check locations
                    WorldPoint1 = coordsystem.transformToWorld(marker1Position);
                    Vector2 CheckPoint1 = new Vector2(WorldPoint1.x, WorldPoint1.z);
                    WorldPoint2 = coordsystem.transformToWorld(marker2Position);
                    Vector2 CheckPoint2 = new Vector2(WorldPoint2.x, WorldPoint2.z);

                    if (BoundingBox.Contains(CheckPoint1) && BoundingBox.Contains(CheckPoint2))
                    {
                        Vector2 NormalizedPoint1 = TerrainUtils.NormalizePointToTerrain(WorldPoint1, BoundingBox);
                        Vector2 NormalizedPoint2 = TerrainUtils.NormalizePointToTerrain(WorldPoint2, BoundingBox);
                        marker1Row = timeseries[DataIndex].Data.GetLength(0) - 1 - (int)Math.Min(Math.Round(timeseries[DataIndex].Data.GetLength(0) * NormalizedPoint1.x), (double)timeseries[DataIndex].Data.GetLength(0) - 1);
                        marker1Col = timeseries[DataIndex].Data.GetLength(1) - 1 - (int)Math.Min(Math.Round(timeseries[DataIndex].Data.GetLength(1) * NormalizedPoint1.y), (double)timeseries[DataIndex].Data.GetLength(1) - 1);
                        marker2Row = timeseries[DataIndex].Data.GetLength(0) - 1 - (int)Math.Min(Math.Round(timeseries[DataIndex].Data.GetLength(0) * NormalizedPoint2.x), (double)timeseries[DataIndex].Data.GetLength(0) - 1);
                        marker2Col = timeseries[DataIndex].Data.GetLength(1) - 1 - (int)Math.Min(Math.Round(timeseries[DataIndex].Data.GetLength(1) * NormalizedPoint2.y), (double)timeseries[DataIndex].Data.GetLength(1) - 1);

                        DataSlice.Clear();
                        BuildSlice();
                        button.SetActive(true);
                    }
                    else
                    {
                        button.SetActive(false);
                    }
                }
            }
            else if(button.activeSelf)
            {
                button.SetActive(false);
            }
        }

        /// <summary>
        /// This will set the dataIndex to the currently viewed slide
        /// </summary>
        /// <param name="index">The index of the slide currently shown.</param>
        public void SetDataIndex(int index)
        {
            if(index != DataIndex)
            {
                marker1Position = Vector3.zero;
                marker2Position = Vector3.zero;
            }            
            DataIndex = index;
        }

        /// <summary>
        /// This is used for testing purposes, it will get the same slide and location every time.
        /// </summary>
        public void PresetData()
        {
            DataIndex = 1545;
            marker1Row = 61;
            marker1Col = 33;
            marker2Row = 46;
            marker2Col = 43;
            currentframeToFile();
            BuildSlice();
        }

        /// <summary>
        /// This wll ensure that the sizing of the trend graph is correct, and if not there will be a recomputation
        /// </summary>
        void OnGUI()
        {
            if (timeseries.Count < 1)
            {
                return;
            }
           
            // cull old records
            if (!Keep)
            {
                var elapsed = (float)(lastDraw - timeseries[0].time).TotalSeconds;
                while (elapsed > timebase && elapsed > 0)
                {
                    timeseries.RemoveAt(0);
                    if (timeseries.Count == 0)
                        return;
                    elapsed = (float)(lastDraw - timeseries[0].time).TotalSeconds;
                }

                // cull future records
                // e.g. SimTimeControl, user scrubbing backwards
                int m = timeseries.Count - 1;
                if (m == -1)
                    return;
                while (timeseries[m].time > (DateTime)lastDraw)
                {
                    timeseries.RemoveAt(m);
                    m = timeseries.Count - 1;
                    if (m == -1)
                        return;
                }
            }

            // Draw a line that represents the current slide on the graph
            timeseries.Sort((s1, s2) => s1.time.CompareTo(s2.time));
            if(DataIndex > timeseries.Count)
            {
                DataIndex = 0;
            }
            float normTime = (float)(timeseries[DataIndex].time - Begin).TotalSeconds / (float)(End - Begin).TotalSeconds;
            Drawing.DrawLine(new Vector2(origin.x + w * normTime * parentCanvas.scaleFactor, origin.y), new Vector2(origin.x + w * normTime * parentCanvas.scaleFactor, origin.y + h * 1 * parentCanvas.scaleFactor), Color.yellow, lineWidth, true);

            
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

            float new_w = rectTransform.rect.width * transform.localScale.x;
            float new_h = rectTransform.rect.height * transform.localScale.y;

            if (w != new_w || h != new_h)
            {
                w = new_w;
                h = new_h;
                Compute();
            }

        }

        /// <summary>
        /// Will clear the currently loaded timeseries.
        /// </summary>
        public void Clear()
        {
            timeseries.Clear();
            Begin = DateTime.MaxValue;
            End = DateTime.MinValue;
            yMax = 100;
            yMin = 0;
            unitsLabel = "";
            DataIndex = 0;
            // OnValidate();
        }

        /// <summary>
        /// Compute will build a new texture with the data at the row and col locations.
        /// This function is build to sprite image that is the graph to be displayed. 
        /// </summary>
        public void Compute()
        {
            // Set the width and height to integers
            int width = (int)w;
            int height = (int)h;

            // Sort the time series
            timeseries.Sort((s1, s2) => s1.time.CompareTo(s2.time));

            // Build the new texture
            if (TrendTexture != null)
            {
                Texture2D.Destroy(TrendTexture);
            }
            TrendTexture = new Texture2D(width, height);

            // Loop through all the time series
            Vector2 prev = Record2PixelCoords(timeseries[0]);            
            for (int i = 0; i < timeseries.Count; i++)
            {
                Vector2 next = Record2PixelCoords(timeseries[i]);
                Line(TrendTexture, (int)prev.x, (int)prev.y, (int)next.x, (int)next.y, Color.blue);
                prev = next;
            }

            // Apply to the world
            TrendTexture.wrapMode = TextureWrapMode.Clamp;
            TrendTexture.Apply();
            GraphImage.sprite = Sprite.Create(TrendTexture, new Rect(0, 0, width, height), new Vector2(0, 0));
        }

        /// <summary>
        /// takes the two given points and builds a interpolated slice off of it.
        /// </summary>
        public void BuildSlice()
        {
            int sample_rate = 50;
            Vector2 vec = new Vector2(marker2Row - marker1Row, marker2Col - marker1Col);
            float mag = Mathf.Sqrt((vec.x * vec.x) + (vec.y * vec.y));
            int x1, y1, x2, y2;
            float x, y;
            
            Vector2 unit = new Vector2((1 / mag) * vec.x, (1 / mag) * vec.y);

            for(int i = 0; i < sample_rate; i++)
            {
                x = marker1Row + (unit.x * (float)((float)i / (float)sample_rate) * mag);
                y = marker1Col + (unit.y * (float)((float)i / (float)sample_rate) * mag);

                x1 = (int)Mathf.Floor(x);
                y1 = (int)Mathf.Floor(y);
                x2 = (int)Mathf.Ceil(x);
                y2 = (int)Mathf.Ceil(y);

                if(x1 == x2)
                {
                    x2 += 1;
                }
                if(y1 == y2)
                {
                    y2 += 1;
                }
                DataSlice.Add(bilinearInterpolation(x1, y1, x2, y2, x, y));
            }

            x = marker1Row + (unit.x * 1 * mag);
            y = marker1Col + (unit.y * 1 * mag);

            x1 = marker2Row - 1;
            y1 = marker2Col - 1;
            x2 = marker2Row;
            y2 = marker2Col;
            DataSlice.Add(bilinearInterpolation(x1, y1, x2, y2, x, y));
        }

        /// <summary>
        /// Computes a bilinear interoplation off the given initial location, end location, and the point to interpolate on
        /// </summary>
        /// <param name="x1">Initial x point</param>
        /// <param name="y1">Initial y point</param>
        /// <param name="x2">End x point</param>
        /// <param name="y2">End y point</param>
        /// <param name="x">Interpol Point x</param>
        /// <param name="y">Interpol point y</param>
        public float bilinearInterpolation(int x1, int y1, int x2, int y2, float x, float y)
        {
            // Debug log for testing the data
            // Debug.LogError("The value x1, y1, x2, y2, x, y, ts(x1,y1), ts(x2,y1), ts(x1,y2), ts(x2,y2): " + x1 + ", " + y1 + ", " + x2 + ", " + y2 + ", " + x + ", " + y + ", " + timeseries[DataIndex].Data[x1, y1] + ", " + timeseries[DataIndex].Data[x2, y1] + ", " + timeseries[DataIndex].Data[x1, y2] + ", " + timeseries[DataIndex].Data[x2, y2]);
            
            // Run the Interpolation, and return.
            float value = (1 / ((x2 - x1) * (y2 - y1))) * ((timeseries[DataIndex].Data[x1, y1] * (x2 - x) * (y2 - y)) + (timeseries[DataIndex].Data[x2, y1] * (x - x1) * (y2 - y)) + (timeseries[DataIndex].Data[x1, y2] * (x2 - x) * (y - y1)) + (timeseries[DataIndex].Data[x2, y2] * (x - x1) * (y - y1)));
            return value;
        }

        /// <summary>
        /// Adds points on the texture that represent a straight line from the initial point
        /// to the ending point.
        /// </summary>
        /// <param name="tex">The texture that will take the piexl locations.</param>
        /// <param name="x0">The starting X location.</param>
        /// <param name="y0">The starting Y location.</param>
        /// <param name="x1">The ending X location.</param>
        /// <param name="y1">The ending Y location.</param>
        /// <param name="col">The color to make the pixel on the texture.</param>
        void Line(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
        {
            int dy = y1 - y0;
            int dx = x1 - x0;
            int stepy, stepx;
            float fraction;

            if (dy < 0)
            {
                dy = -dy;
                stepy = -1;
            }
            else
            {
                stepy = 1;
            }

            if (dx < 0)
            {
                dx = -dx;
                stepx = -1;
            }
            else
            {
                stepx = 1;
            }

            dy <<= 1;
            dx <<= 1;

            tex.SetPixel(x0, y0, col);
            if (dx > dy)
            {
                fraction = dy - (dx >> 1);
                while (x0 != x1)
                {
                    if (fraction >= 0)
                    {
                        y0 += stepy;
                        fraction -= dx;
                    }
                    x0 += stepx;
                    fraction += dy;
                    tex.SetPixel(x0, y0, col);
                }
            }
            else
            {
                fraction = dx - (dy >> 1);
                while (y0 != y1)
                {
                    if (fraction >= 0)
                    {
                        x0 += stepx;
                        fraction -= dy;
                    }
                    y0 += stepy;
                    fraction += dx;
                    tex.SetPixel(x0, y0, col);
                }
            }
        }

        /// <summary>
        /// Takes the given record value, gets the selected location in the array, and builds a vector
        /// of the location the pixel should be based off the value. 
        /// </summary>
        /// <returns>The location of the pixel on the screen.</returns>
        /// <param name="record">The current record that is to be made a pixel.</param>
        Vector2 Record2PixelCoords(TimeseriesRecord record)
        {
            //float s = (float)(lastDraw - record.time).TotalSeconds;
            //float s = (float)(lastDraw - record.time).TotalSeconds;
            //float normTime = Mathf.Clamp01(1 - s / timebase);
            float normTime = (float)(record.time - Begin).TotalSeconds / (float)(End - Begin).TotalSeconds;
            float normHeight = 0;
            if (record.Data != null)
            {
                normHeight = Mathf.Clamp01((record.Data[Row, Col] - yMin) / (yMax - yMin));
            }
            //float normHeight = Mathf.Clamp01((record.value - yMin) / (yMax - yMin));
            return new Vector2(w * normTime,
                               h * (1 - normHeight) - 1);
        }

        /// <summary>
        /// Takes the record passed in and will set it to the timeseries record.
        /// This will also take the time range and update if necessary.
        /// </summary>
        /// <param name="record">The TimeseriesRecord to add to the list of other records.</param>
        public void Add(TimeseriesRecord record)
        {
            timeseries.Add(record);
            if (Begin > record.time)
            {
                Begin = record.time;
            }
            if (End < record.time)
            {
                End = record.time;
            }
            lastDraw = record.time;
            valueText.text = record.value.ToString(valueFormatString);
            OnValidate();
        }

        /// <summary>
        /// This will set the title of the graph with the string value.
        /// </summary>
        /// <param name="unit">The string value to set the unit type to.</param>
        public void SetUnit(string unit)
        {
            variable_name = unit;
            unitsLabel = VariableReference.GetDescription(unit);
            OnValidate();
        }

        /// <summary>
        /// This will add the row and col location of the trendgraph, enabling the graph to be built.
        /// This will also begin to build the data slicer points and compute when done.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        public void SetPosition(int row, int col)
        {
            // Set row and col
            Row = row;
            Col = col;
            Compute();
            Debug.LogError("Trend Graph row: " + Row + " col: " + Col);
        }

        /// <summary>
        /// Takes the parameters and will build a timeseries record to add to the timeseries.
        /// </summary>
        /// <param name="time">The datetime of the data.</param>
        /// <param name="value">This is a float that is currently not used in the record.</param>
        /// <param name="data">A 2D array of the raw data.</param>
        public void Add(DateTime time, float value, float[,] data)
        {
            Add(new TimeseriesRecord(time, value, data));
        }

        /// <summary>
        /// This will set the easting and northing of the selected location.
        /// </summary>
        /// <param name="point">This is the point with a set value of x and z that represent easting and northing respectibly.</param>
        public void SetCoordPoint(Vector3 point)
        {
            easting = point.x;
            northing = point.z;
        }

        /// <summary>
        /// This will send the data on the slicer to a file
        /// </summary>
        public void SlicerToFile()
        {
            // Temp patch to the OS dependen Compute Shader
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            string pathDownload = pathUser + "/slicer_data.csv";
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string pathDownload = pathUser + "\\slicer_data.csv";
#endif
            Debug.LogError("The File Path: " + pathDownload);
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@pathDownload))
            {
                file.WriteLine("This file represenets interpolated data that was calculated from a line across the currently shown dataset.");
                file.WriteLine(variable_name + ": " + unitsLabel);
                file.WriteLine("Time of Frame: " + timeseries[DataIndex].time);
                file.WriteLine("UTM: (" + WorldPoint1.x + ", " + WorldPoint1.z + ") to (" + WorldPoint2.x + ", " + WorldPoint2.z + ").");
                file.WriteLine("UTM Zone: " + coordsystem.localzone);
                foreach (var i in DataSlice)
                {
                    file.Write(i + ", ");
                }
            }
        }

        /// <summary>
        /// This will take all the data at the selected point on the data set, throughout all datasets of time,
        /// and send to a file on the Desktop named graph.txt.
        /// </summary>
        public void dataToFile()
        {
            // Temp patch to the OS dependen Compute Shader
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            string pathDownload = pathUser + "/graph.csv";
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string pathDownload = pathUser + "\\graph.csv";
#endif
            Debug.LogError("The File Path: " + pathDownload);
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@pathDownload))
            {
                file.WriteLine(variable_name + ": " + unitsLabel);
                file.WriteLine("Time Frame: " + Begin.ToString() + " to " + End.ToString());
                file.WriteLine("UTM: (" + easting + ", " + northing + ")");
                file.WriteLine("UTM Zone: " + coordsystem.localzone);
                foreach (var i in timeseries)
                {
                    file.Write(i.Data[Row, Col] + ", ");
                }
            }
        }

        /// <summary>
        /// This will send all the data from the current frame to a file
        /// </summary>
        public void currentframeToFile()
        {
            // Temp patch to the OS dependen Compute Shader
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            string pathDownload = pathUser + "/frameToFile.csv";
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string pathDownload = pathUser + "\\frameToFile.csv";
#endif
            Debug.LogError("The File Path: " + pathDownload);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@pathDownload))
            {
                for (int i = 0; i < timeseries[DataIndex].Data.GetLength(1); i++ )
                {
                    for (int j = 0; j < timeseries[DataIndex].Data.GetLength(0); j++ )
                    {
                        file.Write(timeseries[DataIndex].Data[i, j] + ", ");
                    }
                    file.Write("\n");
                }
            }
        }
    }
}