using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class DownloadManager : MonoBehaviour {

    List<string> Downloaded = new List<string>();
    public Text DownloadedText;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
        // Check to quit the application
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (ModelRunManager.Total > 0)
        {
            // Update the Percent Downloaded
            DownloadedText.text = ((float)(ModelRunManager.Counter) / (float)(ModelRunManager.Total)).ToString("p");

            // Clear the textbox if done downloading 
            if ((float)(ModelRunManager.Counter) / (float)(ModelRunManager.Total) == 100.0f)
            {
                DownloadedText.text = "";
            }
        }
		
	}

    /// <summary>
    /// Adds a modelRun to the list of successfully downloaded modelRuns
    /// </summary>
    /// <param name="UUID">The UUID of the downloaded ModelRun</param>
    public void AddModelRun(string UUID)
    {
		if (!Downloaded.Contains (UUID)) 
		{
			Downloaded.Add (UUID);
		}
    }

    /// <summary>
    /// Gets the list of downloaded ModelRuns
    /// </summary>
    /// <returns>A list of downloaded ModelRuns</returns>
    public List<string> GetModelRuns()
    {
        Logger.WriteLine("<color=green>Number of model runs: " + Downloaded.Count + ".</color>");
        return Downloaded;
    }
    
}
