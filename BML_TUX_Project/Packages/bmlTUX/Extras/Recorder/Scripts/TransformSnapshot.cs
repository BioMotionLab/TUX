using System;
using UnityEngine;

namespace bmlTUX.Recorder
{
    public abstract class TransformSnapshotBase : Snapshot<Transform> {
        protected TransformSnapshotBase(float timeStamp, int frameStamp) : base(timeStamp, frameStamp) { }
        public abstract Vector3 Position { get; }
        public abstract Quaternion Rotation { get; }
        public abstract Vector3 Scale { get; }
    }

    [Serializable]
    public class TransformSnapshot : TransformSnapshotBase {
    
        [SerializeField] public float[] position;
        [SerializeField] public float[] rotation;
        [SerializeField] public float[] scale;
        [SerializeField] public int id;
        [SerializeField] string name;
    
    
        public override Vector3 Position => new Vector3(position[0], position[1], position[2]);
        public override Quaternion Rotation => new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
        public override Vector3 Scale => new Vector3(scale[0], scale[1], scale[2]);

    
        public TransformSnapshot(Transform transform, float timeStamp, int frameStamp) : base(timeStamp, frameStamp) {
        
            Vector3 cachedPos = transform.position;
            position = new[] { cachedPos.x, cachedPos.y, cachedPos.z };
        
            Quaternion cachedRot = transform.rotation;
            rotation = new[] { cachedRot.x, cachedRot.y, cachedRot.z, cachedRot.w };

            Vector3 cachedScale = transform.localScale;
            scale = new[] { cachedScale.x, cachedScale.y, cachedScale.z };

            this.id = transform.gameObject.GetInstanceID();
            this.name = transform.gameObject.name;
        }
    
    }

    [Serializable]
    public class TransformSnapshotOnlyPosition : TransformSnapshotBase {
    
        [SerializeField] public float[] position;
        [SerializeField] public int id;
        [SerializeField] string name;
        public override Vector3 Position => new Vector3(position[0], position[1], position[2]);
        public override Quaternion Rotation => Quaternion.identity;
        public override Vector3 Scale => Vector3.zero;
    
        public TransformSnapshotOnlyPosition(Transform transform, float timeStamp, int frameStamp) : base(timeStamp, frameStamp) {
    
            Vector3 cachedPos = transform.position;
            position = new[] { cachedPos.x, cachedPos.y, cachedPos.z };

            this.id = transform.gameObject.GetInstanceID();
            this.name = transform.gameObject.name;

        }
   
    }
}