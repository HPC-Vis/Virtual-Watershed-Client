//var positionX : Transform;
//var positionZ : Transform;

var timer : float;

function Start () {

}

function Update () {

	timer += Time.deltaTime;
	
	if (timer >= 2)
	{
		transform.localPosition = Vector3(Random.Range(-200,200), 45, Random.Range(-200,200));
		timer = 0;
		
	}

}