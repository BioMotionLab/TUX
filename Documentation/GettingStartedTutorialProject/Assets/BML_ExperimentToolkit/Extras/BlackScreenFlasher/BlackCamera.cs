﻿using System.Collections;
using System.Collections.Generic;
using BML_Utilities.ScriptableObject_Assets;
using UnityEngine;

namespace BML_ExperimentToolkit.Extras.BlackScreenFlasher {
    public class BlackCamera : MonoBehaviour {
        Camera              thisCamera;
        public List<Camera> OtherCameras = new List<Camera>();

        public FloatValue BlackScreenShowTime;

        void Start() {
            thisCamera = GetComponent<Camera>();
            thisCamera.enabled = false;
        }

        public void ShowBlankScreen() {
            StartCoroutine(ShowThisCamera(BlackScreenShowTime));
        }

        void ReturnToPreviousCameras(List<Camera> activeCameras) {
            foreach (Camera cameraToActivate in activeCameras) {
                cameraToActivate.enabled = true;
            }
            thisCamera.enabled = false;
        }

        IEnumerator ShowThisCamera(float time) {
            List<Camera> previouslyActiveCameras = GetPreviouslyActiveCameras();
            thisCamera.enabled = true;
            DeactivateAllOtherCameras();
            yield return new WaitForSeconds(time);
            ReturnToPreviousCameras(previouslyActiveCameras);
        }

        void DeactivateAllOtherCameras() {
            foreach (Camera otherCamera in OtherCameras) {
                otherCamera.enabled = false;
            }
        }

        List<Camera> GetPreviouslyActiveCameras() {
            List<Camera> activeCameras = new List<Camera>();
            foreach (Camera otherCamera in OtherCameras) {
                if (otherCamera.enabled) {
                    activeCameras.Add(otherCamera);
                }
            }

            return activeCameras;
        }
    }
}
