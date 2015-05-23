using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TggleObjects : MonoBehaviour {

    public bool guiOn = true;
    public GameObject[] togglables;
    public GameObject firstPersonController;
    public GameObject ghostController;

	// Use this for initialization
    void Start()
    {
        guiOn = true;
        ghostController = GameObject.Find("NoClipFirstPersonController");
        firstPersonController = GameObject.Find("First Person Controller");
	}

    public void toggleObjects()
    {
        for (int i = 0; i < togglables.Length; i++)
        {

            if (guiOn)
            {
                togglables[i].SetActive(false);
                guiOn = false;
            }
            else
            {
                togglables[i].SetActive(true);
                guiOn = true;
            }
        }

    }
}
