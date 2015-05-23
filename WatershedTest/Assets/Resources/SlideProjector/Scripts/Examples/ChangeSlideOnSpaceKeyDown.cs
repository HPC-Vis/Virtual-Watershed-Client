/*
 * Author: Roger Lew (rogerlew@vandals.uidaho.edu || rogerlew@gmail.com)
 * Date: 2/5/2015
 * License: Public Domain
 * 
 * This illustrates how the slides can be dynamically changed by setting
 * the "_ShadowTex" texture of the projector's material.
 * 
 * The opacity of the shader can specified by setting "_Opacity" on the
 * projector's material. The opacity should be between 0 and 1.
 * 
 */


using UnityEngine;
using System.Collections;

public class ChangeSlideOnSpaceKeyDown : MonoBehaviour
{
    private Projector projector;

    private Texture2D[] textures;
    private int current_tex = 0;

    public void set_texture(Texture2D texture)
    {
        projector.material.SetTexture("_ShadowTex", texture);
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

    void Start()
    {
        projector = gameObject.GetComponent<Projector>();
        textures = new Texture2D[] { Resources.Load<Texture2D>("slope_grad_a"),
                                     Resources.Load<Texture2D>("east_grad_a") };
    }

	// Update is called once per frame
	void Update ()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // east_grad_a isn't included in the prefab
            if (textures[0] == null || textures[1] == null)
                return;

            current_tex += 1;
            current_tex %= 2;
            set_texture(textures[current_tex]);
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            float opacity = get_opacity();
            opacity += 0.1f;
            set_opacity(opacity);
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            float opacity = get_opacity();
            opacity -= 0.1f;
            set_opacity(opacity);
        }

	}
}
