using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public static class Logger
{
    static string path="./log.txt";
    static readonly object LOCK = new object();
    static StreamWriter writer = null;
    static bool ToFile=false;
    static public bool enable=true;
    static bool closed = false;
    public static void SetPath(string dest)
    {
        if (path != null && writer != null)
        {
            writer.Close();
        }

        writer = new StreamWriter(dest);
        path = dest;
    }
    public static void WriteToFile()
    {
        if (writer == null)
        {
            writer = new StreamWriter(path);
        }
        ToFile = true;
    }
    public static void Log(string line)
    {
        lock (LOCK)
        {
            if(closed)
            {
                return;
            }

            if (writer == null)
                WriteToFile();
            if (writer != null && enable)
            {
                writer.WriteLine(line);
            }
        }
    }

    public static void WriteLine(string line)
    {
        if (enable)
        {

#if UNITY_EDITOR
            Debug.Log(line);
#else
            Console.WriteLine(line);
#endif

        }
        if (ToFile && enable)
        {
            Log(line);
        }
    }

    public static void ReadKey()
    {
#if !(UNITY_EDITOR)
        Console.ReadKey();
#endif
    }

    public static void Finalize()
    {
        Close();
    }

    private static void Close()
    {
        lock (LOCK)
        {
            path = null;
            if (writer != null)
                writer.Close();
            writer = null;
            closed = true;
        }
    }
}
