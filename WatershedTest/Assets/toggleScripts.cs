using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class toggleScripts : MonoBehaviour
{

    public bool noClip;
    public bool fps;
    public bool rts;

    public GameObject player;
    Quaternion rotation;

    // Use this for initialization
    void Start()
    {
        noClip = false;
        fps = true;
        rts = false;
    }


    void Update()
    {
        if (Input.GetKeyDown("n"))
        {
            swapToNoClip();
        }
        else if (Input.GetKeyDown("f"))
        {
            swapToFPS();
        }
    }

    public void swapToNoClip()
    {
        rotation = player.transform.rotation;
        player.GetComponent<NoClipFirstPersonController>().enabled = true;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<FPSInputController>().enabled = false;
        player.GetComponent<CharacterMotor>().enabled = false;
        player.transform.rotation = rotation;
    }

    public void swapToFPS()
    {
        rotation = player.transform.rotation;
        player.GetComponent<NoClipFirstPersonController>().enabled = false;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<FPSInputController>().enabled = true;
        player.GetComponent<CharacterMotor>().enabled = true;
        player.transform.rotation = rotation;
    }

    public void toggleRTS()
    {
        player.GetComponent<Camera>().enabled = !(player.GetComponent<Camera>().enabled);
    }
   
}