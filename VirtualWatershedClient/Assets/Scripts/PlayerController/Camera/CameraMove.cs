using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    public void OnGUI() {
     if (Event.current.type==EventType.KeyDown) {
         if (Event.current.keyCode.ToString().Equals( "LeftBracket"))
         {
             RaycastHit hit;
             bool status = Physics.Raycast(transform.localPosition, transform.TransformDirection(Vector3.down), out hit);

             if (status)
             {
                 if (hit.distance > 1.6f)
                     transform.Translate(0, -1, 0);
             }
         }
         else if (Event.current.keyCode.ToString().Equals("RightBracket"))
         {
             transform.Translate(0, 1, 0);
         }
     }
 }
}
