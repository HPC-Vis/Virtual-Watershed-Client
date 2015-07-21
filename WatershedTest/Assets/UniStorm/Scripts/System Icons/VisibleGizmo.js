@ExecuteInEditMode


function Update () {


}

function OnDrawGizmos () {
		// Draw a semitransparent blue cube at the transforms position
		Gizmos.color = Color (1,0,0,.5);
		Gizmos.DrawCube (transform.position, Vector3 (transform.localScale.x,transform.localScale.y,transform.localScale.z));
		 
	}
	
	 	
	 
