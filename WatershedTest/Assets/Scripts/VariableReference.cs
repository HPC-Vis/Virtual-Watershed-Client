using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class VariableReference {

    // The file for loading in the data
#if UNITY_EDITOR
    const string VariableFile = "VariableReferenceData.txt";
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"/../../VariableReference/";
#else
    const string VariableFile = "VariableReferenceData.txt";
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
#endif

    // Failed to find file
    private bool File_Not_Found = false;

    // File Data in Array
    private string[] reference_lines;

    // Use this for initialization
	public VariableReference () {
        if (!Directory.Exists(DirectoryLocation))
        {
            File_Not_Found = true;
            Debug.LogError("Variable Reference File not Found");
            return;
        }
        if (!File.Exists(DirectoryLocation + VariableFile))
        {
            File_Not_Found = true;
            Debug.LogError("Variable Reference File not Found");
            return;
        }

        reference_lines = System.IO.File.ReadAllLines(DirectoryLocation + VariableFile);      
	}


    public string GetDescription(string variable)
    {
        if (!File_Not_Found)
        {
            foreach (var line in reference_lines)
            {
                if (line.StartsWith(variable))
                {
                    return line.Substring(variable.Length + 2, line.Length - (variable.Length + 2));
                }
            }
        }
        return "No Description Found";
    }
	
}
