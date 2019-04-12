using NUnit.Framework;
using UnityEngine;

namespace BML_Utilities.Extensions.Tests.Editor {

    public class TransformExtensionTests {

        public class CreateCopyMethod {

            private GameObject mockObject;


            [SetUp]
            public void Setup() {
                mockObject = new GameObject();
                Vector3 localPosition = new Vector3(1, 2, 3);
                Vector3 localEulers = new Vector3(4f, 5f, 6f);
                Vector3 localScale = new Vector3(7, 8, 9);
                mockObject.transform.localPosition = localPosition;
                mockObject.transform.localEulerAngles = localEulers;
                mockObject.transform.localScale = localScale;
            }


            [Test]
            public void CopiedLocalPosition() {
                GameObject newGameObject = new GameObject();
                newGameObject.transform.CopyLocalFrom(mockObject.transform);
                var expected = new Vector3(1, 2, 3);
                Assert.AreEqual(expected, newGameObject.transform.localPosition);
            }

            [Test]
            public void CopiedLocalEulers() {
                GameObject newGameObject = new GameObject();
                newGameObject.transform.CopyLocalFrom(mockObject.transform);
                var expected = new Vector3(4f, 5f, 6f);
                Assert.IsTrue(expected == newGameObject.transform.localEulerAngles);
            }

            [Test]
            public void CopiedLocalScale() {
                GameObject newGameObject = new GameObject();
                newGameObject.transform.CopyLocalFrom(mockObject.transform);
                var expected = new Vector3(7, 8, 9);
                Assert.AreEqual(expected, newGameObject.transform.localScale);
            }



        }
    }
}
