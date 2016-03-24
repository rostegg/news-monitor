using System;
using System.Collections.Generic;

namespace IGrabber
{
    public interface IGrabber
    {
        List<Tuple<string, string>> GetInfo();

        string Tag { get; }

        string Url { get; }

       int Interval { get; }
    }
}
