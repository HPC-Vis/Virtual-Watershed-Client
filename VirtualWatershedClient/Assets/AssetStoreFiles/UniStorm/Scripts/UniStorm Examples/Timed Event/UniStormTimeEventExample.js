@ExecuteInEditMode

private var uniStormSystem : GameObject;
var hourOfEvent : int;
var eventTestBool : boolean;

function Awake () {

	//Find the UniStorm Weather System Editor, this must match the UniStorm Editor name
	uniStormSystem = GameObject.Find("UniStormSystemEditor");

}

function Start () {

	if (uniStormSystem == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Null Reference:</color> You must have the UniStorm Editor in your scene and named 'UniStormSystemEditor'. Make sure your C# UniStorm Editor has this name. ");
		}

}

function Update () {

 
	if (uniStormSystem != null)
	{
		if (uniStormSystem.GetComponent(UniStormWeatherSystem_JS).hourCounter >= 11 && eventTestBool == false)
		{

     		//It's time for the an event based on the UniStorm time as long as there isn't an error finding the UniStorm Editor
			Debug.Log("This is a printed test event. If you see this your even was successful.");
			eventTestBool = true;

		}
	}

}

