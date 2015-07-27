#pragma strict

var uniStormSystem : GameObject;
var zoneWeather : int;
var shieldArea : float = 5;

function Start () {

	uniStormSystem = GameObject.Find("UniStormSystemEditor");

}

function Update () {

}

function OnTriggerEnter (other : Collider)
{
	if (other.tag == "Player")
	{
		uniStormSystem.GetComponent(UniStormWeatherSystem_JS).weatherForecaster = zoneWeather;
	}
}