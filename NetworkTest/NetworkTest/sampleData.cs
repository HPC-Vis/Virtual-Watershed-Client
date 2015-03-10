using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class sampleData : MonoBehaviour {
    public Text NameId, DescriptionId, TypeId, StartDateId, EndDateId;
    public Toggle nameCheck, descriptionCheck, typeCheck, startDateCheck, endDateCheck;
    public int counter = 1, month, day, year;
    public List<DataRecord> SampleData = new List<DataRecord>();
    public List<int> indices = new List<int>();
    public bool sortingAvailable = new bool();

    enum sorted { name = 1, location = 2, description = 4, type = 8, modelname = 16, projection = 32, start = 64, end = 128};
    sorted sortedBy = sorted.name;
    


	// Use this for initialization
	void Start () {
        NameId = GameObject.Find("Name/ScrollRect/Text").GetComponent<Text>();
        DescriptionId = GameObject.Find("Description/ScrollRect/Text").GetComponent<Text>();
        TypeId = GameObject.Find("Type/ScrollRect/Text").GetComponent<Text>();
        StartDateId = GameObject.Find("StartDate/ScrollRect/Text").GetComponent<Text>();
        EndDateId = GameObject.Find("EndDate/ScrollRect/Text").GetComponent<Text>();
        nameCheck = GameObject.Find("SortByName").GetComponent<Toggle>();
        descriptionCheck = GameObject.Find("SortByDescription").GetComponent<Toggle>();
        typeCheck = GameObject.Find("SortByType").GetComponent<Toggle>();
        startDateCheck = GameObject.Find("SortByStartDate").GetComponent<Toggle>();
        endDateCheck = GameObject.Find("SortByEndDate").GetComponent<Toggle>();
        sortingAvailable = false;

        
	}

    public void setData(List<DataRecord> records)
    {
        SampleData = records;
        indices.Clear();
        for (int i = 0; i < SampleData.Count; i++)
        {
            indices.Add(i);
        }
        sortData();
        GameObject.Find("ScrollRect").GetComponent<ScrollRect>().velocity = new Vector2(0f, 4000f);

    }

    //public void swapElements(int k, int j, List<List<string>> record)
    //{
    //    string temp;
    //    temp = record[k][j - 1];
    //    record[k][j - 1] = record[k][j];
    //    record[k][j] = temp;
    //}

    //public void sortBy(int listInd, List<List<string>> record)
    //{   
    //    for (int i = 0; i < record[listInd].Count; i++)
    //    {
    //        for (int j = 1; j < record[listInd].Count; j++)
    //        {
    //            if (String.Compare(record[listInd][j - 1], record[listInd][j]) > 0)
    //            {
    //                for (int k = 0; k < record.Count; k++)
    //                {
    //                    swapElements(k, j, record);
    //                }
    //            }
    //        }
    //    } 
    //}

    public void sortByType()
    {
        
        switch (sortedBy)
        {
            case sorted.name:
                indices.Sort((x, y) => SampleData[x].name.CompareTo(SampleData[y].name));
                break;

            case sorted.location:
                indices.Sort((x, y) => SampleData[x].location.CompareTo(SampleData[y].location));
                break;

            case sorted.description:
                indices.Sort((x, y) => SampleData[x].description.CompareTo(SampleData[y].description));
                break;

            case sorted.type:
                indices.Sort((x, y) => SampleData[x].TYPE.CompareTo(SampleData[y].TYPE));
                break;

            case sorted.modelname:
                indices.Sort((x, y) => SampleData[x].modelname.CompareTo(SampleData[y].modelname));
                break;

            case sorted.projection:
                indices.Sort((x, y) => SampleData[x].projection.CompareTo(SampleData[y].projection));
                break;

            case sorted.start:
                indices.Sort((x, y) => SampleData[x].start.CompareTo(SampleData[y].start));
                break;

            case sorted.end:
                indices.Sort((x, y) => SampleData[x].end.CompareTo(SampleData[y].end));
                break;

            default:
                indices.Sort((x, y) => SampleData[x].name.CompareTo(SampleData[y].name));
                break;

        }
        
    }

    public void populateList(List<DataRecord> record)
    {
        NameId.text = DescriptionId.text = TypeId.text = StartDateId.text = EndDateId.text = null;
        if (sortingAvailable)
        {
            for (int i = 0; i < record.Count; i++)
            {
                NameId.text += record[indices[i]].name + "\n";
                DescriptionId.text += record[indices[i]].description + "\n";
                TypeId.text += record[indices[i]].TYPE + "\n";
                StartDateId.text += record[indices[i]].start + "\n";
                EndDateId.text += record[indices[i]].end + "\n";
            }
        }
    }

    public void sortData()
    {
        if (nameCheck.isOn)
        {
            sortedBy = sorted.name;
            sortByType();
        }
        else if (descriptionCheck.isOn)
        {
            sortedBy = sorted.description;
            sortByType();
        }
        else if (typeCheck.isOn)
        {
            sortedBy = sorted.type;
            sortByType();
        }
        else if (startDateCheck.isOn)
        {
            sortedBy = sorted.start;
            sortByType();
        }
        else if (endDateCheck.isOn)
        {
            sortedBy = sorted.end;
            sortByType();
        }
        else
        {
            sortedBy = sorted.name;
            sortByType();
        }
        populateList(SampleData);
    }
}
