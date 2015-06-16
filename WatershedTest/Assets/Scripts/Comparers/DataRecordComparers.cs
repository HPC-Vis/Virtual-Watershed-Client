using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DataRecordComparers
{
    // NOTE: (FIRST < SECOND) = 1 means descending       3, 2, 1, ....
    // NOTE: (FIRST > SECOND) = 1 means ascending        1, 2, 3, ....
    public class StartDateAscending : IComparer<DataRecord>
    {
        public int Compare(DataRecord first, DataRecord second)
        {
            if (first.start > second.start) { return 1; }
            else if (first.start == second.start) { return 0; }
            else { return -1; }
        }
    }

    public class StartDateDescending : IComparer<DataRecord>
    {
        public int Compare(DataRecord first, DataRecord second)
        {
            if (first.start < second.start) { return 1; }
            else if (first.start == second.start) { return 0; }
            else { return -1; }
        }
    }

    public class EndDateAscending : IComparer<DataRecord>
    {
        public int Compare(DataRecord first, DataRecord second)
        {
            if (first.end > second.end) { return 1; }
            else if (first.end == second.end) { return 0; }
            else { return -1; }
        }
    }

    public class EndDateDescending : IComparer<DataRecord>
    {
        public int Compare(DataRecord first, DataRecord second)
        {
            if (first.end < second.end) { return 1; }
            else if (first.end == second.end) { return 0; }
            else { return -1; }
        }
    }

    // DateAdded = When added to the server
    public class DateAddedAscending : IComparer<DataRecord>
    {
        public int Compare(DataRecord first, DataRecord second)
        {
            throw new NotImplementedException("See Note at the top of this file for ordering");
        }
    }

    public class DateAddedDescending : IComparer<DataRecord>
    {
        public int Compare(DataRecord first, DataRecord second)
        {
            throw new NotImplementedException("See Note at the top of this file for ordering");
        }
    }

    // DateCreated = When data was collected at location
    public class DateCreatedAscending : IComparer<DataRecord>
    {
        public int Compare(DataRecord first, DataRecord second)
        {
            throw new NotImplementedException("See Note at the top of this file for ordering");
        }
    }

    public class DateCreatedDescending : IComparer<DataRecord>
    {
        public int Compare(DataRecord first, DataRecord second)
        {
            throw new NotImplementedException("See Note at the top of this file for ordering");
        }
    }
}
