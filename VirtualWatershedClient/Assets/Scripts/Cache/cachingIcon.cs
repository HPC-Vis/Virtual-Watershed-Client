using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class cachingIcon : MonoBehaviour {

    public Text CachingText;
    int numDots;

    void Start()
    {
        CachingText.text = "";
    }
	
	// Update is called once per frame
	void Update () {
        if (GlobalConfig.caching == true)
        {
            CachingText.text = "Caching Data ";
            for (int i = 0; i < numDots/20; i++)
            {
                CachingText.text += ". ";
            }
            numDots = (numDots + 1) % 120;
        }
        else
        {
            CachingText.text = "";
        }
	}
}
