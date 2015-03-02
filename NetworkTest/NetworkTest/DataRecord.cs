using System.Collections;
using System.Collections.Generic;
using System;

/** 
 * @author Chase Carthen
 * @brief This is the dataRecord class meant to hold any information downloaded from the virutal watershed.
 * @details The dataRecord class has specific data fields that are meant to be used by the Unity application.
 */
public class DataRecord
{
    public DataRecord()
    {
    }
    public DataRecord( string recordName )
    {
        name = recordName;
    }

    /** These fields are necessary for generating the key used in datamanager datastructure. 
     *
     */
    // The ID, name, description and categories of this dataset or uuid.
    public string id;
    public string name;

    //****** Geospatial MetaData**/
    //public Vector2 resolution;

    // This variable must be in Lat Long
    //Vector2 WGS84Origin;

    // The bounding box of this dataset
    public string bbox; // --- In the dataset class there will be a floating point representation. ---

    // The EPSG of this dataset
    public string projection;

    // A rect in UTM coordinates that will be used for splatting textures onto terrain....
    //public Rect boundingBox;

    // An interface for acquiring the WGS84 Origin.
    /*public Vector2 WGS84origin
    {
        get
        {
            return WGS84Origin;
        }
        set
        {
            WGS84Origin = value;
        }
    }*/

    // This datastructure holds the metadata for the OGC services.
    public Dictionary<string, string> services;

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
    public Dictionary<string, string> metaData;

    // Unused as of this moment.
    public Dictionary<string, string> categories;


    /*
     * Data Fields  
     *
     */

    public byte[] texture;

    // If there is only one point per element in the list then it is a point
    /*List<List<Vector2>> lines;
    public List<List<Vector2>> Lines
    {
        get
        {
            return lines;
        }
        set
        {
            lines = value;
        }
    }*/

    // This could be something else for shapes
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

};
