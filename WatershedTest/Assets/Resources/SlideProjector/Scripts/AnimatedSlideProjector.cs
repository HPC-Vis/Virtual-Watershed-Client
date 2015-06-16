using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using VTL.SimTimeControls;

public class AnimatedSlideProjector : MonoBehaviour
{
    public GameObject timeSliderGameObject;
    private TimeSlider timeSlider;
    private Slider slider;

    public string resSlidesFmtString;

    Projector projector;

    private long currentSlide;

    public void set_texture(long step)
    {
        Texture2D texture;
        texture = Resources.Load<Texture2D>(string.Format(resSlidesFmtString, step));
        // Texture2D is nullable so if it fails it is essentially an empty texture

        projector.material.SetTexture(step % 2 == 0 ? "_ShadowTex2" : "_ShadowTex", texture);
    }

    public float get_opacity()
    {
        return projector.material.GetFloat("_Opacity");
    }

    public void set_opacity(float opacity)
    {
        if (opacity < 0f)
            opacity = 0f;
        else if (opacity > 1f)
            opacity = 1f;

        projector.material.SetFloat("_Opacity", opacity);
    }

    public float get_blend()
    {
        return projector.material.GetFloat("_Blend");
    }

    public void set_blend(float blend)
    {
        if (blend < 0f)
            blend = 0f;
        else if (blend > 1f)
            blend = 1f;

        projector.material.SetFloat("_Blend", blend);
    }

    // Use this for initialization
    void Start()
    {
        timeSlider = timeSliderGameObject.GetComponent<TimeSlider>();
        slider = timeSliderGameObject.GetComponent<Slider>();

        projector = gameObject.GetComponent<Projector>();
        currentSlide = timeSlider.SimStep - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSlide != timeSlider.SimStep)
        {
            currentSlide = timeSlider.SimStep;
            set_texture(currentSlide);
        }
        float blend = slider.value - currentSlide;
        blend = currentSlide % 2 == 0 ? 1 - blend : blend;
        set_blend((1 + Mathf.Cos(blend * Mathf.PI))/2);

    }
}
