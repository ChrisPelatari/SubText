using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Xml;
using MbUnit.Framework;
using Moq;
using Subtext.Extensibility;
using Subtext.Framework;
using Subtext.Framework.Components;
using Subtext.Framework.Configuration;
using Subtext.Framework.Providers;
using Subtext.Framework.Routing;
using Subtext.Framework.Syndication;
using Subtext.Framework.Web.HttpModules;
using UnitTests.Subtext.Framework.Util;

namespace UnitTests.Subtext.Framework.Syndication
{
    /// <summary>
    /// Tests of the RssHandler http handler class.
    /// </summary>
    [TestFixture]
    public class RssHandlerTests
    {
        /// <summary>
        /// Tests writing a simple RSS feed from some database entries.
        /// </summary>
        [Test]
        [RollBack]
        public void RssWriterProducesValidFeedFromDatabase()
        {
            string hostName = UnitTestHelper.GenerateUniqueHostname();
            Config.CreateBlog("Test", "username", "password", hostName, string.Empty);

            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "");
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);

            Config.CurrentBlog.Email = "Subtext@example.com";
            Config.CurrentBlog.RFC3229DeltaEncodingEnabled = false;

            Entry entry = UnitTestHelper.CreateEntryInstanceForSyndication("Author",
                                                                           "testtitle",
                                                                           "testbody",
                                                                           null,
                                                                           NullValue.NullDateTime);
            entry.DateCreated = DateTime.ParseExact("2008/01/23", "yyyy/MM/dd", CultureInfo.InvariantCulture);
            entry.DateSyndicated = entry.DateCreated;
            UnitTestHelper.Create(entry); //persist to db.

            string rssOutput = null;
            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            Mock<UrlHelper> urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/archive/2008/01/23/testtitle.aspx");

            XmlNodeList itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);
            Assert.AreEqual(1, itemNodes.Count, "expected one item nodes.");

            string urlFormat = "http://{0}/archive/2008/01/23/{1}.aspx";
            string expectedUrl = string.Format(urlFormat, hostName, "testtitle");

            Assert.AreEqual("testtitle", itemNodes[0].SelectSingleNode("title").InnerText,
                            "Not what we expected for the title.");
            Assert.AreEqual(expectedUrl, itemNodes[0].SelectSingleNode("link").InnerText,
                            "Not what we expected for the link.");
            Assert.AreEqual(expectedUrl, itemNodes[0].SelectSingleNode("guid").InnerText,
                            "Not what we expected for the link.");
            Assert.AreEqual(expectedUrl + "#feedback", itemNodes[0].SelectSingleNode("comments").InnerText,
                            "Not what we expected for the link.");
        }

        [Test]
        [RollBack]
        public void RssWriterProducesValidFeedWithEnclosureFromDatabase()
        {
            string hostName = UnitTestHelper.GenerateUniqueString() + ".com";
            Config.CreateBlog("Test", "username", "password", hostName, string.Empty);

            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "");
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);
            Config.CurrentBlog.Email = "Subtext@example.com";
            Config.CurrentBlog.RFC3229DeltaEncodingEnabled = false;

            Entry entry = UnitTestHelper.CreateEntryInstanceForSyndication("Author", "testtitle", "testbody", null,
                                                                           NullValue.NullDateTime);
            entry.DateCreated = DateTime.ParseExact("2008/01/23", "yyyy/MM/dd", CultureInfo.InvariantCulture);
            entry.DateSyndicated = entry.DateCreated;
            int entryId = UnitTestHelper.Create(entry); //persist to db.

            string enclosureUrl = "http://perseus.franklins.net/hanselminutes_0107.mp3";
            string enclosureMimeType = "audio/mp3";
            long enclosureSize = 26707573;

            Enclosure enc =
                UnitTestHelper.BuildEnclosure("<Digital Photography Explained (for Geeks) with Aaron Hockley/>",
                                              enclosureUrl, enclosureMimeType, entryId, enclosureSize, true, true);
            Enclosures.Create(enc);

            var subtextContext = new Mock<ISubtextContext>();
            string rssOutput = null;
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());
            Mock<UrlHelper> urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/archive/2008/01/23/testtitle.aspx");

            XmlNodeList itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);
            Assert.AreEqual(1, itemNodes.Count, "expected one item nodes.");

            string urlFormat = "http://{0}/archive/2008/01/23/{1}.aspx";
            string expectedUrl = string.Format(urlFormat, hostName, "testtitle");

            Assert.AreEqual("testtitle", itemNodes[0].SelectSingleNode("title").InnerText,
                            "Not what we expected for the title.");
            Assert.AreEqual(expectedUrl, itemNodes[0].SelectSingleNode("link").InnerText,
                            "Not what we expected for the link.");
            Assert.AreEqual(expectedUrl, itemNodes[0].SelectSingleNode("guid").InnerText,
                            "Not what we expected for the guid.");
            Assert.AreEqual(enclosureUrl, itemNodes[0].SelectSingleNode("enclosure/@url").InnerText,
                            "Not what we expected for the enclosure url.");
            Assert.AreEqual(enclosureMimeType, itemNodes[0].SelectSingleNode("enclosure/@type").InnerText,
                            "Not what we expected for the enclosure mimetype.");
            Assert.AreEqual(enclosureSize.ToString(), itemNodes[0].SelectSingleNode("enclosure/@length").InnerText,
                            "Not what we expected for the enclosure size.");
            Assert.AreEqual(expectedUrl + "#feedback", itemNodes[0].SelectSingleNode("comments").InnerText,
                            "Not what we expected for the link.");
        }

        /// <summary>
        /// Tests that a simple regular RSS feed works.
        /// </summary>
        [Test]
        [RollBack]
        public void RssHandlerProducesValidRssFeed()
        {
            string hostName = UnitTestHelper.GenerateUniqueHostname();
            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "");
            Config.CreateBlog("", "username", "password", hostName, string.Empty);
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);

            UnitTestHelper.Create(UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test",
                                                                                   "Body Rocking"));
            Thread.Sleep(50);
            UnitTestHelper.Create(UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test 2",
                                                                                   "Body Rocking Pt 2"));

            var subtextContext = new Mock<ISubtextContext>();
            string rssOutput = null;
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());
            Mock<UrlHelper> urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/whatever");

            XmlNodeList itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);
            Assert.AreEqual(2, itemNodes.Count, "expected two item nodes.");

            Assert.AreEqual("Title Test 2", itemNodes[0].SelectSingleNode("title").InnerText,
                            "Not what we expected for the second title.");
            Assert.AreEqual("Title Test", itemNodes[1].SelectSingleNode("title").InnerText,
                            "Not what we expected for the first title.");

            Assert.AreEqual("Body Rocking Pt 2",
                            itemNodes[0].SelectSingleNode("description").InnerText.Substring(0,
                                                                                             "Body Rocking pt 2".Length),
                            "Not what we expected for the second body.");
            Assert.AreEqual("Body Rocking",
                            itemNodes[1].SelectSingleNode("description").InnerText.Substring(0, "Body Rocking".Length),
                            "Not what we expected for the first body.");
        }

        /// <summary>
        /// Tests that items without a date syndicated are not syndicated.
        /// </summary>
        [Test]
        [RollBack]
        public void RssHandlerHandlesDateSyndicatedProperly()
        {
            // arrange
            string hostName = UnitTestHelper.GenerateUniqueHostname();
            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "");
            Config.CreateBlog("", "username", "password", hostName, string.Empty);
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);

            //Create two entries, but only include one in main syndication.
            Entry entryForSyndication = UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test",
                                                                                         "Body Rocking");
            UnitTestHelper.Create(entryForSyndication);
            Entry entryTwoForSyndication = UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test 2",
                                                                                            "Body Rocking Pt 2");
            int id = UnitTestHelper.Create(entryTwoForSyndication);
            Entry entry = UnitTestHelper.GetEntry(id, PostConfig.None, false);
            DateTime date = entry.DateSyndicated;
            entry.IncludeInMainSyndication = false;
            entry.Blog = new Blog() { Title = "MyTestBlog" };
            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Blog).Returns(Config.CurrentBlog);
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());
            UnitTestHelper.Update(entry, subtextContext.Object);
            Assert.AreEqual(date, entry.DateSyndicated);

            string rssOutput = null;
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            Mock<UrlHelper> urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/whatever");

            XmlNodeList itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);
            Assert.AreEqual(1, itemNodes.Count, "expected one item node.");

            Assert.AreEqual("Title Test", itemNodes[0].SelectSingleNode("title").InnerText,
                            "Not what we expected for the first title.");
            Assert.AreEqual("Body Rocking",
                            itemNodes[0].SelectSingleNode("description").InnerText.Substring(0, "Body Rocking".Length),
                            "Not what we expected for the first body.");

            //Include the second entry back in the syndication.
            entry.IncludeInMainSyndication = true;
            UnitTestHelper.Update(entry, subtextContext.Object);

            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "", "");
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);
            subtextContext = new Mock<ISubtextContext>();
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());
            urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/whatever");

            itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);
            Assert.AreEqual(2, itemNodes.Count, "Expected two items in the feed now.");
        }

        /// <summary>
        /// Tests that the RssHandler orders items by DateSyndicated.
        /// </summary>
        [Test]
        [RollBack]
        public void RssHandlerSortsByDateSyndicated()
        {
            // Setup
            string hostName = UnitTestHelper.GenerateUniqueHostname();
            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "");
            Config.CreateBlog("", "username", "password", hostName, string.Empty);
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);

            //Create two entries.
            int firstId =
                UnitTestHelper.Create(UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test",
                                                                                       "Body Rocking"));
            Thread.Sleep(1000);
            UnitTestHelper.Create(UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test 2",
                                                                                   "Body Rocking Pt 2"));

            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());
            string rssOutput = null;
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());
            Mock<UrlHelper> urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/whatever");

            XmlNodeList itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);

            //Expect the first item to be the second entry.
            Assert.AreEqual("Title Test 2", itemNodes[0].SelectSingleNode("title").InnerText,
                            "Not what we expected for the first title.");
            Assert.AreEqual("Title Test", itemNodes[1].SelectSingleNode("title").InnerText,
                            "Not what we expected for the second title.");

            //Remove first entry from syndication.
            Entry firstEntry = UnitTestHelper.GetEntry(firstId, PostConfig.None, false);
            firstEntry.IncludeInMainSyndication = false;
            firstEntry.Blog = new Blog() { Title = "MyTestBlog" };
            UnitTestHelper.Update(firstEntry, subtextContext.Object);

            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, string.Empty);
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);
            subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());

            rssOutput = null;
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/whatever");

            itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);
            Assert.AreEqual(1, itemNodes.Count, "Here we were expeting only one item");

            Thread.Sleep(10);
            //Now add it back in changing the DateSyndicated
            firstEntry.IncludeInMainSyndication = true;
            firstEntry.DateSyndicated = Config.CurrentBlog.TimeZone.Now;
            UnitTestHelper.Update(firstEntry, subtextContext.Object);

            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "");
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);
            subtextContext = new Mock<ISubtextContext>();
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());

            rssOutput = null;
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/whatever");

            itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);

            //Expect the second item to be the second entry.
            Assert.AreEqual(2, itemNodes.Count, "Here we were expeting 2 items");
            Assert.AreEqual("Title Test", itemNodes[0].SelectSingleNode("title").InnerText,
                            "Not what we expected for the first title.");
            Assert.AreEqual("Title Test 2", itemNodes[1].SelectSingleNode("title").InnerText,
                            "Not what we expected for the second title.");
        }

        [Test]
        [RollBack]
        public void RssHandlerHandlesDoesNotSyndicateFuturePosts()
        {
            // Setup
            string hostName = UnitTestHelper.GenerateUniqueHostname();
            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "");
            Config.CreateBlog("", "username", "password", hostName, string.Empty);
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);
            Config.CurrentBlog.TimeZoneId = TimeZonesTest.HawaiiTimeZoneId;

            //Create two entries, but only include one in main syndication.
            UnitTestHelper.Create(UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test",
                                                                                   "Body Rocking", null,
                                                                                   NullValue.NullDateTime));
            Entry futureEntry = UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test 2",
                                                                                 "Body Rocking Pt 2", null,
                                                                                 NullValue.NullDateTime);
            futureEntry.DateSyndicated = Config.CurrentBlog.TimeZone.Now.AddMinutes(20);
            UnitTestHelper.Create(futureEntry);

            string rssOutput = null;
            var subtextContext = new Mock<ISubtextContext>();
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());
            Mock<UrlHelper> urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/whatever");

            XmlNodeList itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);
            Assert.AreEqual(1, itemNodes.Count, "expected one item node.");

            Assert.AreEqual("Title Test", itemNodes[0].SelectSingleNode("title").InnerText,
                            "Not what we expected for the first title.");
            Assert.AreEqual("Body Rocking",
                            itemNodes[0].SelectSingleNode("description").InnerText.Substring(0, "Body Rocking".Length),
                            "Not what we expected for the first body.");

            Config.CurrentBlog.TimeZoneId = TimeZonesTest.PacificTimeZoneId;

            UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "");
            BlogRequest.Current.Blog = Config.GetBlog(hostName, string.Empty);
            subtextContext = new Mock<ISubtextContext>();
            rssOutput = null;
            subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            subtextContext.Setup(c => c.Repository).Returns(ObjectProvider.Instance());
            urlHelper = Mock.Get(subtextContext.Object.UrlHelper);
            urlHelper.Setup(u => u.BlogUrl()).Returns("/");
            urlHelper.Setup(u => u.EntryUrl(It.IsAny<Entry>())).Returns("/whatever");

            itemNodes = GetRssHandlerItemNodes(subtextContext.Object, ref rssOutput);
            Assert.AreEqual(2, itemNodes.Count, "Expected two items in the feed now.");
        }

        private static XmlNodeList GetRssHandlerItemNodes(ISubtextContext context, ref string rssOutput)
        {
            var handler = new RssHandler(context);
            handler.ProcessRequest();
            var doc = new XmlDocument();
            doc.LoadXml(rssOutput);
            return doc.SelectNodes("/rss/channel/item");
        }

        private static XmlNodeList GetRssHandlerItemNodes(ISubtextContext context, StringBuilder sb)
        {
            string output = sb.ToString();
            return GetRssHandlerItemNodes(context, ref output);
        }

        /// <summary>
        /// Tests that sending a Gzip compressed RSS Feed sends the feed 
        /// properly compressed.  USed the RSS Bandit decompress code 
        /// to decompress the feed and test it.
        /// </summary>
        [Test]
        [RollBack]
        [Ignore("Need to review")]
        public void TestCompressedFeedWorks()
        {
            //string hostName = UnitTestHelper.GenerateUniqueHostname();
            //StringBuilder sb = new StringBuilder();
            //TextWriter output = new StringWriter(sb);

            //SimulatedHttpRequest workerRequest = UnitTestHelper.SetHttpContextWithBlogRequest(hostName, "", "", "", output);
            //workerRequest.Headers.Add("Accept-Encoding", "gzip");
            //Config.CreateBlog("", "username", "password", hostName, string.Empty);
            //Config.CurrentBlog.UseSyndicationCompression = true;

            //UnitTestHelper.Create(UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test", "Body Rocking"));
            //Thread.Sleep(50);
            //UnitTestHelper.Create(UnitTestHelper.CreateEntryInstanceForSyndication("Haacked", "Title Test 2", "Body Rocking Pt 2"));

            //RssHandler handler = new RssHandler();
            //Assert.IsNotNull(HttpContext.Current.Request.Headers, "Headers collection is null! Not Good.");
            //var subtextContext = new Mock<ISubtextContext>();
            //string rssOutput = null;
            //subtextContext.FakeSyndicationContext(Config.CurrentBlog, "/", s => rssOutput = s);
            //handler.ProcessRequest(subtextContext.Object);

            ////I'm cheating here!
            //MethodInfo method = typeof(HttpResponse).GetMethod("FilterOutput", BindingFlags.NonPublic | BindingFlags.Instance);
            //method.Invoke(HttpContext.Current.Response, new object[] {});

            //MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(sb.ToString()));
            //Stream deflated = UnitTestHelper.GetDeflatedResponse("gzip", stream);
            //using(StreamReader reader = new StreamReader(deflated))
            //{
            //    rssOutput = reader.ReadToEnd();
            //}

            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(rssOutput);

            //XmlNodeList itemNodes = doc.SelectNodes("/rss/channel/item");
            //Assert.AreEqual(2, itemNodes.Count, "expected two item nodes.");

            //Assert.AreEqual("Title Test 2", itemNodes[0].SelectSingleNode("title").InnerText, "Not what we expected for the second title.");
            //Assert.AreEqual("Title Test", itemNodes[1].SelectSingleNode("title").InnerText, "Not what we expected for the first title.");

            //Assert.AreEqual("Body Rocking Pt 2", itemNodes[0].SelectSingleNode("description").InnerText.Substring(0, "Body Rocking pt 2".Length), "Not what we expected for the second body.");
            //Assert.AreEqual("Body Rocking", itemNodes[1].SelectSingleNode("description").InnerText.Substring(0, "Body Rocking".Length), "Not what we expected for the first body.");
        }
    }
}