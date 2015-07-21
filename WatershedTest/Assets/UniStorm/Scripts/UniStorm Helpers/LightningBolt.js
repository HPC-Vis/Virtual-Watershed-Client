var lightningBolt1 : Material;
var lightningBolt2 : Material;
var lightningBolt3 : Material;

private var timer : float;

function Start () {

}

function Update () {
	
	timer += Time.deltaTime;
	
	if (timer >= 0.12)
	{
		GetComponent.<Renderer>().material = lightningBolt2;
	}
	
	if (timer >= 0.24)
	{
		GetComponent.<Renderer>().material = lightningBolt3;
	}
	
	if (timer >= 0.36)
	{
		GetComponent.<Renderer>().material = lightningBolt2;
	}
	
	if (timer >= 0.48)
	{
		GetComponent.<Renderer>().material = lightningBolt1;
	}
	
	if (timer >= 0.60)
	{
		Destroy(gameObject);
	}

}