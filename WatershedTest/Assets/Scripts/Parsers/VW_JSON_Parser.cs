using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * // Parses a response from virtual watershed query.
 */
class VW_JSON_Parser : Parser
{
    public override List<DataRecord> Parse(List<DataRecord> Records, string Contents)
    {
        if(Records != null && Contents != null)
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
        DataRecord current;
        if (encoded["results"].Count > 0)
        {
            // iterating through results
            for (int records = 0; records < encoded["results"].Count; records++)
            {
                //Logger.WriteLine(encoded["results"].Count.ToString());
                //Logger.WriteLine(records.ToString());
                //Logger.ReadKey();
                current = null;
                //first determine if record exists in collection to be updated or added

                var id = encoded["results"][records]["uuid"].ToString().Replace('"', ' ').Trim();
                var name = encoded["results"][records]["name"].ToString().Replace('"', ' ').Trim();

                /*foreach (var i in Records)
                {
                    if(i.name == name)
                    {
                        current = i;
                    }
                }
                if(current == null)
                {*/
                //if()
                current = new DataRecord();
                Records.Add(current);
                //}

                current.services["xml_fgdc"] = encoded["results"][records]["metadata"][0]["FGDC-STD-001-1998"]["xml"];
                //Logger.WriteLine(current.services["xml_fgdc"]);
                var description = encoded["results"][records]["description"].ToString();



                SimpleJSON.JSONNode dict = encoded["results"][records]["spatial"].AsObject;
                current.Type = encoded["results"][records]["description"].ToString().Split(new char[] { ' ' })[0].Replace('"', ' ').Trim();
                if (encoded["results"][records]["description"].ToString().Contains("dem"))
                {
                    current.Type = "DEM";
                }
                current.modelRunUUID = encoded["results"][records]["model_run_uuid"].ToString().Replace('"', ' ').Trim();
                //Logger.WriteLine(current.model_set_type = encoded["results"][records]["model_set_type"].ToString());
                current.model_set_type = encoded["results"][records]["model_set_type"].ToString().Replace('"', ' ').Trim();

                current.modelname = encoded["results"][records]["categories"][0]["modelname"].ToString().Replace('"', ' ').Trim();

                current.state = encoded["results"][records]["categories"][0]["state"].ToString().Replace('"', ' ').Trim();

                current.location = encoded["results"][records]["categories"][0]["location"].ToString().Replace('"', ' ').Trim();
				current.multiLayered = encoded["results"][records]["downloads"][0]["nc"];
                //Logger.WriteLine((current.multiLayered == null).ToString());
                //if (encoded["results"][records].ToString().Contains("valid_dates"))
                //Console.WriteLine(encoded["results"][records].ToString());

                if (encoded["results"][records]["valid_dates"] != null)
                {
                    current.start = getDateTime(encoded["results"][records]["valid_dates"]["start"].ToString().Replace('"', ' ').Trim());
                    current.end = getDateTime(encoded["results"][records]["valid_dates"]["end"].ToString().Replace('"', ' ').Trim());
                    Console.WriteLine("DATE: " + encoded["results"][records]["valid_dates"]["start"] + " " + encoded["results"][records]["valid_dates"]["end"] + " " + records);
                    Console.WriteLine(current.start);

                }

                var projection = dict["epsg"].AsInt;
                var coordinates = dict["bbox"].AsArray;

                string bbox = dict["bbox"].ToString();
                var services_to_replace = encoded["results"][records]["services"].ToString();
                var replaced = services_to_replace.Replace("{", String.Empty).Replace("[", "{").Replace("}", String.Empty).Replace("]", "}");
                var services = SimpleJSON.JSONNode.Parse(replaced);
                if (services != null)
                {
                    // Get ogcservices links
                    if (services["wms"] != null)
                        current.services["wms"] = services["wms"];

                    if (services["wcs"] != null)
                    {
                        // Make a request for wcs coveragess
                        current.services["wcs"] = services["wcs"];
                    }

                    if (services["wfs"] != null)
                        current.services["wfs"] = services["wfs"];
                }

                // Populate datarecord here.
                current.projection = "epsg:" + projection.ToString();
                //current.services = new Dictionary<string, string>();
                current.bbox = bbox;
                current.id = id;
                current.name = name;
                current.variableName = encoded["results"][records]["model_vars"].ToString().Replace('"', ' ').Trim();

                current.description = description;
               // Logger.WriteLine(name);

            }

        }

    }


}