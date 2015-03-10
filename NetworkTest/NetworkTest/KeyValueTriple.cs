using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct KeyValueTriple<TKey, TOne, TTwo>
{
    // Fields
    public readonly TKey Key;

    public readonly TOne VOne;

    public readonly TTwo VTwo;

    public KeyValueTriple(TKey key, TOne v_one, TTwo v_two)
    {
        Key = key;
        VOne = v_one;
        VTwo = v_two;
    }
}
