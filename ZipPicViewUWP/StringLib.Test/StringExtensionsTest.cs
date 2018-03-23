using NUnit.Framework;
using System;


namespace ZipPicViewUWP.StringLib.Test
{
    [TestFixture]
    public class StringExtensionsTest
    {
        [Test]
        public void TestEllipsesShorter()
        {
            Assert.AreEqual("Hello", "Hello".Ellipses(10));
        }

        [Test]
        public void TestEllipsesLonger()
        {
            Assert.AreEqual("He … other", "Hello World, Long Time no see ... brother".Ellipses(10));
        }

        [Test]
        public void TestExtractFileNameNoDirectory()
        {
            Assert.AreEqual("Hello", "Hello".ExtractFilename());
        }

        [Test]
        public void TestExtractFileNameForwardSlash()
        {
            Assert.AreEqual("Hello", "/hell/Hello".ExtractFilename());
        }

        [Test]
        public void TestExtractFileNameBackSlash()
        {
            Assert.AreEqual("Hello", @"\hell\Hello".ExtractFilename());
        }
    }
}
