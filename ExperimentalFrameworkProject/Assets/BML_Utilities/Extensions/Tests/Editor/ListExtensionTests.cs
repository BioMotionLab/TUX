using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BML_Utilities.Extensions.Tests.Editor {
    public class ListExtensionTests {

        public class RandomOtherItemMethod {
            List<int> testTooShortList = new List<int>() { 1};
            List<int> testListTwo = new List<int>() { 1 ,2};

            [Test]
            public void ThrowsOutOfRangeIfListLessThan2Long() {
               Assert.Throws<ArgumentOutOfRangeException>(()=>testTooShortList.RandomOtherItem(1));

            }

            [Test]
            public void ReturnsOtherItemIfOnly2Long() {
                var result = testListTwo.RandomOtherItem(1);
                Assert.AreEqual(2,result);
            }


        }
       

        
    }
}
