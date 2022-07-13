using UnityEngine;

namespace bmlTUX.Recorder
{
    [RequireComponent(typeof(Ghost))]
    public class TransformGhost : GhostComponent<TransformSnapshot> {
    
        protected override void ApplySnapshot(TransformSnapshot transformSnapshot) {
            var cachedTransform = transform;
            cachedTransform.position = transformSnapshot.Position;
            cachedTransform.rotation = transformSnapshot.Rotation;
            cachedTransform.localScale = transformSnapshot.Scale;
        }
    }
}