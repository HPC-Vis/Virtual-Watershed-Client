using UnityEngine;
using System.Collections;

public class objsetalpha : MonoBehaviour {
   
	// Use this for initialization
	void Start () {
        CanvasRenderer cvs = GetComponent<CanvasRenderer>();
        if (cvs != null)
        {
            cvs.SetAlpha(0);
        }
       
	
	}
	
	
}
