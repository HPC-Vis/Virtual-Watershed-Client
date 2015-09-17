/*
 * Copyright (c) 2015, Roger Lew (rogerlew.gmail.com)
 * Date: 4/25/2015
 * License: BSD (3-clause license)
 * 
 * The project described was supported by NSF award number IIA-1301792
 * from the NSF Idaho EPSCoR Program and by the National Science Foundation.
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VTL.ListView
{
    public class Row : MonoBehaviour
    {
        public bool isSelected = false;
        public bool selectedOn = false;
        public Guid guid;

        ListViewManager listViewManager; 
        Image image;

        // ColorType is used as a means to differentiate between  model run and variable
        Color ColorType = Color.white;

        // Who do I belong to!
        Row Parent;

        // Data related to this object
        public string Name;
        public DataRecord record;
        public ModelRun ModelRun;
        public string ModelRunUUID;
        public List<DataRecord> Variable;
        public List<GameObject> rowElements = new List<GameObject>();
        object[] content;

        public void Initialize(object[] fieldData, Guid guid)
        {
            content = fieldData;
            // Because these are instantiated this gets called before Start so
            // it is easier to just find the listViewManager here
            listViewManager = transform.parent.
                              transform.parent.
                              transform.parent.gameObject.GetComponent<ListViewManager>();

            // Need a reference to this to set the background color
            image = gameObject.GetComponent<Image>();

            this.guid = guid;

            // Build the row elements (cells)
            rowElements = new List<GameObject>();
            for (int i=0; i<fieldData.Length; i++)
            {
                // For each cell add a new RowElementPrefab and set the row as its parent
                rowElements.Add(Instantiate(listViewManager.RowElementPrefab));
                rowElements[i].transform.SetParent(transform);

                // Set the text
                Text rowElementText = rowElements[i].GetComponentInChildren<Text>();
                rowElementText.text = 
                    StringifyObject(fieldData[i],
                                    listViewManager.headerElementInfo[i].formatString,
                                    listViewManager.headerElementInfo[i].dataType);

                // Set the preferred width
                rowElements[i].GetComponentInChildren<LayoutElement>()
                              .preferredWidth = listViewManager.headerElementInfo[i].preferredWidth;

                // Set the horizontal alignment
                if (listViewManager.headerElementInfo[i].horizontalAlignment == HorizontalAlignment.Left)
                    rowElementText.alignment = TextAnchor.MiddleLeft;
                else
                    rowElementText.alignment = TextAnchor.MiddleRight;
            }
            //image.color = listViewManager.unselectedColor;
            image.color = ColorType; 
        }

        private static string StringifyObject(object obj, string formatString, DataType dataType)
        {
            if (dataType == DataType.String)
                return (string)obj;
            else if (formatString != null)
            {
                if (dataType == DataType.Int)
                    return ((int)obj).ToString(formatString);
                else if (dataType == DataType.Float)
                    return ((float)obj).ToString(formatString);
                else if (dataType == DataType.Double)
                    return ((double)obj).ToString(formatString);
                else if (dataType == DataType.DateTime)
                    return ((DateTime)obj).ToString(formatString);
                else if (dataType == DataType.TimeSpan)
                    return ((TimeSpan)obj).ToString();
                else
                    return obj.ToString();
            }
            else
                return obj.ToString();
        }

        public void OnPointerEnter(BaseEventData d)
        {
            Debug.LogError("HOVERED OVER ME");
            // Find the description rowelement.
        }

        public void UpdateSelectionAppearance()
        {
            image.color = isSelected ? listViewManager.selectedColor : 
                                       listViewManager.unselectedColor;
        }

        public void SetFields(object[] fieldData, Guid guid, bool selected)
        {
            this.guid = guid;
            isSelected = selected;
            UpdateSelectionAppearance();

            for (int i = 0; i < listViewManager.headerElementInfo.Count; i++)
                rowElements[i].GetComponentInChildren<Text>().text =
                    StringifyObject(fieldData[i],
                                    listViewManager.headerElementInfo[i].formatString,
                                    listViewManager.headerElementInfo[i].dataType); 
        }

        public void SetFields(Dictionary<string, object> rowData, Guid guid, bool selected)
        {
            this.guid = guid;
            isSelected = selected;
            UpdateSelectionAppearance();

            for (int i = 0; i < listViewManager.headerElementInfo.Count; i++)
                rowElements[i].GetComponentInChildren<Text>().text =
                    StringifyObject(rowData[listViewManager.headerElementInfo[i].text],
                                    listViewManager.headerElementInfo[i].formatString,
                                    listViewManager.headerElementInfo[i].dataType);
        }

        public void OnSelectionEvent()
        {
            listViewManager.OnSelectionEvent(guid, transform.GetSiblingIndex());
        }

        public object[] GetContents()
        {
            return content;
        }
    }
}
