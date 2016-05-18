using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// A class to contain a Min of Max and store the location.
/// This is for ease of lookup.
/// </summary>
[Serializable]
public class ValueContainer
{
    // Data
    public float value;
    public Vector2 location;

    public ValueContainer(float newValue, Vector2 newLocation)
    {
        value = newValue;
        location = newLocation;
    }
}

/** 
 * @author Chase Carthen
 * @brief This is the dataRecord class meant to hold any information downloaded from the virutal watershed.
 * @details The dataRecord class has specific data fields that are meant to be used by the Unity application.
 */
[Serializable]
public class DataRecord : IEquatable<DataRecord>
{
    public DataRecord()
    {
    	start = DateTime.MinValue;
    	end = DateTime.MaxValue;
    }
    
    public DataRecord( string recordName )
    {
        name = recordName;
    }
	public DataRecord Clone()
	{
        //DataRecord other = (DataRecord)this.MemberwiseClone();
        //return other;
		DataRecord dr = new DataRecord ();
		dr.name = name;
		dr.boundingBox = boundingBox;
		dr.bbox = bbox;
		dr.bbox2 = bbox2;
		dr.variableName = variableName;
		//dr.data = data;
		//dr.texture = texture.Clone ();
        dr.Max = Max;
        dr.Min = Min;
        dr.Mean = Mean;
        dr.WCSCap = WCSCap;
		dr.start = start;
		dr.end = end;
		dr.description = description;
		dr.modelname = modelname;
		dr.modelRunUUID = modelRunUUID;
		dr.id = id;
		dr.location = location;
		dr.services = services;
		dr.projection = projection;
		dr.WCSOperations = WCSOperations;
		dr.CoverageDescription = CoverageDescription;
		dr.WCSCoverages = WCSCoverages;
		dr.Identifier = Identifier;
		dr.Type = Type;
		dr.Temporal = Temporal;
        dr.numbands = numbands;
		return dr;
	}
	public WMS_CAPABILITIES.WMT_MS_CapabilitiesCapabilityLayerLayer[] wmslayers = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
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
               a.data == b.data && a.band_id == b.band_id;
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
				a.data != b.data || 
				a.band_id == b.band_id;
    }

    public bool Equals(DataRecord b)
    {
        return this == b;
    }

    // Copy Function: overwrited current data with data in src
    public bool copyFrom(DataRecord src)
    {
        // TODO
        return false;
    }

    /** These fields are necessary for generating the key used in datamanager datastructure. 
     *
     */
    // The ID, name, description and categories of this dataset or uuid.
    public string id;
    public string name;
    
    //****** Geospatial MetaData**/
    SerialVector2 resolution = Vector2.zero;

    public SerialVector2 Resolution
    {
        get
        {
            Debug.Log(resolution.x);
            Debug.Log(resolution == Vector2.zero);
            Debug.Log(data == null);
            if (resolution == Vector2.zero)
            {
                if (data == null)
                    return resolution;
                int Width = data[0].GetLength(0);
                int Height = data[0].GetLength(1);

                // boundingBox is LatLong
                float xOrigin = boundingBox.x;
                float yOrigin = boundingBox.y;
                float width = boundingBox.width;
                float height = boundingBox.height;

                // Time to set the resolution if possible

                // First convert these coordinates to UTM
                Vector2 origin = CoordinateUtils.transformToUTM(xOrigin, yOrigin);
                Vector2 lowerRightCorner = CoordinateUtils.transformToUTM(xOrigin + width, yOrigin - height);
                Debug.Log(origin);
                Debug.Log(lowerRightCorner);
                // Getting average resolution in meters due to the points being in UTM
                height = Math.Abs(origin.y - lowerRightCorner.y) / this.height;
                width = Math.Abs(origin.x - lowerRightCorner.x) / this.width;
                resolution = new Vector2(Math.Abs(origin.x - lowerRightCorner.x), Math.Abs(origin.y - lowerRightCorner.y));
                Debug.Log("RESOLUTION: " + resolution.x + " " + resolution.y);
            }
            return resolution;
        }
    }

    SerialRect utmBoundingBox = new SerialRect();
    public SerialRect UTMBoundingBox
    {
        get
        {
            // Calculating utmBoundingBox
            if (utmBoundingBox.x != 0 && utmBoundingBox.y != 0 && utmBoundingBox.width != 0 && utmBoundingBox.height != 0)
            {
                Vector2 UpperLeftCorner = CoordinateUtils.transformToUTM(boundingBox.x, boundingBox.y);
                Vector2 LowerRightCorner = CoordinateUtils.transformToUTM(boundingBox.x + boundingBox.width, boundingBox.y - boundingBox.height);
                utmBoundingBox = new SerialRect(new Rect(UpperLeftCorner.x, UpperLeftCorner.y, Math.Abs(UpperLeftCorner.x - LowerRightCorner.x), Math.Abs(UpperLeftCorner.y - LowerRightCorner.y)));
            }
            return utmBoundingBox;
        }
    }

    // This variable must be in Lat Long
    SerialVector2 WGS84Origin;

    // The bounding box of this dataset
    public string bbox = null; // --- In the dataset class there will be a floating point representation. ---
	public string bbox2 = null; // A temporary fix for WCS's BBOX!!!!!!
    // The EPSG of this dataset
    public string projection;

    public string model_set_type="";

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
    [NonSerializedAttribute]
    public GetCapabilites.Capabilities WCSCap = null;
	[NonSerializedAttribute]
    public GetCapabilites.OperationsMetadataOperation[] WCSOperations = null;
	[NonSerializedAttribute]
	public GetCapabilites.CapabilitiesContentsCoverageSummary[] WCSCoverages = null;
	[NonSerializedAttribute]
	public DescribeCoverageWCS.CoverageDescriptions CoverageDescription = null;
	
    public string WMSCapabilities = "";
    public string WFSCapabilities = "";
    public string WCSCapabilities = "";

    public string GDALPath = "";

    public Dictionary<string, string> services = new Dictionary<string,string>();

    // Description of data **/
    public string description;

    public string state;
    public string location;

    public string title;

    // Model Variables **/
    public string modelname="";
    public string modelRunUUID;
    public string variableName;

    // For the min and max of the record
    public float Min = float.MaxValue;
    public float Max = float.MinValue;
    //public ValueContainer MinContainer = new ValueContainer(float.MaxValue, Vector2.zero);
    //public ValueContainer MaxContainer = new ValueContainer(float.MinValue, Vector2.zero);
    public ValueContainer _MinContainer;
    public ValueContainer MinContainer
    {
        get
        {
            return _MinContainer;
        }
        set
        {
            _MinContainer = value;
            Min = value.value;
        }
    }

    public ValueContainer _MaxContainer;
    public ValueContainer MaxContainer
    {
        get
        {
            return _MaxContainer;
        }
        set
        {
            _MaxContainer = value;
            Max = value.value;
        }
    }


    public float Mean = 0;

	// Another patch to make things go faster
	public string multiLayered = "none";
	
    // Temporal Fields for metadata 
    public DateTime? start = null;
    public DateTime? end = null;
	public int numbands = 1;
	
	// A band_id of negative one means it is not part of a series.
	public int band_id = -1;
	public bool Temporal = false;
	public bool IsTemporal()
	{
		return Temporal;
	}

    // A test set flag for one variable tests
    public bool isSet = false;

    /*
     * Metadata fields
     */
    // This will hold any metadata... -- This will include the height and width of the dataset.
    [NonSerializedAttribute]
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
    List<float[,]> data = new List<float[,]>();
    public List<float[,]> Data
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
    public string Type;


    // Temporary parameters.
    public int width;
    public int height;

    // Use these to rebuild bounding box
    public float xBound, yBound, xwidth, yheight;

    public string Identifier=null;
    public string Layer = "";

};
