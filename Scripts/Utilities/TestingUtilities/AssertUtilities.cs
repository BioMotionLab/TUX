using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

namespace bmlTUX.Scripts.Utilities.TestingUtilities {
    public static class AssertUtilities
    {
    
        [PublicAPI]
        public static void AreEqual(Vector2 expected, Vector2 actual, float delta = 0, string message = null)
        {
            if (delta <= 0)
                delta = 0.00001f;
 
            float distance = Vector2.Distance(expected, actual);
 
            if (string.IsNullOrEmpty(message))
                message = $"Expected: ({expected.x:F2}, {expected.y:F2})\n" +
                          $"But was: ({actual.x:F2}, {actual.y:F2})\n\n" +
                          $"Distance: {distance:F2} greater than {delta:F2}";
 
            Assert.That(distance, Is.LessThanOrEqualTo(delta), message);
        }

        [PublicAPI]
        public static void AreEqual(Vector3 expected, Vector3 actual, float delta = 0, string message = null)
        {
            if (delta <= 0)
                delta = 0.001f;
 
            float distance = Vector3.Distance(expected, actual);

            if (string.IsNullOrEmpty(message))
                message = $"Expected: ({expected.x:F2}, {expected.y:F2}, {expected.z:F2})\n" +
                          $"But was: ({actual.x:F2}, {actual.y:F2}, {actual.z:F2})\n\n" +
                          $"Distance: {distance:F2} greater than {delta:F6}";
            
            Assert.That(distance, Is.LessThanOrEqualTo(delta), message);

        }


    }
}