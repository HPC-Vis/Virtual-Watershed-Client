using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class imageSpin : MonoBehaviour {

    public GameObject obj;
    Image loadingImage;
    float initial, dt;
    bool increase;

    void Start()
    {
        loadingImage = obj.GetComponent<Image>();
        initial = Time.time;
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
            obj.GetComponent<Mask>().showMaskGraphic = false;
        }
    }

    public void spin()
    {
        dt = Time.time - initial;
        loadingImage.color = new Color(Mathf.Abs(Mathf.Sin(dt)+0.3f), Mathf.Abs(Mathf.Cos(dt)+.6f), Mathf.Abs(Mathf.Sin(dt)), 1);

        if (loadingImage.fillAmount == 1.0f || loadingImage.fillAmount == 0.0f)
        {
            increase = !increase;
        }

        if (increase)
        {
            loadingImage.fillClockwise = true;
            loadingImage.fillAmount += 0.01f;
        }
        else
        {
            loadingImage.fillClockwise = false;
            loadingImage.fillAmount -= 0.01f;
        }
    }
}
