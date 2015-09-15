//// Author: Chase Carthen
/// A windows friendly directory browser...
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public struct DirectoryStruct
{
    public string Path;
    public bool IsDirectory;
    public string filename;
    public string bytes;
    public string dateModified;
}
public class FileBrowse : MonoBehaviour {

    public string CurrentDirectory = "";

	// Use this for initialization
	void Start () {
        CurrentDirectory = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetDirectory(string Directory)
    {
        //Debug.LogError("CALLED");
        if(CurrentDirectory == "")
        {
            CurrentDirectory = Directory;
            return;
        }
       
        string PathTo = Path.IsPathRooted(Directory) ? Directory : Path.GetFullPath(CurrentDirectory + @"\" + Directory);
        string Root = Path.GetPathRoot(PathTo);
        Debug.LogError("PATHTO: " + PathTo);
        Debug.LogError("ROOT: " + Root);
        if(Directory == ".." && CurrentDirectory == Root)
        {
            CurrentDirectory = "";
        }
        else
        {
            CurrentDirectory = PathTo;
        }
        
    }

    public List<DirectoryStruct> GetContents()
    {

        Debug.LogError("CURRENT DIRECTORY: " + CurrentDirectory);
        List<DirectoryStruct> Contents = new List<DirectoryStruct>();
        if (CurrentDirectory != "")
        {
            foreach (var i in Directory.GetDirectories(CurrentDirectory))
            {
                DirectoryStruct temp = new DirectoryStruct();
                temp.Path = Path.GetFullPath(i);
                temp.IsDirectory = true;
                temp.filename = i.Replace(CurrentDirectory + @"\", "");
                temp.dateModified = File.GetLastWriteTime(temp.Path).ToString();
                Contents.Add(temp);

            }

            foreach (var i in Directory.GetFiles(CurrentDirectory, "*.nc"))
            {
                Debug.LogError(i);
                DirectoryStruct temp = new DirectoryStruct();
                temp.Path = Path.GetFullPath(i);
                temp.IsDirectory = false;
                temp.filename = Path.GetFileName(i);
                temp.dateModified = File.GetLastWriteTime(temp.Path).ToString();
                float size = (float)new FileInfo(temp.Path).Length / 1024;
                if (size > 1000)
                {
                    size /= 1000;
                    temp.bytes = size.ToString("F2") + " MB";
                }
                else
                {
                    temp.bytes = size.ToString("F2") + " KB";
                }

                Contents.Add(temp);
            }
        }
        else
        {
            Debug.LogError("DISPLAY DRIVES");
            foreach (var i in Directory.GetLogicalDrives())
            {
                DirectoryStruct temp = new DirectoryStruct();
                Debug.LogError(i);
                temp.IsDirectory = true;
                temp.Path = i;
                temp.filename = i;
                Contents.Add(temp);
            }
        }
        return Contents;
    }

    public void GoBack()
    {
        CurrentDirectory = Path.GetFullPath(CurrentDirectory + @"\..");
    }


}
