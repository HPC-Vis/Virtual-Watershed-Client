using UnityEngine;
using System;
using System.IO;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * y spacing
 * -215
 * -353
 * -491
 * -629
*/

public class LayerRenderer : MonoBehaviour {

    private const float pi = (float)Math.PI;
    public Sprite[] sprites;
    private Dictionary<int, Sprite> spritesDict;

    public string TimeSliderTag = "TimeSlider";
    private Slider slider;

    public int modvalue;
    public string layerdir;
    public int siblingIndex = 1;

    private int currentstep;

    private Image image;

    private Sprite emptySprite;


	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();

        emptySprite = Resources.Load<Sprite>("emptySprite");
        sprites = Resources.LoadAll<Sprite>(layerdir);

        spritesDict = new Dictionary<int, Sprite>();

        for (int i = 0; i < sprites.Length; i++ )
        {
            int step = parseStep(sprites[i].name);
            spritesDict.Add(step, sprites[i]);
        }

        slider = GameObject.FindGameObjectWithTag(TimeSliderTag).GetComponent<Slider>();

        currentstep = modvalue;

        if (spritesDict.ContainsKey(currentstep))
            image.sprite = spritesDict[currentstep];
        else
            image.sprite = emptySprite;

        Debug.Log(spritesDict.Count);
	}

    int parseStep(string name)
    {
        string[] split = name.Split('.');

        foreach (string s in split)
        {
            try
            {
                int step = System.Convert.ToInt32(s);
                return step;
            }
            catch
            { }
        }
        return 0;
    }

	// Update is called once per frame
    void Update()
    {
        float value = slider.value;
        float alpha = 1f - Math.Abs((2f - (value - (float)currentstep)) * pi);


        if ((alpha > 0) & (alpha <= 1f))
        {
            image.color = new Color(1f, 1f, 1f, alpha);
        }
        else
        {
            int floorval = (int)value;
            if (floorval % 2 == modvalue)
                currentstep = floorval + 1;
            else
                currentstep = floorval;

            if (spritesDict.ContainsKey(currentstep))
                image.sprite = spritesDict[currentstep];
            else
                image.sprite = emptySprite;

            image.transform.SetSiblingIndex(siblingIndex);

        }
	}
}
