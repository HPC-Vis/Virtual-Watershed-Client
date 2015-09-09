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
            gameObject.GetComponent<Button>()
                      .onClick
                      .AddListener(transform.parent
                                            .gameObject
                                            .GetComponent<Row>().OnSelectionEvent);
            var ev = gameObject.AddComponent<EventTrigger>();
            //ev.triggers.Add()
            EventTrigger.Entry entry = new EventTrigger.Entry();
            var call = new UnityAction<BaseEventData>(transform.parent
                                            .gameObject
                                            .GetComponent<Row>().OnPointerEnter);
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener(call);
            ev.triggers.Add(entry);
        }

    }
}
