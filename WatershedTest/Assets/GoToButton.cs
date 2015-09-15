using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GoToButton : MonoBehaviour {

    public FileBrowse filebrowser;
    public Button button;
    public string SelectedString;
	// Use this for initialization
	void Start () 
    {
        filebrowser = GameObject.Find("FileBrowser").GetComponent<FileBrowse>();
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener( () => SetBrowser(SelectedString));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetBrowser(string Directory)
    {
        if(filebrowser != null)
        {
            filebrowser.SetDirectory(Directory);
        }
    }
}
