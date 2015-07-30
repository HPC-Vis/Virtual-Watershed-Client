using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class toggleScripts : MonoBehaviour
{

    public bool noClip;
    public bool faster;
    public bool rts;

    public GameObject player;
    Quaternion rotation;

    // Use this for initialization
    void Start()
    {
        noClip = false;
        faster = false;
        //player.GetComponent<MouseLook>().enabled = false;
    }


    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            if (noClip)
            {
                swapToFPS();
            }
            else
            {
                swapToNoClip();
            }
            noClip = !noClip;
        }
        else if (Input.GetKeyDown("f"))
        {
            if (faster)
            {
                slowDown();
            }
            else
            {
                speedUp();
            }
            faster = !faster;
            
        }
        //if (Input.GetMouseButtonDown(1))
        //{
        //    player.GetComponent<MouseLook>().enabled = true;
        //}
        //else if(Input.GetMouseButtonUp(1))
        //{
        //    player.GetComponent<MouseLook>().enabled = false;

        //}
    }

    void slowDown()
    {
        player.GetComponent<NoClipFirstPersonController>().movementForwardMultiplier = 100;
        player.GetComponent<NoClipFirstPersonController>().movementSideMultiplier = 100;
        player.GetComponent<CharacterMotor>().movement.maxBackwardsSpeed = 20;
        player.GetComponent<CharacterMotor>().movement.maxForwardSpeed = 20;
        player.GetComponent<CharacterMotor>().movement.maxSidewaysSpeed = 20;

    }

    void speedUp()
    {
        player.GetComponent<NoClipFirstPersonController>().movementForwardMultiplier = 300;
        player.GetComponent<NoClipFirstPersonController>().movementSideMultiplier = 300;
        player.GetComponent<CharacterMotor>().movement.maxBackwardsSpeed = 100;
        player.GetComponent<CharacterMotor>().movement.maxForwardSpeed = 100;
        player.GetComponent<CharacterMotor>().movement.maxSidewaysSpeed = 100;
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