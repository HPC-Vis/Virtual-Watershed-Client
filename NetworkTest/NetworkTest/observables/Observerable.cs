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
/// This class built for convience so that we don't have to crate 100 observers, but just 100 observables that
/// the observers looked into.
/// </summary>
abstract class Observerable
{
    public abstract void Update(string URL)
    {

    }
}

