using System;

namespace Mars_Seal_Crimson
{
    struct FlagDateTime
    {
        public /*DateTime*/Int32 timestamp;
        public bool flag;

        public FlagDateTime(Int32 time1, bool flag1)
        {
            timestamp = time1;
            flag = flag1;
        }
    }
}