using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class imageSpin : MonoBehaviour {

    public static float progress;
    public GameObject obj;
    Image loadingImage;

    void Start()
    {
        progress = 0;
        loadingImage = obj.GetComponent<Image>();
    }

    void Update()
    {
        if (GlobalConfig.loading)
        {
            obj.GetComponent<Mask>().showMaskGraphic = true;
            spin();
        }
        else
        {
            if (loadingImage.fillAmount == 1)
            {
                obj.GetComponent<Mask>().showMaskGraphic = false;
            }
            else
            {
                spin();
            }
        }
    }

    public void spin()
    {
        if (loadingImage.fillAmount < 1)
        {
            loadingImage.fillAmount += 0.01f;
        }
        else
        {
            loadingImage.fillAmount = 0.01f;
        }
    }
}
