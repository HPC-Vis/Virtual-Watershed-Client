using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using System.Collections.ObjectModel;
public class mimeparser : Parser
{
    string name = "Mime Parser";
    public void parseBIL(byte[] bytes, ref string Header, ref byte[] Data)
    {
        // Stopwatch
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        // Initialize variables
        string header = "";
        byte[] byteData = new byte[1];
        string str = Encoding.ASCII.GetString(bytes);
        string[] sections = str.Split(new string[] { "--wcs" }, StringSplitOptions.RemoveEmptyEntries);
        Regex regComp = new Regex("Content.");

        // Loop through all the sections
        foreach (string section in sections)
        {
            // Check if the section contains
            if (section.Contains("ID: coverage/out.hdr"))
            {
                // Split on the "Content-"
                string[] contents = regComp.Split(section);

                // Loop through the "Content-" parts
                foreach (string j in contents)
                {
                    // Look for the disposition inline
                    if (j.Contains("Disposition: INLINE"))
                    {
                        // Skip the first two lines, remove trailing whitespace
                        int dataStart = j.IndexOf("\r\n") + 4;
                        header = j.Substring(dataStart, j.Length - 2 - dataStart);
                    }
                }
            }

            // Check if the section contains
            if (section.Contains("Type: image/bil"))
            {
                // Split on the "Content-"
                string[] contents = regComp.Split(section);

                // Loop through the "Content-" parts
                foreach (string j in contents)
                {
                    // Look for the disposition inline
                    if (j.Contains("Disposition: INLINE"))
                    {
                        // Skip the first two lines, remove trailing whitespace
                        int dataStart = j.IndexOf("\r\n") + 4;
                        string strData = j.Substring(dataStart, j.Length - 2 - dataStart);

                        // Allocate the data byte array and copy the data over
                        byteData = new byte[strData.Length];

                        try
                        {
                            System.Array.Copy(bytes, str.IndexOf(strData), byteData, 0, strData.Length);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }
                }
            }
        }
        Header = header;
        Data = byteData;
        // Stopwatch
        stopWatch.Stop();
    }
    /// <summary>
    /// Parses the MIME. The follow function will take in bytes and parse for a bil file.
    /// </summary>
    /// <param name="bytes">Raw bytes of the MIME message.</param>
    /// <param name="record">Record passed by reference. Parsed data will be stored in here.</param>
    public override DataRecord Parse(DataRecord record, byte[] bytes)
    {
        Console.WriteLine("HERE!!!!!!!!" + record.name);
        // Stopwatch
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        // Initialize variables
        string header = "";
        byte[] byteData = new byte[1];
        parseBIL(bytes, ref header, ref byteData);

        // TODO
        // Still have to save the data in the data record
        record.Data = bilreader.parse(header, byteData);
        record.isSet = true;

        return record;
        /**
         //float[,] outData ;
         //StartCoroutine(giveData(header, data));
         var outData = bilreader.parse(header,data);
         //dpt.downloadData(header, data);
         datamanager.Instance.records [key].Data = outData;
         //GameObject.Find("TerrainBuilder").GetComponent<terrainbuilder>().buildTerrain(outData);

        
         datamanager.Instance.DataBuilder.createTexture(key);

         if(datamanager.Instance.records[key].TYPE.Contains("DEM"))
         datamanager.Instance.DataBuilder.addTerrain(key);
        
         // If isnobal then do othe stuff
        

         stopWatch.Stop();
         if (datamanager.Instance.records[key].dataSetters["data"] != null)
         datamanager.Instance.records[key].dataSetters["data"](outData,outData.GetType());
         Debug.LogError("ELAPSED TIME: " + stopWatch.ElapsedMilliseconds); **/

    }
    /// <summary>
    /// This version of parse parses the given input and outputs it to the file directory.
    /// </summary>
    /// <param name="Path"></param>
    /// <param name="OutputName"></param>
    /// <param name="bytes"></param>
    public override void Parse(string Path, string OutputName, byte[] bytes)
    {

        // Initialize variables
        string header = "";
        byte[] byteData = new byte[1];
        parseBIL(bytes, ref header, ref byteData);

        // Output to files.
        var bw = new System.IO.BinaryWriter(new System.IO.FileStream(Path + OutputName + ".bil", System.IO.FileMode.Create));
        bw.Write(byteData, 0, byteData.Length);
        bw.Close();
        Console.WriteLine("WROTE: " + Path + OutputName + ".bil");

        var sw = new System.IO.StreamWriter(Path + OutputName + ".hdr");
        sw.Write(header);
        sw.Close();
        Console.WriteLine("WROTE: " + Path + OutputName + ".hdr");
    }
}