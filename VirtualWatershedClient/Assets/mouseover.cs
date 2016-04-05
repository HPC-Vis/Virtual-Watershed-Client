using UnityEngine;
using System.Collections;

public class mouseover : MonoBehaviour {

    public GameObject text;

    void OnMouseOver()
    {
        text.SetActive(true);

    }
    void OnMouseExit()
    {
        text.SetActive(true);

    }

}
