using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// THe Obserservable class is used for observer objects that are used for specifics purposes.
/// The Observable object must define its "Update" function that takes in some string that could be a url.
/// Extensible functionality should be added here.
/// This class built for convience so that we don't have to create 100 observers, but just 100 observables that
/// the observers looked into.
/// </summary>
abstract class Observerable
{
    // Fields
    string State;
    string NextState;
    protected DataFactory factory;
    public DataRecord record; // The datarecord to apply changes too

    // Methods
    public Observerable(DataFactory dataFactory)
    {
        factory = dataFactory;
    }

    // Thought: have Update return a bool which returns true if the chain is finished
    // In the VWClient when the function returns true, it can fire the OnDownloadComplete event.
    public abstract string Update();
}

