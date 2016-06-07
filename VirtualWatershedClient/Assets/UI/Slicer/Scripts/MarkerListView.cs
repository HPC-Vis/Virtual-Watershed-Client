using UnityEngine;
using System.Collections;
using VTL.ListView;
using System;
using System.Collections.Generic;

public class MarkerListView : ListViewManagerParent
{
    // The selected graph points
    public Marker markerController;

    // For use with the marker selection
    public delegate void MarkerSelectionChangeAction(Guid guid);
    public static event MarkerSelectionChangeAction MarkerSelectionChangeEvent;

    // Use this for initialization
    void Start()
    {
        // Set the selection event
        MarkerSelectionChangeEvent += SelectionEvent;
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

    /// <summary>
    /// Gets the marker row of the selected object. This class should only allow one selected.
    /// </summary>
    /// <returns>A MarkerRow of the same guid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when there is no row selected.</exception>
    public MarkerRow GetSelectedMarkerRow()
    {
        foreach (var item in rows)
        {
            MarkerRow ROW = item.Value.GetComponent<MarkerRow>();
            var interfaceValues = ROW as Row;
            if (interfaceValues.isSelected)
            {
                return ROW;
            }
        }
        throw new ArgumentNullException("No selected item.");
    }

    /// <summary>
    /// Gets the ow with a given guid.
    /// </summary>
    /// <param name="guid">The GUID to search for.</param>
    /// <returns>A MarkerRow of the same guid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when there is no guid of that name found.</exception>
    public MarkerRow GetRowAtGUID(Guid guid)
    {
        foreach (var item in rows)
        {
            MarkerRow ROW = item.Value.GetComponent<MarkerRow>();
            var interfaceValues = ROW as Row;
            if (interfaceValues.guid == guid)
            {
                return ROW;
            }
        }
        throw new ArgumentNullException("No item with that GUID.");
    }

    public new void OnSelectionEvent(Guid guid, int index)
    {
        // Run the super
        base.OnSelectionEvent(guid, index);

        // Run the action on this item
        if (MarkerSelectionChangeEvent != null)
        {
            MarkerSelectionChangeEvent(guid);
        }
    }

    public void SelectionEvent(Guid guid)
    {
        MarkerRow interaced = GetRowAtGUID(guid);
        Row interfaceValue = interaced as Row;

        if(interfaceValue.isSelected)
        {
            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;
            interaced.GetPositions(out start, out end);
            markerController.SetMarkers(start, end);
        }
        else
        {
            markerController.Clear();
        }        
    }

    public void Remove()
    {
        RemoveSelected();
    }
}
