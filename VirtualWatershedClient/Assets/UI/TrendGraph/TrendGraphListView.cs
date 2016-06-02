using UnityEngine;
using System.Collections;
using VTL.ListView;
using System;
using System.Collections.Generic;
using VTL.TrendGraph;

public class TrendGraphListView : MonoBehaviour {

    public struct GraphColor
    {
        public Color color;
        public bool inUse;

        public GraphColor(Color c)
        {
            color = c;
            inUse = false;
        }
    }

    // The selected graph points
    public ListViewManager selectedList;
    public Queue<GraphColor> colors = new Queue<GraphColor>();
    public TrendGraphController trendgraph;

    // Use this for initialization
    void Start () {
        // Setup the colors
        colors.Enqueue(new GraphColor(Color.blue));
        colors.Enqueue(new GraphColor(Color.green));
        colors.Enqueue(new GraphColor(Color.white));

        // Set the selection event
        ListViewManager.TrendgraphSelectionChangeEvent += SelectionEvent;
    }
	
	// Update is called once per frame
	void Update () {
        // colors.Peek().color, 
	}

    internal void AddRow(object[] v)
    {
        List<TrendGraphRow> selected = selectedList.GetSelectedTrendGraphRows();
        if(selected.Count == 3)
        {
            var interfaceValues = selected[0] as Row;
            selectedList.SetRowSelection(interfaceValues.guid, !interfaceValues.isSelected);            
            selectedList.UpdateRowField(interfaceValues.guid, " ", Color.clear);
        }

        object[] newObj = { colors.Dequeue().color, v[0], v[1], v[2], v[3] };
        colors.Enqueue(new GraphColor((Color)newObj[0]));

        Guid id = selectedList.AddRow(newObj);
        selectedList.SetRowSelection(id, true);
    }

    internal List<object[]> GetSelectedRowContent()
    {
        return selectedList.GetSelectedRowContent(); 
    }

    public void SelectionEvent(Guid guid)
    {
        Color currentRowColor = (Color)selectedList.GetRowContent(guid)[0];
        if (currentRowColor != Color.clear)
        {            
            int colorSize = colors.Count;
            colors.Enqueue(new GraphColor(currentRowColor));
            for(int i = 0; i < colorSize; i++)
            {
                Color temp = colors.Dequeue().color;
                if(temp != currentRowColor)
                {
                    colors.Enqueue(new GraphColor(temp));
                }                
            }
            selectedList.UpdateRowField(guid, " ", Color.clear);
        }
        else
        {
            List<TrendGraphRow> selected = selectedList.GetSelectedTrendGraphRows();

            if (selected.Count > 3)
            {
                int index = 0;
                while ((Color)selected[index].GetContents()[0] != colors.Peek().color)
                {
                    index++;
                }
                var interfaceValues = selected[index] as Row;
                selectedList.SetRowSelection(interfaceValues.guid, !interfaceValues.isSelected);
                selectedList.UpdateRowField(interfaceValues.guid, " ", Color.clear);
            }

            Color newcolor = colors.Dequeue().color;
            selectedList.UpdateRowField(guid, " ", newcolor);
            colors.Enqueue(new GraphColor(newcolor));
        }
        trendgraph.Compute();
    }

    public void Remove()
    {
        selectedList.RemoveSelected();
        trendgraph.Compute();
    }
}
