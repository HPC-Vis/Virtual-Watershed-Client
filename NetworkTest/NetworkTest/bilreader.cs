using System.Collections;
using System;
//using SharpMap.Web;
public class bilreader
{
    // A simple parser that returns a two dimensional float array of data.
    public static float[,] parse(string header, byte[] data)
    {

        // Initialize variables
        float[,] arr = new float[1, 1];

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



            // data type array that will information
            arr = new float[nrows, ncols];

            float max = float.MinValue;

            // Now to convert the array.
            for (int i = 0; i < nrows; i++)
            {
                for (int j = 0; j < ncols; j++)
                {
                    // Endianess

                    // A systematic convertor -- 
                    arr[i, j] = BitConverter.ToSingle(data, 4 * (i * ncols + ncols - 1 - j));
                    if (arr[i, j] > max)
                    {
                        max = arr[i, j];
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }


        return arr;
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