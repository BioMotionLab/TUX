using System;
using UnityEngine;

namespace BML_Utilities {
    
    /// <summary>
    /// Since unity doesn't flag the Vector3 as serializable, we
    /// need to create our own version. This one will automatically convert
    /// between Vector3 and SerializableVector3
    /// 
    /// from: https://answers.unity.com/questions/956047/serialize-quaternion-or-vector3.html 
    /// </summary>
    [Serializable]
    public struct Vector3Serializable
    {
        
        public float X;
        public float Y;
        public float Z;
        
        public Vector3Serializable(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3Serializable(Vector3 vector) : this(vector.x, vector.y, vector.z) {
        }

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }
     
        
        public static implicit operator Vector3(Vector3Serializable value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }
        
        public static implicit operator Vector3Serializable(Vector3 value)
        {
            return new Vector3Serializable(value.x, value.y, value.z);
        }
    }
}