using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using MbUnit.Framework;
using Moq;
using Subtext.Extensibility;
using Subtext.Framework;
using Subtext.Framework.Components;
using Subtext.Framework.Configuration;
using Subtext.Framework.Routing;
using Subtext.Framework.Syndication;
using UnitTests.Subtext.Framework.Util;

namespace UnitTests.Subtext.Framework.Syndication
{
    /// <summary>
    /// Unit tests of the <see cref="CommentRssWriter"/> class.
    /// </summary>
    [TestFixture]
    public class CommentRssWriterTests : SyndicationTestBase
    {
        /// <summary>
        /// Tests that a valid feed is produced even if a post has no comments.
        /// </summary>
        [Test]
        public void CommentRssWriterProducesValidEmptyFeed()
        {
            var blogInfo = new Blog();
            blogInfo.Host = "localhost";
            blogInfo.Subfolder = "blog";
            blogInfo.Email = "Subtext@example.com";
            blogInfo.RFC3229DeltaEncodingEnabled = true;
            blogInfo.Title = "My Blog Rulz";
            blogInfo.TimeZoneId = TimeZonesTest.PacificTimeZoneId;

            Entry entry = UnitTestHelper.CreateEntryInstanceForSyndication(blogInfo, "haacked", "title of the post",
                                                                           "Body of the post.");
            entry.EntryName = "titleofthepost";
            entry.DateCreated =
                entry.DateSyndicated =
                entry.DateModified = DateTime.ParseExact("2006/04/01", "yyyy/MM/dd", CultureInfo.InvariantCulture);

            var context = new Mock<ISubtextContext>();
            context.FakeSyndicationContext(blogInfo, "/", null);
            Mock<UrlHelper> urlHelper = Mock.Get(context.Object.UrlHelper);
            urlHelper.Setup(url => url.EntryUrl(It.IsAny<Entry>())).Returns(
                "/blog/archive/2006/04/01/titleofthepost.aspx");

            var writer = new CommentRssWriter(new StringWriter(), new List<FeedbackItem>(), entry, context.Object);

            Assert.IsTrue(entry.HasEntryName, "This entry should have an entry name.");

            string expected = @"<rss version=""2.0"" "
                              + @"xmlns:dc=""http://purl.org/dc/elements/1.1/"" "
                              + @"xmlns:trackback=""http://madskills.com/public/xml/rss/module/trackback/"" "
                              + @"xmlns:wfw=""http://wellformedweb.org/CommentAPI/"" "
                              + @"xmlns:slash=""http://purl.org/rss/1.0/modules/slash/"" "
                              + @"xmlns:copyright=""http://blogs.law.harvard.edu/tech/rss"" "
                              + @"xmlns:image=""http://purl.org/rss/1.0/modules/image/"">" + Environment.NewLine
                              + indent() + @"<channel>" + Environment.NewLine
                              + indent(2) + @"<title>title of the post</title>" + Environment.NewLine
                              + indent(2) + @"<link>http://localhost/blog/archive/2006/04/01/titleofthepost.aspx</link>" +
                              Environment.NewLine
                              + indent(2) + @"<description>Body of the post.</description>" + Environment.NewLine
                              + indent(2) + @"<language>en-US</language>" + Environment.NewLine
                              + indent(2) + @"<copyright>Subtext Weblog</copyright>" + Environment.NewLine
                              + indent(2) + @"<generator>{0}</generator>" + Environment.NewLine
                              + indent(2) + @"<image>" + Environment.NewLine
                              + indent(3) + @"<title>title of the post</title>" + Environment.NewLine
                              + indent(3) + @"<url>http://localhost/images/RSS2Image.gif</url>" + Environment.NewLine
                              + indent(3) + @"<link>http://localhost/blog/archive/2006/04/01/titleofthepost.aspx</link>" +
                              Environment.NewLine
                              + indent(3) + @"<width>77</width>" + Environment.NewLine
                              + indent(3) + @"<height>60</height>" + Environment.NewLine
                              + indent(2) + @"</image>" + Environment.NewLine
                              + indent(1) + @"</channel>" + Environment.NewLine
                              + @"</rss>";

            expected = string.Format(expected, VersionInfo.VersionDisplayText);

            Assert.AreEqual(expected, writer.Xml);
        }

        /// <summary>
        /// Tests that a valid feed is produced even if a post has no comments.
        /// </summary>
        [Test]
        public void CommentRssWriterProducesValidFeed()
        {
            var blogInfo = new Blog();
            blogInfo.Host = "localhost";
            blogInfo.Email = "Subtext@example.com";
            blogInfo.RFC3229DeltaEncodingEnabled = true;
            blogInfo.Title = "My Blog Rulz";
            blogInfo.TimeZoneId = TimeZonesTest.PacificTimeZoneId;

            Entry entry = UnitTestHelper.CreateEntryInstanceForSyndication(blogInfo, "haacked", "title of the post",
                                                                           "Body of the post.");
            entry.EntryName = "titleofthepost";
            entry.DateCreated =
                entry.DateSyndicated =
                entry.DateModified = DateTime.ParseExact("2006/02/01", "yyyy/MM/dd", CultureInfo.InvariantCulture);
            entry.Id = 1001;

            var comment = new FeedbackItem(FeedbackType.Comment);
            comment.Id = 1002;
            comment.DateCreated =
                comment.DateModified = DateTime.ParseExact("2006/02/01", "yyyy/MM/dd", CultureInfo.InvariantCulture);
            comment.Title = "re: titleofthepost";
            comment.ParentEntryName = entry.EntryName;
            comment.ParentDateCreated = entry.DateCreated;
            comment.Body = "<strong>I rule!</strong>";
            comment.Author = "Jane Schmane";
            comment.Email = "jane@example.com";
            comment.EntryId = entry.Id;

            var comments = new List<FeedbackItem>();
            comments.Add(comment);

            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.FakeSyndicationContext(blogInfo, "/Subtext.Web/Whatever", "Subtext.Web", null);
            Mock<HttpContextBase> httpContext = Mock.Get(subtextContext.Object.RequestContext.HttpContext);
            httpContext.Setup(c => c.Request.ApplicationPath).Returns("/Subtext.Web");
            Mock<UrlHelper> urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.FeedbackUrl(It.IsAny<FeedbackItem>())).Returns(
                "/Subtext.Web/archive/2006/02/01/titleofthepost.aspx#" + comment.Id);
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns(
                "/Subtext.Web/archive/2006/02/01/titleofthepost.aspx");

            var writer = new CommentRssWriter(new StringWriter(), comments, entry, subtextContext.Object);

            Assert.IsTrue(entry.HasEntryName, "This entry should have an entry name.");

            string expected = @"<rss version=""2.0"" "
                              + @"xmlns:dc=""http://purl.org/dc/elements/1.1/"" "
                              + @"xmlns:trackback=""http://madskills.com/public/xml/rss/module/trackback/"" "
                              + @"xmlns:wfw=""http://wellformedweb.org/CommentAPI/"" "
                              + @"xmlns:slash=""http://purl.org/rss/1.0/modules/slash/"" "
                              + @"xmlns:copyright=""http://blogs.law.harvard.edu/tech/rss"" "
                              + @"xmlns:image=""http://purl.org/rss/1.0/modules/image/"">" + Environment.NewLine
                              + indent() + @"<channel>" + Environment.NewLine
                              + indent(2) + @"<title>title of the post</title>" + Environment.NewLine
                              + indent(2) +
                              @"<link>http://localhost/Subtext.Web/archive/2006/02/01/titleofthepost.aspx</link>" +
                              Environment.NewLine
                              + indent(2) + @"<description>Body of the post.</description>" + Environment.NewLine
                              + indent(2) + @"<language>en-US</language>" + Environment.NewLine
                              + indent(2) + @"<copyright>Subtext Weblog</copyright>" + Environment.NewLine
                              + indent(2) + @"<generator>{0}</generator>" + Environment.NewLine
                              + indent(2) + @"<image>" + Environment.NewLine
                              + indent(3) + @"<title>title of the post</title>" + Environment.NewLine
                              + indent(3) + @"<url>http://localhost/Subtext.Web/images/RSS2Image.gif</url>" +
                              Environment.NewLine
                              + indent(3) +
                              @"<link>http://localhost/Subtext.Web/archive/2006/02/01/titleofthepost.aspx</link>" +
                              Environment.NewLine
                              + indent(3) + @"<width>77</width>" + Environment.NewLine
                              + indent(3) + @"<height>60</height>" + Environment.NewLine
                              + indent(2) + @"</image>" + Environment.NewLine
                              + indent(2) + @"<item>" + Environment.NewLine
                              + indent(3) + @"<title>re: titleofthepost</title>" + Environment.NewLine
                              + indent(3) +
                              @"<link>http://localhost/Subtext.Web/archive/2006/02/01/titleofthepost.aspx#1002</link>" +
                              Environment.NewLine
                              + indent(3) + @"<description>&lt;strong&gt;I rule!&lt;/strong&gt;</description>" +
                              Environment.NewLine
                              + indent(3) + @"<dc:creator>Jane Schmane</dc:creator>" + Environment.NewLine
                              + indent(3) +
                              @"<guid>http://localhost/Subtext.Web/archive/2006/02/01/titleofthepost.aspx#1002</guid>" +
                              Environment.NewLine
                              + indent(3) + @"<pubDate>Wed, 01 Feb 2006 08:00:00 GMT</pubDate>" + Environment.NewLine
                              + indent(2) + @"</item>" + Environment.NewLine
                              + indent() + @"</channel>" + Environment.NewLine
                              + @"</rss>";

            expected = string.Format(expected, VersionInfo.VersionDisplayText);

            Assert.AreEqual(expected, writer.Xml);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Ctor_WithNullEntryCollection_ThrowsArgumentNullException()
        {
            UnitTestHelper.AssertThrowsArgumentNullException(() => 
                new CommentRssWriter(new StringWriter(), null, new Entry(PostType.BlogPost),
                                 new Mock<ISubtextContext>().Object)
            );
        }

        [Test]
        public void Ctor_WithNullEntry_ThrowsArgumentNullException()
        {
            UnitTestHelper.AssertThrowsArgumentNullException(() => 
                new CommentRssWriter(new StringWriter(), new List<FeedbackItem>(), null, new Mock<ISubtextContext>().Object)
            );
        }
    }
}