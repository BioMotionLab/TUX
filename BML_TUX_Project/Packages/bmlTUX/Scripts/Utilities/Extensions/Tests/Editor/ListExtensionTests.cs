using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace bmlTUX.Scripts.Utilities.Extensions.Tests.Editor {
    public class ListExtensionTests {
        readonly List<int> testTooShortList = new List<int>() {1};
        readonly List<int> testListTwo      = new List<int>() {1, 2};

        [Test]
        public void ThrowsOutOfRangeIfListLessThan2Long() {
            Assert.Throws<ArgumentOutOfRangeException>(() => testTooShortList.RandomOtherItem(1));
        }

        [Test]
        public void ReturnsOtherItemIfOnly2Long() {
            int result = testListTwo.RandomOtherItem(1);
            Assert.AreEqual(2, result);
        }
    }
}