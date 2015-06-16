using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * // Parses a response from virtual watershed query.
 */
class VW_JSON_MODEL_RUN_Parser : Parser
{
    public override List<DataRecord> Parse(List<DataRecord> Records, string Contents)
    {
        if (Records != null && Contents != null)
        {
            // Place parsing functions for json here :D.
            parseJSON(ref Records, Contents);
        }
        return null;
    }

    /// <summary>
    /// This version of parse parses the given input and outputs it to the file directory.
    /// </summary>
    /// <param name="Path"></param>
    /// <param name="OutputName"></param>
    /// <param name="str"></param>
    public override void Parse(string Path, string OutputName, string Str)
    {

        // Initialize variables
        var sw = new System.IO.StreamWriter(Path + OutputName + ".json");
        sw.Write(Str);
        sw.Close();
    }

    DateTime getDateTime(string time)
    {
        string[] nums = time.Split(new char[] { '-', ':' }, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine(nums[0]);
        return new DateTime(int.Parse(nums[0]), int.Parse(nums[1]), int.Parse(nums[2]), int.Parse(nums[3]), int.Parse(nums[4]), int.Parse(nums[5]));
    }

    void parseJSON(ref List<DataRecord> Records, string response)
    {
        // This code below is for pulling out the services
        string encodedString = response;
        var encoded = SimpleJSON.JSONNode.Parse(encodedString);
        if (encoded == null)
        {
            return;
        }
        
        if (encoded["results"].Count > 0)
        {
            Logger.WriteLine("RESULTS: " + encoded["results"].Count);
            // iterating through results
            for (int records = 0; records < encoded["results"].Count; records++)
            {
                DataRecord current = new DataRecord();

                var result = encoded["results"][records];
                current.modelname = result["Model Run Name"].ToString().Replace('"', ' ').Trim();
                current.modelRunUUID = result["Model Run UUID"].ToString().Replace('"', ' ').Trim();
                current.description = result["Description"];

                Records.Add(current);

                //Logger.WriteLine(result["Keywords"]);
                //Logger.WriteLine(result["Model Run Name"]);
                //Logger.WriteLine(result["Researcher Name"]);
                //Logger.WriteLine(result["Description"]);
                //Logger.WriteLine(result["Model Run UUID"]);
            }

        }

    }


}