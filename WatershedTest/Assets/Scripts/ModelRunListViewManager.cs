using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;


//public class ModelRunListViewManager : ListViewManager {

//    // Use this for initialization
//    void Start () {
	
//    }
	
//    // Update is called once per frame
//    void Update () {
	
//    }


//    public void AddRow(object[] fieldData,string UUID, string ModelRunUUID)
//    {
//        Debug.LogError(UUID);
//        Debug.LogError(ModelRunUUID);
//        if (fieldData.Length != headerElementInfo.Count)
//            throw new System.Exception("fieldData does not match the size of the table!");

//        Guid guid = Guid.NewGuid();

//        rows.Add(guid, Instantiate(RowPrefab));
//        rows[guid].transform.SetParent(listPanel.transform);
//        rows[guid].GetComponent<Row>().Initialize(fieldData, guid);
//        SetListPanelHeight();

//        listData.Add(guid, new Dictionary<string, object>());

//        for (int i = 0; i < fieldData.Length; i++)
//            listData[guid].Add(headerElementInfo[i].text, fieldData[i]);

//        listData[guid].Add(SELECTED, false);
//        listData[guid].Add(GUID, guid);

//    }
//}
