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
using System.Reflection;
using System.Resources;
using System.Web.UI;
using System.Web.UI.WebControls;
using Subtext.Extensibility.Providers;
using Subtext.Framework;
using Subtext.Framework.Web;

namespace Subtext.Providers.BlogEntryEditor.FCKeditor
{
    /// <summary>
    /// Implements the BlogEntryEditorProvider abstract provider to allow the 
    /// user use the FCKEditor control for editing blog posts.
    /// </summary>
    public class FckBlogEntryEditorProvider : BlogEntryEditorProvider
    {
        private static readonly ResourceManager rm =
            new ResourceManager("Subtext.Providers.BlogEntryEditor.FCKeditor.resources.ErrorMessages",
                                Assembly.GetExecutingAssembly());

        static string _fileAllowedExtensions = string.Empty;

        readonly FredCK.FCKeditorV2.FCKeditor _fckCtl = new FredCK.FCKeditorV2.FCKeditor();
        //There's a good reason to do this early.

        string _imageBrowserURL = string.Empty;
        string _imageConnectorURL = string.Empty;
        string _linkBrowserURL = string.Empty;
        string _linkConnectorURL = string.Empty;
        string _skin = string.Empty;
        string _toolbarSet = string.Empty;
        string _webFormFolder = string.Empty;

        public override Control RichTextEditorControl
        {
            get { return _fckCtl; }
        }

        public override string Text
        {
            get { return _fckCtl.Value; }
            set { _fckCtl.Value = value; }
        }

        public override string Xhtml
        {
            get { return Text; }
        }

        public static string ImageAllowedExtensions { get; private set; }

        public static string FileAllowedExtensions { get; private set; }

        public override void Initialize(string name, NameValueCollection configValue)
        {
            if(name == null)
            {
                throw new ArgumentNullException("name", rm.GetString("nameNeeded"));
            }

            if(configValue == null)
            {
                throw new ArgumentNullException("configValue", rm.GetString("configNeeded"));
            }

            if(configValue["WebFormFolder"] != null)
            {
                _webFormFolder = configValue["WebFormFolder"];
            }
            else
            {
                throw new InvalidOperationException(rm.GetString("WebFormFolderNeeded"));
            }

            if(configValue["ImageBrowserURL"] != null)
            {
                _imageBrowserURL = configValue["ImageBrowserURL"];
            }
            else
            {
                throw new InvalidOperationException(rm.GetString("ImageBrowserURLNeeded"));
            }

            if(configValue["LinkBrowserURL"] != null)
            {
                _linkBrowserURL = configValue["LinkBrowserURL"];
            }
            else
            {
                throw new InvalidOperationException(rm.GetString("LinkBrowserURLNeeded"));
            }

            if(configValue["ImageConnectorURL"] != null)
            {
                _imageConnectorURL = configValue["ImageConnectorURL"];
            }
            else
            {
                throw new InvalidOperationException(rm.GetString("ImageConnectorURLNeeded"));
            }

            if(configValue["LinkConnectorURL"] != null)
            {
                _linkConnectorURL = configValue["LinkConnectorURL"];
            }
            else
            {
                throw new InvalidOperationException(rm.GetString("LinkConnectorURLNeeded"));
            }


            if(configValue["FileAllowedExtensions"] != null)
            {
                FileAllowedExtensions = configValue["FileAllowedExtensions"];
            }
            else
            {
                throw new InvalidOperationException(rm.GetString("FileAllowedExtensionsNeeded"));
            }


            if(configValue["ImageAllowedExtensions"] != null)
            {
                ImageAllowedExtensions = configValue["ImageAllowedExtensions"];
            }
            else
            {
                throw new InvalidOperationException(rm.GetString("ImageAllowedExtensionsNeeded"));
            }

            if(configValue["ToolbarSet"] != null)
            {
                _toolbarSet = configValue["ToolbarSet"];
            }

            if(configValue["Skin"] != null)
            {
                _skin = configValue["Skin"];
            }

            base.Initialize(name, configValue);
        }

        public override void InitializeControl(object context)
        {
            var subtextContext = context as ISubtextContext;
            _fckCtl.ID = ControlId;
            if(!String.IsNullOrEmpty(_webFormFolder))
            {
                _fckCtl.BasePath = HttpHelper.ExpandTildePath(_webFormFolder);
            }

            if(Width != Unit.Empty)
            {
                _fckCtl.Width = Width;
            }
            if(Height != Unit.Empty)
            {
                _fckCtl.Height = Height;
            }
            if(_toolbarSet.Length != 0)
            {
                _fckCtl.ToolbarSet = _toolbarSet;
            }
            if(_skin.Length != 0)
            {
                _fckCtl.SkinPath = _fckCtl.BasePath + "editor/skins/" + _skin + "/";
            }
            // Compute user image gallery url
            string blogSubFolder = subtextContext.Blog.Subfolder;
            string currentImageConnector = _imageConnectorURL;
            string currentLinkConnector = _linkConnectorURL;
            if(blogSubFolder.Length > 0)
            {
                currentImageConnector = _imageConnectorURL.Replace("~/", "~/" + blogSubFolder + "/");
                currentLinkConnector = _linkConnectorURL.Replace("~/", "~/" + blogSubFolder + "/");
            }

            _fckCtl.ImageBrowserURL = String.Format(HttpHelper.ExpandTildePath(_imageBrowserURL),
                                                    HttpHelper.ExpandTildePath(currentImageConnector));
            _fckCtl.LinkBrowserURL = String.Format(HttpHelper.ExpandTildePath(_linkBrowserURL),
                                                   HttpHelper.ExpandTildePath(currentLinkConnector));
        }
    }
}