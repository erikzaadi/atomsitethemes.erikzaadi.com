using System;

using AtomSite.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domain.Test
{
    [TestClass]
    public class ThreadInReplyToTest
    {
        [TestMethod]
        public void SimpleCreateTest()
        {
            ThreadInReplyTo threadInReplyTo = new ThreadInReplyTo();

            Assert.IsNotNull(threadInReplyTo);
        }

        [TestMethod]
        public void FullCreateTest()
        {
            ThreadInReplyTo threadInReplyTo = new ThreadInReplyTo()
              {
                  Base = new Uri("http://base.com"),
                  Href = new Uri("http://base.com"),
                  Lang = "EN",
                  Ref = new Uri("http://ref.com"),
                  Source = new Uri("http://source.com"),
                  Type = "foo"
              };

            Assert.IsNotNull(threadInReplyTo);
            Assert.IsNotNull(threadInReplyTo.Base);
            Assert.IsNotNull(threadInReplyTo.Href);
            Assert.IsNotNull(threadInReplyTo.Ref);
            Assert.IsNotNull(threadInReplyTo.Source);

            Assert.AreEqual("EN", threadInReplyTo.Lang);
            Assert.AreEqual("foo", threadInReplyTo.Type);

            Assert.AreEqual(new Uri("http://base.com"), threadInReplyTo.Base);
            Assert.AreEqual(new Uri("http://base.com"), threadInReplyTo.Href);
            Assert.AreEqual(new Uri("http://ref.com"), threadInReplyTo.Ref);
            Assert.AreEqual(new Uri("http://source.com"), threadInReplyTo.Source);

        }

    }
}
