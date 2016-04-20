/*
 * Copyright (c) 2015, Roger Lew (rogerlew.gmail.com)
 * Date: 4/25/2015
 * License: BSD (3-clause license)
 * 
 * The project described was supported by NSF award number IIA-1301792
 * from the NSF Idaho EPSCoR Program and by the National Science Foundation.
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VTL.ListView
{
    public class RowElement : MonoBehaviour
    {
        void Start()
        {
            // Need to push click event to the parent Row component
            UnityAction listener;
            UnityAction<BaseEventData> call;
            if(transform.parent.gameObject.GetComponent<Row>() != null)
            {
                listener = transform.parent.gameObject.GetComponent<Row>().OnSelectionEvent;
                call = transform.parent.gameObject.GetComponent<Row>().OnPointerEnter;
            }
            else if(transform.parent.gameObject.GetComponent<TrendGraphRow>() != null)
            {
                listener = transform.parent.gameObject.GetComponent<TrendGraphRow>().OnSelectionEvent;
                call = transform.parent.gameObject.GetComponent<TrendGraphRow>().OnPointerEnter;
            }
            else
            {
                listener = null;
                call = null;
                Debug.LogError("Problem loading the proper component.");
            }
            gameObject.GetComponent<Button>()
                      .onClick
                      .AddListener(listener);
            var ev = gameObject.AddComponent<EventTrigger>();
            //ev.triggers.Add()
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener(call);
            ev.triggers.Add(entry);
        }

    }
}
