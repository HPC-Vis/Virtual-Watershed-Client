
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ViewableList : MonoBehaviour {

    public ToggleObjects IsRaster;
    public Dropdown ViewableDropDown;
    public InputField xField, yField, zField;
    public Toggle showHide;

    bool ApplyToggle = false;
    // 
    bool IsRasterState = true;
    
	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update() {
        if (ApplyToggle )
        {
            IsRaster.ToggleToState(IsRasterState);
            //IsRaster.toggleObjects();
            ApplyToggle = false;            
        }
	}

    public void SetScale(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.LogError("Scale NOW");
        }
    }

    public void TranslateViewable(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            float x = 0,y=0,z =0;
            if(xField.text != null)
            {
                x = float.Parse(xField.text);
            }
            if (yField.text != null)
            {
                y = float.Parse(yField.text);
            }
            if (zField.text != null)
            {
                z = float.Parse(zField.text);
            }
            Vector3 offset = new Vector3(x, y, z);

            ModelRunManager.sessionData.MoveObject(ViewableDropDown.options[ViewableDropDown.value].text, offset);

            Debug.LogError("Translate NOW: " + offset);
        }

    }

    public void TransformData(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.LogError("Transform NOW");
        }
    }
    
    public void ProjectionChange(int option)
    {

    }

    public void Test(string str)
    {
        Debug.LogError(str);
    }
    public void SetViewableData(int Option)
    {
        Debug.LogError(Option);
        if(Option == 0 || ViewableDropDown.options[Option].text == "")
        {
            return;
        }

        try
        {
            IsRasterState = !ModelRunManager.sessionData.GetSessionObject(ViewableDropDown.options[Option].text).IsRaster;
            showHide.isOn = ModelRunManager.sessionData.GetSessionObject(ViewableDropDown.options[Option].text).gameObject.activeSelf;
            ApplyToggle = true;
        }
        catch(Exception e)
        {
            try
            {
                IsRasterState = true;
                ApplyToggle = true;
                ActiveData.ChangeActive(ViewableDropDown.options[Option].text);
            }
            catch(Exception e1)
            {
                Debug.LogError("There was a problem getting the data for: " + ViewableDropDown.options[Option].text);
            }
        }

        
        /*if (ModelRunManager.sessionData.GetSessionObject(ViewableDropDown.options[Option].text).IsRaster && !IsRasterState)
        {
            ApplyToggle = true;
            IsRasterState = false;
        }
        else if (IsRasterState)
        {
            ApplyToggle = true;
            IsRasterState = true;
        }*/
    }

    // A function meant for hiding and displaying datasets
    public void EnableSelected()
    {
        if (ViewableDropDown.value > 0)
        {
            var go = ModelRunManager.sessionData.GetSessionObject(ViewableDropDown.options[ViewableDropDown.value].text);
            if (go != null)
            {
                go.gameObject.SetActive(!go.gameObject.activeSelf);
            }
        }
    }
}
