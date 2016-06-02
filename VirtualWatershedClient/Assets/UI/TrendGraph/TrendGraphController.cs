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
using VTL.ListView;

namespace VTL.TrendGraph
{
    public class TrendGraphController : MonoBehaviour
    {
        // Local variables
        private float yMax = 1;
        private float yMin = 0;
        private string unitsLabel = "";
        private float w;
        private float h;
        private RectTransform rectTransform;
        private Texture2D TrendTexture = null;
        private DateTime Begin = DateTime.MaxValue;
        private DateTime End = DateTime.MinValue;
        private Rect BoundingBox;

        // Connections
        public Material GraphMaterial;
        public Image GraphImage;        
        public TrendGraphListView selectedList;
        
        
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

        public void UpdateData(string variable)
        {
            // TODO: Handle multiple projections for a projector

            // Get the active variables
            List<string> tempRef = ActiveData.GetCurrentAvtive();
            Rect bb = ActiveData.GetBoundingBox(tempRef[0]);
            Debug.LogError("TrendGraph: " + bb);

            // Set the bounding box to the trendgraph
            WorldTransform tran = new WorldTransform(ActiveData.GetProjection(tempRef[0]));

            // Create a coordinate transform
            tran.createCoordSystem(ActiveData.GetProjection(tempRef[0]));

            // transfor a lat/long bounding box to UTM
            //tran.setOrigin(coordsystem.WorldOrigin);
            Vector2 point = tran.transformPoint(new Vector2(bb.x, bb.y));
            Vector2 point2 = tran.transformPoint(new Vector2(bb.x + bb.width, bb.y - bb.height));

            if ((bb.x > -180 && bb.x < 180 && bb.y < 180 && bb.y > -180))
            {
                bb = new Rect(point.x, point.y, Math.Abs(point.x - point2.x), Math.Abs(point.y - point2.y));
            }

            Debug.LogError("TrendGraph: " + bb);
            BoundingBox = bb;
            SetUnit(variable);
            Compute();
        }

        /// <summary>
        /// This sets the min value of the data. This is for the X axis.
        /// </summary>
        public void UpdateMinMax()
        {
            float min, max;
            List<string> tempRef = ActiveData.GetCurrentAvtive();
            if (tempRef.Count > 1)
            {
                Vector2 d1 = ActiveData.GetMinMax(tempRef[0]);
                Vector2 d2 = ActiveData.GetMinMax(tempRef[1]);
                min = d1.x - d2.y;
                max = d1.y - d2.x;
            }
            else
            {
                Vector2 d1 = ActiveData.GetMinMax(tempRef[0]);
                min = d1.x;
                max = d1.y;
            }
            yMin = min;
            yMax = max;
            OnValidate();
        }

        /// <summary>
        /// Use this for initialization of the trend graph and the locations on the scene.
        /// </summary>
        void Start()
        {
            rectTransform = transform.Find("Graph") as RectTransform;

            // Set image material
            GraphImage.material = GraphMaterial;

            // Set the width and height to integers
            int width = (int)w;
            int height = (int)h;

            // Destroy the old texture
            if (TrendTexture != null)
            {
                Texture2D.Destroy(TrendTexture);
            }

            // Init the texture
            TrendTexture = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    TrendTexture.SetPixel(x, y, Color.black);
                }
            }

            // Apply to the world
            TrendTexture.wrapMode = TextureWrapMode.Clamp;
            TrendTexture.Apply();
            GraphImage.sprite = Sprite.Create(TrendTexture, new Rect(0, 0, width, height), new Vector2(0, 0));
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
                Vector3 WorldPoint = mouseray.CursorPosition; // coordsystem.transformToWorld(mouseray.CursorPosition);
                Vector2 CheckPoint = new Vector2(WorldPoint.x, WorldPoint.z);

                if (BoundingBox.Contains(CheckPoint))
                {
                    int DataIndex = ActiveData.GetCurrentIndex();
                    Vector2 NormalizedPoint = Vector2.zero;
                    NormalizedPoint = TerrainUtils.NormalizePointToTerrain(WorldPoint, BoundingBox);
                    List<String> tempFrameRef = ActiveData.GetCurrentAvtive();
                    int x = (int)Math.Min(Math.Round(ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(0) * NormalizedPoint.x), (double)ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(0) - 1);
                    int y = (int)Math.Min(Math.Round(ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(1) * NormalizedPoint.y), (double)ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(1) - 1);

                    // Set the row then redraw
                    for(int i = 0; i < tempFrameRef.Count; i++)
                    {
                        selectedList.AddRow(new object[] { WorldPoint.z.ToString("#,##0") + "  Long: " + WorldPoint.x.ToString("#,##0"), tempFrameRef[i], ActiveData.GetFrameAt(tempFrameRef[i], DataIndex).Data.GetLength(1) - 1 - y, ActiveData.GetFrameAt(tempFrameRef[i], DataIndex).Data.GetLength(0) - 1 - x });
                    }
                    SetPosition(ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(1) - 1 - y, ActiveData.GetFrameAt(tempFrameRef[0], DataIndex).Data.GetLength(0) - 1 - x);                                        
                }
            }
            
            // Run the old OnGUI function
            RunRecalculationCheck();
        }

        /// <summary>
        /// This wll ensure that the sizing of the trend graph is correct, and if not there will be a recomputation
        /// </summary>
        void RunRecalculationCheck()
        {
            List<String> FrameReference = ActiveData.GetCurrentAvtive();
            if (FrameReference.Count < 1)
            {
                return;
            }
            if (ActiveData.GetCount(FrameReference[0]) < 1)
            {
                return;
            }

            // Draw a line that represents the current slide on the graph
            int DataIndex = ActiveData.GetCurrentIndex();
            if (DataIndex > ActiveData.GetCount(FrameReference[0]))
            {
                DataIndex = 0;
            }
            float normTime = (float)(ActiveData.GetFrameAt(FrameReference[0], DataIndex).starttime - Begin).TotalSeconds / (float)(End - Begin).TotalSeconds;
            // Drawing.DrawLine(new Vector2(origin.x + w * normTime * parentCanvas.scaleFactor, origin.y), new Vector2(origin.x + w * normTime * parentCanvas.scaleFactor, origin.y + h * 1 * parentCanvas.scaleFactor), Color.yellow, lineWidth, true);
            GraphImage.material.SetFloat("Time", normTime);
            
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
            OnValidate();
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

            // Destroy the old texture
            if (TrendTexture != null)
            {
                Texture2D.Destroy(TrendTexture);
            }

            // Init the texture
            TrendTexture = new Texture2D(width, height);
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    TrendTexture.SetPixel(x, y, Color.black);
                }
            }

            // Loop through all the time series
            List<object[]> tempFrameRef = selectedList.GetSelectedRowContent(); // ActiveData.GetCurrentAvtive();
            for (int j = 0; j < tempFrameRef.Count; j++)
            {
                Vector2 prev = Record2PixelCoords(ActiveData.GetFrameAt((string)tempFrameRef[j][2], 0), (int)tempFrameRef[j][3], (int)tempFrameRef[j][4]);
                for (int i = 1; i < ActiveData.GetCount((string)tempFrameRef[j][2]); i++)
                {
                    Vector2 next = Record2PixelCoords(ActiveData.GetFrameAt((string)tempFrameRef[j][2], i), (int)tempFrameRef[j][3], (int)tempFrameRef[j][4]);
                    Line(TrendTexture, (int)prev.x, (int)prev.y, (int)next.x, (int)next.y, (Color)tempFrameRef[j][0]);
                    prev = next; 
                }
            }

            // Apply to the world
            TrendTexture.wrapMode = TextureWrapMode.Clamp;
            TrendTexture.Apply();
            GraphImage.sprite = Sprite.Create(TrendTexture, new Rect(0, 0, width, height), new Vector2(0, 0));
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
        Vector2 Record2PixelCoords(Frame record, int row, int col)
        {
            //float s = (float)(lastDraw - record.time).TotalSeconds;
            //float s = (float)(lastDraw - record.time).TotalSeconds;
            //float normTime = Mathf.Clamp01(1 - s / timebase);
            float normTime = (float)(record.starttime - Begin).TotalSeconds / (float)(End - Begin).TotalSeconds;
            float normHeight = 0;
            if (record.Data != null)
            {
                normHeight = Mathf.Clamp01((record.Data[row, col] - yMin) / (yMax - yMin));
            }
            //float normHeight = Mathf.Clamp01((record.value - yMin) / (yMax - yMin));
            return new Vector2(w * normTime, h * (1 - normHeight) - 1);
        }

        /// <summary>
        /// This will set the title of the graph with the string value.
        /// </summary>
        /// <param name="unit">The string value to set the unit type to.</param>
        public void SetUnit(string unit)
        {
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
            Debug.Log("Trend Graph row: " + row + " col: " + col);
            Compute();            
        }

        public void UpdateTimeDuration(DateTime start, DateTime end)
        {
            Begin = start;
            End = end;
            OnValidate();
        }        

        /// <summary>
        /// This will take all the data at the selected point on the data set, throughout all datasets of time,
        /// and send to a file on the Desktop named graph.txt.
        /// </summary>
        public void dataToFile()
        {
            List<object[]> tempFrameRef = selectedList.GetSelectedRowContent(); // ActiveData.GetCurrentAvtive();
            for (int j = 0; j < tempFrameRef.Count; j++)
            {
                String pathDownload = Utilities.GetFilePath((string)tempFrameRef[j][2] + "_" + tempFrameRef[j][3] + "_" + tempFrameRef[j][4] + "_graph.csv");
                using (StreamWriter file = new StreamWriter(@pathDownload))
                {                 
                    file.WriteLine((string)tempFrameRef[j][2] + ": " + VariableReference.GetDescription((string)tempFrameRef[j][2]));
                    file.WriteLine("Time Frame: " + Begin.ToString() + " to " + End.ToString());
                    file.WriteLine((string)tempFrameRef[j][1]);
                    file.WriteLine("UTM Zone: " + coordsystem.localzone);
                    for (int i = 0; i < ActiveData.GetCount((string)tempFrameRef[j][2]); i++)
                    {
                        file.Write(ActiveData.GetFrameAt(name, i).Data[(int)tempFrameRef[j][3] , (int)tempFrameRef[j][4]] + ", ");
                    }
                }
            }
        }        
    }
}