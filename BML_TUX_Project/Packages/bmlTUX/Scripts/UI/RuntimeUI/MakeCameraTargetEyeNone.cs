using UnityEngine;

namespace bmlTUX.Scripts.UI.EditorUI {
    [RequireComponent(typeof(Camera))]
    public class MakeCameraTargetEyeNone : MonoBehaviour {

        Camera connectedCamera;
    
        void OnValidate() {
            SetTargetEyeNone();
        }

        void OnEnable() {
            SetTargetEyeNone();
        }

        void SetTargetEyeNone() {
            if (connectedCamera == null) connectedCamera = GetComponent<Camera>();
            connectedCamera.stereoTargetEye = StereoTargetEyeMask.None;
        }
    }
    
}
