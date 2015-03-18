using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public static class FileBasedCache
{
    static Dictionary<string, string> _FileMap;
    const string MAPFILENAME = "FileBasedCacheMAP.dat";
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    static FileBasedCache()
    {
        if (!Directory.Exists(DirectoryLocation))
            throw new ArgumentException("directoryLocation msu exist");
        if (File.Exists(MyMapFileName))
        {
            _FileMap = DeSerializeFromBin<Dictionary<string, string>>(MyMapFileName);
        }
        else
            _FileMap = new Dictionary<string, string>();
    }

    public static bool Exists(string key)
    {
        return _FileMap.ContainsKey(key);
    }
    public static void Clear()
    {
        foreach(var i in _FileMap)
        {
            File.Delete(i.Value);
        }
        _FileMap.Clear();
    }
    public static T Get<T>(string key) where T : new()
    {
        if (_FileMap.ContainsKey(key))
            return (T)DeSerializeFromBin<T>(_FileMap[key]);
        else
            throw new ArgumentException("Key not found");
    }
    public static void Insert<T>(string key, T value)
    {
        if (_FileMap.ContainsKey(key))
        {
            SerializeToBin(value, _FileMap[key]);
        }
        else
        {
            _FileMap.Add(key, GetNewFileName);
            SerializeToBin(value, _FileMap[key]);
        }
        SerializeToBin(_FileMap, MyMapFileName);
    }
    private static string GetNewFileName
    {
        get
        {
            return Path.Combine(DirectoryLocation, Guid.NewGuid().ToString());
        }
    }
    private static string MyMapFileName
    {
        get
        {
            return Path.Combine(DirectoryLocation, MAPFILENAME);
        }
    }
    private static void SerializeToBin(object obj, string filename)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filename));
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            bf.Serialize(fs, obj);
        }
    }


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
