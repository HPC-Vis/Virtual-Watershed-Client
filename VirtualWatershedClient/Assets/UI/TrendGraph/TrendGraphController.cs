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
using System.IO;

namespace VTL.TrendGraph
{

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

        public void UpdateData(Rect BoundingBox, string projection, string variable)
        {
            // Set the bounding box to the trendgraph
            transform tran = new transform();
            //Debug.LogError("Coord System: " + record.projection);
            tran.createCoordSystem(projection); // Create a coordinate transform
                                                //Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));

            // transfor a lat/long bounding box to UTM
            tran.setOrigin(coordsystem.WorldOrigin);
            Vector2 point = tran.transformPoint(new Vector2(BoundingBox.x, BoundingBox.y));
            Vector2 point2 = tran.transformPoint(new Vector2(BoundingBox.x + BoundingBox.width, BoundingBox.y - BoundingBox.height));

            if ((BoundingBox.x > -180 && BoundingBox.x < 180 && BoundingBox.y < 180 && BoundingBox.y > -180))
            {
                BoundingBox = new Rect(point.x, point.y, Math.Abs(point.x - point2.x), Math.Abs(point.y - point2.y));
            }

            Clear();
            SetBoundingBox(BoundingBox);
            SetUnit(variable);
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
            // This will get a user click
            if (Input.GetMouseButtonDown(0) && mouselistener.state == mouselistener.mouseState.TERRAIN)
            {
                // Check if mouse is inside bounding box 
                Vector3 WorldPoint = coordsystem.transformToWorld(mouseray.CursorPosition);
                Vector2 CheckPoint = new Vector2(WorldPoint.x, WorldPoint.z);

                if (BoundingBox.Contains(CheckPoint))
                {
                    // Debug.LogError("CONTAINS " + CheckPoint + " Width: " + BoundingBox.width + " Height: " +  BoundingBox.height);
                    Vector2 NormalizedPoint = Vector2.zero;
                    NormalizedPoint = TerrainUtils.NormalizePointToTerrain(WorldPoint, BoundingBox);
                    SetCoordPoint(WorldPoint);
                    int x = (int)Math.Min(Math.Round(ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0) * NormalizedPoint.x), (double)ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0) - 1);
                    int y = (int)Math.Min(Math.Round(ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1) * NormalizedPoint.y), (double)ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1) - 1);

                    SetPosition(ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1) - 1 - y, ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0) - 1 - x);
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.LogError("Current Res: " + Screen.currentResolution);
                Debug.LogError("Scale factor: " + parentCanvas.scaleFactor);
                Debug.LogError("Origin: " + origin);
                Debug.LogError("Width, Height: " + w + ", " + h);
                Debug.LogError("The Data Value: " + ActiveData.GetFrameAt(DataIndex)[0].Data[Row, Col]);
                Debug.LogError("Row, Col: " + Row + ", " + Col);
                currentframeToFile();
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
                        marker1Row = ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0) - 1 - (int)Math.Min(Math.Round(ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0) * NormalizedPoint1.x), (double)ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0) - 1);
                        marker1Col = ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1) - 1 - (int)Math.Min(Math.Round(ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1) * NormalizedPoint1.y), (double)ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1) - 1);
                        marker2Row = ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0) - 1 - (int)Math.Min(Math.Round(ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0) * NormalizedPoint2.x), (double)ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0) - 1);
                        marker2Col = ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1) - 1 - (int)Math.Min(Math.Round(ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1) * NormalizedPoint2.y), (double)ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1) - 1);

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
            if (ActiveData.GetCount() < 1)
            {
                return;
            }
           
            // Draw a line that represents the current slide on the graph
            ActiveData.Sort();
            if(DataIndex > ActiveData.GetCount())
            {
                DataIndex = 0;
            }
            float normTime = (float)(ActiveData.GetFrameAt(DataIndex)[0].starttime - Begin).TotalSeconds / (float)(End - Begin).TotalSeconds;
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
        /// Will clear the currently saved information.
        /// </summary>
        public void Clear()
        {
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
            ActiveData.Sort();

            // Build the new texture
            if (TrendTexture != null)
            {
                Texture2D.Destroy(TrendTexture);
            }
            TrendTexture = new Texture2D(width, height);

            // Loop through all the time series
            Vector2 prev = Record2PixelCoords(ActiveData.GetFrameAt(0)[0]);            
            for (int i = 0; i < ActiveData.GetCount(); i++)
            {
                Vector2 next = Record2PixelCoords(ActiveData.GetFrameAt(i)[0]);
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
            // Debug.LogError("The value x1, y1, x2, y2, x, y, ts(x1,y1), ts(x2,y1), ts(x1,y2), ts(x2,y2): " + x1 + ", " + y1 + ", " + x2 + ", " + y2 + ", " + x + ", " + y + ", " + ActiveData.GetFrameAt(DataIndex).Data[x1, y1] + ", " + ActiveData.GetFrameAt(DataIndex).Data[x2, y1] + ", " + ActiveData.GetFrameAt(DataIndex).Data[x1, y2] + ", " + ActiveData.GetFrameAt(DataIndex).Data[x2, y2]);

            // Run the Interpolation, and return.
            float value = (1 / ((x2 - x1) * (y2 - y1))) * ((ActiveData.GetFrameAt(DataIndex)[0].Data[x1, y1] * (x2 - x) * (y2 - y)) + (ActiveData.GetFrameAt(DataIndex)[0].Data[x2, y1] * (x - x1) * (y2 - y)) + (ActiveData.GetFrameAt(DataIndex)[0].Data[x1, y2] * (x2 - x) * (y - y1)) + (ActiveData.GetFrameAt(DataIndex)[0].Data[x2, y2] * (x - x1) * (y - y1)));
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
        Vector2 Record2PixelCoords(Frame record)
        {
            //float s = (float)(lastDraw - record.time).TotalSeconds;
            //float s = (float)(lastDraw - record.time).TotalSeconds;
            //float normTime = Mathf.Clamp01(1 - s / timebase);
            float normTime = (float)(record.starttime - Begin).TotalSeconds / (float)(End - Begin).TotalSeconds;
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
        /// This will set the easting and northing of the selected location.
        /// </summary>
        /// <param name="point">This is the point with a set value of x and z that represent easting and northing respectibly.</param>
        public void SetCoordPoint(Vector3 point)
        {
            easting = point.x;
            northing = point.z;
        }

        public void UpdateTimeDuration(DateTime start, DateTime end)
        {
            Begin = start;
            End = end;
            OnValidate();
        }

        /// <summary>
        /// This will send the data on the slicer to a file
        /// </summary>
        public void SlicerToFile()
        {
            String pathDownload = Utilities.GetFilePath("slicer_data.csv");
            Debug.LogError("The File Path: " + pathDownload);
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@pathDownload))
            {
                file.WriteLine("This file represenets interpolated data that was calculated from a line across the currently shown dataset.");
                file.WriteLine(variable_name + ": " + unitsLabel);
                file.WriteLine("Time of Frame: " + ActiveData.GetFrameAt(DataIndex)[0].starttime);
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
            String pathDownload = Utilities.GetFilePath("graph.csv");
            Debug.LogError("The File Path: " + pathDownload);
            
            using (StreamWriter file = new StreamWriter(@pathDownload))
            {
                file.WriteLine(variable_name + ": " + unitsLabel);
                file.WriteLine("Time Frame: " + Begin.ToString() + " to " + End.ToString());
                file.WriteLine("UTM: (" + easting + ", " + northing + ")");
                file.WriteLine("UTM Zone: " + coordsystem.localzone);
                for(int i = 0; i < ActiveData.GetCount(); i++)
                {
                    file.Write(ActiveData.GetFrameAt(i)[0].Data[Row, Col] + ", ");
                }
            }
        }

        /// <summary>
        /// This will send all the data from the current frame to a file
        /// </summary>
        public void currentframeToFile()
        {
            String pathDownload = Utilities.GetFilePath("frameToFile.csv");

            Debug.LogError("The File Path: " + pathDownload);

            using (StreamWriter file = new StreamWriter(@pathDownload))
            {
                for (int i = 0; i < ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(1); i++ )
                {
                    for (int j = 0; j < ActiveData.GetFrameAt(DataIndex)[0].Data.GetLength(0); j++ )
                    {
                        file.Write(ActiveData.GetFrameAt(DataIndex)[0].Data[i, j] + ", ");
                    }
                    file.Write("\n");
                }
            }
        }
    }
}