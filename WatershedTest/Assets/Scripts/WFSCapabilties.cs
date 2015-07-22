using UnityEngine;
using System.Collections;

namespace WFS_CAPABILITIES_SERVICE
{
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.opengis.net/wfs", IsNullable = false)]
	public partial class WFS_Capabilities
	{
		
		private WFS_CapabilitiesService serviceField;
		
		private WFS_CapabilitiesCapability capabilityField;
		
		private WFS_CapabilitiesFeatureTypeList featureTypeListField;
		
		//private Filter_Capabilities filter_CapabilitiesField;
		
		private string versionField;
		
		private byte updateSequenceField;
		
		/// <remarks/>
		public WFS_CapabilitiesService Service
		{
			get
			{
				return this.serviceField;
			}
			set
			{
				this.serviceField = value;
			}
		}
		
		/// <remarks/>
		public WFS_CapabilitiesCapability Capability
		{
			get
			{
				return this.capabilityField;
			}
			set
			{
				this.capabilityField = value;
			}
		}
		
		/// <remarks/>
		public WFS_CapabilitiesFeatureTypeList FeatureTypeList
		{
			get
			{
				return this.featureTypeListField;
			}
			set
			{
				this.featureTypeListField = value;
			}
		}
		
		/// <remarks/>
		/*[System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.opengis.net/ogc")]
		public Filter_Capabilities Filter_Capabilities
		{
			get
			{
				return this.filter_CapabilitiesField;
			}
			set
			{
				this.filter_CapabilitiesField = value;
			}
		}*/
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public byte updateSequence
		{
			get
			{
				return this.updateSequenceField;
			}
			set
			{
				this.updateSequenceField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesService
	{
		
		private string nameField;
		
		private string titleField;
		
		private string abstractField;
		
		private string keywordsField;
		
		private string onlineResourceField;
		
		private string feesField;
		
		private string accessConstraintsField;
		
		/// <remarks/>
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}
		
		/// <remarks/>
		public string Title
		{
			get
			{
				return this.titleField;
			}
			set
			{
				this.titleField = value;
			}
		}
		
		/// <remarks/>
		public string Abstract
		{
			get
			{
				return this.abstractField;
			}
			set
			{
				this.abstractField = value;
			}
		}
		
		/// <remarks/>
		public string Keywords
		{
			get
			{
				return this.keywordsField;
			}
			set
			{
				this.keywordsField = value;
			}
		}
		
		/// <remarks/>
		public string OnlineResource
		{
			get
			{
				return this.onlineResourceField;
			}
			set
			{
				this.onlineResourceField = value;
			}
		}
		
		/// <remarks/>
		public string Fees
		{
			get
			{
				return this.feesField;
			}
			set
			{
				this.feesField = value;
			}
		}
		
		/// <remarks/>
		public string AccessConstraints
		{
			get
			{
				return this.accessConstraintsField;
			}
			set
			{
				this.accessConstraintsField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapability
	{
		
		private WFS_CapabilitiesCapabilityRequest requestField;
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequest Request
		{
			get
			{
				return this.requestField;
			}
			set
			{
				this.requestField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequest
	{
		
		private WFS_CapabilitiesCapabilityRequestDCPType[] getCapabilitiesField;
		
		private WFS_CapabilitiesCapabilityRequestDescribeFeatureType describeFeatureTypeField;
		
		private WFS_CapabilitiesCapabilityRequestGetFeature getFeatureField;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItemAttribute("DCPType", IsNullable = false)]
		public WFS_CapabilitiesCapabilityRequestDCPType[] GetCapabilities
		{
			get
			{
				return this.getCapabilitiesField;
			}
			set
			{
				this.getCapabilitiesField = value;
			}
		}
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestDescribeFeatureType DescribeFeatureType
		{
			get
			{
				return this.describeFeatureTypeField;
			}
			set
			{
				this.describeFeatureTypeField = value;
			}
		}
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestGetFeature GetFeature
		{
			get
			{
				return this.getFeatureField;
			}
			set
			{
				this.getFeatureField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDCPType
	{
		
		private WFS_CapabilitiesCapabilityRequestDCPTypeHTTP hTTPField;
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestDCPTypeHTTP HTTP
		{
			get
			{
				return this.hTTPField;
			}
			set
			{
				this.hTTPField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDCPTypeHTTP
	{
		
		private WFS_CapabilitiesCapabilityRequestDCPTypeHTTPPost postField;
		
		private WFS_CapabilitiesCapabilityRequestDCPTypeHTTPGet getField;
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestDCPTypeHTTPPost Post
		{
			get
			{
				return this.postField;
			}
			set
			{
				this.postField = value;
			}
		}
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestDCPTypeHTTPGet Get
		{
			get
			{
				return this.getField;
			}
			set
			{
				this.getField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDCPTypeHTTPPost
	{
		
		private string onlineResourceField;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string onlineResource
		{
			get
			{
				return this.onlineResourceField;
			}
			set
			{
				this.onlineResourceField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDCPTypeHTTPGet
	{
		
		private string onlineResourceField;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string onlineResource
		{
			get
			{
				return this.onlineResourceField;
			}
			set
			{
				this.onlineResourceField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDescribeFeatureType
	{
		
		private WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeSchemaDescriptionLanguage schemaDescriptionLanguageField;
		
		private WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPType[] dCPTypeField;
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeSchemaDescriptionLanguage SchemaDescriptionLanguage
		{
			get
			{
				return this.schemaDescriptionLanguageField;
			}
			set
			{
				this.schemaDescriptionLanguageField = value;
			}
		}
		
		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("DCPType")]
		public WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPType[] DCPType
		{
			get
			{
				return this.dCPTypeField;
			}
			set
			{
				this.dCPTypeField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeSchemaDescriptionLanguage
	{
		
		private object xMLSCHEMAField;
		
		/// <remarks/>
		public object XMLSCHEMA
		{
			get
			{
				return this.xMLSCHEMAField;
			}
			set
			{
				this.xMLSCHEMAField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPType
	{
		
		private WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPTypeHTTP hTTPField;
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPTypeHTTP HTTP
		{
			get
			{
				return this.hTTPField;
			}
			set
			{
				this.hTTPField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPTypeHTTP
	{
		
		private WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPTypeHTTPPost postField;
		
		private WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPTypeHTTPGet getField;
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPTypeHTTPPost Post
		{
			get
			{
				return this.postField;
			}
			set
			{
				this.postField = value;
			}
		}
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPTypeHTTPGet Get
		{
			get
			{
				return this.getField;
			}
			set
			{
				this.getField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPTypeHTTPPost
	{
		
		private string onlineResourceField;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string onlineResource
		{
			get
			{
				return this.onlineResourceField;
			}
			set
			{
				this.onlineResourceField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestDescribeFeatureTypeDCPTypeHTTPGet
	{
		
		private string onlineResourceField;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string onlineResource
		{
			get
			{
				return this.onlineResourceField;
			}
			set
			{
				this.onlineResourceField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestGetFeature
	{
		
		private WFS_CapabilitiesCapabilityRequestGetFeatureResultFormat resultFormatField;
		
		private WFS_CapabilitiesCapabilityRequestGetFeatureDCPType[] dCPTypeField;
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestGetFeatureResultFormat ResultFormat
		{
			get
			{
				return this.resultFormatField;
			}
			set
			{
				this.resultFormatField = value;
			}
		}
		
		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("DCPType")]
		public WFS_CapabilitiesCapabilityRequestGetFeatureDCPType[] DCPType
		{
			get
			{
				return this.dCPTypeField;
			}
			set
			{
				this.dCPTypeField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestGetFeatureResultFormat
	{
		
		private object gML2Field;
		
		/// <remarks/>
		public object GML2
		{
			get
			{
				return this.gML2Field;
			}
			set
			{
				this.gML2Field = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestGetFeatureDCPType
	{
		
		private WFS_CapabilitiesCapabilityRequestGetFeatureDCPTypeHTTP hTTPField;
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestGetFeatureDCPTypeHTTP HTTP
		{
			get
			{
				return this.hTTPField;
			}
			set
			{
				this.hTTPField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestGetFeatureDCPTypeHTTP
	{
		
		private WFS_CapabilitiesCapabilityRequestGetFeatureDCPTypeHTTPPost postField;
		
		private WFS_CapabilitiesCapabilityRequestGetFeatureDCPTypeHTTPGet getField;
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestGetFeatureDCPTypeHTTPPost Post
		{
			get
			{
				return this.postField;
			}
			set
			{
				this.postField = value;
			}
		}
		
		/// <remarks/>
		public WFS_CapabilitiesCapabilityRequestGetFeatureDCPTypeHTTPGet Get
		{
			get
			{
				return this.getField;
			}
			set
			{
				this.getField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestGetFeatureDCPTypeHTTPPost
	{
		
		private string onlineResourceField;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string onlineResource
		{
			get
			{
				return this.onlineResourceField;
			}
			set
			{
				this.onlineResourceField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesCapabilityRequestGetFeatureDCPTypeHTTPGet
	{
		
		private string onlineResourceField;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string onlineResource
		{
			get
			{
				return this.onlineResourceField;
			}
			set
			{
				this.onlineResourceField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesFeatureTypeList
	{
		
		private WFS_CapabilitiesFeatureTypeListOperations operationsField;
		
		private WFS_CapabilitiesFeatureTypeListFeatureType featureTypeField;
		
		/// <remarks/>
		public WFS_CapabilitiesFeatureTypeListOperations Operations
		{
			get
			{
				return this.operationsField;
			}
			set
			{
				this.operationsField = value;
			}
		}
		
		/// <remarks/>

		public WFS_CapabilitiesFeatureTypeListFeatureType FeatureType
		{
			get
			{
				return this.featureTypeField;
			}
			set
			{
				this.featureTypeField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesFeatureTypeListOperations
	{
		
		private object queryField;
		
		/// <remarks/>
		public object Query
		{
			get
			{
				return this.queryField;
			}
			set
			{
				this.queryField = value;
			}
		}
	}
	
	/// <remarks/>
	/// [System.Xml.Serialization.XmlArrayItemAttribute("Keyword", IsNullable = false)]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesFeatureTypeListFeatureType
	{
		
		private string nameField;
		
		private string titleField;
		
		private string abstractField;
		
		private object keywordsField;
		
		private string sRSField;
		
		private WFS_CapabilitiesFeatureTypeListFeatureTypeLatLongBoundingBox latLongBoundingBoxField;
		
		private WFS_CapabilitiesFeatureTypeListFeatureTypeMetadataURL metadataURLField;
		
		/// <remarks/>
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}
		
		/// <remarks/>
		public string Title
		{
			get
			{
				return this.titleField;
			}
			set
			{
				this.titleField = value;
			}
		}
		
		/// <remarks/>
		public string Abstract
		{
			get
			{
				return this.abstractField;
			}
			set
			{
				this.abstractField = value;
			}
		}
		
		/// <remarks/>
		public object Keywords
		{
			get
			{
				return this.keywordsField;
			}
			set
			{
				this.keywordsField = value;
			}
		}
		
		/// <remarks/>
		public string SRS
		{
			get
			{
				return this.sRSField;
			}
			set
			{
				this.sRSField = value;
			}
		}
		
		/// <remarks/>
		public WFS_CapabilitiesFeatureTypeListFeatureTypeLatLongBoundingBox LatLongBoundingBox
		{
			get
			{
				return this.latLongBoundingBoxField;
			}
			set
			{
				this.latLongBoundingBoxField = value;
			}
		}
		
		/// <remarks/>
		public WFS_CapabilitiesFeatureTypeListFeatureTypeMetadataURL MetadataURL
		{
			get
			{
				return this.metadataURLField;
			}
			set
			{
				this.metadataURLField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesFeatureTypeListFeatureTypeLatLongBoundingBox
	{
		
		private decimal minxField;
		
		private decimal minyField;
		
		private decimal maxxField;
		
		private decimal maxyField;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public decimal minx
		{
			get
			{
				return this.minxField;
			}
			set
			{
				this.minxField = value;
			}
		}
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public decimal miny
		{
			get
			{
				return this.minyField;
			}
			set
			{
				this.minyField = value;
			}
		}
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public decimal maxx
		{
			get
			{
				return this.maxxField;
			}
			set
			{
				this.maxxField = value;
			}
		}
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public decimal maxy
		{
			get
			{
				return this.maxyField;
			}
			set
			{
				this.maxyField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/wfs")]
	public partial class WFS_CapabilitiesFeatureTypeListFeatureTypeMetadataURL
	{
		
		private string typeField;
		
		private string formatField;
		
		private string valueField;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string format
		{
			get
			{
				return this.formatField;
			}
			set
			{
				this.formatField = value;
			}
		}
		
		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/ogc")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
	public partial class Filter_Capabilities
	{
		
		private Filter_CapabilitiesSpatial_Capabilities spatial_CapabilitiesField;
		
		private Filter_CapabilitiesScalar_Capabilities scalar_CapabilitiesField;
		
		/// <remarks/>
		public Filter_CapabilitiesSpatial_Capabilities Spatial_Capabilities
		{
			get
			{
				return this.spatial_CapabilitiesField;
			}
			set
			{
				this.spatial_CapabilitiesField = value;
			}
		}
		
		/// <remarks/>
		public Filter_CapabilitiesScalar_Capabilities Scalar_Capabilities
		{
			get
			{
				return this.scalar_CapabilitiesField;
			}
			set
			{
				this.scalar_CapabilitiesField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/ogc")]
	public partial class Filter_CapabilitiesSpatial_Capabilities
	{
		
		private Filter_CapabilitiesSpatial_CapabilitiesSpatial_Operators spatial_OperatorsField;
		
		/// <remarks/>
		public Filter_CapabilitiesSpatial_CapabilitiesSpatial_Operators Spatial_Operators
		{
			get
			{
				return this.spatial_OperatorsField;
			}
			set
			{
				this.spatial_OperatorsField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/ogc")]
	public partial class Filter_CapabilitiesSpatial_CapabilitiesSpatial_Operators
	{
		
		private object equalsField;
		
		private object disjointField;
		
		private object touchesField;
		
		private object withinField;
		
		private object overlapsField;
		
		private object crossesField;
		
		private object intersectField;
		
		private object containsField;
		
		private object dWithinField;
		
		private object bBOXField;
		
		/// <remarks/>
		public object Equals
		{
			get
			{
				return this.equalsField;
			}
			set
			{
				this.equalsField = value;
			}
		}
		
		/// <remarks/>
		public object Disjoint
		{
			get
			{
				return this.disjointField;
			}
			set
			{
				this.disjointField = value;
			}
		}
		
		/// <remarks/>
		public object Touches
		{
			get
			{
				return this.touchesField;
			}
			set
			{
				this.touchesField = value;
			}
		}
		
		/// <remarks/>
		public object Within
		{
			get
			{
				return this.withinField;
			}
			set
			{
				this.withinField = value;
			}
		}
		
		/// <remarks/>
		public object Overlaps
		{
			get
			{
				return this.overlapsField;
			}
			set
			{
				this.overlapsField = value;
			}
		}
		
		/// <remarks/>
		public object Crosses
		{
			get
			{
				return this.crossesField;
			}
			set
			{
				this.crossesField = value;
			}
		}
		
		/// <remarks/>
		public object Intersect
		{
			get
			{
				return this.intersectField;
			}
			set
			{
				this.intersectField = value;
			}
		}
		
		/// <remarks/>
		public object Contains
		{
			get
			{
				return this.containsField;
			}
			set
			{
				this.containsField = value;
			}
		}
		
		/// <remarks/>
		public object DWithin
		{
			get
			{
				return this.dWithinField;
			}
			set
			{
				this.dWithinField = value;
			}
		}
		
		/// <remarks/>
		public object BBOX
		{
			get
			{
				return this.bBOXField;
			}
			set
			{
				this.bBOXField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/ogc")]
	public partial class Filter_CapabilitiesScalar_Capabilities
	{
		
		private object logical_OperatorsField;
		
		private Filter_CapabilitiesScalar_CapabilitiesComparison_Operators comparison_OperatorsField;
		
		/// <remarks/>
		public object Logical_Operators
		{
			get
			{
				return this.logical_OperatorsField;
			}
			set
			{
				this.logical_OperatorsField = value;
			}
		}
		
		/// <remarks/>
		public Filter_CapabilitiesScalar_CapabilitiesComparison_Operators Comparison_Operators
		{
			get
			{
				return this.comparison_OperatorsField;
			}
			set
			{
				this.comparison_OperatorsField = value;
			}
		}
	}
	
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/ogc")]
	public partial class Filter_CapabilitiesScalar_CapabilitiesComparison_Operators
	{
		
		private object simple_ComparisonsField;
		
		private object likeField;
		
		private object betweenField;
		
		/// <remarks/>
		public object Simple_Comparisons
		{
			get
			{
				return this.simple_ComparisonsField;
			}
			set
			{
				this.simple_ComparisonsField = value;
			}
		}
		
		/// <remarks/>
		public object Like
		{
			get
			{
				return this.likeField;
			}
			set
			{
				this.likeField = value;
			}
		}
		
		/// <remarks/>
		public object Between
		{
			get
			{
				return this.betweenField;
			}
			set
			{
				this.betweenField = value;
			}
		}
	}
}
