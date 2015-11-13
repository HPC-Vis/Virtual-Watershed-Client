using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class slideTexture : MonoBehaviour {

    private int textureIndex = 0;
    public Sprite[] sprites;
    public Slider slider;
    public Image testImage;


    void Start()
    {
        testImage.sprite = sprites[0];
    }
   
    public void ChangeTexture()
    {
        textureIndex = (int)slider.value;
        testImage.sprite = sprites[textureIndex];
    }
}
