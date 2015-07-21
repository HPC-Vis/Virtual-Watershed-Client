using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Variable 
{
    // Members

    // Global Min and Max variables for this variable
    public float Min, Max;
    public string Name;

    // These variables represent the start and end date range.
    DateTime Start, End;
    public enum TYPE { Temporal, NonTemporal };
    TYPE type;
    List<DataRecord> Records = new List<DataRecord>();

    public List<DataRecord> Data
    {
        get 
        {
            return Records;
        }
    }

    public Variable(string name="name",TYPE typef=TYPE.NonTemporal)
    {
        Name = name;
        type = typef;
    }

    public void Insert(DataRecord record)
    {
        Records.Add(record);
    }
    
    public bool IsTemporal()
    {
    	return type == TYPE.Temporal;
    }
    
    public void SetType(TYPE t)
    {
    	type = t;
    }
}
