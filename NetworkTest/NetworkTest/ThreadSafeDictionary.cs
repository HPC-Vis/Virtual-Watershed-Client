using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//code from http://www.grumpydev.com/2010/02/25/thread-safe-dictionarytkeytvalue/
public class ThreadSafeDictionary<TKey, TValue>
{
    private readonly object _Padlock = new object();

    private readonly Dictionary<TKey, TValue> _Dictionary = new Dictionary<TKey, TValue>();

    /// <summary>
    /// Removes an entry from the ThreadSafeDictionary
    /// </summary>
    /// <param name="key">The key of the entry to be removed</param>
    public void Remove(TKey key)
    {
        lock (_Padlock)
        {
            _Dictionary.Remove(key);
        }
    }

    /// <summary>
    /// Retrieves the number of elements contained within the ThreadSafeDictionary
    /// </summary>
    /// <returns>The number of elements in the ThreadSafeDictionary</returns>
    public int Count()
    {
        lock (_Padlock)
        {
            return _Dictionary.Count;
        }
    }

    /// <summary>
    /// Gets or sets the value of an entry in the dictionary
    /// </summary>
    /// <param name="key">The key of the entry to be modified or retrieved</param>
    /// <returns>Returns the entry corresponding to the key, or modifies that same entry</returns>
    public TValue this[TKey key]
    {
        get
        {
            lock (_Padlock)
            {

                return _Dictionary[key];

            }
        }
        set
        {
            lock (_Padlock)
            {

                _Dictionary[key] = value;
            }
        }
    }

    /// <summary>
    /// Attempts to retrieve a value from the dictionary if it is safe to do so
    /// </summary>
    /// <param name="key">The key of the entry to be accessed</param>
    /// <param name="value">The value returned should the situation be safe</param>
    /// <returns>The value corresponding to the given key, as long as _Padlock is free</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        lock (_Padlock)
        {
            return _Dictionary.TryGetValue(key, out value);
        }
    }
}
