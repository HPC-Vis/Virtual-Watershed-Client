using UnityEngine;
using System.Collections;
using OSGeo.GDAL;

public class WCSInterface  
{
    
    public WCSInterface()
    { 
    }

    public string XMLElement(string name, string value)
    {
        return "<"+name + ">" + value + "</" + name + ">";

    }

    public string buildXMLString(DataRecord record)
    {
        if(record.WCSOperations == null)
        {
            return "";
        }

        // Grab the identifier of the record
        string identifer = record.Identifier;

        // Grab the service url of the record
        string ServiceUrl = record.WCSCap.ServiceProvider.ProviderSite.href;

        // produce a xml file.
        string wcs_xml_string = XMLElement("WCS_GDAL>",XMLElement("ServiceURL", ServiceUrl) + "\n" + XMLElement("CoverageName", identifer) );
        return "";
    }

    public void GetData()
    {
        // Get the data with some class here
    }
}
