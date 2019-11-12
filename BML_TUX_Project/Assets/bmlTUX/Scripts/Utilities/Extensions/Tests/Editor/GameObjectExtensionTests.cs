using NUnit.Framework;
using UnityEngine;

namespace BML_Utilities.Extensions.Tests.Editor {
    public class GameObjectExtensionTests
    {
        GameObject baseObject;
        GameObject aSubObject;
        GameObject aSubSubObject;

        [SetUp]
        public void SetUp() {
            baseObject = new GameObject();
            int n = 5;
            for (int i = 0; i < n; i++) {
                GameObject subObject = new GameObject();
                subObject.transform.SetParent(baseObject.transform);
                aSubObject = subObject;
                GameObject subSubObject = new GameObject();
                subSubObject.transform.SetParent(subObject.transform);
                aSubSubObject = subSubObject;
            }
        
        }

        [Test]
        public void SetLayerRecursivelySetsOwnLayer() {
            baseObject.SetLayerRecursively(5);
            Assert.AreEqual(5, baseObject.layer);
        }

        [Test]
        public void SetLayerRecursivelySetsChildLayer() {
            baseObject.SetLayerRecursively(6);
            Assert.AreEqual(6, aSubObject.layer);
        }
    
        [Test]
        public void SetLayerRecursivelySetsChildOfChildLayer() {
            baseObject.SetLayerRecursively(7);
            Assert.AreEqual(7, aSubSubObject.layer);
        }
    
    
    }
}
