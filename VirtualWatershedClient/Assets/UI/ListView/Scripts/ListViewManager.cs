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
            SelectionChangeEvent();
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