﻿using System;
using System.Collections.Specialized;
using System.Web;
using MbUnit.Framework;
using Moq;
using Subtext.Framework;
using Subtext.Framework.Services.Account;

namespace UnitTests.Subtext.Framework.Services.Account
{
    [TestFixture]
    public class AccountServiceTests
    {
        [Test]
        public void Logout_ClearsAuthCookie()
        {
            // arrange
            var responseCookies = new HttpCookieCollection();
            var context = new Mock<ISubtextContext>();
            context.Setup(c => c.HttpContext.Request.QueryString).Returns(new NameValueCollection());
            context.Setup(c => c.HttpContext.Response.Cookies).Returns(responseCookies);
            var service = new AccountService();

            // act
            try
            {
                service.Logout(context.Object);
            }
            catch // Exception thrown due to call to FormsAuthentication.SignOut();
            {
            }

            // assert
            Assert.AreEqual(1, responseCookies.Count);
            Assert.IsTrue(responseCookies[0].Expires < DateTime.Now);
        }
    }
}
