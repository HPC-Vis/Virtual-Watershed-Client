using UnityEngine;
using System.Collections;

public class maintainAcrossScenes : MonoBehaviour {

    public GameObject[] maintainables;

	// Use this for initialization
	void Start () {
        foreach (var obj in maintainables)
        {
            DontDestroyOnLoad(obj);
        }
	}
	

}
