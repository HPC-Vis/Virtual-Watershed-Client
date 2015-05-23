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

        public TimeseriesRecord(DateTime time, float value)
        {
            this.time = time;
            this.value = value;
        }
    }

    public class TrendGraphController : MonoBehaviour
    {
		readonly static object Lock = new object();
        public Color lineColor = Color.white;
        public float lineWidth = 1f;
        public float yMax = 1;
        public float yMin = 0;
        public float timebase = 300; // in seconds
        public string timebaseLabel = "-5 min"; // Its on the developer to make sure 
                                                // this makes sense with the timebase
        public string unitsLabel = "F"; // the units label
        public string valueFormatString = "D3";

        List<TimeseriesRecord> timeseries;
        DateTime lastDraw;
        Text valueText;
        Vector3 origin;
        float w;
        float h;

        RectTransform rectTransform;
        Transform lineAnchor;

        public void OnValidate()
        {
            transform.Find("Ymax")
                     .GetComponent<Text>()
                     .text = yMax.ToString();

            transform.Find("Ymin")
                     .GetComponent<Text>()
                     .text = yMin.ToString();

            transform.Find("Timebase")
                     .GetComponent<Text>()
                     .text = timebaseLabel;

            transform.Find("Units")
                     .GetComponent<Text>()
                     .text = unitsLabel;
        }

        // Use this for initialization
        void Start()
        {
            timeseries = new List<TimeseriesRecord>();
            rectTransform = transform.Find("Graph") as RectTransform;
            lineAnchor = transform.Find("Graph")
                                  .Find("LineAnchor");
            valueText = transform.Find("Value").GetComponent<Text>();
        }

        // The Drawing.DrawLine method using the GL and GUI class and 
        // has to be drawn in draw line
        void OnGUI()
        {
			lock (Lock) {
				int n = timeseries.Count;

				if (n < 2)
					return;

				// Need to check the origin and the width and height everytime
				// just in case the panel has been resized
				origin = rectTransform.position;
				origin.y = Screen.height - origin.y;

				w = rectTransform.rect.width * transform.localScale.x;
				h = rectTransform.rect.height * transform.localScale.y;

				// Need to save the latest time so we can remove old records
				lastDraw = timeseries [n - 1].time;

				// Iterate through the timeseries and draw the trend segment
				// by segment.
				var prev = Record2PixelCoords (timeseries [0]);
				for (int i = 1; i < n; i++) {
					var next = Record2PixelCoords (timeseries [i]);
					Drawing.DrawLine (prev, next, lineColor, lineWidth, false);
					prev = next;
				}
			}
        }

        // converts a TimeseriesRecord to screen pixel coordinates for plotting
        Vector2 Record2PixelCoords(TimeseriesRecord record)
        {
            float s = (float)(lastDraw - record.time).TotalSeconds;
            float normTime = Mathf.Clamp01(1 - s / timebase);
            float normHeight = Mathf.Clamp01((record.value - yMin) / (yMax - yMin));
            return new Vector2(origin.x + w * normTime,
                               origin.y + h * (1 - normHeight));
        }

        // add a time series to the trend graph
        public void Add(TimeseriesRecord record)
        {
            timeseries.Add(record);
            valueText.text = record.value.ToString(valueFormatString);

            // cull old records
            while ((lastDraw - timeseries[0].time).TotalSeconds > (double)timebase)
                timeseries.RemoveAt(0);
        }
        
		public void Clear()
		{
			lock(Lock)
			timeseries.Clear ();
		}

        public void Add(DateTime time, float value)
        {
            Add(new TimeseriesRecord(time, value));
        }
    }
}
