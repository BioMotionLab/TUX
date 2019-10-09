using UnityEngine;

namespace BML_Utilities {
    /// <summary>
    /// Since unity doesn't flag the Quaternion as serializable, we
    /// need to create our own version. This one will automatically convert
    /// between Quaternion and QuaternionSerializable
    /// </summary>
    [System.Serializable]
    public struct QuaternionSerializable {
        
        public float X;
        public float Y;
        public float Z;
        public float W;
        
        public QuaternionSerializable(float x, 
                                      float y, 
                                      float z, 
                                      float w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public QuaternionSerializable(Quaternion quaternion)
            : this(quaternion.x,
                   quaternion.y,
                   quaternion.z,
                   quaternion.w) {
        }

        public override string ToString() {
            return $"{X}, {Y}, {Z}, {W}";
        }
        
        public static implicit operator Quaternion(QuaternionSerializable value) {
            return new Quaternion(value.X, value.Y, value.Z, value.W);
        }
        
        public static implicit operator QuaternionSerializable(Quaternion value) {
            return new QuaternionSerializable(value.x, value.y, value.z, value.w);
        }
    }
}