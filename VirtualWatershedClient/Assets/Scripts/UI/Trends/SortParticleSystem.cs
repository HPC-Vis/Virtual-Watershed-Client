using UnityEngine;
using System.Collections;

public class SortParticleSystem : MonoBehaviour 
{

    public string layerName = "Particles";

	// Use this for initialization
	void Start () 
    {
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = layerName;
	}
}
