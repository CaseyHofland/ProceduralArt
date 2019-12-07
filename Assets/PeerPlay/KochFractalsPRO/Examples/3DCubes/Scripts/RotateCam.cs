using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCam : MonoBehaviour {
    public AudioPeer _audioPeer;
    public Vector3 _rotateAxis, _rotateSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.GetChild(0).transform.LookAt(this.transform);

        this.transform.Rotate(_rotateAxis.x * _rotateSpeed.x * Time.deltaTime * _audioPeer._AmplitudeBuffer,
            _rotateAxis.y * _rotateSpeed.y * Time.deltaTime * _audioPeer._AmplitudeBuffer,
            _rotateAxis.z * _rotateSpeed.z * Time.deltaTime * _audioPeer._AmplitudeBuffer);

	}
}
