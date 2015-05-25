using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

/// <summary>
/// Allows for the caching of datasets
/// Reference used for code : http://www.codeproject.com/Questions/180136/File-Based-Cache-c
/// </summary>
public static class FileBasedCache
{
    private static readonly object _Padlock = new object();
    static Dictionary<string, string> _FileMap;
#if UNITY_EDITOR
    const string MAPFILENAME = "FileBasedCacheMAP.dat";
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"/../../Cache";
#else
    const string MAPFILENAME = "FileBasedCacheMAP.dat";
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#endif

   
    static FileBasedCache()
    {
        if (!Directory.Exists (DirectoryLocation)) 
		{
			Directory.CreateDirectory(DirectoryLocation);
			//throw new ArgumentException ("directoryLocation msu exist");
		}
        if (File.Exists(MyMapFileName))
        {
            _FileMap = DeSerializeFromBin<Dictionary<string, string>>(MyMapFileName);
        }
        else
        {
            _FileMap = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Checks to see if an entry has been cached
    /// </summary>
    /// <param name="key">The key of the enrtry to be checked for</param>
    /// <returns>True if the entry is in the cache, false otherwise</returns>
    public static bool Exists(string key)
    {
        return _FileMap.ContainsKey(key);
    }

    /// <summary>
    /// Clears the file based cache
    /// </summary>
    public static void Clear()
    {
        // Delete the files contained in the cache
        foreach(var i in _FileMap)
        {
            File.Delete(i.Value);
        }

        // Clear the dictionary used for mapping to the cache
        _FileMap.Clear();
    }

    /// <summary>
    /// Retrieves a record from the cache
    /// </summary>
    /// <typeparam name="T">The data type of the entry being retrieved</typeparam>
    /// <param name="key">The key of the entry to be retrieved</param>
    /// <returns>The datarecord requested from the file cache, if it exists</returns>
    public static T Get<T>(string key) where T : new()
    {
        if (_FileMap.ContainsKey(key))
            return (T)DeSerializeFromBin<T>(_FileMap[key]);
        else
            throw new ArgumentException("Key not found");
    }

    /// <summary>
    /// Caches an entry (dataRecord in our case)
    /// </summary>
    /// <typeparam name="T">the data type of the entry being inserted</typeparam>
    /// <param name="key">A key for the entry being inserted</param>
    /// <param name="value">The entry to be serialized and input to the cache</param>
    public static void Insert<T>(string key, T value)
    {
        //if the key is already in use, overwrite its corresponding value
        lock (_Padlock)
        {
            if (_FileMap.ContainsKey(key))
            {
                SerializeToBin(value, _FileMap[key]);
            }
            //else, add a new entry to the cache
            else
            {
                _FileMap.Add(key, GetNewFileName);
                SerializeToBin(value, _FileMap[key]);
            }
            SerializeToBin(_FileMap, MyMapFileName);
        }
    }

    /// <summary>
    /// Generates a unique file name
    /// </summary>
    private static string GetNewFileName
    {
        get
        {
            return Path.Combine(DirectoryLocation, Guid.NewGuid().ToString());
        }
    }

    /// <summary>
    /// Returns the name of the cache map
    /// </summary>
    private static string MyMapFileName
    {
        get
        {
            return Path.Combine(DirectoryLocation, MAPFILENAME);
        }
    }

    /// <summary>
    /// Serializes an object and writes it to a file
    /// </summary>
    /// <param name="obj">The object to be serialized</param>
    /// <param name="filename">The file for the serialized data to be written to</param>
    private static void SerializeToBin(object obj, string filename)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filename));
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            bf.Serialize(fs, obj);
        }
    }

    /// <summary>
    /// Deserialzes an entry of the file cache and returns an object containing that data
    /// </summary>
    /// <typeparam name="T">The type of the object being deserialized</typeparam>
    /// <param name="filename">The filename containing the data to be deserialized</param>
    /// <returns>An object of type T containing the deserialized data from filename</returns>
    private static T DeSerializeFromBin<T>(string filename) where T : new()
    {
        if (File.Exists(filename))
        {
            T ret = new T();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                ret = (T)bf.Deserialize(fs);
            }
            return ret;
        }
        else
        {
            throw new FileNotFoundException(string.Format("file {0} does not exist", filename));
        }
    }

}
