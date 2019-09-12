using System;
using NUnit.Framework;

namespace BML_Utilities.Extensions.Tests.Editor {
    public class StringExtensionTests {
        const string TestString = "12345678AndManyMoreCharacters";

        [Test]
        public void TruncateTo8Has8() {
            string result = TestString.Truncate(8);
            Assert.True(result.Length == 8);
        }

        [Test]
        public void TruncateTo6IsCorrect() {
            string result = TestString.Truncate(6);
            Assert.AreEqual("123456", result);
        }

        [Test]
        public void TruncateNegativeThrowsException() {
            Assert.Throws<ArgumentException>(() => TestString.Truncate(-1));
        }

        [Test]
        public void TruncateWithElipsesIsCorrect() {
            string result = TestString.Truncate(6, ellipses: true);
            Assert.AreEqual("1234..", result);
        }
    }
}