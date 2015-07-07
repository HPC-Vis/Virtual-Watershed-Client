using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class rangeSelector : MonoBehaviour {

    public List<GameObject> RangeBoxes = new List<GameObject>();
    bool updateHeight = false;
    int selected = -1;
    bool enabled = false;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (updateHeight)
        {
            float height = 0;

            foreach (Transform child in gameObject.transform)
            {
                height += child.GetComponent<RectTransform>().sizeDelta.y;
            }

            height += 12.5f;
            //gameObject.GetComponent<RectTransform>()
            Debug.LogError(height);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, height);
            updateHeight = false;
        }
        if (!enabled)
        {
            AddRanges(4);
            enabled = true;
        }
	}

    public void SetSelected(int index)
    {
        selected = index;
        Debug.LogError("selected: " + selected);
    }

    public void AddRanges(int numRanges)
    {
        for (int i = 0; i < numRanges; i++)
        {
            Debug.LogError(i);
            var go = Resources.Load<GameObject>("UI/RangeRow");
            //go.activeSelf()
            go = GameObject.Instantiate(go);
            go.transform.SetParent(gameObject.transform);
            RangeBoxes.Add(go);
        }
        

        gameObject.transform.GetChild(0).SetAsLastSibling();
        updateHeight = true;
    }
}
