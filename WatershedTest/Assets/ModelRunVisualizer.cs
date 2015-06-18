using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;

public class ModelRunVisualizer : MonoBehaviour {
    
    public DownloadManager downloadManager;
    public ListViewManager listView;

	//Adding the variable reference class here for now..
	VariableReference vr = new VariableReference();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
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

            foreach (var variable in variables)
            {
				if(variables.Count >= 1 && !MR.ModelName.ToLower().Contains("reference"))//MR.IsTemporal.ContainsKey(variable))
				{
					string desription = vr.GetDescription(variable);
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
