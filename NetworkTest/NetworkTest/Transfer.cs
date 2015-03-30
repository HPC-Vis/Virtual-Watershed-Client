using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Transfer
{
    /// <summary>
    /// Defines the different types of paths that can be used.
    /// </summary>
    public enum Type
    {
        URL, FILE, UNKNOWN
    };

    /// <summary>
    /// Determines the type of file path being used
    /// </summary>
    /// <param name="str">File to be downloades's location, either a url or in a local directory</param>
    /// <returns>The Type of the file path : URL or FILE</returns>
    public static Type GetType(ref String str)
    {
        if (str.StartsWith("url://"))
        {
            str = str.Substring(6);
            return Type.URL;
        }
        else if (str.StartsWith("file://"))
        {
            str = str.Substring(7);
            return Type.FILE;
        }

        // Else
        return Type.UNKNOWN;
    }
}
