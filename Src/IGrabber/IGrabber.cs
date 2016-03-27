using System;
using System.Collections.Generic;

namespace IGrabber
{
    /// <summary>
    ///  This interface is used to determine the structure of the plug
    /// </summary>
    public interface IGrabber
    {
        /// <summary>
        /// The method comprising the logic of parsing web pages
        /// Retur list of pair News Title - Webpage Address 
        /// </summary>
        List<Tuple<string, string>> GetInfo();

        /// <summary>
        ///  Field, which indicates the plug-in tag.On the basis of this tag will be configured settings.
        /// </summary>
        string Tag { get; }

        /// <summary>
        ///  Field that indicates the website address
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Field , which indicates how often (in minutes) the update will be checked
        /// </summary>
        int Interval { get; }
    }
}
