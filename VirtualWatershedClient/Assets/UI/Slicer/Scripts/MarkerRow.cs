using UnityEngine;
using System.Collections;
using VTL.ListView;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class MarkerRow : MonoBehaviour, Row {

    public bool _isSelected = false;
    public bool _selectedOn = false;
    public Guid _guid;

    MarkerListView listViewManager;
    Image image;

    // ColorType is used as a means to differentiate between  model run and variable
    Color ColorType = Color.white;


    // Data related to this object
    public Image key;
    private string Name;
    private Vector3 StartPosition;
    private Vector3 EndPosition;

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
                            transform.parent.gameObject.GetComponent<MarkerListView>();

        // Need a reference to this to set the background color
        image = gameObject.GetComponent<Image>();

        _guid = guid;

        // Step 1 of this patch grab the row transform ...
        RowTransform = gameObject.GetComponent<RectTransform>();
        RowTransform.localScale = Vector3.one;


        // Build the row elements (cells)
        for (int i = 0; i < fieldData.Length - 2; i++)
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

        Name = (string)fieldData[0];

        // Additional pieces not shown in header
        StartPosition = (Vector3)fieldData[1];
        EndPosition = (Vector3)fieldData[2];

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
        image.color = _isSelected ? listViewManager.selectedColor : listViewManager.unselectedColor;
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

    private void SetValues(object[] fieldData)
    {
        // Build the row elements (cells)
        for (int i = 0; i < fieldData.Length; i++)
        {
            // Set the text
            Text rowElementText = rowElements[i].GetComponentInChildren<Text>();
            rowElementText.text =
                StringifyObject(fieldData[i],
                                listViewManager.headerElementInfo[i].formatString,
                                listViewManager.headerElementInfo[i].dataType);
            content[i] = fieldData[i];
        }

        Name = (string)fieldData[0];
    }

    public void GetPositions(out Vector3 m1, out Vector3 m2)
    {
        m1 = StartPosition;
        m2 = EndPosition;
    }
}
