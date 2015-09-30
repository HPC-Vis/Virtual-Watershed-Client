using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

static class VariableReference {

    // The file for loading in the data
	const string VariableFile = "VariableReferenceData.txt";
#if UNITY_EDITOR
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"/../../VariableReference/";
#else
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/VariableReference/";
#endif

    // Failed to find file
    static private bool File_Not_Found = false;

    // File Data in Array
    static private string[] reference_lines;

    // Dictionary of values
    static private Dictionary<string, string> reference = new Dictionary<string, string>();

	/// <summary>
	/// Initializes a new instance of the <see cref="VariableReference"/> class.
	/// This will open the file, and read in all the data that is inside line by line.
	/// These lines are added to an array of each line.
	/// </summary>
	static VariableReference () {
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
        reference = new Dictionary<string, string>();

        foreach (var line in reference_lines)
        {
            string[] split_data = line.Split('\t');
            reference.Add(split_data[0], split_data[1]);
        }
	}

    /// <summary>
    /// This will get the variable name in the list of refrences names, and extract the description from there.
    /// </summary>
    /// <param name="variable">This is a string to specify what the variable name is.</param>
    /// <returns>A string of the description of the sent in variable string.</returns>
    public static string GetDescription(string variable)
    {
        if (!File_Not_Found)
        {
            if(reference.ContainsKey(variable))
            {
                return reference[variable];
            }
        }
        return "No Description Found";
    }

    public static void AddDescription(string variable, string description)
    {
        reference.Add(variable, description);
    }
	
}
