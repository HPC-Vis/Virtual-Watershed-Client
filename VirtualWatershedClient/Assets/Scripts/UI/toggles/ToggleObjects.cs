using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ToggleObjects : MonoBehaviour
{

    public bool objectsOn;
    public GameObject[] togglables;

    // Use this for initialization
    void Start()
    {
        objectsOn = false;
    }

    public void toggleObjects()
    {
        // Toggle the active state of the objects
        for(int i = 0; i < togglables.Length; i++)
        {
            togglables[i].SetActive(!togglables[i].activeSelf);
        }

        objectsOn = !objectsOn;
    }

    public void ToggleToState(bool stateToggle)
    {
        if(stateToggle != objectsOn)
        {
            toggleObjects();
        }
    }

    public void toggleOn()
    {
        for (int i = 0; i < togglables.Length; i++)
        {
            if(!togglables[i].activeSelf)
            togglables[i].SetActive(true);
        }
    }

    public void toggleOff()
    {
        for (int i = 0; i < togglables.Length; i++)
        {
            if (togglables[i].activeSelf)
                togglables[i].SetActive(false);
        }
    }
}