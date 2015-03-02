using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

/// <summary>
/// @author Chase Carthen
/// @brief This class is meant to be used as a data container for downloads.
/// This container will keep track of priority and even allows itself to be sorted in compatible containers
///  
/// </summary>
public class DownloadRequest : IComparable<DownloadRequest>
{
    public string Url;
    public int Priority;
    public StringFunction stringFunction;
    public ByteFunction byteFunction;
    public bool isByte;

    public DownloadRequest(string url, ByteFunction byteFunc, int priority = 1)
    {
        Url = url;
        byteFunction = byteFunc;
        isByte = true;
        Priority = priority;
    }

    public DownloadRequest(string url, StringFunction stringFunc, int priority = 1)
    {
        Url = url;
        stringFunction = stringFunc;
        isByte = false;
        Priority = priority;
    }

    public int CompareTo(DownloadRequest other)
    {
        if (this.Priority < other.Priority) return -1;
        else if (this.Priority > other.Priority) return 1;
        else return 0;
    }

    public void Callback(string str)
    {
        if (!isByte)
        {
            stringFunction(str);
        }
    }

    public void Callback(byte[] bytes)
    {
        if (isByte)
        {
            byteFunction(bytes);
        }
    }
}