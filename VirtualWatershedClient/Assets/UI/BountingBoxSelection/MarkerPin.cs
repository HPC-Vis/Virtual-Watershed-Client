using UnityEngine;
using System.Collections;

public class MarkerPin : MonoBehaviour {

    public BoundingBoxSelection box;
    // Use this for initialization
    void Start() {
      
    }
    public void addPin(Vector3 pos)
    {
        GameObject Pin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Pin.AddComponent<Rigidbody>();
        //box = GetComponent<BoundingBoxSelection>();
        //  Pin.transform.position = new Vector3(box.startPos.x, box.startPos.y, box.startPos.z);
        Pin.transform.position = new Vector3(pos.x, pos.y, pos.z);
        //Pin.transform.position = new Vector3(10, 230, 12);
        Pin.transform.localScale = new Vector3(0.1f, 2.0f, 0.1f);
        return;
        
    }

    // Update is called once per frame
    void Update () {
	
	}
}
