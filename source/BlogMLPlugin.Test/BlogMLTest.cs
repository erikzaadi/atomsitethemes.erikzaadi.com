using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomSite.Plugins.BlogMLPlugin;
using AtomSite.WebCore;
using StructureMap;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Hosting;
using System.IO;
using BlogML.Xml;
using System.Xml;
using AtomSite.Domain;
using AtomSite.Utils;
using System.Web;
using System.Security.Principal;
using System.Collections.Specialized;
using System.Reflection;
using AtomSite.Repository;

namespace AtomSite.Plugins.BlogMLPlugin.Test
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class BlogMLTest
  {
    public BlogMLTest()
    {
    }

    private IContainer container;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext { get; set; }

    #region Additional test attributes
    
    // Use TestInitialize to run code before running each test 
    [TestInitialize()]
    public void MyTestInitialize()
    {
      string appPath = FileHelper.CombineTrackbacksInPath(
          Path.Combine(TestContext.TestDir, @"..\..\WebCore\"));
      container = new Container(new FileRegistry(appPath));

      var mockHttpContext = new Moq.Mock<HttpContextBase>();
      var mockRequest = new Moq.Mock<HttpRequestBase>();
      var fakeResponse = new FakeResponse();
      var fakeAppState = new FakeApplicationState();
      fakeAppState.Add("Container", container);
      mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
      mockHttpContext.Setup(x => x.Response).Returns(fakeResponse);
      mockHttpContext.Setup(x => x.Application).Returns(fakeAppState);
      mockRequest.Setup(x => x.ApplicationPath).Returns("/");
      var requestContext = new RequestContext(mockHttpContext.Object, new RouteData());
      container.Configure(a => a.For<UrlHelper>().Use(f => new UrlHelper(requestContext)));
      PluginEngine.LoadPlugins(container, RouteTable.Routes, ViewEngines.Engines, ModelBinders.Binders, appPath, TestContext.TestDeploymentDir);
    }
    
    // Use TestCleanup to run code after each test has run
    [TestCleanup()]
    public void MyTestCleanup()
    {
    }
    
    #endregion

    [TestMethod]
    [DeploymentItem("BlogML20Sample.xml")]
    public void TestBlogMLImportMerge()
    {
      System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(new User() { Ids = new string[] { "Admin" }, Name = "Admin" }, null);

      //assemble
      IAtomPubService atompub = container.GetInstance<IAtomPubService>();
      var appSvc = atompub.GetService();
      IBlogMLService svc = container.GetInstance<IBlogMLService>();
      BlogMLBlog blog = null;
      string path = Path.Combine(TestContext.TestDeploymentDir, "BlogML20Sample.xml");
      using (XmlReader r = new XmlTextReader(File.OpenRead(path)))
      {
        blog = BlogMLSerializer.Deserialize(r);
      }

      //act
      svc.Import(new Uri("tag:example.com,2009:blog"), new Uri("tag:example.com,2009:pages"),
        new Uri("tag:example.com,2009:media"), ImportMode.Merge, blog);

      //assert
      var coll = appSvc.GetCollection(new Uri("tag:example.com,2009:blog"));

      Assert.AreEqual(coll.Title.Text, "BlogML 2.0 Sample Blog");
      Assert.AreEqual(coll.Subtitle.Text, "This is a sample blog content for BlogML 2.0");

      //TODO: other assertions


    }


    [TestMethod]
    [DeploymentItem("BlogML20Sample.xml")]
    public void TestBlogMLImportNew()
    {

      //assemble
      IAppServiceRepository atompub = container.GetInstance<IAppServiceRepository>();
      var appSvc = atompub.GetService();
      IBlogMLService svc = container.GetInstance<IBlogMLService>();
      BlogMLBlog blog = null;
      string path = "BlogML20Sample.xml";
      using (XmlReader r = new XmlTextReader(File.OpenRead(path)))
      {
        blog = BlogMLSerializer.Deserialize(r);
      }

      //act
      svc.Import(new Uri("tag:test.com,2010:blog"), new Uri("tag:test.com,2010:pages"),
        new Uri("tag:test.com,2010:media"), ImportMode.New, blog);

      //assert
      var coll = appSvc.GetCollection(new Uri("tag:test.com,2010:blog"));

      Assert.AreEqual(coll.Title.Text, "BlogML 2.0 Sample Blog");
      Assert.AreEqual(coll.Subtitle.Text, "This is a sample blog content for BlogML 2.0");

      //TODO: other assertions
    }

    private class FakeResponse : HttpResponseBase
    {
      // Routing calls this to account for cookieless sessions
      // It's irrelevant for the test, so just return the path unmodified
      public override string ApplyAppPathModifier(string x) { return x; }
    }

    private class FakeApplicationState : System.Web.HttpApplicationStateBase
    {
      Dictionary<string, object> c = new Dictionary<string, object>();
      public FakeApplicationState()
      {

      }

      public override void Add(string name, object value)
      {
        c.Add(name, value);
      }

      public override object this[string name]
      {
        get
        {
           return c[name];
        }
        set
        {
          c[name] = value;
        }
      }

      public override object Get(string name)
      {
        return c[name];
      }
    }
  }
}

