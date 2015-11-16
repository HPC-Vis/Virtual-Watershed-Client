using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

public delegate void DataRecordSetter(List<DataRecord> Records);

/// <summary>
/// THe Obserservable class is used for observer objects that are used for specifics purposes.
/// The Observable object must define its "Update" function that takes in some string that could be a url.
/// Extensible functionality should be added here.
/// This class built for convience so that we don't have to create 100 observers, but just 100 observables that
/// the observers looked into.
/// </summary>
public enum DownloadType { Record, File };
abstract class Observerable  : IComparable<Observerable>
{
    // Fields
    public string Token;
    public DataRecordSetter callback;
    protected DataFactory factory;
    protected List<DataRecord> records;
    protected string FilePath;
    protected string FileName;
    protected DownloadType type;
	public int Priority=1;
	public string ModelRunUUID;

    // Methods
    public Observerable(DataFactory dataFactory,DownloadType Type,string OutputPath="",string OutputName="")
    {
        factory = dataFactory;
        type = Type;
        FilePath = OutputPath;
        FileName = OutputName;
    }

    // Call to indicate raise the error state
    public abstract void Error();

    // Call to go to the next state
    public abstract string Update();

    // Call to finish chain
    public abstract void CallBack();

	public int CompareTo(Observerable other)
	{
		if (this.Priority < other.Priority) return 1;
		else if (this.Priority > other.Priority) return -1;
		else return 0;
	}
}

