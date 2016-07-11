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
using System;
using System.Collections;
using System.Collections.Generic;

namespace VTL.ListView
{
        
    public class ListViewManager : ListViewManagerParent
    {
        public delegate void SelectionChangeAction();
        public static event SelectionChangeAction SelectionChangeEvent;

        const string SELECTED = "__Selected__";
        const string GUID = "__Guid__";

        // Use this for initialization
        void Awake()
        {
            header = transform.Find("Header").gameObject;
            listPanel = transform.Find("List/ListPanel").gameObject;
            listPanelRectTransform = listPanel.GetComponent<RectTransform>();

            // Destroy unneeded header elements
            foreach (Transform child in header.transform)
                if (!child.gameObject.activeSelf)
                    Destroy(child.gameObject);
        }

        void SetListPanelHeight()
        {
            listPanelRectTransform.sizeDelta =
                new Vector2(listPanelRectTransform.sizeDelta.x, rows.Count * rowHeight);
        }

        public new void OnSelectionEvent(Guid guid, int index)
        {          
            // Run the super
            base.OnSelectionEvent(guid, index);

            // Run the action on this item
            if (SelectionChangeEvent != null)
            {
                SelectionChangeEvent();
            }

            else if (RowPrefab.GetComponent("TrendGraphRow") != null)
            {
                if (TrendgraphSelectionChangeEvent != null)
                {
                    TrendgraphSelectionChangeEvent(guid);
                }
            }
            else
            {
                Debug.LogError("Problem loading the proper component.");
            }
        }

        public bool IsSelectedOn(Guid guid)
        {
            if (!rows.ContainsKey(guid))
            {
                return false;
            }
                
            if (RowPrefab.GetComponent("Row") != null)
            {
                return rows[guid].GetComponent<Row>().selectedOn;
            }
            else if (RowPrefab.GetComponent("TrendGraphRow") != null)
            {
                return rows[guid].GetComponent<TrendGraphRow>().selectedOn;
            }
            else
            {
                Debug.LogError("Problem loading the proper component.");
                return false;
            }            
        }

        public void SelectAll()
        {
            foreach (var item in rows)
                SetRowSelection(item.Key, true);
        }

        public void DeselectAll()
        {
            foreach (var item in rows)
                SetRowSelection(item.Key, false);
        }

        public void SetRowSelection(int index, bool selectedState)
        {
            SetRowSelection(GetGuidAtIndex(index), selectedState);
        }

        public void SetRowSelection(Guid guid, bool selectedState)
        {
            listData[guid][SELECTED] = selectedState;
         
            if (RowPrefab.GetComponent("Row") != null)
            {
                Row row = rows[guid].GetComponent<Row>();
                row.isSelected = selectedState;
                row.UpdateSelectionAppearance();
            }
            else if (RowPrefab.GetComponent("TrendGraphRow") != null)
            {
                TrendGraphRow row = rows[guid].GetComponent<TrendGraphRow>();
                row.isSelected = selectedState;
                row.UpdateSelectionAppearance();
            }
            else
            {
                Debug.LogError("Problem loading the proper component.");
            }            
        }

        public void Sort(string key)
        {
            Sort(key, true);
        }

        public void Sort(string key, bool sortAscending)
        {
            // Check that key is valid
            bool foundKey = false;
            foreach (var info in headerElementInfo)
                if (info.text.Equals(key))
                    foundKey = true;

            if (!foundKey)
                throw new System.Exception("Key not in listview: " + key);

            // Here we sort without Linq for maximum platform compatibility
            // We only need to sort unique elements of a column. So we create
            // a lookup dictionary to get all Guids that coorespond to a
            // particular unique element
            Dictionary<object, List<Guid>> lookup = new Dictionary<object, List<Guid>>();
            foreach (var item in listData)
            {
                if (!lookup.ContainsKey(item.Value[key]))
                    lookup[item.Value[key]] = new List<Guid>();

                lookup[item.Value[key]].Add(item.Key);
            }

            // Now sort the keys to the lookup table
            List<object> uniqueElements = new List<object>(lookup.Keys);
            uniqueElements.Sort(); // Sort in place

            if (!sortAscending)
                uniqueElements.Reverse(); // Reverse in place

            // Reorder the rows
            int i = 0;
            foreach (object objKey in uniqueElements)
            {
                foreach (Guid guid in lookup[objKey])
                    rows[guid].transform.SetSiblingIndex(i++);
            }

            // Set the arrow states for the header fields
            foreach (Transform child in header.transform)
            {
                var headerElement = child.GetComponent<HeaderElement>();
                if (headerElement != null)
                    headerElement.SetSortState(headerElement.text == key ? sortAscending : (bool?)null);
            }
        }

        public Guid GetGuidAtIndex(int index)
        {
            return listPanel.transform.GetChild(index).GetComponent<Row>().guid;
        }

        public Guid GetGuidAtSelectedIndex(int index)
        {
            int indexCount = 0;
            if (RowPrefab.GetComponent("Row") != null)
            {
                foreach (var item in rows)
                {
                    var ROW = item.Value.GetComponent<Row>();
                    if (ROW.isSelected)
                    {
                        if(indexCount == index)
                        {
                            return ROW.guid;
                        }
                        indexCount++;
                    }
                }
                throw new ArgumentNullException("No index selected of that value.");
            }
            else if (RowPrefab.GetComponent("TrendGraphRow") != null)
            {
                foreach (var item in rows)
                {
                    var ROW = item.Value.GetComponent<TrendGraphRow>();
                    if (ROW.isSelected)
                    {
                        if (indexCount == index)
                        {
                            return ROW.guid;
                        }
                        indexCount++;
                    }
                }
                throw new ArgumentNullException("No index selected of that value.");
            }
            else
            {
                throw new ArgumentNullException("Problem loading the proper component.");
            }            
        }

        public void UpdateRow(Guid guid, object[] fieldData)
        {
            if (fieldData.Length < headerElementInfo.Count)
                throw new System.Exception("fieldData does not match the size of the table!");

            for (int i = 0; i < fieldData.Length; i++)
            {
                listData[guid][headerElementInfo[i].text] = fieldData[i];
            }               

            bool selected = (bool)listData[guid][SELECTED];
            rows[guid].GetComponent<Row>().SetFields(fieldData, guid, selected);
        }

        public void UpdateRow(int index, object[] fieldData)
        {
            UpdateRow(GetGuidAtIndex(index), fieldData);
        }

        public void UpdateRow(Guid guid, Dictionary<string, object> rowData)
        {
            foreach (var item in rowData)
                listData[guid][item.Key] = item.Value;

            bool selected = (bool)listData[guid][SELECTED];
            rows[guid].GetComponent<Row>().SetFields(listData[guid], guid, selected);
        }

        public void UpdateRow(int index, Dictionary<string, object> rowData)
        {
            UpdateRow(GetGuidAtIndex(index), rowData);
        }

        public void UpdateRowField(Guid guid, string key, object data)
        {
            listData[guid][key] = data;

            bool selected = (bool)listData[guid][SELECTED];

            if (RowPrefab.GetComponent("Row") != null)
            {
                rows[guid].GetComponent<Row>().SetFields(listData[guid], guid, selected);
            }
            else if (RowPrefab.GetComponent("TrendGraphRow") != null)
            {
                rows[guid].GetComponent<TrendGraphRow>().SetFields(listData[guid], guid, selected);
            }
            else
            {
                throw new ArgumentNullException("Problem loading the proper component.");
            }            
        }

        public void UpdateRowField(int index, string key, object data)
        {
            UpdateRowField(GetGuidAtIndex(index), key, data);
        }

        public IEnumerator Selected()
        {
            var buffer = new List<Guid>();
            foreach (var rowData in listData.Values)
                if ((bool)rowData[SELECTED])
                    buffer.Add((Guid)rowData[GUID]);
 
            foreach (Guid guid in buffer)
                yield return guid;
        }

        public void RemoveSelected()
        {
            IEnumerator ienObj = Selected();

            while (ienObj.MoveNext())
                Remove((Guid)ienObj.Current);
        }

        public void Remove(Guid guid)
        {
            Destroy(rows[guid]);
            rows.Remove(guid);
            listData.Remove(guid);
            SetListPanelHeight();
        }

        public void RemoveAt(int index)
        {
            Remove(GetGuidAtIndex(index));
        }

        public void Clear()
        {
            //Debug.Log("DESTROY ALL");
            SelectAll();
            RemoveSelected();

        }

        public void AddRow(object[] fieldData, DataRecord record)
        {
            if (fieldData.Length != headerElementInfo.Count)
                throw new System.Exception("fieldData does not match the size of the table!");

            Guid guid = Guid.NewGuid();

            rows.Add(guid, Instantiate(RowPrefab));
            rows[guid].transform.SetParent(listPanel.transform);
            rows[guid].GetComponent<ModelRow>().Initialize(fieldData, guid);
            rows[guid].GetComponent<ModelRow>().record = record;
            SetListPanelHeight();

            listData.Add(guid, new Dictionary<string, object>());

            for (int i = 0; i < fieldData.Length; i++)
                listData[guid].Add(headerElementInfo[i].text, fieldData[i]);

            listData[guid].Add(SELECTED, false);
            listData[guid].Add(GUID, guid);

        }

        public void AddRow(object[] fieldData, ModelRun modelRun)
        {
            if (fieldData.Length != headerElementInfo.Count)
                throw new System.Exception("fieldData does not match the size of the table!");

            Guid guid = Guid.NewGuid();

            rows.Add(guid, Instantiate(RowPrefab));
            rows[guid].transform.SetParent(listPanel.transform);
            rows[guid].GetComponent<ModelRow>().Initialize(fieldData, guid);
            rows[guid].GetComponent<ModelRow>().ModelRun = modelRun;
            rows[guid].GetComponent<ModelRow>().ModelRunUUID = modelRun.ModelRunUUID;

            SetListPanelHeight();

            listData.Add(guid, new Dictionary<string, object>());

            for (int i = 0; i < fieldData.Length; i++)
                listData[guid].Add(headerElementInfo[i].text, fieldData[i]);

            listData[guid].Add(SELECTED, false);
            listData[guid].Add(GUID, guid);

        }

        public List<DataRecord> GetSelected()
        {
            List<DataRecord> records = new List<DataRecord>();
            foreach (var item in rows)
            {
                var ROW = item.Value.GetComponent<ModelRow>();
                var interfaceValues = ROW as Row;
                if (interfaceValues.isSelected)
                {
                    records.Add(ROW.record);
                }
            }
            return records;
        }        

        public List<ModelRun> GetSelectedModelRuns()
        {

            List<ModelRun> mrs = new List<ModelRun>();
            foreach (var item in rows)
            {
                var ROW = item.Value.GetComponent<ModelRow>();
                var interfaceValues = ROW as Row;
                if (interfaceValues.isSelected)
                {
                    mrs.Add(ROW.ModelRun);
                }
            }
            return mrs;
        }
    }
}