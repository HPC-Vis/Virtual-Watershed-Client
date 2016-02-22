using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;

public class ModelRunLoader : MonoBehaviour {
    
    public DownloadManager downloadManager;
    public ListViewManager listView;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.LogError("HERE");
            //ModelRunManager.sessionData.SaveSessionData(Utilities.GetFilePath("test.json"));
        }
	}

    void OnEnable()
    {
        //SetFields();
    }

    public void SetFields()
    {
        listView.Clear();

        // Debug.LogError("SET FIELDS");
        foreach (var i in downloadManager.GetModelRuns())
        {
            var MR = ModelRunManager.GetByUUID(i);
            Logger.WriteLine("<color=green>Adding Model Run Variables: " + MR.GetVariables().Count + ".</color>");
            List<string> variables = MR.GetVariables();
			Debug.LogError("VARIABLES COUNT: " + variables.Count);
			
            foreach (var variable in variables)
            {
				if(variables.Count >= 1 && MR.GetVariable(variable).IsTemporal())//MR.IsTemporal.ContainsKey(variable))
				{
                    string desription = VariableReference.GetDescription(variable);
					if(desription == "")
					{
						desription = MR.Description;
					}

                	listView.AddRow(new object[] { MR.ModelName, MR.ModelRunUUID, variable, desription},MR);
				}
            }
            
            //downloadManager.AddModelRun(i);
        }
    }
}
