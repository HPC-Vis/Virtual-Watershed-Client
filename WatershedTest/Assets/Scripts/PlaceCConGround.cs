using UnityEngine;
using System.Collections;

public class PlaceCConGround : MonoBehaviour 
{

	void Start () {
		RaycastHit hit;
		bool status = Physics.Raycast(transform.localPosition, transform.TransformDirection(Vector3.down), out hit);

		if (status)
		{

			Vector3 newPos = transform.position;
			newPos.y -= hit.distance - 1;
			transform.position = newPos;
		}
	}
}
