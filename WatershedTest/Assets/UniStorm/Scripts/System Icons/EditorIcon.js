@ExecuteInEditMode


function OnDrawGizmos () {
	 	//Gizmos.DrawGUITexture(Rect(this.transform.position.x, this.transform.position.y, 20, 20), iconTexture);
	 	//Gizmos.DrawIcon(this.transform.position, "Resources/EditorIcons/TimeEventIcon.png");
	 	
	 	Gizmos.color = Color (1,1,1,.5);
		Gizmos.DrawCube (transform.position, Vector3 (2,2,2));
	}