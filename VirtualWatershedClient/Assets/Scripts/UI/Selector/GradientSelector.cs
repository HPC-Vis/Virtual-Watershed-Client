using UnityEngine;
using System.Collections;

public class GradientSelector : MonoBehaviour {

    public string projectorTag;
	private Projector projectorComponent;

	// Use this for initialization
	void Start () {

        GameObject gameObj = GameObject.FindGameObjectWithTag(projectorTag);
        projectorComponent = gameObj.GetComponent<Projector>();
	}


    public void OnSelectionChange(Texture2D newTexture)
    {
		projectorComponent.material.SetTexture("_ShadowTex", newTexture);
    }
}
