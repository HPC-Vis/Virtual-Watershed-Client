using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/** 
 * @author Chase Carthen
 * @brief This is the dataRecord class meant to hold any information downloaded from the virutal watershed.
 * @details The dataRecord class has specific data fields that are meant to be used by the Unity application.
 */
[Serializable]
public class DataRecord
{
    public DataRecord()
    {
    }
    public DataRecord( string recordName )
    {
        name = recordName;
    }

    public static bool operator ==(DataRecord a, DataRecord b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.name == b.name &&
               a.id == b.id &&
               a.description == b.description &&
               a.location == b.location &&
               a.modelname == b.modelname &&
               a.state == b.state &&
               a.start == b.start &&
               a.end == b.end &&
               a.modelRunUUID == b.modelRunUUID &&
               a.data == b.data;
    }

    public static bool operator !=(DataRecord a, DataRecord b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return false;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields don't match:
        return a.name != b.name ||
               a.id != b.id ||
               a.description != b.description ||
               a.location != b.location ||
               a.modelname != b.modelname ||
               a.state != b.state ||
               a.start != b.start ||
               a.end != b.end ||
               a.modelRunUUID != b.modelRunUUID ||
               a.data != b.data;
    }

    /** These fields are necessary for generating the key used in datamanager datastructure. 
     *
     */
    // The ID, name, description and categories of this dataset or uuid.
    public string id;
    public string name;
    
    //****** Geospatial MetaData**/
    public SerialVector2 resolution;

    // This variable must be in Lat Long
    SerialVector2 WGS84Origin;

    // The bounding box of this dataset
    public string bbox; // --- In the dataset class there will be a floating point representation. ---

    // The EPSG of this dataset
    public string projection;

    // A rect in UTM coordinates that will be used for splatting textures onto terrain....
    public SerialRect boundingBox;

    // An interface for acquiring the WGS84 Origin.
    public SerialVector2 WGS84origin
    {
        get
        {
            return WGS84Origin;
        }
        set
        {
            WGS84Origin = value;
        }
    }

    // This datastructure holds the metadata for the OGC services.
    /// <summary>
    /// The WCS Operations is used to generate WCS downloads ~ to call the bil (or tif) factory.
    /// 
    /// </summary>
    /// WCS Operations
    public GetCapabilites.OperationsMetadataOperation[] WCSOperations = null;
    public string WMSCapabilities = "";
    public string WFSCapabilities = "";
    public string WCSCapabilities = "";

    public Dictionary<string, string> services = new Dictionary<string,string>();

    // Description of data **/
    public string description;

    public string state;
    public string location;

    public string title;

    // Model Variables **/
    public string modelname;
    public string modelRunUUID;
    public string variableName;

    // Temporal Fields for metadata 
    public DateTime start;
    public DateTime end;

    // A test set flag for one variable tests
    public bool isSet = false;

    /*
     * Metadata fields
     */
    // This will hold any metadata... -- This will include the height and width of the dataset.
    public metadata metaData;

    // Unused as of this moment.
    public Dictionary<string, string> categories;


    /*
     * Data Fields  
     *
     */


    //This for pngs
    public byte[] texture;

    // This for shapes 
    // If there is only one point per element in the list then it is a point
    List<List<SerialVector2>> lines;
    public List<List<SerialVector2>> Lines
    {
        get
        {
            return lines;
        }
        set
        {
            lines = value;
        }
    }


    // Thif for DEM,model runs, and raw float values
    float[,] data;
    public float[,] Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
        }
    }

    // Data setters for the dataRecord class that are utilized for setting data in these fields.
    public delegate void giveData(object o, System.Type type);

    public Dictionary<string, giveData> dataSetters = new Dictionary<string, giveData>();

    // This is the holy grail of what the data should be represented as in the visualization.
    // Currently the applciation uses the TYPE to designate the difference between Shapes, DEMs, and model data.
    public string TYPE;


    // Temporary parameters.
    public int width;
    public int height;

};
