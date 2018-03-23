using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZipPicViewUWP.StringLib.Test
{
    [TestFixture]
    class PathNameComparerTest
    {
        [Test]
        public void FileNameCompareNoNumber()
        {
            Assert.Positive(PathNameComparer.FileNameComparer.Invoke("Name", "Mame"));
            Assert.Negative(PathNameComparer.FileNameComparer.Invoke("Mame", "Name"));
        }

        [Test]
        public void FileNameCompareWithNumber()
        {
            Assert.Positive(PathNameComparer.FileNameComparer.Invoke("Name (10)", "Name (2)"));
            Assert.Negative(PathNameComparer.FileNameComparer.Invoke("Name (10)", "Name (200)"));
        }

        [Test]
        public void ExtractNumberNoNonDigit()
        {
            var iter = "500".GetEnumerator();
            iter.MoveNext();
            var (number, hasNext) = PathNameComparer.ExtractNumber(iter);

            Assert.IsFalse(hasNext);
            Assert.AreEqual(500, number);
        }

        [Test]
        public void ExtractNumberWithNonDigit()
        {
            var iter = "500)abcde".GetEnumerator();
            iter.MoveNext();

            var (number, hasNext) = PathNameComparer.ExtractNumber(iter);

            Assert.IsTrue(hasNext);
            Assert.AreEqual(500, number);
        }
    }
}
