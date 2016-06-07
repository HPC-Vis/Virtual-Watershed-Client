using UnityEngine;
using System.Collections;
using VTL.ListView;
using System;
using System.Collections.Generic;
using VTL.TrendGraph;

public class TrendGraphListView : ListViewManagerParent
{
    // The selected graph points
    public Queue<Color> colors = new Queue<Color>();
    public TrendGraphController trendgraph;

    // For use with the trend graph selection
    public delegate void TrendgraphSelectionChangeAction(Guid guid);
    public static event TrendgraphSelectionChangeAction TrendgraphSelectionChangeEvent;

    // Use this for initialization
    void Start () {
        // Setup the colors
        colors.Enqueue(Color.blue);
        colors.Enqueue(Color.green);
        colors.Enqueue(Color.white);

        // Set the selection event
        TrendgraphSelectionChangeEvent += SelectionEvent;
    }

    // Use this for initialization
    void Awake()
    {
        header = transform.Find("Header").gameObject;
        listPanel = transform.Find("List/ListPanel").gameObject;
        listPanelRectTransform = listPanel.GetComponent<RectTransform>();

        // Destroy unneeded header elements
        foreach (Transform child in header.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                Destroy(child.gameObject);
            }                
        }            
    }

    void SetListPanelHeight()
    {
        listPanelRectTransform.sizeDelta =
            new Vector2(listPanelRectTransform.sizeDelta.x, rows.Count * rowHeight);
    }

    public new void AddRow(object[] v)
    {
        List<TrendGraphRow> selected = GetSelectedTrendGraphRows();
        if(selected.Count == 3)
        {
            var interfaceValues = selected[0] as Row;
            SetRowSelection(interfaceValues.guid, !interfaceValues.isSelected);
            UpdateRowField(interfaceValues.guid, " ", Color.clear);
        }

        object[] newObj = { colors.Dequeue(), v[0], v[1], v[2], v[3] };
        colors.Enqueue((Color)newObj[0]);

        Guid id = base.AddRow(newObj);
        SetRowSelection(id, true);
    }

    public List<TrendGraphRow> GetSelectedTrendGraphRows()
    {
        List<TrendGraphRow> sel = new List<TrendGraphRow>();
        foreach (var item in rows)
        {
            TrendGraphRow ROW = item.Value.GetComponent<TrendGraphRow>();
            var interfaceValues = ROW as Row;
            if (interfaceValues.isSelected)
            {
                sel.Add(ROW);
            }
        }
        return sel;
    }

    public new void OnSelectionEvent(Guid guid, int index)
    {
        // Run the super
        base.OnSelectionEvent(guid, index);

        // Run the action on this item
        if (TrendgraphSelectionChangeEvent != null)
        {
            TrendgraphSelectionChangeEvent(guid);
        }
    }

    public void SelectionEvent(Guid guid)
    {
        Color currentRowColor = (Color)GetRowContent(guid)[0];
        if (currentRowColor != Color.clear)
        {            
            int colorSize = colors.Count;
            colors.Enqueue(currentRowColor);
            for(int i = 0; i < colorSize; i++)
            {
                Color temp = colors.Dequeue();
                if(temp != currentRowColor)
                {
                    colors.Enqueue(temp);
                }                
            }
            UpdateRowField(guid, " ", Color.clear);
        }
        else
        {
            List<TrendGraphRow> selected = GetSelectedTrendGraphRows();

            if (selected.Count > 3)
            {
                int index = 0;
                while ((Color)selected[index].GetContents()[0] != colors.Peek())
                {
                    index++;
                }
                var interfaceValues = selected[index] as Row;
                SetRowSelection(interfaceValues.guid, !interfaceValues.isSelected);
                UpdateRowField(interfaceValues.guid, " ", Color.clear);
            }

            Color newcolor = colors.Dequeue();
            UpdateRowField(guid, " ", newcolor);
            colors.Enqueue(newcolor);
        }
        trendgraph.Compute();
    }

    public void Remove()
    {
        RemoveSelected();
        trendgraph.Compute();
    }
}
