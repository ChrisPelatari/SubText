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

using System.Web;
using System.Web.Routing;
using Subtext.Framework.XmlRpc;
using Subtext.Infrastructure;

namespace Subtext.Framework.Routing
{
    public class XmlRpcRouteHandler<THandler> : IRouteHandler where THandler : SubtextXmlRpcService
    {
        public XmlRpcRouteHandler(IServiceLocator serviceLocator)
        {
            ServiceLocator = serviceLocator;
        }

        protected IServiceLocator ServiceLocator { get; private set; }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            Bootstrapper.RequestContext = requestContext;
            return ServiceLocator.GetService<THandler>();
        }
    }
}