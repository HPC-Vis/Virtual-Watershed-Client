using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// @author Chase Carthen
/// bilreader 
/// The bilreader is meant to handle any bile passed to it.
/// It expects that you pass a the bil header and data.
/// The header is used to help parse the byte data passed to it.
/// A future consideration for this reader is to actually pass out any url information...
/// As of current it only handles one band.
/// This could be easily be extended to test more than one band.
/// </summary>
public class bilreader
{
    // A simple parser that returns a two dimensional float array of data.
    public static List<float[,]> parse(string header, byte[] data)
    {
        if(header == "")
        {
            Logger.Log("BILREADER: Error no Header for " + data.Length + " Bytes.");
            return null;
        }

        List<float[,]> OutData = new List<float[,]>();
        // Parse Header file for stuff
        try
        {
            string[] param = header.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string i in param)
            {
                string[] query = i.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                switch (query[0])
                {
                    case "NBITS":
                        nbits = int.Parse(query[1]);
                        break;
                    case "NROWS":
                        nrows = int.Parse(query[1]);
                        break;
                    case "NCOLS":
                        ncols = int.Parse(query[1]);
                        break;
                    case "NBANDS":
                        nbands = int.Parse(query[1]);
                        break;
                    case "BYTEORDER":
                        break;
                    case "LAYOUT":
                        layout = query[1];
                        break;
                    case "BANDROWBYTES":
                        bandrowbytes = int.Parse(query[1]);
                        break;
                    case "TOTALROWBYTES":
                        totalrowbytes = int.Parse(query[1]);
                        break;
                    case "PIXELTYPE":
                        pixeltype = query[1];
                        break;
                    case "XDIM":
                        xdim = float.Parse(query[1]);
                        break;
                    case "YDIM":
                        ydim = float.Parse(query[1]);
                        break;
                    case "ULXMAP":
                        ulxmap = float.Parse(query[1]);
                        break;
                    case "ULYMAP":
                        ulymap = float.Parse(query[1]);
                        break;
                }
            }


            float max = float.MinValue;
            UnityEngine.Debug.LogError(header);
            UnityEngine.Debug.LogError(data.Length);
            UnityEngine.Debug.LogError(bandrowbytes);
            UnityEngine.Debug.LogError(totalrowbytes);
            // Now to convert the array.
            for (int b = 0; b < nbands; b++)
            {
                // Initialize variables
                float[,] arr = new float[nrows, ncols];
                for (int i = 0; i < nrows; i++)
                {
                    for (int j = 0; j < ncols; j++)
                    {
                        // Endianess

                        // A systematic convertor -- assuming no bits to skip getting help from http://webhelp.esri.com/arcgisdesktop/9.2/index.cfm?TopicName=BIL,_BIP,_and_BSQ_raster_files
                        arr[i, ncols - 1 - j] = BitConverter.ToSingle(data, b*bandrowbytes + j * 4 + i * totalrowbytes);
                        if (arr[i, j] > max)
                        {
                            max = arr[i, j];
                        }
                    }
                }
                OutData.Add(arr);
            }
        }
        catch (Exception e)
        {
            Logger.WriteLine(e.Message);
            Logger.WriteLine(e.StackTrace);
            Logger.Log("BILREADER: " + e.Message);
            Logger.Log("BILREADER: " + e.StackTrace);
            Logger.Log("BILREADER: " + header);
            /// Return if there is an error.
            return null;
        }


        return OutData;
    }

    static int nbits;
    static int nrows;
    static int ncols;
    static int nbands;
    static bool byteorder;
    static string layout;
    static int bandrowbytes;
    static int totalrowbytes;
    static string pixeltype;
    static float ydim;
    static float xdim;
    static float ulxmap;
    static float ulymap;
}