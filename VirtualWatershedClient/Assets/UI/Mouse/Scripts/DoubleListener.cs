using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;
public delegate void DoTheDouble(System.Guid abc);
public class DoubleListener : MonoBehaviour 
{
    public ListViewManager LVM;
    List<System.Guid> GUIDSelected = new List<System.Guid>();
    
    //public event DoTheDouble Doubleness;
    public DoTheDouble DoAction;
    bool doubleDetected = false;
    
	// Use this for initialization
    public void Start()
    {
        ListViewManager.SelectionChangeEvent += ChangeDetected;
    }

    public void ChangeDetected()
    {
        doubleDetected = false;
        var Selected = LVM.Selected();
        List<System.Guid> GUIDOn = new List<System.Guid>();
        
        while(Selected.MoveNext())
        {
            var guid = (System.Guid)Selected.Current;
            GUIDOn.Add(guid);
            //if(GUIDSelected.Contains(guid))
            //{
            //    GUIDSelected.Remove(guid);
            //}
        }

        foreach(var i in GUIDSelected)
        {
            if ( LVM.IsSelectedOn(i))
            {
                if (DoAction != null)
                {
                    DoAction(i);
                }
                doubleDetected = true;
            }
        }

        GUIDSelected = GUIDOn;
    }

}
