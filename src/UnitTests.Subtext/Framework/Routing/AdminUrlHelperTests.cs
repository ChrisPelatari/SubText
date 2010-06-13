using System.Web;
using System.Web.Routing;
using MbUnit.Framework;
using Moq;
using Ninject;
using Subtext.Framework.Components;
using Subtext.Framework.Routing;
using Subtext.Infrastructure;

namespace UnitTests.Subtext.Framework.Routing
{
    [TestFixture]
    public class AdminUrlHelperTests
    {
        [Test]
        public void PostsList_WithBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.PostsList();

            //assert
            Assert.AreEqual("/sub/admin/posts/default.aspx", url);
        }

        [Test]
        public void PostsEdit_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.PostsEdit();

            //assert
            Assert.AreEqual("/sub/admin/posts/edit.aspx", url);
        }

        [Test]
        public void ArticlesList_WithBlogHavingSubfolder_RendersUrlToArticlesListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.ArticlesList();

            //assert
            Assert.AreEqual("/sub/admin/articles/default.aspx", url);
        }

        [Test]
        public void ArticlesEdit_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToArticlesListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.ArticlesEdit();

            //assert
            Assert.AreEqual("/sub/admin/articles/edit.aspx", url);
        }

        [Test]
        public void FeedbackList_WithBlogHavingSubfolder_RendersUrlToFeedbackListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.FeedbackList();

            //assert
            Assert.AreEqual("/sub/admin/feedback/default.aspx", url);
        }

        [Test]
        public void FeedbackEdit_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToFeedbackListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.FeedbackEdit(123);

            //assert
            Assert.AreEqual("/sub/admin/feedback/edit.aspx?return-to-post=true&FeedbackID=123", url);
        }

        [Test]
        public void LinksEdit_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.LinksEdit();

            //assert
            Assert.AreEqual("/sub/admin/EditLinks.aspx", url);
        }

        [Test]
        public void GalleriesEdit_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.GalleriesEdit();

            //assert
            Assert.AreEqual("/sub/admin/EditGalleries.aspx", url);
        }

        [Test]
        public void EditCategories_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.EditCategories(CategoryType.PostCollection);

            //assert
            Assert.AreEqual("/sub/admin/EditCategories.aspx?catType=PostCollection", url);
        }

        [Test]
        public void ErrorLog_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.ErrorLog();

            //assert
            Assert.AreEqual("/sub/admin/ErrorLog.aspx", url);
        }

        [Test]
        public void Home_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.Home();

            //assert
            Assert.AreEqual("/sub/admin/default.aspx", url);
        }

        [Test]
        public void ImportExport_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.ImportExport();

            //assert
            Assert.AreEqual("/sub/admin/ImportExport.aspx", url);
        }

        [Test]
        public void ExportUrl_WithEmbedFalseAndSubFolder_RendersUrlWithQueryStringParameter()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.Export(false);

            //assert
            Assert.AreEqual("/sub/admin/export.ashx?embed=False", url);
        }

        [Test]
        public void ExportUrl_WithEmbedTrue_RendersUrlWithQueryStringParameter()
        {
            //arrange
            AdminUrlHelper helper = SetupUrlHelper("/");

            //act
            string url = helper.Export(true);

            //assert
            Assert.AreEqual("/admin/export.ashx?embed=True", url);
        }

        [Test]
        public void Statistics_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.Statistics();

            //assert
            Assert.AreEqual("/sub/admin/Statistics.aspx", url);
        }

        [Test]
        public void Options_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.Options();

            //assert
            Assert.AreEqual("/sub/admin/Options.aspx", url);
        }

        [Test]
        public void Credits_WithCategoryTypeAndBlogHavingSubfolder_RendersUrlToPostsListPage()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("subfolder", "sub");
            AdminUrlHelper helper = SetupUrlHelper("/", routeData);

            //act
            string url = helper.Credits();

            //assert
            Assert.AreEqual("/sub/admin/Credits.aspx", url);
        }

        private static AdminUrlHelper SetupUrlHelper(string appPath)
        {
            return SetupUrlHelper(appPath, new RouteData());
        }

        private static AdminUrlHelper SetupUrlHelper(string appPath, RouteData routeData)
        {
            var routes = new RouteCollection();
            var subtextRoutes = new SubtextRouteMapper(routes, new Mock<IServiceLocator>().Object);
            Routes.RegisterRoutes(subtextRoutes);
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.Request.ApplicationPath).Returns(appPath);
            httpContext.Setup(c => c.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);
            var requestContext = new RequestContext(httpContext.Object, routeData);
            var helper = new UrlHelper(requestContext, routes);
            return new AdminUrlHelper(helper);
        }
    }
}