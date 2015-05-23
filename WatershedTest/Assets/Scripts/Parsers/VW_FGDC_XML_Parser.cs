using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// VW_FGDC_XML_Parser is used to parse FGDC XML data records from the virtual watershed.
/// </summary>
class VW_FGDC_XML_Parser : Parser
{
    public override DataRecord Parse(DataRecord record, byte[] Contents)
    {
        var reader = System.Xml.XmlTextReader.Create(new System.IO.MemoryStream(Contents));

        Dictionary<string, string> temp;

        XmlSerializer serial = new XmlSerializer(typeof(metadata));
        metadata metaData = new metadata();

        if (serial.CanDeserialize(reader))
        {
            try
            {
                record.metaData = (metadata)serial.Deserialize(reader);
            }
            catch (System.Exception e)
            {
                Logger.WriteLine(e.Message);
            }
        }
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

// The following below should go in another class.
/// Visual Studio Generated Class..... for the metadata
/// <remarks/>
[Serializable]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class metadata
{

    private metadataIdinfo idinfoField;

    private metadataDataqual dataqualField;

    private metadataSpdoinfo spdoinfoField;

    private metadataSpref sprefField;

    private metadataEainfo eainfoField;

    private metadataDistinfo distinfoField;

    private metadataMetainfo metainfoField;

    /// <remarks/>
    public metadataIdinfo idinfo
    {
        get
        {
            return this.idinfoField;
        }
        set
        {
            this.idinfoField = value;
        }
    }

    /// <remarks/>
    public metadataDataqual dataqual
    {
        get
        {
            return this.dataqualField;
        }
        set
        {
            this.dataqualField = value;
        }
    }

    /// <remarks/>
    public metadataSpdoinfo spdoinfo
    {
        get
        {
            return this.spdoinfoField;
        }
        set
        {
            this.spdoinfoField = value;
        }
    }

    /// <remarks/>
    public metadataSpref spref
    {
        get
        {
            return this.sprefField;
        }
        set
        {
            this.sprefField = value;
        }
    }

    /// <remarks/>
    public metadataEainfo eainfo
    {
        get
        {
            return this.eainfoField;
        }
        set
        {
            this.eainfoField = value;
        }
    }

    /// <remarks/>
    public metadataDistinfo distinfo
    {
        get
        {
            return this.distinfoField;
        }
        set
        {
            this.distinfoField = value;
        }
    }

    /// <remarks/>
    public metadataMetainfo metainfo
    {
        get
        {
            return this.metainfoField;
        }
        set
        {
            this.metainfoField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfo
{

    private object datsetidField;

    private metadataIdinfoCitation citationField;

    private metadataIdinfoDescript descriptField;

    private metadataIdinfoTimeperd timeperdField;

    private metadataIdinfoStatus statusField;

    private metadataIdinfoSpdom spdomField;

    private metadataIdinfoKeywords keywordsField;

    private string accconstField;

    private string useconstField;

    private metadataIdinfoPtcontac ptcontacField;

    private string nativeField;

    /// <remarks/>
    public object datsetid
    {
        get
        {
            return this.datsetidField;
        }
        set
        {
            this.datsetidField = value;
        }
    }

    /// <remarks/>
    public metadataIdinfoCitation citation
    {
        get
        {
            return this.citationField;
        }
        set
        {
            this.citationField = value;
        }
    }

    /// <remarks/>
    public metadataIdinfoDescript descript
    {
        get
        {
            return this.descriptField;
        }
        set
        {
            this.descriptField = value;
        }
    }

    /// <remarks/>
    public metadataIdinfoTimeperd timeperd
    {
        get
        {
            return this.timeperdField;
        }
        set
        {
            this.timeperdField = value;
        }
    }

    /// <remarks/>
    public metadataIdinfoStatus status
    {
        get
        {
            return this.statusField;
        }
        set
        {
            this.statusField = value;
        }
    }

    /// <remarks/>
    public metadataIdinfoSpdom spdom
    {
        get
        {
            return this.spdomField;
        }
        set
        {
            this.spdomField = value;
        }
    }

    /// <remarks/>
    public metadataIdinfoKeywords keywords
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
    public string accconst
    {
        get
        {
            return this.accconstField;
        }
        set
        {
            this.accconstField = value;
        }
    }

    /// <remarks/>
    public string useconst
    {
        get
        {
            return this.useconstField;
        }
        set
        {
            this.useconstField = value;
        }
    }

    /// <remarks/>
    public metadataIdinfoPtcontac ptcontac
    {
        get
        {
            return this.ptcontacField;
        }
        set
        {
            this.ptcontacField = value;
        }
    }

    /// <remarks/>
    public string native
    {
        get
        {
            return this.nativeField;
        }
        set
        {
            this.nativeField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoCitation
{

    private metadataIdinfoCitationCiteinfo citeinfoField;

    /// <remarks/>
    public metadataIdinfoCitationCiteinfo citeinfo
    {
        get
        {
            return this.citeinfoField;
        }
        set
        {
            this.citeinfoField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoCitationCiteinfo
{

    private string originField;

    private uint pubdateField;

    private string titleField;

    private string geoformField;

    private metadataIdinfoCitationCiteinfoPubinfo pubinfoField;

    private string[] onlinkField;

    /// <remarks/>
    public string origin
    {
        get
        {
            return this.originField;
        }
        set
        {
            this.originField = value;
        }
    }

    /// <remarks/>
    public uint pubdate
    {
        get
        {
            return this.pubdateField;
        }
        set
        {
            this.pubdateField = value;
        }
    }

    /// <remarks/>
    public string title
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
    public string geoform
    {
        get
        {
            return this.geoformField;
        }
        set
        {
            this.geoformField = value;
        }
    }

    /// <remarks/>
    public metadataIdinfoCitationCiteinfoPubinfo pubinfo
    {
        get
        {
            return this.pubinfoField;
        }
        set
        {
            this.pubinfoField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("onlink")]
    public string[] onlink
    {
        get
        {
            return this.onlinkField;
        }
        set
        {
            this.onlinkField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoCitationCiteinfoPubinfo
{

    private string pubplaceField;

    private string publishField;

    /// <remarks/>
    public string pubplace
    {
        get
        {
            return this.pubplaceField;
        }
        set
        {
            this.pubplaceField = value;
        }
    }

    /// <remarks/>
    public string publish
    {
        get
        {
            return this.publishField;
        }
        set
        {
            this.publishField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoDescript
{

    private string abstractField;

    private string purposeField;

    private string supplinfField;

    /// <remarks/>
    public string @abstract
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
    public string purpose
    {
        get
        {
            return this.purposeField;
        }
        set
        {
            this.purposeField = value;
        }
    }

    /// <remarks/>
    public string supplinf
    {
        get
        {
            return this.supplinfField;
        }
        set
        {
            this.supplinfField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoTimeperd
{

    private metadataIdinfoTimeperdTimeinfo timeinfoField;

    private string currentField;

    /// <remarks/>
    public metadataIdinfoTimeperdTimeinfo timeinfo
    {
        get
        {
            return this.timeinfoField;
        }
        set
        {
            this.timeinfoField = value;
        }
    }

    /// <remarks/>
    public string current
    {
        get
        {
            return this.currentField;
        }
        set
        {
            this.currentField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoTimeperdTimeinfo
{

    private metadataIdinfoTimeperdTimeinfoRngdates rngdatesField;

    /// <remarks/>
    public metadataIdinfoTimeperdTimeinfoRngdates rngdates
    {
        get
        {
            return this.rngdatesField;
        }
        set
        {
            this.rngdatesField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoTimeperdTimeinfoRngdates
{

    private uint begdateField;

    private uint enddateField;

    /// <remarks/>
    public uint begdate
    {
        get
        {
            return this.begdateField;
        }
        set
        {
            this.begdateField = value;
        }
    }

    /// <remarks/>
    public uint enddate
    {
        get
        {
            return this.enddateField;
        }
        set
        {
            this.enddateField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoStatus
{

    private object progressField;

    private string updateField;

    /// <remarks/>
    public object progress
    {
        get
        {
            return this.progressField;
        }
        set
        {
            this.progressField = value;
        }
    }

    /// <remarks/>
    public string update
    {
        get
        {
            return this.updateField;
        }
        set
        {
            this.updateField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoSpdom
{

    private metadataIdinfoSpdomBounding boundingField;

    /// <remarks/>
    public metadataIdinfoSpdomBounding bounding
    {
        get
        {
            return this.boundingField;
        }
        set
        {
            this.boundingField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoSpdomBounding
{

    private decimal westbcField;

    private decimal eastbcField;

    private decimal northbcField;

    private decimal southbcField;

    /// <remarks/>
    public decimal westbc
    {
        get
        {
            return this.westbcField;
        }
        set
        {
            this.westbcField = value;
        }
    }

    /// <remarks/>
    public decimal eastbc
    {
        get
        {
            return this.eastbcField;
        }
        set
        {
            this.eastbcField = value;
        }
    }

    /// <remarks/>
    public decimal northbc
    {
        get
        {
            return this.northbcField;
        }
        set
        {
            this.northbcField = value;
        }
    }

    /// <remarks/>
    public decimal southbc
    {
        get
        {
            return this.southbcField;
        }
        set
        {
            this.southbcField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoKeywords
{

    private metadataIdinfoKeywordsTheme[] themeField;

    private metadataIdinfoKeywordsPlace[] placeField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("theme")]
    public metadataIdinfoKeywordsTheme[] theme
    {
        get
        {
            return this.themeField;
        }
        set
        {
            this.themeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("place")]
    public metadataIdinfoKeywordsPlace[] place
    {
        get
        {
            return this.placeField;
        }
        set
        {
            this.placeField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoKeywordsTheme
{

    private string themektField;

    private string[] themekeyField;

    /// <remarks/>
    public string themekt
    {
        get
        {
            return this.themektField;
        }
        set
        {
            this.themektField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("themekey")]
    public string[] themekey
    {
        get
        {
            return this.themekeyField;
        }
        set
        {
            this.themekeyField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoKeywordsPlace
{

    private string placektField;

    private string[] placekeyField;

    /// <remarks/>
    public string placekt
    {
        get
        {
            return this.placektField;
        }
        set
        {
            this.placektField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("placekey")]
    public string[] placekey
    {
        get
        {
            return this.placekeyField;
        }
        set
        {
            this.placekeyField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoPtcontac
{

    private metadataIdinfoPtcontacCntinfo cntinfoField;

    /// <remarks/>
    public metadataIdinfoPtcontacCntinfo cntinfo
    {
        get
        {
            return this.cntinfoField;
        }
        set
        {
            this.cntinfoField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoPtcontacCntinfo
{

    private metadataIdinfoPtcontacCntinfoCntperp cntperpField;

    private metadataIdinfoPtcontacCntinfoCntaddr cntaddrField;

    private string cntvoiceField;

    private string cntemailField;

    /// <remarks/>
    public metadataIdinfoPtcontacCntinfoCntperp cntperp
    {
        get
        {
            return this.cntperpField;
        }
        set
        {
            this.cntperpField = value;
        }
    }

    /// <remarks/>
    public metadataIdinfoPtcontacCntinfoCntaddr cntaddr
    {
        get
        {
            return this.cntaddrField;
        }
        set
        {
            this.cntaddrField = value;
        }
    }

    /// <remarks/>
    public string cntvoice
    {
        get
        {
            return this.cntvoiceField;
        }
        set
        {
            this.cntvoiceField = value;
        }
    }

    /// <remarks/>
    public string cntemail
    {
        get
        {
            return this.cntemailField;
        }
        set
        {
            this.cntemailField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoPtcontacCntinfoCntperp
{

    private string cntperField;

    private object cntorgField;

    /// <remarks/>
    public string cntper
    {
        get
        {
            return this.cntperField;
        }
        set
        {
            this.cntperField = value;
        }
    }

    /// <remarks/>
    public object cntorg
    {
        get
        {
            return this.cntorgField;
        }
        set
        {
            this.cntorgField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataIdinfoPtcontacCntinfoCntaddr
{

    private string addrtypeField;

    private string addressField;

    private string cityField;

    private string stateField;

    private uint postalField;

    private string countryField;

    /// <remarks/>
    public string addrtype
    {
        get
        {
            return this.addrtypeField;
        }
        set
        {
            this.addrtypeField = value;
        }
    }

    /// <remarks/>
    public string address
    {
        get
        {
            return this.addressField;
        }
        set
        {
            this.addressField = value;
        }
    }

    /// <remarks/>
    public string city
    {
        get
        {
            return this.cityField;
        }
        set
        {
            this.cityField = value;
        }
    }

    /// <remarks/>
    public string state
    {
        get
        {
            return this.stateField;
        }
        set
        {
            this.stateField = value;
        }
    }

    /// <remarks/>
    public uint postal
    {
        get
        {
            return this.postalField;
        }
        set
        {
            this.postalField = value;
        }
    }

    /// <remarks/>
    public string country
    {
        get
        {
            return this.countryField;
        }
        set
        {
            this.countryField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqual
{

    private string logicField;

    private string completeField;

    private metadataDataqualPosacc posaccField;

    private metadataDataqualLineage lineageField;

    /// <remarks/>
    public string logic
    {
        get
        {
            return this.logicField;
        }
        set
        {
            this.logicField = value;
        }
    }

    /// <remarks/>
    public string complete
    {
        get
        {
            return this.completeField;
        }
        set
        {
            this.completeField = value;
        }
    }

    /// <remarks/>
    public metadataDataqualPosacc posacc
    {
        get
        {
            return this.posaccField;
        }
        set
        {
            this.posaccField = value;
        }
    }

    /// <remarks/>
    public metadataDataqualLineage lineage
    {
        get
        {
            return this.lineageField;
        }
        set
        {
            this.lineageField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualPosacc
{

    private metadataDataqualPosaccHorizpa horizpaField;

    /// <remarks/>
    public metadataDataqualPosaccHorizpa horizpa
    {
        get
        {
            return this.horizpaField;
        }
        set
        {
            this.horizpaField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualPosaccHorizpa
{

    private string horizparField;

    private metadataDataqualPosaccHorizpaQhorizpa qhorizpaField;

    /// <remarks/>
    public string horizpar
    {
        get
        {
            return this.horizparField;
        }
        set
        {
            this.horizparField = value;
        }
    }

    /// <remarks/>
    public metadataDataqualPosaccHorizpaQhorizpa qhorizpa
    {
        get
        {
            return this.qhorizpaField;
        }
        set
        {
            this.qhorizpaField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualPosaccHorizpaQhorizpa
{

    private string horizpavField;

    private string horizpaeField;

    /// <remarks/>
    public string horizpav
    {
        get
        {
            return this.horizpavField;
        }
        set
        {
            this.horizpavField = value;
        }
    }

    /// <remarks/>
    public string horizpae
    {
        get
        {
            return this.horizpaeField;
        }
        set
        {
            this.horizpaeField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineage
{

    private metadataDataqualLineageSrcinfo srcinfoField;

    private metadataDataqualLineageProcstep procstepField;

    /// <remarks/>
    public metadataDataqualLineageSrcinfo srcinfo
    {
        get
        {
            return this.srcinfoField;
        }
        set
        {
            this.srcinfoField = value;
        }
    }

    /// <remarks/>
    public metadataDataqualLineageProcstep procstep
    {
        get
        {
            return this.procstepField;
        }
        set
        {
            this.procstepField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageSrcinfo
{

    private metadataDataqualLineageSrcinfoSrccite srcciteField;

    private string typesrcField;

    private metadataDataqualLineageSrcinfoSrctime srctimeField;

    private string srcciteaField;

    private string srccontrField;

    /// <remarks/>
    public metadataDataqualLineageSrcinfoSrccite srccite
    {
        get
        {
            return this.srcciteField;
        }
        set
        {
            this.srcciteField = value;
        }
    }

    /// <remarks/>
    public string typesrc
    {
        get
        {
            return this.typesrcField;
        }
        set
        {
            this.typesrcField = value;
        }
    }

    /// <remarks/>
    public metadataDataqualLineageSrcinfoSrctime srctime
    {
        get
        {
            return this.srctimeField;
        }
        set
        {
            this.srctimeField = value;
        }
    }

    /// <remarks/>
    public string srccitea
    {
        get
        {
            return this.srcciteaField;
        }
        set
        {
            this.srcciteaField = value;
        }
    }

    /// <remarks/>
    public string srccontr
    {
        get
        {
            return this.srccontrField;
        }
        set
        {
            this.srccontrField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageSrcinfoSrccite
{

    private metadataDataqualLineageSrcinfoSrcciteCiteinfo citeinfoField;

    /// <remarks/>
    public metadataDataqualLineageSrcinfoSrcciteCiteinfo citeinfo
    {
        get
        {
            return this.citeinfoField;
        }
        set
        {
            this.citeinfoField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageSrcinfoSrcciteCiteinfo
{

    private string originField;

    private ushort pubdateField;

    private string titleField;

    private metadataDataqualLineageSrcinfoSrcciteCiteinfoPubinfo pubinfoField;

    /// <remarks/>
    public string origin
    {
        get
        {
            return this.originField;
        }
        set
        {
            this.originField = value;
        }
    }

    /// <remarks/>
    public ushort pubdate
    {
        get
        {
            return this.pubdateField;
        }
        set
        {
            this.pubdateField = value;
        }
    }

    /// <remarks/>
    public string title
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
    public metadataDataqualLineageSrcinfoSrcciteCiteinfoPubinfo pubinfo
    {
        get
        {
            return this.pubinfoField;
        }
        set
        {
            this.pubinfoField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageSrcinfoSrcciteCiteinfoPubinfo
{

    private string pubplaceField;

    private string publishField;

    /// <remarks/>
    public string pubplace
    {
        get
        {
            return this.pubplaceField;
        }
        set
        {
            this.pubplaceField = value;
        }
    }

    /// <remarks/>
    public string publish
    {
        get
        {
            return this.publishField;
        }
        set
        {
            this.publishField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageSrcinfoSrctime
{

    private metadataDataqualLineageSrcinfoSrctimeTimeinfo timeinfoField;

    private string srccurrField;

    /// <remarks/>
    public metadataDataqualLineageSrcinfoSrctimeTimeinfo timeinfo
    {
        get
        {
            return this.timeinfoField;
        }
        set
        {
            this.timeinfoField = value;
        }
    }

    /// <remarks/>
    public string srccurr
    {
        get
        {
            return this.srccurrField;
        }
        set
        {
            this.srccurrField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageSrcinfoSrctimeTimeinfo
{

    private metadataDataqualLineageSrcinfoSrctimeTimeinfoRngdates rngdatesField;

    /// <remarks/>
    public metadataDataqualLineageSrcinfoSrctimeTimeinfoRngdates rngdates
    {
        get
        {
            return this.rngdatesField;
        }
        set
        {
            this.rngdatesField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageSrcinfoSrctimeTimeinfoRngdates
{

    private uint begdateField;

    private uint enddateField;

    /// <remarks/>
    public uint begdate
    {
        get
        {
            return this.begdateField;
        }
        set
        {
            this.begdateField = value;
        }
    }

    /// <remarks/>
    public uint enddate
    {
        get
        {
            return this.enddateField;
        }
        set
        {
            this.enddateField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageProcstep
{

    private string procdescField;

    private uint procdateField;

    private metadataDataqualLineageProcstepProccont proccontField;

    /// <remarks/>
    public string procdesc
    {
        get
        {
            return this.procdescField;
        }
        set
        {
            this.procdescField = value;
        }
    }

    /// <remarks/>
    public uint procdate
    {
        get
        {
            return this.procdateField;
        }
        set
        {
            this.procdateField = value;
        }
    }

    /// <remarks/>
    public metadataDataqualLineageProcstepProccont proccont
    {
        get
        {
            return this.proccontField;
        }
        set
        {
            this.proccontField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageProcstepProccont
{

    private metadataDataqualLineageProcstepProccontCntinfo cntinfoField;

    /// <remarks/>
    public metadataDataqualLineageProcstepProccontCntinfo cntinfo
    {
        get
        {
            return this.cntinfoField;
        }
        set
        {
            this.cntinfoField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageProcstepProccontCntinfo
{

    private metadataDataqualLineageProcstepProccontCntinfoCntorgp cntorgpField;

    private string cntposField;

    private metadataDataqualLineageProcstepProccontCntinfoCntaddr cntaddrField;

    private string cntvoiceField;

    private string cntfaxField;

    private string cntemailField;

    private string hoursField;

    /// <remarks/>
    public metadataDataqualLineageProcstepProccontCntinfoCntorgp cntorgp
    {
        get
        {
            return this.cntorgpField;
        }
        set
        {
            this.cntorgpField = value;
        }
    }

    /// <remarks/>
    public string cntpos
    {
        get
        {
            return this.cntposField;
        }
        set
        {
            this.cntposField = value;
        }
    }

    /// <remarks/>
    public metadataDataqualLineageProcstepProccontCntinfoCntaddr cntaddr
    {
        get
        {
            return this.cntaddrField;
        }
        set
        {
            this.cntaddrField = value;
        }
    }

    /// <remarks/>
    public string cntvoice
    {
        get
        {
            return this.cntvoiceField;
        }
        set
        {
            this.cntvoiceField = value;
        }
    }

    /// <remarks/>
    public string cntfax
    {
        get
        {
            return this.cntfaxField;
        }
        set
        {
            this.cntfaxField = value;
        }
    }

    /// <remarks/>
    public string cntemail
    {
        get
        {
            return this.cntemailField;
        }
        set
        {
            this.cntemailField = value;
        }
    }

    /// <remarks/>
    public string hours
    {
        get
        {
            return this.hoursField;
        }
        set
        {
            this.hoursField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageProcstepProccontCntinfoCntorgp
{

    private string cntorgField;

    /// <remarks/>
    public string cntorg
    {
        get
        {
            return this.cntorgField;
        }
        set
        {
            this.cntorgField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDataqualLineageProcstepProccontCntinfoCntaddr
{

    private string addrtypeField;

    private string[] addressField;

    private string cityField;

    private string stateField;

    private string postalField;

    private string countryField;

    /// <remarks/>
    public string addrtype
    {
        get
        {
            return this.addrtypeField;
        }
        set
        {
            this.addrtypeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("address")]
    public string[] address
    {
        get
        {
            return this.addressField;
        }
        set
        {
            this.addressField = value;
        }
    }

    /// <remarks/>
    public string city
    {
        get
        {
            return this.cityField;
        }
        set
        {
            this.cityField = value;
        }
    }

    /// <remarks/>
    public string state
    {
        get
        {
            return this.stateField;
        }
        set
        {
            this.stateField = value;
        }
    }

    /// <remarks/>
    public string postal
    {
        get
        {
            return this.postalField;
        }
        set
        {
            this.postalField = value;
        }
    }

    /// <remarks/>
    public string country
    {
        get
        {
            return this.countryField;
        }
        set
        {
            this.countryField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataSpdoinfo
{

    private string indsprefField;

    private string directField;

    private metadataSpdoinfoRastinfo rastinfoField;

    /// <remarks/>
    public string indspref
    {
        get
        {
            return this.indsprefField;
        }
        set
        {
            this.indsprefField = value;
        }
    }

    /// <remarks/>
    public string direct
    {
        get
        {
            return this.directField;
        }
        set
        {
            this.directField = value;
        }
    }

    /// <remarks/>
    public metadataSpdoinfoRastinfo rastinfo
    {
        get
        {
            return this.rastinfoField;
        }
        set
        {
            this.rastinfoField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataSpdoinfoRastinfo
{

    private string cvaltypeField;

    private string rasttypeField;

    private ushort rowcountField;

    private ushort colcountField;

    private byte vrtcountField;

    /// <remarks/>
    public string cvaltype
    {
        get
        {
            return this.cvaltypeField;
        }
        set
        {
            this.cvaltypeField = value;
        }
    }

    /// <remarks/>
    public string rasttype
    {
        get
        {
            return this.rasttypeField;
        }
        set
        {
            this.rasttypeField = value;
        }
    }

    /// <remarks/>
    public ushort rowcount
    {
        get
        {
            return this.rowcountField;
        }
        set
        {
            this.rowcountField = value;
        }
    }

    /// <remarks/>
    public ushort colcount
    {
        get
        {
            return this.colcountField;
        }
        set
        {
            this.colcountField = value;
        }
    }

    /// <remarks/>
    public byte vrtcount
    {
        get
        {
            return this.vrtcountField;
        }
        set
        {
            this.vrtcountField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataSpref
{

    private metadataSprefHorizsys horizsysField;

    /// <remarks/>
    public metadataSprefHorizsys horizsys
    {
        get
        {
            return this.horizsysField;
        }
        set
        {
            this.horizsysField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataSprefHorizsys
{

    private metadataSprefHorizsysGeograph geographField;

    private metadataSprefHorizsysGeodetic geodeticField;

    /// <remarks/>
    public metadataSprefHorizsysGeograph geograph
    {
        get
        {
            return this.geographField;
        }
        set
        {
            this.geographField = value;
        }
    }

    /// <remarks/>
    public metadataSprefHorizsysGeodetic geodetic
    {
        get
        {
            return this.geodeticField;
        }
        set
        {
            this.geodeticField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataSprefHorizsysGeograph
{

    private decimal latresField;

    private decimal longresField;

    private string geogunitField;

    /// <remarks/>
    public decimal latres
    {
        get
        {
            return this.latresField;
        }
        set
        {
            this.latresField = value;
        }
    }

    /// <remarks/>
    public decimal longres
    {
        get
        {
            return this.longresField;
        }
        set
        {
            this.longresField = value;
        }
    }

    /// <remarks/>
    public string geogunit
    {
        get
        {
            return this.geogunitField;
        }
        set
        {
            this.geogunitField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataSprefHorizsysGeodetic
{

    private string horizdnField;

    private string ellipsField;

    private decimal semiaxisField;

    private decimal denflatField;

    /// <remarks/>
    public string horizdn
    {
        get
        {
            return this.horizdnField;
        }
        set
        {
            this.horizdnField = value;
        }
    }

    /// <remarks/>
    public string ellips
    {
        get
        {
            return this.ellipsField;
        }
        set
        {
            this.ellipsField = value;
        }
    }

    /// <remarks/>
    public decimal semiaxis
    {
        get
        {
            return this.semiaxisField;
        }
        set
        {
            this.semiaxisField = value;
        }
    }

    /// <remarks/>
    public decimal denflat
    {
        get
        {
            return this.denflatField;
        }
        set
        {
            this.denflatField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataEainfo
{

    private metadataEainfoDetailed detailedField;

    /// <remarks/>
    public metadataEainfoDetailed detailed
    {
        get
        {
            return this.detailedField;
        }
        set
        {
            this.detailedField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataEainfoDetailed
{

    private metadataEainfoDetailedEnttyp enttypField;

    private metadataEainfoDetailedAttr attrField;

    /// <remarks/>
    public metadataEainfoDetailedEnttyp enttyp
    {
        get
        {
            return this.enttypField;
        }
        set
        {
            this.enttypField = value;
        }
    }

    /// <remarks/>
    public metadataEainfoDetailedAttr attr
    {
        get
        {
            return this.attrField;
        }
        set
        {
            this.attrField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataEainfoDetailedEnttyp
{

    private string enttyplField;

    private string enttypdField;

    private string enttypdsField;

    /// <remarks/>
    public string enttypl
    {
        get
        {
            return this.enttyplField;
        }
        set
        {
            this.enttyplField = value;
        }
    }

    /// <remarks/>
    public string enttypd
    {
        get
        {
            return this.enttypdField;
        }
        set
        {
            this.enttypdField = value;
        }
    }

    /// <remarks/>
    public string enttypds
    {
        get
        {
            return this.enttypdsField;
        }
        set
        {
            this.enttypdsField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataEainfoDetailedAttr
{

    private string attrlablField;

    private string attrdefField;

    private string attrdefsField;

    private metadataEainfoDetailedAttrAttrdomv attrdomvField;

    /// <remarks/>
    public string attrlabl
    {
        get
        {
            return this.attrlablField;
        }
        set
        {
            this.attrlablField = value;
        }
    }

    /// <remarks/>
    public string attrdef
    {
        get
        {
            return this.attrdefField;
        }
        set
        {
            this.attrdefField = value;
        }
    }

    /// <remarks/>
    public string attrdefs
    {
        get
        {
            return this.attrdefsField;
        }
        set
        {
            this.attrdefsField = value;
        }
    }

    /// <remarks/>
    public metadataEainfoDetailedAttrAttrdomv attrdomv
    {
        get
        {
            return this.attrdomvField;
        }
        set
        {
            this.attrdomvField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataEainfoDetailedAttrAttrdomv
{

    private metadataEainfoDetailedAttrAttrdomvRdom rdomField;

    /// <remarks/>
    public metadataEainfoDetailedAttrAttrdomvRdom rdom
    {
        get
        {
            return this.rdomField;
        }
        set
        {
            this.rdomField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataEainfoDetailedAttrAttrdomvRdom
{

    private string rdomminField;

    private string rdommaxField;

    private string attrunitField;

    /// <remarks/>
    public string rdommin
    {
        get
        {
            return this.rdomminField;
        }
        set
        {
            this.rdomminField = value;
        }
    }

    /// <remarks/>
    public string rdommax
    {
        get
        {
            return this.rdommaxField;
        }
        set
        {
            this.rdommaxField = value;
        }
    }

    /// <remarks/>
    public string attrunit
    {
        get
        {
            return this.attrunitField;
        }
        set
        {
            this.attrunitField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfo
{

    private metadataDistinfoDistrib distribField;

    private string resdescField;

    private string distliabField;

    private metadataDistinfoStdorder stdorderField;

    private string customField;

    private string techpreqField;

    /// <remarks/>
    public metadataDistinfoDistrib distrib
    {
        get
        {
            return this.distribField;
        }
        set
        {
            this.distribField = value;
        }
    }

    /// <remarks/>
    public string resdesc
    {
        get
        {
            return this.resdescField;
        }
        set
        {
            this.resdescField = value;
        }
    }

    /// <remarks/>
    public string distliab
    {
        get
        {
            return this.distliabField;
        }
        set
        {
            this.distliabField = value;
        }
    }

    /// <remarks/>
    public metadataDistinfoStdorder stdorder
    {
        get
        {
            return this.stdorderField;
        }
        set
        {
            this.stdorderField = value;
        }
    }

    /// <remarks/>
    public string custom
    {
        get
        {
            return this.customField;
        }
        set
        {
            this.customField = value;
        }
    }

    /// <remarks/>
    public string techpreq
    {
        get
        {
            return this.techpreqField;
        }
        set
        {
            this.techpreqField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoDistrib
{

    private metadataDistinfoDistribCntinfo cntinfoField;

    /// <remarks/>
    public metadataDistinfoDistribCntinfo cntinfo
    {
        get
        {
            return this.cntinfoField;
        }
        set
        {
            this.cntinfoField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoDistribCntinfo
{

    private metadataDistinfoDistribCntinfoCntorgp cntorgpField;

    private string cntposField;

    private metadataDistinfoDistribCntinfoCntaddr cntaddrField;

    private string cntvoiceField;

    private string cntfaxField;

    private string cntemailField;

    private string hoursField;

    /// <remarks/>
    public metadataDistinfoDistribCntinfoCntorgp cntorgp
    {
        get
        {
            return this.cntorgpField;
        }
        set
        {
            this.cntorgpField = value;
        }
    }

    /// <remarks/>
    public string cntpos
    {
        get
        {
            return this.cntposField;
        }
        set
        {
            this.cntposField = value;
        }
    }

    /// <remarks/>
    public metadataDistinfoDistribCntinfoCntaddr cntaddr
    {
        get
        {
            return this.cntaddrField;
        }
        set
        {
            this.cntaddrField = value;
        }
    }

    /// <remarks/>
    public string cntvoice
    {
        get
        {
            return this.cntvoiceField;
        }
        set
        {
            this.cntvoiceField = value;
        }
    }

    /// <remarks/>
    public string cntfax
    {
        get
        {
            return this.cntfaxField;
        }
        set
        {
            this.cntfaxField = value;
        }
    }

    /// <remarks/>
    public string cntemail
    {
        get
        {
            return this.cntemailField;
        }
        set
        {
            this.cntemailField = value;
        }
    }

    /// <remarks/>
    public string hours
    {
        get
        {
            return this.hoursField;
        }
        set
        {
            this.hoursField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoDistribCntinfoCntorgp
{

    private string cntorgField;

    /// <remarks/>
    public string cntorg
    {
        get
        {
            return this.cntorgField;
        }
        set
        {
            this.cntorgField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoDistribCntinfoCntaddr
{

    private string addrtypeField;

    private string[] addressField;

    private string cityField;

    private string stateField;

    private string postalField;

    private string countryField;

    /// <remarks/>
    public string addrtype
    {
        get
        {
            return this.addrtypeField;
        }
        set
        {
            this.addrtypeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("address")]
    public string[] address
    {
        get
        {
            return this.addressField;
        }
        set
        {
            this.addressField = value;
        }
    }

    /// <remarks/>
    public string city
    {
        get
        {
            return this.cityField;
        }
        set
        {
            this.cityField = value;
        }
    }

    /// <remarks/>
    public string state
    {
        get
        {
            return this.stateField;
        }
        set
        {
            this.stateField = value;
        }
    }

    /// <remarks/>
    public string postal
    {
        get
        {
            return this.postalField;
        }
        set
        {
            this.postalField = value;
        }
    }

    /// <remarks/>
    public string country
    {
        get
        {
            return this.countryField;
        }
        set
        {
            this.countryField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoStdorder
{

    private metadataDistinfoStdorderDigform digformField;

    private string feesField;

    private string orderingField;

    /// <remarks/>
    public metadataDistinfoStdorderDigform digform
    {
        get
        {
            return this.digformField;
        }
        set
        {
            this.digformField = value;
        }
    }

    /// <remarks/>
    public string fees
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
    public string ordering
    {
        get
        {
            return this.orderingField;
        }
        set
        {
            this.orderingField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoStdorderDigform
{

    private metadataDistinfoStdorderDigformDigtinfo digtinfoField;

    private metadataDistinfoStdorderDigformDigtopt digtoptField;

    /// <remarks/>
    public metadataDistinfoStdorderDigformDigtinfo digtinfo
    {
        get
        {
            return this.digtinfoField;
        }
        set
        {
            this.digtinfoField = value;
        }
    }

    /// <remarks/>
    public metadataDistinfoStdorderDigformDigtopt digtopt
    {
        get
        {
            return this.digtoptField;
        }
        set
        {
            this.digtoptField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoStdorderDigformDigtinfo
{

    private string formnameField;

    private ushort transizeField;

    /// <remarks/>
    public string formname
    {
        get
        {
            return this.formnameField;
        }
        set
        {
            this.formnameField = value;
        }
    }

    /// <remarks/>
    public ushort transize
    {
        get
        {
            return this.transizeField;
        }
        set
        {
            this.transizeField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoStdorderDigformDigtopt
{

    private metadataDistinfoStdorderDigformDigtoptOnlinopt onlinoptField;

    /// <remarks/>
    public metadataDistinfoStdorderDigformDigtoptOnlinopt onlinopt
    {
        get
        {
            return this.onlinoptField;
        }
        set
        {
            this.onlinoptField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoStdorderDigformDigtoptOnlinopt
{

    private metadataDistinfoStdorderDigformDigtoptOnlinoptComputer computerField;

    private string accinstrField;

    /// <remarks/>
    public metadataDistinfoStdorderDigformDigtoptOnlinoptComputer computer
    {
        get
        {
            return this.computerField;
        }
        set
        {
            this.computerField = value;
        }
    }

    /// <remarks/>
    public string accinstr
    {
        get
        {
            return this.accinstrField;
        }
        set
        {
            this.accinstrField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoStdorderDigformDigtoptOnlinoptComputer
{

    private metadataDistinfoStdorderDigformDigtoptOnlinoptComputerNetworka networkaField;

    /// <remarks/>
    public metadataDistinfoStdorderDigformDigtoptOnlinoptComputerNetworka networka
    {
        get
        {
            return this.networkaField;
        }
        set
        {
            this.networkaField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataDistinfoStdorderDigformDigtoptOnlinoptComputerNetworka
{

    private string networkrField;

    /// <remarks/>
    public string networkr
    {
        get
        {
            return this.networkrField;
        }
        set
        {
            this.networkrField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataMetainfo
{

    private uint metdField;

    private metadataMetainfoMetc metcField;

    private string metstdnField;

    private string metstdvField;

    private string mettcField;

    /// <remarks/>
    public uint metd
    {
        get
        {
            return this.metdField;
        }
        set
        {
            this.metdField = value;
        }
    }

    /// <remarks/>
    public metadataMetainfoMetc metc
    {
        get
        {
            return this.metcField;
        }
        set
        {
            this.metcField = value;
        }
    }

    /// <remarks/>
    public string metstdn
    {
        get
        {
            return this.metstdnField;
        }
        set
        {
            this.metstdnField = value;
        }
    }

    /// <remarks/>
    public string metstdv
    {
        get
        {
            return this.metstdvField;
        }
        set
        {
            this.metstdvField = value;
        }
    }

    /// <remarks/>
    public string mettc
    {
        get
        {
            return this.mettcField;
        }
        set
        {
            this.mettcField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataMetainfoMetc
{

    private metadataMetainfoMetcCntinfo cntinfoField;

    /// <remarks/>
    public metadataMetainfoMetcCntinfo cntinfo
    {
        get
        {
            return this.cntinfoField;
        }
        set
        {
            this.cntinfoField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataMetainfoMetcCntinfo
{

    private metadataMetainfoMetcCntinfoCntorgp cntorgpField;

    private string cntposField;

    private metadataMetainfoMetcCntinfoCntaddr cntaddrField;

    private string cntvoiceField;

    private string cntfaxField;

    private string cntemailField;

    private string hoursField;

    /// <remarks/>
    public metadataMetainfoMetcCntinfoCntorgp cntorgp
    {
        get
        {
            return this.cntorgpField;
        }
        set
        {
            this.cntorgpField = value;
        }
    }

    /// <remarks/>
    public string cntpos
    {
        get
        {
            return this.cntposField;
        }
        set
        {
            this.cntposField = value;
        }
    }

    /// <remarks/>
    public metadataMetainfoMetcCntinfoCntaddr cntaddr
    {
        get
        {
            return this.cntaddrField;
        }
        set
        {
            this.cntaddrField = value;
        }
    }

    /// <remarks/>
    public string cntvoice
    {
        get
        {
            return this.cntvoiceField;
        }
        set
        {
            this.cntvoiceField = value;
        }
    }

    /// <remarks/>
    public string cntfax
    {
        get
        {
            return this.cntfaxField;
        }
        set
        {
            this.cntfaxField = value;
        }
    }

    /// <remarks/>
    public string cntemail
    {
        get
        {
            return this.cntemailField;
        }
        set
        {
            this.cntemailField = value;
        }
    }

    /// <remarks/>
    public string hours
    {
        get
        {
            return this.hoursField;
        }
        set
        {
            this.hoursField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataMetainfoMetcCntinfoCntorgp
{

    private string cntorgField;

    /// <remarks/>
    public string cntorg
    {
        get
        {
            return this.cntorgField;
        }
        set
        {
            this.cntorgField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class metadataMetainfoMetcCntinfoCntaddr
{

    private string addrtypeField;

    private string[] addressField;

    private string cityField;

    private string stateField;

    private string postalField;

    private string countryField;

    /// <remarks/>
    public string addrtype
    {
        get
        {
            return this.addrtypeField;
        }
        set
        {
            this.addrtypeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("address")]
    public string[] address
    {
        get
        {
            return this.addressField;
        }
        set
        {
            this.addressField = value;
        }
    }

    /// <remarks/>
    public string city
    {
        get
        {
            return this.cityField;
        }
        set
        {
            this.cityField = value;
        }
    }

    /// <remarks/>
    public string state
    {
        get
        {
            return this.stateField;
        }
        set
        {
            this.stateField = value;
        }
    }

    /// <remarks/>
    public string postal
    {
        get
        {
            return this.postalField;
        }
        set
        {
            this.postalField = value;
        }
    }

    /// <remarks/>
    public string country
    {
        get
        {
            return this.countryField;
        }
        set
        {
            this.countryField = value;
        }
    }
}

