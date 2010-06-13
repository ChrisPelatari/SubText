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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Subtext.Framework.Components;
using Subtext.Framework.Data;
using Subtext.Framework.Providers;
using Subtext.Framework.Services.SearchEngine;

namespace Subtext.Web.UI.Controls
{
    public class RelatedLinks : BaseControl
    {
        public ISearchEngineService SearchEngineService { 
            get
            {
                return SubtextPage.SearchEngineService;
            }
        }

        public RelatedLinks()
        {
            RowCount = 5;
        }

        public int RowCount { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            int blogId = Blog.Id >= 1 ? Blog.Id : 0;
            var urlRelatedLinks = FindControl("Links") as Repeater;
            Entry entry = Cacher.GetEntryFromRequest(true, SubtextContext);

            urlRelatedLinks.DataSource = SearchEngineService.RelatedContents(entry.Id, RowCount, blogId); ;
            urlRelatedLinks.DataBind();
            if (urlRelatedLinks.Items.Count == 0)
                this.Visible = false;
            base.OnLoad(e);
        }


        protected virtual void MoreReadingCreated(object sender, RepeaterItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var pi = (SearchEngineResult)e.Item.DataItem;
                BindLink(e, pi);
            }
        }

        private void BindLink(RepeaterItemEventArgs e, SearchEngineResult searchResult)
        {
            var relatedLink = (HyperLink)e.Item.FindControl("Link");
            var datePublished = (Literal)e.Item.FindControl("DatePublished");
            var score = (Literal)e.Item.FindControl("Score");
            if(relatedLink != null)
            {
                relatedLink.Text = searchResult.Title;
                relatedLink.NavigateUrl = Url.EntryUrl(searchResult);
                if (datePublished!=null) datePublished.Text = searchResult.DateSyndicated.ToShortDateString();
                if (score!=null) score.Text = searchResult.Score.ToString();
            }
        }
    }
}