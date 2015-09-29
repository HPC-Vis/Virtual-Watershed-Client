using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The ListView is contained in its own name space
using VTL.ListView;

public class SelectedDatabaseController : MonoBehaviour
{
    public ListViewManager listViewManager;
    private ListViewManager thisListViewManager;

    // Use this for initialization
    void Start()
    {
        thisListViewManager = GetComponent<ListViewManager>();
        ListViewManager.SelectionChangeEvent += OnSelectionChange;
    }

    // Update is called once per frame
    public void OnSelectionChange()
    {
        // out of sync issues.. patch
        try
        {
            IEnumerator ienObj = listViewManager.Selected();
            var inListView = new List<System.Guid>();

            while (ienObj.MoveNext())
            {
                var guid = (System.Guid)ienObj.Current;
                inListView.Add(guid);

                if (!thisListViewManager.listData.ContainsKey(guid))
                {
                    thisListViewManager.AddRow(new object[] { listViewManager.listData[guid]["Name"] }, guid);
                }

            }


            var listData = thisListViewManager.listData.Keys;
            List<System.Guid> GUIDS = new List<System.Guid>();
            foreach (var item in listData)
                if (!inListView.Contains(item))
                    GUIDS.Add(item);
            foreach(var item in GUIDS)
             thisListViewManager.Remove(item);
        }
        catch (System.Exception e)
        {
            //Debug.LogError(e.Message);
            //Debug.LogError(e.StackTrace);
        }
    }

    public void Clear()
    {
        thisListViewManager.Clear();
    }
}
