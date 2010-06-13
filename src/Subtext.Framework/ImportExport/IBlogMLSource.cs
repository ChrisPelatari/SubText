﻿#region Disclaimer/Info

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

using BlogML.Xml;
using System.Collections.Generic;

namespace Subtext.ImportExport
{
    public interface IBlogMLSource
    {
        BlogMLBlog GetBlog();
        IEnumerable<BlogMLPost> GetBlogPosts(bool embedAttachments);
    }
}
