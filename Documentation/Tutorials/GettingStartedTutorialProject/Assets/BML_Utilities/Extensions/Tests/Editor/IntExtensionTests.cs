using NUnit.Framework;

namespace BML_Utilities.Extensions.Tests.Editor {
    public class IntExtensionTests {
        [Test]
        public void FourIsWithin3And7() {
            Assert.True(4.IsWithin(3, 7));
        }

        [Test]
        public void FiveIsNotWithin2And4() {
            Assert.False(5.IsWithin(2, 4));
        }

        [Test]
        public void Minus3WithinMinus5AndMinus1() {
            int minusFive = -5;
            Assert.True(minusFive.IsWithin(-5, -1));
        }
    }
}