using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;

public class TrendGraphRow : MonoBehaviour, Row {

    public bool _isSelected = false;
    public bool _selectedOn = false;
    public Guid _guid;

    ListViewManager listViewManager;
    Image image;

    // ColorType is used as a means to differentiate between  model run and variable
    Color ColorType = Color.white;


    // Data related to this object
    public Image key;
    public string Name;
    public string Variable;
    private int Row;
    private int Col;

    public List<GameObject> rowElements = new List<GameObject>();
    object[] content;
    RectTransform RowTransform = null;
    bool once = false;
    public GameObject imageRow;

    bool Row.isSelected
    {
        get { return _isSelected; }
        set { _isSelected = value; }
    }

    bool Row.selectedOn
    {
        get { return _selectedOn; }
        set { _selectedOn = value; }
    }

    Guid Row.guid
    {
        get { return _guid; }
        set { _guid = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldData"></param>
    /// <param name="guid"></param>
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

        _guid = guid;

        // Step 1 of this patch grab the row transform ...
        RowTransform = gameObject.GetComponent<RectTransform>();
        RowTransform.localScale = Vector3.one;

        // For each cell add a new RowElementPrefab and set the row as its parent
        rowElements.Add(Instantiate(imageRow));
        rowElements[0].transform.SetParent(transform);
        rowElements[0].transform.localScale = Vector3.one;

        // Set the color
        Image rowElementImage = rowElements[0].GetComponentInChildren<Image>();
        rowElementImage.color = (Color)fieldData[0];

        // Set the preferred width
        rowElements[0].GetComponentInChildren<LayoutElement>()
                        .preferredWidth = listViewManager.headerElementInfo[0].preferredWidth;



        // Build the row elements (cells)
        for (int i = 1; i < fieldData.Length - 2; i++)
        {
            // For each cell add a new RowElementPrefab and set the row as its parent
            rowElements.Add(Instantiate(listViewManager.RowElementPrefab));
            rowElements[i].transform.SetParent(transform);
            rowElements[i].transform.localScale = Vector3.one;

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

        Name = (string)fieldData[1];
        Variable = (string)fieldData[2];

        // Additional pieces not shown in header
        Row = (int)fieldData[3];
        Col = (int)fieldData[4];

        //image.color = listViewManager.unselectedColor;
        image.color = ColorType;
    }

    string Row.StringifyObject(object obj, string formatString, DataType dataType)
    {
        return StringifyObject(obj, formatString, dataType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="formatString"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
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
        //Debug.LogError("HOVERED OVER ME");
        // Find the description rowelement.
    }

    public void UpdateSelectionAppearance()
    {
        image.color = _isSelected ? listViewManager.selectedColor :
                                    listViewManager.unselectedColor;
    }

    public void SetFields(object[] fieldData, Guid guid, bool selected)
    {
        _guid = guid;
        _isSelected = selected;
        UpdateSelectionAppearance();

        SetValues(fieldData);
    }

    public void SetFields(Dictionary<string, object> rowData, Guid guid, bool selected)
    {
        object[] values = new object[listViewManager.headerElementInfo.Count];
        for (int i = 0; i < listViewManager.headerElementInfo.Count; i++)
        {
            values[i] = rowData[listViewManager.headerElementInfo[i].text];
        }
        SetFields(values, guid, selected);
    }

    public void OnSelectionEvent()
    {
        listViewManager.OnSelectionEvent(_guid, transform.GetSiblingIndex());
    }

    public object[] GetContents()
    {
        return content;
    }

    private void SetValues(object [] fieldData)
    {
        // Set the color
        Image rowElementImage = rowElements[0].GetComponentInChildren<Image>();
        rowElementImage.color = (Color)fieldData[0];
        content[0] = (Color)fieldData[0];

        // Build the row elements (cells)
        for (int i = 1; i < fieldData.Length - 2; i++)
        {
            // Set the text
            Text rowElementText = rowElements[i].GetComponentInChildren<Text>();
            rowElementText.text =
                StringifyObject(fieldData[i],
                                listViewManager.headerElementInfo[i].formatString,
                                listViewManager.headerElementInfo[i].dataType);
            content[i] = fieldData[i];
        }

        Name = (string)fieldData[1];
        Variable = (string)fieldData[2];
    }    
}
