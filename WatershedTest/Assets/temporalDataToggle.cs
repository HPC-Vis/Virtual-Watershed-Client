using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class temporalDataToggle : MonoBehaviour {
		
	public GameObject temporal;
	public Button temporalButton;
	public Button nontemporalButton;
	public GameObject nontemporal;
	public Color temp;
	public Color nonTemp;
		
	// Use this for initialization
	void Start()
	{
		temporal.SetActive (true);
		nontemporal.SetActive (false);

		temp = temporalButton.image.color;
		nonTemp = nontemporalButton.image.color;

		temporalButton.image.color = new Color(temp.r, temp.g, temp.b, 255);
		nontemporalButton.image.color = new Color(nonTemp.r, nonTemp.g, nonTemp.b, 50);
	}
	
	public void toggleNonTemporal()
	{
		temporal.SetActive (false);
		nontemporal.SetActive (!nontemporal.activeSelf);

		temporalButton.image.color = new Color(temp.r, temp.g, temp.b, 50);
		nontemporalButton.image.color = new Color(nonTemp.r, nonTemp.g, nonTemp.b, 255);
	}

	public void toggleTemporal()
	{
		nontemporal.SetActive (false);
		temporal.SetActive (!temporal.activeSelf);

		temporalButton.image.color = new Color(temp.r, temp.g, temp.b, 255);
		nontemporalButton.image.color = new Color(nonTemp.r, nonTemp.g, nonTemp.b, 50);
	}
}