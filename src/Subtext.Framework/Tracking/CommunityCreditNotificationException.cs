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

namespace Subtext.Framework.Tracking
{
    public class CommunityCreditNotificationException : ApplicationException
    {
        public CommunityCreditNotificationException()
        {
        }

        public CommunityCreditNotificationException(string message) : base(message)
        {
        }

        public CommunityCreditNotificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}