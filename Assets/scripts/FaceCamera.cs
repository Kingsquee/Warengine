using UnityEngine;
using System.Collections;

//TODO: This could be better done with a shader, or only updating when camera has rotated.
public class FaceCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.forward = -Camera.main.transform.forward;
        //transform.LookAt( Camera.main.transform , Vector3.up );
        //transform.LookAt( Camera.main.transform, Vector3.forward);
	}
}
