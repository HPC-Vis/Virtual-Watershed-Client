using UnityEngine;
using System.Collections;
using VTL.ListView;
using System;
using System.Collections.Generic;

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

    // Use this for initialization
    void Start () {
        // Setup the colors
        colors.Enqueue(new GraphColor(Color.blue));
        colors.Enqueue(new GraphColor(Color.green));
        colors.Enqueue(new GraphColor(Color.white));
    }
	
	// Update is called once per frame
	void Update () {
        // colors.Peek().color, 
	}

    internal void AddRow(object[] v)
    {
        GraphColor temp = colors.Dequeue();
        object[] newObj = { temp.color, v[0], v[1], v[2], v[3] };
        
        selectedList.AddRow(newObj);
    }

    internal List<object[]> GetSelectedRowContent()
    {
        throw new NotImplementedException();
    }
}
