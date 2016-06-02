using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;

namespace VTL.ListView
{
    internal interface Row
    {

        void Initialize(object[] fieldData, Guid guid);
        string StringifyObject(object obj, string formatString, DataType dataType);
        void OnPointerEnter(BaseEventData d);
        void UpdateSelectionAppearance();
        void SetFields(object[] fieldData, Guid guid, bool selected);
        void SetFields(Dictionary<string, object> rowData, Guid guid, bool selected);
        void OnSelectionEvent();
        object[] GetContents();
        bool isSelected { get; set; }
        bool selectedOn { get; set; }
        Guid guid { get; set; }
    }
}
