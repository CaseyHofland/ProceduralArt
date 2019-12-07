using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandalaCam : MonoBehaviour {
    public AudioPeer _audioPeer;
    public Vector3 _rotateAxis;
    public Vector3 _startPos, _endPos;
    public int band;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(_rotateAxis.x * _audioPeer._audioBand[band] * Time.deltaTime, _rotateAxis.y * _audioPeer._audioBand[band] * Time.deltaTime, _rotateAxis.z * _audioPeer._audioBand[band] * Time.deltaTime);

        this.transform.position = Vector3.Lerp(_startPos, _endPos, _audioPeer._AmplitudeBuffer);
    }
}
