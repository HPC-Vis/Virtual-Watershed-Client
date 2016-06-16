using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using VTL.ListView;

public class FileListViewManager : MonoBehaviour
{
    public delegate void UseSelected();
    public UseSelected action = null;
    FileBrowse fileBrowser = new FileBrowse();

    // Keeps track of what is currently selected
    //public SelectedDatabaseController selectedDatabaseController;
    public DoubleListener doubleClickListener; // listens for double clicks

    public ListViewManager fileListView;

    public Button LoadSaveButton;

    public void Start()
    {
        fileBrowser.CurrentDirectory = ".";
        //doubleClickListener = gameObject.GetComponent<DoubleListener>();
        doubleClickListener.DoAction = new DoTheDouble(ChangeDirectoryOnClick);
        //selectedDatabaseController.Clear();
        populateFileWindow();
        SetTextToSave();

    }

    public void SetSearchPattern(string [] formats)
    {
        if(formats == null)
        {
            return;
        }

        string searchString = "";
        for(int i = 0; i < formats.Length; i++)
        {
            searchString += formats[i] + "|";
        }
        fileBrowser.SearchString = searchString;
    }

    /// <summary>
    /// Change on double click from the double listener...
    /// </summary>
    /// <param name="guid"></param>
    void ChangeDirectoryOnClick(System.Guid guid)
    {
        var contents = fileListView.GetRowContent(guid);
        if (contents.Length > 0)
        {
            if (contents[2].ToString().ToLower() == "directory")
            {
                fileBrowser.SetDirectory((string)contents[0]);
                populateFileWindow();
                //currentDirectory.text = "Current Directory: " + fileBrowser.CurrentDirectory;
            }
        }
    }

    /// <summary>
    /// Change the directory to the relevant directory.
    /// </summary>
    public void ChangeDirectory()
    {
        var contents = fileListView.GetSelectedRowContent();
        if (contents != null && contents.Count > 0)
        {
            if (contents[0][2].ToString().ToLower() == "directory")
            {
                fileBrowser.SetDirectory((string)contents[0][0]);
                populateFileWindow();
                //currentDirectory.text = "Current Directory: " + fileBrowser.CurrentDirectory;

            }
        }

    }

    /// <summary>
    /// Populate data based on what is acquired from the filebrowse class.
    /// </summary>
    void populateFileWindow()
    {
        var Contents = fileBrowser.GetContents();
        fileListView.Clear();
        DirectoryStruct ds = new DirectoryStruct();
        ds.filename = "..";
        ds.IsDirectory = true;
        Contents.Add(ds);
        foreach (var i in Contents)
        {
            fileListView.AddRow(new object[] { i.filename, i.dateModified, i.IsDirectory ? "Directory" : "File", i.IsDirectory ? "" : i.bytes });
        }
    }

    public void ApplySelection()
    {
        if (action != null)
        {
            action();
        }
    }

    public void SetTextToLoad()
    {
        LoadSaveButton.transform.GetChild(0).GetComponent<Text>().text = "Load";
    }

    public void SetTextToSave()
    {
        LoadSaveButton.transform.GetChild(0).GetComponent<Text>().text = "Save";
    }

}
