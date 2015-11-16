using UnityEngine;
using System.Collections;
using System.IO;

public class clearCache : MonoBehaviour {
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../";
    public void ClearCache()
    {
        if(Directory.Exists(DirectoryLocation + "Cache")){
            Directory.Delete(DirectoryLocation + "Cache", true);
        }
    }
}
