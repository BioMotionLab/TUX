using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to set a camera's aspect ratio
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraAspect : MonoBehaviour {

    [Tooltip("square = 1")]
    public float AspectRatio;

    private Camera thisCamera;

	void Start () {
	    thisCamera = this.GetComponent<Camera>();
	    thisCamera.aspect = AspectRatio;
	}

    
	

}
