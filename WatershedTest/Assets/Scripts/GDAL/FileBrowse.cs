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

    public string CurrentDirectory = ".";
    string relativepath = ".";
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetDirectory(string Directory)
    {
        CurrentDirectory = Path.GetFullPath(CurrentDirectory + @"\" + Directory);
    }

    public List<DirectoryStruct> GetContents()
    {
        List<DirectoryStruct> Contents = new List<DirectoryStruct>();
        foreach(var i in Directory.GetDirectories(CurrentDirectory))
        {
            DirectoryStruct temp = new DirectoryStruct();
            temp.Path = Path.GetFullPath(i);
            temp.IsDirectory = true;
            temp.filename = i.Replace(CurrentDirectory+@"\","");
            temp.dateModified = File.GetLastWriteTime(temp.Path).ToString();
            Contents.Add(temp);
            
        }

        foreach(var i in Directory.GetFiles(CurrentDirectory,"*.nc"))
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
        return Contents;
    }

    public void GoBack()
    {
        CurrentDirectory = Path.GetFullPath(CurrentDirectory + @"\..");
    }


}
