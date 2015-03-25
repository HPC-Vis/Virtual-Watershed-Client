using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//http://www.grumpydev.com/2010/02/25/thread-safe-dictionarytkeytvalue/
public class ThreadSafeDictionary<TKey, TValue>
{
    private readonly object _Padlock = new object();

    private readonly Dictionary<TKey, TValue> _Dictionary = new Dictionary<TKey, TValue>();

    public void Remove(TKey key)
    {
        lock (_Padlock)
        {
            _Dictionary.Remove(key);
        }
    }

    public int Count()
    {
        lock (_Padlock)
        {
            return _Dictionary.Count;
        }
    }

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

    public bool TryGetValue(TKey key, out TValue value)
    {
        lock (_Padlock)
        {
            return _Dictionary.TryGetValue(key, out value);
        }
    }

    public bool ContainsKey(TKey Key)
    {
        lock (_Padlock)
        {
            return _Dictionary.ContainsKey(Key);
        }
    }
}
