using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

class WCS_GetCapabilites_Parser : Parser
{
    void ParseWCSCapabilities(DataRecord Record, string Str)
    {
    	if(Str == null)
    	{
    		Logger.WriteLine("NULLL");
    	}
        var reader = System.Xml.XmlTextReader.Create(new System.IO.StringReader(Str));
        Record.WCSCapabilities = Str;
        //var c = new XmlReaderSettings();
        //var x = XmlReader.Create("",new XmlReaderSettings());
        

        XmlSerializer serial = new XmlSerializer(typeof(GetCapabilites.Capabilities));
        
        GetCapabilites.Capabilities capabilities = new GetCapabilites.Capabilities();

        if (serial.CanDeserialize(reader))
        {
            capabilities = ((GetCapabilites.Capabilities)serial.Deserialize(reader));
            Record.WCSCap = capabilities;
            Record.WCSOperations = capabilities.OperationsMetadata;
            Record.WCSCoverages = capabilities.Contents;
            if(Record.Identifier == null)
            {
            	Logger.WriteLine("GETTING THE IDENTIFIERS");
				GetCapabilites.OperationsMetadataOperation gc = new GetCapabilites.OperationsMetadataOperation();
				foreach (GetCapabilites.OperationsMetadataOperation i in Record.WCSOperations)
				{
					if (i.name == "DescribeCoverage")
					{
						gc = i;
						break;
					}
				}
				string parameters = "";
				bool done = false;
				// For now picking first valid parameters
				foreach (GetCapabilites.OperationsMetadataOperationParameter i in gc.Parameter)
				{
					foreach (string j in i.AllowedValues)
					{
						if(i.name == "identifiers")
						{
							Record.Identifier = i.AllowedValues[0];
							done = true;
							Logger.WriteLine("SMILE");
							break;
						}
						
						//break;
					}
					if(done)
					{
						break;
					}
				}
            }
        }
    }

    public override DataRecord Parse(DataRecord record, string Contents)
    {
        // Do stuff here
        ParseWCSCapabilities(record, Contents);
        return record;
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
        var sw = new System.IO.StreamWriter(Path + OutputName + ".xml");
        sw.Write(Str);
        sw.Close();
    }
}

