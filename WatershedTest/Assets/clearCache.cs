using UnityEngine;
using System.Collections;

public class clearCache : MonoBehaviour {

    public void ClearCache()
    {
        FileBasedCache.Clear();
    }
}
