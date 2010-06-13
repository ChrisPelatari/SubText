#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext@googlegroups.com 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using Subtext.Extensibility;

namespace Subtext.Framework.Components
{
    /// <summary>
    /// Represents a trackback within this system. This is essentially 
    /// a comment created via the Trackback/Pingback API.
    /// </summary>
    public class Trackback : FeedbackItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trackback"/> class.
        /// </summary>
        /// <param name="entryId">The parent id.</param>
        /// <param name="title">The title.</param>
        /// <param name="sourceUrl">The title URL.</param>
        /// <param name="author">The author.</param>
        /// <param name="body">The body.</param>
        /// <param name="dateCreated">The date created.</param>
        public Trackback(int entryId, string title, Uri sourceUrl, string author, string body, DateTime dateCreated)
            : base(FeedbackType.PingTrack)
        {
            EntryId = entryId;
            Title = title;
            SourceUrl = sourceUrl;
            Author = author;
            Body = body;

            Approved = true;
            DateCreated = DateModified = dateCreated;
        }
    }
}