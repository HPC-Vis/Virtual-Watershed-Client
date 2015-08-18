using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class VariableReference {

    // The file for loading in the data
	const string VariableFile = "VariableReferenceData.txt";
#if UNITY_EDITOR
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"/../../VariableReference/";
#else
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
#endif

    // Failed to find file
    private bool File_Not_Found = false;

    // File Data in Array
    private string[] reference_lines;

	/// <summary>
	/// Initializes a new instance of the <see cref="VariableReference"/> class.
	/// This will open the file, and read in all the data that is inside line by line.
	/// These lines are added to an array of each line.
	/// </summary>
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

    /// <summary>
    /// This will get the variable name in the list of refrences names, and extract the description from there.
    /// </summary>
    /// <param name="variable">This is a string to specify what the variable name is.</param>
    /// <returns>A string of the description of the sent in variable string.</returns>
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
