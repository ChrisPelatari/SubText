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
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace Subtext.Framework.Configuration
{
    public class MimeTypesMapper
    {
        public static readonly MimeTypesMapper Mappings =
            new MimeTypesMapper((NameValueCollection)ConfigurationManager.GetSection("EnclosureMimetypes"));

        public MimeTypesMapper(NameValueCollection config)
        {
            if(config == null)
            {
                throw new ArgumentNullException("config");
            }
            List = config;
            Count = config.Keys.Count;
        }

        public int Count { get; private set; }

        public NameValueCollection List { get; private set; }

        /// <summary>
        /// Returns the mimetype that corresponds to a file extension.
        /// </summary>
        /// <param name="ext">Extension of a file.</param>
        /// <returns>The MimeType</returns>
        public string GetMimeType(string ext)
        {
            if(ext == null)
            {
                throw new ArgumentNullException("ext");
            }
            if(List[ext] != null)
            {
                return List[ext];
            }
            return null;
        }

        /// <summary>
        /// Detect the mimetype of the url of a file.
        /// </summary>
        /// <param name="url">Url of the file</param>
        /// <returns>The MimeType.</returns>
        public string ParseUrl(string url)
        {
            if(url == null)
            {
                throw new ArgumentNullException("url");
            }
            Uri uri;

            if(!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                throw new ArgumentException("Url not valid.", "url");
            }

            string path = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

            string ext = Path.GetExtension(path);

            return GetMimeType(ext);
        }
    }
}